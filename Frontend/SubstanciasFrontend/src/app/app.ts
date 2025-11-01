import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./header/header.component/header.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  standalone: true,
  template: `
    <app-header></app-header>
    <main class="main-container">
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [
    `
      .main-container {
        padding: 20px;
      }
    `
  ]
})
export class App {
  protected readonly title = signal('SubstanciasFrontend');
}
