/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ParametrosDAOIF para llamados a metodos de Entity
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
using System.Collections.Generic;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
  public interface ParametrosDAOIF:IGenericRepository<Parametros>
    {
        Mensaje agregar(Parametros entity, DBContextAdapter dbContext);

        Mensaje actualizar(Parametros entity, DBContextAdapter dbContext);

        Mensaje eliminar(Parametros entity, DBContextAdapter dbContext);

        Mensaje getAllParametros(DBContextAdapter dbContext);

        Mensaje getPorClaveParametros(decimal clave, DBContextAdapter dbContext);

        Mensaje getParametrosPorModulo(decimal id, DBContextAdapter dbContext);

        Mensaje getParametrosPorModuloAsc(decimal id, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosParametros(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosParametros(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        //    void SaveParametros(List<Parametros> p, DBContextAdapter dbContext);
        //
        //    void DeleteParametros(List<Parametros> p, DBContextAdapter dbContext);
        Mensaje getParametrosPorModuloYClave(String claveModulo, decimal clave, DBContextAdapter dbContext);

        Mensaje getParametrosPorModuloYClaves(String claveModulo, Object[] clavesParametros, DBContextAdapter dbContext);

        Mensaje getParametrosYListCrucePorModuloYClaves(String claveModulo, Object[] clavesParametros, DBContextAdapter dbContext);

        Mensaje getParametrosPorModuloYParametro(String nombreModulo, String nombreParametro, DBContextAdapter dbContext);

        Mensaje saveDeleteParametros(List<Parametros> AgreModif, List<Parametros> eliminados, int rango, DBContextAdapter dbContext);

        Mensaje existeParametro(decimal claveParametros, decimal idClave, DBContextAdapter dbContext);

        Mensaje getParametrosPorRango(int inicio, int end,int? clasificacion, DBContextAdapter dbContext);

        Mensaje getPorIdParametro(decimal? idParametro, DBContextAdapter dbContext);

        
    }
}
