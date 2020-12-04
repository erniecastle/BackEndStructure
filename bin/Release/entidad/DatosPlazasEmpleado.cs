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

    public partial class DatosPlazasEmpleado {

        public DatosPlazasEmpleado()
        {
           
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string apellidoMaterno
        {
            get;
            set;
        }

        public virtual string apellidoPaterno
        {
            get;
            set;
        }

        public virtual string claveEmpleado
        {
            get;
            set;
        }

        public virtual string clavePlaza
        {
            get;
            set;
        }

        public virtual string clavePlazaEmpleado
        {
            get;
            set;
        }

        public virtual string descripcionCentroCosto
        {
            get;
            set;
        }

        public virtual string descripcionPuesto
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaFinUltReingreso
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaFinal
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaIngreso
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaInicial
        {
            get;
            set;
        }

        public virtual int? horas
        {
            get;
            set;
        }

        public virtual double? importe
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual string tipoNominaClave
        {
            get;
            set;
        }

        public virtual string tipoNominaDescripcion
        {
            get;
            set;
        }
    }

}
