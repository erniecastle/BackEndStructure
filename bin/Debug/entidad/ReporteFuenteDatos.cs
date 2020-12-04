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

    public partial class ReporteFuenteDatos {

        public ReporteFuenteDatos()
        {
            this.parametrosConsulta = new List<ParametrosConsulta>();
            this.reporteDinamico = new List<ReporteDinamico>();
            
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

        public virtual string nombreEntidad
        {
            get;
            set;
        }

        public virtual int? orden
        {
            get;
            set;
        }

        public virtual bool usaFormulas
        {
            get;
            set;
        }

        public virtual IList<ParametrosConsulta> parametrosConsulta
        {
            get;
            set;
        }

        public virtual IList<ReporteDinamico> reporteDinamico
        {
            get;
            set;
        }
    }

}
