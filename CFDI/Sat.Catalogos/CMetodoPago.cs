using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.CFDI.Sat.Catalogos
{
    public enum CMetodoPago
    {
        [Description("PUE")]
        PUE,
        [Description("PIP")]
        PIP,
        [Description("PPD")]
        PPD,
    }
}
