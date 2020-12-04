
using System.ComponentModel;

namespace Exitosw.Payroll.Core.genericos.campos
{
    public enum TipoJoin
    {
        [Description("INNER JOIN")]
        INNER_JOIN,
        [Description("LEFT JOIN")]
        LEFT_JOIN,
        [Description("RIGHT JOIN")]
        RIGHT_JOIN
    }
}
