/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 * MODIFICACIONES:
 * -----------------------------------------------------------------------------
 * Clave: 
 * Autor: 
 * Fecha:
 * Descripción: 
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


    public partial class AguinaldoConfiguracion {
    
        public AguinaldoConfiguracion()
        {
          
        }

   
        public virtual decimal id
        {
            get;
            set;
        }

    

        public virtual int? modoCalculo
        {
            get;
            set;
        }


        public virtual int numPagos
        {
            get;
            set;
        }

    

        public virtual int pagarEnUnaSolaExhibicion
        {
            get;
            set;
        }

    

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }
    }

}
