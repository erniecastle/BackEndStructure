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

    public partial class CFDIReciboIncapacidad {

        public CFDIReciboIncapacidad()
        {
           
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int diasIncapacidad
        {
            get;
            set;
        }

        public virtual double importeMonetario
        {
            get;
            set;
        }

        public virtual string tipoIncapacidad
        {
            get;
            set;
        }

        public virtual CFDIRecibo cfdiRecibo
        {
            get;
            set;
        }
    }

}
