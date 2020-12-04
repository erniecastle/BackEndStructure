/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase CategoriasPuestosDAO para llamados a metodos de Entity
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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class CategoriasPuestosDAO : GenericRepository<CategoriasPuestos>, CategoriasPuestosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<CategoriasPuestos> listCategoriasPuestos = new List<CategoriasPuestos>();

        public Mensaje agregar(CategoriasPuestos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CategoriasPuestos>().Add(entity);
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

        public Mensaje modificar(CategoriasPuestos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CategoriasPuestos>().AddOrUpdate(entity);
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

        public Mensaje eliminar(CategoriasPuestos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CategoriasPuestos>().Attach(entity);
                getSession().Set<CategoriasPuestos>().Remove(entity);
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

        public Mensaje consultaPorRangosCategoriasPuestos(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
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

        public Mensaje DeleteCategoriaPuesto(CategoriasPuestos entity, DBContextAdapter dbContext)
        {
            bool exito = true;
            inicializaVariableMensaje();
            setSession(dbContext.context);
            getSession().Database.BeginTransaction();
            try
            {
                // deleteListQuery(getSession(), "PercepcionesFijas", "categoriasPuestos_id", entity.getId());
                deleteListQuery("PercepcionesFijas", new CamposWhere("PercepcionesFijas.categoriasPuestos.id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                // deleteListQuery(getSession(), "CategoriasPuestos", "id", entity.getId());
                deleteListQuery("CategoriasPuestos", new CamposWhere("categoriasPuestos.id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                if (exito)
                {
                    mensajeResultado.resultado = true;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("DeleteCategoriaPuesto()1_Error: ").Append(ex));
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
        //        //existe = existeDato("CategoriasPuestos", campo, valor);
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

        public Mensaje getAllCategoriasPuestos(DBContextAdapter dbContext)
        {
           // listCategoriasPuestos = new List<CategoriasPuestos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               var listCategoriasPuestos = (from a in getSession().Set<CategoriasPuestos>()
                                     select new {
                                         a.clave,
                                         a.descripcion,
                                         a.estado,
                                         a.id,
                                         a.pagarPorHoras,
                                         a.tablaBase
                                     }).ToList();
                mensajeResultado.resultado = listCategoriasPuestos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CategoriasPuestosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje SaveCategoriaPuesto(List<PercepcionesFijas> agrega, object[] eliminados, CategoriasPuestos entity, DBContextAdapter dbContext)
        {
            bool exitoRegistro = true;
            inicializaVariableMensaje();
            setSession(dbContext.context);
            CategoriasPuestos categoriaPuesto = null;
            try
            {
                getSession().Database.BeginTransaction();
                if(eliminados.Count() > 0)
                {
                    // deleteListQueryEn(getSession(), "PercepcionesFijas", "id", eliminados);
                    deleteListQuery("PercepcionesFijas", new CamposWhere("PercepcionesFijas.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                getSession().Set<CategoriasPuestos>().Add(entity);
                getSession().SaveChanges();
                for(int i=0; i <agrega.Count(); i++)
                {
                    agrega[i].categoriasPuestos = entity;
                    getSession().Set<CategoriasPuestos>().Add(entity);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                if (exitoRegistro)
                {
                    categoriaPuesto = entity;
                    mensajeResultado.resultado = categoriaPuesto;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("SaveCategoriaPuesto()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteCategoriasPuestos(List<CategoriasPuestos> entitysCambios, object[] clavesDelete, int rango, DBContextAdapter dbContext)
        {
            listCategoriasPuestos = new List<CategoriasPuestos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if(clavesDelete != null)
                {
                    //deleteListQuerys("CategoriasPuestos", "Clave", clavesDelete);
                    deleteListQuery("CategoriasPuestos", new CamposWhere("CategoriasPuestos.clave", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if(listCategoriasPuestos != null)
                {
                    mensajeResultado.resultado = listCategoriasPuestos;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }else
                {
                    getSession().Database.CurrentTransaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteCategoriasPuestos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback(); ;
            }
            return mensajeResultado;
        }
        private void deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
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
                listCategoriasPuestos = null;
            }
        }

        public Mensaje UpdateCategoriaPuesto(List<PercepcionesFijas> agrega, object[] eliminados, CategoriasPuestos entity, DBContextAdapter dbContext)
        {
            bool exito = true;
            inicializaVariableMensaje();
            setSession(dbContext.context);
            try
            {
                getSession().Database.BeginTransaction();
                if(eliminados.Count() > 0)
                {
                    //deleteListQueryEn(getSession(), "PercepcionesFijas", "id", eliminados);
                    deleteListQuery("PercepcionesFijas", new CamposWhere("PercepcionesFijas.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                getSession().Set<CategoriasPuestos>().AddOrUpdate(entity);
                getSession().SaveChanges();
                for(int i=0; i < agrega.Count(); i++)
                {
                    agrega[i].categoriasPuestos = entity;
                    getSession().Set<CategoriasPuestos>().AddOrUpdate(entity);
                    if (i % 100 == 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                if (exito)
                {
                    mensajeResultado.resultado = true;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("UpdateCategoriaPuesto()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorIdCategoriasPuestos(decimal? idCategoriasPuestos, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var CatPuesto = (from cp in getSession().Set<CategoriasPuestos>() 
                                 where cp.id==idCategoriasPuestos
                                 select new {
                                     cp.id,
                                     cp.clave,
                                     cp.descripcion,
                                     cp.pagarPorHoras,
                                     cp.estado
                                 }).FirstOrDefault();
                mensajeResultado.resultado = CatPuesto;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdCategoriasPuestos()1_Error: ").Append(ex));
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
                mensajeResultado = existeClave(tabla,campoWhere,null);
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