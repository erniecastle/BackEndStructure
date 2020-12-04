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

    public partial class ConfiguracionIMSS {
 
        public ConfiguracionIMSS()
        {
            this.calculoIMSS = new List<CalculoIMSS>();
            this.calculoIMSSPatron = new List<CalculoIMSSPatron>();
           
        }


        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int tipoTasa
        {
            get;
            set;
        }

        public virtual double aportacionInfonavitPatron
        {
            get;
            set;
        }

        public virtual double cuotaFijaPatron
        {
            get;
            set;
        }

        public virtual double excedenteEspecie
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaAplica
        {
            get;
            set;
        }

        public virtual double tasaAportacionInfonavitPatron
        {
            get;
            set;
        }

        public virtual double tasaAportacionRetiroPatron
        {
            get;
            set;
        }

        public virtual double tasaCesanVejezPatron
        {
            get;
            set;
        }

        public virtual double tasaCesantiaVejez
        {
            get;
            set;
        }

        public virtual double tasaDineEnfermeMater
        {
            get;
            set;
        }

        public virtual double tasaEspecieEnfermeMater
        {
            get;
            set;
        }

        public virtual double tasaExcedentePatron
        {
            get;
            set;
        }

        public virtual double tasaFijaPatron
        {
            get;
            set;
        }

        public virtual double tasaGastosPensPatron
        {
            get;
            set;
        }

        public virtual double tasaGastosPension
        {
            get;
            set;
        }

        public virtual double tasaGuarderiaPatron
        {
            get;
            set;
        }

        public virtual double tasaInvaliVidaPatron
        {
            get;
            set;
        }

        public virtual double tasaInvalidezVida
        {
            get;
            set;
        }

        public virtual double tasaPrestDinePatron
        {
            get;
            set;
        }

        public virtual double tasaRiesgosPatron
        {
            get;
            set;
        }

        public virtual double topeCesanVejez
        {
            get;
            set;
        }

        public virtual double topeInvaliVida
        {
            get;
            set;
        }

        public virtual double topeEnfermedadMaternidad
        {
            get;
            set;
        }

        public virtual double topeInfonavit
        {
            get;
            set;
        }

        public virtual double topeRetiro
        {
            get;
            set;
        }

        public virtual double topeRiesgoTrabajoGuarderias
        {
            get;
            set;
        }

        public virtual IList<CalculoIMSS> calculoIMSS
        {
            get;
            set;
        }

        public virtual IList<CalculoIMSSPatron> calculoIMSSPatron
        {
            get;
            set;
        }
    }

}
