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

    public partial class Empleados {

        public Empleados()
        {
            this.aguinaldoPagos = new List<AguinaldoPagos>();
            this.asistencias = new List<Asistencias>();
            this.calculoUnidades = new List<CalculoUnidades>();
            this.capacitaciones = new List<Capacitaciones>();
            this.creditoPorEmpleado = new List<CreditoPorEmpleado>();
            this.detalleAsistencia = new List<DetalleAsistencia>();
            this.documentacion = new List<Documentacion>();
            this.experienciaLaboralExterna = new List<ExperienciaLaboralExterna>();
            this.experienciaLaboralInterna = new List<ExperienciaLaboralInterna>();
            this.familiares = new List<Familiares>();
            this.finiquitosLiquida = new List<FiniquitosLiquida>();
            this.formacionAcademica = new List<FormacionAcademica>();
            this.ingresosBajas = new List<IngresosBajas>();
            this.movNomConcep = new List<MovNomConcep>();
            this.plazasPorEmpleado = new List<PlazasPorEmpleado>();
            this.ptuEmpleados = new List<PtuEmpleados>();
            this.registroIncapacidad = new List<RegistroIncapacidad>();
            this.salariosIntegrados = new List<SalariosIntegrados>();
            this.vacacionesDisfrutadas = new List<VacacionesDisfrutadas>();
            this.vacacionesDevengadas = new List<VacacionesDevengadas>();

        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string CURP
        {
            get;
            set;
        }

        public virtual string IMSS
        {
            get;
            set;
        }

        public virtual string RFC
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

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string clinicaIMSS
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

        public virtual string domicilio
        {
            get;
            set;
        }

        public virtual int? estadoCivil
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaIngresoEmpresa
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaNacimiento
        {
            get;
            set;
        }

        public virtual byte[] foto
        {
            get;
            set;
        }

        public virtual string lugarNacimiento
        {
            get;
            set;
        }

        public virtual string nacionalidad
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

        public virtual string numeroExt
        {
            get;
            set;
        }

        public virtual string numeroInt
        {
            get;
            set;
        }

        public virtual bool status
        {
            get;
            set;
        }

        public virtual string telefono
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

        public virtual IList<Capacitaciones> capacitaciones
        {
            get;
            set;
        }

        public virtual Ciudades ciudades
        {
            get;
            set;
        }

        public virtual Cp cp
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

        public virtual IList<Documentacion> documentacion
        {
            get;
            set;
        }

        public virtual Estados estado_estadoNacimiento_ID
        {
            get;
            set;
        }

        public virtual Municipios municipios
        {
            get;
            set;
        }

        public virtual Paises paise_paises_ID
        {
            get;
            set;
        }

        public virtual Paises paise_paisOrigen_ID
        {
            get;
            set;
        }

        public virtual Genero genero
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual Estados estado_estados_ID
        {
            get;
            set;
        }

        public virtual IList<ExperienciaLaboralExterna> experienciaLaboralExterna
        {
            get;
            set;
        }

        public virtual IList<ExperienciaLaboralInterna> experienciaLaboralInterna
        {
            get;
            set;
        }

        public virtual IList<Familiares> familiares
        {
            get;
            set;
        }

        public virtual IList<FiniquitosLiquida> finiquitosLiquida
        {
            get;
            set;
        }

        public virtual IList<FormacionAcademica> formacionAcademica
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

        public virtual IList<PlazasPorEmpleado> plazasPorEmpleado
        {
            get;
            set;
        }

        public virtual IList<PtuEmpleados> ptuEmpleados
        {
            get;
            set;
        }

        public virtual IList<RegistroIncapacidad> registroIncapacidad
        {
            get;
            set;
        }

        public virtual IList<SalariosIntegrados> salariosIntegrados
        {
            get;
            set;
        }

        public virtual IList<VacacionesDisfrutadas> vacacionesDisfrutadas
        {
            get;
            set;
        }

        public virtual IList<VacacionesDevengadas> vacacionesDevengadas
        {
            get;
            set;
        }

    }

}
