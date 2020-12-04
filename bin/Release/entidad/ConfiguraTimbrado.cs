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

    public partial class ConfiguraTimbrado {

        public ConfiguraTimbrado()
        {
            this.razonesSociales = new List<RazonesSociales>();
           
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string URL
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string contraseña
        {
            get;
            set;
        }

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual bool principal
        {
            get;
            set;
        }

        public virtual string usuario
        {
            get;
            set;
        }

        public virtual IList<RazonesSociales> razonesSociales
        {
            get;
            set;
        }
    }

}
