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

    public partial class TurnosHorariosFijos {

        public TurnosHorariosFijos()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int diaSemana
        {
            get;
            set;
        }

        public virtual int? ordenDia
        {
            get;
            set;
        }

        public virtual int? statusDia
        {
            get;
            set;
        }

        public virtual Horario horario
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual Turnos turnos
        {
            get;
            set;
        }
    }

}
