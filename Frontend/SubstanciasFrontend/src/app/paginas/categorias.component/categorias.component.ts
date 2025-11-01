import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoriaService } from '../../services/categoria.service';
import { Categoria } from '../../../models/categoria';

@Component({
  selector: 'app-categorias',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './categorias.component.html',
  styleUrls: ['./categorias.component.scss']
})
export class CategoriasComponent implements OnInit {
  categorias: Categoria[] = [];
  novaCategoria: Categoria = { nome: '' };

  constructor(private categoriaService: CategoriaService) {}

  ngOnInit(): void {
    this.listar();
  }

  listar(): void {
    this.categoriaService.listar().subscribe(r => (this.categorias = r));
  }

  salvar(): void {
    const nome = (this.novaCategoria.nome ?? '').trim();
    if (!nome) return;

    this.categoriaService.criar({ nome }).subscribe({
      next: () => {
        alert('Categoria criada!');
        this.listar();
        this.novaCategoria = { nome: '' };
      },
      error: err => alert('Erro ao salvar: ' + err.message)
    });
  }

  deletar(id: number): void {
    if (confirm('Deseja excluir esta categoria?')) {
      this.categoriaService.deletar(id).subscribe(() => this.listar());
    }
  }

  /** trackBy para *ngFor — evita recriar linhas à toa */
  trackById = (_index: number, item: Categoria) => item.id ?? _index;
}
