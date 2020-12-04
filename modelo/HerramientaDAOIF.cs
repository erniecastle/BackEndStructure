using Exitosw.Payroll.Core.util;
/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface HerramientaDAOIF para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/
using Exitosw.Payroll.Entity.entidad;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modelo
{
    public interface HerramientaDAOIF : IGenericRepository<Herramienta>
    {
        Mensaje getAllHerramienta(DBContextAdapter dbContext);

        Mensaje getHerramientaCompartidas(DBContextAdapter dbContext);

        Mensaje getHerramientasPrincipales(Usuario usuario, DBContextAdapter dbContext);

        Mensaje agregar(Herramienta entity, DBContextAdapter dbContext);

        Mensaje modificar(Herramienta entity, DBContextAdapter dbContext);

        Mensaje eliminar(Herramienta entity, DBContextAdapter dbContext);

        Mensaje SaveHerramientas(List<Herramienta> h, DBContextAdapter dbContext);

        Mensaje DeleteHerramientas(List<Herramienta> h, DBContextAdapter dbContext);

        Mensaje getHerramientasPrincipalesCompartidas(Usuario usuario, DBContextAdapter dbContext);
    }
}
