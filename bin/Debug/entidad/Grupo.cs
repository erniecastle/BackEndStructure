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

    public partial class Grupo {

        public Grupo()
        {
            this.baseAfectadaGrupo = new List<BaseAfectadaGrupo>();
            this.concepNomDefi = new List<ConcepNomDefi>();
            
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

        public virtual string descripcionAbreviada
        {
            get;
            set;
        }

        public virtual IList<BaseAfectadaGrupo> baseAfectadaGrupo
        {
            get;
            set;
        }

        public virtual IList<ConcepNomDefi> concepNomDefi
        {
            get;
            set;
        }
    }

}
