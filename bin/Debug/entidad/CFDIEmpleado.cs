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

    public partial class CFDIEmpleado {
    
 
        public CFDIEmpleado()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string CLABE
        {
            get;
            set;
        }

        public virtual string CURP
        {
            get;
            set;
        }

        public virtual string RFC
        {
            get;
            set;
        }

        public virtual int? antiguedad
        {
            get;
            set;
        }

        public virtual string antiguedadYMD
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

        public virtual string calle
        {
            get;
            set;
        }

        public virtual string ciudad
        {
            get;
            set;
        }

        public virtual string claveBancoSat
        {
            get;
            set;
        }

        public virtual string codigoPostal
        {
            get;
            set;
        }

        public virtual string colonia
        {
            get;
            set;
        }

        public virtual string correoElectronico
        {
            get;
            set;
        }

        public virtual string cuentaBancaria
        {
            get;
            set;
        }

        public virtual string departamento
        {
            get;
            set;
        }

        public virtual string estado
        {
            get;
            set;
        }

        public virtual System.DateTime fechaFinalPago
        {
            get;
            set;
        }

        public virtual System.DateTime fechaInicioPago
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaInicioRelLaboral
        {
            get;
            set;
        }

        public virtual System.DateTime fechaPago
        {
            get;
            set;
        }

        public virtual string formaPago
        {
            get;
            set;
        }

        public virtual string jornada
        {
            get;
            set;
        }

        public virtual string municipio
        {
            get;
            set;
        }

        public virtual string noExterior
        {
            get;
            set;
        }

        public virtual string noInterior
        {
            get;
            set;
        }

        public virtual string noRegistroPatronal
        {
            get;
            set;
        }

        public virtual string noSeguroSocial
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual int numeroDiasPago
        {
            get;
            set;
        }

        public virtual string pais
        {
            get;
            set;
        }

        public virtual string periodiciadadPago
        {
            get;
            set;
        }

        public virtual string puesto
        {
            get;
            set;
        }

        public virtual string regimenContratacion
        {
            get;
            set;
        }

        public virtual string riesgoPuesto
        {
            get;
            set;
        }

        public virtual double? salBaseCotAport
        {
            get;
            set;
        }

        public virtual double? salIntIMSS
        {
            get;
            set;
        }

        public virtual string tipoContrato
        {
            get;
            set;
        }

        public virtual CFDIRecibo cfdiRecibo
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

        public virtual PlazasPorEmpleadosMov plazasPorEmpleadosMov
        {
            get;
            set;
        }

        public virtual PeriodosNomina periodosNomina
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
