import { Routes } from '@angular/router';
import { SubstanciasComponent } from './paginas/substancias.component/substancias.component';
import { CategoriasComponent } from './paginas/categorias.component/categorias.component';
import { PropriedadesComponent } from './paginas/propriedades.component/propriedades.component';

export const routes: Routes = [
  { path: '', component: SubstanciasComponent },
  { path: 'categorias', component: CategoriasComponent },
  { path: 'propriedades', component: PropriedadesComponent },
  { path: '**', redirectTo: '' },
];
