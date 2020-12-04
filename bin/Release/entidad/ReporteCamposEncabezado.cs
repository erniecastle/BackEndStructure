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

    public partial class ReporteCamposEncabezado {

        public ReporteCamposEncabezado()
        {
            this.reporteOtrosDatosEncabezado = new List<ReporteOtrosDatosEncabezado>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual bool autocompletar
        {
            get;
            set;
        }

        public virtual string camposCombinar
        {
            get;
            set;
        }

        public virtual int? fila
        {
            get;
            set;
        }

        public virtual int? orden
        {
            get;
            set;
        }

        public virtual int tipoEncabezado
        {
            get;
            set;
        }

        public virtual string titulo
        {
            get;
            set;
        }

        public virtual ReporteEstilos reporteEstilos
        {
            get;
            set;
        }

        public virtual ReporteDinamico reporteDinamico
        {
            get;
            set;
        }

        public virtual IList<ReporteOtrosDatosEncabezado> reporteOtrosDatosEncabezado
        {
            get;
            set;
        }
    }

}
