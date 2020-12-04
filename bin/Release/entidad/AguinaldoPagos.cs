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

    public partial class AguinaldoPagos {
    

        public AguinaldoPagos()
        {
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double? aguinaldo
        {
            get;
            set;
        }

    
        public virtual double? diasAguinaldos
        {
            get;
            set;
        }

        public virtual double? diasPagados
        {
            get;
            set;
        }

        public virtual int ejercicio
        {
            get;
            set;
        }

   
        public virtual double? isr
        {
            get;
            set;
        }

    
        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual PeriodosNomina periodosNomina
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNomina
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
