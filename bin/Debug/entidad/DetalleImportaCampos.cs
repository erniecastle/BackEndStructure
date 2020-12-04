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

    public partial class DetalleImportaCampos
    {

        public DetalleImportaCampos()
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

        public virtual int noColumna
        {
            get;
            set;
        }

        public virtual int? orden
        {
            get;
            set;
        }

        public virtual decimal? importaCamposID
        {
            get;
            set;
        }
    }

}
