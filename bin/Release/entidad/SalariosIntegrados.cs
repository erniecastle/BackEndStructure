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

    public partial class SalariosIntegrados {
 
        public SalariosIntegrados()
        {
            this.salariosIntegradosDet = new List<SalariosIntegradosDet>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double factorIntegracion
        {
            get;
            set;
        }

        public virtual System.DateTime? fecha
        {
            get;
            set;
        }
        
        public virtual double salarioDiarioFijo
        {
            get;
            set;
        }

        public virtual double salarioDiarioIntegrado
        {
            get;
            set;
        }

        public virtual double salarioDiarioVariable
        {
            get;
            set;
        }

        public virtual int? tipoDeSalario
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual FiniquitosLiquida finiquitosLiquida
        {
            get;
            set;
        }

        public virtual RegistroPatronal registroPatronal
        {
            get;
            set;
        }

        public virtual IList<SalariosIntegradosDet> salariosIntegradosDet
        {
            get;
            set;
        }
    }

}
