
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Configuration;
using Exitosw.Payroll.Entity.genericos;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.campos;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.servicios.extras
{

    public class ConectorQuerysGenericos
    {
        private GeneradorQueries generadorQueries = new GeneradorQueries();
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private StringBuilder concatena = new StringBuilder(200);
        private Mensaje mensajeResultado = new Mensaje();
        private string nameProject = ConfigurationManager.AppSettings["routeEntitiesEF"];
       

        public Mensaje consultaGenerica(DBContextAdapter dbContext, TipoResultado tipoResultado, TipoOperacion tipoOperacion, string tabla, OperadorSelect operadorSelect, List<CamposFrom> listCamposFrom, List<CamposWhere> listCamposWhere, List<CamposGrupo> listCamposGrupo, List<CamposOrden> listCamposOrden, ValoresRango valoresRango)
        {

            Mensaje resultado = null;
            ConnectionDB conexion = dbContext.connectionDB;
            //Code 12630
            // Type type = dbContext.context.Database.Connection.GetType();
            TypeDB tipoServer = conexion.tipoServer;

            //Add the type of database
           

            string query = generadorQueries.construyeQuery(tipoServer, tipoOperacion, "", operadorSelect, listCamposFrom, listCamposWhere, listCamposGrupo, listCamposOrden, valoresRango);


            if (tipoServer == TypeDB.MySQL)
            {

            }
            else if (tipoServer == TypeDB.Oracle)
            {

            }
            if (tipoServer == TypeDB.PostgreSQL)
            {

            }
            else
            {
                resultado = creaConexionSQLServer(conexion, query, tipoResultado);
            }
            return resultado;
        }

        private Mensaje creaConexionSQLServer(ConnectionDB connexion, string query, TipoResultado tipoResultado)
        {
            List<object> resultado = new List<object>();
            ////SqlTransaction sqlTrans = null;
            inicializaVariableMensaje();
            try
            {
                //Code 12630
                //(SqlConnection)dbContext.Database.Connection
               
                ///new SqlConnection(dbContext.Database.Connection.ConnectionString)
                using (SqlConnection conn = (SqlConnection)EntityFrameworkCxn.createDbConnection(connexion))
                {
                    conn.Open();
                    //sqlTrans = conn.BeginTransaction();

                    SqlCommand command = new SqlCommand(query, conn); //, sqlTrans

                    foreach (var item in generadorQueries.valoresParametrosQuery)
                    {
                        command.Parameters.Add(new SqlParameter(item.Key, item.Value));

                    }

                    object instancia = null;
                    object[] valores;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int cont = 0, columnas = reader.FieldCount;
                        while (reader.Read())
                        {
                            valores = new object[generadorQueries.tipoDatosSelect.Count];
                            foreach (var item in generadorQueries.tipoDatosSelect)
                            {
                                instancia = generaInstanciaValores(reader, item, cont);
                                valores[cont] = instancia;
                                cont++;
                                Console.WriteLine(instancia);
                            }
                            resultado.Add(valores);
                            cont = 0;
                        }
                    }
                    
                }
                //  sqlTrans.Commit();
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                if (tipoResultado == TipoResultado.Unico)
                {
                    if (resultado.Count > 0)
                    {
                        if (generadorQueries.tipoDatosSelect.Count == 1)
                        {
                            object[] valor = (object[])resultado[0];
                            mensajeResultado.resultado = valor[0];
                        }
                        else
                        {
                            mensajeResultado.resultado = resultado[0];
                        }
                        return mensajeResultado;
                    }
                    else
                    {
                        mensajeResultado.resultado = null;
                    }
                }
                else
                {
                    mensajeResultado.resultado = resultado;
                }

            }
            catch (Exception ex)
            {
                //if (sqlTrans != null)
                //{
                //    sqlTrans.Rollback();
                //}
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("creaConexionSQLServer()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }
            return mensajeResultado;
        }

        private object creaConexionMYSql(ConnectionDB connexion, string query, TipoResultado tipoResultado)
        {
            List<object> resultado = new List<object>();
            //////using (MySqlConnection conn = (MySqlConnection)genereDbConnection(connexion))
            //////{
            //////    conn.Open();
            //////    MySqlCommand command = new MySqlCommand(query, conn);

            //////    foreach (var item in generadorQueries.valoresParametrosQuery)
            //////    {
            //////        command.Parameters.Add(new MySqlParameter(item.Key, item.Value));

            //////    }

            //////    //Type clasePrincipal = null;
            //////    //object instancia = null;

            //////    //foreach (var item in generadorQueries.tipoDatosSelect)
            //////    //{
            //////    //    clasePrincipal = item;
            //////    //    break;

            //////    //}
            //////    object instancia = null;
            //////    object[] valores;

            //////    using (MySqlDataReader reader = command.ExecuteReader())
            //////    {
            //////        int cont = 0, columnas = reader.FieldCount;
            //////        while (reader.Read())
            //////        {
            //////            valores = new object[generadorQueries.tipoDatosSelect.Count];
            //////            foreach (var item in generadorQueries.tipoDatosSelect)
            //////            {
            //////                instancia = generaInstanciaValores(reader, item, cont);
            //////                valores[cont] = instancia;
            //////                cont++;
            //////                Console.WriteLine(instancia);
            //////            }
            //////            resultado.Add(valores);
            //////            cont = 0;
            //////            //instancia = generaInstanciaValores(reader, clasePrincipal, cont);
            //////            //resultado.Add(instancia);
            //////        }
            //////    }
            //////}
            //////if (tipoResultado == TipoResultado.Unico)
            //////{
            //////    if (resultado.Count > 0)
            //////    {
            //////        return resultado[0];
            //////    }
            //////}
            return resultado;
        }

        private object generaInstanciaValores(DbDataReader reader, Type clasePrincipal, int index)
        {
            object instancia = null;
            PropertyInfo[] propertie;
            if (clasePrincipal != null)
            {
                if (clasePrincipal.FullName.StartsWith("System"))
                {
                    instancia = reader[index];
                    index++;
                    return instancia;
                }
                else
                {
                    instancia = Activator.CreateInstance(clasePrincipal);
                    propertie = clasePrincipal.GetProperties();
                }
            }
            else
            {
                propertie = new PropertyInfo[] { };
            }

            foreach (PropertyInfo item in propertie)
            {
                if (!item.PropertyType.FullName.StartsWith("System.Collections.Generic"))
                {
                    PropertyInfo numberPropertyInfo = clasePrincipal.GetProperty(item.Name);
                    if (item.PropertyType.FullName.StartsWith(nameProject + ".entidad"))
                    {
                        //numberPropertyInfo.SetValue(instancia, generaInstanciaValores(reader, item.PropertyType, index), null);
                    }
                    else
                    {
                        //object val = reader[index];
                        //if (val.GetType() == null) {
                        //    numberPropertyInfo.SetValue(instancia, "", null);
                        //}
                        if (!Convert.IsDBNull(reader[index]))
                            numberPropertyInfo.SetValue(instancia, reader[index], null);
                        index++;
                    }

                }
            }
            return instancia;
        }

        private void inicializaVariableMensaje()
        {
            if (mensajeResultado == null)
            {
                mensajeResultado = new Mensaje();

            }
            mensajeResultado.resultado = null;
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
        }
    }
}
