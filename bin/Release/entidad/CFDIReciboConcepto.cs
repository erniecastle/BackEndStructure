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

    public partial class CFDIReciboConcepto {

        public CFDIReciboConcepto()
        {
           
        }

 
        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string claveConcepto
        {
            get;
            set;
        }

        public virtual string claveSAT
        {
            get;
            set;
        }

        public virtual string descripcionConcepto
        {
            get;
            set;
        }

        public virtual double importeExento
        {
            get;
            set;
        }

        public virtual double importeGravable
        {
            get;
            set;
        }

        public virtual bool otroPago
        {
            get;
            set;
        }

        public virtual string tipoNaturaleza
        {
            get;
            set;
        }

        public virtual CFDIRecibo cfdiRecibo
        {
            get;
            set;
        }
    }

}
