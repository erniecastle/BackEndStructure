using System;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.servicios.extras
{

    public class DataBase
    {

        public String nombreDB { get; set; }
        public DbContext DbContext { get; set; }

        public DataBase()
        {
        }

        public DataBase(String nombreDB, DbContext DbContext)
        {
            this.nombreDB = nombreDB;
            this.DbContext = DbContext;
        }
    }
}
