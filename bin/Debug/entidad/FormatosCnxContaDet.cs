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

    public partial class FormatosCnxContaDet {

        public FormatosCnxContaDet()
        {
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int? adjuntCfdi
        {
            get;
            set;
        }

        public virtual int? agrupacion
        {
            get;
            set;
        }

        public virtual string conceptoMovto
        {
            get;
            set;
        }

        public virtual string elemento
        {
            get;
            set;
        }

        public virtual double? factor
        {
            get;
            set;
        }

        public virtual byte[] filtro
        {
            get;
            set;
        }

        public virtual int? incluirCC
        {
            get;
            set;
        }

        public virtual bool incluirEnCeros
        {
            get;
            set;
        }

        public virtual int? modoConcepto
        {
            get;
            set;
        }

        public virtual int? modoReferencia
        {
            get;
            set;
        }

        public virtual string referenciaMovto
        {
            get;
            set;
        }

        public virtual int? tipoMovto
        {
            get;
            set;
        }

        public virtual string valor
        {
            get;
            set;
        }

        public virtual CuentasContables cuentasContables
        {
            get;
            set;
        }

        public virtual DatosDisponiblesCxnConta datosDisponiblesCxnConta
        {
            get;
            set;
        }

        public virtual FormatoCnxConta formatoCnxConta
        {
            get;
            set;
        }
    }

}
