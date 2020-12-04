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
    public interface VacacionesDisfrutadasHDAOIF : IRepository<VacacionesDisfrutadas>
    {
        Mensaje agregar(VacacionesDisfrutadas entity, ISession uuidCxn);

        Mensaje actualizar(VacacionesDisfrutadas entity, ISession uuidCxn);

        Mensaje eliminar(VacacionesDisfrutadas entity, ISession uuidCxn);

        Mensaje getVacacionesDisfrutadasAll(String claveRazonesSocial, ISession uuidCxn);

        Mensaje getVacacionesPorEmpleado(String claveEmpleado, String claveRazonSocial, ISession uuidCxn);

        Mensaje saveDeleteVacacionesDisfrutadas(List<VacacionesDisfrutadas> entitysCambios, Object[] clavesDelete, int rango, ISession uuidCxn);

        Mensaje ObtenerVacacionesDisfruradasMaxima(String claveEmpleado, String claveRazonSocial, ISession uuidCxn);

        Mensaje EliminarVacacionesDisfrutadas(List<VacacionesAplicacion> vacAplicacion, ISession uuidCxn);
    }
}
