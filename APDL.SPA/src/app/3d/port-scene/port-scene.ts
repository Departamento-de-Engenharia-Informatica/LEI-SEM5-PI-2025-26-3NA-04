import { Component, OnInit, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import { PortLayoutService, PortLayout } from '../../services/port-layout';

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

    this.controls = new OrbitControls(this.camera, this.renderer.domElement);
    this.controls.enableDamping = true;
    this.controls.dampingFactor = 0.05;

    const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
    this.scene.add(ambientLight);

    const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
    directionalLight.position.set(50, 100, 50);
    this.scene.add(directionalLight);

    const groundGeometry = new THREE.PlaneGeometry(500, 500);
    const groundMaterial = new THREE.MeshStandardMaterial({ color: 0x228b22 });
    const ground = new THREE.Mesh(groundGeometry, groundMaterial);
    ground.rotation.x = -Math.PI / 2;
    ground.position.y = -1;
    this.scene.add(ground);

    const gridHelper = new THREE.GridHelper(500, 50, 0x000000, 0x444444);
    gridHelper.position.y = -0.5;
    this.scene.add(gridHelper);

    window.addEventListener('resize', () => this.onWindowResize());
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

      dock.stsCranes.forEach(crane => {
        this.createSTSCrane(crane);
      });
    });

    layout.containerYards.forEach(yard => {
      const geometry = new THREE.BoxGeometry(
        yard.dimensions.width,
        yard.dimensions.height,
        yard.dimensions.depth
      );
      const material = new THREE.MeshStandardMaterial({
        color: 0x808080,
        roughness: 0.8
      });
      const mesh = new THREE.Mesh(geometry, material);
      mesh.position.set(yard.position.x, yard.position.y, yard.position.z);
      mesh.name = yard.id;
      this.scene.add(mesh);
      this.addLabel(yard.id, yard.position);
    });

    layout.warehouses.forEach(warehouse => {
      const geometry = new THREE.BoxGeometry(
        warehouse.dimensions.width,
        warehouse.dimensions.height,
        warehouse.dimensions.depth
      );
      const material = new THREE.MeshStandardMaterial({
        color: 0xb22222,
        roughness: 0.6
      });
      const mesh = new THREE.Mesh(geometry, material);
      mesh.position.set(warehouse.position.x, warehouse.position.y, warehouse.position.z);
      mesh.name = warehouse.id;
      this.scene.add(mesh);
      this.addLabel(warehouse.id, warehouse.position);
    });
  }

  private createSTSCrane(crane: any): void {
    const baseWidth = 6;
    const baseDepth = 6;

    const towerGeometry = new THREE.BoxGeometry(baseWidth, crane.height, baseDepth);
    const craneMaterial = new THREE.MeshStandardMaterial({
      color: 0xffa500,
      roughness: 0.5,
      metalness: 0.3
    });
    const tower = new THREE.Mesh(towerGeometry, craneMaterial);
    tower.position.set(
      crane.position.x,
      crane.height / 2,
      crane.position.z
    );
    tower.name = crane.id;
    this.scene.add(tower);

    const boomGeometry = new THREE.BoxGeometry(40, 2, 3);
    const boom = new THREE.Mesh(boomGeometry, craneMaterial);
    boom.position.set(
      crane.position.x + 20,
      crane.height - 5,
      crane.position.z
    );
    boom.name = `${crane.id}-boom`;
    this.scene.add(boom);

    this.addLabel(crane.id, { x: crane.position.x, y: crane.height + 5, z: crane.position.z });
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
    this.controls.update();
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