/**
* @author: Daniel Ruelas
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Interface ParametrosDAO para llamados a metodos de Entity
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
using System.Collections.Generic;
using System.Linq;
using Exitosw.Payroll.Entity.entidad;
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.util;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.genericos.campos;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class ParametrosDAO : GenericRepository<Parametros>, ParametrosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private bool commit = false;

        public Mensaje agregar(Parametros entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<ElementosAplicacion> lista = new List<ElementosAplicacion>();
                foreach (var item in entity.elementosAplicacion)
                {
                    ElementosAplicacion a = getSession().Set<ElementosAplicacion>().Find(item.id);
                    lista.Add(a);
                }
                entity.elementosAplicacion = null;
                entity.elementosAplicacion = lista;
                getSession().Set<Parametros>().Add(entity);
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
        public Mensaje actualizar(Parametros entity, DBContextAdapter dbContext)
        {

            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<ElementosAplicacion> listaaux = new List<ElementosAplicacion>();
                List<ElementosAplicacion> lista = entity.elementosAplicacion;
                entity.elementosAplicacion = null;
                getSession().Entry(entity).State=EntityState.Modified;
                getSession().SaveChanges();
                Parametros param = getSession().Set<Parametros>().Include(a => a.elementosAplicacion).ToList().Find(ca => ca.id == entity.id);

                param.elementosAplicacion.Clear();
                for (int i = 0; i < lista.Count; i++)
                {
                    ElementosAplicacion a = getSession().Set<ElementosAplicacion>().Find(lista[i].id);
                    listaaux.Add(a);
                }
                param.elementosAplicacion = listaaux;

                getSession().SaveChanges();
                //var genero = getSession().Set<Genero>().FirstOrDefault(g => g.id == entity.id);
                //genero.clave = entity.clave;
                //genero.descripcion = entity.descripcion;
                //genero.empleados = entity.empleados;
                getSession().Set<Parametros>().AddOrUpdate(entity);
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

        public Mensaje eliminar(Parametros entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<Parametros>().Attach(entity);
                getSession().Set<Parametros>().Remove(entity);
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

        public Mensaje consultaPorFiltrosParametros(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<Parametros> parametros = new List<Parametros>();
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
                        campo.campo = "Parametros." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosParametro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosParametros(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<Parametros> parametros = new List<Parametros>();
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

        public Mensaje existeParametro(decimal claveParametros, decimal idClave, DBContextAdapter dbContext)
        {
            bool exite = false;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var query = from p in getSession().Set<Parametros>()
                            select p;
                if (idClave > 0)
                {
                    query = from sub in query
                            where sub.id != idClave
                            select sub;
                }
                int valor = (from sub2 in query
                             where sub2.clave == claveParametros
                             select sub2).Count();
                if (valor > 0)
                {
                    exite = true;
                }
                mensajeResultado.resultado = exite;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existeParametro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;

        }

        public Mensaje getAllParametros(DBContextAdapter dbContext)
        {
            //List<Parametros> parametros = new List<Parametros>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var parametros = (from p in getSession().Set<Parametros>()
                                  select new {
                                      p.clasificacion,
                                      p.clave,
                                      elementosAplicacion = p.elementosAplicacion.Select(ea=>new {
                                          ea.id,
                                          ea.clave,
                                          ea.entidad,
                                          ea.nombre,
                                          ea.ordenId,
                                          ea.parentId
                                      }).ToList(),
                                      p.id,
                                      p.imagen,
                                      p.modulo_ID,
                                      p.nombre,
                                      p.opcionesParametros,
                                      p.ordenId,
                                      p.propiedadConfig,
                                      p.tipoConfiguracion,
                                      p.valor
                              }).ToList();
                mensajeResultado.resultado = parametros;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveParametros(decimal clave, DBContextAdapter dbContext)
        {
            //Parametros parametros;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               var  parametros = (from p in getSession().Set<Parametros>()
                              where p.clave == clave
                              select new {
                                  p.clasificacion,
                                  p.clave,
                                  elementosAplicacion = p.elementosAplicacion.Select(ea => new {
                                      ea.id,
                                      ea.clave,
                                      ea.entidad,
                                      ea.nombre,
                                      ea.ordenId,
                                      ea.parentId
                                  }).ToList(),
                                  p.id,
                                  p.imagen,
                                  p.modulo_ID,
                                  p.nombre,
                                  p.opcionesParametros,
                                  p.ordenId,
                                  p.propiedadConfig,
                                  p.tipoConfiguracion,
                                  p.valor
                              }).SingleOrDefault();
                mensajeResultado.resultado = parametros;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getParametrosPorModulo(decimal id, DBContextAdapter dbContext)
        {
            List<Parametros> parametros = new List<Parametros>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                parametros = (from p in getSession().Set<Parametros>()
                              where p.modulo.id == id
                              select p).ToList();
                mensajeResultado.resultado = parametros;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosPorModulo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getParametrosPorModuloAsc(decimal id, DBContextAdapter dbContext)
        {
            List<Parametros> parametros = new List<Parametros>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                parametros = (from p in getSession().Set<Parametros>()
                              where p.modulo.id == id
                              orderby p.ordenId ascending
                              select p).ToList();
                mensajeResultado.resultado = parametros;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosPorModuloAsc()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getParametrosPorModuloYClave(string claveModulo, decimal clave, DBContextAdapter dbContext)
        {
            Parametros parametros;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                parametros = (from p in getSession().Set<Parametros>()
                              where p.clave == clave && p.modulo.clave == claveModulo
                              select p).SingleOrDefault();
                mensajeResultado.resultado = parametros;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosPorModuloYClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getParametrosPorModuloYClaves(string claveModulo, object[] clavesParametros, DBContextAdapter dbContext)
        {
            List<Parametros> parametros = new List<Parametros>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                parametros = (from p in getSession().Set<Parametros>()
                              where p.modulo.clave == claveModulo && clavesParametros.Contains(p.clave)
                              select p).ToList();
                mensajeResultado.resultado = parametros;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosPorModuloYClaves()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getParametrosPorModuloYParametro(string nombreModulo, string nombreParametro, DBContextAdapter dbContext)
        {
            Parametros parametros;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                parametros = (from p in getSession().Set<Parametros>()
                              where p.modulo.nombre == nombreModulo && p.nombre == nombreParametro
                              select p).SingleOrDefault();
                mensajeResultado.resultado = parametros;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosPorModuloYParametro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getParametrosYListCrucePorModuloYClaves(string claveModulo, object[] clavesParametros, DBContextAdapter dbContext)
        {
            List<Object[]> listParametrosYListCruce = new List<Object[]>();
            List<Parametros> listparametros;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listparametros = (from p in getSession().Set<Parametros>()
                                  where p.modulo.clave == claveModulo && clavesParametros.Contains(p.clave)
                                  select p).ToList();
                if (listparametros.Count > 0)
                {
                    for (int i = 0; i < listparametros.Count; i++)
                    {
                        List<Cruce> values;//Si el parametro no tiene seleccionado elementos de aplicacion quiere decir que no se va filtrar o profuncidar por algun elemento de aplicacion
                        if (listparametros[i].elementosAplicacion == null ? false : listparametros[i].elementosAplicacion.Count > 0)
                        {
                            values = (from c in getSession().Set<Cruce>()
                                      where c.parametros.clave == listparametros[i].clave && listparametros[i].elementosAplicacion.Contains(c.elementosAplicacion)
                                      orderby c.elementosAplicacion.ordenId descending
                                      select c).ToList();
                        }
                        else
                        {
                            values = new List<Cruce>();
                        }
                        Object[] objects = new Object[2];
                        objects[0] = listparametros[i];
                        objects[1] = values;
                        listParametrosYListCruce.Add(objects);
                        values = null;
                    }
                }
                mensajeResultado.resultado = listParametrosYListCruce;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosYListCrucePorModuloYClaves()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteParametros(List<Parametros> AgreModif, List<Parametros> eliminados, int rango, DBContextAdapter dbContext)
        {
            List<Parametros> parametros = new List<Parametros>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (eliminados.Count>0) {
                    foreach (Parametros con in eliminados)
                    {
                        con.elementosAplicacion = null;
                        getSession().Set<Parametros>().AddOrUpdate(con);
                        getSession().Set<Parametros>().Attach(con);
                        getSession().Set<Parametros>().Remove(con);
                    }
                }
                AgreModif = (AgreModif == null ? new List<Parametros>() : AgreModif);
                if (commit && AgreModif.Count>0) {
                    parametros= agregarListaParametros(AgreModif, rango);
                }
                if (commit)
                {
                    mensajeResultado.resultado = parametros;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else {
                    getSession().Database.CurrentTransaction.Rollback();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteParametros()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private List<Parametros> agregarListaParametros(List<Parametros> entitys, int rango)
        {
            List<Parametros> parametros = new List<Parametros>();
            try
            {
                int i = 0;
                for (i = 0; i < entitys.Count; i++)
                {
                    if (entitys[i].id == 0)
                    {
                        parametros.Add(getSession().Set<Parametros>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<Parametros>().AddOrUpdate(entitys[i]);
                    }

                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaParametros()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                commit = false;
            }
            return parametros;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getExisteClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                getSession().Database.CurrentTransaction.Rollback();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje getParametrosPorRango(int inicio, int end,int? clasificacion, DBContextAdapter dbContext)
        {
            try
            {
                int Count = 0;
                object[] arreglo = new object[2];
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                
                if (clasificacion == null)
                {
                    if (inicio == 0)
                    {
                        Count = (from p in getSession().Set<Parametros>()
                                 select p).Count();
                    }
                    var parametros = (from p in getSession().Set<Parametros>()
                                      orderby p.clave
                                      select new
                                      {
                                          p.clasificacion,
                                          p.clave,
                                          elementosAplicacion = p.elementosAplicacion.Select(ea => new
                                          {
                                              ea.id,
                                              ea.clave,
                                              ea.entidad,
                                              ea.nombre,
                                              ea.ordenId,
                                              ea.parentId
                                          }).ToList(),
                                          p.id,
                                          p.imagen,
                                          p.modulo_ID,
                                          p.nombre,
                                          p.opcionesParametros,
                                          p.ordenId,
                                          p.propiedadConfig,
                                          p.tipoConfiguracion,
                                          p.valor
                                      }).Skip(inicio).Take(end).ToList();
                    arreglo[1] = parametros;
                }
                else {
                    if (inicio == 0)
                    {
                        Count = (from p in getSession().Set<Parametros>()
                                 where p.clasificacion == clasificacion
                                 select p).Count();
                    }
                    var parametros = (from p in getSession().Set<Parametros>()
                                      where p.clasificacion==clasificacion
                                      orderby p.clave
                                      select new
                                      {
                                          p.clasificacion,
                                          p.clave,
                                          elementosAplicacion = p.elementosAplicacion.Select(ea => new
                                          {
                                              ea.id,
                                              ea.clave,
                                              ea.entidad,
                                              ea.nombre,
                                              ea.ordenId,
                                              ea.parentId
                                          }).ToList(),
                                          p.id,
                                          p.imagen,
                                          p.modulo_ID,
                                          p.nombre,
                                          p.opcionesParametros,
                                          p.ordenId,
                                          p.propiedadConfig,
                                          p.tipoConfiguracion,
                                          p.valor
                                      }).Skip(inicio).Take(end).ToList();
                    arreglo[1] = parametros;
                }
              
               
                arreglo[0] = Count;
               
                mensajeResultado.resultado = arreglo;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosPorRango1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                getSession().Database.CurrentTransaction.Rollback();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje getPorIdParametro(decimal? idParametro, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var parametros = (from p in getSession().Set<Parametros>()
                                  where p.id==idParametro
                                  select new
                                  {
                                      p.clasificacion,
                                      p.clave,
                                      elementosAplicacion = p.elementosAplicacion.Select(ea => new {
                                          ea.id,
                                          ea.clave,
                                          ea.entidad,
                                          ea.nombre,
                                          ea.ordenId,
                                          ea.parentId
                                      }).ToList(),
                                      p.id,
                                      p.imagen,
                                      p.modulo_ID,
                                      p.nombre,
                                      p.opcionesParametros,
                                      p.ordenId,
                                      p.propiedadConfig,
                                      p.tipoConfiguracion,
                                      p.valor
                                  }).SingleOrDefault();
                mensajeResultado.resultado = parametros;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getParametrosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}