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

    public partial class CategoriasPuestos {

        public CategoriasPuestos()
        {
            this.percepcionesFijas = new List<PercepcionesFijas>();
            this.plazas = new List<Plazas>();
            this.puestos = new List<Puestos>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual bool estado
        {
            get;
            set;
        }

        public virtual bool pagarPorHoras
        {
            get;
            set;
        }

        public virtual byte[] tablaBase
        {
            get;
            set;
        }

        public virtual IList<PercepcionesFijas> percepcionesFijas
        {
            get;
            set;
        }

        public virtual IList<Plazas> plazas
        {
            get;
            set;
        }

        public virtual IList<Puestos> puestos
        {
            get;
            set;
        }
    }

}
