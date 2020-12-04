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

    public partial class Puestos {

        public Puestos()
        {
            this.experienciaLaboralInterna = new List<ExperienciaLaboralInterna>();
            this.percepcionesFijas = new List<PercepcionesFijas>();
            this.plazas = new List<Plazas>();
            this.plazasPorEmpleadosMov = new List<PlazasPorEmpleadosMov>();
            this.ptuEmpleados = new List<PtuEmpleados>();
            
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

        public virtual string descripcionPrevia
        {
            get;
            set;
        }

        public virtual bool directivo
        {
            get;
            set;
        }

        public virtual string funciones
        {
            get;
            set;
        }

        public virtual double maximo
        {
            get;
            set;
        }

        public virtual double minimo
        {
            get;
            set;
        }

        public virtual double salarioTabular
        {
            get;
            set;
        }

        public virtual CategoriasPuestos categoriasPuestos
        {
            get;
            set;
        }

        public virtual IList<ExperienciaLaboralInterna> experienciaLaboralInterna
        {
            get;
            set;
        }

        public virtual IList<PercepcionesFijas> percepcionesFijas
        {
            get;
            set;
        }

        public virtual IList<Plazas> plazas
        {
            get;
            set;
        }

        public virtual IList<PlazasPorEmpleadosMov> plazasPorEmpleadosMov
        {
            get;
            set;
        }

        public virtual IList<PtuEmpleados> ptuEmpleados
        {
            get;
            set;
        }

        public virtual RegistroPatronal registroPatronal
        {
            get;
            set;
        }
    }

}
