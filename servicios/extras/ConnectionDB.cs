/**
* @author: Ernesto Castillo
* Fecha de Creación: 22/07/2019
* Compañía: Macropro
* Descripción del programa: Clase para multiconexiones de BD
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/
using System;
using System.Text;

namespace Exitosw.Payroll.Core.servicios.extras
{
    public class ConnectionDB
    {

        public ConnectionDB()
        {

        }

        public ConnectionDB(string uuidCnx)
        {

        }


        public ConnectionDB(string[] accesLog)
        {
            this.tipoServer = TypeDB.SQLServer;
            this.usuario = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[1]));
            this.password = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[2]));
            this.puertoServer = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[3]));
            this.server = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[4]));
        }


        public ConnectionDB(TypeDB tipoServer, string server, string puertoServer, string usuario, string password)
        {
            this.tipoServer = tipoServer;
            this.server = server;
            this.puertoServer = puertoServer;
            this.usuario = usuario;
            this.password = password;
        }

        public string usuario { get; set; }
        public string password { get; set; }
        public string puertoServer { get; set; }
        public TypeDB tipoServer { get; set; }/*Mysql, Sql server, Oracle*/
        public string server { get; set; }
        public String nombreBd { get; set; }


    }
}