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

    public partial class TablaPersonalizada {

        public TablaPersonalizada()
        {
            this.tablaDatos = new List<TablaDatos>();
            
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

        public virtual byte[] fileXml
        {
            get;
            set;
        }

        public virtual bool renglonSeleccionado
        {
            get;
            set;
        }

        public virtual TablaBase tablaBase
        {
            get;
            set;
        }

        public virtual IList<TablaDatos> tablaDatos
        {
            get;
            set;
        }

        public virtual TipoTabla tipoTabla
        {
            get;
            set;
        }
    }

}
