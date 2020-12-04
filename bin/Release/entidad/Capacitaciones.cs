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

    public partial class Capacitaciones {

        public Capacitaciones()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string comentarios
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaFin
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaInicio
        {
            get;
            set;
        }

        public virtual Cursos cursos
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }
    }

}
