/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: clase BaseAfectadaConceptoNominaDAO para llamados a metodos de Entity
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
using System.Reflection;
using System.Text;
using System.Linq;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;

namespace Exitosw.Payroll.Core.modelo
{
    public class BaseAfectadaConceptoNominaDAO : GenericRepository<BaseAfecConcepNom>, BaseAfectadaConceptoNominaDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private List<BaseAfecConcepNom> listbaseafeconcep = new List<BaseAfecConcepNom>();
        public Mensaje agregar(BaseAfecConcepNom entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<BaseAfecConcepNom>().Add(entity);
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
        public Mensaje modificar(BaseAfecConcepNom entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<BaseAfecConcepNom>().AddOrUpdate(entity);
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

        public Mensaje eliminar(BaseAfecConcepNom entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<BaseAfecConcepNom>().Attach(entity);
                getSession().Set<BaseAfecConcepNom>().Remove(entity);
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

        public Mensaje agregarListaBaseAfectadaConceptoNomina(List<BaseAfecConcepNom> entitys, int rango, DBContextAdapter dbContext)
        {
            listbaseafeconcep.Clear();
            try
            {
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                int i = 0;
                for (i=0; i < entitys.Count; i++)
                {
                    if(entitys[i].id == 0)
                    {
                        listbaseafeconcep.Add(getSession().Set<BaseAfecConcepNom>().Add(entitys[i]));
                    }
                    else
                    {
                       getSession().Set<BaseAfecConcepNom>().AddOrUpdate(entitys[i]);
                    }

                     if(i % rango ==0 & i > 0)
                     {
                         getSession().SaveChanges();
                     }
                }
                mensajeResultado.resultado = listbaseafeconcep;
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

        //public Mensaje consultaPorRangos(int inicio, int rango, DBContextAdapter dbContext)
        //{
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        setSession(dbContext);
        //        getSession().Database.BeginTransaction();
        //        //listbaseafeconcep = (List<BaseAfecConcepNom>)consultaPorRangos(inicio, rango, null, null);
        //        mensajeResultado.resultado = listbaseafeconcep;
        //        mensajeResultado.noError = 0;
        //        mensajeResultado.error = "";
        //        getSession().Database.CurrentTransaction.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorRangos()1_Error: ").Append(ex));
        //        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
        //        mensajeResultado.error = ex.GetBaseException().ToString();
        //        mensajeResultado.resultado = null;
        //        getSession().Database.CurrentTransaction.Rollback();
        //    }
        //    return mensajeResultado;
        //}

        //public Mensaje deleteListQuerys(string tabla, string campo, object[] valores, DBContextAdapter dbContext)
        //{
        //    try
        //    {
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
        //        //existe = existeDato("BaseAfectadaConceptoNomina", campo, valor);
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

        public Mensaje getAllBaseAfecConcepNom(DBContextAdapter dbContext)
        {
            //List<BaseAfecConcepNom> listabaseafec = new List<BaseAfecConcepNom>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               var listabaseafec = (from a in getSession().Set<BaseAfecConcepNom>() select new {
                   a.baseNomina_ID,
                   a.concepNomDefin_ID,
                   a.formulaExenta,
                   a.id,
                   a.tipoAfecta,
                   a.periodoExentoISR
                }).ToList();
                mensajeResultado.resultado = listabaseafec;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("BaseAfectadaConceptoNominaAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveBaseAfecConcepNom(string clave, DBContextAdapter dbContext)
        {

            //Esta función no es usada en el cliente y en la consulta 
            //hace una comparacion con el campo de clave pero la tabla de 
            //BaseAfecConcepNom no contiene dicho campo



            /* BaseAfecConcepNom baseafec = new BaseAfecConcepNom();
             try
             {
                 inicializaVariableMensaje();
                 setSession(dbContext);
                 getSession().Database.BeginTransaction();
                 baseafec = (from b in getSession().Set<BaseAfecConcepNom>()
                           where  b.clave== clave
                           select b).SingleOrDefault();
                 mensajeResultado.resultado = baseafec;
                 mensajeResultado.noError = 0;
                 mensajeResultado.error = "";
                 getSession().Database.CurrentTransaction.Commit();
             }
             catch (Exception ex)
             {
                 System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("AguinaldoFechasPorClave()1_Error: ").Append(ex));
                 mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                 mensajeResultado.error = ex.GetBaseException().ToString();
                 mensajeResultado.resultado = null;
                 getSession().Database.CurrentTransaction.Rollback();
             }*/
            return mensajeResultado;
        }
    }
}