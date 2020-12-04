using System;
using System.Collections.Generic;
using System.Linq;
using Exitosw.Payroll.Entity.entidad;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Exitosw.Payroll.Core.util;
using System.Configuration;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class FuenteDatosDAO : GenericRepository<CamposOrigenDatos>, FuenteDatosDAOIF
    {

        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private string appName = ConfigurationManager.AppSettings["routeEntitiesEF"];
        private String[] path = null;

        public FuenteDatosDAO(string appName)
        {
            path = new String[] { appName + ".entidad", appName + ".entidad.contabilidad", appName + ".entidad.cfdi" };
        }
        public Mensaje getOrigenDatosTablas()
        {
            try
            {
                inicializaVariableMensaje();
                List<string> datos = new List<string>();
                for (int i = 0; i < path.Length; i++)
                {
                    datos.AddRange(GetClasses(path[i]));
                }
                datos.Sort();
                var tablas = datos;
                mensajeResultado.resultado = tablas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getDatosTabla(string fuente)
        {
            try
            {
                inicializaVariableMensaje();
                var tablas = GetData(fuente);
                mensajeResultado.resultado = tablas;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }
            return mensajeResultado;
        }

        public Mensaje getOrigenDatos(DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                Console.Write(dbContext.connectionDB);//Quitar este
                getSession().Database.BeginTransaction();
                var origenDatos = (from b in getSession().Set<OrigenDatos>()
                                   where b.origen == "BD"
                                   select new { id = b.id, nombre = b.nombre }).ToList();
                origenDatos.Sort((a, b) =>
                {
                    // compare b to a to get ascending order or descending
                    int result = a.nombre.CompareTo(b.nombre);
                    /*if (result == 0)
                    {
                        // if nombre are the same, sort by id
                        result = a.id.CompareTo(b.id);
                    }*/
                    return result;
                });
                mensajeResultado.resultado = origenDatos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCamposPorOrigen()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public List<Dictionary<string, object>> GetData(string nameSpace)
        {
            Assembly asm = Assembly.Load(appName);
            Type type = asm.GetType(appName + ".entidad." + nameSpace);
            if (type == null)
            {
                type = asm.GetType(appName + ".entidad.contabilidad." + nameSpace);
                if (type == null)
                {
                    type = asm.GetType(appName + ".entidad.cfdi." + nameSpace);
                }
            }
            StringBuilder builder = new StringBuilder();
            List<Dictionary<string, object>> listaDeDicionarios = new List<Dictionary<string, object>>();
            Dictionary<string, object> listProperties = null;
            var table = type.GetCustomAttribute<TableAttribute>();
            var dataTable = new Dictionary<string, object>();
            if (table != null)
            {
                PropertyInfo[] props = type.GetProperties();

                foreach (PropertyInfo prop in props)
                {
                    object[] attrs = prop.GetCustomAttributes(true);
                    var attribute = prop.GetCustomAttribute<Marked>();

                    if (attribute != null)
                    {
                        listProperties = new Dictionary<string, object>();
                        Marked marked = attribute as Marked;

                        var isRequired = prop.GetCustomAttribute<RequiredAttribute>();

                        //Id
                        listProperties.Add("id", "");

                        //Campo
                        listProperties.Add("campo", prop.Name);

                        //Estado
                        listProperties.Add("estado", true);

                        if (marked.llavePrincipal)
                        {
                            //Llave prinicpal
                            listProperties.Add("llavePrincipal", marked.llavePrincipal);
                        }

                        if (marked.isPassword)
                        {
                            listProperties.Add("isPassword", marked.isPassword);
                        }
                        //Llave
                        listProperties.Add("llave", marked.llave);

                        //Requerido
                        if (isRequired != null)
                        {
                            listProperties.Add("requerido", true);
                        }
                        else {
                            listProperties.Add("requerido", false);
                        }

                        String nameKeyField = prop.Name;

                        if (nameKeyField.Contains("_ID") || marked.referencia != null)
                        {
                            nameKeyField = UppercaseFirst(nameKeyField);
                            nameKeyField = nameKeyField.Replace("_ID", "");
                            if (marked.referencia != null)
                            {
                                nameKeyField = marked.referencia.Name;
                            }
                            listProperties.Add("idEtiqueta", nameKeyField);
                        }
                        else {

                            listProperties.Add("idEtiqueta", table.Name + FirstCharacterToLower(prop.Name));

                        }
                        //IdEtiqueta
                        if (prop.PropertyType == typeof(string))
                        {
                            listProperties.Add("tipoDeDato", 1);
                        }
                        else if (prop.PropertyType == typeof(int?))
                        {
                            listProperties.Add("tipoDeDato", 2);
                        }

                        else if (prop.PropertyType.Equals(typeof(Int32)))
                        {
                            listProperties.Add("tipoDeDato", 2);
                        }

                        else if (prop.PropertyType == typeof(double?) || prop.PropertyType == typeof(double))
                        {
                            listProperties.Add("tipoDeDato", 3);
                        }
                        else if (prop.PropertyType == typeof(decimal?) || prop.PropertyType == typeof(decimal))
                        {
                            listProperties.Add("tipoDeDato", 3);
                        }
                        else if (prop.PropertyType == typeof(DateTime?))
                        {
                            var isDatTime = prop.GetCustomAttribute<DataTypeAttribute>();
                            if (isDatTime != null)
                            {
                                DataType dt = isDatTime.DataType;
                                if (dt == DataType.Date)
                                {
                                    listProperties.Add("tipoDeDato", 4);
                                }
                                else if (dt == DataType.Time)
                                {
                                    listProperties.Add("tipoDeDato", 6);
                                }
                            }
                        }
                        else if (prop.PropertyType == typeof(bool))
                        {
                            listProperties.Add("tipoDeDato", 5);
                        }

                        else if (prop.PropertyType == typeof(Exitosw.Payroll.Entity.entidad.Naturaleza))
                        {
                            listProperties.Add("tipoDeDato", 2);
                        }

                        //compAncho
                        listProperties.Add("compAncho", 30);

                        //tipoDeCaptura
                        listProperties.Add("tipoCaptura", "");

                        if (marked.referencia != null)
                        {
                            //Referencia a tabla
                            listProperties.Add("referencia", marked.referencia.Name);
                        }

                        listProperties.Add("activarGlobal", false);

                        listProperties.Add("activarCaptura", false);

                        //To end
                        listaDeDicionarios.Add(listProperties);
                    }
                }
                //Console.Write(listaDeDicionarios);
            }
            return listaDeDicionarios;
        }

        private List<string> GetClasses(string nameSpace)
        {
            //Assembly.Load("Server");
            //Assembly asm = Assembly.GetExecutingAssembly();
            Assembly asm = Assembly.Load(appName);
       
            List<string> classlist = new List<string>();
            var dictionary = new Dictionary<string, string>();
            foreach (Type type in asm.GetTypes())
            {
                if (type.Namespace == nameSpace)
                {
                    var attribute = type.GetCustomAttribute<TableAttribute>();
                    if (attribute != null)
                    {
                        //dictionary.Add(attribute.Name, type.Name);
                        classlist.Add(type.Name);
                    }
                }
            }
            classlist.Sort();
            return classlist;
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string FirstCharacterToLower(string s)
        {
            if (String.IsNullOrEmpty(s) || Char.IsLower(s, 0))
                return s;

            return Char.ToLowerInvariant(s[0]) + s.Substring(1);
        }


    }
}