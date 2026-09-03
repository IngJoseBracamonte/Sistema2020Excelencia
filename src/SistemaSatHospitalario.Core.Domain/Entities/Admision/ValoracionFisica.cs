using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ValoracionFisica
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        
        public string EstadoConciencia { get; private set; } // Alerta, Somnoliento, Estuporoso, Inconsciente
        public int GlasgowOcular { get; private set; }
        public int GlasgowVerbal { get; private set; }
        public int GlasgowMotor { get; private set; }

        /// <summary>
        /// LEGACY (3FN): total Glasgow calculado y persistido. Fuente de verdad:
        /// <see cref="GlasgowOcular"/> + <see cref="GlasgowVerbal"/> + <see cref="GlasgowMotor"/>.
        /// Alias de compatibilidad hasta el DROP de columna.
        /// </summary>
        [Obsolete("Calcular como GlasgowOcular + GlasgowVerbal + GlasgowMotor. Columna legacy pendiente de DROP.")]
        public int GlasgowTotal { get; private set; }
        
        public string ViaAerea { get; private set; } // Permeable, Obstruida, Con Apoyo Mecánico
        public string Ventilacion { get; private set; } // Normal, Taquipnea, Disnea, Apnea
        
        public string Pulso { get; private set; } // Rítmico, Arrítmico, Débil, Fuerte
        public string PielMucosas { get; private set; } // Normocoloreada, Pálida, Cianótica, Deshidratada
        public string LlenadoCapilar { get; private set; } // < 2 segundos, > 2 segundos
        public string Pupilas { get; private set; } // Isocóricas, Anisocóricas, Mióticas, Midriáticas
        
        public string Alergias { get; private set; }
        public string AccesosVenosos { get; private set; }
        public string Pertenencias { get; private set; }
        public string AntecedentesMedicos { get; private set; }
        
        public DateTime FechaRegistro { get; private set; }

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioRegistroId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioRegistroId. Columna legacy pendiente de DROP.")]
        public string UsuarioRegistro { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que registró la valoración.</summary>
        public Guid? UsuarioRegistroId { get; private set; }

        public virtual CuentaServicios CuentaServicio { get; private set; }

        protected ValoracionFisica() { }

        public ValoracionFisica(
            Guid cuentaServicioId, 
            string estadoConciencia, 
            int glasgowOcular, 
            int glasgowVerbal, 
            int glasgowMotor, 
            int glasgowTotal, 
            string viaAerea, 
            string ventilacion, 
            string pulso, 
            string pielMucosas, 
            string llenadoCapilar, 
            string pupilas, 
            string allergies, 
            string accesosVenosos, 
            string pertenencias, 
            string antecedentesMedicos, 
            string usuarioRegistro)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            EstadoConciencia = estadoConciencia ?? throw new ArgumentNullException(nameof(estadoConciencia));
            GlasgowOcular = glasgowOcular;
            GlasgowVerbal = glasgowVerbal;
            GlasgowMotor = glasgowMotor;
            GlasgowTotal = glasgowTotal;
            ViaAerea = viaAerea ?? throw new ArgumentNullException(nameof(viaAerea));
            Ventilacion = ventilacion ?? throw new ArgumentNullException(nameof(ventilacion));
            Pulso = pulso ?? throw new ArgumentNullException(nameof(pulso));
            PielMucosas = pielMucosas ?? throw new ArgumentNullException(nameof(pielMucosas));
            LlenadoCapilar = llenadoCapilar ?? throw new ArgumentNullException(nameof(llenadoCapilar));
            Pupilas = pupilas ?? throw new ArgumentNullException(nameof(pupilas));
            Alergias = allergies ?? "";
            AccesosVenosos = accesosVenosos ?? "";
            Pertenencias = pertenencias ?? "";
            AntecedentesMedicos = antecedentesMedicos ?? "";
            FechaRegistro = DateTime.UtcNow;
            UsuarioRegistro = usuarioRegistro ?? throw new ArgumentNullException(nameof(usuarioRegistro));
        }

        public void ActualizarDatos(
            string estadoConciencia, 
            int glasgowOcular, 
            int glasgowVerbal, 
            int glasgowMotor, 
            int glasgowTotal, 
            string viaAerea, 
            string ventilacion, 
            string pulso, 
            string pielMucosas, 
            string llenadoCapilar, 
            string pupilas, 
            string allergies, 
            string accesosVenosos, 
            string pertenencias, 
            string antecedentesMedicos, 
            string usuarioRegistro)
        {
            EstadoConciencia = estadoConciencia ?? throw new ArgumentNullException(nameof(estadoConciencia));
            GlasgowOcular = glasgowOcular;
            GlasgowVerbal = glasgowVerbal;
            GlasgowMotor = glasgowMotor;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            GlasgowTotal = glasgowTotal;
#pragma warning restore CS0618
            ViaAerea = viaAerea ?? throw new ArgumentNullException(nameof(viaAerea));
            Ventilacion = ventilacion ?? throw new ArgumentNullException(nameof(ventilacion));
            Pulso = pulso ?? throw new ArgumentNullException(nameof(pulso));
            PielMucosas = pielMucosas ?? throw new ArgumentNullException(nameof(pielMucosas));
            LlenadoCapilar = llenadoCapilar ?? throw new ArgumentNullException(nameof(llenadoCapilar));
            Pupilas = pupilas ?? throw new ArgumentNullException(nameof(pupilas));
            Alergias = allergies ?? "";
            AccesosVenosos = accesosVenosos ?? "";
            Pertenencias = pertenencias ?? "";
            AntecedentesMedicos = antecedentesMedicos ?? "";
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioRegistro = usuarioRegistro ?? throw new ArgumentNullException(nameof(usuarioRegistro));
#pragma warning restore CS0618
        }
    }
}
