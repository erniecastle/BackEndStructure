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

    public partial class MovNomBaseAfecta {

        public MovNomBaseAfecta()
        {
            
        }
        public static MovNomBaseAfecta copiaMovBaseAfecta(MovNomBaseAfecta movBaseAfecta)
        {
            MovNomBaseAfecta baseAfecta = new MovNomBaseAfecta();
            baseAfecta.baseAfecConcepNom=movBaseAfecta.baseAfecConcepNom;
            baseAfecta.movNomConcep=movBaseAfecta.movNomConcep;
            baseAfecta.resultado=movBaseAfecta.resultado;
            baseAfecta.resultadoExento=movBaseAfecta.resultadoExento;
            baseAfecta.uso=movBaseAfecta.uso;
            baseAfecta.id=movBaseAfecta.id;
            return baseAfecta;
        }
        public virtual decimal id
        {
            get;
            set;
        }

        public virtual double? resultado
        {
            get;
            set;
        }

        public virtual double? resultadoExento
        {
            get;
            set;
        }

        public virtual int uso
        {
            get;
            set;
        }
        
        public virtual BaseAfecConcepNom baseAfecConcepNom
        {
            get;
            set;
        }

        public virtual MovNomConcep movNomConcep
        {
            get;
            set;
        }
    }

}
