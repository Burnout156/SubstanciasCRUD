import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SubstanciaService } from '../../services/substancia.service';
import { CategoriaService } from '../../services/categoria.service';
import { PropriedadeService } from '../../services/propriedade.service';

import { Categoria } from '../../../models/categoria';
import { Propriedade, PropertyValueType } from '../../../models/propriedade';
import { Substancia } from '../../../models/substancia';
import { SubstanciaListItem } from '../../../models/pagination';
import { forkJoin } from 'rxjs';


@Component({
  selector: 'app-substancias',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './substancias.component.html',
  styleUrls: ['./substancias.component.scss']
})
export class SubstanciasComponent implements OnInit {
  substancias: SubstanciaListItem[] = [];
  categorias: Categoria[] = [];
  propriedades: Propriedade[] = [];

  novaSubstancia: Substancia = {
    codigo: '',
    nome: '',
    descricao: '',
    notas: '',
    categoriaId: 0,
    propriedades: []
  };

  constructor(
    private substanciaService: SubstanciaService,
    private categoriaService: CategoriaService,
    private propriedadeService: PropriedadeService
  ) {}

  ngOnInit(): void {
    this.carregarTudo();
  }

  carregarTudo() {
    forkJoin({
      substancias: this.substanciaService.listar(),
      categorias: this.categoriaService.listar(),
      propriedades: this.propriedadeService.listar()
    }).subscribe({
      next: ({ substancias, categorias, propriedades }) => {
        this.substancias = substancias;
        this.categorias = categorias;
        this.propriedades = propriedades;
      },
      error: err => {
        console.error('Falha ao carregar dados', err);
        alert('Falha ao carregar dados.');
      }
    });
  }

  isChecked(propriedadeId: number): boolean {
    return this.novaSubstancia.propriedades.some(p => p.propriedadeId === propriedadeId);
    // se quiser performance: return this._checkedSet.has(propriedadeId);
  }

  togglePropriedade(p: Propriedade, event: Event) {
    const checked = (event.target as HTMLInputElement).checked;

    if (checked) {
      this.novaSubstancia.propriedades.push({
        propriedadeId: p.id!,
        valueType: p.valueType,
        boolValue: p.valueType === PropertyValueType.Boolean ? false : undefined,
        decimalValue: p.valueType === PropertyValueType.Decimal ? 0 : undefined
      });
    } else {
      this.novaSubstancia.propriedades = this.novaSubstancia.propriedades.filter(
        x => x.propriedadeId !== p.id
      );
    }
  }

  salvar() {
    this.substanciaService.criar(this.novaSubstancia).subscribe({
      next: () => {
        alert('Substância criada!');
        this.carregarTudo();
        this.resetarFormulario();
      },
      error: err => {
        // tenta exibir a mensagem do backend ("Código já existe.")
        const msg = err?.error?.title || err?.error?.detail || err?.error?.message || err?.message || 'Erro ao criar.';
        alert(msg);
      }
    });
  }

  deletar(id: number) {
    if (confirm('Tem certeza que deseja excluir esta substância?')) {
      this.substanciaService.deletar(id).subscribe(() => this.carregarTudo());
    }
  }

  resetarFormulario() {
    this.novaSubstancia = {
      codigo: '',
      nome: '',
      descricao: '',
      notas: '',
      categoriaId: 0,
      propriedades: []
    };
  }

  /** Fallback só se o backend não mandar categoriaNome */
  getCategoriaNome(id: number, categoriaNome?: string): string {
    if (categoriaNome) return categoriaNome;
    const categoria = this.categorias.find(c => c.id === id);
    return categoria ? categoria.nome : '—';
  }

  trackById(_: number, s: SubstanciaListItem) { return s.id; }
}
