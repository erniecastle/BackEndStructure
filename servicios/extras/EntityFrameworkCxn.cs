using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Exitosw.Payroll.Core.servicios.extras
{
    public class EntityFrameworkCxn
    {
        public static DbConnection createDbConnection(ConnectionDB conexion)
        {
            DbConnection dbConnection = null;
            String stringConexion = "";
            if (conexion.tipoServer == TypeDB.SQLServer)
            {
                stringConexion = @"Data Source=" + conexion.server + "," + conexion.puertoServer + ";Initial Catalog=" + conexion.nombreBd + ";User ID=" + conexion.usuario
                   + ";Password=" + conexion.password + ";Trusted_Connection=False;";
                //try
                //{
                //    var con = new SqlConnectionStringBuilder(stringConexion);
                //    using (SqlConnection connectionCheck = new SqlConnection(stringConexion))
                //    {
                //        connectionCheck.Open();
                //        connectionCheck.Close();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    EventLog.WriteEntry("Application", "Error en conexion a base de datos: " + ex.Message + " " + ex.Source + " " + ex.StackTrace + " " + ex.InnerException, EventLogEntryType.Information, 99);
                //    dbConnection = null;
                //}
                SqlConnection connection = new SqlConnection(stringConexion);
                dbConnection = connection;
            }
            else if (conexion.tipoServer == TypeDB.MySQL)
            {
                stringConexion = "server=" + conexion.server + ";port=" + conexion.puertoServer + ";database=" + conexion.nombreBd + ";uid=" + conexion.usuario
                    + ";pwd=" + conexion.password;
                MySqlConnection connection = new MySqlConnection(stringConexion);
                dbConnection = connection;
            }
            return dbConnection;
        }
    }
}
