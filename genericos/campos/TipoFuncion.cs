
using System.ComponentModel;

namespace Exitosw.Payroll.Core.genericos.campos
{
    public enum TipoFuncion
    {
        [Description("COUNT")]
        CONTAR,
        [Description("SUM")]
        SUMAR,
        [Description("MAX")]
        MAXIMO,
        [Description("MIN")]
        MINIMO,
        [Description("CONCAT")]
        CONCATENAR,
        [Description("")]
        NINGUNO
    }
}
