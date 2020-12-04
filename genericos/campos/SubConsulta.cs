using Exitosw.Payroll.Core.campos;
using System.Collections.Generic;

namespace Exitosw.Payroll.Core.genericos.campos
{
    public class SubConsulta
    {
        public SubConsulta()
        {
            listCamposFrom = new List<CamposFrom>();
            listCamposWhere = new List<CamposWhere>();
            listCamposGrupo = new List<CamposGrupo>();
            listCamposOrden = new List<CamposOrden>();
        }

        public SubConsulta(OperadorSelect operadorSelect, List<CamposFrom> listCamposFrom, List<CamposWhere> listCamposWhere, List<CamposGrupo> listCamposGrupo, List<CamposOrden> listCamposOrden, ValoresRango valoresTango)
        {
            this.operadorSelect = operadorSelect;
            this.listCamposFrom = listCamposFrom == null ? new List<CamposFrom>() : listCamposFrom;
            this.listCamposWhere = listCamposWhere == null ? new List<CamposWhere>() : listCamposWhere;
            this.listCamposGrupo = listCamposGrupo == null ? new List<CamposGrupo>() : listCamposGrupo;
            this.listCamposOrden = listCamposOrden == null ? new List<CamposOrden>() : listCamposOrden;
        }

        public OperadorSelect operadorSelect { get; set; }

        public List<CamposFrom> listCamposFrom { get; set; }

        public List<CamposWhere> listCamposWhere { get; set; }

        public List<CamposGrupo> listCamposGrupo { get; set; }

        public List<CamposOrden> listCamposOrden { get; set; }

        public ValoresRango valoresRango { get; set; }

    }
}
