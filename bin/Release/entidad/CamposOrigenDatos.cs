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

    public partial class CamposOrigenDatos {
  
        public CamposOrigenDatos()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string campo
        {
            get;
            set;
        }

        public virtual bool estado
        {
            get;
            set;
        }

        public virtual bool llave
        {
            get;
            set;
        }

        public virtual bool requerido
        {
            get;
            set;
        }

        public virtual string idEtiqueta
        {
            get;
            set;
        }

        public virtual int tipoDeDato
        {
            get;
            set;
        }

        public virtual int compAncho
        {
            get;
            set;
        }

        public virtual string configuracionTipoCaptura
        {
            get;
            set;
        }

        public virtual OrigenDatos origenDatos
        {
            get;
            set;
        }
    }

}
