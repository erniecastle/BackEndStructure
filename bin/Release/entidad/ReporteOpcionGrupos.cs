﻿/**
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

    public partial class ReporteOpcionGrupos {

        public ReporteOpcionGrupos()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int? bandaGrupo
        {
            get;
            set;
        }

        public virtual int? columna
        {
            get;
            set;
        }

        public virtual string datosEspecialesConsulta
        {
            get;
            set;
        }

        public virtual int operacion
        {
            get;
            set;
        }

        public virtual int? orden
        {
            get;
            set;
        }

        public virtual bool repiteInformacion
        {
            get;
            set;
        }

        public virtual int? tamColumna
        {
            get;
            set;
        }

        public virtual DatosConsulta datosConsulta
        {
            get;
            set;
        }

        public virtual ReporteEstilos reporteEstilos_reporteEstiloDetalle_ID
        {
            get;
            set;
        }

        public virtual ReporteEstilos reporteEstilos_reporteEstiloTitulo_ID
        {
            get;
            set;
        }

        public virtual ReporteOrdenGrupo reporteOrdenGrupo
        {
            get;
            set;
        }
    }

}