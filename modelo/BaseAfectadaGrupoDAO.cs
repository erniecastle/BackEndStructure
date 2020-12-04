/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: clase BaseAfectadaGrupoDAO para llamados a metodos de Entity
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.util;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class BaseAfectadaGrupoDAO : GenericRepository<BaseAfectadaGrupo>, BaseAfectadaGrupoDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private List<BaseAfectadaGrupo> listbaseafecgru = new List<BaseAfectadaGrupo>();
        

        public Mensaje agregar(BaseAfectadaGrupo entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<BaseAfectadaGrupo>().Add(entity);
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

        public Mensaje modificar(BaseAfectadaGrupo entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<BaseAfectadaGrupo>().AddOrUpdate(entity);
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

        public Mensaje eliminar(BaseAfectadaGrupo entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<BaseAfectadaGrupo>().Attach(entity);
                getSession().Set<BaseAfectadaGrupo>().Remove(entity);
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
        public Mensaje agregarListaBaseAfectadaGrupo(List<BaseAfectadaGrupo> entitys, int rango, DBContextAdapter dbContext)
        {
            listbaseafecgru.Clear();
            try
            {
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                int i = 0;
                for (i = 0; i < entitys.Count; i++)
                {
                    if (entitys[i].id == 0)
                    {
                        listbaseafecgru.Add(getSession().Set<BaseAfectadaGrupo>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<BaseAfectadaGrupo>().AddOrUpdate(entitys[i]);
                    }

                     if (i % rango == 0 & i > 0)
                     {
                         getSession().SaveChanges();
                     }
                }
                mensajeResultado.resultado = listbaseafecgru;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaBaseAfectadaGrupo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosBaseAfectadaGrupo(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
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

        //public Mensaje deleteListQuerys(string tabla, string campo, object[] valores, DBContextAdapter dbContext)
        //{
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        setSession(dbContext);
        //        getSession().Database.BeginTransaction();
        //        //deleteListQuery(tabla, campo, valores);
        //        mensajeResultado.resultado = true;
        //        mensajeResultado.noError = 0;
        //        mensajeResultado.error = "";
        //        getSession().Database.CurrentTransaction.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
        //        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
        //        mensajeResultado.error = ex.GetBaseException().ToString();
        //        mensajeResultado.resultado = null;
        //        getSession().Database.CurrentTransaction.Rollback();
        //    }
        //    return mensajeResultado;
        //}

        //public Mensaje existeDato(string campo, object valor, DBContextAdapter dbContext)
        //{
        //    bool existe = false;
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        setSession(dbContext);
        //        getSession().Database.BeginTransaction();
        //        //existe = existeDato("BaseAfectadaGrupo", campo, valor);
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

        public Mensaje getAllBaseAfectadaGrupo(DBContextAdapter dbContext)
        {
          //  List<BaseAfectadaGrupo> listabaseafecgru = new List<BaseAfectadaGrupo>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listabaseafecgru = (from a in getSession().Set<BaseAfectadaGrupo>() select new {
                    a.baseNomina_ID,
                    a.formulaExenta,
                    a.grupo_ID,
                    a.id,
                    a.periodoExentoISR,
                    a.tipoAfecta
                }).ToList();
                mensajeResultado.resultado = listabaseafecgru;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("BaseAfectadaGrupoAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveBaseAfectadaGrupo(string clave, DBContextAdapter dbContext)
        {

            //Esta función no es usada en el cliente y en la consulta 
            //hace una comparacion con el campo de clave pero la tabla de 
            //BaseAfectadaGrupo no contiene dicho campo


            /* BaseAfectadaGrupo baseafectgru = new BaseAfectadaGrupo();
             try
             {
                 inicializaVariableMensaje();
                 setSession(dbContext);
                 getSession().Database.BeginTransaction();
                 baseafectgru = (from b in getSession().Set<BaseAfectadaGrupo>()
                           where b.clave == clave
                           select b).SingleOrDefault();
                 mensajeResultado.resultado = baseafectgru;
                 mensajeResultado.noError = 0;
                 mensajeResultado.error = "";
                 getSession().Database.CurrentTransaction.Commit();
             }
             catch (Exception ex)
             {
                 System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("BaseAfectadaGrupoPorClave()1_Error: ").Append(ex));
                 mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                 mensajeResultado.error = ex.GetBaseException().ToString();
                 mensajeResultado.resultado = null;
                 getSession().Database.CurrentTransaction.Rollback();
             }*/
            return mensajeResultado;
        }
    }
}