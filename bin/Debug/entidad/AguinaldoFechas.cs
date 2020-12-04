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

    public partial class AguinaldoFechas {
    

        public AguinaldoFechas()
        {
   
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int ejercicio
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaProgramada
        {
            get;
            set;
        }

    
        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }
    }

}
