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

    
    public partial class BaseAfecConcepNom {
    
       
        public BaseAfecConcepNom()
        {
            this.movNomBaseAfecta = new List<MovNomBaseAfecta>();
        
        }

        public virtual decimal id
        {
            get;
            set;
        }

    
        public virtual string formulaExenta
        {
            get;
            set;
        }
 
        public virtual string periodoExentoISR
        {
            get;
            set;
        }

        public virtual int tipoAfecta
        {
            get;
            set;
        }

        public virtual BaseNomina baseNomina
        {
            get;
            set;
        }
  
        public virtual ConcepNomDefi concepNomDefi
        {
            get;
            set;
        }

        public virtual IList<MovNomBaseAfecta> movNomBaseAfecta
        {
            get;
            set;
        }
    }

}
