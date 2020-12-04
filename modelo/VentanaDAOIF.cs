/**
* @author: Daniel Ruelas
* Fecha de Creación: 18/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface VentanaDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface VentanaDAOIF:IGenericRepository<Ventana>
    {
        Mensaje getAllVentana(DBContextAdapter dbContext);

        Mensaje getVentanaPorTipoVentana(TipoVentana[] tipoVentanas, DBContextAdapter dbContext);

        Mensaje getVentanaPorNombre(String nombreVentana, DBContextAdapter dbContext);

        Mensaje agregar(Ventana entity, DBContextAdapter dbContext);

        Mensaje modificar(Ventana entity, DBContextAdapter dbContext);

        Mensaje eliminar(Ventana entity, DBContextAdapter dbContext);

        Mensaje getVentanaPorSistemas(int id, DBContextAdapter dbContext);

        Mensaje getVentanaPorClave(int clave, DBContextAdapter dbContext);

        Mensaje getPorIdVentana(decimal? id,DBContextAdapter dbContext);
    }
}
