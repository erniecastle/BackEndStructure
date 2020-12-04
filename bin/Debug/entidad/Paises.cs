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

    public partial class Paises {

        public Paises()
        {
            this.centroDeCosto = new List<CentroDeCosto>();
            this.empleados_paises_ID = new List<Empleados>();
            this.empleados_paisOrigen_ID = new List<Empleados>();
            this.estados = new List<Estados>();
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

        public virtual string nacionalidad
        {
            get;
            set;
        }

        public virtual IList<CentroDeCosto> centroDeCosto
        {
            get;
            set;
        }

        public virtual IList<Empleados> empleados_paises_ID
        {
            get;
            set;
        }

        public virtual IList<Empleados> empleados_paisOrigen_ID
        {
            get;
            set;
        }

        public virtual IList<Estados> estados
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
