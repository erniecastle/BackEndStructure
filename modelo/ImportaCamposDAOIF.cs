using Exitosw.Payroll.Core.util;
/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ImportaCamposDAOIF para llamados a metodos de Entity
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
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
  public  interface ImportaCamposDAOIF:IGenericRepository<ImportaCampos>
    {
        Mensaje agregar(ImportaCampos entity, DBContextAdapter dbContext);

        Mensaje actualizar(ImportaCampos entity, DBContextAdapter dbContext);

        Mensaje eliminar(ImportaCampos entity, DBContextAdapter dbContext);

        Mensaje getAllImportaCampos(DBContextAdapter dbContext);

        Mensaje getImportaCamposPorVentana(String nombreVentana, DBContextAdapter dbContext);

        Mensaje saveDeleteImportaCampos(ImportaCampos entity, Object[] eliminarDetalleImportCampos, DBContextAdapter dbContext);

    }
}
