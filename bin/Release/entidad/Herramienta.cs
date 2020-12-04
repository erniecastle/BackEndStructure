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

    public partial class Herramienta
    {

        public Herramienta()
        {
            this.contenedor = new List<Contenedor>();
            this.externoPersonalizado = new List<ExternoPersonalizado>();
            this.ventana = new List<Ventana>();
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual bool habilitado
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual bool visible
        {
            get;
            set;
        }

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual IList<Contenedor> contenedor
        {
            get;
            set;
        }

        public virtual IList<ExternoPersonalizado> externoPersonalizado
        {
            get;
            set;
        }

        public virtual TipoHerramienta tipoHerramienta
        {
            get;
            set;
        }

        public virtual IList<Ventana> ventana
        {
            get;
            set;
        }
    }

}
