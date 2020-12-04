using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public interface PaisesDAOIF:IGenericRepository<Paises>
    {
        Mensaje agregar(Paises entity, DBContextAdapter dbContext);

        Mensaje modificar(Paises entity, DBContextAdapter dbContext);

        Mensaje eliminar(Paises entity, DBContextAdapter dbContext);
        Mensaje getAllPaises(DBContextAdapter dbContext);

        Mensaje getPorClavePaises(String clave, DBContextAdapter dbContext);

        Mensaje consultaPorFiltrosPaises(Dictionary<string,object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        Mensaje consultaPorRangosPaises(Int64 inicio, Int64 rango, DBContextAdapter dbContext);

        //Mensaje existeDato(String campo, Object valor, DBContextAdapter dbContext);

        Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext);

        Mensaje saveDeletePaises(List<Paises> entitysCambios, Object[] clavesDelete, int rango, DBContextAdapter dbContext);

        Mensaje saveDetallesPaises(Paises entity, Dictionary<string,object> Detalles, DBContextAdapter dbContext);

        Mensaje deleteDetallesPaises(Paises entity, Dictionary<string, object> Detalles, DBContextAdapter dbContext);

        Mensaje getPorIdPaises(decimal? idPaises,DBContextAdapter dbContext);
    }
}
