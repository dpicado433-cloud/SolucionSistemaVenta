namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMVenta
    {
        public int IdVenta { get; set; }
        public string? NumeroVenta { get; set; }
        public int? IdTipoDocumentoVenta { get; set; }
        public int? TipoDocumentoVenta { get; set; }
        public int? IdUsuario { get; set; }
        public string? DocumentoCliente { get; set; }
        public string? NombreCliente { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? ImpuestoTotal { get; set; }
        public decimal? Total { get; set; }
        public virtual ICollection<VMDetalleVenta> DetalleVenta { get; set; }
        public object Usuario { get; internal set; }
        public object FechaRegisrtro { get; internal set; }
    }
}
