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
    public interface VacacionesAplicacionHDAOIF : IRepository<VacacionesAplicacion>
    {
        Mensaje agregar(VacacionesAplicacion entity, ISession uuidCxn);

        Mensaje actualizar(VacacionesAplicacion entity, ISession uuidCxn);

        Mensaje eliminar(VacacionesAplicacion entity, ISession uuidCxn);

        Mensaje getVacacionesAplicacionAll(String claveRazonesSocial, ISession uuidCxn);

        Mensaje getVacacionesPorEmpleado(String claveEmpleado, String claveRazonSocial, ISession uuidCxn);

        Mensaje saveDeleteVacacionesAplicacion(List<VacacionesAplicacion> entitysCambios, Object[] clavesDelete, int rango, ISession uuidCxn);
    }
}
