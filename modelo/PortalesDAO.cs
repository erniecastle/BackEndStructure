using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Exitosw.Payroll.Hibernate.util;
using Exitosw.Payroll.Hibernate.modelo;
using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Core.servicios.extras;

namespace Exitosw.Payroll.Core.modelo
{
    public class PortalesDAO : NHibernateRepository<Portales>, PortalesDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("WebLogin").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje modificar(Portales entity, string[] accesLog)
        {
            try
            {
                ConnectionDB conn = new ConnectionDB();
                conn.tipoServer = TypeDB.SQLServer;
                conn.nombreBd = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[0]));
                conn.usuario = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[1]));
                conn.password = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[2]));
                conn.puertoServer = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[3]));
                conn.server = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[4]));
                inicializaVariableMensaje();
                ISession session = NHibernateCxn.createSessionFactorie(conn, 3).OpenSession();
                setSession(session);
                getSession().BeginTransaction();
                getSession().SaveOrUpdate(entity);

                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                EndSession(getSession());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("modificar()1_Error: ").Append(ex));
                if (getSession().Transaction.IsActive)
                {
                    getSession().Transaction.Rollback();
                }
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;

        }

        public Mensaje getPortalesByKey(string clave, string[] accesLog)
        {
            try
            {
                ConnectionDB conn = new ConnectionDB();
                conn.tipoServer = TypeDB.SQLServer;
                conn.nombreBd = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[0]));
                conn.usuario = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[1]));
                conn.password = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[2]));
                conn.puertoServer = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[3]));
                conn.server = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[4]));
                inicializaVariableMensaje();
                ISession session = NHibernateCxn.createSessionFactorie(conn, 3).OpenSession();
                setSession(session);
                getSession().BeginTransaction();

                var portal = (from p in getSession().Query<Portales>()
                              where p.clave == clave
                              select new
                              {
                                  id = p.id,
                                  clave = p.clave,
                                  nombre = p.nombre,
                                  textoPrincipal = p.textoPrincipal,
                                  url = p.url,
                                  estado = p.estado,
                                  postLogin = p.postLogin,
                                  imgLogo = p.imgLogo,
                                  imgFondo = p.imgFondo,
                                  imgBanner = p.imgBanner,
                                  isNombreCorpo = p.isNombreCorpo,
                                  isBannerCorpo = p.isBannerCorpo,
                                  textoBienvenida = p.textoBienvenida,
                                  imgBannerLogo = p.imgBannerLogo,
                                  colorFondo = p.colorFondo,
                                  colorFuente = p.colorFuente,
                                  colorExceEncabe = p.colorExceEncabe,
                                  colorExceTextoEncabe = p.colorExceTextoEncabe,
                                  colorExceGruposTot = p.colorExceGruposTot,
                                  configLoader=p.configLoader
                              }).FirstOrDefault();

                mensajeResultado.resultado = portal;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                EndSession(getSession());
                /*getSession().Transaction.Commit();*/
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPortalesByHost()1_Error: ").Append(ex));
                if (getSession().Transaction.IsActive)
                {
                    getSession().Transaction.Rollback();
                }
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje getPortalesByHost(string url, string[] accesLog)
        {
            try
            {
                ConnectionDB conn = new ConnectionDB();
                conn.tipoServer = TypeDB.SQLServer;
                conn.nombreBd = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[0]));
                conn.usuario = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[1]));
                conn.password = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[2]));
                conn.puertoServer = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[3]));
                conn.server = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[4]));
                inicializaVariableMensaje();
                ISession session = NHibernateCxn.createSessionFactorie(conn, 3).OpenSession();
                setSession(session);
                getSession().BeginTransaction();

                var portal = (from p in getSession().Query<Portales>()
                              where p.url == url
                              select new
                              {
                                  clave = p.clave,
                                  nombre = p.nombre,
                                  textoPrincipal = p.textoPrincipal,
                                  estado = p.estado,
                                  postLogin = p.postLogin,
                                  imgLogo = p.imgLogo,
                                  imgFondo = p.imgFondo,
                                  imgBanner = p.imgBanner,
                                  imgBannerLogo = p.imgBannerLogo,
                                  isNombreCorpo = p.isNombreCorpo,
                                  isBannerCorpo = p.isBannerCorpo,
                                  textoBienvenida = p.textoBienvenida,
                                  colorFondo = p.colorFondo,
                                  colorFuente = p.colorFuente,
                                  colorExceEncabe = p.colorExceEncabe,
                                  colorExceTextoEncabe = p.colorExceTextoEncabe,
                                  colorExceGruposTot = p.colorExceGruposTot,
                                  configLoader = p.configLoader,
                                  avisos = p.avisos.Select(a => new
                                  {
                                      clave = a.clave,
                                      /* servidoresBd = p.servidoresBd,*/
                                      estado = a.estado,
                                      titulo = a.titulo,
                                      mensaje = a.mensaje,
                                      tipo = a.tipo,
                                      inicia = a.inicia,
                                      vence = a.vence
                                  }).ToList()

                              }).FirstOrDefault();

                mensajeResultado.resultado = portal;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                EndSession(getSession());
                /*getSession().Transaction.Commit();*/
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPortalesByHost()1_Error: ").Append(ex));
                if (getSession().Transaction.IsActive)
                {
                    getSession().Transaction.Rollback();
                }
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje getLogin(string keyPortal, string user, string password, string[] accesLog)
        {
            try
            {
                string nameDb = Encoding.UTF8.GetString(Convert.FromBase64String(accesLog[0]));
                inicializaVariableMensaje();
                ConnectionDB con = new ConnectionDB(accesLog);
                con.nombreBd = nameDb;
                ISession session = NHibernateCxn.createSessionFactorie(con, 3).OpenSession();
                setSession(session);
                getSession().BeginTransaction();
                List<PortalesServidoresBd> portalesServ = null;
                portalesServ = (from p in getSession().Query<PortalesServidoresBd>()
                                where p.portales.clave == keyPortal
                                select p).ToList();

                mensajeResultado.resultado = portalesServ;
                if (mensajeResultado.resultado != null)
                {
                    //Make Login
                    mensajeResultado = validateAccesUser(portalesServ, user, password);

                    if (mensajeResultado.resultado != null)
                    {

                    }
                }

                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                EndSession(getSession());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getLogin()1_Error: ").Append(ex));
                if (getSession().Transaction.IsActive)
                {
                    getSession().Transaction.Rollback();
                }
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje validateAccesUser(List<PortalesServidoresBd> configs, string user, string password)
        {
            ISession sessionMaster = null;
            try
            {
                inicializaVariableMensaje();
                PortalesServidoresBd serverMaster = new PortalesServidoresBd();
                foreach (PortalesServidoresBd item in configs)
                {
                    if (item.tipoDeBD == 1)
                    {
                        serverMaster = item;
                        break;
                    }
                }

                String encodePass = Exitosw.Payroll.Core.util.Utilerias.createMD5(password);
                ConnectionDB conn = JsonConvert.DeserializeObject<ConnectionDB>(serverMaster.servidoresBd.cadenaCxn);
                conn.nombreBd = serverMaster.nombreBd;
                ISessionFactory factorieAcces = NHibernateCxn.createSessionFactorie(conn, serverMaster.tipoDeBD);
                sessionMaster = factorieAcces.OpenSession();
                IQuery q;//GET Global variables
                sessionMaster.BeginTransaction();
                q = sessionMaster.CreateQuery("from Usuario u where u.nombre = :nombre and u.password = :password");
                q.SetParameter("nombre", user);
                q.SetParameter("password", encodePass);
                Usuario us = q.UniqueResult<Usuario>();

                //q = sessionMaster.CreateQuery("select razonSocial.claveRazonSocial from RazonSocialConfiguracion ra where ra.usuario.clave = :clave");
                //q.SetParameter("clave", us.clave);

                //IList<string> rzCnf = q.List<string>();
                //string rzCnfSep = "";
                //if (rzCnf.Count > 0)
                //{
                //    string[] array = rzCnf.ToArray();
                //    rzCnfSep = string.Join(".", array);
                //} , rzCnfSep

                Object[] info;
                if (us == null)
                {
                    info = null;
                }
                else
                {
                    info = new Object[] { us.id, us.nombre };
                }

                mensajeResultado.resultado = info;
                if (sessionMaster.Transaction != null && sessionMaster.Transaction.IsActive)
                {
                    sessionMaster.Transaction.Commit();
                }
                sessionMaster.Dispose();
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("validateAccesUser()1_Error: ").Append(ex));
                if (sessionMaster.Transaction.IsActive)
                {
                    sessionMaster.Transaction.Rollback();
                }
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }


    }
}