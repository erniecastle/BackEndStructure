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

    public partial class DatosDisponiblesCxnConta {

        public DatosDisponiblesCxnConta()
        {
            this.formatosCnxContaDet = new List<FormatosCnxContaDet>();
           
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

        public virtual IList<FormatosCnxContaDet> formatosCnxContaDet
        {
            get;
            set;
        }
    }

}
