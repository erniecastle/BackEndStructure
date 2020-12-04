using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modelo
{
    public interface ConsultaGenericaEspecialesHDAOIF : IRepository<Object>
    {
        Mensaje existeClave(String identificador, String[] campowhere, Object[] valoreswhere, ISession sessionSimple, ISession sessionSimpleMaster);

        Mensaje consultaPorRangosFiltro(String identificador, int inicio, int rango, String[] camposWhere, Object[] valoresWhere, ISession sessionSimple, ISession sessionSimpleMaster);

        Mensaje obtenerDatosCriterioConsulta(String[] tablas, String[] camposMostrar, String[] camposWhere, Object[] valoresWhere, String[] camposOrden, ISession sessionSimple);

        Mensaje consultaPorFiltros(String query, Object[] campos, Object[] valores, int inicio, int rango, ISession sessionSimple, ISession sessionSimpleMaster);
    }
}
