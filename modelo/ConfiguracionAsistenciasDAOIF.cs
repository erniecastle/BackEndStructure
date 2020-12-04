

using Exitosw.Payroll.Core.util;
/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Interface ConfiguracionAsistenciasIF para llamados a metodos de Entity
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
    public interface ConfiguracionAsistenciasDAOIF : IGenericRepository<ConfigAsistencias>
    {
        Mensaje agregar(ConfigAsistencias entity, DBContextAdapter dbContext);

        Mensaje actualizar(ConfigAsistencias entity, DBContextAdapter dbContext);

        Mensaje eliminar(ConfigAsistencias entity, DBContextAdapter dbContext);

        Mensaje getExcepcionesPorConfiguracionAsistencias(Decimal id, DBContextAdapter dbContext);

        Mensaje getConfiguracionAsistenciasAll(String claveRazonesSocial, DBContextAdapter dbContext);

        Mensaje PorGrupoMenu(String idContenedor, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje saveDeleteConfiguracionAsistencias(List<ConfigAsistencias> AgreModif, List<ConfigAsistencias> Ordenados, List<ConfigAsistencias> eliminados, DBContextAdapter dbContext);

        Mensaje buscaPorIdYRazonSocial(decimal id, String claveRazonSocial, DBContextAdapter dbContext);

        Mensaje buscaConfiguracionAsistenciasSistema(decimal id, DBContextAdapter dbContext);

        Mensaje getAllConfiguracionAsistenciasSistema(DBContextAdapter dbContext);

        Mensaje getConfiguracionAsistenciaPorRango(int[] values, DBContextAdapter dbContext);
    }
}
