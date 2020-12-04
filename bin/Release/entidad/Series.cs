using System.Collections.Generic;


namespace Exitosw.Payroll.Hibernate.entidad
{
    public class Series
    {
        public Series()
        {
            this.tipoNomina = new List<TipoNomina>();
            this.razonesSociales = new List<RazonesSociales>();
            this.registroPatronal = new List<RegistroPatronal>();
        }
        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string serie
        {
            get;
            set;
        }

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual int longitudFolio
        {
            get;
            set;
        }

        public virtual int folioInicial
        {
            get;
            set;
        }

        public virtual int limiteAdvertencia
        {
            get;
            set;
        }

        public virtual IList<TipoNomina> tipoNomina
        {
            get;
            set;
        }

        public virtual IList<RazonesSociales> razonesSociales
        {
            get;
            set;
        }

        public virtual IList<RegistroPatronal> registroPatronal
        {
            get;
            set;
        }

    }
}
