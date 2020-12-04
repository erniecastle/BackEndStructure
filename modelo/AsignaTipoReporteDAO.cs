/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: clase AsignaTipoReporteDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad;
using System.Linq;
using Exitosw.Payroll.Core.util;
using System.Text;
using System.Reflection;
using System.Data.Entity.Migrations;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class AsignaTipoReporteDAO : GenericRepository<AsignaTipoReporte>, AsignaTipoReporteDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        bool commit = true;
        public Mensaje agregar(AsignaTipoReporte entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<AsignaTipoReporte>().Add(entity);
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

        public Mensaje actualizar(AsignaTipoReporte entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<AsignaTipoReporte>().AddOrUpdate(entity);
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

        public Mensaje eliminar(AsignaTipoReporte entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<AsignaTipoReporte>().Attach(entity);
                getSession().Set<AsignaTipoReporte>().Remove(entity);
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

        //public Mensaje existeDato(string tabla, CamposWhere campoWhere, Conexion conexion)
        //{
            
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        setSession(dbContext);
        //        getSession().Database.BeginTransaction();
        //        mensajeResultado = existeDato("AsignaTipoReporte", campoWhere, conexion);
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

        public Mensaje getAsignaPorTipoReporte(TipoReporte tipoReporte, DBContextAdapter dbContext)
        {
            AsignaTipoReporte tiporeporte = new AsignaTipoReporte();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                tiporeporte = (from a in getSession().Set<AsignaTipoReporte>()
                               where a.tipoReporte == tipoReporte
                               select a).SingleOrDefault();
                mensajeResultado.resultado = tiporeporte;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AsignaTipoReporteAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAsignaPorTipoReporteDinamico(string claveReporteDinamico, DBContextAdapter dbContext)
        {
            AsignaTipoReporte tiporeporteDina = new AsignaTipoReporte();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                tiporeporteDina = (from a in getSession().Set<AsignaTipoReporte>()
                                   where a.reporteDinamico.clave == claveReporteDinamico
                                   select a).SingleOrDefault();
                mensajeResultado.resultado = tiporeporteDina;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AsignaTipoReporteAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAsignaTipoReporteAll(DBContextAdapter dbContext)
        {
            List<AsignaTipoReporte> listaguinaldopagos = new List<AsignaTipoReporte>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listaguinaldopagos = (from a in getSession().Set<AsignaTipoReporte>() select a).ToList();
                mensajeResultado.resultado = listaguinaldopagos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AsignaTipoReporteAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteAsignaTipoReporte(List<AsignaTipoReporte> entitysCambios, object[] clavesDelete, int rango, DBContextAdapter dbContext)
        {
            List<AsignaTipoReporte> listtiporeport = new List<AsignaTipoReporte>();
            try
            {

                inicializaVariableMensaje();
                clavesDelete = clavesDelete == null ? new object[] { } : clavesDelete;
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete.Count() > 0)
                {
                    commit = deleteListQuerys(typeof(AsignaTipoReporte).Name, new CamposWhere("AsignaTipoReporte.id",clavesDelete,OperadorComparacion.IN,OperadorLogico.AND), dbContext);
                }
                entitysCambios = (entitysCambios == null ? new List<AsignaTipoReporte>() : entitysCambios);
                if (commit && entitysCambios.Count() > 0)
                {
                    listtiporeport = agregarListaAsignaTipoReporte(entitysCambios, rango);
                }
                if (commit)
                {
                    mensajeResultado.resultado = listtiporeport;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteAsignaTipoReporte()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private List<AsignaTipoReporte> agregarListaAsignaTipoReporte(List<AsignaTipoReporte> entitys, int rango)
        {
            List<AsignaTipoReporte> listasigna = new List<AsignaTipoReporte>();
            try
            {
                int i;
                for (i = 0; i < entitys.Count(); i++)
                {
                    if (entitys[i].id == 0)
                    {
                        listasigna.Add(getSession().Set<AsignaTipoReporte>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<AsignaTipoReporte>().AddOrUpdate(entitys[i]);
                    }
                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaAsignaTipoReporte()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                commit = false;
            }
            return listasigna;
        }
        private bool deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            try
            {
                deleteListQuery(tabla, campoWhere, dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return true;
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
    }
}