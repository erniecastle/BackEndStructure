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

    public partial class PtuEmpleados {

        public PtuEmpleados()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string clasificacion
        {
            get;
            set;
        }

        public virtual double? diasLaborados
        {
            get;
            set;
        }

        public virtual int? ejercicio
        {
            get;
            set;
        }

        public virtual bool esDirectivo
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaBaja
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaIngreso
        {
            get;
            set;
        }

        public virtual string observaciones
        {
            get;
            set;
        }

        public virtual bool participa
        {
            get;
            set;
        }

        public virtual double? percepciones
        {
            get;
            set;
        }

        public virtual double? ptuDias
        {
            get;
            set;
        }

        public virtual double? ptuPercepciones
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual PeriodosNomina periodosNomina_periodoPtuDias_ID
        {
            get;
            set;
        }

        public virtual PeriodosNomina periodosNomina_periodoPtuPercep_ID
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNomina_tipoNominaPtuDias_ID
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNomina_tipoNominaPtuPercep_ID
        {
            get;
            set;
        }

        public virtual Puestos puestos
        {
            get;
            set;
        }

        public virtual TipoCorrida tipoCorrida_tipoCorridaPtuDias_ID
        {
            get;
            set;
        }

        public virtual TipoCorrida tipoCorrida_tipoCorridaPtuPercep_ID
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
