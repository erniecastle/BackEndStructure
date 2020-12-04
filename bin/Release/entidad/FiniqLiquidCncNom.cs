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

    public partial class FiniqLiquidCncNom {

        public FiniqLiquidCncNom()
        {
            this.movNomConcep = new List<MovNomConcep>();
           
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double? cantidad
        {
            get;
            set;
        }

        public virtual double? importe
        {
            get;
            set;
        }

        public virtual bool aplicar
        {
            get;
            set;
        }

        public virtual ConceptoPorTipoCorrida conceptoPorTipoCorrida
        {
            get;
            set;
        }

        public virtual FiniquitosLiquida finiquitosLiquida
        {
            get;
            set;
        }

        public virtual IList<MovNomConcep> movNomConcep
        {
            get;
            set;
        }
    }

}
