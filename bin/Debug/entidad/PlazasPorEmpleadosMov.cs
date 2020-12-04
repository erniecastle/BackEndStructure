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

    public partial class PlazasPorEmpleadosMov
    {

        public PlazasPorEmpleadosMov()
        {
            this.cfdiEmpleado = new List<CFDIEmpleado>();
            this.inasistenciaPorHora = new List<InasistenciaPorHora>();

        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual bool cambioCentroDeCostos
        {
            get;
            set;
        }

        public virtual bool cambioDepartamento
        {
            get;
            set;
        }

        public virtual bool cambioHoras
        {
            get;
            set;
        }

        public virtual bool cambioPlazasPosOrganigrama
        {
            get;
            set;
        }

        public virtual bool cambioPuestos
        {
            get;
            set;
        }

        public virtual bool cambioRegimenContratacion
        {
            get;
            set;
        }

        public virtual bool cambioTipoSalario
        {
            get;
            set;
        }

        public virtual bool cambioSalario
        {
            get;
            set;
        }

        public virtual bool cambioTipoContrato
        {
            get;
            set;
        }

        public virtual bool cambioSindicalizado
        {
            get;
            set;
        }

        public virtual bool cambioJornada
        {
            get;
            set;
        }

        public virtual bool cambioTipoDeNomina
        {
            get;
            set;
        }

        public virtual bool cambioTipoRelacionLaboral
        {
            get;
            set;
        }

        public virtual bool cambioTurno
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaIMSS
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

        public virtual string regimenContratacion
        {
            get;
            set;
        }

        public virtual int? salarioPor
        {
            get;
            set;
        }

        public virtual int? tipoRelacionLaboral
        {
            get;
            set;
        }

        public virtual CentroDeCosto centroDeCosto
        {
            get;
            set;
        }

        public virtual IList<CFDIEmpleado> cfdiEmpleado
        {
            get;
            set;
        }

        public virtual Departamentos departamentos
        {
            get;
            set;
        }

        //public virtual FormasDePago formasDePago
        //{
        //    get;
        //    set;
        //}

        public virtual IList<InasistenciaPorHora> inasistenciaPorHora
        {
            get;
            set;
        }

        public virtual Plazas plazas
        {
            get;
            set;
        }

        public virtual PlazasPorEmpleado plazasPorEmpleado
        {
            get;
            set;
        }

        public virtual TipoContrato tipoContrato
        {
            get;
            set;
        }

        public virtual bool sindicalizado
        {
            get;
            set;
        }

        public virtual Jornada jornada
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNomina
        {
            get;
            set;
        }

        public virtual Turnos turnos
        {
            get;
            set;
        }

        public virtual Puestos puestos
        {
            get;
            set;
        }

        public virtual double sueldoDiario { get; set; }
    }

}
