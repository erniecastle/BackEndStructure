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

    public partial class CalculoISR {
    
 
        public CalculoISR()
        {
          
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double isrACargoAnual
        {
            get;
            set;
        }

        public virtual double isrACargoDirecto
        {
            get;
            set;
        }

        public virtual double isrACargoNormal
        {
            get;
            set;
        }

        public virtual double isrNetoAnual
        {
            get;
            set;
        }

        public virtual double isrNetoDirecto
        {
            get;
            set;
        }

        public virtual double isrNetoNormal
        {
            get;
            set;
        }

        public virtual double? isrRetenidoAnual
        {
            get;
            set;
        }

        public virtual double? isrRetenidoDirecto
        {
            get;
            set;
        }

        public virtual double? isrRetenidoNormal
        {
            get;
            set;
        }

        public virtual double isrSubsidioAnual
        {
            get;
            set;
        }

        public virtual double isrSubsidioDirecto
        {
            get;
            set;
        }

        public virtual double isrSubsidioNormal
        {
            get;
            set;
        }

        public virtual double subsidioEmpleoAnual
        {
            get;
            set;
        }

        public virtual double subsidioEmpleoDirecto
        {
            get;
            set;
        }

        public virtual double subsidioEmpleoNormal
        {
            get;
            set;
        }

        public virtual MovNomConcep movNomConcep
        {
            get;
            set;
        }
    }

}
