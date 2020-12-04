
using Exitosw.Payroll.Core.servicios.extras;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.util
{


    public class DBContextAdapter
    {

        public DBContextAdapter(DbContext context, ConnectionDB connectionDB)
        {
            this.context = context;
            this.connectionDB = connectionDB;
            this.msgtoEn = null;

        }

        public DBContextAdapter(bool defaultInit)
        {
            this.context = null;
            this.connectionDB = null;

            this.msgtoEn = new Exitosw.Payroll.Entity.entidad.Mensaje();
            msgtoEn.resultado = null;
            msgtoEn.noError = 55555;
            msgtoEn.error = "no connection";

            this.msgtoHb = new Exitosw.Payroll.Hibernate.entidad.Mensaje();
            msgtoEn.resultado = null;
            msgtoEn.noError = 55555;
            msgtoEn.error = "no connection";

        }

        public ConnectionDB connectionDB { get; set; }

        public DbContext context { get; set; }

        public Exitosw.Payroll.Entity.entidad.Mensaje msgtoEn { get; set; }

        public Exitosw.Payroll.Hibernate.entidad.Mensaje msgtoHb { get; set; }


    }
}
