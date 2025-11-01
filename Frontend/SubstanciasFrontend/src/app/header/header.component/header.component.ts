import { Component, HostListener } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent {
  mobileOpen = false;
  toggleMobile() { this.mobileOpen = !this.mobileOpen; }
  closeMobile() { this.mobileOpen = false; }

  @HostListener('document:click', ['$event'])
  onDocClick(ev: MouseEvent) {
    const inside = (ev.composedPath() as any[]).some(
      el => (el as HTMLElement)?.classList?.contains?.('navbar')
    );
    if (!inside) this.mobileOpen = false;
  }
}
