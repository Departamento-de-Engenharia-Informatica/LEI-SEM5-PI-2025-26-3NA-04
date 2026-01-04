import { Component, OnInit, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import { PortLayoutService, PortLayout } from '../../services/port-layout';

const SCENE_LIMIT = 200;
const MIN_CAM_Y = 5;
const MAX_CAM_Y = 300;

@Component({
  selector: 'app-port-scene',
  standalone: true,
  templateUrl: './port-scene.html',
  styleUrls: ['./port-scene.css']
})
export class PortSceneComponent implements OnInit, OnDestroy {
  @ViewChild('canvas', { static: true }) canvasRef!: ElementRef<HTMLCanvasElement>;

  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private renderer!: THREE.WebGLRenderer;
  private controls!: OrbitControls;
  private animationId: number = 0;
  private textureLoader!: THREE.TextureLoader;
  private keyState: { [key: string]: boolean } = {};
  private readonly movementSpeed = 5.0;
  private raycaster!: THREE.Raycaster;
  private mouse!: THREE.Vector2;
  private selectedObject: THREE.Object3D | null = null;
  private selectedObjectOriginalMaterials: (THREE.Material | THREE.Material[])[] = [];
  private objectLabels: Map<string, THREE.Sprite> = new Map();
  private currentLabel: THREE.Sprite | null = null;
  private isAnimatingCamera: boolean = false;
  private cameraAnimationStartPos!: THREE.Vector3;
  private cameraAnimationTargetPos!: THREE.Vector3;
  private cameraAnimationStartTarget!: THREE.Vector3;
  private cameraAnimationTargetTarget!: THREE.Vector3;
  private cameraAnimationStartTime: number = 0;
  private readonly CAMERA_ANIMATION_DURATION = 1500;
  private readonly OPTIMAL_VIEW_DISTANCE_MIN = 60;
  private readonly OPTIMAL_VIEW_DISTANCE_MAX = 150;

  constructor(private portLayoutService: PortLayoutService) { }

  ngOnInit(): void {
    this.initScene();
    this.loadPortLayout();
    this.animate();
  }

  ngOnDestroy(): void {
    cancelAnimationFrame(this.animationId);
    this.controls.dispose();
    this.renderer.dispose();
  }

  private initScene(): void {
    this.scene = new THREE.Scene();
    this.scene.background = new THREE.Color(0x87ceeb);

    const canvas = this.canvasRef.nativeElement;
    const width = canvas.clientWidth || window.innerWidth;
    const height = canvas.clientHeight || window.innerHeight;
    this.camera = new THREE.PerspectiveCamera(
      75,
      width / height,
      0.1,
      1000
    );
    this.camera.position.set(100, 100, 100);
    this.camera.lookAt(0, 0, 0);

    this.renderer = new THREE.WebGLRenderer({
      canvas: this.canvasRef.nativeElement,
      antialias: true
    });
    this.updateRendererSize();
    this.renderer.setPixelRatio(window.devicePixelRatio);

    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;

    this.controls = new OrbitControls(this.camera, this.renderer.domElement);
    this.controls.enableDamping = true;
    this.controls.dampingFactor = 0.05;

    this.controls.mouseButtons = { LEFT: THREE.MOUSE.ROTATE, MIDDLE: THREE.MOUSE.DOLLY, RIGHT: THREE.MOUSE.PAN };

    this.controls.minDistance = 50;
    this.controls.maxDistance = 500;

    this.controls.minPolarAngle = Math.PI * 0.1;
    this.controls.maxPolarAngle = Math.PI * 0.45;

    const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
    this.scene.add(ambientLight);

    const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
    directionalLight.position.set(50, 100, 50);
    this.scene.add(directionalLight);

    directionalLight.castShadow = true;
    
    directionalLight.shadow.camera.left = -200;
    directionalLight.shadow.camera.right = 200;
    directionalLight.shadow.camera.top = 200;
    directionalLight.shadow.camera.bottom = -200;
    directionalLight.shadow.camera.near = 0.1;
    directionalLight.shadow.camera.far = 300;

    const groundGeometry = new THREE.PlaneGeometry(500, 500);
    const groundMaterial = new THREE.MeshStandardMaterial({ color: 0x228b22 });
    const ground = new THREE.Mesh(groundGeometry, groundMaterial);
    ground.rotation.x = -Math.PI / 2;
    ground.position.y = -1;

    ground.receiveShadow = true;

    this.scene.add(ground);

    const gridHelper = new THREE.GridHelper(500, 50, 0x000000, 0x444444);
    gridHelper.position.y = -0.5;
    this.scene.add(gridHelper);

    window.addEventListener('resize', () => this.onWindowResize());

    this.textureLoader = new THREE.TextureLoader();

    this.raycaster = new THREE.Raycaster();
    this.mouse = new THREE.Vector2();

    this.setupKeyboardControls();
    this.setupMouseControls();
  }

  private setupKeyboardControls(): void {
    window.addEventListener('keydown', (event) => {
      this.keyState[event.key.toLowerCase()] = true;
    });

    window.addEventListener('keyup', (event) => {
      this.keyState[event.key.toLowerCase()] = false;
    });
  }

  private setupMouseControls(): void {
    this.renderer.domElement.addEventListener('click', (event) => {
      this.onMouseClick(event);
    });
  }

  private onMouseClick(event: MouseEvent): void {
    if (event.button !== 0) return;

    const rect = this.renderer.domElement.getBoundingClientRect();
    this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
    this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;

    this.raycaster.setFromCamera(this.mouse, this.camera);

    const selectableObjects = this.scene.children.filter(child => {
      return child instanceof THREE.Mesh && 
             child.name !== '' && 
             !child.name.includes('-door') && 
             !child.name.includes('-roof') &&
             child.name !== 'ground' &&
             !(child instanceof THREE.GridHelper);
    });

    const intersects = this.raycaster.intersectObjects(selectableObjects, true);

    if (intersects.length > 0) {
      let selectedMesh = intersects[0].object as THREE.Mesh;
      
      if (selectedMesh.name.includes('-boom')) {
        const craneId = selectedMesh.name.replace('-boom', '');
        const craneTower = this.scene.children.find(child => 
          child instanceof THREE.Mesh && child.name === craneId
        ) as THREE.Mesh;
        if (craneTower) {
          selectedMesh = craneTower;
        }
      }
      
      this.selectObject(selectedMesh);
    } else {
      this.deselectObject();
    }
  }

  private selectObject(object: THREE.Mesh): void {
    this.deselectObject();

    this.selectedObject = object;
    this.selectedObjectOriginalMaterials = [];

    if (Array.isArray(object.material)) {
      object.material.forEach(mat => {
        this.selectedObjectOriginalMaterials.push(mat.clone());
      });
      object.material.forEach(mat => {
        if (mat instanceof THREE.MeshStandardMaterial) {
          mat.emissive.setHex(0x444444);
        }
      });
    } else {
      this.selectedObjectOriginalMaterials.push(object.material.clone());
      if (object.material instanceof THREE.MeshStandardMaterial) {
        object.material.emissive.setHex(0x444444);
      }
    }

    this.showLabel(object.name);
    this.animateCameraToObject(object);
  }

  private deselectObject(): void {
    if (this.selectedObject && this.selectedObject instanceof THREE.Mesh) {
      if (Array.isArray(this.selectedObject.material)) {
        this.selectedObject.material.forEach((mat, index) => {
          const originalMat = this.selectedObjectOriginalMaterials[index];
          if (mat instanceof THREE.MeshStandardMaterial && originalMat instanceof THREE.MeshStandardMaterial) {
            mat.emissive.copy(originalMat.emissive);
          }
        });
      } else {
        const originalMat = this.selectedObjectOriginalMaterials[0];
        if (this.selectedObject.material instanceof THREE.MeshStandardMaterial && originalMat instanceof THREE.MeshStandardMaterial) {
          this.selectedObject.material.emissive.copy(originalMat.emissive);
        }
      }
    }

    this.hideLabel();
    this.selectedObject = null;
    this.selectedObjectOriginalMaterials = [];
  }

  private animateCameraToObject(object: THREE.Mesh): void {
    if (this.isAnimatingCamera) return;

    const worldPosition = new THREE.Vector3();
    object.getWorldPosition(worldPosition);

    const boundingBox = new THREE.Box3().setFromObject(object);
    const objectSize = boundingBox.getSize(new THREE.Vector3());
    const maxDimension = Math.max(objectSize.x, objectSize.y, objectSize.z);
    
    const optimalDistance = Math.max(
      this.OPTIMAL_VIEW_DISTANCE_MIN,
      Math.min(this.OPTIMAL_VIEW_DISTANCE_MAX, maxDimension * 2.5)
    );

    const currentCameraToObject = new THREE.Vector3();
    currentCameraToObject.subVectors(this.camera.position, worldPosition);
    const currentDistance = currentCameraToObject.length();

    const targetDistance = optimalDistance;
    const viewAngle = Math.PI / 4;
    const targetHeight = Math.max(MIN_CAM_Y, Math.min(MAX_CAM_Y, worldPosition.y + targetDistance * Math.sin(viewAngle)));

    const horizontalDistance = Math.sqrt(targetDistance * targetDistance - Math.pow(targetHeight - worldPosition.y, 2));
    
    const currentHorizontalDirection = new THREE.Vector2(currentCameraToObject.x, currentCameraToObject.z);
    if (currentHorizontalDirection.length() > 0.001) {
      currentHorizontalDirection.normalize();
    } else {
      currentHorizontalDirection.set(1, 0);
    }

    const targetCameraPos = new THREE.Vector3(
      worldPosition.x - currentHorizontalDirection.x * horizontalDistance,
      targetHeight,
      worldPosition.z - currentHorizontalDirection.y * horizontalDistance
    );

    this.cameraAnimationStartPos = this.camera.position.clone();
    this.cameraAnimationTargetPos = targetCameraPos;
    this.cameraAnimationStartTarget = this.controls.target.clone();
    this.cameraAnimationTargetTarget = worldPosition.clone();

    this.isAnimatingCamera = true;
    this.cameraAnimationStartTime = performance.now();
  }
  

  private loadPortLayout(): void {
    this.portLayoutService.getPortLayout().subscribe({
      next: (data: PortLayout) => {
        this.createPortObjects(data);
      },
      error: (error) => {
        console.error('Error loading port layout:', error);
      }
    });
  }

  private createPortObjects(layout: PortLayout): void {
    console.log(`Loading layout: ${layout.name}`);

    layout.docks.forEach(dock => {
      const geometry = new THREE.BoxGeometry(
        dock.dimensions.width,
        dock.dimensions.height,
        dock.dimensions.depth
      );
      const material = new THREE.MeshStandardMaterial({
        color: 0x8b4513,
        roughness: 0.7
      });
      const mesh = new THREE.Mesh(geometry, material);
      mesh.position.set(dock.position.x, dock.position.y, dock.position.z);
      mesh.name = dock.id;
      this.scene.add(mesh);
      
      const maxCraneHeight = dock.stsCranes.length > 0 
        ? Math.max(...dock.stsCranes.map(crane => crane.height))
        : 0;
      const labelHeight = Math.max(dock.dimensions.height, maxCraneHeight);
      this.addLabel(dock.id, dock.position, labelHeight);

      mesh.castShadow = true;
      mesh.receiveShadow = true;

      dock.stsCranes.forEach(crane => {
        this.createSTSCrane(crane);
      });
    });

    layout.containerYards.forEach(yard => {
        const geometry = new THREE.BoxGeometry(
            yard.dimensions.width,
            yard.dimensions.height, // A altura é geralmente pequena para pátios
            yard.dimensions.depth
        );

        const texturePath = `textures/black-road-texture.jpg`;
        
        // Fator de repetição
        const repeatFactor = 5; 
        
        // Repetição X (Largura do Pátio) e Z (Profundidade do Pátio)
        const repeatX = yard.dimensions.width / repeatFactor;
        const repeatZ = yard.dimensions.depth / repeatFactor;
        
        this.textureLoader.load(texturePath, 
            
            // Callback de Sucesso: Executado quando a textura é carregada
            (yardTexture) => {
                
                yardTexture.wrapS = THREE.RepeatWrapping;
                yardTexture.wrapT = THREE.RepeatWrapping;
                
                // Aplicar a repetição no plano horizontal (X e Z)
                yardTexture.repeat.set(repeatX, repeatZ);
                yardTexture.needsUpdate = true;
                
                // Material com o mapa de textura, SEM cor base sólida
                const texturedMaterial = new THREE.MeshStandardMaterial({
                    map: yardTexture,
                    roughness: 0.8
                    // Nota: 'color' é omitido para que a cor da textura domine
                });
                
                // Material de cor sólida para as laterais (opcional, pois são muito baixas)
                const sideMaterial = new THREE.MeshStandardMaterial({
                    color: 0x555555,
                    roughness: 0.8
                });

                // Array de Materiais: (+X, -X, +Y, -Y, +Z, -Z)
                const materials = [
                    sideMaterial,       // Face Direita (+X)
                    sideMaterial,       // Face Esquerda (-X)
                    texturedMaterial,   // Face Superior (+Y) 👈 Textura principal aqui
                    sideMaterial,       // Face Inferior (-Y)
                    sideMaterial,       // Face Frontal (+Z)
                    sideMaterial        // Face Traseira (-Z)
                ];

                // CRIAÇÃO DO MESH COM O ARRAY DE MATERIAIS
                const mesh = new THREE.Mesh(geometry, materials);
                
                // Aplicação de Posicionamento e Sombras DENTRO do callback:
                mesh.position.set(yard.position.x, yard.position.y, yard.position.z);
                mesh.name = yard.id;
                mesh.castShadow = true;
                mesh.receiveShadow = true;

                this.scene.add(mesh);
                this.addLabel(yard.id, yard.position, yard.dimensions.height);

            }, 
            // Callback de Progresso (Opcional)
            undefined, 
            
            // Callback de Erro/Fallback: Se a textura falhar, usamos a cor sólida
            (error) => {
                console.error('Erro ao carregar a textura do pátio:', texturePath, error);
                
                // Material de Fallback (cor sólida original)
                const fallbackMaterial = new THREE.MeshStandardMaterial({ color: 0x808080, roughness: 0.8 });
                const fallbackMesh = new THREE.Mesh(geometry, fallbackMaterial);
                
                fallbackMesh.position.set(yard.position.x, yard.position.y, yard.position.z);
                fallbackMesh.name = yard.id;
                fallbackMesh.castShadow = true;
                fallbackMesh.receiveShadow = true;
                this.scene.add(fallbackMesh);
                this.addLabel(yard.id, yard.position);
            }
        ); 
    });

    layout.warehouses.forEach(warehouse => {
    const geometry = new THREE.BoxGeometry(
        warehouse.dimensions.width,
        warehouse.dimensions.height,
        warehouse.dimensions.depth
    );

    const texturePath = `textures/warehouse-texture.jpg`;
    const repeatFactor = 10; 
    
    const repeatX = warehouse.dimensions.width / repeatFactor;
    const repeatY = warehouse.dimensions.height / repeatFactor; 
    const repeatZ = warehouse.dimensions.depth / repeatFactor;
    
    this.textureLoader.load(texturePath, 
        
        // Callback de Sucesso (Wall Texture Loaded)
        (wallTextureOriginal) => {
            
            wallTextureOriginal.wrapS = THREE.RepeatWrapping;
            wallTextureOriginal.wrapT = THREE.RepeatWrapping;
            wallTextureOriginal.needsUpdate = true;

            const frontBackTexture = wallTextureOriginal.clone();
            frontBackTexture.repeat.set(repeatX, repeatY);
            frontBackTexture.needsUpdate = true;
            
            const frontBackMaterial = new THREE.MeshStandardMaterial({ map: frontBackTexture, roughness: 0.6 });

            const sideTexture = wallTextureOriginal.clone();
            sideTexture.repeat.set(repeatZ, repeatY);
            sideTexture.needsUpdate = true;
          
            const sideMaterial = new THREE.MeshStandardMaterial({ map: sideTexture, roughness: 0.6 });
            
            const topTexture = wallTextureOriginal.clone();
            topTexture.repeat.set(repeatX, repeatZ);
            topTexture.needsUpdate = true;
           
            const topMaterial = new THREE.MeshStandardMaterial({ map: topTexture, roughness: 0.6 });

            const materials = [
                sideMaterial, sideMaterial, 
                topMaterial,  
                new THREE.MeshStandardMaterial({ color: 0x333333, roughness: 0.9 }), // Base
                frontBackMaterial, frontBackMaterial  
            ];

         
            const mesh = new THREE.Mesh(geometry, materials); 
            
            mesh.position.set(warehouse.position.x, warehouse.position.y, warehouse.position.z);
            mesh.name = warehouse.id;
            mesh.castShadow = true;
            mesh.receiveShadow = true;
            this.scene.add(mesh);
            this.addLabel(warehouse.id, warehouse.position, warehouse.dimensions.height);
            
           const doorWidth = 5;
            const doorHeight = 8;
            const doorDepth = 0.5;
            
            const doorGeometry = new THREE.BoxGeometry(doorWidth, doorHeight, doorDepth);
            const doorMaterial = new THREE.MeshStandardMaterial({
                color: 0x222222, 
                roughness: 0.8
            });
            const doorMesh = new THREE.Mesh(doorGeometry, doorMaterial);
            
           /*doorMesh.position.set(
                warehouse.position.x, 
                warehouse.position.y + 2,
                warehouse.position.z + (warehouse.dimensions.depth / 2) - (doorDepth / 2) - 0.2
            );*/
            doorMesh.position.set(
                    warehouse.position.x, 
                    warehouse.position.y + (doorHeight / 2) - 0.93,
                    warehouse.position.z + (warehouse.dimensions.depth / 2) - 0.1
                );
            doorMesh.name = `${warehouse.id}-door`;

            doorMesh.castShadow = true;
            doorMesh.receiveShadow = true;
            this.scene.add(doorMesh);
            
            const roofHeight = 1; 
            
            const roofGeometry = new THREE.BoxGeometry(
                warehouse.dimensions.width + 2, 
                roofHeight,
                warehouse.dimensions.depth + 2
            );
            
            const roofTexturePath = `textures/roof-texture.jpg`; 

            this.textureLoader.load(roofTexturePath, 
                (roofTexture) => {
                    roofTexture.wrapS = THREE.RepeatWrapping;
                    roofTexture.wrapT = THREE.RepeatWrapping;
                    roofTexture.repeat.set(
                        (warehouse.dimensions.width + 2) / 8, 
                        (warehouse.dimensions.depth + 2) / 8
                    );
                    roofTexture.needsUpdate = true;

                    const roofMaterial = new THREE.MeshStandardMaterial({
                        map: roofTexture,
                        roughness: 0.9,
                    });
                    
                    const roofMesh = new THREE.Mesh(roofGeometry, roofMaterial);
                    
                    roofMesh.position.set(
                        warehouse.position.x,
                        warehouse.position.y + warehouse.dimensions.height / 2 + roofHeight / 2, 
                        warehouse.position.z
                    );
                    roofMesh.name = `${warehouse.id}-roof`;
                    roofMesh.castShadow = true;
                    roofMesh.receiveShadow = true;
                    this.scene.add(roofMesh);
                },
                undefined,
                (error) => {
                     // Fallback do telhado: Cor sólida escura
                    console.error('Erro ao carregar a textura do telhado:', roofTexturePath, error);
                    const fallbackRoofMaterial = new THREE.MeshStandardMaterial({ color: 0x444444 });
                    const fallbackRoofMesh = new THREE.Mesh(roofGeometry, fallbackRoofMaterial);
                    fallbackRoofMesh.position.set(
                        warehouse.position.x,
                        warehouse.position.y + warehouse.dimensions.height / 2 + roofHeight / 2,
                        warehouse.position.z
                    );
                    this.scene.add(fallbackRoofMesh);
                }
            ); 
        }, 
        // Callback de Erro/Fallback da Parede: Executado se a imagem da parede falhar
        (error) => {
            console.error('Erro ao carregar a textura do armazém:', texturePath, error);
            
            const fallbackMaterial = new THREE.MeshStandardMaterial({ color: 0xb22222, roughness: 0.6 });
            const fallbackMesh = new THREE.Mesh(geometry, fallbackMaterial);
            
            fallbackMesh.position.set(warehouse.position.x, warehouse.position.y, warehouse.position.z);
            fallbackMesh.name = warehouse.id;
            fallbackMesh.castShadow = true;
            fallbackMesh.receiveShadow = true;
            this.scene.add(fallbackMesh);
            this.addLabel(warehouse.id, warehouse.position);
        }
      ); 
    });
  }


private createSTSCrane(crane: any): void {
    const baseWidth = 6;
    const baseDepth = 6;
    
    const texturePath = `textures/stscranes-texture.jpg`; 
    
    const towerHeight = crane.height;
    const boomLength = 40; 
    const boomHeight = 2;
    
    const towerGeometry = new THREE.BoxGeometry(baseWidth, towerHeight, baseDepth);
    const boomGeometry = new THREE.BoxGeometry(boomLength, boomHeight, 3);
    
    this.textureLoader.load(texturePath, 
        
        (craneTextureOriginal) => {
            
            craneTextureOriginal.wrapS = THREE.RepeatWrapping;
            craneTextureOriginal.wrapT = THREE.RepeatWrapping;
            
            const repeatFactor = 5; 
            
            // Para a torre, as faces laterais (X-Y e Z-Y) usam repetição baseada na altura
            const towerRepeatX = baseWidth / repeatFactor;
            const towerRepeatY = towerHeight / repeatFactor; 
            const towerRepeatZ = baseDepth / repeatFactor;
            
            const sideTexture = craneTextureOriginal.clone();
            sideTexture.repeat.set(towerRepeatX, towerRepeatY); 
            sideTexture.needsUpdate = true;
            const sideMaterial = new THREE.MeshStandardMaterial({ map: sideTexture, color: 0xffa500, metalness: 0.3 });

            const topTexture = craneTextureOriginal.clone();
            topTexture.repeat.set(towerRepeatX, towerRepeatZ);
            topTexture.needsUpdate = true;
            const topMaterial = new THREE.MeshStandardMaterial({ map: topTexture, color: 0xffa500, metalness: 0.3 });
            
            
            const towerMaterials = [
                sideMaterial, sideMaterial, // Lado esquerdo e lado direito
                topMaterial,                // Topo 
                new THREE.MeshStandardMaterial({ color: 0x555555 }), // Base com cor sólida
                sideMaterial, sideMaterial  // Frente e Trás
            ];
            
            // A garra precisa de repetição ao longo do seu comprimento (X)
            const boomRepeatX = boomLength / repeatFactor;
            const boomRepeatY = boomHeight / repeatFactor; 
            
            const boomTexture = craneTextureOriginal.clone();
            boomTexture.repeat.set(boomRepeatX, boomRepeatY); 
            boomTexture.needsUpdate = true;
            const boomMaterial = new THREE.MeshStandardMaterial({ map: boomTexture, color: 0xffa500, metalness: 0.5 });
            
            // Torre
            const tower = new THREE.Mesh(towerGeometry, towerMaterials);
            tower.position.set(
                crane.position.x,
                towerHeight / 2,
                crane.position.z
            );
            tower.name = crane.id;
            tower.castShadow = true;
            tower.receiveShadow = true;
            this.scene.add(tower);

            // Garra (Boom)
            const boom = new THREE.Mesh(boomGeometry, boomMaterial);
            boom.position.set(
                crane.position.x + 20,
                towerHeight - 5,
                crane.position.z
            );
            boom.name = `${crane.id}-boom`;
            boom.castShadow = true;
            boom.receiveShadow = true;
            this.scene.add(boom);

            this.addLabel(crane.id, { x: crane.position.x, y: towerHeight + 5, z: crane.position.z });
        }, 
    
        // Callback de Erro/Fallback (Cor Sólida)
        (error) => {
            console.error('Erro ao carregar a textura do guindaste:', texturePath, error);

            // Material Fallback (Cor Laranja Sólida Original)
            const fallbackMaterial = new THREE.MeshStandardMaterial({
                color: 0xffa500,
                roughness: 0.5,
                metalness: 0.3
            });

            // Criação com Fallback - Torre
            const fallbackTower = new THREE.Mesh(towerGeometry, fallbackMaterial);
            fallbackTower.position.set(crane.position.x, towerHeight / 2, crane.position.z);
            fallbackTower.name = crane.id;
            fallbackTower.castShadow = true;
            fallbackTower.receiveShadow = true;
            this.scene.add(fallbackTower);

            // Criação com Fallback - Garra
            const fallbackBoom = new THREE.Mesh(boomGeometry, fallbackMaterial);
            fallbackBoom.position.set(crane.position.x + 20, towerHeight - 5, crane.position.z);
            fallbackBoom.name = `${crane.id}-boom`;
            fallbackBoom.castShadow = true;
            fallbackBoom.receiveShadow = true;
            this.scene.add(fallbackBoom);

            this.addLabel(crane.id, { x: crane.position.x, y: towerHeight + 5, z: crane.position.z });
        }
    );
}

  private addLabel(text: string, position: { x: number; y: number; z: number }, objectHeight?: number): void {
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d')!;
    canvas.width = 256;
    canvas.height = 64;

    context.fillStyle = 'rgba(255, 255, 255, 0.95)';
    context.fillRect(0, 0, canvas.width, canvas.height);
    context.strokeStyle = '#333';
    context.lineWidth = 2;
    context.strokeRect(0, 0, canvas.width, canvas.height);
    context.fillStyle = 'black';
    context.font = 'bold 24px Arial';
    context.textAlign = 'center';
    context.fillText(text, 128, 40);

    const texture = new THREE.CanvasTexture(canvas);
    const spriteMaterial = new THREE.SpriteMaterial({ map: texture, transparent: true });
    const sprite = new THREE.Sprite(spriteMaterial);
    
    const labelHeight = objectHeight ? position.y + objectHeight + 8 : position.y + 10;
    sprite.position.set(position.x, labelHeight, position.z);
    sprite.scale.set(20, 5, 1);
    sprite.visible = false;
    
    this.scene.add(sprite);
    this.objectLabels.set(text, sprite);
  }

  private showLabel(objectName: string): void {
    this.hideLabel();
    const label = this.objectLabels.get(objectName);
    if (label) {
      label.visible = true;
      this.currentLabel = label;
    }
  }

  private hideLabel(): void {
    if (this.currentLabel) {
      this.currentLabel.visible = false;
      this.currentLabel = null;
    }
  }

  private animate(): void {
    this.animationId = requestAnimationFrame(() => this.animate());

    if (this.isAnimatingCamera) {
      const elapsed = performance.now() - this.cameraAnimationStartTime;
      const progress = Math.min(elapsed / this.CAMERA_ANIMATION_DURATION, 1);
      const easeProgress = this.easeInOutQuart(progress);

      this.camera.position.lerpVectors(this.cameraAnimationStartPos, this.cameraAnimationTargetPos, easeProgress);
      this.controls.target.lerpVectors(this.cameraAnimationStartTarget, this.cameraAnimationTargetTarget, easeProgress);

      if (progress >= 1) {
        this.isAnimatingCamera = false;
        this.camera.position.copy(this.cameraAnimationTargetPos);
        this.controls.target.copy(this.cameraAnimationTargetTarget);
      }
    } else {
      const direction = new THREE.Vector3();
      this.camera.getWorldDirection(direction);
      direction.y = 0;
      direction.normalize();

      const right = new THREE.Vector3();
      right.crossVectors(this.camera.up, direction);
      
      const moveVector = new THREE.Vector3(0, 0, 0); 
      const moveStep = this.movementSpeed * 0.3; 

      if (this.keyState['shift']) {
        if (this.keyState['w']) {
          moveVector.y = moveStep;
        }
        if (this.keyState['s']) {
          moveVector.y = -moveStep;
        }
      }
      
      if (!this.keyState['shift']) {
        if (this.keyState['w']) {
          moveVector.addScaledVector(direction, moveStep); 
        }
        if (this.keyState['s']) {
          moveVector.addScaledVector(direction, -moveStep); 
        }
      }

      if (this.keyState['a']) {
        moveVector.addScaledVector(right, moveStep); 
      }
      if (this.keyState['d']) {
        moveVector.addScaledVector(right, -moveStep); 
      }
      
      this.camera.position.add(moveVector);
      this.controls.target.add(moveVector);
    }

    this.controls.update();

    const target = this.controls.target;

    if (target.x > SCENE_LIMIT) {
      target.x = SCENE_LIMIT;
    } else if (target.x < -SCENE_LIMIT) {
      target.x = -SCENE_LIMIT;
    }

    if (target.z > SCENE_LIMIT) {
      target.z = SCENE_LIMIT;
    } else if (target.z < -SCENE_LIMIT) {
      target.z = -SCENE_LIMIT;
    }

    const cameraPosition = this.camera.position;
    cameraPosition.y = Math.max(MIN_CAM_Y, Math.min(MAX_CAM_Y, cameraPosition.y));

    this.renderer.render(this.scene, this.camera);
  }

  private easeInOutCubic(t: number): number {
    return t < 0.5 
      ? 4 * t * t * t 
      : 1 - Math.pow(-2 * t + 2, 3) / 2;
  }

  private easeInOutQuart(t: number): number {
    return t < 0.5
      ? 8 * t * t * t * t
      : 1 - Math.pow(-2 * t + 2, 4) / 2;
  }

  private updateRendererSize(): void {
    const canvas = this.canvasRef.nativeElement;
    const width = canvas.clientWidth || window.innerWidth;
    const height = canvas.clientHeight || window.innerHeight;
    
    this.camera.aspect = width / height;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(width, height, false);
  }

  private onWindowResize(): void {
    this.updateRendererSize();
  }

  currentLayoutId: string = 'layout1';

  switchLayout(layoutId: string): void {
    this.deselectObject();
    this.objectLabels.clear();
    this.currentLabel = null;
    
    while (this.scene.children.length > 0) {
      this.scene.remove(this.scene.children[0]);
    }

    this.initScene();

    this.currentLayoutId = layoutId;
    this.portLayoutService.getPortLayout(layoutId).subscribe({
      next: (data: PortLayout) => {
        this.createPortObjects(data);
      },
      error: (error) => console.error('Error loading layout:', error)
    });
  }
}