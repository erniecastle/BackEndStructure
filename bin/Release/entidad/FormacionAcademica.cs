/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class FormacionAcademica {

        public FormacionAcademica()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string comentarios
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaFin
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaInicio
        {
            get;
            set;
        }

        public virtual string institucion
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual Estudios estudios
        {
            get;
            set;
        }
    }

}
