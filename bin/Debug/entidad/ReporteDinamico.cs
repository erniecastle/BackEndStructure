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

    public partial class ReporteDinamico {

        public ReporteDinamico()
        {
            this.asignaTipoReporte = new List<AsignaTipoReporte>();
            this.reporteCamposEncabezado = new List<ReporteCamposEncabezado>();
            this.reporteCamposWhere = new List<ReporteCamposWhere>();
            this.reporteDatosIncluir = new List<ReporteDatosIncluir>();
            this.reporteDatosResumen = new List<ReporteDatosResumen>();
            this.reporteOrdenGrupo = new List<ReporteOrdenGrupo>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int? columnasXPagina
        {
            get;
            set;
        }

        public virtual int? orden
        {
            get;
            set;
        }

        public virtual int? cbbPosicionX
        {
            get;
            set;
        }

        public virtual int? cbbPosicionY
        {
            get;
            set;
        }

        public virtual int? cbbSizeImagen
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual bool cortarDatoDetalle
        {
            get;
            set;
        }

        public virtual bool cortarTituloDetalle
        {
            get;
            set;
        }

        public virtual int? espaciadoColumnas
        {
            get;
            set;
        }

        public virtual byte[] fondo
        {
            get;
            set;
        }

        public virtual int? fondoAncho
        {
            get;
            set;
        }

        public virtual int? fondoLargo
        {
            get;
            set;
        }

        public virtual int? fondoPosicionX
        {
            get;
            set;
        }

        public virtual int? fondoPosicionY
        {
            get;
            set;
        }

        public virtual bool incluirFechaActual
        {
            get;
            set;
        }

        public virtual bool incluirNoPagina
        {
            get;
            set;
        }

        public virtual bool incluirTotalGeneral
        {
            get;
            set;
        }

        public virtual int? margenDerecho
        {
            get;
            set;
        }

        public virtual int? margenInferior
        {
            get;
            set;
        }

        public virtual int? margenIzquierdo
        {
            get;
            set;
        }

        public virtual int? margenSuperior
        {
            get;
            set;
        }

        public virtual bool mostrarDetalleColumnas
        {
            get;
            set;
        }

        public virtual int? noFilasEncabezado
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual string nombreAbreviado
        {
            get;
            set;
        }

        public virtual bool ocultarColumnas
        {
            get;
            set;
        }

        public virtual bool orientacion
        {
            get;
            set;
        }

        public virtual byte[] reportFileXml
        {
            get;
            set;
        }

        public virtual byte[] subReportFileXml
        {
            get;
            set;
        }

        public virtual int tipoHoja
        {
            get;
            set;
        }

        public virtual bool usaCBB
        {
            get;
            set;
        }

        public virtual bool usaFiltroCorrida
        {
            get;
            set;
        }

        public virtual bool usaTodoAnchoPagina
        {
            get;
            set;
        }

        public virtual IList<AsignaTipoReporte> asignaTipoReporte
        {
            get;
            set;
        }

        public virtual Contenedor contenedor
        {
            get;
            set;
        }

        public virtual RazonSocial razonSocial
        {
            get;
            set;
        }

        public virtual IList<ReporteCamposEncabezado> reporteCamposEncabezado
        {
            get;
            set;
        }

        public virtual IList<ReporteCamposWhere> reporteCamposWhere
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

        public virtual ReporteFuenteDatos reporteFuenteDatos
        {
            get;
            set;
        }

        public virtual ReporteEstilos reporteEstilos_reporteEstiloNoPagina_ID
        {
            get;
            set;
        }

        public virtual ReporteEstilos reporteEstilos_reporteEstiloTotal_ID
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
