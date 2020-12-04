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

    public partial class Mascaras {

        public Mascaras()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string caracterMarcador
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual bool definirCaracterMarcador
        {
            get;
            set;
        }

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual string mascara
        {
            get;
            set;
        }

        public virtual bool permitirModificarMascara
        {
            get;
            set;
        }

        public virtual string tipoMascara
        {
            get;
            set;
        }
    }

}
