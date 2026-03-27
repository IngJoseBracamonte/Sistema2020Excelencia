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
        public const string Reservado = "RESERVADO";
        public const string NoVinculado = "No Vinculado";
        public const string ReservaTemporal = "RESERVA TEMPORAL";
        public const string Desconocido = "Desconocido";
        public const string Laboratorio = "LABORATORIO";
        public const string PrefixLab = "LAB-";

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
