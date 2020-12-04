/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

using System.Collections.Generic;

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class Contenedor
    {

        public Contenedor()
        {
            this.contenedorPersonalizado = new List<ContenedorPersonalizado>();
            this.elementoExterno = new List<ElementoExterno>();
            this.externoPersonalizado = new List<ExternoPersonalizado>();
            this.parametrosConsulta = new List<ParametrosConsulta>();
            this.permisos = new List<Permisos>();
            this.reporteDinamico = new List<ReporteDinamico>();

        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string accion
        {
            get;
            set;
        }

        //public virtual byte[] icono
        //{
        //    get;
        //    set;
        //}

        public virtual string nombreIcono
        {
            get;
            set;
        }

        public virtual bool habilitado
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual int ordenId
        {
            get;
            set;
        }

        public virtual int tipoAcciones
        {
            get;
            set;
        }

        public virtual int tipoIcono
        {
            get;
            set;
        }

        public virtual bool visible
        {
            get;
            set;
        }



        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual Herramienta herramienta
        {
            get;
            set;
        }

        public virtual CategoriaHerramienta categoriaHerramienta
        {
            get;
            set;
        }

        public virtual Ventana ventana
        {
            get;
            set;
        }

        public virtual RazonSocial razonSocial
        {
            get;
            set;
        }

        public virtual TipoElemento tipoElemento
        {
            get;
            set;
        }

        public virtual IList<ContenedorPersonalizado> contenedorPersonalizado
        {
            get;
            set;
        }

        public virtual IList<ElementoExterno> elementoExterno
        {
            get;
            set;
        }

        public virtual IList<ExternoPersonalizado> externoPersonalizado
        {
            get;
            set;
        }

        public virtual IList<ParametrosConsulta> parametrosConsulta
        {
            get;
            set;
        }

        public virtual IList<Permisos> permisos
        {
            get;
            set;
        }

        public virtual IList<ReporteDinamico> reporteDinamico
        {
            get;
            set;
        }
    }

}
