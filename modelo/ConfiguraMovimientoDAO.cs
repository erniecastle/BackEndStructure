/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ConfiguraMovimientoDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class ConfiguraMovimientoDAO : GenericRepository<ConfiguraMovimiento>, ConfiguraMovimientoDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<ConfiguraMovimiento> listConfMov = new List<ConfiguraMovimiento>();
        bool commit;

        public Mensaje agregar(ConfiguraMovimiento entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguraMovimiento>().Add(entity);
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

        public Mensaje actualizar(ConfiguraMovimiento entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguraMovimiento>().AddOrUpdate(entity);
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

        public Mensaje eliminar(ConfiguraMovimiento entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConfiguraMovimiento>().Attach(entity);
                getSession().Set<ConfiguraMovimiento>().Remove(entity);
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
        public Mensaje buscaConfiguracionMovimSistema(decimal id, DBContextAdapter dbContext)
        {
           // ConfiguraMovimiento configMov = new ConfiguraMovimiento();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var configMov = (from cm in getSession().Set<ConfiguraMovimiento>()
                                 where cm.id == id
                                 //cm.sistema == true
                                 select new
                                 {
                                     cm.activadosFiltro,
                                     cm.activadosMovimientos,
                                     cm.compartir,
                                     cm.contenedorPadre_ID,
                                     cm.filtro,
                                     cm.habilitado,
                                     cm.icono,
                                     cm.id,
                                     cm.keyCode,
                                     cm.modifiers,
                                     cm.movimiento,
                                     cm.movimientoExistente,
                                     cm.nombre,
                                     cm.ordenId,
                                     cm.razonesSociales_ID,
                                     cm.sistema,
                                     cm.visible
                                 }).SingleOrDefault();
                mensajeResultado.resultado = configMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaConfiguracionMovimSistema()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje buscaPorIdYRazonSocial(decimal id, string claveRazonSocial, DBContextAdapter dbContext)
        {
            ConfiguraMovimiento configMovporidyrazon = new ConfiguraMovimiento();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                configMovporidyrazon = (from cm in getSession().Set<ConfiguraMovimiento>()
                                        where cm.id == id &&
                                        cm.razonesSociales.clave.Equals(claveRazonSocial)
                                        select cm).SingleOrDefault();
                mensajeResultado.resultado = configMovporidyrazon;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaPorIdYRazonSocial()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje  consultaPorRangosConfiguraMovimiento(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listConfMov = new List<ConfiguraMovimiento>();

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

        //public Mensaje existeDato(string campo, object valor, DbContext dbContext)
        //{
        //    bool existe = false;
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        setSession(dbContext);
        //        getSession().Database.BeginTransaction();
        //        //existe = existeDato("ConfiguraMovimiento", campo, valor);
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

        public Mensaje getAllConfiguracionMovimSistema(DBContextAdapter dbContext)
        {
            //listConfMov = new List<ConfiguraMovimiento>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listConfMov = (from cm in getSession().Set<ConfiguraMovimiento>()
                                       //where cm.sistema == true
                                       //&&cm.razonesSociales != null
                                   select new
                                   {
                                       cm.id,
                                       cm.nombre,
                                       cm.activadosFiltro,
                                       cm.activadosMovimientos,
                                       cm.filtro,
                                       cm.movimientoExistente,
                                       cm.movimiento

                                   }).ToList();
                mensajeResultado.resultado = listConfMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AllConfiguracionMovimSistema()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getConfiguraMovimientoAll(string claveRazonesSocial, DBContextAdapter dbContext)
        {
            listConfMov = new List<ConfiguraMovimiento>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listConfMov = (from cm in getSession().Set<ConfiguraMovimiento>()
                               where cm.razonesSociales.clave.Equals(claveRazonesSocial)
                               orderby cm.id
                               select cm).ToList();
                mensajeResultado.resultado = listConfMov;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConfiguraMovimientoAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje PorGrupoMenu(string idContenedor, string claveRazonSocial, DBContextAdapter dbContext)
        {
            listConfMov = new List<ConfiguraMovimiento>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listConfMov = (from cm in getSession().Set<ConfiguraMovimiento>()
                               where cm.contenedorPadre_ID.Equals(idContenedor) &&
                               cm.razonesSociales.clave.Equals(claveRazonSocial)
                               select cm).ToList();
                mensajeResultado.resultado = listConfMov;
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

        public Mensaje saveDeleteConfiguraMovimiento(List<ConfiguraMovimiento> entitysCambios, object[] clavesDelete, int rango, DBContextAdapter dbContext)
        {
            listConfMov = new List<ConfiguraMovimiento>();
            inicializaVariableMensaje();
            setSession(dbContext.context);
            bool commit = true;
            try
            {
                getSession().Database.BeginTransaction();
                clavesDelete = clavesDelete == null ? new Object[0] : clavesDelete;
                if (clavesDelete.Count() > 0)
                {
                    String[] claves;
                    decimal[] ids = new decimal[clavesDelete.Count()];
                    int i = 0;
                    for (i = 0; i < clavesDelete.Count(); i++)
                    {
                        claves = clavesDelete[i].ToString().Split(',');
                        ids[i] = Convert.ToDecimal(claves[0]);
                    }
                    commit = deleteListQuerys("ConfiguraMovimiento", "id", ids);
                }
                entitysCambios = (entitysCambios == null ? new List<ConfiguraMovimiento>() : entitysCambios);
                if(commit && !entitysCambios.Any())
                {
                    for(int i=0; i < entitysCambios.Count(); i++)
                    {
                        if(entitysCambios[i].id== 0)
                        {
                            getSession().Set<ConfiguraMovimiento>().Add(entitysCambios[i]);
                            listConfMov.Add(entitysCambios[i]);
                        }
                        else
                        {
                            getSession().Set<ConfiguraMovimiento>().Add(entitysCambios[i]);
                        }
                        getSession().SaveChanges();
                    }
                }
                if (commit)
                {
                    mensajeResultado.resultado = listConfMov;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteConfiguraMovimiento()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            throw new NotImplementedException();
        }
        private bool deleteListQuerys(String tabla, String campo, decimal[] valores)
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
            return commit;

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

        public Mensaje getConfiguracionMovimientoPorRango(int[] values, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                int start = values[0];
                int end = values[1];
                
                  setSession(dbContext.context);
                
                getSession().Database.BeginTransaction();
                var myList = (from cm in getSession().Set<ConfiguraMovimiento>()
                              orderby cm.id ascending
                              select new
                              {
                                  cm.id,
                                  cm.nombre,
                                  cm.activadosFiltro,
                                  cm.activadosMovimientos,
                                  cm.filtro,
                                  cm.movimientoExistente,
                                  cm.movimiento
                              }
                             ).Skip(start).Take(end).ToList();

                int count = (from a in getSession().Set<ConfiguraMovimiento>()
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