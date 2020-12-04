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

    public partial class MovNomConceParam {

        public MovNomConceParam()
        {
           
        }
        public static MovNomConceParam copiaMovConceParam(MovNomConceParam movNomConceParam)
        {
            MovNomConceParam conceParam = new MovNomConceParam();
            conceParam.movNomConcep=movNomConceParam.movNomConcep;
            conceParam.paraConcepDeNom=movNomConceParam.paraConcepDeNom;
            conceParam.valor=movNomConceParam.valor;
            conceParam.id=movNomConceParam.id;
            return conceParam;
        }
        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string valor
        {
            get;
            set;
        }

        public virtual MovNomConcep movNomConcep
        {
            get;
            set;
        }

        public virtual ParaConcepDeNom paraConcepDeNom
        {
            get;
            set;
        }
    }

}
