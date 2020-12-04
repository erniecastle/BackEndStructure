


using System;
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

    public partial class IngresosBajas
    {

        public IngresosBajas()
        {
            this.plazasPorEmpleado = new List<PlazasPorEmpleado>();
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaBaja
        {
            get;
            set;
        }

        public virtual System.DateTime fechaIngreso
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual IList<PlazasPorEmpleado> plazasPorEmpleado
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual RegistroPatronal registroPatronal
        {
            get;
            set;
        }

        public virtual bool aplicar
        {
            get;
            set;
        }

        public virtual int? causaBaja
        {
            get;
            set;
        }


        public virtual string tipoSeparacion
        {
            get;
            set;
        }

        public virtual DateTime? fechaCalculo
        {
            get;
            set;
        }
        public virtual bool calculado
        {
            get;
            set;
        }
        public virtual bool complementaria
        {
            get;
            set;
        }

        public virtual DateTime? fechaComplementaria
        {
            get;
            set;
        }
        public virtual bool previa
        {
            get;
            set;
        }
        public virtual bool procesado
        {
            get;
            set;
        }

    }

}
