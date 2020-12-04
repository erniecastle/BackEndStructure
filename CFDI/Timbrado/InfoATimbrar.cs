using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.CFDI.Timbrado
{
    public class InfoATimbrar
    {
        public List<InfoExtra> infoExtras { get; set; }
        public String usuario { get; set; }
        public String password { get; set; }
        public String urlWebServices { get; set; }
        public byte[] archivoPfx { get; set; }
        public byte[] archivoKey { get; set; }
        public String passwordKey { get; set; }
        public TipoOperacionWS tipoOperacion { get; set; }
    }
}
