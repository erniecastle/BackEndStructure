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

    public partial class ConcepNomDefi {

        public ConcepNomDefi()
        {
            this.baseAfecConcepNom = new List<BaseAfecConcepNom>();
            this.camposDimConceptos = new List<CamposDimConceptos>();
            this.paraConcepDeNom = new List<ParaConcepDeNom>();
            this.conceptoPorTipoCorrida = new List<ConceptoPorTipoCorrida>();
            this.configConceptosSat = new List<ConfigConceptosSat>();
            this.creditoAhorro_cNDescuento_ID = new List<CreditoAhorro>();
            this.creditoAhorro_cNDInteresMensual_ID = new List<CreditoAhorro>();
            this.creditoAhorro_concepNomiDefin_ID = new List<CreditoAhorro>();
            this.excepciones = new List<Excepciones>();
            this.movNomConcep = new List<MovNomConcep>();
            this.salariosIntegradosDet = new List<SalariosIntegradosDet>();
           
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual bool activado
        {
            get;
            set;
        }

        public virtual int? agregarSubcuentasPor
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string comportamiento
        {
            get;
            set;
        }

        public virtual string condicionConcepto
        {
            get;
            set;
        }

        public virtual string cuentaContable
        {
            get;
            set;
        }

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual string descripcionAbreviada
        {
            get;
            set;
        }

        public virtual System.DateTime? fecha
        {
            get;
            set;
        }

        public virtual string formulaConcepto
        {
            get;
            set;
        }

        public virtual bool imprimirEnListadoNomina
        {
            get;
            set;
        }

        public virtual bool imprimirEnReciboNomina
        {
            get;
            set;
        }

        public virtual string mascara
        {
            get;
            set;
        }

        public virtual int naturaleza
        {
            get;
            set;
        }

        public virtual int? prioridadDeCalculo
        {
            get;
            set;
        }

        public virtual string subCuenta
        {
            get;
            set;
        }

        public virtual int tipo
        {
            get;
            set;
        }

        public virtual TipoAccionMascaras tipoAccionMascaras
        {
            get;
            set;
        }

        public virtual int? tipoMovto
        {
            get;
            set;
        }

        public virtual IList<BaseAfecConcepNom> baseAfecConcepNom
        {
            get;
            set;
        }

        public virtual IList<CamposDimConceptos> camposDimConceptos
        {
            get;
            set;
        }

        public virtual Grupo grupo
        {
            get;
            set;
        }

        public virtual ConceptoDeNomina conceptoDeNomina
        {
            get;
            set;
        }

        public virtual IList<ParaConcepDeNom> paraConcepDeNom
        {
            get;
            set;
        }


        public virtual IList<ConceptoPorTipoCorrida> conceptoPorTipoCorrida
        {
            get;
            set;
        }

        public virtual IList<ConfigConceptosSat> configConceptosSat
        {
            get;
            set;
        }

        public virtual IList<CreditoAhorro> creditoAhorro_cNDescuento_ID
        {
            get;
            set;
        }

        public virtual IList<CreditoAhorro> creditoAhorro_cNDInteresMensual_ID
        {
            get;
            set;
        }

        public virtual IList<CreditoAhorro> creditoAhorro_concepNomiDefin_ID
        {
            get;
            set;
        }

        public virtual IList<Excepciones> excepciones
        {
            get;
            set;
        }

        public virtual IList<MovNomConcep> movNomConcep
        {
            get;
            set;
        }

        public virtual IList<SalariosIntegradosDet> salariosIntegradosDet
        {
            get;
            set;
        }

        public virtual bool activarPlaza { get; set; }

        public virtual bool activarDesglose { get; set; }

        public virtual CategoriasPuestos categoriaPuestos { get; set; }
    }

}
