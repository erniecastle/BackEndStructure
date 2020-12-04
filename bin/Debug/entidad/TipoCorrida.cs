/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

using System.Collections.Generic;

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class TipoCorrida {

        public TipoCorrida()
        {
            this.calculoUnidades = new List<CalculoUnidades>();
            this.cfdiEmpleado = new List<CFDIEmpleado>();
            this.conceptoPorTipoCorrida = new List<ConceptoPorTipoCorrida>();
            this.movNomConcep = new List<MovNomConcep>();
            this.periodosNomina = new List<PeriodosNomina>();
            this.ptuEmpleados_tipoCorridaPtuDias_ID = new List<PtuEmpleados>();
            this.ptuEmpleados_tipoCorridaPtuPercep_ID = new List<PtuEmpleados>();
            this.VacacionesDis_corridaApli = new List<VacacionesDisfrutadas>();
            this.VacacionesDis_corridaPago = new List<VacacionesDisfrutadas>();

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

        public virtual string detalleConceptoRecibo
        {
            get;
            set;
        }

        public virtual bool mostrarMenuCalc
        {
            get;
            set;
        }

        public virtual short orden
        {
            get;
            set;
        }

        public virtual bool sistema
        {
            get;
            set;
        }

        public virtual short tipoDeCalculo
        {
            get;
            set;
        }

        public virtual bool usaCorrPeriodica
        {
            get;
            set;
        }

        public virtual IList<CalculoUnidades> calculoUnidades
        {
            get;
            set;
        }

        public virtual IList<CFDIEmpleado> cfdiEmpleado
        {
            get;
            set;
        }

        public virtual IList<ConceptoPorTipoCorrida> conceptoPorTipoCorrida
        {
            get;
            set;
        }

        public virtual IList<MovNomConcep> movNomConcep
        {
            get;
            set;
        }

        public virtual IList<PeriodosNomina> periodosNomina
        {
            get;
            set;
        }

        public virtual IList<PtuEmpleados> ptuEmpleados_tipoCorridaPtuDias_ID
        {
            get;
            set;
        }

        public virtual IList<PtuEmpleados> ptuEmpleados_tipoCorridaPtuPercep_ID
        {
            get;
            set;
        }

        public virtual IList<VacacionesDisfrutadas> VacacionesDis_corridaApli
        {
            get;
            set;
        }

        public virtual IList<VacacionesDisfrutadas> VacacionesDis_corridaPago
        {
            get;
            set;
        }
    }

}
