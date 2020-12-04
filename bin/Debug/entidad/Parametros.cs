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

    public partial class Parametros {

        public Parametros()
        {
            this.cruce = new List<Cruce>();
            this.elementosAplicacion = new List<ElementosAplicacion>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int clasificacion
        {
            get;
            set;
        }

        public virtual decimal clave
        {
            get;
            set;
        }

        public virtual byte[] imagen
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual string opcionesParametros
        {
            get;
            set;
        }

        public virtual int? ordenId
        {
            get;
            set;
        }

        public virtual string propiedadConfig
        {
            get;
            set;
        }

        public virtual int tipoConfiguracion
        {
            get;
            set;
        }

        public virtual string valor
        {
            get;
            set;
        }

        public virtual IList<Cruce> cruce
        {
            get;
            set;
        }

        public virtual IList<ElementosAplicacion> elementosAplicacion
        {
            get;
            set;
        }

        public virtual Modulo modulo
        {
            get;
            set;
        }
    }

}
