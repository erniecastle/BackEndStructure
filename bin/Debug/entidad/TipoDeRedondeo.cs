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

    public partial class TipoDeRedondeo {

        public TipoDeRedondeo()
        {
            this.datosTipoValor = new List<DatosTipoValor>();
            
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

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual string tipoDeValor
        {
            get;
            set;
        }

        public virtual IList<DatosTipoValor> datosTipoValor
        {
            get;
            set;
        }
    }

}
