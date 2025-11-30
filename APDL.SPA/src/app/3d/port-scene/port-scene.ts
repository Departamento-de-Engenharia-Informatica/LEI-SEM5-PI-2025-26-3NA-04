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
  private textureLoader!: THREE.TextureLoader;private keyState: { [key: string]: boolean } = {};
  private readonly movementSpeed = 5.0;

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

    this.camera = new THREE.PerspectiveCamera(
      75,
      window.innerWidth / window.innerHeight,
      0.1,
      1000
    );
    this.camera.position.set(100, 100, 100);
    this.camera.lookAt(0, 0, 0);

    this.renderer = new THREE.WebGLRenderer({
      canvas: this.canvasRef.nativeElement,
      antialias: true
    });
    this.renderer.setSize(window.innerWidth, window.innerHeight);
    this.renderer.setPixelRatio(window.devicePixelRatio);

    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;

    this.controls = new OrbitControls(this.camera, this.renderer.domElement);
    this.controls.enableDamping = true;
    this.controls.dampingFactor = 0.05;

    this.controls.mouseButtons = { LEFT: THREE.MOUSE.PAN, MIDDLE: THREE.MOUSE.DOLLY, RIGHT: THREE.MOUSE.ROTATE };

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

    this.setupKeyboardControls(); 
  }

  private setupKeyboardControls(): void {
    window.addEventListener('keydown', (event) => {
      this.keyState[event.key.toLowerCase()] = true;
    });

    window.addEventListener('keyup', (event) => {
      this.keyState[event.key.toLowerCase()] = false;
    });
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
      this.addLabel(dock.id, dock.position);

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
                this.addLabel(yard.id, yard.position);

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
            this.addLabel(warehouse.id, warehouse.position);
            
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

  private addLabel(text: string, position: { x: number; y: number; z: number }): void {
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d')!;
    canvas.width = 256;
    canvas.height = 64;

    context.fillStyle = 'white';
    context.fillRect(0, 0, canvas.width, canvas.height);
    context.fillStyle = 'black';
    context.font = 'bold 24px Arial';
    context.textAlign = 'center';
    context.fillText(text, 128, 40);

    const texture = new THREE.CanvasTexture(canvas);
    const spriteMaterial = new THREE.SpriteMaterial({ map: texture });
    const sprite = new THREE.Sprite(spriteMaterial);
    sprite.position.set(position.x, position.y + 10, position.z);
    sprite.scale.set(20, 5, 1);
    this.scene.add(sprite);
  }

  private animate(): void {
  this.animationId = requestAnimationFrame(() => this.animate());

  const direction = new THREE.Vector3();
  this.camera.getWorldDirection(direction); // Obtém o vetor para onde a câmara está a olhar
  direction.y = 0; // Impede a câmara de voar (movimento vertical)
  direction.normalize();

  const right = new THREE.Vector3();
  right.crossVectors(this.camera.up, direction); // Vetor perpendicular (para strafe A/D)
  
  // Vetor que irá acumular o deslocamento (por frame)
  const moveVector = new THREE.Vector3(0, 0, 0); 
  const moveStep = this.movementSpeed * 0.3; 

  // 💡 LÓGICA DE MOVIMENTO VERTICAL (Shift + W/S)
    if (this.keyState['shift']) {
        if (this.keyState['w']) {
            moveVector.y = moveStep; // Subir
        }
        if (this.keyState['s']) {
            moveVector.y = -moveStep; // Descer
        }
    }
    
    // LÓGICA DE MOVIMENTO HORIZONTAL (W/S/A/D)
    // Se Shift NÃO estiver pressionado, W/S move para frente/trás (Horizontal).
    if (!this.keyState['shift']) {
        if (this.keyState['w']) {
            moveVector.addScaledVector(direction, moveStep); 
        }
        if (this.keyState['s']) {
            moveVector.addScaledVector(direction, -moveStep); 
        }
    }

    // Movimento Strafe (A/D) - Pode ser combinado com o movimento vertical
    if (this.keyState['a']) {
        moveVector.addScaledVector(right, moveStep); 
    }
    if (this.keyState['d']) {
        moveVector.addScaledVector(right, -moveStep); 
    }
  
  // Aplica o vetor de movimento à POSIÇÃO da câmara e ao PONTO ALVO (target)
  // Mover ambos pelo mesmo vetor simula translação pura
  this.camera.position.add(moveVector);
  this.controls.target.add(moveVector);

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

  private onWindowResize(): void {
    this.camera.aspect = window.innerWidth / window.innerHeight;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(window.innerWidth, window.innerHeight);
  }

  currentLayoutId: string = 'layout1';

  switchLayout(layoutId: string): void {
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