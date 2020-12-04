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

    public partial class Monedas {

        public Monedas()
        {
            this.tiposDeCambio = new List<TiposDeCambio>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string centimosPlural
        {
            get;
            set;
        }

        public virtual string centimosSingular
        {
            get;
            set;
        }

        public virtual int? decimales
        {
            get;
            set;
        }

        public virtual bool generoCentimos
        {
            get;
            set;
        }

        public virtual bool generoMoneda
        {
            get;
            set;
        }

        public virtual string monedaPlural
        {
            get;
            set;
        }

        public virtual string monedaSingular
        {
            get;
            set;
        }

        public virtual string simbolo
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string identificador
        {
            get;
            set;
        }

        public virtual IList<TiposDeCambio> tiposDeCambio
        {
            get;
            set;
        }
    }

}
