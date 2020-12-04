/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase RazonesSocialesDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.campos;
using Exitosw.Payroll.Core.genericos.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class RazonesSocialesDAO : GenericRepository<RazonesSociales>, RazonesSocialesDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(RazonesSociales entity, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            RazonesSociales razonesSociales;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                razonesSociales = getSession().Set<RazonesSociales>().Add(entity);
                getSession().SaveChanges();
                getSession().Database.CurrentTransaction.Commit();
                getSession().Database.Connection.Close();

                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                RazonSocial razonSocial = new RazonSocial();
                razonSocial.claveRazonSocial = razonesSociales.clave;
                razonSocial.nombreRazonSocial = razonesSociales.razonsocial;
                getSession().Set<RazonSocial>().AddOrUpdate(razonSocial);
                getSession().SaveChanges();
                mensajeResultado.resultado = razonesSociales;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
                getSession().Database.Connection.Close();
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
        public Mensaje modificar(RazonesSociales entity, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<RazonesSociales>().AddOrUpdate(entity);
                getSession().SaveChanges();
                getSession().Database.CurrentTransaction.Commit();
                getSession().Database.Connection.Close();

                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                RazonSocial razonsocial = (from r in getSession().Set<RazonSocial>()
                                           where r.claveRazonSocial == entity.clave
                                           select r).SingleOrDefault();
                razonsocial.nombreRazonSocial = entity.razonsocial;
                getSession().Set<RazonSocial>().AddOrUpdate(razonsocial);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
                getSession().Database.Connection.Close();
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
        public Mensaje eliminar(RazonesSociales entity, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                string clave = entity.clave;
                getSession().Set<RazonesSociales>().Attach(entity);
                getSession().Set<RazonesSociales>().Remove(entity);
                getSession().SaveChanges();
                getSession().Database.CurrentTransaction.Commit();
                getSession().Database.Connection.Close();

                setSession(dbContextMaestra.context);
                getSession().Database.BeginTransaction();
                RazonSocial razonsocial = (from r in getSession().Set<RazonSocial>()
                                           where r.claveRazonSocial == clave
                                           select r).SingleOrDefault();
                getSession().Set<RazonSocial>().Attach(razonsocial);
                getSession().Set<RazonSocial>().Remove(razonsocial);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
                getSession().Database.Connection.Close();


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
        public Mensaje consultaPorFiltroIN(string query, object[] campos, object[] valores, DBContextAdapter dbContext)
        {
            List<RazonesSociales> razonesSociales = new List<RazonesSociales>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                ///no se usa en el cliente
                mensajeResultado.resultado = razonesSociales;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltroIN()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje consultaPorFiltrosRazonesSociales(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<RazonesSociales> razonesSociales = new List<RazonesSociales>();
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
                        campo.campo = "RazonesSociales." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosRazonSocial()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje consultaPorRangosRazonesSociales(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<RazonesSociales> razonesSociales = new List<RazonesSociales>();
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

        public Mensaje existeRFC(string rfc, DBContextAdapter dbContext)
        {
            bool exite = false;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                RazonesSociales razonesSociales = (from p in getSession().Set<RazonesSociales>()
                                                   where
                                                      p.rfc == rfc
                                                   select p).SingleOrDefault();
                if (razonesSociales != null)
                {
                    exite = true;
                }
                mensajeResultado.resultado = exite;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRazonesAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getAllRazonesSociales(DBContextAdapter dbContext)
        {
            //List<RazonesSociales> razonesSociales = new List<RazonesSociales>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var razonesSociales = (from p in getSession().Set<RazonesSociales>()
                                       select new
                                       {
                                           p.calle,
                                           p.certificadoSAT,
                                           p.ciudades_ID,
                                           p.clave,
                                           p.colonia,
                                           p.configuraTimbrado_ID,
                                           p.cp_ID,
                                           p.descripcionRecibo,
                                           p.estados_ID,
                                           p.folio,
                                           p.id,
                                           p.llaveSAT,
                                           p.municipios_ID,
                                           p.numeroex,
                                           p.numeroin,
                                           p.paises_ID,
                                           p.password,
                                           p.razonsocial,
                                           p.regimenFiscal,
                                           p.representantelegal,
                                           p.rfc,
                                           p.rutaCert,
                                           p.rutaLlave,
                                           p.series,
                                           p.telefono,
                                           p.ubicacionXML
                                       }).ToList();
                mensajeResultado.resultado = razonesSociales;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRazonesAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getPorClaveRazonesSociales(string clave, DBContextAdapter dbContext)
        {
            //RazonesSociales razoneSociale;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var razoneSociale = (from p in getSession().Set<RazonesSociales>()
                                     where p.clave == clave
                                     select new
                                     {
                                         p.calle,
                                         p.certificadoSAT,
                                         p.ciudades_ID,
                                         p.clave,
                                         p.colonia,
                                         p.configuraTimbrado_ID,
                                         p.cp_ID,
                                         p.descripcionRecibo,
                                         p.estados_ID,
                                         p.folio,
                                         p.id,
                                         p.llaveSAT,
                                         p.municipios_ID,
                                         p.numeroex,
                                         p.numeroin,
                                         p.paises_ID,
                                         p.password,
                                         p.razonsocial,
                                         p.regimenFiscal,
                                         p.representantelegal,
                                         p.rfc,
                                         p.rutaCert,
                                         p.rutaLlave,
                                         p.series,
                                         p.telefono,
                                         p.ubicacionXML
                                     }).SingleOrDefault();
                mensajeResultado.resultado = razoneSociale;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRazonesPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getSeriePorRazonesSociales(string clave, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var razoneSociale = (from p in getSession().Set<RazonesSociales>()
                                     where p.clave == clave
                                     select new
                                     {
                                         p.id,
                                         p.razonsocial,
                                         p.series.serie

                                     }).SingleOrDefault();
                mensajeResultado.resultado = razoneSociale;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getSeriePorRazonesSociales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje getRazonesSocialesPorClaves(string[] claveRazonesSociales, DBContextAdapter dbContext)
        {
            List<RazonesSociales> razonesSociales = new List<RazonesSociales>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                razonesSociales = (from p in getSession().Set<RazonesSociales>()
                                   where claveRazonesSociales.Contains(p.clave)
                                   select p).ToList();
                mensajeResultado.resultado = razonesSociales;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRazonesSocialesPorClaves()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje saveDeleteRazonesSociales(List<RazonesSociales> entitysCambios, object[] clavesDelete, int rango, DBContextAdapter dbContext)
        {
            List<RazonesSociales> razonesSociales = new List<RazonesSociales>();
            try
            {
                commit = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (clavesDelete != null)
                {
                    commit = deleteListQuerys("RazonesSociales", new CamposWhere("RazonesSociales.clave", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    //deleteListQuerys("RazonesSociales", "Clave", clavesDelete);
                }
                entitysCambios = (entitysCambios == null ? new List<RazonesSociales>() : entitysCambios);
                if (commit && entitysCambios.Count > 0)
                {
                    razonesSociales = agregarListaRazonesSociales(entitysCambios, rango);
                }

                if (commit)
                {
                    getSession().SaveChanges();
                    mensajeResultado.resultado = razonesSociales;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteRazonesSociales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        bool commit = true;
        private List<RazonesSociales> agregarListaRazonesSociales(List<RazonesSociales> entitys, int rango)
        {
            List<RazonesSociales> razonesSociales = new List<RazonesSociales>();
            try
            {
                commit = true;
                int i;
                for (i = 0; i < entitys.Count; i++)
                {
                    if (entitys[i].id == 0)
                    {
                        razonesSociales.Add(getSession().Set<RazonesSociales>().Add(entitys[i]));
                    }
                    else
                    {
                        getSession().Set<RazonesSociales>().Add(entitys[i]);
                    }
                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaRazonesSociales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                commit = false;
            }
            return razonesSociales;
        }
        private bool deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            bool exito = true;
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
                exito = false;
            }
            return exito;
        }

        public Mensaje getPorIdRazonesSociales(decimal? id, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var razonSocial = (from rs in getSession().Set<RazonesSociales>()
                                   where rs.id == id
                                   select new
                                   {
                                       rs.calle,
                                       rs.certificadoSAT,
                                       ciudades = rs.ciudades == null ? null : new { rs.ciudades.descripcion },
                                       rs.ciudades_ID,
                                       rs.clave,
                                       rs.colonia,
                                       rs.configuraTimbrado_ID,
                                       rs.cp_ID,
                                       rs.descripcionRecibo,
                                       estados = rs.estados == null ? null : new { rs.estados.descripcion },
                                       rs.estados_ID,
                                       rs.folio,
                                       rs.id,
                                       rs.llaveSAT,
                                       rs.municipios_ID,
                                       rs.numeroex,
                                       rs.numeroin,
                                       rs.paises_ID,
                                       rs.password,
                                       rs.razonsocial,
                                       rs.regimenFiscal,
                                       rs.representantelegal,
                                       rs.rfc,
                                       rs.rutaCert,
                                       rs.rutaLlave,
                                       rs.series_ID,
                                       rs.telefono,
                                       rs.ubicacionXML
                                   }).SingleOrDefault();
                mensajeResultado.resultado = razonSocial;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdRazonesSociales()1_Error: ").Append(ex));
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