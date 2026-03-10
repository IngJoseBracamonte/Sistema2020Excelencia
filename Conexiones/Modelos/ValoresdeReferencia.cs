using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class mayoromenorreferencial
    {
        public int IdEspecie { get; set; }
        public int IdAnalisis { get; set; }
        public int TipoAnalisis { get; set; }
        public String ValorMayor
        {
            get { return _ValorMayor ?? String.Empty; }
            set { _ValorMayor = value; }
        }
        public String ValorMenor
        {
            get { return _ValorMenor ?? String.Empty; }
            set { _ValorMenor = value; }
        }
        public String MultiplesValores
        {
            get { return _MultiplesValores ?? String.Empty; }
            set { _MultiplesValores = value; }
        }
        public String Unidad
        {
            get { return _Unidad ?? String.Empty; }
            set { _Unidad = value; }
        }
        private string _Unidad { get; set; }
        private string _ValorMayor { get; set; }
        private string _ValorMenor { get; set; }
        private string _MultiplesValores { get; set; }
        public int lineas { get; set; }

    }
}
