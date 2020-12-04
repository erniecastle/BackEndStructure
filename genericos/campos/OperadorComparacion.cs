using System.ComponentModel;

namespace Exitosw.Payroll.Core.genericos.campos
{
    public enum OperadorComparacion
    {
        [Description("=")]
        IGUAL,
        [Description("!=")]  //<>  postgress
        DIFERENTE,
        [Description(">")]
        MAYOR,
        [Description("<")]
        MENOR,
        [Description(">=")]
        MAYOR_IGUAL,
        [Description("<=")]
        MENOR_IGUAL,
        [Description("BETWEEN")]
        BETWEEN,
        [Description("NOT BETWEEN")]
        NOT_BETWEEN,
        [Description("NOT LIKE")]
        NOT_LIKE,
        [Description("LIKE")]
        LIKE,
        [Description("IS NULL")]
        IS_NULL,
        [Description("IS NOT NULL")]
        IS_NOT_NULL,
        [Description("IN")]
        IN,
        [Description("NOT IN")]
        NOT_IN
    }
}
