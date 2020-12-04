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
    public interface GenerarReportesDAOIF: IRepository<object>
    {
        Mensaje getMovimientosNomina(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session);

        Mensaje getMovimientosReciboNomina(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session);

        Mensaje getReportesMovimientos(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session);

        Mensaje getReportesBaseGravables(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session);

        Mensaje getReportesDeAsimilados(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session);

        Mensaje getReportesCreditosAhorro(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session);

        Mensaje getReporteIntegrados(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session);

        Mensaje getReporteListadoCFDI(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session);

    }
}
