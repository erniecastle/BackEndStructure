/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class CreditoAhorro {

        public CreditoAhorro()
        {
            this.creditoPorEmpleado = new List<CreditoPorEmpleado>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual bool activarManejoDescuento
        {
            get;
            set;
        }

        public virtual bool asignaAutoNumCredAho
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string conceptoDcto
        {
            get;
            set;
        }

        public virtual System.DateTime corteMesDia
        {
            get;
            set;
        }

        public virtual int? cuandoDescontar
        {
            get;
            set;
        }

        public virtual bool cuotaFija
        {
            get;
            set;
        }

        public virtual bool definirNumEmp
        {
            get;
            set;
        }

        public virtual bool descPropDiasPer
        {
            get;
            set;
        }

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual string descripcionAbrev
        {
            get;
            set;
        }

        public virtual byte? factorProporcional
        {
            get;
            set;
        }

        public virtual bool fondoAhorro
        {
            get;
            set;
        }

        public virtual double? importeDescuento
        {
            get;
            set;
        }

        public virtual bool inicioDescuento
        {
            get;
            set;
        }

        public virtual string longitudNumCredAho
        {
            get;
            set;
        }

        public virtual string longitudNumEmp
        {
            get;
            set;
        }

        public virtual string mascaraNumCredAho
        {
            get;
            set;
        }

        public virtual byte modoAgregarCredAhoIngEmp
        {
            get;
            set;
        }

        public virtual int? modoCapturaDescuento
        {
            get;
            set;
        }

        public virtual int? modoCapturaDescuentoPorc
        {
            get;
            set;
        }

        public virtual int? modoCapturaDescuentoVSMG
        {
            get;
            set;
        }

        public virtual byte modoDescontarCredAhoFini
        {
            get;
            set;
        }

        public virtual byte modoDescontarCredAhoLiqu
        {
            get;
            set;
        }

        public virtual byte modoDescuento
        {
            get;
            set;
        }

        public virtual byte? modoManejoDescuento
        {
            get;
            set;
        }

        public virtual int? numDecimalesDescuento
        {
            get;
            set;
        }

        public virtual int? numDecimalesDescuentoPorc
        {
            get;
            set;
        }

        public virtual int? numDecimalesDescuentoVSMG
        {
            get;
            set;
        }

        public virtual int? periodicidadDescuento
        {
            get;
            set;
        }

        public virtual bool porcentaje
        {
            get;
            set;
        }

        public virtual bool solicitarFecVen
        {
            get;
            set;
        }

        public virtual string tasaIntMens
        {
            get;
            set;
        }

        public virtual string tipoConfiguracion
        {
            get;
            set;
        }

        public virtual bool vsmg
        {
            get;
            set;
        }

        public virtual bool capturarCreditoTotal 
        { 
            get; 
            set; 
        }

        public virtual int? valorVSMG
        {
            get;
            set;
        }

        public virtual ConcepNomDefi concepNomDefi_cNDescuento_ID
        {
            get;
            set;
        }

        public virtual ConcepNomDefi concepNomDefi_cNDInteresMensual_ID
        {
            get;
            set;
        }

        public virtual ConcepNomDefi concepNomDefi_concepNomiDefin_ID
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual IList<CreditoPorEmpleado> creditoPorEmpleado
        {
            get;
            set;
        }
        public virtual int versionCalculoPrestamoAhorro { get; set; }
    }

}
