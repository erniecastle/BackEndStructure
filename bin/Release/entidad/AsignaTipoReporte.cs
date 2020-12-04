/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */


namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class AsignaTipoReporte {
    
        public AsignaTipoReporte()
        {
        }

    
        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int tipoReporte
        {
            get;
            set;
        }

        public virtual ReporteDinamico reporteDinamico
        {
            get;
            set;
        }
    }

}
