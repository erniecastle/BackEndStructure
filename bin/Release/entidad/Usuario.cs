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

    public partial class Usuario
    {

        public Usuario()
        {
            this.externoPersonalizado = new List<ExternoPersonalizado>();
            this.herramientaPersonalizada = new List<HerramientaPersonalizada>();
            this.permisos = new List<Permisos>();
            this.razonSocialConfiguracion = new List<RazonSocialConfiguracion>();

        }

        public virtual int id
        {
            get;
            set;
        }

        public virtual bool activaFechaEx
        {
            get;
            set;
        }

        public virtual string clave
        {
            get;
            set;
        }

        public virtual string email
        {
            get;
            set;
        }

        public virtual System.DateTime? fechaExpiracion
        {
            get;
            set;
        }

        public virtual int? idioma
        {
            get;
            set;
        }

        public virtual string nombre
        {
            get;
            set;
        }

        public virtual string lastConfig
        {
            get;
            set;
        }

        public virtual string password
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

        public virtual Perfiles perfiles
        {
            get;
            set;
        }

        public virtual IList<Permisos> permisos
        {
            get;
            set;
        }

        public virtual IList<RazonSocialConfiguracion> razonSocialConfiguracion
        {
            get;
            set;
        }

    }

}
