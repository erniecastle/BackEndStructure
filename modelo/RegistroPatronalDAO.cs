/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase RegistroPatronalDAO para llamados a metodos de Entity
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
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class RegistroPatronalDAO : GenericRepository<RegistroPatronal>, RegistroPatronalDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private List<RegistroPatronal> listReg = new List<RegistroPatronal>();
        private List<Primas> listPrim = new List<Primas>();
        public Mensaje agregar(RegistroPatronal entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<RegistroPatronal>().Add(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = entity;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (/*Exception ex*/DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
                /*  System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregar()1_Error: ").Append(ex));
                  mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                  mensajeResultado.error = ex.GetBaseException().ToString();
                  mensajeResultado.resultado = null;*/
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        public Mensaje modificar(RegistroPatronal entity, DBContextAdapter dbContext)
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
                getSession().Set<RegistroPatronal>().AddOrUpdate(entity);
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

        public Mensaje eliminar(RegistroPatronal entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<RegistroPatronal>().Attach(entity);
                getSession().Set<RegistroPatronal>().Remove(entity);
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

        public Mensaje agregarListaRegistrosPatronales(List<RegistroPatronal> cambios, List<RegistroPatronal> temporales, List<Primas> cambioprima, object[] clavesDelete, object[] clavesPrimasDelete, int rango, DBContextAdapter dbContext)
        {
            listReg = new List<RegistroPatronal>();
            listPrim = new List<Primas>();
            try
            {
                RegistroPatronal r;
                Primas p;
                List<Object> nEntitys = new List<Object>(2);
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                int i;
                for (i = 0; i < cambios.Count; i++)
                {
                    for (int j = 0; j < temporales.Count; j++)
                    {
                        if (temporales[j].id == cambios[i].id)
                        {
                            cambios[i].id = 0;
                            temporales.RemoveAt(j);
                            j--;
                            break;
                        }
                    }
                    if (cambios[i].id == 0)
                    {
                        getSession().Set<RegistroPatronal>().AddOrUpdate(cambios[i]);
                        r = cambios[i];
                        listReg.Add(r);
                        foreach (Primas primas in cambioprima)
                        {
                            if (primas.registroPatronal.clave.Equals(r.clave))
                            {
                                primas.registroPatronal = r;
                                getSession().Set<Primas>().AddOrUpdate(primas);
                                p = primas;
                                listPrim.Add(p);
                            }
                        }
                    }
                    else
                    {
                        getSession().Set<RegistroPatronal>().AddOrUpdate(cambios[i]);
                        bool nPrima = false;
                        foreach (Primas prima in cambioprima)
                        {
                            if (prima.registroPatronal.id == cambios[i].id)
                            {
                                if (prima.id == 0)
                                {
                                    nPrima = true;
                                }
                                getSession().Set<Primas>().AddOrUpdate(prima);
                                if (nPrima)
                                {
                                    p = prima;
                                    listPrim.Add(p);
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
                if (clavesPrimasDelete != null)
                {
                    exito = deleteListQuerys("Primas", new CamposWhere("Primas.id", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    //deleteListQuerys("Primas", "id", clavesPrimasDelete);
                }
                if (clavesDelete != null & exito)
                {
                    deleteListQuerys("RegistroPatronal", new CamposWhere("RegistroPatronal.clave", clavesDelete, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    //deleteListQuerys("RegistroPatronal", "Clave", clavesDelete);
                }
                if (exito)
                {
                    nEntitys.Add(listReg);
                    nEntitys.Add(listPrim);
                    mensajeResultado.resultado = nEntitys;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaRegistrosPatronales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorFiltrosRegistroPatronal(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
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
                        campo.campo = "RegistroPatronal." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosPatronal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosRegistroPatronal(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorRangosPatronal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje deleteListClavesRegistroPatronal(object[] valores, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //deleteListQuery("RegistroPatronal", "Clave", valores);
                deleteListQuerys("RegistroPatronal", new CamposWhere("RegistroPatronal.clave", valores, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListClavesRegistroPatronal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje DeleteRegistroPatronal(RegistroPatronal entity, DBContextAdapter dbContext)
        {
            bool exitoRegistro = true;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                exitoRegistro = deleteListQuerys("Primas", new CamposWhere("Primas.RegistroPatronal.id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                //deleteListQuery(session, "Primas", "registrospatronal_id", entity.getId());
                if (exitoRegistro)
                {
                    //    exitoRegistro = deleteListQuery(session, "RegistroPatronal", "id", entity.getId());
                    exitoRegistro = deleteListQuerys("RegistroPatronal", new CamposWhere("RegistroPatronal.id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                }
                if (exitoRegistro)
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("DeleteRegistroPatronal()1_Error: ").Append(ex));
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

        public Mensaje getAllRegistroPatronal(string claveRazonesSocial, DBContextAdapter dbContext)
        {
            //List<RegistroPatronal> registrosPatronales = new List<RegistroPatronal>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var registrosPatronales = (from p in getSession().Set<RegistroPatronal>()
                                           where p.razonesSociales.clave == claveRazonesSocial
                                           select new
                                           {
                                               // tipoNomina = p.tipoNomina,
                                               telefono = p.telefono,
                                               subdelegacion = p.subdelegacion,
                                               // salariosIntegrados = p.salariosIntegrados,
                                               riesgoPuesto = p.riesgoPuesto,
                                               registroPatronal = p.registroPatronal,
                                               razonesSociales_ID = p.razonesSociales_ID,
                                               // razonesSociales = p.razonesSociales,
                                               // puestos = p.puestos,
                                               // primas = p.primas,
                                               //plazasPorEmpleado = p.plazasPorEmpleado,
                                               // plazas = p.plazas,
                                               paises_ID = p.paises_ID,
                                               // paises = p.paises,
                                               paginainter = p.paginainter,
                                               numeroin = p.numeroin,
                                               numeroex = p.numeroex,
                                               nombreregtpatronal = p.nombreregtpatronal,
                                               municipios_ID = p.municipios_ID,
                                               // municipios = p.municipios,
                                               // ingReingresosBajas = p.ingReingresosBajas,
                                               id = p.id,
                                               fax = p.fax,
                                               estados_ID = p.estados_ID,
                                               // estados = p.estados,
                                               delegacion = p.delegacion,
                                               cp_ID = p.cp_ID,
                                               // cp = p.cp,
                                               correoelec = p.correoelec,
                                               convenio = p.convenio,
                                               colonia = p.colonia,
                                               clavesubdelegacion = p.clavesubdelegacion,
                                               clavedelegacion = p.clavedelegacion,
                                               clave = p.clave,
                                               ciudades_ID = p.ciudades_ID,
                                               //ciudades = p.ciudades,
                                               //centroDeCosto = p.centroDeCosto,
                                               calle = p.calle
                                           }).ToList();
                mensajeResultado.resultado = registrosPatronales;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRegistroPatronalAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getSeriesPorRegistroPatronal(string claveRazonesSocial, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var registrosPatronales = (from r in getSession().Set<RegistroPatronal>()
                                           where r.razonesSociales.clave == claveRazonesSocial
                                           select new
                                           {
                                               id = r.id,
                                               registroPatronal = r.registroPatronal,
                                               serie = r.series.serie
                                           }).ToList();

                mensajeResultado.resultado = registrosPatronales;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getSeriesPorRegistroPatronal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorClaveRegistroPatronal(string clave, string claveRazonesSocial, DBContextAdapter dbContext)
        {
            //RegistroPatronal registroPatronal;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var registroPatronal = (from p in getSession().Set<RegistroPatronal>()
                                        where p.clave == clave && p.razonesSociales.clave == claveRazonesSocial
                                        select new
                                        {
                                            tipoNomina = p.tipoNomina,
                                            telefono = p.telefono,
                                            subdelegacion = p.subdelegacion,
                                            salariosIntegrados = p.salariosIntegrados,
                                            riesgoPuesto = p.riesgoPuesto,
                                            registroPatronal = p.registroPatronal,
                                            razonesSociales_ID = p.razonesSociales_ID,
                                            razonesSociales = p.razonesSociales,
                                            puestos = p.puestos,
                                            primas = p.primas,
                                            plazasPorEmpleado = p.plazasPorEmpleado,
                                            plazas = p.plazas,
                                            paises_ID = p.paises_ID,
                                            paises = p.paises,
                                            paginainter = p.paginainter,
                                            numeroin = p.numeroin,
                                            numeroex = p.numeroex,
                                            nombreregtpatronal = p.nombreregtpatronal,
                                            municipios_ID = p.municipios_ID,
                                            municipios = p.municipios,
                                            ingReingresosBajas = p.ingresosBajas,
                                            id = p.id,
                                            fax = p.fax,
                                            estados_ID = p.estados_ID,
                                            estados = p.estados,
                                            delegacion = p.delegacion,
                                            cp_ID = p.cp_ID,
                                            cp = p.cp,
                                            correoelec = p.correoelec,
                                            convenio = p.convenio,
                                            colonia = p.colonia,
                                            clavesubdelegacion = p.clavesubdelegacion,
                                            clavedelegacion = p.clavedelegacion,
                                            clave = p.clave,
                                            ciudades_ID = p.ciudades_ID,
                                            ciudades = p.ciudades,
                                            centroDeCosto = p.centroDeCosto,
                                            calle = p.calle
                                        }).SingleOrDefault();
                mensajeResultado.resultado = registroPatronal;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRegistroPatronalAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje SaveRegistroPatronal(List<Primas> agrega, object[] eliminados, RegistroPatronal entity, DBContextAdapter dbContext)
        {
            try
            {
                RegistroPatronal registroPatronal;
                bool exitoRegistro = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (eliminados.Length > 0)
                {
                    //  exitoRegistro = deleteListQueryEn(session, "Primas", "id", eliminados);
                    exitoRegistro = deleteListQuerys("Primas", new CamposWhere("Primas.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }

                if (exitoRegistro)
                {
                    getSession().Set<RegistroPatronal>().AddOrUpdate(entity);
                    agrega = agrega == null ? new List<Primas>() : agrega;
                    for (int i = 0; i < agrega.Count; i++)
                    {
                        agrega[i].registroPatronal = entity;
                        getSession().Set<Primas>().AddOrUpdate(agrega[i]);
                        if (i % 100 == 0)
                        {
                            getSession().SaveChanges();
                        }
                    }
                }

                if (exitoRegistro)
                {
                    registroPatronal = entity;
                    mensajeResultado.resultado = registroPatronal;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("SaveRegistroPatronal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje UpdateRegistroPatronal(List<Primas> agrega, object[] eliminados, RegistroPatronal entity, DBContextAdapter dbContext)
        {
            try
            {
                bool exitoRegistro = true;
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (eliminados.Length > 0)
                {
                    //  exitoRegistro = deleteListQueryEn(session, "Primas", "id", eliminados);
                    exitoRegistro = deleteListQuerys("Primas", new CamposWhere("Primas.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }

                if (exitoRegistro)
                {
                    getSession().Set<RegistroPatronal>().AddOrUpdate(entity);
                    agrega = agrega == null ? new List<Primas>() : agrega;
                    for (int i = 0; i < agrega.Count; i++)
                    {
                        agrega[i].registroPatronal = entity;
                        getSession().Set<Primas>().AddOrUpdate(agrega[i]);
                        if (i % 100 == 0)
                        {
                            getSession().SaveChanges();
                        }
                    }
                }

                if (exitoRegistro)
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("UpdateRegistroPatronal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private bool deleteListQuerys(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            try
            {
                //  deleteListQuery(tabla, campo, valores);
                deleteListQuery(tabla, campoWhere, dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getRegistroPatronalAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                return false;
            }
            return true;
        }

        public Mensaje getPorIdRegistroPatronal(decimal? id, string claveRazonesSocial, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var registroPatro = (from p in getSession().Set<RegistroPatronal>()
                                     where p.id == id && p.razonesSociales.clave == claveRazonesSocial
                                     select new
                                     {
                                         //  tipoNomina = p.tipoNomina,
                                         telefono = p.telefono,
                                         subdelegacion = p.subdelegacion,
                                         //salariosIntegrados = p.salariosIntegrados,
                                         riesgoPuesto = p.riesgoPuesto,
                                         registroPatronal = p.registroPatronal,
                                         razonesSociales_ID = p.razonesSociales_ID,
                                         // razonesSociales = p.razonesSociales,
                                         // puestos = p.puestos,
                                         primas = p.primas,
                                         // plazasPorEmpleado = p.plazasPorEmpleado,
                                         ///plazas = p.plazas,
                                         paises_ID = p.paises_ID,
                                         //paises = p.paises,
                                         paginainter = p.paginainter,
                                         numeroin = p.numeroin,
                                         numeroex = p.numeroex,
                                         nombreregtpatronal = p.nombreregtpatronal,
                                         municipios_ID = p.municipios_ID,
                                         //municipios = p.municipios,
                                         // ingReingresosBajas = p.ingReingresosBajas,
                                         id = p.id,
                                         fax = p.fax,
                                         estados_ID = p.estados_ID,
                                         // estados = p.estados,
                                         delegacion = p.delegacion,
                                         cp_ID = p.cp_ID,
                                         //  cp = p.cp,
                                         correoelec = p.correoelec,
                                         convenio = p.convenio,
                                         colonia = p.colonia,
                                         clavesubdelegacion = p.clavesubdelegacion,
                                         clavedelegacion = p.clavedelegacion,
                                         clave = p.clave,
                                         ciudades_ID = p.ciudades_ID,
                                         // ciudades = p.ciudades,
                                         //centroDeCosto = p.centroDeCosto,
                                         calle = p.calle
                                     }).SingleOrDefault();
                mensajeResultado.resultado = registroPatro;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdRegistroPatronal()1_Error: ").Append(ex));
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