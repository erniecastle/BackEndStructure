using System.Collections.Generic;

namespace Exitosw.Payroll.Core.genericos.campos
{
    public class OperadorSelect
    {
        public OperadorSelect()
        {
            listCamposSelect = new List<CamposSelect>();
        }

        public OperadorSelect(List<CamposSelect> listCamposSelect)
        {
            this.listCamposSelect = listCamposSelect;
        }

        public bool usaDistinct { get; set; } = false;

        public bool todosDatos { get; set; } = false;

        //Solo usar COUNT
        public TipoFuncion tipoFuncion { get; set; } = TipoFuncion.NINGUNO; // solo se usa cuando todosDatos es true seria *

        public List<CamposSelect> listCamposSelect { get; set; }

        //return objeto lista pendiente
    }
}
