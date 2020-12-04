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

    public partial class Permisos {

        public Permisos()
        {
            
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual bool consultar
        {
            get;
            set;
        }

        public virtual bool eliminar
        {
            get;
            set;
        }

        public virtual bool imprimir
        {
            get;
            set;
        }

        public virtual bool insertar
        {
            get;
            set;
        }

        public virtual bool modificar
        {
            get;
            set;
        }

        public virtual Contenedor contenedor
        {
            get;
            set;
        }

        public virtual Perfiles perfiles
        {
            get;
            set;
        }

        public virtual Ventana ventana
        {
            get;
            set;
        }

        public virtual Usuario usuario
        {
            get;
            set;
        }
    }

}
