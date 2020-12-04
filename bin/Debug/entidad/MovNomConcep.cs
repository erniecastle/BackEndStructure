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

    public partial class MovNomConcep {

        public MovNomConcep()
        {
            this.calculoIMSS = new List<CalculoIMSS>();
            this.calculoIMSSPatron = new List<CalculoIMSSPatron>();
            this.calculoISR = new List<CalculoISR>();
            this.movNomBaseAfecta = new List<MovNomBaseAfecta>();
            this.movNomConceParam = new List<MovNomConceParam>();
            
        }
        public static MovNomConcep copiaMovimiento(MovNomConcep mov)
        {
            MovNomConcep nuevo = new MovNomConcep();
            if (mov != null)
            {
                nuevo.calculado=mov.calculado;
                nuevo.centroDeCosto=mov.centroDeCosto;
                nuevo.concepNomDefi=mov.concepNomDefi;
                nuevo.ejercicio=mov.ejercicio;
                nuevo.empleados=mov.empleados;
                nuevo.fechaCierr=mov.fechaCierr;
                nuevo.fechaIni=mov.fechaIni;
                nuevo.mes=mov.mes;
                nuevo.numero=mov.numero;
                nuevo.ordenId=mov.ordenId;
                nuevo.periodosNomina=mov.periodosNomina;
                nuevo.plazasPorEmpleado=mov.plazasPorEmpleado;
                nuevo.razonesSociales=mov.razonesSociales;
                nuevo.resultado=mov.resultado;
                nuevo.tipoCorrida=mov.tipoCorrida;
                nuevo.tipoNomina=mov.tipoNomina;
                nuevo.tipoPantalla=mov.tipoPantalla;
                nuevo.uso=mov.uso;
                nuevo.numMovParticion=mov.numMovParticion;
                nuevo.finiqLiquidCncNom=mov.finiqLiquidCncNom;
                int i;
                List<MovNomBaseAfecta> listBase = new List<MovNomBaseAfecta>();
                if (mov.movNomBaseAfecta != null)
                {
                    for (i = 0; i < mov.movNomBaseAfecta.Count; i++)
                    {
                        listBase.Add(MovNomBaseAfecta.copiaMovBaseAfecta(mov.movNomBaseAfecta[i]));
                        listBase[i].movNomConcep=nuevo;
                    }
                }
                nuevo.movNomBaseAfecta=listBase;
                List<MovNomConceParam> listParam = new List<MovNomConceParam>();
                if (mov.movNomConceParam != null)
                {
                    for (i = 0; i < mov.movNomConceParam.Count; i++)
                    {
                        listParam.Add(MovNomConceParam.copiaMovConceParam(mov.movNomConceParam[i]));
                        listParam[i].movNomConcep=nuevo;
                    }
                }
                nuevo.movNomConceParam=listParam;
            }
            return nuevo;
        }
        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double? calculado
        {
            get;
            set;
        }

        public virtual int ejercicio
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaCierr
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaIni
        {
            get;
            set;
        }

        public virtual int? mes
        {
            get;
            set;
        }

        public virtual int numMovParticion
        {
            get;
            set;
        }

        public virtual int? numero
        {
            get;
            set;
        }

        public virtual int ordenId
        {
            get;
            set;
        }

        public virtual double? resultado
        {
            get;
            set;
        }

        public virtual int? tipoPantalla
        {
            get;
            set;
        }

        public virtual int uso
        {
            get;
            set;
        }

        public virtual IList<CalculoIMSS> calculoIMSS
        {
            get;
            set;
        }

        public virtual IList<CalculoIMSSPatron> calculoIMSSPatron
        {
            get;
            set;
        }

        public virtual IList<CalculoISR> calculoISR
        {
            get;
            set;
        }

        public virtual CentroDeCosto centroDeCosto
        {
            get;
            set;
        }

        public virtual ConcepNomDefi concepNomDefi
        {
            get;
            set;
        }

        public virtual CreditoMovimientos creditoMovimientos
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual FiniqLiquidCncNom finiqLiquidCncNom
        {
            get;
            set;
        }

        public virtual IList<MovNomBaseAfecta> movNomBaseAfecta
        {
            get;
            set;
        }

        public virtual PlazasPorEmpleado plazasPorEmpleado
        {
            get;
            set;
        }

        public virtual TipoCorrida tipoCorrida
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNomina
        {
            get;
            set;
        }

        public virtual PeriodosNomina periodosNomina
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }

        public virtual IList<MovNomConceParam> movNomConceParam
        {
            get;
            set;
        }

        public virtual bool IsEnBD { get; set; }

        public virtual VacacionesAplicacion vacacionesAplicacion { get; set; }
    }

}
