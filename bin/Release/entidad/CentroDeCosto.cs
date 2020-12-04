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

    public partial class CentroDeCosto {

        public CentroDeCosto()
        {
            this.asistencias = new List<Asistencias>();
            this.detalleAsistencia = new List<DetalleAsistencia>();
            this.experienciaLaboralInterna = new List<ExperienciaLaboralInterna>();
            this.movNomConcep = new List<MovNomConcep>();
            this.plazas = new List<Plazas>();
            this.plazasPorEmpleadosMov = new List<PlazasPorEmpleadosMov>();
           
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

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual string descripcionPrevia
        {
            get;
            set;
        }

        public virtual string numeroExterior
        {
            get;
            set;
        }

        public virtual string numeroInterior
        {
            get;
            set;
        }

        public virtual string subCuenta
        {
            get;
            set;
        }

        public virtual string telefono
        {
            get;
            set;
        }

        public virtual IList<Asistencias> asistencias
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

        public virtual RegistroPatronal registroPatronal
        {
            get;
            set;
        }

        public virtual Estados estados
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

        public virtual TipoCentroCostos tipoCentroCostos
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual IList<DetalleAsistencia> detalleAsistencia
        {
            get;
            set;
        }

        public virtual IList<ExperienciaLaboralInterna> experienciaLaboralInterna
        {
            get;
            set;
        }

        public virtual IList<MovNomConcep> movNomConcep
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
    }

}
