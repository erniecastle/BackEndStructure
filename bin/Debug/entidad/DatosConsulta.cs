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

    public partial class DatosConsulta {

        public DatosConsulta()
        {
            this.reporteDatosIncluir = new List<ReporteDatosIncluir>();
            this.reporteDatosResumen = new List<ReporteDatosResumen>();
            this.reporteOpcionGrupos = new List<ReporteOpcionGrupos>();
            this.reporteOrdenGrupo = new List<ReporteOrdenGrupo>();
           
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string camposCombinar
        {
            get;
            set;
        }

        public virtual string formato
        {
            get;
            set;
        }

        public virtual string nombreBD
        {
            get;
            set;
        }

        public virtual string nombreEtiqueta
        {
            get;
            set;
        }

        public virtual string nombreMostrar
        {
            get;
            set;
        }

        public virtual string tipoDato
        {
            get;
            set;
        }

        public virtual IList<ReporteDatosIncluir> reporteDatosIncluir
        {
            get;
            set;
        }

        public virtual IList<ReporteDatosResumen> reporteDatosResumen
        {
            get;
            set;
        }

        public virtual IList<ReporteOpcionGrupos> reporteOpcionGrupos
        {
            get;
            set;
        }

        public virtual IList<ReporteOrdenGrupo> reporteOrdenGrupo
        {
            get;
            set;
        }
    }

}
