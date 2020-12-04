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

    public partial class PeriodosNomina {

        public PeriodosNomina()
        {
            this.aguinaldoPagos = new List<AguinaldoPagos>();
            this.asistencias = new List<Asistencias>();
            this.calculoUnidades = new List<CalculoUnidades>();
            this.cfdiEmpleado = new List<CFDIEmpleado>();
            this.creditoMovimientos = new List<CreditoMovimientos>();
            this.creditoPorEmpleado = new List<CreditoPorEmpleado>();
            this.detalleAsistencia = new List<DetalleAsistencia>();
            this.inasistenciaPorHora = new List<InasistenciaPorHora>();
            this.movNomConcep = new List<MovNomConcep>();
            this.ptuEmpleados_periodoPtuDias_ID = new List<PtuEmpleados>();
            this.ptuEmpleados_periodoPtuPercep_ID = new List<PtuEmpleados>();
            this.semaforoCalculoNomina = new List<SemaforoCalculoNomina>();
            this.semaforoTimbradoPac = new List<SemaforoTimbradoPac>();
            this.vacacionesDisfrutadas_periodoAplicacion_ID = new List<VacacionesDisfrutadas>();
           
            this.vacacionesDisfrutadas_periodoPago_ID = new List<VacacionesDisfrutadas>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual System.DateTime? acumularAMes
        {
            get;
            set;
        }

        public virtual int año
        {
            get;
            set;
        }

        public virtual bool bloquear
        {
            get;
            set;
        }

        public virtual bool cierreMes
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string claveUsuario
        {
            get;
            set;
        }

        public virtual bool descontarAhorro
        {
            get;
            set;
        }

        public virtual bool descontarPrestamo
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

        public virtual int? diasIMSS
        {
            get;
            set;
        }

        public virtual int? diasPago
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaAsistenciInicial
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaAsistenciaFinal
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaCierre
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaFinal
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaInicial
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaModificado
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaPago
        {
            get;
            set;
        }

        public virtual bool incluirBajas
        {
            get;
            set;
        }

        public virtual bool origen
        {
            get;
            set;
        }

        public virtual bool soloPercepciones
        {
            get;
            set;
        }

        public virtual bool status
        {
            get;
            set;
        }

        public virtual int? tipoUso
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

        public virtual IList<CreditoMovimientos> creditoMovimientos
        {
            get;
            set;
        }

        public virtual IList<CreditoPorEmpleado> creditoPorEmpleado
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

        public virtual TipoCorrida tipoCorrida
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNomina
        {
            get;
            set;
        }

        public virtual IList<PtuEmpleados> ptuEmpleados_periodoPtuDias_ID
        {
            get;
            set;
        }

        public virtual IList<PtuEmpleados> ptuEmpleados_periodoPtuPercep_ID
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

        public virtual IList<VacacionesDisfrutadas> vacacionesDisfrutadas_periodoAplicacion_ID
        {
            get;
            set;
        }

     

        public virtual IList<VacacionesDisfrutadas> vacacionesDisfrutadas_periodoPago_ID
        {
            get;
            set;
        }
    }

}
