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

    public partial class RazonSocial {

        public RazonSocial()
        {
            this.contenedor = new List<Contenedor>();
            this.razonSocialConfiguracion = new List<RazonSocialConfiguracion>();
            this.reporteDinamico = new List<ReporteDinamico>();
            
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string claveRazonSocial
        {
            get;
            set;
        }

        public virtual string nombreRazonSocial
        {
            get;
            set;
        }

        public virtual IList<Contenedor> contenedor
        {
            get;
            set;
        }

        public virtual IList<RazonSocialConfiguracion> razonSocialConfiguracion
        {
            get;
            set;
        }

        public virtual IList<ReporteDinamico> reporteDinamico
        {
            get;
            set;
        }
    }

}
