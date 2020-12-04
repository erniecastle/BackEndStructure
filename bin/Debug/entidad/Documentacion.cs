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

    public partial class Documentacion {

        public Documentacion()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual byte[] archivo
        {
            get;
            set;
        }

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual string documento
        {
            get;
            set;
        }

        public virtual bool entrego
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaDevolucion
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaEntrega
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }
    }

}
