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

   
    public partial class BaseAfectadaGrupo {
    
     
        public BaseAfectadaGrupo()
        {
           
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

        public virtual Grupo grupo
        {
            get;
            set;
        }

        public virtual BaseNomina baseNomina
        {
            get;
            set;
        }
    }

}
