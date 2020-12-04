/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
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

    public partial class ConceptoDeNomina {
  
        public ConceptoDeNomina()
        {
            this.concepNomDefi = new List<ConcepNomDefi>();
            this.conceptosEspeciales = new List<ConceptosEspeciales>();
           
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

        public virtual IList<ConcepNomDefi> concepNomDefi
        {
            get;
            set;
        }

        public virtual IList<ConceptosEspeciales> conceptosEspeciales
        {
            get;
            set;
        }
    }

}
