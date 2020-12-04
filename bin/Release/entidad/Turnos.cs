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

    public partial class Turnos {

        public Turnos()
        {
            this.plazas = new List<Plazas>();
            this.plazasPorEmpleadosMov = new List<PlazasPorEmpleadosMov>();
            this.turnosHorariosFijos = new List<TurnosHorariosFijos>();
            
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

        public virtual int diasJornada
        {
            get;
            set;
        }

        public virtual double horaJornada
        {
            get;
            set;
        }

        public virtual int primerDiaSemana
        {
            get;
            set;
        }

        public virtual int tipoDeJornadaIMSS
        {
            get;
            set;
        }

        public virtual int tipoDeTurno
        {
            get;
            set;
        }

        public virtual int? topeHorasDoblesSemanal
        {
            get;
            set;
        }

        public virtual Jornada jornada
        {
            get;
            set;
        }

        public virtual IList<Plazas> plazas
        {
            get;
            set;
        }

        public virtual IList<PlazasPorEmpleadosMov> plazasPorEmpleadosMov
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual IList<TurnosHorariosFijos> turnosHorariosFijos
        {
            get;
            set;
        }
    }

}
