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

    public partial class PtuDatosGenerales {

        public PtuDatosGenerales()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int ejercicio
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaCalculo
        {
            get;
            set;
        }

        public virtual bool nuevosCalculos
        {
            get;
            set;
        }

        public virtual double? ptuArepartir
        {
            get;
            set;
        }

        public virtual string status
        {
            get;
            set;
        }

        public virtual double? topeSalario
        {
            get;
            set;
        }

        public virtual double? totalDias
        {
            get;
            set;
        }

        public virtual double? totalDiasptu
        {
            get;
            set;
        }

        public virtual double? totalPercepciones
        {
            get;
            set;
        }

        public virtual double? totalPercepcionesptu
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }
    }

}
