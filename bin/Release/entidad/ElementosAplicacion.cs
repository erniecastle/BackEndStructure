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

    public partial class ElementosAplicacion {

        public ElementosAplicacion()
        {
            this.cruce = new List<Cruce>();
            this.parametros = new List<Parametros>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string entidad
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual int? ordenId
        {
            get;
            set;
        }

        public virtual int? parentId
        {
            get;
            set;
        }

        public virtual IList<Cruce> cruce
        {
            get;
            set;
        }

        public virtual IList<Parametros> parametros
        {
            get;
            set;
        }
    }

}
