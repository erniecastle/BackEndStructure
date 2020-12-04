using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Hibernate.entidad;
using NHibernate;
using System;
using System.Collections.Generic;

namespace Exitosw.Payroll.Core.modeloHB
{
    public interface EnvioCorreoHDAOIF
    {
        Mensaje getCorreoEmpleadosPorFiltros(Dictionary<string, object> filtros, ISession uuidCxn);

        Mensaje enviarCorreoMasivo(Dictionary<string, object> filtros, ISession uuidCxn, DBContextAdapter dbContextSimple);

    }
}
