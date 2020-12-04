/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface CreditoAhorroDAOIF para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface CreditoAhorroDAOIF : IGenericRepository<CreditoAhorro>
    {
        Mensaje agregar(CreditoAhorro entity, DBContextAdapter dbContext);

        Mensaje actualizar(CreditoAhorro entity, DBContextAdapter dbContext);

        Mensaje eliminar(CreditoAhorro entity, DBContextAdapter dbContext);

        Mensaje getAllCreditoAhorro(String claveRazonesSociales, String tipoConfiguracion, DBContextAdapter dbContext);

        Mensaje getPorClaveCreditoAhorro(String clave, String claveRazonesSociales, String tipoConfiguracion, DBContextAdapter dbContext);

        Mensaje getPorIDCreditoAhorro(decimal idCreditoAhorro, DBContextAdapter dbContext);

        Mensaje getAllTipoCreditoAhorro(String claveRazonesSociales, String tipoConfiguracion, DBContextAdapter dbContext);

        Mensaje saveCreditoAhorroContenedor(CreditoAhorro entity,Contenedor contenedor, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster);

        Mensaje DeleteCreditoAhorroContenedor(CreditoAhorro entity, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster);
    }
}
