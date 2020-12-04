using Exitosw.Payroll.Entity.entidad.cfdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.CFDI.Timbrado
{
   public class cUUIDCancelado
    {
        public string UUID { get; set; }

        public StatusXmlSat statusSAT { get; set; }

        public string status { get; set; }

        public string observaciones { get; set; }

        public byte[] Acuse { get; set; }
    }
}
