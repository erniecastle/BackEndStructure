using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.CFDI.Timbrado
{
   public  class DatosTimbreFiscalDigital
    {
        public String version { get; set; }
        public String uuid { get; set; }
        public DateTime fechaTimbrado { get; set; }
        public String selloCFD { get; set; }
        public String noCertificadoSAT { get; set; }
        public String selloSAT { get; set; }
        public String status { get; set; }
        public String referenciasProveedor { get; set; }
        public String descripcion { get; set; }
        public String folio { get; set; }

        public byte[] xmlTimbrado { get; set; }

        public int error { get; set; }


    }
}
