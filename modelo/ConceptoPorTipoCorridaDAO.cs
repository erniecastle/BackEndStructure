/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ConceptoPorTipoCorridaDAO para llamados a metodos de Entity
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
    public class ConceptoPorTipoCorridaDAO : GenericRepository<ConceptoPorTipoCorrida>, ConceptoPorTipoCorridaDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<ConceptoPorTipoCorrida> listConceptoPorTipoCorrida = new List<ConceptoPorTipoCorrida>();

        public Mensaje agregar(ConceptoPorTipoCorrida entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConceptoPorTipoCorrida>().Add(entity);
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

        public Mensaje modificar(ConceptoPorTipoCorrida entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConceptoPorTipoCorrida>().AddOrUpdate(entity);
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

        public Mensaje eliminar(ConceptoPorTipoCorrida entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<ConceptoPorTipoCorrida>().Attach(entity);
                getSession().Set<ConceptoPorTipoCorrida>().Remove(entity);
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

        public Mensaje consultaPorRangosConceptoPorTipoCorrida(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
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

        //public Mensaje existeDato(string campo, object valor, DBContextAdapter dbContext)
        //{
        //    bool existe = false;
        //    try
        //    {
        //        inicializaVariableMensaje();
        //        setSession(dbContext);
        //        getSession().Database.BeginTransaction();
        //        //existe = existeDato("ConceptoPorTipoCorrida", campo, valor);
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

        public Mensaje getConceptoPorTipoCorrida(TipoCorrida tipoCorrida, DBContextAdapter dbContext)
        {
            listConceptoPorTipoCorrida = new List<ConceptoPorTipoCorrida>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listConceptoPorTipoCorrida = (from ctc in getSession().Set<ConceptoPorTipoCorrida>()
                                              where ctc.tipoCorrida.id == tipoCorrida.id &&
                                              ctc.concepNomDefi.fecha ==
                                              (from cnd in getSession().Set<ConcepNomDefi>()
                                               select new { cnd.fecha }).Max(p => p.fecha) &&
                                               ctc.concepNomDefi.activado == true
                                              select ctc).ToList();
                mensajeResultado.resultado = listConceptoPorTipoCorrida;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConceptoPorTipoCorrida1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllConceptoPorTipoCorrida(DBContextAdapter dbContext)
        {
            //listConceptoPorTipoCorrida = new List<ConceptoPorTipoCorrida>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listConceptoPorTipoCorrida = (from a in getSession().Set<ConceptoPorTipoCorrida>() select new {
                    a.cantidad,
                    a.concepNomDefi_ID,
                    a.id,
                    a.incluir,
                    a.modificarCantidad,
                    a.modificarImporte,
                    a.mostrar,
                    a.opcional,
                    a.tipoCorrida_ID
                }).ToList();
                mensajeResultado.resultado = listConceptoPorTipoCorrida;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConceptoPorTipoCorridaAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getConceptoPorTipoCorridaMostrarActivo(string tipoCorrida, DBContextAdapter dbContext)
        {
            listConceptoPorTipoCorrida = new List<ConceptoPorTipoCorrida>(); try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listConceptoPorTipoCorrida = (from a in getSession().Set<ConceptoPorTipoCorrida>()
                                              where a.tipoCorrida.clave == tipoCorrida &&
                                              a.mostrar == true
                                              orderby a.concepNomDefi.clave
                                              select a).ToList();
                mensajeResultado.resultado = listConceptoPorTipoCorrida;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConceptoPorTipoCorridaMostrarActivo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteConceptoPorTipoCorrida(List<ConceptoPorTipoCorrida> entitysCambios, object[] eliminados, DBContextAdapter dbContext)
        {
            listConceptoPorTipoCorrida = new List<ConceptoPorTipoCorrida>();
            bool exitoRegristro = true;
            setSession(dbContext.context);
            try
            {
                getSession().Database.BeginTransaction();
                if (eliminados != null && eliminados.Count() > 0)
                {
                    exitoRegristro = deleteListQuerys("ConceptoPorTipoCorrida", new CamposWhere("ConceptoPorTipoCorrida.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                entitysCambios = (entitysCambios == null) ? new List<ConceptoPorTipoCorrida>() : entitysCambios;
                if(exitoRegristro && entitysCambios.Any())
                {
                    for (int i = 0; i < entitysCambios.Count; i++)
                    {
                        getSession().Set<ConceptoPorTipoCorrida>().AddOrUpdate(entitysCambios[i]);
                        getSession().SaveChanges();
                        listConceptoPorTipoCorrida.Add(entitysCambios[i]);
                    }
                }
                if (exitoRegristro){
                    mensajeResultado.resultado = listConceptoPorTipoCorrida;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuery()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }
        private bool deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                //deleteListQuery("ConceptoPorTipoCorrida", "Id", eliminados);
                 deleteListQuery(tabla, campoWhere, dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuery()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return exito;
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

        public Mensaje getConceptoPorTipoCorridaIdClavesConcep(decimal? tipoCorrida, List<string> clavesconcep, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listaConcep = (from cct in getSession().Set<ConceptoPorTipoCorrida>()
                                   where cct.tipoCorrida_ID == tipoCorrida
                                   select cct.concepNomDefi_ID).ToList();
                var listConceptoPorTipoCorrida = (from c in getSession().Set<ConcepNomDefi>()
                                                  where !(listaConcep).Contains(c.id) && !(clavesconcep).Contains(c.clave)
                                                  select new
                                                  {
                                                      c.clave,
                                                      c.descripcion,
                                                      c.descripcionAbreviada,
                                                      c.id
                                                  }).ToList();
                mensajeResultado.resultado = listConceptoPorTipoCorrida;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getConceptoPorTipoCorridaId()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getConceptoPorTipoCorridaId(decimal? idTipoCorrida, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listConceptoPorTipoCorrida = (from a in getSession().Set<ConceptoPorTipoCorrida>()
                                                  where a.tipoCorrida_ID == idTipoCorrida
                                                  orderby a.concepNomDefi.clave ascending
                                                  select new
                                                  {
                                                      a.cantidad,
                                                      a.concepNomDefi_ID,
                                                      concepNomDefi = new
                                                      {
                                                          a.concepNomDefi.clave,
                                                          a.concepNomDefi.descripcion,
                                                          a.concepNomDefi.id
                                                      },
                                                      a.id,
                                                      a.incluir,
                                                      a.modificarCantidad,
                                                      a.modificarImporte,
                                                      a.mostrar,
                                                      a.opcional,
                                                      a.tipoCorrida_ID,
                                                      a.descuentoCreditos
                                                  }).ToList();
                mensajeResultado.resultado = listConceptoPorTipoCorrida;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ConceptoPorTipoCorridaAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
    }
}