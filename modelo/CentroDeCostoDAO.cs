/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase CentroDeCostosDAO para llamados a metodos de Entity
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
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.util;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class CentroDeCostoDAO : GenericRepository<CentroDeCosto>, CentroDeCostoDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<CentroDeCosto> listCentroDeCosto = new List<CentroDeCosto>();
        bool commit = false;

        public Mensaje agregar(CentroDeCosto entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CentroDeCosto>().Add(entity);
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

        public Mensaje modificar(CentroDeCosto entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CentroDeCosto>().AddOrUpdate(entity);
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

        public Mensaje eliminar(CentroDeCosto entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<CentroDeCosto>().Attach(entity);
                getSession().Set<CentroDeCosto>().Remove(entity);
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

        public Mensaje consultaPorFiltrosCentroDeCosto(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listCentroDeCosto = new List<CentroDeCosto>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                List<CamposWhere> camposwheres = new List<CamposWhere>();
                foreach (var item in campos)
                {
                    if (!item.Value.ToString().Equals("") && item.Value != null)
                    {
                        CamposWhere campo = new CamposWhere();
                        campo.campo = "CentroDeCosto." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosCentroCosto()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosCentroDeCosto(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listCentroDeCosto = new List<CentroDeCosto>();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorRangosCentroCosto()1_Error: ").Append(ex));
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
        //        //existe = existeDato("CentroDeCosto", campo, valor);
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

        public Mensaje getCentroDeCostoPorClaveYRazon(string clave, string claveRazon, DBContextAdapter dbContext)
        {
            CentroDeCosto centroDeCosto = new CentroDeCosto();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                centroDeCosto = (from b in getSession().Set<CentroDeCosto>()
                          where b.clave == clave &&
                          b.razonesSociales.clave == clave
                          select b).SingleOrDefault();
                mensajeResultado.resultado = centroDeCosto;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CentroDeCostoPorClaveYRazon()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteCentroDeCosto(List<CentroDeCosto> entitysCambios, object[] clavesDelete, int rango, DBContextAdapter dbContext)
        {
            listCentroDeCosto = new List<CentroDeCosto>();       
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if(clavesDelete != null)
                {
                    //deleteListQuerys("CentroDeCosto", "Clave", clavesDelete);
                    deleteListQuery("CentroDeCosto", new CamposWhere("CentroDeCosto.clave", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                entitysCambios = (entitysCambios == null ? new List<CentroDeCosto>() : entitysCambios);
                if(commit && entitysCambios.Count() > 0)
                {
                    listCentroDeCosto= agregarListaCentrosDeCostos(entitysCambios, rango);
                }
                if (commit)
                {
                    mensajeResultado.resultado = listCentroDeCosto;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteCentroDeCosto()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private List<CentroDeCosto> agregarListaCentrosDeCostos(List<CentroDeCosto> entitys, int rango)
        {
            if(listCentroDeCosto.Count > 0)
            {
                listCentroDeCosto.Clear();
            }
            try
            {
                int i = 0;
                for(i=0;i < entitys.Count(); i++)
                {
                    if (entitys[i].id == 0)
                    {
                        listCentroDeCosto.Add(getSession().Set<CentroDeCosto>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<CentroDeCosto>().AddOrUpdate(entitys[i]);
                    }
                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaCentrosDeCostos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return listCentroDeCosto;
        }
        public Mensaje getAllCentroDeCosto(string claveRazonesSocial, DBContextAdapter dbContext)
        {
           // listCentroDeCosto = new List<CentroDeCosto>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
              var  listCentroDeCosto = (from a in getSession().Set<CentroDeCosto>()
                                     select new {
                                        // a.asistencias,
                                         a.calle,
                                        // a.ciudades,
                                         a.ciudades_ID,
                                         a.clave,
                                         a.colonia,
                                        // a.cp,
                                         a.cp_ID,
                                         a.descripcion,
                                         a.descripcionPrevia,
                                        // a.detalleAsistencia,
                                        // a.estados,
                                         a.estados_ID,
                                         a.experienciaLaboralInterna,
                                         a.id,
                                        // a.movNomConcep,
                                         //a.municipios,
                                         a.municipios_ID,
                                         a.numeroExterior,
                                         a.numeroInterior,
                                        // a.paises,
                                         a.paises_ID,
                                       //  a.plazas,
                                        // a.plazasPorEmpleadosMov,
                                         //a.razonesSociales,
                                         a.razonesSociales_ID,
                                         //a.registroPatronal,
                                         a.registroPatronal_ID,
                                         a.subCuenta,
                                         a.telefono,
                                        // a.tipoCentroCostos,
                                         a.tipoCentroCosto_ID
                                         

                                     }).ToList();
                mensajeResultado.resultado = listCentroDeCosto;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("CentroDeCostoAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorIdCentroDeCosto(decimal? id, string claveRazon, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listCentroDeCosto = (from a in getSession().Set<CentroDeCosto>()
                                         where a.id == id && a.razonesSociales.clave.Equals(claveRazon)
                                         select new
                                         {
                                             
                                             a.calle,
                                            
                                             a.ciudades_ID,
                                             a.clave,
                                             a.colonia,
                                             a.cp_ID,
                                             a.descripcion,
                                             a.descripcionPrevia,
                                           
                                             a.estados_ID,
                                           
                                             a.id,
                                             a.municipios_ID,
                                             a.numeroExterior,
                                             a.numeroInterior,
                                             a.paises_ID,
                                             a.razonesSociales_ID,
                                             a.registroPatronal_ID,
                                             a.subCuenta,
                                             a.telefono,
                                             a.tipoCentroCosto_ID
                                         }).SingleOrDefault();
                mensajeResultado.resultado = listCentroDeCosto;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdCentroDeCosto()1_Error: ").Append(ex));
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