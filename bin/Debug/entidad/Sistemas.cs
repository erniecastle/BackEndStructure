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

    public partial class Sistemas {

        public Sistemas()
        {
            this.modulo = new List<Modulo>();
            this.ventana = new List<Ventana>();
            
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual IList<Modulo> modulo
        {
            get;
            set;
        }

        public virtual IList<Ventana> ventana
        {
            get;
            set;
        }
    }

}
