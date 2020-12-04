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

    public partial class SalariosIntegradosDet {

        public SalariosIntegradosDet()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int cambio
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaCambio
        {
            get;
            set;
        }

        public virtual double horas
        {
            get;
            set;
        }

        public virtual double importe
        {
            get;
            set;
        }

        public virtual ConcepNomDefi concepNomDefi
        {
            get;
            set;
        }

        public virtual PlazasPorEmpleado plazasPorEmpleado
        {
            get;
            set;
        }

        public virtual SalariosIntegrados salariosIntegrados
        {
            get;
            set;
        }
    }

}
