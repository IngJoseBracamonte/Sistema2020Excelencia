namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ValoracionFisica
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        
        // Signos Neurológicos (Escala de Glasgow)
        public string EstadoConciencia { get; private set; } // Alerta, Somnoliento, Estuporoso, Inconsciente
        public int GlasgowOcular { get; private set; }
        public int GlasgowVerbal { get; private set; }
        public int GlasgowMotor { get; private set; }

        // ✅ Propiedad calculada dinámica (Sin warnings, EF Core la ignora automáticamente)
        public int GlasgowTotal => GlasgowOcular + GlasgowVerbal + GlasgowMotor;
        
        // Evaluación Física Primaria
        public string ViaAerea { get; private set; } // Permeable, Obstruida, Con Apoyo Mecánico
        public string Ventilacion { get; private set; } // Normal, Taquipnea, Disnea, Apnea
        public string Pulso { get; private set; } // Rítmico, Arrítmico, Débil, Fuerte
        public string PielMucosas { get; private set; } // Normocoloreada, Pálida, Cianótica, Deshidratada
        public string LlenadoCapilar { get; private set; } // < 2 segundos, > 2 segundos
        public string Pupilas { get; private set; } // Isocóricas, Anisocóricas, Mióticas, Midriáticas
        
        // Información Clínica Adicional
        public string Alergias { get; private set; }
        public string AccesosVenosos { get; private set; }
        public string Pertenencias { get; private set; }
        public string AntecedentesMedicos { get; private set; }
        
        public DateTime FechaRegistro { get; private set; }

        // Auditoría e Identidad (3FN)
        public Guid? UsuarioRegistroId { get; private set; }
        public string? UsuarioRegistro { get; private set; } // Alias legacy opcional

        public virtual CuentaServicios CuentaServicio { get; private set; } = null!;

        protected ValoracionFisica() { }

            public ValoracionFisica(
                Guid cuentaServicioId, 
                string estadoConciencia, 
                int glasgowOcular, 
                int glasgowVerbal, 
                int glasgowMotor, 
                string viaAerea, 
                string ventilacion, 
                string pulso, 
                string pielMucosas, 
                string llenadoCapilar, 
                string pupilas, 
                string? allergies = null, 
                string? accesosVenosos = null, 
                string? pertenencias = null, 
                string? antecedentesMedicos = null, 
                Guid? usuarioRegistroId = null,
                string? usuarioRegistro = null)
            {
                Id = Guid.NewGuid();
                CuentaServicioId = cuentaServicioId;
                EstadoConciencia = estadoConciencia ?? throw new ArgumentNullException(nameof(estadoConciencia));
                GlasgowOcular = glasgowOcular;
                GlasgowVerbal = glasgowVerbal;
                GlasgowMotor = glasgowMotor;
                ViaAerea = viaAerea ?? throw new ArgumentNullException(nameof(viaAerea));
                Ventilacion = ventilacion ?? throw new ArgumentNullException(nameof(ventilacion));
                Pulso = pulso ?? throw new ArgumentNullException(nameof(pulso));
                PielMucosas = pielMucosas ?? throw new ArgumentNullException(nameof(pielMucosas));
                LlenadoCapilar = llenadoCapilar ?? throw new ArgumentNullException(nameof(llenadoCapilar));
                Pupilas = pupilas ?? throw new ArgumentNullException(nameof(pupilas));
                Alergias = allergies ?? string.Empty;
                AccesosVenosos = accesosVenosos ?? string.Empty;
                Pertenencias = pertenencias ?? string.Empty;
                AntecedentesMedicos = antecedentesMedicos ?? string.Empty;
            }

            public void ActualizarDatos(
                string estadoConciencia, 
                int glasgowOcular, 
                int glasgowVerbal, 
                int glasgowMotor, 
                string viaAerea, 
                string ventilacion, 
                string pulso, 
                string pielMucosas, 
                string llenadoCapilar, 
                string pupilas, 
                string? allergies = null, 
                string? accesosVenosos = null, 
                string? pertenencias = null, 
                string? antecedentesMedicos = null)
            {
                EstadoConciencia = estadoConciencia ?? throw new ArgumentNullException(nameof(estadoConciencia));
                GlasgowOcular = glasgowOcular;
                GlasgowVerbal = glasgowVerbal;
                GlasgowMotor = glasgowMotor;
                ViaAerea = viaAerea ?? throw new ArgumentNullException(nameof(viaAerea));
                Ventilacion = ventilacion ?? throw new ArgumentNullException(nameof(ventilacion));
                Pulso = pulso ?? throw new ArgumentNullException(nameof(pulso));
                PielMucosas = pielMucosas ?? throw new ArgumentNullException(nameof(pielMucosas));
                LlenadoCapilar = llenadoCapilar ?? throw new ArgumentNullException(nameof(llenadoCapilar));
                Pupilas = pupilas ?? throw new ArgumentNullException(nameof(pupilas));
                Alergias = allergies ?? string.Empty;
                AccesosVenosos = accesosVenosos ?? string.Empty;
                Pertenencias = pertenencias ?? string.Empty;
                AntecedentesMedicos = antecedentesMedicos ?? string.Empty;
            }
        }
    }