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

    public partial class ConfigAsistencias {

        public ConfigAsistencias()
        {
            this.excepciones = new List<Excepciones>();
            
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string activadosFiltro
        {
            get;
            set;
        }

        public virtual string activadosMovimientos
        {
            get;
            set;
        }

        public virtual bool compartir
        {
            get;
            set;
        }

        public virtual string contenedorPadreID
        {
            get;
            set;
        }

        public virtual string filtro
        {
            get;
            set;
        }

        public virtual bool habilitado
        {
            get;
            set;
        }

        public virtual string icono
        {
            get;
            set;
        }

        public virtual string keyCode
        {
            get;
            set;
        }

        public virtual string modifiers
        {
            get;
            set;
        }

        public virtual string movimiento
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

        public virtual string perfilesID
        {
            get;
            set;
        }

        public virtual bool sistema
        {
            get;
            set;
        }

        public virtual string usuarioID
        {
            get;
            set;
        }

        public virtual bool visible
        {
            get;
            set;
        }


        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual IList<Excepciones> excepciones
        {
            get;
            set;
        }
    }

}
