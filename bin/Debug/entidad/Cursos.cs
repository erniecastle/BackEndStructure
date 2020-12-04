/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Macropro
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class Cursos {
    
        public Cursos()
        {
            this.capacitaciones = new List<Capacitaciones>();
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
  
        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual IList<Capacitaciones> capacitaciones
        {
            get;
            set;
        }
    }

}
