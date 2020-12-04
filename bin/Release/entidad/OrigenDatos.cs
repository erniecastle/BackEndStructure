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

    public partial class OrigenDatos {

        public OrigenDatos()
        {
            this.camposOrigenDatos = new List<CamposOrigenDatos>();
            this.configuracionCapturas = new List<ConfiguracionCapturas>();
            this.detalleConfigCapturas = new List<DetalleConfigCapturas>();
            this.origenDatosFuente = new List<OrigenDatos>();
            this.origenDatosPrincipal = new List<OrigenDatos>();
            
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

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual string origen
        {
            get;
            set;
        }

        public virtual string recurso
        {
            get;
            set;
        }

        public virtual bool estado
        {
            get;
            set;
        }

        public virtual IList<CamposOrigenDatos> camposOrigenDatos
        {
            get;
            set;
        }

        public virtual IList<ConfiguracionCapturas> configuracionCapturas
        {
            get;
            set;
        }

        public virtual IList<DetalleConfigCapturas> detalleConfigCapturas
        {
            get;
            set;
        }

        public virtual IList<OrigenDatos> origenDatosFuente
        {
            get;
            set;
        }

        public virtual IList<OrigenDatos> origenDatosPrincipal
        {
            get;
            set;
        }
    }

}
