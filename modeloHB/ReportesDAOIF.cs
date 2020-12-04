using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Hibernate.entidad;
using NHibernate;
using System;
using System.Collections.Generic;

namespace Exitosw.Payroll.Core.modeloHB
{

    public interface ReportesDAOIF
    {

        Mensaje getResumenPercepDeducc(Dictionary<string, object> filtros, ISession uuidCxn);
    }
}
