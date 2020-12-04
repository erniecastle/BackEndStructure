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

    public partial class ConfiguracionCapturas {

        public ConfiguracionCapturas()
        {
            this.detalleConfigCapturas = new List<DetalleConfigCapturas>();
           
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

        public virtual int tipoDeCaptura
        {
            get;
            set;
        }

        public virtual bool selectRegistros
        {
            get;
            set;
        }

        public virtual bool busquedaRegistros
        {
            get;
            set;
        }

        public virtual string fileForma1
        {
            get;
            set;
        }

        public virtual string fileForma2
        {
            get;
            set;
        }

        public virtual string fileForma3
        {
            get;
            set;
        }

        public virtual OrigenDatos origenDatos
        {
            get;
            set;
        }

        public virtual IList<DetalleConfigCapturas> detalleConfigCapturas
        {
            get;
            set;
        }
    }

}
