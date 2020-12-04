using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.util
{
    class ConstruyeQueries
    {
        private Dictionary<string, string> aliasTablaOuter;
        private int countTablasOuter = 0;
        private Dictionary<string, string> parametrosQuery;
        private List<string> tablasOuter;
        private List<string> lisParametros;
        public static string LEFTJOIN = "Left Outer Join";
        public static string RIGHTJOIN = "Right Outer Join";
        public static string INNERJOIN = "Inner Join";
        private List<string> listaDatosFormula;
        private string fuenteDatos = "";
        private string tipoOrdenado  = "";
    }
}
