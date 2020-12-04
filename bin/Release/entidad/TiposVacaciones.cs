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

    public partial class TiposVacaciones {

        public TiposVacaciones()
        {
            this.calculoUnidades = new List<CalculoUnidades>();
            this.vacacionesDisfrutadas = new List<VacacionesDisfrutadas>();
            
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

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual IList<CalculoUnidades> calculoUnidades
        {
            get;
            set;
        }

        public virtual IList<VacacionesDisfrutadas> vacacionesDisfrutadas
        {
            get;
            set;
        }
    }

}
