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


    public partial class CalculoIMSSPatron {
    
 
        public CalculoIMSSPatron()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double valorTasaAportacionInfonavitPatron
        {
            get;
            set;
        }

        public virtual double valorTasaAportacionRetiroPatron
        {
            get;
            set;
        }

        public virtual double valorTasaCesanVejezPatron
        {
            get;
            set;
        }

        public virtual double valorTasaExcedentePatron
        {
            get;
            set;
        }

        public virtual double valorTasaFijaPatron
        {
            get;
            set;
        }

        public virtual double valorTasaGastosPensPatron
        {
            get;
            set;
        }

        public virtual double valorTasaGuarderiaPatron
        {
            get;
            set;
        }

        public virtual double valorTasaInvaliVidaPatron
        {
            get;
            set;
        }

        public virtual double valorTasaPrestDinePatron
        {
            get;
            set;
        }

        public virtual double valorTasaRiesgosPatron
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
