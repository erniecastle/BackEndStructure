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

    public partial class ReporteOtrosDatosEncabezado {

        public ReporteOtrosDatosEncabezado()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string datos
        {
            get;
            set;
        }

        public virtual byte[] logo
        {
            get;
            set;
        }

        public virtual string otrasPropiedades
        {
            get;
            set;
        }

        public virtual ReporteCamposEncabezado reporteCamposEncabezado
        {
            get;
            set;
        }
    }

}
