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

    public partial class ReporteEstilos {

        public ReporteEstilos()
        {
            this.reporteCamposEncabezado = new List<ReporteCamposEncabezado>();
            this.reporteDatosIncluir_reporteEstiloDetalle_ID = new List<ReporteDatosIncluir>();
            this.reporteDatosIncluir_reporteEstiloTitulo_ID = new List<ReporteDatosIncluir>();
            this.reporteDatosResumen_reporteEstiloDetalle_ID = new List<ReporteDatosResumen>();
            this.reporteDatosResumen_reporteEstiloTitulo_ID = new List<ReporteDatosResumen>();
            this.reporteDinamico_reporteEstiloNoPagina_ID = new List<ReporteDinamico>();
            this.reporteDinamico_reporteEstiloTotal_ID = new List<ReporteDinamico>();
            this.reporteOpcionGrupos_reporteEstiloDetalle_ID = new List<ReporteOpcionGrupos>();
            this.reporteOpcionGrupos_reporteEstiloTitulo_ID = new List<ReporteOpcionGrupos>();
            this.reporteOrdenGrupo_reporteEstiloEncabezado_ID = new List<ReporteOrdenGrupo>();
            this.reporteOrdenGrupo_reporteEstiloGrupo_ID = new List<ReporteOrdenGrupo>();
            this.reporteOrdenGrupo_reporteEstiloPie_ID = new List<ReporteOrdenGrupo>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int? alineacion
        {
            get;
            set;
        }

        public virtual string bordes
        {
            get;
            set;
        }

        public virtual string font
        {
            get;
            set;
        }

        public virtual IList<ReporteCamposEncabezado> reporteCamposEncabezado
        {
            get;
            set;
        }

        public virtual IList<ReporteDatosIncluir> reporteDatosIncluir_reporteEstiloDetalle_ID
        {
            get;
            set;
        }

        public virtual IList<ReporteDatosIncluir> reporteDatosIncluir_reporteEstiloTitulo_ID
        {
            get;
            set;
        }

        public virtual IList<ReporteDatosResumen> reporteDatosResumen_reporteEstiloDetalle_ID
        {
            get;
            set;
        }

        public virtual IList<ReporteDatosResumen> reporteDatosResumen_reporteEstiloTitulo_ID
        {
            get;
            set;
        }

        public virtual IList<ReporteDinamico> reporteDinamico_reporteEstiloNoPagina_ID
        {
            get;
            set;
        }

        public virtual IList<ReporteDinamico> reporteDinamico_reporteEstiloTotal_ID
        {
            get;
            set;
        }

        public virtual IList<ReporteOpcionGrupos> reporteOpcionGrupos_reporteEstiloDetalle_ID
        {
            get;
            set;
        }

        public virtual IList<ReporteOpcionGrupos> reporteOpcionGrupos_reporteEstiloTitulo_ID
        {
            get;
            set;
        }

        public virtual IList<ReporteOrdenGrupo> reporteOrdenGrupo_reporteEstiloEncabezado_ID
        {
            get;
            set;
        }

    
        public virtual IList<ReporteOrdenGrupo> reporteOrdenGrupo_reporteEstiloGrupo_ID
        {
            get;
            set;
        }

        public virtual IList<ReporteOrdenGrupo> reporteOrdenGrupo_reporteEstiloPie_ID
        {
            get;
            set;
        }
    }

}
