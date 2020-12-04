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

    public partial class Modulo {

        public Modulo()
        {
            this.parametros = new List<Parametros>();
            
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual Sistemas sistemas
        {
            get;
            set;
        }

        public virtual IList<Parametros> parametros
        {
            get;
            set;
        }
    }

}
