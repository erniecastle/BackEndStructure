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

    public partial class ConfiguracionNivelCuenta {
  
        public ConfiguracionNivelCuenta()
        {
            this.estrucCuenta = new List<EstrucCuenta>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual bool activarCxnContable
        {
            get;
            set;
        }

        public virtual int? orgPoliza
        {
            get;
            set;
        }

        public virtual string rutaArchivoPolizas
        {
            get;
            set;
        }

        public virtual IList<EstrucCuenta> estrucCuenta
        {
            get;
            set;
        }
    }

}
