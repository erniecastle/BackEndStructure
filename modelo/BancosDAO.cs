    /**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: clase BancosDAO para llamados a metodos de Entity
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
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Reflection;
using System.Text;
using System.Linq;
using Exitosw.Payroll.Core.genericos.campos;
using Newtonsoft.Json;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class BancosDAO : GenericRepository<Bancos>, BancosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private List<Bancos> listBancos = new List<Bancos>();
        private List<Contactos> listContactos = new List<Contactos>();

        public Mensaje agregar(Bancos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Bancos>().Add(entity);
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

        public Mensaje modificar(Bancos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Bancos>().AddOrUpdate(entity);
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

        public Mensaje eliminar(Bancos entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Bancos>().Attach(entity);
                getSession().Set<Bancos>().Remove(entity);
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

        public Mensaje agregarListaBancos(List<Bancos> cambios, List<Bancos> temporales, List<Contactos> cambioContactos, object[] clavesContactosDelete, int rango, DBContextAdapter dbContext)
        {
            listContactos.Clear();
            listBancos.Clear();
            Bancos bancos = new Bancos(); //r
            Contactos contactos = new Contactos(); //p
            ArrayList nEntitys = new ArrayList();
            int i = 0, j = 0;
            inicializaVariableMensaje();
            setSession(dbContext.context);
            try
            {
                getSession().Database.BeginTransaction();
                for (i = 0; i < cambios.Count; i++)
                {
                    for (j = 0; j < temporales.Count; j++)
                    {
                        if (temporales[j].id == cambios[i].id)
                        {
                            cambios[i].id = 0;
                            temporales.RemoveAt(j);
                            j--;
                        }
                    }
                    if (cambios[i].id > 0)
                    {
                        bancos = getSession().Set<Bancos>().Add(cambios[i]);
                        listBancos.Add(bancos);
                        foreach (Contactos contactos2 in cambioContactos)
                        {
                            if (contactos2.bancos.clave == bancos.clave)
                            {
                                contactos2.bancos = bancos;
                                contactos = getSession().Set<Contactos>().Add(contactos2);
                                listContactos.Add(contactos);
                            }
                        }
                    }
                    else
                    {
                        getSession().Set<Bancos>().Add(cambios[i]);
                        bool nPrima = false;
                        foreach (Contactos contactos2 in cambioContactos)
                        {
                            if (contactos2.bancos.id == cambios[i].id)
                            {
                                if (contactos2.id > 0)
                                {
                                    nPrima = true;
                                }
                                getSession().Set<Contactos>().Add(contactos2);
                                if (nPrima)
                                {
                                    contactos = contactos2;
                                    listContactos.Add(contactos);
                                }
                            }
                        }
                    }

                    if (i % rango == 0 & i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                bool exito = true;
                if (clavesContactosDelete != null)
                {
                    //exito = deleteListQuerys(Contactos.class.getSimpleName(), "id", clavesContactosDelete);
                    Mensaje mensaje = deleteListQuery(typeof(Contactos).Name, new CamposWhere("Contactos.id", clavesContactosDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    exito = (bool)mensaje.resultado;
                }
                if (exito)
                {
                    nEntitys.Add(listBancos);
                    nEntitys.Add(listContactos);
                    mensajeResultado.resultado = nEntitys;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    mensajeResultado.resultado = null;
                    getSession().Database.CurrentTransaction.Rollback();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaBancos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorFiltrosBancos(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
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
                        campo.campo = "Bancos." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosBancos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosBancos(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
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
        public Mensaje SaveBanco(List<Contactos> agrega, object[] eliminados, Bancos entity, DBContextAdapter dbContext)
        {
            bool exitoRegistro = true;
            Bancos banco = new Bancos();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (eliminados.Count() > 0)
                {
                    // deleteListQueryEn(getSession(), "Contactos", "id", eliminados);
                    Mensaje mensaje = deleteListQuery(typeof(Contactos).Name, new CamposWhere("Contactos.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                getSession().Set<Bancos>().AddOrUpdate(entity);
                getSession().SaveChanges();
                for (int i = 0; i < agrega.Count(); i++)
                {
                    agrega[i].bancos = entity;
                    getSession().Set<Contactos>().AddOrUpdate(agrega[i]);
                    if (i % 100 == 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                if (exitoRegistro)
                {
                    banco = entity;
                    mensajeResultado.resultado = banco;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("SaveBanco()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje UpdateBanco(List<Contactos> agrega, object[] eliminados, Bancos entity, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (eliminados.Count() > 0)
                {
                    //deleteListQueryEn(getSession(), "Contactos", "id", eliminados);
                    Mensaje mensaje = deleteListQuery(typeof(Contactos).Name, new CamposWhere("Contactos.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                getSession().Set<Bancos>().AddOrUpdate(entity);
                getSession().SaveChanges();
                for (int i = 0; i < agrega.Count(); i++)
                {
                    agrega[i].bancos = entity;
                    getSession().Set<Contactos>().AddOrUpdate(agrega[i]);
                    if (i % 100 == 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                if (exito)
                {
                    mensajeResultado.resultado = exito;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("UpdateBanco()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje DeleteBanco(Bancos entity, DBContextAdapter dbContext)
        {
            bool exito = true;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                deleteListQuery("Contactos", new CamposWhere("Contactos.bancos.id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                //deleteListQuery(getSession(), "Contactos", "bancos_id", entity.getId());
                deleteListQuery("Bancos", new CamposWhere("Bancos.id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                //deleteListQuery(getSession(), "Bancos", "id", entity.getId());
                if (exito)
                {
                    mensajeResultado.resultado = exito;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("DeleteBanco()1_Error: ").Append(ex));
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
        //        //existe = existeDato("Bancos", campo, valor);
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

        public Mensaje existeRFC(string rfc, string claveBancos, DBContextAdapter dbContext)
        {
            int cantidad;
            bool existe = false;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (claveBancos != null)
                {
                    cantidad = (from b in getSession().Set<Bancos>()
                                where b.RFC == rfc &&
                                b.clave == claveBancos
                                select b).Count();
                }
                else
                {
                    cantidad = (from b in getSession().Set<Bancos>()
                                where b.RFC == rfc
                                select b).Count();
                }
                if (cantidad > 0)
                {
                    existe = true;
                }
                mensajeResultado.resultado = existe;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existeRFC()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllBancos(DBContextAdapter dbContext)
        {
            //List<Bancos> listabancos = new List<Bancos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listabancos = (from a in getSession().Set<Bancos>()
                                   select new
                                   {
                                       a.id,
                                       a.clave,
                                       a.descripcion,
                                       a.domicilio,
                                       a.notas,
                                       a.paginaweb,
                                       a.RFC

                                   }).ToList();
                mensajeResultado.resultado = listabancos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("BancosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveBancos(string clave, DBContextAdapter dbContext)
        {
            Bancos bancos = new Bancos();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                bancos = (from b in getSession().Set<Bancos>()
                          where b.clave == clave
                          select b).SingleOrDefault();
                mensajeResultado.resultado = bancos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("BancosPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorIdBancos(decimal? idBancos, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var banco = (from b in getSession().Set<Bancos>()
                             where b.id == idBancos
                             select new
                             {
                                 b.id,
                                 b.clave,
                                 b.descripcion,
                                 b.domicilio,
                                 contactos = b.contactos.Select(c => new
                                 {
                                     c.id,
                                     c.bancos_ID,
                                     c.email,
                                     c.extencion_fax,
                                     c.extencion_telefono,
                                     c.fax,
                                     c.movil,
                                     c.nombre,
                                     c.puesto,
                                     c.telefono
                                 }).ToList(),
                                 b.notas,
                                 b.paginaweb,
                                 b.RFC
                             }).FirstOrDefault();
                mensajeResultado.resultado = banco;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdBancos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDetallesBancos(Bancos entity, Dictionary<string, object> Detalles, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                if (Detalles.ContainsKey("SaveUpdate"))
                {
                    string saveUpdate = Detalles["SaveUpdate"].ToString();
                    var arregloSave = JsonConvert.DeserializeObject<object[]>(saveUpdate);
                    for (int i = 0; i < arregloSave.Length; i++)
                    {
                        if (mensajeResultado.error.Equals(""))
                        {
                            string detalle = arregloSave[i].ToString();
                            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(detalle);
                            string tabla = dic["Tabla"].ToString();
                            if (entity.id == 0)
                            {
                                dic["bancos"] = entity;
                            }
                            else {
                                dic["bancos_ID"] = entity.id;
                            }

                            object entidad = crearobjeto(dic);
                            object instanceDAO = creaInstanciaDao(tabla + "DAO");
                            Type typeDao = instanceDAO.GetType();
                            MethodInfo staticMethodInfo = typeDao.GetMethod("modificar");
                            mensajeResultado = (Mensaje)staticMethodInfo.Invoke(instanceDAO, new object[] { entidad, dbContext });
                        }
                        else {
                            break;
                        }
                    }
                }

                if (Detalles.ContainsKey("Delete"))
                {
                    string delete = Detalles["Delete"].ToString();
                    var arregloDelete = JsonConvert.DeserializeObject<object[]>(delete);
                    for (int i = 0; i < arregloDelete.Length; i++)
                    {
                        if (mensajeResultado.error.Equals(""))
                        {
                            string detalle = arregloDelete[i].ToString();
                            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(detalle);
                            string tabla = dic["Tabla"].ToString();
                            //dic["paises_ID"] = entity.id;
                            object entidad = crearobjeto(dic);
                            object instanceDAO = creaInstanciaDao(tabla + "DAO");
                            Type typeDao = instanceDAO.GetType();
                            MethodInfo staticMethodInfo = typeDao.GetMethod("eliminar");
                            mensajeResultado = (Mensaje)staticMethodInfo.Invoke(instanceDAO, new object[] { entidad, "" });
                        }
                        else
                        {
                            break;
                        }

                    }

                }

                if (mensajeResultado.error.Equals(""))
                {
                    getSession().Set<Bancos>().AddOrUpdate(entity);
                    getSession().SaveChanges();
                    mensajeResultado.resultado = true;
                    mensajeResultado.error = "";
                    mensajeResultado.noError = 0;
                    getSession().Database.CurrentTransaction.Commit();
                }
                else {

                    getSession().Database.CurrentTransaction.Rollback();
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDetallesBancos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje deleteDetallesBancos(Bancos entity, Dictionary<string, object> Detalles, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (Detalles.ContainsKey("Delete"))
                {
                    string delete = Detalles["Delete"].ToString();
                    var arregloDelete = JsonConvert.DeserializeObject<object[]>(delete);
                    for (int i = 0; i < arregloDelete.Length; i++)
                    {
                        if (mensajeResultado.error.Equals(""))
                        {
                            string detalle = arregloDelete[i].ToString();
                            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(detalle);
                            string tabla = dic["Tabla"].ToString();

                            dic.Remove("bancos");
                            Dictionary<string, object> dicAux = new Dictionary<string, object>();
                            foreach (var item in dic)
                            {
                                if (item.Key.ToString().Equals("Tabla"))
                                {
                                    dicAux.Add(item.Key, item.Value);
                                }
                                else
                                {
                                    dicAux.Add(item.Key.Substring(0, 1).ToLower() + item.Key.Substring(1), item.Value);
                                }
                            }
                            object entidad = crearobjeto(dicAux);
                            object instanceDAO = creaInstanciaDao(tabla + "DAO");
                            Type typeDao = instanceDAO.GetType();
                            MethodInfo staticMethodInfo = typeDao.GetMethod("eliminar");
                            mensajeResultado = (Mensaje)staticMethodInfo.Invoke(instanceDAO, new object[] { entidad, dbContext });

                        }
                        else {
                            break;
                        }
                    }
                }
                if (mensajeResultado.error.Equals(""))
                {
                    getSession().Set<Bancos>().Attach(entity);
                    getSession().Set<Bancos>().Remove(entity);
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

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteDetallesBancos()1_Error: ").Append(ex));
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