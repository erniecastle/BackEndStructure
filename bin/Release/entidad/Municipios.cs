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

    public partial class Municipios {

        public Municipios()
        {
            this.centroDeCosto = new List<CentroDeCosto>();
            this.ciudades = new List<Ciudades>();
            this.empleados = new List<Empleados>();
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

        public virtual IList<Ciudades> ciudades
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
