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

    public partial class RegistroIncapacidad {

        public RegistroIncapacidad()
        {
            this.registroIncapacidads_incapacidadAnterior_ID = new List<RegistroIncapacidad>();
            
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

        public virtual int? controlIncapacidad
        {
            get;
            set;
        }

        public virtual int? diasIncapacidad
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaFinal
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaInicial
        {
            get;
            set;
        }

        public virtual bool pagarTresPrimeroDias
        {
            get;
            set;
        }

        public virtual int? porcentaje
        {
            get;
            set;
        }

        public virtual int? ramoSeguro
        {
            get;
            set;
        }

        public virtual int? secuelaConsecuencia
        {
            get;
            set;
        }

        public virtual int? tipoRiesgo
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual IList<RegistroIncapacidad> registroIncapacidads_incapacidadAnterior_ID
        {
            get;
            set;
        }

        public virtual RegistroIncapacidad registroIncapacidad_incapacidadAnterior_ID
        {
            get;
            set;
        }
    }

}
