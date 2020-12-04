/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */


namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class ContenedorPersonalizado
    {

        public ContenedorPersonalizado()
        {

        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual int ordenId
        {
            get;
            set;
        }

        public virtual Contenedor contenedor
        {
            get;
            set;
        }

        public virtual HerramientaPersonalizada herramientaPersonalizada
        {
            get;
            set;
        }

        public virtual Perfiles perfiles
        {
            get;
            set;
        }

    }

}
