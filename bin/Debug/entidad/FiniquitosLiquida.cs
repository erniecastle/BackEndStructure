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

    public partial class FiniquitosLiquida {

        public FiniquitosLiquida()
        {
            this.finiqLiquidCncNom = new List<FiniqLiquidCncNom>();
            this.finiqLiquidPlazas = new List<FiniqLiquidPlazas>();
            this.finiquitosLiquidas_finiquitosComplementario_ID = new List<FiniquitosLiquida>();
            this.ingresosBajas = new List<IngresosBajas>();
            this.salariosIntegrados = new List<SalariosIntegrados>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual bool bajaPorRiesgo
        {
            get;
            set;
        }

        public virtual bool calculado
        {
            get;
            set;
        }

        public virtual int causaBaja
        {
            get;
            set;
        }

        public virtual string referencia
        {
            get;
            set;
        }

        public virtual int contImpreso
        {
            get;
            set;
        }

        public virtual string descripcionBaja
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaBaja
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaCalculo
        {
            get;
            set;
        }

        public virtual int modoBaja
        {
            get;
            set;
        }

        public virtual string observaciones
        {
            get;
            set;
        }

        public virtual int status
        {
            get;
            set;
        }

        public virtual int? tipoBaja
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual IList<FiniqLiquidCncNom> finiqLiquidCncNom
        {
            get;
            set;
        }

        public virtual IList<FiniqLiquidPlazas> finiqLiquidPlazas
        {
            get;
            set;
        }

        public virtual IList<FiniquitosLiquida> finiquitosLiquidas_finiquitosComplementario_ID
        {
            get;
            set;
        }

        public virtual FiniquitosLiquida finiquitosLiquida_finiquitosComplementario_ID
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual IList<IngresosBajas> ingresosBajas
        {
            get;
            set;
        }

        public virtual IList<SalariosIntegrados> salariosIntegrados
        {
            get;
            set;
        }
    }

}
