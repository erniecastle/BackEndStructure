/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

using System.Collections.Generic;

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class DatosTipoValor {

        public DatosTipoValor()
        {
            this.tipoDeRedondeo = new List<TipoDeRedondeo>();
           
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string desde
        {
            get;
            set;
        }

        public virtual string hasta
        {
            get;
            set;
        }

        public virtual string valor
        {
            get;
            set;
        }

        public virtual IList<TipoDeRedondeo> tipoDeRedondeo
        {
            get;
            set;
        }
    }

}
