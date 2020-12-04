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

    public partial class Horario {

        public Horario()
        {
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

        public virtual System.DateTime? horaEntrada
        {
            get;
            set;
        }

        public virtual System.DateTime? horaFinal1erCoffeBreak
        {
            get;
            set;
        }

        public virtual System.DateTime? horaFinal2doCoffeBreak
        {
            get;
            set;
        }

        public virtual System.DateTime? horaFinalComer
        {
            get;
            set;
        }

        public virtual System.DateTime? horaInicio1erCoffeBreak
        {
            get;
            set;
        }

        public virtual System.DateTime? horaInicio2doCoffeBreak
        {
            get;
            set;
        }

        public virtual System.DateTime? horaInicioComer
        {
            get;
            set;
        }

        public virtual System.DateTime? horaInicioHrsExtra
        {
            get;
            set;
        }

        public virtual System.DateTime? horaSalida
        {
            get;
            set;
        }

        public virtual double? tiempo1erCoffeBreak
        {
            get;
            set;
        }

        public virtual double? tiempo2doCoffeBreak
        {
            get;
            set;
        }

        public virtual double? tiempoComer
        {
            get;
            set;
        }

        public virtual double? topeDiarioHrsExtras
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
