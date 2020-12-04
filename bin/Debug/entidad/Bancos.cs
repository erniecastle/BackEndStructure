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

    public partial class Bancos {

        public Bancos()
        {
            this.contactos = new List<Contactos>();
            this.plazasPorEmpleadosMov = new List<PlazasPorEmpleadosMov>();
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string RFC
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

        public virtual string domicilio
        {
            get;
            set;
        }

        public virtual string notas
        {
            get;
            set;
        }

        public virtual string paginaweb
        {
            get;
            set;
        }

        public virtual IList<Contactos> contactos
        {
            get;
            set;
        }

        public virtual IList<PlazasPorEmpleadosMov> plazasPorEmpleadosMov
        {
            get;
            set;
        }
    }

}
