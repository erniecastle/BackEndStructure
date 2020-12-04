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

    public partial class ConceptoPorTipoCorrida {

        public ConceptoPorTipoCorrida()
        {
            this.finiqLiquidCncNom = new List<FiniqLiquidCncNom>();
            
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double cantidad
        {
            get;
            set;
        }

        public virtual int descuentoCreditos
        {
            get;
            set;
        }

        public virtual bool incluir
        {
            get;
            set;
        }

        public virtual bool modificarCantidad
        {
            get;
            set;
        }

        public virtual bool modificarImporte
        {
            get;
            set;
        }

        public virtual bool mostrar
        {
            get;
            set;
        }

        public virtual bool opcional
        {
            get;
            set;
        }

        public virtual ConcepNomDefi concepNomDefi
        {
            get;
            set;
        }

        public virtual TipoCorrida tipoCorrida
        {
            get;
            set;
        }

        public virtual IList<FiniqLiquidCncNom> finiqLiquidCncNom
        {
            get;
            set;
        }
    }

}
