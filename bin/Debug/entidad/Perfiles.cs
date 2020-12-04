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

    public partial class Perfiles {

        public Perfiles()
        {
            this.contenedorPersonalizado = new List<ContenedorPersonalizado>();
            this.externoPersonalizado = new List<ExternoPersonalizado>();
            this.herramientaPersonalizada = new List<HerramientaPersonalizada>();
            this.permisos = new List<Permisos>();
            this.usuario = new List<Usuario>();
            
        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual byte? nivelAccesoSistema
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual bool reporte
        {
            get;
            set;
        }

        public virtual string skin
        {
            get;
            set;
        }

        public virtual IList<ContenedorPersonalizado> contenedorPersonalizado
        {
            get;
            set;
        }

        public virtual IList<ExternoPersonalizado> externoPersonalizado
        {
            get;
            set;
        }

        public virtual IList<HerramientaPersonalizada> herramientaPersonalizada
        {
            get;
            set;
        }

        public virtual IList<Permisos> permisos
        {
            get;
            set;
        }

        public virtual IList<Usuario> usuario
        {
            get;
            set;
        }
    }

}
