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

    public partial class ReporteOrdenGrupo {

        public ReporteOrdenGrupo()
        {
            this.reporteOpcionGrupos = new List<ReporteOpcionGrupos>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual bool agrupar
        {
            get;
            set;
        }

        public virtual bool incluirEncabezado
        {
            get;
            set;
        }

        public virtual bool incluirPie
        {
            get;
            set;
        }

        public virtual int? orden
        {
            get;
            set;
        }

        public virtual DatosConsulta datosConsulta
        {
            get;
            set;
        }

        public virtual ReporteDinamico reporteDinamico
        {
            get;
            set;
        }

        public virtual ReporteEstilos reporteEstilos_reporteEstiloEncabezado_ID
        {
            get;
            set;
        }

        public virtual ReporteEstilos reporteEstilos_reporteEstiloGrupo_ID
        {
            get;
            set;
        }

        public virtual ReporteEstilos reporteEstilos_reporteEstiloPie_ID
        {
            get;
            set;
        }

        public virtual IList<ReporteOpcionGrupos> reporteOpcionGrupos
        {
            get;
            set;
        }
    }

}
