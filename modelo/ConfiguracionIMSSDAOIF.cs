/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConfiguracionIMSSDAOIF para llamados a metodos de Entity
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
    public interface ConfiguracionIMSSDAOIF : IGenericRepository<ConfiguracionIMSS>
    {
        Mensaje agregar(ConfiguracionIMSS entity, DBContextAdapter dbContext);

        Mensaje modificar(ConfiguracionIMSS entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConfiguracionIMSS entity, DBContextAdapter dbContext);

        //Mensaje getConfiguracionIMSSPorClave(Long clave, DBContextAdapter dbContext);

        Mensaje getPorIdConfiguracionIMSS(decimal? idIMSS, DBContextAdapter dbContext);

        Mensaje getConfiguracionIMSSActual(DateTime date, DBContextAdapter dbContext);

        Mensaje getUltimaConfiguracionIMSS(DateTime date, DBContextAdapter dbContext);

        Mensaje exiteConfiguracionIMSS(DateTime date, DBContextAdapter dbContext);

        Mensaje getAllConfiguracionIMSS(DBContextAdapter dbContext);

        Mensaje obtenerRelacionConfIMSS(decimal id_configuracionIMSS, DBContextAdapter dbContext);
    }
}
