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

    public partial class RazonesSociales {

        public RazonesSociales()
        {
            this.aguinaldoConfiguracion = new List<AguinaldoConfiguracion>();
            this.aguinaldoFechas = new List<AguinaldoFechas>();
            this.aguinaldoPagos = new List<AguinaldoPagos>();
            this.asistencias = new List<Asistencias>();
            this.calculoUnidades = new List<CalculoUnidades>();
            this.centroDeCosto = new List<CentroDeCosto>();
            this.cfdiEmpleado = new List<CFDIEmpleado>();
            this.configAsistencias = new List<ConfigAsistencias>();
            this.configuraMovimiento = new List<ConfiguraMovimiento>();
            this.controlVacDeveng = new List<ControlVacDeveng>();
            this.creditoAhorro = new List<CreditoAhorro>();
            this.creditoPorEmpleado = new List<CreditoPorEmpleado>();
            this.departamentos = new List<Departamentos>();
            this.detalleAsistencia = new List<DetalleAsistencia>();
            this.diasAguinaldo = new List<DiasAguinaldo>();
            this.empleados = new List<Empleados>();
            this.finiquitosLiquida = new List<FiniquitosLiquida>();
            this.firmas = new List<Firmas>();
            this.horario = new List<Horario>();
            this.ingresosBajas = new List<IngresosBajas>();
            this.movNomConcep = new List<MovNomConcep>();
            this.plazas = new List<Plazas>();
            this.plazasPorEmpleado = new List<PlazasPorEmpleado>();
            this.ptuDatosGenerales = new List<PtuDatosGenerales>();
            this.ptuEmpleados = new List<PtuEmpleados>();
            this.registroPatronal = new List<RegistroPatronal>();
            this.tipoCentroCostos = new List<TipoCentroCostos>();
            this.turnos = new List<Turnos>();
            this.turnosHorariosFijos = new List<TurnosHorariosFijos>();
            this.vacacionesDevengadas = new List<VacacionesDevengadas>();
            this.vacacionesDisfrutadas = new List<VacacionesDisfrutadas>();
           
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string calle
        {
            get;
            set;
        }

        public virtual byte[] certificadoSAT
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string colonia
        {
            get;
            set;
        }

        public virtual string descripcionRecibo
        {
            get;
            set;
        }

        public virtual string folio
        {
            get;
            set;
        }

        public virtual byte[] llaveSAT
        {
            get;
            set;
        }

        public virtual string numeroex
        {
            get;
            set;
        }

        public virtual string numeroin
        {
            get;
            set;
        }

        public virtual string password
        {
            get;
            set;
        }

        public virtual string razonsocial
        {
            get;
            set;
        }

        public virtual string regimenFiscal
        {
            get;
            set;
        }

        public virtual string representantelegal
        {
            get;
            set;
        }

        public virtual string rfc
        {
            get;
            set;
        }

        public virtual string rutaCert
        {
            get;
            set;
        }

        public virtual string rutaLlave
        {
            get;
            set;
        }

        public virtual string telefono
        {
            get;
            set;
        }

        public virtual string ubicacionXML
        {
            get;
            set;
        }

        public virtual IList<AguinaldoConfiguracion> aguinaldoConfiguracion
        {
            get;
            set;
        }

        public virtual IList<AguinaldoFechas> aguinaldoFechas
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

        public virtual IList<CentroDeCosto> centroDeCosto
        {
            get;
            set;
        }

        public virtual IList<CFDIEmpleado> cfdiEmpleado
        {
            get;
            set;
        }

        public virtual Ciudades ciudades
        {
            get;
            set;
        }

        public virtual IList<ConfigAsistencias> configAsistencias
        {
            get;
            set;
        }

        public virtual IList<ConfiguraMovimiento> configuraMovimiento
        {
            get;
            set;
        }

        public virtual ConfiguraTimbrado configuraTimbrado
        {
            get;
            set;
        }

        public virtual IList<ControlVacDeveng> controlVacDeveng
        {
            get;
            set;
        }

        public virtual Cp cp
        {
            get;
            set;
        }

        public virtual IList<CreditoAhorro> creditoAhorro
        {
            get;
            set;
        }

        public virtual IList<CreditoPorEmpleado> creditoPorEmpleado
        {
            get;
            set;
        }

        public virtual IList<Departamentos> departamentos
        {
            get;
            set;
        }

        public virtual IList<DetalleAsistencia> detalleAsistencia
        {
            get;
            set;
        }


        public virtual IList<DiasAguinaldo> diasAguinaldo
        {
            get;
            set;
        }

        public virtual IList<Empleados> empleados
        {
            get;
            set;
        }

        public virtual Estados estados
        {
            get;
            set;
        }

        public virtual IList<FiniquitosLiquida> finiquitosLiquida
        {
            get;
            set;
        }

        public virtual IList<Firmas> firmas
        {
            get;
            set;
        }

        public virtual IList<Horario> horario
        {
            get;
            set;
        }

        public virtual IList<IngresosBajas> ingresosBajas
        {
            get;
            set;
        }


        public virtual IList<MovNomConcep> movNomConcep
        {
            get;
            set;
        }

        public virtual Municipios municipios
        {
            get;
            set;
        }

        public virtual Paises paises
        {
            get;
            set;
        }

        public virtual IList<Plazas> plazas
        {
            get;
            set;
        }

        public virtual IList<PlazasPorEmpleado> plazasPorEmpleado
        {
            get;
            set;
        }

        public virtual IList<PtuDatosGenerales> ptuDatosGenerales
        {
            get;
            set;
        }

        public virtual IList<PtuEmpleados> ptuEmpleados
        {
            get;
            set;
        }


        public virtual IList<RegistroPatronal> registroPatronal
        {
            get;
            set;
        }

        public virtual IList<TipoCentroCostos> tipoCentroCostos
        {
            get;
            set;
        }

        public virtual IList<Turnos> turnos
        {
            get;
            set;
        }

        public virtual IList<TurnosHorariosFijos> turnosHorariosFijos
        {
            get;
            set;
        }

        public virtual IList<VacacionesDevengadas> vacacionesDevengadas
        {
            get;
            set;
        }

        public virtual IList<VacacionesDisfrutadas> vacacionesDisfrutadas
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
