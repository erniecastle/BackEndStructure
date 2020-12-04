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

    public partial class Despensa {

        public Despensa()
        {
            this.baseNomina = new List<BaseNomina>();
            this.incidencias = new List<Incidencias>();
            
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string condicionesPagos
        {
            get;
            set;
        }

        public virtual int? diasMes
        {
            get;
            set;
        }

        public virtual double? importeDias
        {
            get;
            set;
        }

        public virtual double? importeHoras
        {
            get;
            set;
        }

        public virtual int? pagoDias
        {
            get;
            set;
        }

        public virtual int? pagoHoras
        {
            get;
            set;
        }

        public virtual int? periodicidadPago
        {
            get;
            set;
        }

        public virtual int? retencionISR
        {
            get;
            set;
        }

        public virtual System.DateTime? vigencia
        {
            get;
            set;
        }

        public virtual IList<BaseNomina> baseNomina
        {
            get;
            set;
        }

        public virtual IList<Incidencias> incidencias
        {
            get;
            set;
        }
    }

}
