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

    public partial class VacacionesDisfrutadas {

        public VacacionesDisfrutadas()
        {
            this.vacacionesAplicacion = new List<VacacionesAplicacion>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double? diasPrimaDisfrutados
        {
            get;
            set;
        }

        public virtual int? diasVacDisfrutados
        {
            get;
            set;
        }

        public virtual int? ejercicioAplicacion
        {
            get;
            set;
        }

        public virtual string observaciones
        {
            get;
            set;
        }

        public virtual bool pagarPrimaVacacional
        {
            get;
            set;
        }

        public virtual bool pagarVacaciones
        {
            get;
            set;
        }

        public virtual bool registroInicial
        {
            get;
            set;
        }

        public virtual System.DateTime? regresoVac
        {
            get;
            set;
        }

        public virtual System.DateTime? salidaVacac
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaPago
        {
            get;
            set;
        }

        public virtual int statusVacaciones
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual PeriodosNomina periodosNomina_periodoAplicacion_ID
        {
            get;
            set;
        }

       

        public virtual PeriodosNomina periodosNomina_periodoPago_ID
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual TipoCorrida tipoCorridaAplicacion
        {
            get;
            set;
        }

        public virtual TipoCorrida tipoCorridaPago
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNominaAplicacion
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNominaPago
        {
            get;
            set;
        }

        public virtual TiposVacaciones tiposVacaciones
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
