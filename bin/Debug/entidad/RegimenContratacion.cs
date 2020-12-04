/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 14/11/2019
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class RegimenContratacion
    {



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


    }
}
