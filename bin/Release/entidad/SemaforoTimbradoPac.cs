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

    public partial class SemaforoTimbradoPac {

        public SemaforoTimbradoPac()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual System.DateTime tiempoInicio
        {
            get;
            set;
        }

        public virtual int tipoTimbrado
        {
            get;
            set;
        }

        public virtual string usuario
        {
            get;
            set;
        }

        public virtual PeriodosNomina periodosNomina
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNomina
        {
            get;
            set;
        }
    }

}
