/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 09/07/2020
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class RegimenFiscal
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
