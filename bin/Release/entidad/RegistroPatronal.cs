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

    public partial class RegistroPatronal {

        public RegistroPatronal()
        {
            this.centroDeCosto = new List<CentroDeCosto>();
            this.ingresosBajas = new List<IngresosBajas>();
            this.plazas = new List<Plazas>();
            this.plazasPorEmpleado = new List<PlazasPorEmpleado>();
            this.primas = new List<Primas>();
            this.puestos = new List<Puestos>();
            this.salariosIntegrados = new List<SalariosIntegrados>();
            this.tipoNomina = new List<TipoNomina>();
            
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

        public virtual string clavedelegacion
        {
            get;
            set;
        }

        public virtual string clavesubdelegacion
        {
            get;
            set;
        }

        public virtual string colonia
        {
            get;
            set;
        }

        public virtual bool convenio
        {
            get;
            set;
        }

        public virtual string correoelec
        {
            get;
            set;
        }

        public virtual string delegacion
        {
            get;
            set;
        }

        public virtual string fax
        {
            get;
            set;
        }

        public virtual string nombreregtpatronal
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

        public virtual string paginainter
        {
            get;
            set;
        }

        public virtual string registroPatronal
        {
            get;
            set;
        }

        public virtual string riesgoPuesto
        {
            get;
            set;
        }

        public virtual string subdelegacion
        {
            get;
            set;
        }

        public virtual string telefono
        {
            get;
            set;
        }

        public virtual IList<CentroDeCosto> centroDeCosto
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

        public virtual Estados estados
        {
            get;
            set;
        }

        public virtual IList<IngresosBajas> ingresosBajas
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

        public virtual IList<Primas> primas
        {
            get;
            set;
        }

        public virtual IList<Puestos> puestos
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual IList<SalariosIntegrados> salariosIntegrados
        {
            get;
            set;
        }

        public virtual IList<TipoNomina> tipoNomina
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
