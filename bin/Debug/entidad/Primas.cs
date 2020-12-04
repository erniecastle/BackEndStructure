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

    public partial class Primas {

        public Primas()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual System.DateTime? fecha
        {
            get;
            set;
        }

        public virtual double? prima
        {
            get;
            set;
        }

        public virtual RegistroPatronal registroPatronal
        {
            get;
            set;
        }
    }

}
