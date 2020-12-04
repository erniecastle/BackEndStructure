/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase PlazasPorEmpleadoDAO para llamados a metodos de Entity
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
    public class PlazasPorEmpleadoDAO : GenericRepository<PlazasPorEmpleado>, PlazasPorEmpleadoDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(PlazasPorEmpleado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<PlazasPorEmpleado>().Add(entity);
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
        public Mensaje actualizar(PlazasPorEmpleado entity, DBContextAdapter dbContext)
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
                getSession().Set<PlazasPorEmpleado>().AddOrUpdate(entity);
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
        public Mensaje eliminar(PlazasPorEmpleado entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                //var sentencia = (from g in getSession().Set<Genero>()
                //                 where g.id == entity.id
                //                 select g).FirstOrDefault();
                getSession().Set<PlazasPorEmpleado>().Attach(entity);
                getSession().Set<PlazasPorEmpleado>().Remove(entity);
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

        public Mensaje agregarListaPlazasPorEmpleados(List<PlazasPorEmpleado> entitys, int rango, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleado> plazasPorEmpleados = new List<PlazasPorEmpleado>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                int i;
                for (i = 0; i < entitys.Count; i++)
                {
                    if (entitys[i].id == 0)
                    {
                        plazasPorEmpleados.Add(getSession().Set<PlazasPorEmpleado>().Add(entitys[i]));
                    }
                    else {
                        getSession().Set<PlazasPorEmpleado>().AddOrUpdate(entitys[i]);
                    }
                }

                getSession().SaveChanges();
                mensajeResultado.resultado = plazasPorEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregarListaPlazasPorEmpleados()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorFiltrosPlazasPorEmpleado(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleado> plazasPorEmpleados = null;
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
                        campo.campo = "PlazasPorEmpleado." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosPlazasPorEmpleado(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleado> plazasPorEmpleados = null;
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


        //public Mensaje existeDato(string campo, object valor, DbContext dbContext)
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

        public Mensaje getPlazasPorEmpleadoPorClave(string clave, string razonSocial, DBContextAdapter dbContext)
        {
            PlazasPorEmpleado plazaPorEmpleado;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazaPorEmpleado = (from pe in getSession().Set<PlazasPorEmpleado>()
                                    join rs in getSession().Set<RazonesSociales>()
                                    on pe.razonesSociales.id equals rs.id
                                    where pe.referencia == clave && rs.clave == razonSocial

                                    select pe).SingleOrDefault();
                mensajeResultado.resultado = plazaPorEmpleado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadoPorClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadoPorClavePorRazonSocialActivo(string clavePlaza, string claveEmpleado, string razonSocial, DateTime fecha, DBContextAdapter dbContext)
        {
            PlazasPorEmpleado plazaPorEmpleado;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazaPorEmpleado = (from pe in getSession().Set<PlazasPorEmpleado>()
                                    where pe.referencia == clavePlaza && pe.empleados.clave == claveEmpleado
                                    && pe.razonesSociales.clave == razonSocial && pe.fechaFinal >= fecha
                                    select pe).SingleOrDefault();
                mensajeResultado.resultado = plazaPorEmpleado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadoPorClavePorRazonSocialActivo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadoPorRazonSocial(string clave, string razonSocial, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleado> plazasPorEmpleados;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazasPorEmpleados = (from pe in getSession().Set<PlazasPorEmpleado>()
                                      where pe.empleados.clave == clave && pe.razonesSociales.clave == razonSocial
                                      select pe).ToList();
                mensajeResultado.resultado = plazasPorEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadoPorRazonSocial()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadoPorRazonSocialActivo(string claveEmpleado, string razonSocial, DateTime fecha, int result, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleado> plazasPorEmpleados = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var queryRes = (from pe in getSession().Set<PlazasPorEmpleado>()
                                where pe.empleados.clave == claveEmpleado && pe.razonesSociales.clave == razonSocial
                                && pe.fechaFinal >= fecha
                                select pe);


                if (result > 0)
                {
                    plazasPorEmpleados = queryRes.Take(result).ToList();
                }
                else
                {
                    plazasPorEmpleados = queryRes.ToList();
                }
                mensajeResultado.resultado = plazasPorEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadoPorRazonSocialActivo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadoReingreso(string claveReingreso, string claveRazonesSociales, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleado> plazasPorEmpleados;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazasPorEmpleados = (from pe in getSession().Set<PlazasPorEmpleado>()
                                      join rs in getSession().Set<RazonesSociales>()
                                      on pe.razonesSociales.id equals rs.id
                                      join re in getSession().Set<PlazasPorEmpleado>()
                                      on pe.plazaReIngreso.id equals re.id
                                      where re.referencia == claveReingreso && rs.clave == claveRazonesSociales
                                      select pe).ToList();
                mensajeResultado.resultado = plazasPorEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadoReingreso()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosActivos(string claveEmpleado, string claveRazonSocial, DateTime fechaInicial, DateTime fechaFinal, string claveTipoNomina, int result, DBContextAdapter dbContext)
        {
            List<PlazasPorEmpleado> plazasPorEmpleados = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazasPorEmpleados = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                                      where
                                            (from m in getSession().Set<PlazasPorEmpleadosMov>()
                                             where m.plazasPorEmpleado.empleados.clave == claveEmpleado &&
                                                   m.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial &&
                                                   m.fechaInicial <= fechaInicial &&
                                                  m.plazasPorEmpleado.fechaFinal >= fechaFinal &&
                                                   m.tipoNomina.clave == claveTipoNomina
                                             group new { m.plazasPorEmpleado, m } by new
                                             {
                                                 m.plazasPorEmpleado.referencia
                                             } into g
                                             select new
                                             {
                                                 Column1 = g.Max(p => p.m.id)
                                             }).Contains(new { Column1 = o.id })
                                      orderby
                                        o.fechaInicial
                                      select o.plazasPorEmpleado).ToList();


                mensajeResultado.resultado = plazasPorEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosActivos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosPorReferenciaActiva(string claveEmpleado, string claveRazonSocial, DateTime fechaInicial, DateTime fechaFinal, string claveTipoNomina, string claveReferencia, DBContextAdapter dbContext)
        {
            PlazasPorEmpleado plazasPorEmpleados;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazasPorEmpleados = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                                      where
                                            (from m in getSession().Set<PlazasPorEmpleadosMov>()
                                             where m.plazasPorEmpleado.empleados.clave == claveEmpleado &&
                                                   m.plazasPorEmpleado.razonesSociales.clave == claveRazonSocial &&
                                                   m.fechaInicial <= fechaInicial &&
                                                  m.plazasPorEmpleado.fechaFinal >= fechaFinal &&
                                                  m.plazasPorEmpleado.referencia == claveReferencia &&
                                                   m.tipoNomina.clave == claveTipoNomina
                                             group new { m.plazasPorEmpleado, m } by new
                                             {
                                                 m.plazasPorEmpleado.referencia
                                             } into g
                                             select new
                                             {
                                                 Column1 = g.Max(p => p.m.id)
                                             }).Contains(new { Column1 = o.id })
                                      orderby
                                        o.fechaInicial
                                      select o.plazasPorEmpleado).SingleOrDefault();


                mensajeResultado.resultado = plazasPorEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosPorReferenciaActiva()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosPorReferencia(decimal idReferencia, DBContextAdapter dbContext)
        {
            string claveReferencia = "4";
            decimal idRazon = 1;

            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();

                var plazasPorEmpleados = (from pxe in dbContext.context.Set<PlazasPorEmpleado>()
                                          join pem in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                          on pxe.id equals pem.plazasPorEmpleado.id
                                          where pxe.referencia == claveReferencia && pxe.razonesSociales_ID == idRazon
                                           && pem.id == (from ppm in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                                         where ppm.plazasPorEmpleado.id == pxe.id
                                                         select new { ppm.id }).Min(p => p.id)

                                          select pxe).Select(PXEM => new
                                          {
                                              PXEM.fechaFinal,
                                              PXEM.fechaPrestaciones,
                                              claveRegistroPatronal = PXEM.registroPatronal.clave,
                                              empleados = new
                                              {
                                                  PXEM.empleados.clave,
                                                  PXEM.empleados.apellidoPaterno,
                                                  PXEM.empleados.apellidoMaterno,
                                                  PXEM.empleados.nombre,
                                                  PXEM.empleados.lugarNacimiento,
                                                  PXEM.empleados.fechaNacimiento,
                                                  PXEM.empleados.genero.descripcion,
                                                  PXEM.empleados.nacionalidad,

                                              },

                                              plazasPorEmpleadoMov = PXEM.plazasPorEmpleadosMov
                                              .Select(PXMOV => new
                                              {
                                                  clavePlazas = PXMOV.plazas.clave,
                                                  PXMOV.tipoRelacionLaboral,
                                                  /*is sindicalizado*/
                                                  PXMOV.fechaInicial,
                                                  PXMOV.fechaIMSS,
                                                  idTipoContrato = PXMOV.tipoContrato.id,
                                                  idRegimenContratacion = PXMOV.regimenContratacion,
                                                  idTipoNomina = PXMOV.tipoNomina.id,
                                                  idCentroDeCosto = PXMOV.centroDeCosto.id,
                                                  idDepartamentos = PXMOV.departamentos.id,
                                                  idPuestos = PXMOV.puestos.id,
                                                  idTurnos = PXMOV.turnos.id,
                                                  PXMOV.horas,
                                                  PXMOV.importe

                                              }),

                                              ingresosBajas = new
                                              {
                                                  PXEM.ingresosBajas.fechaIngreso,
                                                  PXEM.ingresosBajas.fechaBaja

                                              },
                                          }).SingleOrDefault();

                mensajeResultado.resultado = plazasPorEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosActivos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPlazasPorEmpleadosActivosId(decimal? claveEmpleado, decimal? claveRazonSocial, DateTime? fechaInicial, DateTime? fechaFinal, decimal? claveTipoNomina, int result, DBContextAdapter dbContext)
        {
            PlazasPorEmpleado plazasPorEmpleados;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                plazasPorEmpleados = (from p in getSession().Set<PlazasPorEmpleadosMov>()
                                      where p.plazasPorEmpleado.empleados.id == claveEmpleado &&
                                      p.plazasPorEmpleado.razonesSociales.id == claveRazonSocial &&
                                      p.tipoNomina.id == claveTipoNomina && p.fechaInicial <= fechaInicial &&
                                      p.plazasPorEmpleado.fechaFinal >= fechaFinal
                                      select p.plazasPorEmpleado).SingleOrDefault();
                //plazasPorEmpleados = (from o in getSession().Set<PlazasPorEmpleadosMov>()
                //                      where (from p in getSession().Set<PlazasPorEmpleadosMov>()
                //                             where p.plazasPorEmpleado.empleados.id == claveEmpleado &&
                //                             p.plazasPorEmpleado.razonesSociales.id == claveRazonSocial &&
                //                             p.fechaInicial <= fechaInicial && p.plazasPorEmpleado.fechaFinal >= fechaFinal &&
                //                             p.tipoNomina.id == claveTipoNomina
                //                             group new { p.plazasPorEmpleado, p } by new
                //                             {
                //                                 p.plazasPorEmpleado.referencia
                //                             } into g
                //                             select new
                //                             {
                //                                 Column1 = g.Max(m => m.p.id)
                //                             }).Contains(new { Column1 = o.id })
                //                      orderby
                //                      o.fechaInicial
                //                      select o.plazasPorEmpleado).SingleOrDefault();
                mensajeResultado.resultado = plazasPorEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosActivosId()1_Error: ").Append(ex));
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

        public Mensaje getPlazasPorEmpleadosActivosIdList(decimal? claveEmpleado, decimal? claveRazonSocial, DateTime? fechaInicial, DateTime? fechaFinal, decimal? claveTipoNomina, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var plazasPorEmpleados = (from pl in getSession().Set<PlazasPorEmpleadosMov>()
                                          where pl.plazasPorEmpleado.empleados_ID == claveEmpleado
                                          && pl.plazasPorEmpleado.razonesSociales_ID == claveRazonSocial
                                          && pl.tipoNomina_ID == claveTipoNomina
                                          && pl.fechaInicial <= fechaInicial &&
                                           pl.plazasPorEmpleado.fechaFinal >= fechaFinal
                                          select new
                                          {
                                              //     pl.clabe,
                                              pl.fechaInicial,
                                              pl.id,
                                              plazasPorEmpleado = new
                                              {
                                                  pl.plazasPorEmpleado.id,
                                                  pl.plazasPorEmpleado.fechaFinal,
                                                  pl.plazasPorEmpleado.fechaPrestaciones,
                                                  pl.plazasPorEmpleado.plazaPrincipal,
                                                  pl.plazasPorEmpleado.referencia
                                              }
                                          }).ToList();
                mensajeResultado.resultado = plazasPorEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosActivosIdList()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }

        public Mensaje getPlazaPorID(decimal? idPlaza, DBContextAdapter dbContext)
        {
           // PlazasPorEmpleado plazasPorEmpleados;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               var plazasPorEmpleados = (from p in getSession().Set<PlazasPorEmpleado>()
                                      where p.id == idPlaza
                                      select new { 
                                      p.empleados_ID,
                                      p.fechaFinal,
                                      p.fechaPrestaciones,
                                      p.id,
                                      p.ingresosBajas_ID,
                                      p.plazaPrincipal,
                                      p.razonesSociales_ID,
                                      p.referencia,
                                      p.registroPatronal_ID,
                                      p.reIngreso_ID
                                      }).SingleOrDefault();
              
                mensajeResultado.resultado = plazasPorEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPlazasPorEmpleadosActivosId()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }
    }
}