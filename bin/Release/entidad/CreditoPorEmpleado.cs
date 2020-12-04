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

    public partial class CreditoPorEmpleado {

        public CreditoPorEmpleado()
        {
            this.creditoMovimientos = new List<CreditoMovimientos>();
           
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string cuentaContable
        {
            get;
            set;
        }

    
        public virtual System.DateTime? fechaAutorizacion
        {
            get;
            set;
        }

        public virtual System.DateTime fechaCredito
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaVence
        {
            get;
            set;
        }

        public virtual System.DateTime? inicioDescuento
        {
            get;
            set;
        }

        public virtual int? modoDescuentoCredito
        {
            get;
            set;
        }

        public virtual double montoDescuento
        {
            get;
            set;
        }

        public virtual string numeroCredito
        {
            get;
            set;
        }

        public virtual string numeroEmpleadoExtra
        {
            get;
            set;
        }

        public virtual int? numeroParcialidades
        {
            get;
            set;
        }

        public virtual double totalCredito
        {
            get;
            set;
        }

        public virtual CreditoAhorro creditoAhorro
        {
            get;
            set;
        }

        public virtual IList<CreditoMovimientos> creditoMovimientos
        {
            get;
            set;
        }

        public virtual PeriodosNomina periodosNomina
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
        public virtual double saldo { get; set; }
    }

}
