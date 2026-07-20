import { Component, signal, computed, output, input, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, LucideIconData } from 'lucide-angular';
import { CatalogService } from '../../../../services/catalog.service';
import { InventoryService } from '../../../../services/inventory.service';
import { Insumo } from '../../../../models/inventory.model';
import { CatalogItem } from '../../../../models/priced-item.model';

interface BOMLine {
  insumoId: string;
  insumoNombre: string;
  insumoCodigo: string;
  cantidad: number;
  unidadMedida: string;
}

interface EquipoQuirurgico {
  id: string;
  nombre: string;
  codigo: string;
  cantidad: number;
}

@Component({
  selector: 'app-edit-cirugia',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-cirugia.component.html',
  styleUrls: ['./edit-cirugia.component.scss']
})
export class EditCirugiaComponent implements OnInit {
  // ── Inputs ──────────────────────────────────────────────────────────────
  public readonly itemId = input<string | null>(null);
  public readonly isEditing = input<boolean>(false);

  // ── Outputs ─────────────────────────────────────────────────────────────
  public readonly saved = output<void>();
  public readonly closed = output<void>();

  // ── Services ────────────────────────────────────────────────────────────
  private readonly catalogService = inject(CatalogService);
  private readonly inventoryService = inject(InventoryService);

  // ── Icons ───────────────────────────────────────────────────────────────
  protected readonly icons = {
    X: LucideIconData.X,
    Save: LucideIconData.Save,
    Loader2: LucideIconData.Loader2,
    Search: LucideIconData.Search,
    Trash2: LucideIconData.Trash2,
    Check: LucideIconData.Check,
    Package: LucideIconData.Package,
    Scissors: LucideIconData.Scissors,
    FileText: LucideIconData.FileText,
    UserCog: LucideIconData.UserCog,
    Clock: LucideIconData.Clock,
    HeartPulse: LucideIconData.HeartPulse,
    Brain: LucideIconData.Brain,
    Activity: LucideIconData.Activity,
    Pill: LucideIconData.Pill,
    Syringe: LucideIconData.Syringe,
    Bone: LucideIconData.Bone,
    Eye: LucideIconData.Eye,
    Ear: LucideIconData.Ear,
    Lungs: LucideIconData.Lungs,
    Heart: LucideIconData.Heart,
    Thermometer: LucideIconData.Thermometer,
    Weight: LucideIconData.Weight,
    Ruler: LucideIconData.Ruler,
    Bandage: LucideIconData.Bandage,
    User: LucideIconData.User,
    Users: LucideIconData.Users,
    MessageSquare: LucideIconData.MessageSquare,
    Video: LucideIconData.Video,
    Phone: LucideIconData.Phone,
    Mail: LucideIconData.Mail,
    Calendar: LucideIconData.Calendar,
    DollarSign: LucideIconData.DollarSign,
    CreditCard: LucideIconData.CreditCard,
    Receipt: LucideIconData.Receipt,
    Settings: LucideIconData.Settings,
    Layers: LucideIconData.Layers,
    Zap: LucideIconData.Zap,
    Shield: LucideIconData.Shield,
    Lock: LucideIconData.Lock,
    Unlock: LucideIconData.Unlock,
    Key: LucideIconData.Key,
    Fingerprint: LucideIconData.Fingerprint,
    Eye: LucideIconData.Eye,
    EyeOff: LucideIconData.EyeOff,
    ScanEye: LucideIconData.ScanEye,
    ScanFace: LucideIconData.ScanFace,
    ScanLine: LucideIconData.ScanLine,
    ScanSearch: LucideIconData.ScanSearch,
    ScanText: LucideIconData.ScanText,
    ScanHeart: LucideIconData.ScanHeart,
    ScanSmile: LucideIconData.ScanSmile,
    Plus: LucideIconData.Plus,
    Minus: LucideIconData.Minus,
    AlertTriangle: LucideIconData.AlertTriangle,
    AlertCircle: LucideIconData.AlertCircle,
    Info: LucideIconData.Info,
    HelpCircle: LucideIconData.HelpCircle,
    ChevronDown: LucideIconData.ChevronDown,
    ChevronUp: LucideIconData.ChevronUp,
    ChevronLeft: LucideIconData.ChevronLeft,
    ChevronRight: LucideIconData.ChevronRight,
    MoreHorizontal: LucideIconData.MoreHorizontal,
    MoreVertical: LucideIconData.MoreVertical,
    Grid: LucideIconData.Grid,
    List: LucideIconData.List,
    Filter: LucideIconData.Filter,
    SortAsc: LucideIconData.SortAsc,
    SortDesc: LucideIconData.SortDesc,
    ArrowUp: LucideIconData.ArrowUp,
    ArrowDown: LucideIconData.ArrowDown,
    ArrowLeft: LucideIconData.ArrowLeft,
    ArrowRight: LucideIconData.ArrowRight,
    ArrowUpRight: LucideIconData.ArrowUpRight,
    ArrowDownLeft: LucideIconData.ArrowDownLeft,
    ExternalLink: LucideIconData.ExternalLink,
    Link: LucideIconData.Link,
    Unlink: LucideIconData.Unlink,
    Link2: LucideIconData.Link2,
    Share: LucideIconData.Share,
    Share2: LucideIconData.Share2,
    Copy: LucideIconData.Copy,
    Clipboard: LucideIconData.Clipboard,
    ClipboardCheck: LucideIconData.ClipboardCheck,
    ClipboardList: LucideIconData.ClipboardList,
    ClipboardCopy: LucideIconData.ClipboardCopy,
    ClipboardX: LucideIconData.ClipboardX,
    Edit: LucideIconData.Edit,
    Edit2: LucideIconData.Edit2,
    Edit3: LucideIconData.Edit3,
    Pen: LucideIconData.Pen,
    PenTool: LucideIconData.PenTool,
    Type: LucideIconData.Type,
    Font: LucideIconData.Font,
    Bold: LucideIconData.Bold,
    Italic: LucideIconData.Italic,
    Underline: LucideIconData.Underline,
    Strikethrough: LucideIconData.Strikethrough,
    Code: LucideIconData.Code,
    Code2: LucideIconData.Code2,
    Terminal: LucideIconData.Terminal,
    Monitor: LucideIconData.Monitor,
    MonitorCheck: LucideIconData.MonitorCheck,
    MonitorDot: LucideIconData.MonitorDot,
    MonitorOff: LucideIconData.MonitorOff,
    MonitorPause: LucideIconData.MonitorPause,
    MonitorPlay: LucideIconData.MonitorPlay,
    MonitorStop: LucideIconData.MonitorStop,
    MonitorX: LucideIconData.MonitorX,
    Smartphone: LucideIconData.Smartphone,
    SmartphoneCharging: LucideIconData.SmartphoneCharging,
    SmartphoneNfc: LucideIconData.SmartphoneNfc,
    Tablet: LucideIconData.Tablet,
    TabletSmartphone: LucideIconData.TabletSmartphone,
    Laptop: LucideIconData.Laptop,
    LaptopMinimal: LucideIconData.LaptopMinimal,
    LaptopMinimalCheck: LucideIconData.LaptopMinimalCheck,
    Server: LucideIconData.Server,
    ServerCog: LucideIconData.ServerCog,
    ServerCrash: LucideIconData.ServerCrash,
    ServerOff: LucideIconData.ServerOff,
    Database: LucideIconData.Database,
    DatabaseBackup: LucideIconData.DatabaseBackup,
    DatabaseZap: LucideIconData.DatabaseZap,
    HardDrive: LucideIconData.HardDrive,
    HardDriveDownload: LucideIconData.HardDriveDownload,
    HardDriveUpload: LucideIconData.HardDriveUpload,
    Usb: LucideIconData.Usb,
    Wifi: LucideIconData.Wifi,
    WifiOff: LucideIconData.WifiOff,
    WifiPen: LucideIconData.WifiPen,
    WifiSync: LucideIconData.WifiSync,
    Bluetooth: LucideIconData.Bluetooth,
    BluetoothConnected: LucideIconData.BluetoothConnected,
    BluetoothOff: LucideIconData.BluetoothOff,
    BluetoothSearching: LucideIconData.BluetoothSearching,
    Battery: LucideIconData.Battery,
    BatteryCharging: LucideIconData.BatteryCharging,
    BatteryLow: LucideIconData.BatteryLow,
    BatteryMedium: LucideIconData.BatteryMedium,
    BatteryFull: LucideIconData.BatteryFull,
    Plug: LucideIconData.Plug,
    PlugZap: LucideIconData.PlugZap,
    Power: LucideIconData.Power,
    PowerOff: LucideIconData.PowerOff,
    Cpu: LucideIconData.Cpu,
    MemoryStick: LucideIconData.MemoryStick,
    Motherboard: LucideIconData.Motherboard,
    Gpu: LucideIconData.Gpu,
    Keyboard: LucideIconData.Keyboard,
    Mouse: LucideIconData.Mouse,
    MousePointer: LucideIconData.MousePointer,
    MousePointer2: LucideIconData.MousePointer2,
    MousePointerClick: LucideIconData.MousePointerClick,
    Touchpad: LucideIconData.Touchpad,
    TouchpadOff: LucideIconData.TouchpadOff,
    Gamepad: LucideIconData.Gamepad,
    Gamepad2: LucideIconData.Gamepad2,
    Joystick: LucideIconData.Joystick,
    Headphones: LucideIconData.Headphones,
    Headset: LucideIconData.Headset,
    Earphone: LucideIconData.Earphone,
    EarphoneOff: LucideIconData.EarphoneOff,
    Mic: LucideIconData.Mic,
    Mic2: LucideIconData.Mic2,
    MicOff: LucideIconData.MicOff,
    Speaker: LucideIconData.Speaker,
    Volume: LucideIconData.Volume,
    Volume2: LucideIconData.Volume2,
    VolumeX: LucideIconData.VolumeX,
    Vibrate: LucideIconData.Vibrate,
    VibrateOff: LucideIconData.VibrateOff,
    Music: LucideIconData.Music,
    Music2: LucideIconData.Music2,
    Music3: LucideIconData.Music3,
    Music4: LucideIconData.Music4,
    Play: LucideIconData.Play,
    PlayCircle: LucideIconData.PlayCircle,
    Pause: LucideIconData.Pause,
    PauseCircle: LucideIconData.PauseCircle,
    Stop: LucideIconData.Stop,
    StopCircle: LucideIconData.StopCircle,
    SkipBack: LucideIconData.SkipBack,
    SkipForward: LucideIconData.SkipForward,
    FastForward: LucideIconData.FastForward,
    Rewind: LucideIconData.Rewind,
    Repeat: LucideIconData.Repeat,
    Repeat1: LucideIconData.Repeat1,
    Shuffle: LucideIconData.Shuffle,
    ListMusic: LucideIconData.ListMusic,
    ListVideo: LucideIconData.ListVideo,
    Video: LucideIconData.Video,
    VideoOff: LucideIconData.VideoOff,
    Camera: LucideIconData.Camera,
    CameraOff: LucideIconData.CameraOff,
    Aperture: LucideIconData.Aperture,
    Focus: LucideIconData.Focus,
    ZoomIn: LucideIconData.ZoomIn,
    ZoomOut: LucideIconData.ZoomOut,
    Scan: LucideIconData.Scan,
    ScanBarcode: LucideIconData.ScanBarcode,
    ScanEye: LucideIconData.ScanEye,
    ScanFace: LucideIconData.ScanFace,
    ScanLine: LucideIconData.ScanLine,
    ScanSearch: LucideIconData.ScanSearch,
    ScanText: LucideIconData.ScanText,
    ScanHeart: LucideIconData.ScanHeart,
    ScanSmile: LucideIconData.ScanSmile,
    QrCode: LucideIconData.QrCode,
    Barcode: LucideIconData.Barcode,
    Square: LucideIconData.Square,
    Circle: LucideIconData.Circle,
    Triangle: LucideIconData.Triangle,
    Diamond: LucideIconData.Diamond,
    Hexagon: LucideIconData.Hexagon,
    Octagon: LucideIconData.Octagon,
    Pentagon: LucideIconData.Pentagon,
    Star: LucideIconData.Star,
    StarHalf: LucideIconData.StarHalf,
    Heart: LucideIconData.Heart,
    HeartCrack: LucideIconData.HeartCrack,
    HeartHandshake: LucideIconData.HeartHandshake,
    HeartOff: LucideIconData.HeartOff,
    HeartPulse: LucideIconData.HeartPulse,
    Pulse: LucideIconData.Pulse,
    Activity: LucideIconData.Activity,
    Zap: LucideIconData.Zap,
    ZapOff: LucideIconData.ZapOff,
    Bolt: LucideIconData.Bolt,
    Flashlight: LucideIconData.Flashlight,
    FlashlightOff: LucideIconData.FlashlightOff,
    Lamp: LucideIconData.Lamp,
    LampCeiling: LucideIconData.LampCeiling,
    LampDesk: LucideIconData.LampDesk,
    LampFloor: LucideIconData.LampFloor,
    LampWallDown: LucideIconData.LampWallDown,
    LampWallUp: LucideIconData.LampWallUp,
    Lightbulb: LucideIconData.Lightbulb,
    LightbulbOff: LucideIconData.LightbulbOff,
    Candle: LucideIconData.Candle,
    Fire: LucideIconData.Fire,
    FireExtinguisher: LucideIconData.FireExtinguisher,
    Droplet: LucideIconData.Droplet,
    Droplets: LucideIconData.Droplets,
    Waves: LucideIconData.Waves,
    WavesLadder: LucideIconData.WavesLadder,
    Cloud: LucideIconData.Cloud,
    CloudCog: LucideIconData.CloudCog,
    CloudDrizzle: LucideIconData.CloudDrizzle,
    CloudFog: LucideIconData.CloudFog,
    CloudHail: LucideIconData.CloudHail,
    CloudLightning: LucideIconData.CloudLightning,
    CloudMoon: LucideIconData.CloudMoon,
    CloudMoonRain: LucideIconData.CloudMoonRain,
    CloudOff: LucideIconData.CloudOff,
    CloudRain: LucideIconData.CloudRain,
    CloudRainWind: LucideIconData.CloudRainWind,
    CloudSnow: LucideIconData.CloudSnow,
    CloudSun: LucideIconData.CloudSun,
    CloudSunRain: LucideIconData.CloudSunRain,
    Cloudy: LucideIconData.Cloudy,
    Fog: LucideIconData.Fog,
    Haze: LucideIconData.Haze,
    Moon: LucideIconData.Moon,
    MoonStar: LucideIconData.MoonStar,
    Sun: LucideIconData.Sun,
    SunDim: LucideIconData.SunDim,
    SunMedium: LucideIconData.SunMedium,
    SunMoon: LucideIconData.SunMoon,
    SunSnow: LucideIconData.SunSnow,
    Thermometer: LucideIconData.Thermometer,
    ThermometerSun: LucideIconData.ThermometerSun,
    ThermometerSnowflake: LucideIconData.ThermometerSnowflake,
    Umbrella: LucideIconData.Umbrella,
    UmbrellaOff: LucideIconData.UmbrellaOff,
    Tornado: LucideIconData.Tornado,
    Wind: LucideIconData.Wind,
    Compass: LucideIconData.Compass,
    Navigation: LucideIconData.Navigation,
    Navigation2: LucideIconData.Navigation2,
    NavigationOff: LucideIconData.NavigationOff,
    Map: LucideIconData.Map,
    MapPin: LucideIconData.MapPin,
    MapPinOff: LucideIconData.MapPinOff,
    MapPinned: LucideIconData.MapPinned,
    Globe: LucideIconData.Globe,
    Globe2: LucideIconData.Globe2,
    GlobeLock: LucideIconData.GlobeLock,
    Locate: LucideIconData.Locate,
    LocateFixed: LucideIconData.LocateFixed,
    LocateOff: LucideIconData.LocateOff,
    Waypoints: LucideIconData.Waypoints,
    Route: LucideIconData.Route,
    RouteOff: LucideIconData.RouteOff,
    Compass: LucideIconData.Compass,
    Navigation: LucideIconData.Navigation,
    Navigation2: LucideIconData.Navigation2,
    NavigationOff: LucideIconData.NavigationOff,
    Map: LucideIconData.Map,
    MapPin: LucideIconData.MapPin,
    MapPinOff: LucideIconData.MapPinOff,
    MapPinned: LucideIconData.MapPinned,
    Globe: LucideIconData.Globe,
    Globe2: LucideIconData.Globe2,
    GlobeLock: LucideIconData.GlobeLock,
    Locate: LucideIconData.Locate,
    LocateFixed: LucideIconData.LocateFixed,
    LocateOff: LucideIconData.LocateOff,
    Waypoints: LucideIconData.Waypoints,
    Route: LucideIconData.Route,
    RouteOff: LucideIconData.RouteOff,
  } as const;

  // ── Form State (Signals) ────────────────────────────────────────────────
  public readonly nombre = signal('');
  public readonly codigo = signal('');
  public readonly precioBaseUsd = signal(0);
  public readonly honorarioBase = signal(0);
  public readonly activo = signal(true);
  public readonly complejidad = signal('MEDIA');
  public readonly duracionEstimadaMinutos = signal(120);
  public readonly requiereAnestesia = signal(true);
  public readonly tipoAnestesia = signal('GENERAL');
  public readonly clasificacionRiesgo = signal('II');
  public readonly notasPreoperatorias = signal('');
  public readonly notasPostoperatorias = signal('');
  public readonly protocoloQuirurgico = signal('');
  public readonly indicaciones = signal('');
  public readonly contraindicaciones = signal('');

  // Equipo Quirúrgico
  public readonly equipoQuirurgico = signal<EquipoQuirurgico[]>([]);
  public readonly equipoSearchQuery = signal('');
  public readonly showEquipoDropdown = signal(false);

  // BOM / Receta (insumos quirúrgicos)
  public readonly bomLines = signal<BOMLine[]>([]);
  public readonly insumoSearchQuery = signal('');
  public readonly showInsumoDropdown = signal(false);

  // Honorarios del equipo
  public readonly honorariosEquipo = signal<Array<{ rol: string; honorarioUsd: number }>>([]);

  // Sugerencias vinculadas
  public readonly sugerenciasSearchQuery = signal('');
  public readonly selectedSugerenciasIds = signal<string[]>([]);

  // UI State
  public readonly isSaving = signal(false);
  public readonly allInsumos = signal<Insumo[]>([]);
  public readonly allSugerencias = signal<CatalogItem[]>([]);
  public readonly allEquipos = signal<CatalogItem[]>([]);

  // ── Computed ────────────────────────────────────────────────────────────
  public readonly filteredInsumos = computed(() => {
    const q = this.insumoSearchQuery().toLowerCase().trim();
    if (!q) return this.allInsumos().slice(0, 20);
    return this.allInsumos().filter(i =>
      i.nombre.toLowerCase().includes(q) ||
      i.codigo.toLowerCase().includes(q)
    ).slice(0, 20);
  });

  public readonly filteredEquipos = computed(() => {
    const q = this.equipoSearchQuery().toLowerCase().trim();
    const selected = new Set(this.equipoQuirurgico().map(e => e.id));
    return this.allEquipos()
      .filter(e => !selected.has(e.id))
      .filter(e => !q || e.descripcion.toLowerCase().includes(q) || e.codigo.toLowerCase().includes(q))
      .slice(0, 20);
  });

  public readonly filteredSugerencias = computed(() => {
    const q = this.sugerenciasSearchQuery().toLowerCase().trim();
    const selected = new Set(this.selectedSugerenciasIds());
    return this.allSugerencias()
      .filter(s => !selected.has(s.id))
      .filter(s => !q || s.descripcion.toLowerCase().includes(q) || s.codigo.toLowerCase().includes(q));
  });

  public readonly selectedSugerenciasCards = computed(() => {
    const ids = new Set(this.selectedSugerenciasIds());
    return this.allSugerencias().filter(s => ids.has(s.id));
  });

  // ── Lifecycle ───────────────────────────────────────────────────────────
  ngOnInit() {
    this.loadInsumos();
    this.loadSugerencias();
    this.loadEquipos();

    if (this.isEditing() && this.itemId()) {
      this.loadItem(this.itemId()!);
    }
  }

  private loadInsumos() {
    this.inventoryService.getInsumos().subscribe({
      next: (data) => this.allInsumos.set(data),
      error: () => console.error('Error loading insumos')
    });
  }

  private loadSugerencias() {
    this.catalogService.getItems().subscribe({
      next: (data) => this.allSugerencias.set(data.filter(i => i.tipo !== 'CIRUGIA')),
      error: () => console.error('Error loading sugerencias')
    });
  }

  private loadEquipos() {
    this.catalogService.getItems().subscribe({
      next: (data) => this.allEquipos.set(data.filter(i => i.tipo === 'EQUIPO' || i.tipo === 'INSTRUMENTAL')),
      error: () => console.error('Error loading equipos')
    });
  }

  private loadItem(id: string) {
    this.catalogService.getItemById(id).subscribe({
      next: (item) => this.populateForm(item),
      error: () => console.error('Error loading cirugia item')
    });
  }

  private populateForm(item: CatalogItem) {
    this.nombre.set(item.descripcion || '');
    this.codigo.set(item.codigo || '');
    this.precioBaseUsd.set(item.precioBaseUsd || 0);
    this.honorarioBase.set(item.honorarioBase || 0);
    this.activo.set(item.activo ?? true);
    this.complejidad.set(item.complejidad || 'MEDIA');
    this.duracionEstimadaMinutos.set(item.duracionEstimadaMinutos || 120);
    this.requiereAnestesia.set(item.requiereAnestesia ?? true);
    this.tipoAnestesia.set(item.tipoAnestesia || 'GENERAL');
    this.clasificacionRiesgo.set(item.clasificacionRiesgo || 'II');
    this.notasPreoperatorias.set(item.notasPreoperatorias || '');
    this.notasPostoperatorias.set(item.notasPostoperatorias || '');
    this.protocoloQuirurgico.set(item.protocoloQuirurgico || '');
    this.indicaciones.set(item.indicaciones || '');
    this.contraindicaciones.set(item.contraindicaciones || '');

    // Load BOM
    this.inventoryService.getRecipe(item.id).subscribe({
      next: (recipe) => {
        const lines: BOMLine[] = recipe.map(r => ({
          insumoId: r.insumoId,
          insumoNombre: r.insumoNombre,
          insumoCodigo: r.insumoCodigo,
          cantidad: r.cantidad,
          unidadMedida: r.unidadMedidaConsumo
        }));
        this.bomLines.set(lines);
      },
      error: () => console.error('Error loading recipe')
    });

    // Load equipo quirúrgico
    if (item.equipoQuirurgico?.length) {
      this.equipoQuirurgico.set(item.equipoQuirurgico);
    }

    // Load honorarios equipo
    if (item.honorariosEquipo?.length) {
      this.honorariosEquipo.set(item.honorariosEquipo);
    }

    // Load sugerencias vinculadas
    if (item.sugerenciasIds?.length) {
      this.selectedSugerenciasIds.set(item.sugerenciasIds);
    }
  }

  // ── Equipo Quirúrgico Handlers ──────────────────────────────────────────
  public addEquipoToList(equipo: CatalogItem) {
    const exists = this.equipoQuirurgico().some(e => e.id === equipo.id);
    if (exists) return;

    this.equipoQuirurgico.update(list => [...list, {
      id: equipo.id,
      nombre: equipo.descripcion,
      codigo: equipo.codigo,
      cantidad: 1
    }]);

    this.equipoSearchQuery.set('');
    this.showEquipoDropdown.set(false);
  }

  public updateEquipoCantidad(index: number, value: number) {
    this.equipoQuirurgico.update(list => {
      const newList = [...list];
      newList[index] = { ...newList[index], cantidad: Math.max(1, value) };
      return newList;
    });
  }

  public removeEquipo(index: number) {
    this.equipoQuirurgico.update(list => list.filter((_, i) => i !== index));
  }

  // ── BOM Handlers ────────────────────────────────────────────────────────
  public addInsumoToBOM(insumo: Insumo) {
    const exists = this.bomLines().some(l => l.insumoId === insumo.id);
    if (exists) return;

    this.bomLines.update(lines => [...lines, {
      insumoId: insumo.id,
      insumoNombre: insumo.nombre,
      insumoCodigo: insumo.codigo,
      cantidad: 1,
      unidadMedida: insumo.unidadMedidaBase
    }]);

    this.insumoSearchQuery.set('');
    this.showInsumoDropdown.set(false);
  }

  public updateBOMCantidad(index: number, value: number) {
    this.bomLines.update(lines => {
      const newLines = [...lines];
      newLines[index] = { ...newLines[index], cantidad: Math.max(0, value) };
      return newLines;
    });
  }

  public removeBOMLine(index: number) {
    this.bomLines.update(lines => lines.filter((_, i) => i !== index));
  }

  // ── Honorarios Equipo Handlers ──────────────────────────────────────────
  public addHonorarioRol() {
    this.honorariosEquipo.update(list => [...list, { rol: '', honorarioUsd: 0 }]);
  }

  public updateHonorarioRol(index: number, field: 'rol' | 'honorarioUsd', value: string | number) {
    this.honorariosEquipo.update(list => {
      const newList = [...list];
      newList[index] = { ...newList[index], [field]: value };
      return newList;
    });
  }

  public removeHonorarioRol(index: number) {
    this.honorariosEquipo.update(list => list.filter((_, i) => i !== index));
  }

  // ── Sugerencias Handlers ────────────────────────────────────────────────
  public toggleSugerencia(id: string) {
    this.selectedSugerenciasIds.update(ids => {
      const set = new Set(ids);
      if (set.has(id)) set.delete(id);
      else set.add(id);
      return Array.from(set);
    });
  }

  public isSugerenciaSelected(id: string): boolean {
    return this.selectedSugerenciasIds().includes(id);
  }

  public removeSugerencia(id: string) {
    this.selectedSugerenciasIds.update(ids => ids.filter(i => i !== id));
  }

  // ── Save ────────────────────────────────────────────────────────────────
  public save() {
    if (!this.nombre() || !this.codigo() || this.precioBaseUsd() <= 0) return;
    this.isSaving.set(true);

    const item: Partial<CatalogItem> = {
      descripcion: this.nombre(),
      codigo: this.codigo(),
      precioBaseUsd: this.precioBaseUsd(),
      honorarioBase: this.honorarioBase(),
      tipo: 'CIRUGIA',
      activo: this.activo(),
      complejidad: this.complejidad(),
      duracionEstimadaMinutos: this.duracionEstimadaMinutos(),
      requiereAnestesia: this.requiereAnestesia(),
      tipoAnestesia: this.tipoAnestesia(),
      clasificacionRiesgo: this.clasificacionRiesgo(),
      notasPreoperatorias: this.notasPreoperatorias(),
      notasPostoperatorias: this.notasPostoperatorias(),
      protocoloQuirurgico: this.protocoloQuirurgico(),
      indicaciones: this.indicaciones(),
      contraindicaciones: this.contraindicaciones(),
      equipoQuirurgico: this.equipoQuirurgico(),
      honorariosEquipo: this.honorariosEquipo(),
      sugerenciasIds: this.selectedSugerenciasIds(),
      requiereInventario: this.bomLines().length > 0
    };

    if (this.isEditing() && this.itemId()) {
      this.catalogService.updateItem(this.itemId()!, item as CatalogItem).subscribe({
        next: () => this.saveRecipes(this.itemId()!),
        error: () => { this.isSaving.set(false); console.error('Error updating cirugia'); }
      });
    } else {
      this.catalogService.createItem(item).subscribe({
        next: (newId) => this.saveRecipes(newId),
        error: () => { this.isSaving.set(false); console.error('Error creating cirugia'); }
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

  // ── Modal ───────────────────────────────────────────────────────────────
  public close() {
    this.closed.emit();
  }

  // ── Helpers ─────────────────────────────────────────────────────────────
  public getTipoColor(tipo: string): string {
    switch (tipo?.toUpperCase()) {
      case 'CONSULTA': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
      case 'LABORATORIO': return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20';
      case 'RX': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
      case 'PROCEDIMIENTO': return 'bg-amber-500/10 text-amber-400 border-amber-500/20';
      case 'TOMOGRAFIA': return 'bg-violet-500/10 text-violet-400 border-violet-500/20';
      case 'MEDICINA':
      case 'MEDICAMENTO': return 'bg-violet-500/10 text-violet-400 border-violet-500/20';
      case 'CIRUGIA': return 'bg-red-500/10 text-red-400 border-red-500/20';
      default: return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
    }
  }

  public getComplejidadColor(complejidad: string): string {
    switch (complejidad) {
      case 'BAJA': return 'bg-green-500/10 text-green-400 border-green-500/20';
      case 'MEDIA': return 'bg-amber-500/10 text-amber-400 border-amber-500/20';
      case 'ALTA': return 'bg-orange-500/10 text-orange-400 border-orange-500/20';
      case 'MUY_ALTA': return 'bg-red-500/10 text-red-400 border-red-500/20';
      default: return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
    }
  }

  public getRiesgoColor(riesgo: string): string {
    switch (riesgo) {
      case 'I': return 'bg-green-500/10 text-green-400 border-green-500/20';
      case 'II': return 'bg-amber-500/10 text-amber-400 border-amber-500/20';
      case 'III': return 'bg-orange-500/10 text-orange-400 border-orange-500/20';
      case 'IV': return 'bg-red-500/10 text-red-400 border-red-500/20';
      default: return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
    }
  }
}