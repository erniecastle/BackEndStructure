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

    public partial class Estados {

        public Estados()
        {
            this.centroDeCosto = new List<CentroDeCosto>();
            this.empleados_estadoNacimiento_ID = new List<Empleados>();
            this.empleados_estados_ID = new List<Empleados>();
            this.municipios = new List<Municipios>();
            this.razonesSociales = new List<RazonesSociales>();
            this.registroPatronal = new List<RegistroPatronal>();
           
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

        public virtual IList<CentroDeCosto> centroDeCosto
        {
            get;
            set;
        }

        public virtual IList<Empleados> empleados_estadoNacimiento_ID
        {
            get;
            set;
        }

        public virtual IList<Empleados> empleados_estados_ID
        {
            get;
            set;
        }

        public virtual Paises paises
        {
            get;
            set;
        }

        public virtual IList<Municipios> municipios
        {
            get;
            set;
        }

        public virtual IList<RazonesSociales> razonesSociales
        {
            get;
            set;
        }

        public virtual IList<RegistroPatronal> registroPatronal
        {
            get;
            set;
        }
    }

}
