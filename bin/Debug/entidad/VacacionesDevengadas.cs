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

    public partial class VacacionesDevengadas {

        public VacacionesDevengadas()
        {
            this.vacacionesAplicacion = new List<VacacionesAplicacion>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double? diasPrimaVaca
        {
            get;
            set;
        }

        public virtual int? diasVacaciones
        {
            get;
            set;
        }

        public virtual int? ejercicio
        {
            get;
            set;
        }

        public virtual double? factorPrima
        {
            get;
            set;
        }

        public virtual bool registroInicial
        {
            get;
            set;
        }

        public virtual double? salarioAniversario
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual IList<VacacionesAplicacion> vacacionesAplicacion
        {
            get;
            set;
        }
    }

}
