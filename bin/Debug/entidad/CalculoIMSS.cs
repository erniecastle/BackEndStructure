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

    public partial class CalculoIMSS {
 
        public CalculoIMSS()
        {
           
        }

  
        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double? diasCotizados
        {
            get;
            set;
        }

        public virtual double valorCesantiaVejez
        {
            get;
            set;
        }

        public virtual double valorDineEnfermeMater
        {
            get;
            set;
        }

        public virtual double valorEspecieEnfermeMater
        {
            get;
            set;
        }

        public virtual double valorGastosPension
        {
            get;
            set;
        }

        public virtual double valorInvalidezVida
        {
            get;
            set;
        }

         public virtual ConfiguracionIMSS configuracionIMSS
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
