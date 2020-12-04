using System.Collections.Generic;


namespace Exitosw.Payroll.Core.genericos.campos
{
    public class CamposWhere
    {
        public CamposWhere()
        {
            listCamposAgrupados = new List<CamposWhere>();
        }

        public CamposWhere(string campo, object valor, OperadorComparacion? operadorComparacion, OperadorLogico? operadorLogico)
        {
            this.campo = campo;
            this.valor = valor;
            this.operadorComparacion = operadorComparacion;
            this.operadorLogico = operadorLogico;
            listCamposAgrupados = new List<CamposWhere>();
        }

        public string campo { get; set; }

        public object valor { get; set; }

        public OperadorComparacion? operadorComparacion { get; set; }

        public OperadorLogico? operadorLogico { get; set; }

        public TipoFuncion? tipoFuncion { get; set; }

        //public Type tipoDato { get; set; }

        public bool formula { get; set; } = false;

        public SubConsulta subConsulta { get; set; }

        public List<CamposWhere> listCamposAgrupados { get; set; }  //usado para agrupar campos parentesis ()
    }
}
