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

    public partial class TablaBase {

        public TablaBase()
        {
            this.tablaDatos = new List<TablaDatos>();
            this.tablaPersonalizada = new List<TablaPersonalizada>();
            
        }

        public TablaBase(String clave, String nombreTipoTabla, String controladores)
        {
            tipoTabla = new TipoTabla();
            tipoTabla.nombre=(nombreTipoTabla);
            this.clave = clave;
            this.controladores = controladores;
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

        public virtual string controladores
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

        public virtual TipoTabla tipoTabla
        {
            get;
            set;
        }

        public virtual IList<TablaDatos> tablaDatos
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
