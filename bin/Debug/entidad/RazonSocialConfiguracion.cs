/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2019
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class RazonSocialConfiguracion {

        public RazonSocialConfiguracion()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual bool permitido
        {
            get;
            set;
        }

        public virtual RazonSocial razonSocial
        {
            get;
            set;
        }

        public virtual Usuario usuario
        {
            get;
            set;
        }
    }

}
