

namespace Exitosw.Payroll.Core.genericos.campos
{
    public class CamposOrden
    {

        public CamposOrden()
        {
        }

        public CamposOrden(string campo)
        {
            this.campo = campo;
            this.tipoOrden = TipoOrden.ASCENDENTE;
        }

        public CamposOrden(string campo, TipoOrden tipoOrden)
        {
            this.campo = campo;
            this.tipoOrden = tipoOrden;
        }

        public string campo { get; set; }

        public TipoOrden tipoOrden { get; set; }

        //public Type tipoDato { get; set; }
    }
}
