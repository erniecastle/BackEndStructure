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

    public partial class CreditoMovimientos
    {

        public CreditoMovimientos()
        {
            this.movNomConcep = new List<MovNomConcep>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual System.DateTime fecha
        {
            get;
            set;
        }

        public virtual double? importe
        {
            get;
            set;
        }

        public virtual int? numeroPeriodosBloquear
        {
            get;
            set;
        }

        public virtual string observaciones
        {
            get;
            set;
        }

        public virtual int tiposMovimiento
        {
            get;
            set;
        }

        public virtual CreditoPorEmpleado creditoPorEmpleado
        {
            get;
            set;
        }

        public virtual PeriodosNomina periodosNomina
        {
            get;
            set;
        }

        public virtual List<MovNomConcep> movNomConcep
        {
            get;
            set;
        }
    }

}
