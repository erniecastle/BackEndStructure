/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 17/01/2018
 * Compañía: Exito Software.
 * Descripción del programa: clase PaisesDAO para llamados a metodos de Entity
 * -----------------------------------------------------------------------------
 * MODIFICACIONES:
 * -----------------------------------------------------------------------------
 * Clave: 
 * Autor: 
 * Fecha: 
 * Descripción: 
 * -----------------------------------------------------------------------------
 */
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data.Entity.Migrations;
using Newtonsoft.Json;
using System.Data.Entity;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class PaisesDAO : GenericRepository<Paises>, PaisesDAOIF

    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        bool commit;

        public Mensaje agregar(Paises entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Paises>().Add(entity);
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

        public Mensaje modificar(Paises entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var genero = getSession().Set<Genero>().FirstOrDefault(g => g.id == entity.id);
                //genero.clave = entity.clave;
                //genero.descripcion = entity.descripcion;
                //genero.empleados = entity.empleados;
                getSession().Set<Paises>().AddOrUpdate(entity);
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

        public Mensaje eliminar(Paises entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<Paises>().Attach(entity);
                getSession().Set<Paises>().Remove(entity);
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

        public Mensaje getAllPaises(DBContextAdapter dbContext)
        {
            // List<Paises> paises = new List<Paises>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var paises = (from m in getSession().Set<Paises>()
                              select new
                              {
                                  id = m.id,
                                  clave = m.clave,
                                  descripcion = m.descripcion,
                                  nacionalidad = m.nacionalidad
                                  /*  estados =m.estados*/
                              }).ToList();
                mensajeResultado.resultado = paises;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPaisAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClavePaises(string clave, DBContextAdapter dbContext)
        {
            //Paises paises;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var paises = (from m in getSession().Set<Paises>()
                              where m.clave == clave
                              select new
                              {
                                  id = m.id,
                                  clave = m.clave,
                                  descripcion = m.descripcion,
                                  nacionalidad = m.nacionalidad,
                                  estados = m.estados.Select(a => new
                                  {
                                      id = a.id,
                                      clave = a.clave,
                                      descripcion = a.descripcion,
                                      //paises = a.paises,
                                      paises_ID = a.paises_ID
                                  }).ToList()
                                  //centroDeCosto = m.centroDeCosto,
                                  //empleados = m.empleados,
                                  //razonesSociales = m.razonesSociales,
                                  //registroPatronal = m.registroPatronal,
                                  //listapaisOrigenEmpleados=m.listapaisOrigenEmpleados

                              }).SingleOrDefault();
                mensajeResultado.resultado = paises;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPaisPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorFiltrosPaises(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<Paises> paises = new List<Paises>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<CamposWhere> camposwheres = new List<CamposWhere>();
                foreach (var item in campos)
                {
                    if (!item.Value.ToString().Equals("") && item.Value != null)
                    {
                        CamposWhere campo = new CamposWhere();
                        campo.campo = "Paises." + item.Key.ToString();
                        campo.valor = item.Value;
                        if (operador == "=")
                        {
                            campo.operadorComparacion = OperadorComparacion.IGUAL;
                        }
                        else if (operador == "like")
                        {
                            campo.operadorComparacion = OperadorComparacion.LIKE;
                        }
                        campo.operadorLogico = OperadorLogico.AND;
                        camposwheres.Add(campo);
                    }


                }
                ValoresRango rangos = new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));
                mensajeResultado.resultado = consultaPorRangos(rangos, camposwheres, null);
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosPais()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosPaises(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<Paises> paises = new List<Paises>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ValoresRango rangos; ; //= new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));
                if (Convert.ToInt32(rango) > 0)
                {
                    rangos = new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));
                }
                else
                {
                    rangos = null;
                }

                mensajeResultado = consultaPorRangos(rangos, null, null);
                //mensajeResultado.noError = 0;
                //mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorRangosPais()1_Error: ").Append(ex));
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

        public Mensaje saveDeletePaises(List<Paises> entitysCambios, object[] clavesDelete, int rango, DBContextAdapter dbContext)
        {
            List<Paises> paises = new List<Paises>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete != null)
                {
                    //deleteListQuerys("Paises", "Clave", clavesDelete);
                    deleteListQuerys("Paises", new CamposWhere("Paises.clave", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                entitysCambios = (entitysCambios == null ? new List<Paises>() : entitysCambios);
                if (commit && entitysCambios.Count > 0)
                {
                    paises = agregarListaPaises(entitysCambios, rango);
                }
                if (commit)
                {
                    getSession().SaveChanges();
                    mensajeResultado.resultado = paises;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    getSession().Database.CurrentTransaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeletePaises()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private List<Paises> agregarListaPaises(List<Paises> entitys, int rango)
        {
            List<Paises> paises = new List<Paises>();
            try
            {
                int i = 0;
                for (i = 0; i < entitys.Count; i++)
                {
                    if (entitys[i].id == 0)
                    {
                        paises.Add(getSession().Set<Paises>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<Paises>().AddOrUpdate(entitys[i]);
                    }

                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaPaises()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                commit = false;
            }
            return paises;
        }
        private bool deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                //deleteListQuery(tabla, campo, valores);
                deleteListQuery(tabla, campoWhere, dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                exito = false;
            }
            return exito;
        }

        public Mensaje saveDetallesPaises(Paises entity, Dictionary<string, object> Detalles, DBContextAdapter dbContext)
        {

            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                if (Detalles.ContainsKey("SaveUpdate"))
                {
                    string saveUpdate = Detalles["SaveUpdate"].ToString();
                    var arregloSave = JsonConvert.DeserializeObject<object[]>(saveUpdate);
                    for (int i = 0; i < arregloSave.Length; i++)
                    {
                        if (mensajeResultado.error.Equals(""))
                        {
                            string detalle = arregloSave[i].ToString();
                            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(detalle);
                            string tabla = dic["Tabla"].ToString();
                            if (entity.id == 0)
                            {
                                dic["paises"] = entity;
                            }
                            else
                            {
                                dic["paises_ID"] = entity.id;
                            }

                            object entidad = crearobjeto(dic);

                            object instanceDAO = creaInstanciaDao(tabla + "DAO");
                            Type typeDao = instanceDAO.GetType();
                            MethodInfo staticMethodInfo = typeDao.GetMethod("modificar");
                            mensajeResultado = (Mensaje)staticMethodInfo.Invoke(instanceDAO, new object[] { entidad, "" });
                        }
                        else
                        {
                            break;
                        }

                    }
                }
                if (Detalles.ContainsKey("Delete"))
                {
                    string delete = Detalles["Delete"].ToString();
                    var arregloDelete = JsonConvert.DeserializeObject<object[]>(delete);
                    for (int i = 0; i < arregloDelete.Length; i++)
                    {
                        if (mensajeResultado.error.Equals(""))
                        {
                            string detalle = arregloDelete[i].ToString();
                            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(detalle);
                            string tabla = dic["Tabla"].ToString();
                            //dic["paises_ID"] = entity.id;
                            object entidad = crearobjeto(dic);
                            object instanceDAO = creaInstanciaDao(tabla + "DAO");
                            Type typeDao = instanceDAO.GetType();
                            MethodInfo staticMethodInfo = typeDao.GetMethod("eliminar");
                            mensajeResultado = (Mensaje)staticMethodInfo.Invoke(instanceDAO, new object[] { entidad, "" });
                        }
                        else
                        {
                            break;
                        }

                    }
                }
                if (mensajeResultado.error.Equals(""))
                {
                    getSession().Set<Paises>().AddOrUpdate(entity);
                    getSession().SaveChanges();
                    // getSession().SaveChanges();
                    mensajeResultado.resultado = true;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {

                    getSession().Database.CurrentTransaction.Rollback();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("SaveDetallesPaises()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje deleteDetallesPaises(Paises entity, Dictionary<string, object> Detalles, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                string delete = Detalles["Delete"].ToString();
                var arregloDelete = JsonConvert.DeserializeObject<object[]>(delete);
                if (Detalles.ContainsKey("Delete"))
                {
                    for (int i = 0; i < arregloDelete.Length; i++)
                    {
                        if (mensajeResultado.error.Equals(""))
                        {
                            string detalle = arregloDelete[i].ToString();
                            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(detalle);
                            string tabla = dic["Tabla"].ToString();

                            dic.Remove("paises");
                            Dictionary<string, object> dicAux = new Dictionary<string, object>();
                            foreach (var item in dic)
                            {
                                if (item.Key.ToString().Equals("Tabla"))
                                {
                                    dicAux.Add(item.Key, item.Value);
                                }
                                else
                                {
                                    dicAux.Add(item.Key.Substring(0, 1).ToLower() + item.Key.Substring(1), item.Value);
                                }
                            }
                            object entidad = crearobjeto(dicAux);
                            object instanceDAO = creaInstanciaDao(tabla + "DAO");
                            Type typeDao = instanceDAO.GetType();
                            MethodInfo staticMethodInfo = typeDao.GetMethod("eliminar");
                            mensajeResultado = (Mensaje)staticMethodInfo.Invoke(instanceDAO, new object[] { entidad, "" });
                        }
                        else
                        {
                            break;
                        }

                    }
                }
                if (mensajeResultado.error.Equals(""))
                {
                    getSession().Set<Paises>().Attach(entity);
                    getSession().Set<Paises>().Remove(entity);
                    getSession().SaveChanges();
                    mensajeResultado.resultado = true;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {

                    getSession().Database.CurrentTransaction.Rollback();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("SaveDetallesPaises()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                getSession().Database.CurrentTransaction.Rollback();
                mensajeResultado.resultado = null;

            }
            return mensajeResultado;
        }

        public Mensaje getPorIdPaises(decimal? idPaises, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var paises = (from m in getSession().Set<Paises>()
                              where m.id == idPaises
                              select new
                              {
                                  id = m.id,
                                  clave = m.clave,
                                  descripcion = m.descripcion,
                                  nacionalidad = m.nacionalidad
                                  //estados = m.estados.Select(a => new
                                  //{
                                  //    id = a.id,
                                  //    clave = a.clave,
                                  //    descripcion = a.descripcion,
                                  //    paises = a.paises,
                                  //    paises_ID = a.paises_ID
                                  //}).ToList()


                              }).FirstOrDefault();
                mensajeResultado.resultado = paises;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdPaises()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getExisteClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                mensajeResultado = existeClave(tabla, campoWhere, dbContext);
            

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existeClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                getSession().Database.CurrentTransaction.Rollback();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }
    }
}