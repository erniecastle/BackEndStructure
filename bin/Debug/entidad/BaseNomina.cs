﻿/**
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

 
    public partial class BaseNomina {
    
   
        public BaseNomina()
        {
            this.baseAfecConcepNom = new List<BaseAfecConcepNom>();
            this.baseAfectadaGrupo = new List<BaseAfectadaGrupo>();
            this.despensa = new List<Despensa>();
        
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string descripcion
        {
            get;
            set;
        }

        public virtual bool reservado
        {
            get;
            set;
        }

        public virtual IList<BaseAfecConcepNom> baseAfecConcepNom
        {
            get;
            set;
        }

        public virtual IList<BaseAfectadaGrupo> baseAfectadaGrupo
        {
            get;
            set;
        }

        public virtual IList<Despensa> despensa
        {
            get;
            set;
        }
    }

}
