/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

using System.Collections.Generic;

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class Excepciones
    {

        public Excepciones()
        {
            this.asistencias = new List<Asistencias>();
            this.configAsistencias = new List<ConfigAsistencias>();
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

        public virtual string excepcion
        {
            get;
            set;
        }

        public virtual int naturaleza
        {
            get;
            set;
        }

        public virtual int orden
        {
            get;
            set;
        }

        public virtual int tipoDatoExcepcion
        {
            get;
            set;
        }

        public virtual bool unico
        {
            get;
            set;
        }

        public virtual IList<Asistencias> asistencias
        {
            get;
            set;
        }

        public virtual ConcepNomDefi concepNomDefi
        {
            get;
            set;
        }

        public virtual IList<ConfigAsistencias> configAsistencias
        {
            get;
            set;
        }
    }

}
