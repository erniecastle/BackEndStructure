using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.CFDI.Timbrado
{
   public class InfoExtra
    {
        public byte[] archivoXML { get; set; }
        public String nombreArchivo { get; set; }

        public decimal total { get; set; }

        public String folio { get; set; }

        public String serie { get; set; }

        public String usuario { get; set; }

        public decimal cfdirecibo_id { get; set; }

        public String rfcEmisor { get; set; }
        public String rfcReceptor { get; set; }
        public String version { get; set; }
        public String UUID { get; set; }
    }
}
