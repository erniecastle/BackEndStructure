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

    public partial class Resultado {
 
        public Resultado()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string IMSS
        {
            get;
            set;
        }

        public virtual string RFCBanco
        {
            get;
            set;
        }

        public virtual string RFCRazonSocial
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

        public virtual string calleRazonSocial
        {
            get;
            set;
        }

        public virtual string claveBanco
        {
            get;
            set;
        }

        public virtual string claveCentroDeCosto
        {
            get;
            set;
        }

        public virtual string claveDepartamento
        {
            get;
            set;
        }

        public virtual string claveEmpleado
        {
            get;
            set;
        }

        public virtual string clavePeriodosNomina
        {
            get;
            set;
        }

        public virtual string clavePuesto
        {
            get;
            set;
        }

        public virtual string claveRegistrotPatronal
        {
            get;
            set;
        }

        public virtual string claveTipoContrato
        {
            get;
            set;
        }

        public virtual string claveTipoNomina
        {
            get;
            set;
        }

        public virtual string claveTurno
        {
            get;
            set;
        }

        public virtual string colonia
        {
            get;
            set;
        }

        public virtual string coloniaRazonSocial
        {
            get;
            set;
        }

        public virtual string condicionConcepto
        {
            get;
            set;
        }

        public virtual string cp
        {
            get;
            set;
        }

        public virtual string cpRazonSocial
        {
            get;
            set;
        }

        public virtual string cuentaBancaria
        {
            get;
            set;
        }

        public virtual string curp
        {
            get;
            set;
        }

        public virtual string descripcionAbreviadaConceptoNomina
        {
            get;
            set;
        }

        public virtual string descripcionBanco
        {
            get;
            set;
        }

        public virtual string descripcionCentroDeCosto
        {
            get;
            set;
        }

        public virtual string descripcionConceptoNomina
        {
            get;
            set;
        }

        public virtual string descripcionDepartamento
        {
            get;
            set;
        }

        public virtual string descripcionPreviaCentroDeCosto
        {
            get;
            set;
        }

        public virtual string descripcionPreviaPuesto
        {
            get;
            set;
        }

        public virtual string descripcionPuesto
        {
            get;
            set;
        }

        public virtual string descripcionTipoContrato
        {
            get;
            set;
        }

        public virtual string descripcionTurno
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

        public virtual string domicilio
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaCierre
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaCierrePeriodo
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaFinalPeriodo
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaIMSS
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

        public virtual System.DateTime? fechaInicialPeriodo
        {
            get;
            set;
        }

        public virtual string formaDePago
        {
            get;
            set;
        }

        public virtual string formulaConcepto
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

        public virtual double? importePlazaPorEmplMov
        {
            get;
            set;
        }

        public virtual string indicativoGravadoExento
        {
            get;
            set;
        }

        public virtual int? naturaleza
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual string nombreAbreviado
        {
            get;
            set;
        }

        public virtual string nombrePeriodosNomina
        {
            get;
            set;
        }

        public virtual string nombreRegistrotPatronal
        {
            get;
            set;
        }

        public virtual string nombreTipoNomina
        {
            get;
            set;
        }

        public virtual string numeroConceptoNomina
        {
            get;
            set;
        }

        public virtual string numeroExRazonSocial
        {
            get;
            set;
        }

        public virtual string numeroExt
        {
            get;
            set;
        }

        public virtual string numeroInRazonSocial
        {
            get;
            set;
        }

        public virtual string numeroInt
        {
            get;
            set;
        }

        public virtual string plazas
        {
            get;
            set;
        }

        public virtual string razonsocial
        {
            get;
            set;
        }

        public virtual string registroPatronal
        {
            get;
            set;
        }

        public virtual string rfc
        {
            get;
            set;
        }

        public virtual double? salarioDiario
        {
            get;
            set;
        }

        public virtual double? salarioDiarioIntegrado
        {
            get;
            set;
        }

        public virtual int? salarioPor
        {
            get;
            set;
        }

        public virtual bool sindicalizado
        {
            get;
            set;
        }

        public virtual string subCuentaConcepto
        {
            get;
            set;
        }

        public virtual string subCuentaDepartamento
        {
            get;
            set;
        }

        public virtual string telefono
        {
            get;
            set;
        }

        public virtual int? tipoRelacionLaboral
        {
            get;
            set;
        }
    }

}
