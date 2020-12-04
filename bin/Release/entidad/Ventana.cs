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

    public partial class Ventana {

        public Ventana()
        {
            this.contenedor = new List<Contenedor>();
            this.herramienta = new List<Herramienta>();
            this.permisos = new List<Permisos>();
            
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual int clave
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual int tipoVentana
        {
            get;
            set;
        }

        public virtual IList<Contenedor> contenedor
        {
            get;
            set;
        }

        public virtual IList<Herramienta> herramienta
        {
            get;
            set;
        }

        public virtual IList<Permisos> permisos
        {
            get;
            set;
        }

        public virtual Sistemas sistemas
        {
            get;
            set;
        }
    }

}
