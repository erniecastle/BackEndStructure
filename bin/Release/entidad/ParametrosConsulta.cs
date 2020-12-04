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

    public partial class ParametrosConsulta {

        public ParametrosConsulta()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string addSeparadorGrupos
        {
            get;
            set;
        }

        public virtual string autoCompletarGrupos
        {
            get;
            set;
        }

        public virtual string camposAgrupar
        {
            get;
            set;
        }

        public virtual string camposMostrar
        {
            get;
            set;
        }

        public virtual string camposTotalizar
        {
            get;
            set;
        }

        public virtual string camposWhereExtras
        {
            get;
            set;
        }

        public virtual string datosEspecialesConsulta
        {
            get;
            set;
        }

        public virtual bool modoVisualizarTabla
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

        public virtual string sizeColumnas
        {
            get;
            set;
        }

        public virtual string tipoFormatoCamposMostrar
        {
            get;
            set;
        }

        public virtual string tipoFormatoTotales
        {
            get;
            set;
        }

        public virtual string tipoOrdenado
        {
            get;
            set;
        }

        public virtual string tituloCamposVisualizar
        {
            get;
            set;
        }

        public virtual string tituloGrupoVisualizar
        {
            get;
            set;
        }

        public virtual string tituloTotalVisualizar
        {
            get;
            set;
        }

        public virtual bool totalGlobal
        {
            get;
            set;
        }

        public virtual string totalizarGrupos
        {
            get;
            set;
        }

        public virtual bool usaFiltroCorrida
        {
            get;
            set;
        }

        public virtual Contenedor contenedor
        {
            get;
            set;
        }

        public virtual ReporteFuenteDatos reporteFuenteDatos
        {
            get;
            set;
        }
    }

}
