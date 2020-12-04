
/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface IGenericRepository para llamados a metodos  genericos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exitosw.Payroll.Core.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
   public  interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> GetById(decimal id);

        Task Create(TEntity entity);

        Task Update(decimal id, TEntity entity);

        Task Delete(decimal id);

        Mensaje obtenerIdMax(DBContextAdapter dbContext);

        ////List<object> selectIdNombreEntidad();

        // List<object>
        Mensaje consultaPorFiltros(string tabla, List<CamposWhere> camposWhere, ValoresRango valoresRango, DBContextAdapter dbContext);

        // List<object>
        Mensaje consultaPorRangos(ValoresRango valoresRango, List<CamposWhere> camposWhere, DBContextAdapter dbContext);

        //bool
        Mensaje existeDato(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        //object
        Mensaje existeClave(string tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext);

        //string
        Mensaje obtenerClaveStringMax(List<CamposWhere> camposWhere, DBContextAdapter dbContext);

        bool existeTablaDBContextMaster(string tabla);

        object castTiposDatos(Type tipo, object valor);
        object creaInstanciaDao(String nombreDao);
        object creaInstancia(string tabla);
        object crearobjeto(Dictionary<string, object> data);
    }
}
