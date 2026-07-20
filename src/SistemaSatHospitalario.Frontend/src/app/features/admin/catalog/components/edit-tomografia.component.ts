import { Component, inject, signal, computed, OnInit, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { Insumo, ServicioInsumoReceta } from '../../../../core/models/inventory.model';
import {
    LucideAngularModule,
    Package,
    Search,
    Plus,
    Trash2,
    X,
    Check,
    FlaskConical,
    Syringe,
    Stethoscope,
    Beaker,
    Save,
    Loader2,
    Contrast,
    FileText,
    UserCog,
    Brain
} from 'lucide-angular';

interface BOMLine {
    insumoId: string;
    insumoNombre: string;
    insumoCodigo: string;
    cantidad: number;
    unidadMedida: string;
    isNew: boolean;
    originalId?: string;
}

@Component({
    selector: 'app-edit-tomografia',
    standalone: true,
    imports: [CommonModule, FormsModule, LucideAngularModule],
    templateUrl: './edit-tomografia.component.html'
})
export class EditTomografiaComponent implements OnInit {
    private catalogService = inject(CatalogService);
    private inventoryService = inject(InventoryService);

    // Inputs / Outputs
    readonly itemId = input<string | null>(null);
    readonly closed = output<void>();
    readonly saved = output<void>();

    // State
    public isLoading = signal<boolean>(false);
    public isSaving = signal<boolean>(false);
    public isEditing = signal<boolean>(false);

    // Form fields
    public nombre = signal<string>('');
    public codigo = signal<string>('');
    public precioBaseUsd = signal<number>(0);
    public honorarioBase = signal<number>(0);
    public activo = signal<boolean>(true);
    public requiereContraste = signal<boolean>(false);
    public protocoloTecnico = signal<string>('');

    // BOM (Bill of Materials / Receta)
    public bomLines = signal<BOMLine[]>([]);
    public insumoSearchQuery = signal<string>('');
    public insumos = signal<Insumo[]>([]);
    public showInsumoDropdown = signal<boolean>(false);

    // Sugerencias (right panel)
    public sugerenciasSearchQuery = signal<string>('');
    public allServices = signal<CatalogItem[]>([]);
    public selectedSugerenciasIds = signal<string[]>([]);

    // Computed
    public filteredInsumos = computed(() => {
        const query = this.insumoSearchQuery().toLowerCase().trim();
        const list = this.insumos();
        if (!query) return list.slice(0, 10);
        return list
            .filter(i =>
                i.nombre.toLowerCase().includes(query) ||
                i.codigo.toLowerCase().includes(query)
            )
            .slice(0, 10);
    });

    public filteredSugerencias = computed(() => {
        const query = this.sugerenciasSearchQuery().toLowerCase().trim();
        const list = this.allServices();
        const currentId = this.itemId();
        if (!query) {
            return list
                .filter(s => s.id !== currentId)
                .slice(0, 20);
        }
        return list
            .filter(s =>
                s.id !== currentId &&
                (s.descripcion.toLowerCase().includes(query) ||
                    s.codigo.toLowerCase().includes(query))
            )
            .slice(0, 20);
    });

    public selectedSugerenciasCards = computed(() => {
        const ids = this.selectedSugerenciasIds();
        return this.allServices().filter(s => ids.includes(s.id));
    });

    readonly icons = {
        Package, Search, Plus, Trash2, X, Check,
        FlaskConical, Syringe, Stethoscope, Beaker,
        Save, Loader2, Contrast, FileText, UserCog, Brain
    };

    ngOnInit() {
        this.loadInsumos();
        this.loadAllServices();

        const id = this.itemId();
        if (id) {
            this.isEditing.set(true);
            this.loadExistingItem(id);
        }
    }

    private loadInsumos() {
        this.inventoryService.getInsumos(true).subscribe({
            next: (res) => this.insumos.set(res),
            error: () => console.error('Error loading insumos')
        });
    }

    private loadAllServices() {
        this.catalogService.getUnifiedCatalog().subscribe({
            next: (res) => this.allServices.set(res),
            error: () => console.error('Error loading catalog')
        });
    }

    private loadExistingItem(id: string) {
        this.isLoading.set(true);
        this.catalogService.getUnifiedCatalog().subscribe({
            next: (res) => {
                const item = res.find(i => i.id === id);
                if (item) {
                    this.nombre.set(item.descripcion || '');
                    this.codigo.set(item.codigo || '');
                    this.precioBaseUsd.set(item.precioUsd ?? 0);
                    this.honorarioBase.set(item.honorarioBase ?? 0);
                    this.activo.set(item.activo ?? true);
                    this.requiereContraste.set(item.requiereContraste ?? false);
                    this.protocoloTecnico.set(item.protocoloTecnico ?? '');
                    this.selectedSugerenciasIds.set(item.sugerenciasIds ?? []);

                    // Load existing BOM/recipe
                    this.loadExistingRecipe(id);
                }
                this.isLoading.set(false);
            },
            error: () => this.isLoading.set(false)
        });
    }

    private loadExistingRecipe(servicioId: string) {
        this.inventoryService.getRecetas().subscribe({
            next: (recetas) => {
                const relevant = recetas.filter(r => r.servicioClinicoId === servicioId);
                const lines: BOMLine[] = relevant.map(r => ({
                    insumoId: r.insumoId,
                    insumoNombre: r.insumo?.nombre ?? '',
                    insumoCodigo: r.insumo?.codigo ?? '',
                    cantidad: r.cantidad,
                    unidadMedida: r.unidadMedidaConsumo || r.insumo?.unidadMedidaBase || 'UNIDAD',
                    isNew: false,
                    originalId: r.id
                }));
                this.bomLines.set(lines);
            },
            error: () => console.error('Error loading recipes')
        });
    }

    // ── BOM Methods ──────────────────────────────────────────────────────

    public addInsumoToBOM(insumo: Insumo) {
        const current = this.bomLines();
        if (current.some(l => l.insumoId === insumo.id)) {
            this.insumoSearchQuery.set('');
            this.showInsumoDropdown.set(false);
            return;
        }
        this.bomLines.set([...current, {
            insumoId: insumo.id,
            insumoNombre: insumo.nombre,
            insumoCodigo: insumo.codigo,
            cantidad: 1,
            unidadMedida: insumo.unidadMedidaBase || 'UNIDAD',
            isNew: true
        }]);
        this.insumoSearchQuery.set('');
        this.showInsumoDropdown.set(false);
    }

    public removeBOMLine(index: number) {
        const current = [...this.bomLines()];
        current.splice(index, 1);
        this.bomLines.set(current);
    }

    public updateBOMCantidad(index: number, value: number) {
        const current = [...this.bomLines()];
        current[index] = { ...current[index], cantidad: isNaN(value) ? 0 : value };
        this.bomLines.set(current);
    }

    // ── Sugerencias Methods ──────────────────────────────────────────────

    public toggleSugerencia(id: string) {
        const current = [...this.selectedSugerenciasIds()];
        const idx = current.indexOf(id);
        if (idx > -1) {
            current.splice(idx, 1);
        } else {
            current.push(id);
        }
        this.selectedSugerenciasIds.set(current);
    }

    public isSugerenciaSelected(id: string): boolean {
        return this.selectedSugerenciasIds().includes(id);
    }

    public removeSugerencia(id: string) {
        this.selectedSugerenciasIds.update(ids => ids.filter(i => i !== id));
    }

    // ── Save ─────────────────────────────────────────────────────────────

    public save() {
        this.isSaving.set(true);
        const item: Partial<CatalogItem> = {
            id: this.itemId() ?? undefined,
            descripcion: this.nombre(),
            codigo: this.codigo(),
            precioUsd: this.precioBaseUsd(),
            honorarioBase: this.honorarioBase(),
            tipo: 'TOMOGRAFIA',
            activo: this.activo(),
            requiereContraste: this.requiereContraste(),
            protocoloTecnico: this.protocoloTecnico(),
            sugerenciasIds: this.selectedSugerenciasIds(),
            requiereInventario: this.bomLines().length > 0
        };

        if (this.isEditing() && this.itemId()) {
            this.catalogService.updateItem(item as CatalogItem).subscribe({
                next: () => this.saveRecipes(this.itemId()!),
                error: () => { this.isSaving.set(false); console.error('Error updating tomografia'); }
            });
        } else {
            this.catalogService.createItem(item).subscribe({
                next: (newId) => this.saveRecipes(newId),
                error: () => { this.isSaving.set(false); console.error('Error creating tomografia'); }
            });
        }
    }

    private saveRecipes(servicioId: string) {
        const lines = this.bomLines();
        if (lines.length === 0) {
            this.isSaving.set(false);
            this.saved.emit();
            this.closed.emit();
            return;
        }

        let completed = 0;
        const total = lines.length;

        lines.forEach(line => {
            this.inventoryService.createOrUpdateRecipe({
                servicioClinicoId: servicioId,
                insumoId: line.insumoId,
                cantidad: line.cantidad,
                unidadMedidaConsumo: line.unidadMedida
            }).subscribe({
                next: () => {
                    completed++;
                    if (completed >= total) {
                        this.isSaving.set(false);
                        this.saved.emit();
                        this.closed.emit();
                    }
                },
                error: () => {
                    completed++;
                    if (completed >= total) {
                        this.isSaving.set(false);
                        this.saved.emit();
                        this.closed.emit();
                    }
                }
            });
        });
    }

    // ── Modal ────────────────────────────────────────────────────────────

    public close() {
        this.closed.emit();
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    public getTipoColor(tipo: string): string {
        switch (tipo?.toUpperCase()) {
            case 'CONSULTA': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
            case 'LABORATORIO': return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20';
            case 'RX': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
            case 'PROCEDIMIENTO': return 'bg-amber-500/10 text-amber-400 border-amber-500/20';
            case 'TOMOGRAFIA': return 'bg-violet-500/10 text-violet-400 border-violet-500/20';
            case 'MEDICINA':
            case 'MEDICAMENTO': return 'bg-violet-500/10 text-violet-400 border-violet-500/20';
            default: return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
        }
    }
}