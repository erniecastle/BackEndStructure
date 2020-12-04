/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ConfigMascarasDAO para llamados a metodos de Entity
* -----------------------------------------------------------------------------
* MODIFICACIONES:
* -----------------------------------------------------------------------------
* Clave: 
* Autor: 
* Fecha:
* Descripción: 
* -----------------------------------------------------------------------------
*/
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using Spring.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Exitosw.Payroll.Entity.util;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class ConfigMascarasDAO : GenericRepository<Mascaras>, ConfigMascarasDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private String DEFAULT_FILE = "ConfigMascaras";
        private List<Mascaras> listMascaras = new List<Mascaras>();
        public Mensaje agregar(Mascaras entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Mascaras>().Add(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = entity;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
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

        public Mensaje actualizar(Mascaras entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Mascaras>().AddOrUpdate(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("actualizar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje eliminar(Mascaras entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Mascaras>().Attach(entity);
                getSession().Set<Mascaras>().Remove(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosMascaras(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listMascaras = new List<Mascaras>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ValoresRango rangos = new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));

                mensajeResultado = consultaPorRangos(rangos, null, null);
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorRangos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        //public Mensaje existeDato(string campo, object valor, DBContextAdapter dbContext)
        //{

        //    bool existe = false;
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        setSession(dbContext);
        //        getSession().Database.BeginTransaction();
        //        //existe = existeDato("Mascaras", campo, valor);
        //        mensajeResultado.resultado = existe;
        //        mensajeResultado.noError = 0;
        //        mensajeResultado.error = "";
        //        getSession().Database.CurrentTransaction.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existeDato()1_Error: ").Append(ex));
        //        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
        //        mensajeResultado.error = ex.GetBaseException().ToString();
        //        mensajeResultado.resultado = null;
        //        getSession().Database.CurrentTransaction.Rollback();
        //    }
        //    return mensajeResultado;
        //}

        public Mensaje getAllConfigMascaras(DBContextAdapter dbContext)
        {
            listMascaras = new List<Mascaras>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listMascaras = (from a in getSession().Set<Mascaras>() select a).ToList();
                mensajeResultado.resultado = listMascaras;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConfigMascarasAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveConfigMascaras(string clave, DBContextAdapter dbContext)
        {
            Mascaras mascaras = new Mascaras();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                mascaras = (from m in getSession().Set<Mascaras>()
                            where m.clave.Equals(clave)
                            select m).SingleOrDefault();
                mensajeResultado.resultado = mascaras;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConfigMascarasAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getFilePropertiesMascara(string directorioReportesDelSistema, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                Spring.Util.Properties properties = obtenerPropertiesMascara(directorioReportesDelSistema, dbContext);
                //   ResXResourceSet resxr = obtenerPropertiesMascara(directorioReportesDelSistema, "");
                byte[] serelizedPrint;
                string code = "";
                if (properties != null)
                {
                    // var resources = new List<string>();
                    /*     Dictionary<string, string> dic = new Dictionary<string, string>();
                         foreach (System.Collections.DictionaryEntry entry in resxr)
                         {
                             dic.Add(entry.Key.ToString(), entry.Value.ToString());
                            // dic.Add(entry.Key.ToString());
                             //resources.Add(entry.Value);
                         }
                         */
                    code = SerializationHelper.SerializeToJson(properties);
                    serelizedPrint = SerializationHelper.StringToUTF8ByteArray(code);
                    // code2 = SerializationHelper.UTF8ByteArrayToString(serelizedPrint);
                    // properties = SerializationHelper.DeserializeFromJson<Properties>(code2);
                    if (mensajeResultado.noError == 0)
                    {
                        mensajeResultado.resultado = serelizedPrint;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("FilePropertiesMascara()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getSaveMascarasAfectaClaves(List<Mascaras> listMascaras, bool soloAplicarReEstructuracion, byte[] propertiesMascara, string ubicacion, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
            try
            {
                inicializaVariableMensaje();
                try
                {
                    MemoryStream bis = null;  //ByteArrayInputStream
                    StreamWriter ois = null; //ObjectInputStream
                    Object obj = null;
                    try
                    {
                        bis = new MemoryStream(propertiesMascara);
                        ois = new StreamWriter(bis);
                        try
                        {
                            obj = ois;
                            FileStream fileStream = File.Create(ubicacion + getNameFileConfigurationMask(null));

                        }
                        catch (Exception ex)
                        {
                            mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                            mensajeResultado.error = ex.GetBaseException().ToString();
                            return mensajeResultado;
                        }
                    }
                    finally
                    {
                        if (bis != null)
                        {
                            bis.Close();
                        }
                        if (ois != null)
                        {
                            ois.Close();
                        }
                    }
                }
                catch (FileNotFoundException ex)
                {
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                    return mensajeResultado;
                }
                catch (IOException ex)
                {
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                    return mensajeResultado;
                }

                /* }
                 catch (IOException ex)
                 {
                     mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                     mensajeResultado.error = ex.GetBaseException().ToString();
                     return mensajeResultado;
                 }*/
                int i;
                for (i = 0; i < listMascaras.Count(); i++)
                {
                    if (!soloAplicarReEstructuracion)
                    {
                        getSession().Database.BeginTransaction();
                        getSession().Set<Mascaras>().AddOrUpdate(listMascaras[i]);
                        getSession().SaveChanges();
                        getSession().Database.CurrentTransaction.Commit();

                    }
                    if (listMascaras[i].mascara.Length > 0)
                    {
                        string tabla = listMascaras[i].clave.Substring(0, listMascaras[i].clave.IndexOf("Clave"));
                        if (existeTablaDBContextMaster(tabla))
                        {
                            setSession(dbContext.context);
                        }
                        else
                        {
                            setSession(dbContext.context);
                        }
                        getSession().Database.BeginTransaction();
                        //getSession().Database.CurrentTransaction.Commit();
                        concatena = new StringBuilder(200);
                        // concatena.Append("UPDATE ").Append(tabla).Append(" e ");
                        concatena.Append("UPDATE ").Append(" e ");
                        concatena.Append("SET ").Append(" e.clave = ");
                        string mascaras;
                        bool mascaraAlfanumerico = false;
                        if (listMascaras[i].mascara.IndexOf("A") > -1)
                        {
                            mascaraAlfanumerico = true;
                        }
                        mascaras = listMascaras[i].mascara.Replace("#", "0");
                        if (!mascaraAlfanumerico)
                        {
                            concatena.Append("(CASE WHEN CAST(e.clave as decimal) > 0 THEN ");
                            concatena.Append("(CASE WHEN len('").Append(mascaras).Append("') = len(e.clave) THEN e.clave ");
                            concatena.Append("when len(e.clave) < len('").Append(mascaras).Append("') then (concat(SUBSTRING('").Append(mascaras).Append("', 1, ");
                            concatena.Append("len('").Append(mascaras).Append("') - len(e.clave)),e.clave)) ");
                            concatena.Append("else SUBSTRING(e.clave, len(e.clave) - len('").Append(mascaras).Append("')+1,len(e.clave) ) END)  END )");
                            concatena.Append("From ").Append(tabla).Append(" AS e ");
                        }
                        else
                        {
                            concatena.Append("(CASE WHEN len('").Append(mascaras).Append("') = len(e.clave) or len(e.clave) < len('").Append(mascaras).Append("') ");
                            concatena.Append("THEN e.clave else SUBSTRING(e.clave, len(e.clave) - len('").Append(mascaras).Append("')+1,len(e.clave) ) END )");
                            concatena.Append("From ").Append(tabla).Append(" AS e ");
                        }
                        try
                        {
                            int x = getSession().Database.ExecuteSqlCommand(Convert.ToString(concatena));
                            getSession().Database.CurrentTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                            mensajeResultado.error = ex.GetBaseException().ToString();
                            listMascaras[i].message = ex.Message;
                            mensajeResultado.resultado = null;
                        }
                        if (typeof(RazonesSociales).Name.Equals(tabla, StringComparison.InvariantCultureIgnoreCase)) //.equals(string,StringComparison.InvariantCultureIgnoreCase) = equalsIgnoreCase
                        {
                            tabla = typeof(RazonesSociales).Name;
                            if (existeTablaDBContextMaster(tabla))
                            {
                                setSession(dbContext.context);
                            }
                            else
                            {
                                setSession(dbContext.context);
                            }
                            concatena = new StringBuilder(200);
                            concatena.Append("UPDATE ").Append(" e ");
                            concatena.Append("SET ").Append(" e.claveRazonSocial = ");
                            if (!mascaraAlfanumerico)
                            {
                                concatena.Append("(CASE WHEN CAST(e.claveRazonSocial as decimal) > 0 THEN ");
                                concatena.Append("(CASE WHEN len('").Append(mascaras).Append("') = len(e.claveRazonSocial) THEN e.claveRazonSocial ");
                                concatena.Append("when len(e.claveRazonSocial) < len('").Append(mascaras).Append("') then (concat(SUBSTRING('").Append(mascaras).Append("', 1, ");
                                concatena.Append("len('").Append(mascaras).Append("') - len(e.claveRazonSocial)),e.claveRazonSocial)) ");
                                concatena.Append("else SUBSTRING(e.claveRazonSocial, len(e.claveRazonSocial) - len('").Append(mascaras).Append("')+1,len(e.claveRazonSocial) ) END)  END )");
                                concatena.Append("From ").Append(tabla).Append(" AS e ");
                            }
                            else
                            {
                                concatena.Append("(CASE WHEN len('").Append(mascaras).Append("') = len(e.claveRazonSocial) or len(e.claveRazonSocial) < len('").Append(mascaras).Append("') ");
                                concatena.Append("THEN e.claveRazonSocial else SUBSTRING(e.claveRazonSocial, len(e.claveRazonSocial) - len('").Append(mascaras).Append("')+1,len(e.claveRazonSocial) ) END )");
                                concatena.Append("From ").Append(tabla).Append(" AS e ");
                                try
                                {
                                    int x = getSession().Database.ExecuteSqlCommand(Convert.ToString(concatena));

                                }
                                catch (Exception ex)
                                {
                                    mensajeResultado.noError = 1;
                                    listMascaras[i].message = tabla + " " + ex.Message;
                                    try
                                    {
                                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                                        mensajeResultado.error = ex.GetBaseException().ToString();
                                    }
                                    catch (Exception exx)
                                    {
                                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(exx);
                                        mensajeResultado.error = exx.GetBaseException().ToString();
                                    }
                                    mensajeResultado.resultado = null;
                                }
                            }
                        }
                    }
                }
                List<TablaDatos> listabladatos = new List<TablaDatos>();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                try
                {
                    var query = (from td in getSession().Set<TablaDatos>()
                                 select new
                                 {
                                     contro = td.controladores.Trim(),
                                     td
                                 });
                    listabladatos = (from sub in query
                                     where sub.contro.Length > 0
                                     select sub.td).ToList();
                    int j, k;
                    if (listabladatos != null)
                    {
                        for (i = 0; i < listabladatos.Count(); i++)
                        {
                            List<string> controladoresEntidad = convierteXML(listabladatos[i].tablaBase != null ? listabladatos[i].tablaBase.fileXml : listabladatos[i].tablaPersonalizada.fileXml);
                            if (controladoresEntidad == null)
                            {
                                controladoresEntidad = new List<string>();
                            }
                            string[] controladores = listabladatos[i].controladores.Split('#');
                            for (j = 0; j < controladores.Count(); j++)
                            {
                                for (k = 0; k < controladoresEntidad.Count(); k++)
                                {
                                    string[] controlador = controladoresEntidad[i].Split('.');
                                    if (controladores[j].Contains(controlador[controlador.Length - 1]))
                                    {
                                        string tabla, valor;
                                        tabla = controladores[j].Substring(0, controlador[controlador.Count() - 1].Length);
                                        valor = controladores[j].Substring(controlador[controlador.Length - 1].Length);
                                        Mascaras masca = buscarMascara(tabla, listMascaras);
                                        if (masca != null)
                                        {
                                            string mascaras = masca.mascara.Replace("#", "0");
                                            controladores[j] = tabla + (mascaras.Substring(0, mascaras.Length - valor.Length) + valor);
                                        }
                                        break;
                                    }
                                }
                            }
                            concatena = new StringBuilder(200);
                            if (controladores.Count() > 1)
                            {
                                concatena.Append(controladores[0]);
                                for (j = 1; j < controladores.Count(); j++)
                                {
                                    concatena.Append("#").Append(controladores[j]);
                                }
                                listabladatos[i].controladores = concatena.ToString();
                            }
                            else
                            {
                                listabladatos[i].controladores = controladores[0];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    mensajeResultado.noError = 3;
                    mensajeResultado.error = "Error tabla datos xml.- " + ex.GetBaseException().ToString();
                    try
                    {
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                    }
                    catch (Exception exx)
                    {
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(exx);
                        mensajeResultado.error = exx.GetBaseException().ToString();
                    }
                    mensajeResultado.resultado = null;
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerPropertiesMascara()1_Error: ").Append(ex));
                mensajeResultado.noError = 2;
                mensajeResultado.error = ex.GetBaseException().ToString();
                try
                {
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                }
                catch (Exception exc)
                {
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(exc);
                    mensajeResultado.error = exc.GetBaseException().ToString();
                }
                return mensajeResultado;
            }
            if (mensajeResultado.noError == 1)
            {
                mensajeResultado.resultado = listMascaras;
            }
            else if (mensajeResultado.noError == 0)
            {
                mensajeResultado.resultado = listMascaras;
            }
            return mensajeResultado;
        }
        private Mascaras buscarMascara(String Tabla, List<Mascaras> listMask)
        {
            for (int i = 0; i < listMask.Count(); i++)
            {
                if (listMask[i].clave.Substring(0, listMask[i].clave.IndexOf("Clave")).Equals(Tabla, StringComparison.InvariantCultureIgnoreCase) & listMask[i].message == null)
                {
                    return listMask[i];
                }
            }
            return null;
        }
        private List<String> convierteXML(byte[] xmlString)
        {
            List<string> controladores = new List<string>();
            try
            {

                MemoryStream ms = new MemoryStream(xmlString);
                string cadenas = SerializationHelper.UTF8ByteArrayToString(xmlString);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(cadenas);
                XmlNode node = doc.DocumentElement;
                XmlNodeList list;
                int i;
                node = doc.GetElementsByTagName("Controladores").Item(0);
                if (node != null)
                {
                    list = node.ChildNodes;
                    controladores = new List<string>();
                    for (i = 0; i < list.Count; i++)
                    {
                        XmlElement element = (XmlElement)list.Item(i);
                        if (element.HasAttributes)
                        {

                            if ((TipoControlador)ManejadorEnum.GetValue(element.GetAttribute("identificador"), typeof(TipoControlador)) == TipoControlador.CONTROLADORENTIDAD)
                            {
                                element.GetAttribute("entidad");
                                element.GetAttribute("sistema");
                                controladores.Add(element.GetAttribute("entidad"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("convierteXML()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return controladores;
        }
        private Spring.Util.Properties obtenerPropertiesMascara(String directorioReportesDelSistema, DBContextAdapter dbContext)
        //private ResXResourceSet obtenerPropertiesMascara(String directorioReportesDelSistema, DBContextAdapter dbContext)

        {
            concatena = new StringBuilder();
            concatena.Append(directorioReportesDelSistema);
            Spring.Util.Properties properties = null;
            // ResXResourceSet resxSet = null;
            if (!directorioReportesDelSistema.Substring(directorioReportesDelSistema.Length - 2).Contains(Path.DirectorySeparatorChar))
            {
                concatena.Append(Path.DirectorySeparatorChar);
            }
            concatena.Append(getNameFileConfigurationMask(null));
            string ubicacionFile = concatena.ToString();

            if (File.Exists(ubicacionFile))
            {
                properties = abrirPropiedad(ubicacionFile);
            }
            else
            {
                concatena = new StringBuilder();
                Assembly assembly = Assembly.GetExecutingAssembly();
                StreamReader inputStream = new StreamReader(assembly.GetManifestResourceStream(concatena.Append("Exitosw.Payroll.Core.util.").Append(DEFAULT_FILE).Append(".properties").ToString()));
                try
                {
                    properties = new Spring.Util.Properties();
                    properties.Load(inputStream);
                    //resxSet = new ResXResourceSet(ubicacion);

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerPropertiesMascara()1_Error: ").Append(ex));
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                    mensajeResultado.resultado = null;
                }
            }
            //return resourcemanager;
            return properties;
        }
        private String getNameFileConfigurationMask(DBContextAdapter dbContext)
        {
            //"" = "DBSimple";
            // String[] estructuraConexion = "".Split('|');
            //return DEFAULT_FILE + estructuraConexion[1] + ".properties";
            return DEFAULT_FILE + "" + ".properties";
        }
        private Spring.Util.Properties abrirPropiedad(String file)
        {
            Spring.Util.Properties properties = null;
            try
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(file);
                writer.Flush();
                stream.Position = 0;
                properties = new Spring.Util.Properties();
                properties.Load(new StreamReader(stream));

            }
            catch (IOException ex)
            {
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return properties;
        }

        public Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                mensajeResultado = existeClave(tabla, campoWhere, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getExisteClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                getSession().Database.CurrentTransaction.Rollback();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje getAllConfigMascarasJS(DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               var listMascaras = (from a in getSession().Set<Mascaras>() select new { 
               a.activaMascara,
               a.caracterMarcador,
               a.clave,
               a.definirCaracterMarcador,
               a.descripcion,
               a.id,
               a.mascara,
             //  a.message,
               a.permitirModificarMascara,
               a.tipoMascara
               }).ToList();
                mensajeResultado.resultado = listMascaras;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConfigMascarasAllJS()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveConfigMascara(List<Mascaras> entitys, DBContextAdapter dbContext)
        {
            try
            {
                //commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                for (int i = 0; i < entitys.Count; i++)
                {
                    getSession().Set<Mascaras>().AddOrUpdate(entitys[i]);
                    getSession().SaveChanges();
                }

                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveConfigMascara()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }

        public Mensaje getConfigMascaraPorClaveJS(string claveMascara, DBContextAdapter dbContext)
        {
           // Mascaras mascaras = new Mascaras();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
             var   mascaras = (from m in getSession().Set<Mascaras>()
                            where m.clave.Equals(claveMascara)
                            select new { 
                            m.activaMascara,
                            m.caracterMarcador,
                            m.clave,
                            m.definirCaracterMarcador,
                            m.descripcion,
                            m.id,
                            m.mascara,
                            m.permitirModificarMascara,
                            m.tipoMascara
                            }).SingleOrDefault();
                mensajeResultado.resultado = mascaras;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConfigMascarasAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        /*   private ResXResourceSet abrirPropiedad(String file)
           {
               /* ResourceManager resourceManager = new ResourceManager("ConfigMascaras.resx",
                    Assembly.GetExecutingAssembly());

                return resourceManager;*/
        /*      ResXResourceSet resxSet = new ResXResourceSet(file);
              return resxSet;
          }*/
    }
}