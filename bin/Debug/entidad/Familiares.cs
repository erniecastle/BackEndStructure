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

    public partial class Familiares {

        public Familiares()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual bool dependiente
        {
            get;
            set;
        }

        public virtual bool empleado
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaNacimiento
        {
            get;
            set;
        }

        public virtual bool finado
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual bool sexo
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual Parentesco parentesco
        {
            get;
            set;
        }
    }

}
