/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 26/02/2018
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

    public partial class Contactos {
    
      
  
        public Contactos()
        {
       
        }

        public virtual decimal id
        {
            get;
            set;
        }


        public virtual string email
        {
            get;
            set;
        }

    
        public virtual string extencion_fax
        {
            get;
            set;
        }

    
        public virtual string extencion_telefono
        {
            get;
            set;
        }

    
        public virtual string fax
        {
            get;
            set;
        }

    
        public virtual string movil
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual string puesto
        {
            get;
            set;
        }

        public virtual string telefono
        {
            get;
            set;
        }

        public virtual Bancos bancos
        {
            get;
            set;
        }
    }

}
