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

    public partial class TipoTabla {

        public TipoTabla()
        {
            this.tablaBase = new List<TablaBase>();
            this.tablaPersonalizada = new List<TablaPersonalizada>();
            
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual bool sistema
        {
            get;
            set;
        }

        public virtual IList<TablaBase> tablaBase
        {
            get;
            set;
        }

        public virtual IList<TablaPersonalizada> tablaPersonalizada
        {
            get;
            set;
        }
    }

}
