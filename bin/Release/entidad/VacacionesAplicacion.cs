﻿/**
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

    public partial class VacacionesAplicacion {

        public VacacionesAplicacion()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double? diasPrima
        {
            get;
            set;
        }

        public virtual int? diasVac
        {
            get;
            set;
        }

        public virtual VacacionesDevengadas vacacionesDevengadas
        {
            get;
            set;
        }

        public virtual VacacionesDisfrutadas vacacionesDisfrutadas
        {
            get;
            set;
        }
    }

}
