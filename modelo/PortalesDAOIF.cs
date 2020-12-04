using NHibernate;
using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Core.servicios.extras;

namespace Exitosw.Payroll.Core.modelo
{
    public interface PortalesDAOIF
    {

        Mensaje modificar(Portales entity, string[] accesLog);
        Mensaje getPortalesByKey(string clave, string[] accesLog);
        Mensaje getPortalesByHost(string url, string[] accesLog);
        Mensaje getLogin(string keyPortal, string user, string password, string[] accesLog);

    }
}
