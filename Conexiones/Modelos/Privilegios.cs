namespace Conexiones
{
    public class Privilegios
    {
        public int IDPrivilegios { get; set; }
        public int IdUsuario { get; set; }
        public int ImprimirResultado { get; set; }
        public int ReimprimirResultado { get; set; }
        public int Validar { get; set; }
        public int Modificar { get; set; }
        public int AgregarConvenio { get; set; }
        public int QuitarConvenio { get; set; }
        public int VerLibroVenta { get; set; }
        public int VerCierreCaja { get; set; }
        public int VerOrdenes { get; set; }
        public int VerEstadisticas { get; set; }
        public int VerReporteBioanalista { get; set; }
        public int VerReferidos { get; set; }
        public int ImprimirFactura { get; set; }
        public int ReImprimirFactura { get; set; }
        public int AgregarUsuario { get; set; }
        public int ModificarUsuario { get; set; }
        public int CambioDePrecios { get; set; }
        public int ImprimirEtiqueta { get; set; }
        public int TeclasHematologia { get; set; }
        public int AgregarAnalisis { get; set; }
        public int ModificarAnalisis { get; set; }
        public int EliminarAnalisis { get; set; }
        public int ModificarOrden { get; set; }
        public int AnularOrden{ get; set; }
    }
}
