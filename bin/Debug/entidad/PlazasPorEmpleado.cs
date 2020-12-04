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

    public partial class PlazasPorEmpleado
    {

        public PlazasPorEmpleado()
        {
            this.calculoUnidades = new List<CalculoUnidades>();
            this.finiqLiquidPlazas = new List<FiniqLiquidPlazas>();
          //this.ingresosBajas = new List<IngresosBajas>();
            this.movNomConcep = new List<MovNomConcep>();
            this.plazasPorEmpleados_reIngreso_ID = new List<PlazasPorEmpleado>();
            this.plazasPorEmpleadosMov = new List<PlazasPorEmpleadosMov>();
            this.salariosIntegradosDet = new List<SalariosIntegradosDet>();
            

        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string referencia
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaFinal
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaPrestaciones
        {
            get;
            set;
        }

        public virtual bool plazaPrincipal
        {
            get;
            set;
        }

        public virtual IList<CalculoUnidades> calculoUnidades
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual IList<FiniqLiquidPlazas> finiqLiquidPlazas
        {
            get;
            set;
        }


        public virtual IngresosBajas ingresosBajas
        {
            get;
            set;
        }

        public virtual IList<MovNomConcep> movNomConcep
        {
            get;
            set;
        }

        public virtual IList<PlazasPorEmpleado> plazasPorEmpleados_reIngreso_ID
        {
            get;
            set;
        }

        public virtual PlazasPorEmpleado plazasPorEmpleado_reIngreso_ID
        {
            get;
            set;
        }

        public virtual RegistroPatronal registroPatronal
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual IList<PlazasPorEmpleadosMov> plazasPorEmpleadosMov
        {
            get;
            set;
        }

        public virtual IList<SalariosIntegradosDet> salariosIntegradosDet
        {
            get;
            set;
        }

      
        public virtual bool status
        {
            get;
            set;
        }

    }
}
