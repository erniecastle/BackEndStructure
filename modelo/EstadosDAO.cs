/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase EstadosDAO para llamados a metodos de Entity
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
    public class EstadosDAO : GenericRepository<Estados>, EstadosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<Estados> listaEstados = new List<Estados>();
        bool commit;
        public Mensaje agregar(Estados entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Estados>().Add(entity);
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
        //public Mensaje modificar(Estados entity, DBContextAdapter dbContext)
        //{
        //    return modificar2(entity, "", true);
        //}
        public Mensaje modificar(Estados entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //getSession().Set<Estados>().Attach(entity);
                getSession().Set<Estados>().AddOrUpdate(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

                //getSession().SaveChanges();
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

        public Mensaje eliminar(Estados entity, DBContextAdapter dbContext)
        {

            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Estados>().Attach(entity);
                getSession().Set<Estados>().Remove(entity);
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
        public Mensaje getPorClaveEstados(string clave, DBContextAdapter dbContext)
        {
            //Paises paises;
            //Estados estados;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var estados = (from m in getSession().Set<Estados>()
                               where m.clave == clave
                               select new
                               {
                                   id = m.id,
                                   descripcion = m.descripcion,
                                   clave = m.clave,
                                   centroDeCosto = m.centroDeCosto,
                                   empleados = m.empleados,
                                   empleados_estadoNacimiento = m.empleados_estadoNacimiento,
                                   municipios = m.municipios.Select(a => new
                                   {

                                       id = a.id,
                                       clave = a.clave,
                                       descripcion = a.descripcion,
                                       centroDeCosto = a.centroDeCosto,
                                       ciudades = a.ciudades,
                                       empleados = a.empleados,
                                       estados = a.estados,
                                       estados_ID = a.estados_ID,
                                       razonesSociales = a.razonesSociales,
                                       registroPatronal = a.registroPatronal
                                   }).ToList(),
                                   paises = m.paises,
                                   paises_ID = m.paises_ID,
                                   razonesSociales = m.razonesSociales,
                                   registroPatronal = m.registroPatronal

                               }).FirstOrDefault();
                mensajeResultado.resultado = estados;
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
        public Mensaje consultaPorFiltrosEstados(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listaEstados = new List<Estados>();
            ValoresRango rangos;
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
                        campo.campo = "Estados." + item.Key.ToString();
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
                if (Convert.ToInt32(rango) > 0)
                {
                    rangos = new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));
                }
                else
                {
                    rangos = null;
                }
                //  ValoresRango rangos = new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));
                mensajeResultado.resultado = consultaPorRangos(rangos, camposwheres, null);
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosEstado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosEstados(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listaEstados = new List<Estados>();
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

        public Mensaje getAllEstados(DBContextAdapter dbContext)
        {
            listaEstados = new List<Estados>();
            //List<object> esta = new List<object>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var esta = (from e in getSession().Set<Estados>()
                            select new
                            {
                                id = e.id,
                                clave = e.clave,
                                descripcion = e.descripcion,
                                paises_ID = e.paises_ID,
                            }).ToList();
                mensajeResultado.resultado = esta;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("EstadosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getEstadosPorPais(string clavePais, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listaEstados = (from e in getSession().Set<Estados>()
                                    where e.paises.clave.Equals(clavePais)
                                    select new
                                    {
                                        id = e.id,
                                        clave = e.clave,
                                        descripcion = e.descripcion,
                                        paises_ID = e.paises_ID
                                    }).ToList();
                mensajeResultado.resultado = listaEstados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getEstadosPorPais()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getPorPaisEstados(string clavePais, DBContextAdapter dbContext)
        {
            //listaEstados = new List<Estados>(); 
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listaEstados = (from e in getSession().Set<Estados>()
                                    where e.paises.clave.Equals(clavePais)
                                    select new
                                    {
                                        id = e.id,
                                        clave = e.clave,
                                        descripcion = e.descripcion,
                                        paises = e.paises,
                                        paises_ID = e.paises_ID
                                    }).ToList();
                mensajeResultado.resultado = listaEstados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("EstadosPorPais()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje saveDeleteEstados(List<object> entitysCambios, List<object> clavesDelete, int rango, DBContextAdapter dbContext)
        {
            listaEstados = new List<Estados>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                object[] clavesDel = new object[clavesDelete.Count];
                for (int i = 0; i < clavesDelete.Count; i++)
                {
                    clavesDel[i] = ((Estados)clavesDelete[i]).id;// clavesDelete[i].id;
                }
                if (clavesDel != null && clavesDel.Length > 0)
                {
                    //deleteListQuerys("Estados", "Clave", clavesDelete, "");
                    deleteListQuerys("Estados", new CamposWhere("Estados.id", clavesDel, OperadorComparacion.IN, OperadorLogico.AND), null);
                    //getSession().SaveChanges();

                }
                entitysCambios = (entitysCambios == null ? new List<object>() : entitysCambios);
                if (commit && entitysCambios.Any())
                {
                    listaEstados = agregarListaEstados(entitysCambios, rango, dbContext);
                }
                if (commit)
                {
                    mensajeResultado.resultado = listaEstados;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteEstados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private List<Estados> agregarListaEstados(List<object> entitys, int rango, DBContextAdapter dbContext)
        {
            listaEstados.Clear();
            try
            {
                int i = 0;
                for (i = 0; i < entitys.Count(); i++)
                {
                    if (((Estados)entitys[i]).id == 0)
                    {
                        listaEstados.Add(getSession().Set<Estados>().Add((Estados)entitys[i]));
                    }
                    else
                    {
                        getSession().Set<Estados>().AddOrUpdate((Estados)entitys[i]);
                    }
                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                    getSession().SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaEstados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return listaEstados;
        }

        private void deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            try
            {
                // deleteListQuery(tabla, campo, valores);
                mensajeResultado = deleteListQuery(tabla, campoWhere, dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteConfigConceptosSat()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
        }

        public Mensaje getPorIdEstados(decimal? idEstados, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var estados = (from m in getSession().Set<Estados>()
                               where m.id == idEstados
                               select new
                               {
                                   id = m.id,
                                   descripcion = m.descripcion,
                                   clave = m.clave,
                                   //centroDeCosto = m.centroDeCosto,
                                   //empleados = m.empleados,
                                   //empleados_estadoNacimiento = m.empleados_estadoNacimiento,
                                   //municipios = m.municipios.Select(a => new
                                   //{

                                   //    id = a.id,
                                   //    clave = a.clave,
                                   //    descripcion = a.descripcion,
                                   //    centroDeCosto = a.centroDeCosto,
                                   //    ciudades = a.ciudades,
                                   //    empleados = a.empleados,
                                   //    estados = a.estados,
                                   //    estados_ID = a.estados_ID,
                                   //    razonesSociales = a.razonesSociales,
                                   //    registroPatronal = a.registroPatronal
                                   //}).ToList(),
                                   //paises = m.paises,
                                   paises_ID = m.paises_ID,
                                   //razonesSociales = m.razonesSociales,
                                   //registroPatronal = m.registroPatronal

                               }).FirstOrDefault();
                mensajeResultado.resultado = estados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdEstados()1_Error: ").Append(ex));
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
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }
    }
}