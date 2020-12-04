using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Exitosw.Payroll.Core.modelo
{
    public class FormasCapturaDAO : GenericRepository<FormasCaptura>, FormasCapturaDAOIF

    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<CamposOrigenDatos> listcamposOrigenDatos = new List<CamposOrigenDatos>();
        public Mensaje getDetalleOrigen(string clase, DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();

                List<CamposOrigenDatos> listCampDat = getcamposOrigenDatos(clase);
                int i = 0;
                for (i = 0; i <= listCampDat.Count; i++)
                {
                    listcamposOrigenDatos.Add(listCampDat[i]);
                    List<CamposOrigenDatos> sublistCampDat = getcamposOrigenDatos(listCampDat[i].campo);
                    if (sublistCampDat.Count > 0)
                    {
                        listCampDat.AddRange(sublistCampDat);
                    }
                    else {
                        break;
                    }
                }
                //(r, i) => new SelectListItem{Text = r, Value = i.ToString()}
                object[] array = listcamposOrigenDatos.Select(x => new object[] { x.campo, x.estado }).ToArray();


                mensajeResultado.resultado = JsonConvert.SerializeObject(array);
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
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public decimal getOrigenId(string clase)
        {
            decimal id = -1;
            try
            {
                inicializaVariableMensaje();
                id = (from od in getSession().Set<OrigenDatos>()
                      where od.recurso.Equals(clase)
                      select od.id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getOrigenId()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();

            }

            return id;
        }

        public List<CamposOrigenDatos> getcamposOrigenDatos(string clase, TipoDatoOrigen tipoDato = TipoDatoOrigen.Relacion)
        {
            List<CamposOrigenDatos> listOrigenDatos = null;
            try
            {
                decimal idOrigen = getOrigenId(clase);

                listOrigenDatos =
                   (from cod in getSession().Set<CamposOrigenDatos>()
                    where cod.origenDatos_ID == idOrigen && cod.tipoDeDato == tipoDato
                    select cod).ToList().Select(o =>
                    new CamposOrigenDatos { campo = o.campo, estado = o.estado }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getcamposOrigenDatos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return listOrigenDatos;
        }
        public Mensaje getcolumnasOrigenDatos(string clase, DBContextAdapter dbContextMaster)
        {
            List<CamposOrigenDatos> listOrigenDatos = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                decimal idOrigen = getOrigenId(clase);

                listOrigenDatos =
                   (from cod in getSession().Set<CamposOrigenDatos>()
                    where cod.origenDatos_ID == idOrigen
                    select cod).ToList();
                object[] array = listOrigenDatos.Select(x => new object[] { x.campo, x.estado, x.llave, x.requerido, x.idEtiqueta, x.tipoDeDato, x.compAncho, x.configuracionTipoCaptura, x.origenDatos_ID }).ToArray();
                mensajeResultado.resultado = array; /*JsonConvert.SerializeObject(array);*/
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getcolumnasOrigenDatos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getOrigesDatosALL(DBContextAdapter dbContextMaster)
        {
            List<OrigenDatos> origenes = new List<OrigenDatos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                origenes = (from o in getSession().Set<OrigenDatos>()
                            select o).ToList().Select(p => new OrigenDatos(p.id, p.clave, p.nombre, p.origen, p.recurso, p.estado)).ToList();
                mensajeResultado.resultado = origenes;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getOrigesDatosALL()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getDetallaOrigenesDatos(decimal idorigen, DBContextAdapter dbContextMaster)
        {
            List<DetalleOrigenDatos> datalle = new List<DetalleOrigenDatos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                datalle = (from d in getSession().Set<DetalleOrigenDatos>()
                           where d.origenDatos.id == idorigen
                           select d).ToList();

                if (datalle.Count == 0)
                {
                    mensajeResultado.resultado = "";
                }
                else {
                    mensajeResultado.resultado = datalle.Select(p => new object[] { p.origenDatosFuente.id, p.origenDatosFuente.nombre }).ToArray();
                }
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getDetallaOrigenesDatos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }

        public Mensaje getCamposOrigenDatos(decimal idOrigen, bool toDictionary, DBContextAdapter dbContextMaster)
        {
            List<CamposOrigenDatos> camposOrigenDatos = new List<CamposOrigenDatos>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContextMaster.context);
                getSession().Database.BeginTransaction();
                camposOrigenDatos = (from ca in getSession().Set<CamposOrigenDatos>()
                                     where ca.origenDatos.id == idOrigen
                                     select ca).ToList();

                var cmapos = camposOrigenDatos.Select(

                    x => new
                    {
                        id = x.GetType().GetProperty("id").GetValue(x, null),
                        idOrigen = GetPropValue("origenDatos.id", x).ToString(),
                        origenNombre = GetPropValue("origenDatos.nombre", x).ToString(),
                        campo = x.GetType().GetProperty("campo").GetValue(x, null),
                        estado = x.GetType().GetProperty("estado").GetValue(x, null),
                        llave = x.GetType().GetProperty("llave").GetValue(x, null),
                        requerido = x.GetType().GetProperty("requerido").GetValue(x, null),
                        idEtiqueta = x.GetType().GetProperty("idEtiqueta").GetValue(x, null),
                        tipoDeDato = x.GetType().GetProperty("tipoDeDato").GetValue(x, null),
                        compAncho = x.GetType().GetProperty("compAncho").GetValue(x, null),
                        activarGlobal = x.GetType().GetProperty("activarGlobal").GetValue(x, null),
                        activarCaptura = x.GetType().GetProperty("activarCaptura").GetValue(x, null),
                        configuracionTipoCaptura = x.GetType().GetProperty("configuracionTipoCaptura").GetValue(x, null),
                        subOrigen = x.configuracionTipoCaptura == "" ? "" :
                        JObject.Parse(x.configuracionTipoCaptura) == null ? "" :
                        JObject.Parse(x.configuracionTipoCaptura)["origenes"] == null ? "" :
                        JObject.Parse(x.configuracionTipoCaptura)["origenes"]["origen"],
                        camposSubOrigen = x.configuracionTipoCaptura == "" ? "" :
                        JObject.Parse(x.configuracionTipoCaptura) == null ? "" :
                        JObject.Parse(x.configuracionTipoCaptura)["origenes"] == null ? "" :
                        JObject.Parse(x.configuracionTipoCaptura)["origenes"]["camposAdicionales"],
                        // nombreFuente = x.GetType().GetProperty("id").GetValue(x.origenDatos.nombre, null),
                    }).ToList();

                mensajeResultado.resultado = cmapos;

                if (toDictionary)
                {
                    var cmaposDic = cmapos.ToDictionary(ele => ele.id, ele => ele);
                    mensajeResultado.resultado = cmaposDic;
                }

                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
                getSession().Database.CurrentTransaction.Commit();

            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCamposOrigenDatos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            getSession().Database.Connection.Close();
            return mensajeResultado;
        }



        public Object GetPropValue(String name, Object obj)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

    }
}