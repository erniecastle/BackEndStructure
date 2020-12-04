/**
* @author: Ernesto Castillo
* Fecha de Creación: 09/04/2019
* Compañía: Macropro
* Descripción del programa: clase Ciudades para llamados a metodos de Hibernate
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
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

    public partial class Ciudades {
    
        public Ciudades()
        {
            this.centroDeCosto = new List<CentroDeCosto>();
            this.cp = new List<Cp>();
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

    
    
        public virtual Municipios municipios
        {
            get;
            set;
        }

    
    
        public virtual IList<Cp> cp
        {
            get;
            set;
        }

    
  
        public virtual IList<Empleados> empleados
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
