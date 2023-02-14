
namespace WebApplicationMostrar.Models
{
    public class CriptoConversion
    {

         public  IEnumerable<CryptocurrencyWithLatestQuote> Datos { get; set; }

        public double IdCantidad { get; set; }
        public string IdLstCritomoneda { get; set; }

        public double? ValorTotal { get; set; }

        public double? CantidadConvertida { get; set; }
        public CriptoConversion(IEnumerable<CryptocurrencyWithLatestQuote> datos, double cantidad)
        {
            this.Datos = datos;
            this.IdCantidad = cantidad;
        }


    }
}
