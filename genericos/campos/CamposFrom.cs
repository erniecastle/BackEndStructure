

namespace Exitosw.Payroll.Core.genericos.campos
{
    public class CamposFrom
    {

        public CamposFrom()
        {

        }

        public CamposFrom(string campo)
        {
            this.campo = campo;
        }

        public CamposFrom(string campo, TipoJoin tipoJoin)
        {
            this.campo = campo;
            this.tipoJoin = tipoJoin;
        }

        public string campo { get; set; }

        public TipoJoin tipoJoin { get; set; }

    }
}
