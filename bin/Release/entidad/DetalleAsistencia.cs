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

    public partial class DetalleAsistencia {

        public DetalleAsistencia()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual System.DateTime? dia
        {
            get;
            set;
        }

        public virtual double? horaDoble
        {
            get;
            set;
        }

        public virtual double? horaTriple
        {
            get;
            set;
        }

        public virtual int? tipoPantalla
        {
            get;
            set;
        }

        public virtual CentroDeCosto centroDeCosto
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNomina
        {
            get;
            set;
        }

        public virtual PeriodosNomina periodosNomina
        {
            get;
            set;
        }

        public virtual Empleados empleados
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
