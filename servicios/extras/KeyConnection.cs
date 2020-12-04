using NHibernate;
using System;


namespace Exitosw.Payroll.Core.servicios.extras
{
    public class KeyConnection
    {

        public KeyConnection(DateTime fecha, String keyCxn, ISessionFactory sessionFactory)
        {
            this.fecha = fecha;
            this.keyCxn = keyCxn;
            this.sessionFactory = sessionFactory;
        }

        public DateTime fecha { get; set; }

        public String keyCxn { get; set; }

        public ISessionFactory sessionFactory { get; set; }

    }
}
