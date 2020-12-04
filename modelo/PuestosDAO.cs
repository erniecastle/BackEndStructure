/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase PuestosDAO para llamados a metodos de Entity
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
using System.Data.Entity;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class PuestosDAO : GenericRepository<Puestos>, PuestosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(Puestos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Puestos>().Add(entity);
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
        public Mensaje modificar(Puestos entity, DBContextAdapter dbContext)
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
                getSession().Set<Puestos>().AddOrUpdate(entity);
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

        public Mensaje eliminar(Puestos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<Puestos>().Attach(entity);
                getSession().Set<Puestos>().Remove(entity);
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
        public Mensaje agregarListaPuestos(List<Puestos> entitys, int rango, DBContextAdapter dbContext)
        {
            List<Puestos> puestos = new List<Puestos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i < entitys.Count; i++)
                {
                    if (entitys[i].id == 0)
                    {
                        puestos.Add(getSession().Set<Puestos>().Add(entitys[i]));
                    }
                    else {
                        getSession().Set<Puestos>().AddOrUpdate(entitys[i]);
                    }
                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                getSession().SaveChanges();
                mensajeResultado.resultado = puestos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaPuestos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorFiltrosPuestos(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<Puestos> puestos = new List<Puestos>();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosPuesto()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosPuestos(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<Puestos> puestos = new List<Puestos>();
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

        public Mensaje deleteListQuerys(string tabla, string campo, object[] valores, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                deleteListQuery(tabla, new CamposWhere(tabla + "." + campo, valores, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQuerys()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje DeletePuesto(Puestos entity, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //exito = deleteListQuery(session, "PercepcionesFijas", "puestos_id", entity.getId());
                Mensaje mensaje = deleteListQuery("PercepcionesFijas", new CamposWhere("PercepcionesFijas.puestos.id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                exito = (bool)mensaje.resultado;

                if (exito)
                {
                    Mensaje mensaje2 = deleteListQuery("Puestos", new CamposWhere("Puestos.id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                    exito = (bool)mensaje2.resultado;
                    // exito = deleteListQuery(session, "Puestos", "id", entity.getId());

                }
                if (exito)
                {
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("DeletePuesto()1_Error: ").Append(ex));
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

        public Mensaje getAllPuestos(DBContextAdapter dbContext)
        {
            //List<Puestos> puestos = new List<Puestos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var puestos = (from p in getSession().Set<Puestos>()
                               select new
                               {
                                   p.categoriasPuestos_ID,
                                   p.clave,
                                   p.descripcion,
                                   p.descripcionPrevia,
                                   p.directivo,
                                   p.funciones,
                                   p.id,
                                   p.maximo,
                                   p.minimo,
                                   p.registroPatronal_ID,
                                   p.salarioTabular
                               }).ToList();
                mensajeResultado.resultado = puestos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPuestosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClavePuestos(string clave, DBContextAdapter dbContext)
        {
            Puestos puesto;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                puesto = (from p in getSession().Set<Puestos>()
                          where p.clave == clave
                          select p).SingleOrDefault();
                mensajeResultado.resultado = puesto;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPuestosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje SavePuesto(List<PercepcionesFijas> agrega, object[] eliminados, Puestos entity, DBContextAdapter dbContext)
        {
            bool exitoRegistro = true;
            Puestos puesto = null;

            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (eliminados.Length > 0)
                {
                    // exitoRegistro = deleteListQueryEn("", "PercepcionesFijas", "id", eliminados);
                    Mensaje mensaje = deleteListQuery("PercepcionesFijas", new CamposWhere("PercepcionesFijas.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    exitoRegistro = (bool)mensaje.resultado;
                }
                if (exitoRegistro)
                {
                    getSession().Set<Puestos>().AddOrUpdate(entity);

                    agrega = agrega == null ? new List<PercepcionesFijas>() : agrega;
                    for (int i = 0; i < agrega.Count; i++)
                    {
                        agrega[i].puestos = entity;
                        getSession().Set<PercepcionesFijas>().AddOrUpdate(agrega[i]);
                        if (i % 100 == 0 && i > 0)
                        {
                            getSession().SaveChanges();
                        }
                    }
                }
                if (exitoRegistro)
                {
                    puesto = entity;
                    getSession().SaveChanges();
                    mensajeResultado.resultado = puesto;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("SavePuesto()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje UpdatePuesto(List<PercepcionesFijas> agrega, object[] eliminados, Puestos entity, DBContextAdapter dbContext)
        {
            bool exito = true;

            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (eliminados.Length > 0)
                {
                    //exito = deleteListQueryEn(session, "PercepcionesFijas", "id", eliminados);
                    Mensaje mensaje = deleteListQuery("PercepcionesFijas", new CamposWhere("PercepcionesFijas.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    exito = (bool)mensaje.resultado;
                }
                if (exito)
                {
                    getSession().Set<Puestos>().AddOrUpdate(entity);
                    agrega = agrega == null ? new List<PercepcionesFijas>() : agrega;
                    for (int i = 0; i < agrega.Count; i++)
                    {
                        agrega[i].puestos = entity;
                        getSession().Set<PercepcionesFijas>().AddOrUpdate(agrega[i]);
                        if (i % 100 == 0 && i > 0)
                        {
                            getSession().SaveChanges();
                        }
                    }
                }
                if (exito)
                {

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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("UpdatePuesto()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorIdPuestos(decimal? id, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var puestos = (from p in getSession().Set<Puestos>()
                               where p.id == id
                               select new
                               {
                                   p.categoriasPuestos_ID,
                                   p.clave,
                                   p.descripcion,
                                   p.descripcionPrevia,
                                   p.directivo,
                                   p.funciones,
                                   p.id,
                                   p.maximo,
                                   p.minimo,
                                   p.registroPatronal_ID,
                                   p.salarioTabular
                               }).SingleOrDefault();
                mensajeResultado.resultado = puestos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdPuestos()1_Error: ").Append(ex));
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