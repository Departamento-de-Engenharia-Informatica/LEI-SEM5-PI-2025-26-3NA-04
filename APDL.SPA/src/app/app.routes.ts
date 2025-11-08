import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Cube } from './3d/cube/cube';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: Home },
  { path: 'cube', component: Cube }
];