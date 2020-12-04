/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ConfiguracionAsistenciasDAO para llamados a metodos de Entity
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
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class ConfiguracionAsistenciasDAO : GenericRepository<ConfigAsistencias>, ConfiguracionAsistenciasDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<ConfigAsistencias> listConfigAsistencias = new List<ConfigAsistencias>();
        bool commit;
        private int order;

        public Mensaje agregar(ConfigAsistencias entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<Excepciones> lista = new List<Excepciones>();
                foreach (var item in entity.excepciones)
                {
                    Excepciones a = getSession().Set<Excepciones>().Find(item.id);
                    lista.Add(a);
                }
                entity.excepciones = null;
                entity.excepciones = lista;
                getSession().Set<ConfigAsistencias>().Add(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
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
        public Mensaje actualizar(ConfigAsistencias entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<Excepciones> listaaux = new List<Excepciones>();
                List<Excepciones> lista = entity.excepciones;
                entity.excepciones = null;
                getSession().Entry(entity).State = EntityState.Modified;
                getSession().SaveChanges();
                ConfigAsistencias param = getSession().Set<ConfigAsistencias>().Include(a => a.excepciones).ToList().Find(ca => ca.id == entity.id);

                param.excepciones.Clear();
                for (int i = 0; i < lista.Count; i++)
                {
                    Excepciones a = getSession().Set<Excepciones>().Find(lista[i].id);
                    listaaux.Add(a);
                }
                param.excepciones = listaaux;

                getSession().SaveChanges();
                getSession().Set<ConfigAsistencias>().AddOrUpdate(entity);
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
        public Mensaje eliminar(ConfigAsistencias entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfigAsistencias>().Attach(entity);
                getSession().Set<ConfigAsistencias>().Remove(entity);
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

        public Mensaje buscaConfiguracionAsistenciasSistema(decimal id, DBContextAdapter dbContext)
        {
           // ConfigAsistencias configAsistencias = new ConfigAsistencias();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               var configAsistencias = (from ca in getSession().Set<ConfigAsistencias>()
                                     where ca.id == id &&
                                     ca.sistema == true
                                     select new {
                                         ca.activadosFiltro,
                                         ca.activadosMovimientos,
                                         ca.compartir,
                                         ca.contenedorPadre_ID,
                                         ca.filtro,
                                         ca.habilitado,
                                         ca.icono,
                                         ca.id,
                                         ca.keyCode,
                                         ca.modifiers,
                                         ca.movimiento,
                                         ca.nombre,
                                         ca.ordenId,
                                         ca.perfiles_ID,
                                         ca.razonesSociales_ID,
                                         ca.sistema,
                                         ca.usuario_ID,
                                         ca.visible,
                                         excepciones=ca.excepciones.Select(p =>new {
                                             p.clave,
                                             p.concepNomDefi_ID,
                                             p.excepcion,
                                             p.id,
                                             p.naturaleza,
                                             p.orden,
                                             p.tipoDatoExcepcion,
                                             p.unico

                                         })

                                     }).SingleOrDefault();
                mensajeResultado.resultado = configAsistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaConfiguracionAsistenciasSistema()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje buscaPorIdYRazonSocial(decimal id, string claveRazonSocial, DBContextAdapter dbContext)
        {
            ConfigAsistencias configAsistencias = new ConfigAsistencias();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                configAsistencias = (from ca in getSession().Set<ConfigAsistencias>()
                                     where ca.id == id &&
                                     ca.razonesSociales.clave.Equals(claveRazonSocial)
                                     select ca).SingleOrDefault();
                mensajeResultado.resultado = configAsistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaConfiguracionAsistenciasSistema()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllConfiguracionAsistenciasSistema(DBContextAdapter dbContext)
        {
            List<ConfigAsistencias> listConfigAsistencias = new List<ConfigAsistencias>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listConfigAsistencias = (from a in getSession().Set<ConfigAsistencias>()
                                         where a.sistema == true &&
                                         a.razonesSociales != null
                                         select a).ToList();
                mensajeResultado.resultado = listConfigAsistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AllConfiguracionAsistenciasSistema()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getConfiguracionAsistenciasAll(string claveRazonesSocial, DBContextAdapter dbContext)
        {
            listConfigAsistencias = new List<ConfigAsistencias>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listConfigAsistencias = (from a in getSession().Set<ConfigAsistencias>()
                                         where a.razonesSociales.clave.Equals(claveRazonesSocial)
                                         orderby a.ordenId ascending
                                         select a).ToList();
                mensajeResultado.resultado = listConfigAsistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConfiguracionAsistenciasAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getExcepcionesPorConfiguracionAsistencias(decimal id, DBContextAdapter dbContext)
        {
            ConfigAsistencias configAsistencias = new ConfigAsistencias();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                configAsistencias = (from ca in getSession().Set<ConfigAsistencias>()
                                     where ca.id == id
                                     select ca).SingleOrDefault();
                mensajeResultado.resultado = configAsistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ExcepcionesPorConfiguracionAsistencias()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje PorGrupoMenu(string idContenedor, string claveRazonSocial, DBContextAdapter dbContext)
        {
            listConfigAsistencias = new List<ConfigAsistencias>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listConfigAsistencias = (from c in getSession().Set<ConfigAsistencias>()
                                         where c.contenedorPadre_ID.Equals(idContenedor) &&
                                         c.razonesSociales.clave.Equals(claveRazonSocial)
                                         orderby c.ordenId ascending
                                         select c).ToList();
                mensajeResultado.resultado = listConfigAsistencias;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("PorGrupoMenu()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteConfiguracionAsistencias(List<ConfigAsistencias> AgreModif, List<ConfigAsistencias> Ordenados, List<ConfigAsistencias> eliminados, DBContextAdapter dbContext)
        {
            listConfigAsistencias = new List<ConfigAsistencias>();
            inicializaVariableMensaje();
            setSession(dbContext.context);
            try
            {
                commit = true;
                getSession().Database.BeginTransaction();
                if (eliminados.Count() > 0)
                {
                    foreach (ConfigAsistencias con in eliminados)
                    {
                        con.excepciones = null;
                        getSession().Set<ConfigAsistencias>().Add(con);
                        getSession().SaveChanges();
                    }
                    Object[] clavesConf = new Object[eliminados.Count()];
                    for (int i = 0; i < eliminados.Count(); i++)
                    {
                        clavesConf[i] = eliminados[i].id;
                    }
                    clavesConf = clavesConf == null ? new Object[0] : clavesConf;
                    if (clavesConf.Count() > 0)
                    {
                        deleteListQuerys("ConfiguracionAsistencias", "id", clavesConf);

                    }
                }
                AgreModif = (AgreModif == null ? new List<ConfigAsistencias>() : AgreModif);
                if(AgreModif.Count() > 0)
                {
                    if(commit && !AgreModif.Any())
                    {
                        listConfigAsistencias= agregarListaConfiguraAsistencias(AgreModif, 100, false);
                    }
                }
                Ordenados = (Ordenados == null ? new List<ConfigAsistencias>() : Ordenados);
                if(Ordenados.Count() > 0)
                {
                    if(commit && !Ordenados.Any())
                    {
                        listConfigAsistencias.AddRange(agregarListaConfiguraAsistencias(Ordenados, 100, true));
                    }
                }
                if (commit) { 
                    mensajeResultado.resultado = listConfigAsistencias;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteConfiguracionAsistencias()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }
        private List<ConfigAsistencias> agregarListaConfiguraAsistencias(List<ConfigAsistencias> entitys, int rango, bool ordenado)
        {
            if (listConfigAsistencias != null || listConfigAsistencias.Count()>0)
            {
                listConfigAsistencias.Clear();
            }
            try
            {
                commit = true;
                int i = 0;
                order = (ordenado == false ? 0 : Convert.ToInt32(entitys[0].id));
                StringBuilder accion = new StringBuilder(0);
                for(i=0; i <entitys.Count(); i++)
                {
                    if(entitys[i].id == 0)
                    {
                        if (ordenado)
                        {
                            entitys[i].ordenId = order;
                            order++;
                        }
                        getSession().Set<ConfigAsistencias>().Add(entitys[i]);
                        getSession().SaveChanges();
                        listConfigAsistencias.Add(entitys[i]);
                    }else
                    {
                        if (ordenado)
                        {
                            entitys[i].ordenId = order;
                            order++;
                        }
                        if (entitys[i].cambio)
                        {
                            getSession().Set<ConfigAsistencias>().AddOrUpdate(entitys[i]);
                        }else
                        {
                            getSession().Set<ConfigAsistencias>().AddOrUpdate(entitys[i]);
                        }
                        if (i % rango == 0 & i > 0)
                        {
                            getSession().SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return listConfigAsistencias;
        }

        private void deleteListQuerys(String tabla, String campo, Object[] valores)
        {
            try
            {
                commit = true;
                consulta = new StringBuilder("delete ");
                consulta.Append(tabla).Append(" where ").Append(campo).Append(" in(@valores)");
                int noOfRowDeleted = getSession().Database.ExecuteSqlCommand(consulta.ToString(), new SqlParameter("@valores", valores));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                commit = false;
            }
        }

        public Mensaje getConfiguracionAsistenciaPorRango(int[] values, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                int start = values[0];
                int end = values[1];

                setSession(dbContext.context);

                getSession().Database.BeginTransaction();
                var myList = (from asis in getSession().Set<ConfigAsistencias>()
                              orderby asis.id ascending
                              select new
                              {
                                  asis.id,
                                  asis.activadosFiltro,
                                  asis.activadosMovimientos,
                                  asis.filtro,
                                  asis.movimiento,
                                  asis.nombre
                              }
                             ).Skip(start).Take(end).ToList();

                int count = (from a in getSession().Set<ConfigAsistencias>()
                             select a).Count();
                object[] valores = new object[2];
                valores[0] = myList;
                valores[1] = count;
                mensajeResultado.resultado = valores;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
                // getSession().Database.Connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConfiguracionMovimientoPorRango()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }
    }
}