using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Exitosw.Payroll.Core.servicios.extras
{
    public class NHibernateCxn
    {

        private static List<KeyConnection> lstemail = new List<KeyConnection>();
        private static StringBuilder concatena = new StringBuilder(200);
        private static StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public static ISessionFactory createSessionFactorie(ConnectionDB conexion = null, int tipoDb = -1)
        {
            var cfg = new Configuration();
            String server = conexion == null ? "" : conexion.server;
            String database = conexion == null ? "" : conexion.nombreBd;
            String user = conexion == null ? "" : conexion.usuario;
            String pass = conexion == null ? "" : conexion.password;
            String port = conexion == null ? "" : conexion.puertoServer;

            String cxn = null;
            if (conexion.tipoServer == TypeDB.SQLServer)//SQLServer
            {
                cfg.DataBaseIntegration(x =>
                {
                    cxn = "Server=" + server + ";" + "Database=" + database + ";" + "Uid=" + user + ";" + "Pwd=" + pass + ";";
                    x.ConnectionString = cxn;
                    x.Dialect<MsSql2012Dialect>();
                    x.LogSqlInConsole = true;
                    x.BatchSize = 30;

                });

                //  cfg.SetProperty(NHibernate.Cfg.Environment.DefaultSchema, "dbo");

            }
            if (conexion.tipoServer == TypeDB.MySQL)
            {
                cfg.DataBaseIntegration(x =>
                {
                    cxn = "Server=" + server + ";" + "Port=" + port + ";" + "database=" + database + ";" + "user=" + user + ";"
                    + "password=" + pass + ";" /*+ "sslmode=" + "none" + ";"*/;
                    x.ConnectionString = cxn;
                    x.Dialect<MySQL5Dialect>();
                    x.LogSqlInConsole = true;
                    x.BatchSize = 30;
                });
            }
           
            cfg.SetProperty("current_session_context_class", "web");
            //Create or update for first | validate for Production
            if (conexion == null)
            {
                /* cfg.AddFile(HttpContext.Current.Server.MapPath(@"~\mapeos\ServidoresBd.hbm.xml"));*/
            }

            if (tipoDb == 1)
            {   //Master
                /*Options: create, update, validate*/
                cfg.SetProperty("hbm2ddl.auto", "validate");
                Hibernate.HBClass.AddAssemblyMaster(cfg);
            }
            else if (tipoDb == 2)
            {   //Simple
                /*Options: create, update, validate*/
                cfg.SetProperty("hbm2ddl.auto", "validate");
                Hibernate.HBClass.AddAssemblySimple(cfg);
            }
            else if (tipoDb == 3)
            {   //DBLogin
                /*Options: create, update, validate*/
                cfg.SetProperty("hbm2ddl.auto", "validate");
                Hibernate.HBClass.AddAssemblyLogin(cfg);
            }

            var sefact = createFactory(cfg, cxn);

            return sefact;
        }

        private static ISessionFactory createFactory(Configuration cfg, String cxn)
        {
            ISessionFactory sefact = null;
            try
            {
                lstemail.RemoveAll(m => (DateTime.Now - m.fecha).TotalDays >= 1);
                var exitsFactory = lstemail.Where(m => m.keyCxn == cxn);
                //Verify if exits session
                if (exitsFactory.Count() == 0)
                {
                    var sesFactorie = cfg.BuildSessionFactory();
                    DateTime currentTime = DateTime.Now;
                    lstemail.Add(new KeyConnection(currentTime, cxn, sesFactorie));
                    sefact = sesFactorie;
                }
                else
                {
                    sefact = exitsFactory.Select(n => n.sessionFactory).First();
                }
            }
            catch (HibernateException ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("createFactory()1_Error: ").Append(ex));

            }
            return sefact;
        }
    }
}