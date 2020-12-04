
using System.Collections.Generic;

namespace Exitosw.Payroll.Hibernate.entidad
{
    public class CategoriaHerramienta
    {
        public CategoriaHerramienta()
        {
            contenedor = new List<Contenedor>();
        }

        public virtual int? id { get; set; }

        public virtual string nombre { get; set; }

        public virtual bool visible { get; set; }

        public virtual List<Contenedor> contenedor { get; set; }

    }
}
