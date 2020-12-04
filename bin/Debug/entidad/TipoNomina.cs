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

    public partial class TipoNomina {

        public TipoNomina()
        {
            this.aguinaldoPagos = new List<AguinaldoPagos>();
            this.asistencias = new List<Asistencias>();
            this.calculoUnidades = new List<CalculoUnidades>();
            this.cfdiEmpleado = new List<CFDIEmpleado>();
            this.detalleAsistencia = new List<DetalleAsistencia>();
            this.inasistenciaPorHora = new List<InasistenciaPorHora>();
            this.movNomConcep = new List<MovNomConcep>();
            this.periodosNomina = new List<PeriodosNomina>();
            this.plazas = new List<Plazas>();
            this.plazasPorEmpleadosMov = new List<PlazasPorEmpleadosMov>();
            this.ptuEmpleados_tipoNominaPtuDias_ID = new List<PtuEmpleados>();
            this.ptuEmpleados_tipoNominaPtuPercep_ID = new List<PtuEmpleados>();
            this.semaforoCalculoNomina = new List<SemaforoCalculoNomina>();
            this.semaforoTimbradoPac = new List<SemaforoTimbradoPac>();
            this.VacacionesDis_NominaApli = new List<VacacionesDisfrutadas>();
            this.VacacionesDis_NominaPago = new List<VacacionesDisfrutadas>();

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

        public virtual string folio
        {
            get;
            set;
        }

        public virtual int tipoReporte
        {
            get;
            set;
        }

        public virtual IList<AguinaldoPagos> aguinaldoPagos
        {
            get;
            set;
        }

        public virtual IList<Asistencias> asistencias
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

        public virtual IList<DetalleAsistencia> detalleAsistencia
        {
            get;
            set;
        }

        public virtual IList<InasistenciaPorHora> inasistenciaPorHora
        {
            get;
            set;
        }

        public virtual IList<MovNomConcep> movNomConcep
        {
            get;
            set;
        }

        public virtual Periodicidad periodicidad
        {
            get;
            set;
        }

        public virtual IList<PeriodosNomina> periodosNomina
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

        public virtual IList<PtuEmpleados> ptuEmpleados_tipoNominaPtuDias_ID
        {
            get;
            set;
        }

        public virtual IList<PtuEmpleados> ptuEmpleados_tipoNominaPtuPercep_ID
        {
            get;
            set;
        }

        public virtual RegistroPatronal registroPatronal
        {
            get;
            set;
        }

        public virtual IList<SemaforoCalculoNomina> semaforoCalculoNomina
        {
            get;
            set;
        }

        public virtual IList<SemaforoTimbradoPac> semaforoTimbradoPac
        {
            get;
            set;
        }

        public virtual IList<VacacionesDisfrutadas> VacacionesDis_NominaApli
        {
            get;
            set;
        }

        public virtual IList<VacacionesDisfrutadas> VacacionesDis_NominaPago
        {
            get;
            set;
        }

        public virtual Series series
        {
            get;
            set;
        }

    }

}
