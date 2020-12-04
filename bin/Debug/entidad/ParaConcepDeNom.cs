/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Macropro
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

    public partial class ParaConcepDeNom {
    

        public ParaConcepDeNom()
        {
            this.movNomConceParam = new List<MovNomConceParam>();
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int clasificadorParametro
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

        public virtual int? numero
        {
            get;
            set;
        }

        public virtual string tipo
        {
            get;
            set;
        }

        public virtual string unidad
        {
            get;
            set;
        }

        public virtual ConcepNomDefi concepNomDefi
        {
            get;
            set;
        }

        public virtual IList<MovNomConceParam> movNomConceParam
        {
            get;
            set;
        }
    }

}
