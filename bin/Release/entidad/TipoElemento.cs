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

    public partial class TipoElemento {

        public TipoElemento()
        {
            this.contenedor = new List<Contenedor>();
            
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual bool externo
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual IList<Contenedor> contenedor
        {
            get;
            set;
        }
    }

}
