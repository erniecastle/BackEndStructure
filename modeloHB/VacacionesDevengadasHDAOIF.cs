using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modeloHB
{
    public interface VacacionesDevengadasHDAOIF : IRepository<VacacionesDevengadas>
    {
        Mensaje getVacacionesDevengadasAll(ISession uuidCxn);

        Mensaje calcularVacacionesDevengadasEmpleados(RazonesSociales razonesSociales, ISession uuidCxn, ISession uuidCxnMaestra);

        Mensaje getVacacionesDenvengadasPorEmpleado(String claveEmpleado, String claveRazonSocial, ISession uuidCxn);

        Mensaje saveDeleteVacacionesDevengadas(List<VacacionesDevengadas> entitysCambios, int rango, ISession uuidCxn);
    }
}
