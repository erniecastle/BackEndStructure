


using System.Collections.Generic;
/**
* @author: Ernesto Castillo
* Fecha de Creación: 21/02/2018
* Compañía: Exito
* Descripción del programa: Entidad para HBRequest
* -----------------------------------------------------------------------------
*/
namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class HerramientaPersonalizada {

        public HerramientaPersonalizada()
        {
            contenedorPersonalizado = new List<ContenedorPersonalizado>();
        }

        public virtual int id
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

        public virtual bool visible
        {
            get;
            set;
        }

        public virtual Usuario usuario
        {
            get;
            set;
        }

        public virtual Perfiles perfiles
        {
            get;
            set;
        }

        public virtual IList<ContenedorPersonalizado> contenedorPersonalizado { get; set; }

    }

}
