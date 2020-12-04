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

    public partial class ConfigFoliacion {

        public ConfigFoliacion()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual bool activarFoliacion
        {
            get;
            set;
        }

        public virtual bool activarPrefijo
        {
            get;
            set;
        }

        public virtual string campoClave
        {
            get;
            set;
        }

        public virtual int? lgnFolio
        {
            get;
            set;
        }

        public virtual int? lngPrefijo
        {
            get;
            set;
        }

        public virtual bool mostrarMsjFolioAjustado
        {
            get;
            set;
        }

        public virtual string prefijosValidos
        {
            get;
            set;
        }

        public virtual bool saltarFolios
        {
            get;
            set;
        }

        public virtual string tabla
        {
            get;
            set;
        }
    }

}
