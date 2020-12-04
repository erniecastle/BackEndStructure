using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.CFDI.Sat.Catalogos
{
    public enum CTipoDeComprobante
    {
        [Description("I")]
        I,
        [Description("E")]
        E,
        [Description("T")]
        T,
        [Description("N")]
        N,
        [Description("P")]
        P,
    }
}
