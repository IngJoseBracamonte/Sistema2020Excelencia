using System;

namespace SistemaSatHospitalario.Core.Domain.Constants
{
    /// <summary>
    /// Centraliza los estados de las entidades para asegurar consistencia 
    /// en las consultas de base de datos y lógica de dominio. (Senior Pattern: State Constants)
    /// </summary>
    public static class EstadoConstants
    {
        // Estados Generales / Facturación
        public const string Anulada = "Anulada";
        public const string Borrador = "Borrador";
        public const string Emitida = "Emitida";
        
        // Estados de Cuenta de Servicios
        public const string Abierta = "Abierta";
        public const string Facturada = "Facturada";

        // Tipos de Ingreso / Convenios
        public const string Particular = "Particular";
        public const string Seguro = "Seguro";
        public const string Hospitalizacion = "Hospitalizacion";
        public const string Emergencia = "Emergencia";

        // Tipos de Appointments / UI
        public const string TypeCita = "Cita";
        public const string TypeReserva = "Reserva";
        public const string TypeBloqueo = "Bloqueo";

        // UI Labels para Calendario / Turnos
        public const string LabelLibre = "Libre";
        public const string LabelTuCita = "Tu Cita (Agregada)";
        public const string LabelOcupado = "Ocupado";
        public const string LabelBloqueadoAdmin = "Bloqueado Administrativamente";
        public const string LabelTuReserva = "Tu reserva actual (Vigente)";
        public const string LabelEnProceso = "En proceso de facturación...";

        public const string DefaultCajero = "Cajero";

        // Acciones Administrativas
        public const string ActionDelete = "Delete";
        public const string ActionUpdate = "Update";

        public const string Reservado = "RESERVADO";
        public const string NoVinculado = "No Vinculado";
        public const string ReservaTemporal = "RESERVA TEMPORAL";
        public const string Desconocido = "Desconocido";
        public const string Laboratorio = "LABORATORIO";
        public const string PrefixLab = "LAB-";
        public const string RX = "RX";
        public const string Medico = "MEDICO";

        public static bool EsLaboratorio(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo)) return false;
            return tipo.Equals(Laboratorio, StringComparison.OrdinalIgnoreCase);
        }

        // Prefijos para Identificación de Consultas (Senior Recognition Pattern)
        public static readonly string[] ConsultaPrefixes = { "CONS", "MEDI", "MÉDI", "OBST", "GINE" };

        public static bool EsConsulta(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo)) return false;
            var t = tipo.ToUpper();
            // Senior Logic: Match by first 4 characters or presence of prefix
            foreach (var prefix in ConsultaPrefixes)
            {
                if (t.Contains(prefix)) return true;
            }
            return false;
        }

        // Estados de Citas Médicas
        public const string Pendiente = "Pendiente";
        public const string Confirmada = "Confirmada";
        public const string Cancelada = "Cancelada";
        public const string Atendida = "Atendida";
        public const string Cancelado = "Cancelado"; // Nota: Algunas entidades usan masculino por legado

        // Estados de Cuentas por Cobrar
        public const string Parcial = "Parcial";
        public const string Pagada = "Pagada";
        public const string Cobrada = "Cobrada";
    }
}
