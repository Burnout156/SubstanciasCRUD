import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PropriedadeService } from '../../services/propriedade.service';
import { Propriedade, PropertyValueType } from '../../../models/propriedade';

@Component({
  selector: 'app-propriedades',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './propriedades.component.html',
  styleUrls: ['./propriedades.component.scss']
})
export class PropriedadesComponent implements OnInit {
  propriedades: Propriedade[] = [];
  novaPropriedade: Propriedade = { nome: '', valueType: PropertyValueType.Boolean };

  PropertyValueType = PropertyValueType;

  constructor(private propriedadeService: PropriedadeService) {}

  ngOnInit(): void { this.listar(); }

  listar(): void {
    this.propriedadeService.listar().subscribe(r => (this.propriedades = r));
  }

  salvar(): void {
    if (!this.novaPropriedade.nome.trim()) return;

    this.propriedadeService.criar(this.novaPropriedade).subscribe({
      next: () => {
        alert('Propriedade criada!');
        this.listar();
        this.novaPropriedade = { nome: '', valueType: PropertyValueType.Boolean };
      },
      error: err => alert('Erro ao salvar: ' + err.message)
    });
  }

  deletar(id: number): void {
    if (confirm('Deseja excluir esta propriedade?')) {
      this.propriedadeService.deletar(id).subscribe(() => this.listar());
    }
  }

  /** Performance no *ngFor */
  trackById(_: number, item: Propriedade): number | undefined { return item.id; }

  /** Rótulo amigável do enum */
  getTypeLabel(v: PropertyValueType): string {
    return v === PropertyValueType.Boolean ? 'Boolean' : 'Decimal';
  }

  /** Classe do badge (resolve o erro do template) */
  getTypeBadgeClass(v: PropertyValueType): 'boolean' | 'decimal' {
    return v === PropertyValueType.Boolean ? 'boolean' : 'decimal';
  }

  /** Helpers para evitar optional chaining no template */
  get count(): number { return this.propriedades.length; }
  get hasPropriedades(): boolean { return this.propriedades.length > 0; }
}
