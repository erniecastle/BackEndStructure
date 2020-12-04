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

    public partial class Plazas {

        public Plazas()
        {
            this.experienciaLaboralInterna = new List<ExperienciaLaboralInterna>();
            this.plazas_plazasSubordinadoA_ID = new List<Plazas>();
            this.plazasPorEmpleadosMov = new List<PlazasPorEmpleadosMov>();
           
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int? cantidadPlazas
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string datosAdicionales
        {
            get;
            set;
        }

        public virtual string funciones
        {
            get;
            set;
        }

        public virtual string habilidades
        {
            get;
            set;
        }

        public virtual double? horas
        {
            get;
            set;
        }

        public virtual double importe
        {
            get;
            set;
        }

        public virtual string observaciones
        {
            get;
            set;
        }

        public virtual string perfil
        {
            get;
            set;
        }

        public virtual int? salarioPor
        {
            get;
            set;
        }

        public virtual System.DateTime? salida
        {
            get;
            set;
        }

        public virtual int? tipoRelacionLaboral
        {
            get;
            set;
        }

        public virtual int? tipoSalario
        {
            get;
            set;
        }

        public virtual CategoriasPuestos categoriasPuestos
        {
            get;
            set;
        }

        public virtual CentroDeCosto centroDeCosto
        {
            get;
            set;
        }

        public virtual Departamentos departamentos
        {
            get;
            set;
        }

        public virtual IList<ExperienciaLaboralInterna> experienciaLaboralInterna
        {
            get;
            set;
        }

        public virtual IList<Plazas> plazas_plazasSubordinadoA_ID
        {
            get;
            set;
        }

        public virtual Plazas plaza_plazasSubordinadoA_ID
        {
            get;
            set;
        }

        public virtual TipoContrato tipoContrato
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
    }

}
