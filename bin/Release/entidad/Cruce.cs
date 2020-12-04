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

    public partial class Cruce {

        public Cruce()
        {
           
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string claveElemento
        {
            get;
            set;
        }

        public virtual byte[] imagen
        {
            get;
            set;
        }

        public virtual int ordenId
        {
            get;
            set;
        }

        public virtual string valor
        {
            get;
            set;
        }

        public virtual ElementosAplicacion elementosAplicacion
        {
            get;
            set;
        }

        public virtual Parametros parametros
        {
            get;
            set;
        }
    }

}
