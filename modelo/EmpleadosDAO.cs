/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase EmpleadosDAO para llamados a metodos de Entity
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
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Exitosw.Payroll.Core.modelo
{
    public class EmpleadosDAO : GenericRepository<Empleados>, EmpleadosDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");

        public Mensaje agregar(Empleados entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Empleados>().Add(entity);
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
        public Mensaje actualizar(Empleados entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Empleados>().AddOrUpdate(entity);
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
        public Mensaje eliminar(Empleados entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Empleados>().Attach(entity);
                getSession().Set<Empleados>().Remove(entity);
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
        public Mensaje DeleteEmpleado(Empleados entity, DBContextAdapter dbContext)
        {
            inicializaVariableMensaje();
            setSession(dbContext.context);
            try
            {
                getSession().Database.BeginTransaction();
                /*deleteListQuery(getSession(), "Familiares", "empleados_id", entity.getId(), "");
                deleteListQuery(getSession(), "FormacionEconomica", "empleados_id", entity.getId(), "");
                deleteListQuery(getSession(), "Capacitaciones", "empleados_id", entity.getId(), "");
                deleteListQuery(getSession(), "ExperienciaLaboralExterna", "empleados_id", entity.getId(), "");
                deleteListQuery(getSession(), "ExperienciaLaboralInterna", "empleados_id", entity.getId(), "");
                deleteListQuery(getSession(), "Documentacion", "empleados_id", entity.getId(), "");
                deleteListQueryVacaciones(getSession(), entity.id);
                deleteListQuery(getSession(), "Empleados", "id", entity.id, "");*/
                deleteListQuery("Familiares", new CamposWhere("Familiares.empleados_id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                deleteListQuery("FormacionEconomica", new CamposWhere("FormacionEconomica.empleados_id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                deleteListQuery("Capacitaciones", new CamposWhere("Capacitaciones.empleados_id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                deleteListQuery("ExperienciaLaboralExterna", new CamposWhere("ExperienciaLaboralExterna.empleados_id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                deleteListQuery("ExperienciaLaboralInterna", new CamposWhere("ExperienciaLaboralInterna.empleados_id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                deleteListQuery("Documentacion", new CamposWhere("Documentacion.empleados_id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                deleteListQueryVacaciones(dbContext, entity.id);
                //deleteListQuery("ExperienciaLaboralExterna", new CamposWhere("ExperienciaLaboralExterna.empleados_id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                deleteListQuery("Empleados", new CamposWhere("empleados_id", entity.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("DeleteEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        /*private void deleteListQuery(Session session, String tabla, String campo, Object valores, DbContext dbContext)
        {
            consulta = new StringBuilder("delete ");
            consulta.append(tabla).append(" where ").append(campo).append("=:valores)");
            query = session.createQuery(consulta.toString());
            query.setParameter("valores", valores);
            query.executeUpdate();
        }*/
        private void deleteListQueryVacaciones(DBContextAdapter dbContext, Object valores)
        {
            try
            {
                List<decimal> ids = (from v in getSession().Set<VacacionesAplicacion>()
                                     where v.vacacionesDevengadas.empleados.id == Convert.ToDecimal(valores)
                                     select v.id).ToList();
                if (ids == null ? false : !ids.Any())
                {
                    getSession().Database.ExecuteSqlCommand("delete from VacacionesDevengadas o  where o.id in(@valores)", new SqlParameter(" @valores", ids.ToArray()));
                }
                ids = (from v in getSession().Set<VacacionesDevengadas>()
                       where v.empleados.id == Convert.ToDecimal(valores)
                       select v.id).ToList();
                if (ids == null ? false : !ids.Any())
                {
                    getSession().Database.ExecuteSqlCommand("delete from VacacionesAplicacion o  where o.id in(@valores)", new SqlParameter(" @valores", ids.ToArray()));
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQueryVacaciones()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
        }
        public Mensaje existeRFC(string rfc, string claveEmpleado, DBContextAdapter dbContext)
        {
            int cantidad;
            bool existe = false;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                if (claveEmpleado != null)
                {
                    cantidad = (from e in getSession().Set<Empleados>()
                                where e.RFC == rfc &&
                                e.clave == claveEmpleado
                                select e).Count();
                }
                else
                {
                    cantidad = (from e in getSession().Set<Empleados>()
                                where e.RFC == rfc
                                select e).Count();
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

        public Mensaje getAllEmpleados(string claveRazonesSocial, DBContextAdapter dbContext)
        {
            // List<Empleados> listaEmpleados = new List<Empleados>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listaEmpleados = (from e in getSession().Set<Empleados>()
                                      where e.razonesSociales.clave == claveRazonesSocial
                                      select new
                                      {
                                          // aguinaldoPagos = e.aguinaldoPagos,
                                          apellidoMaterno = e.apellidoMaterno,
                                          apellidoPaterno = e.apellidoPaterno,
                                          // asistencias = e.asistencias,
                                          //  calculoUnidades = e.calculoUnidades,
                                          // capacitaciones = e.capacitaciones,
                                          //  ciudades = e.ciudades,
                                          ciudades_ID = e.ciudades_ID,
                                          clave = e.clave,
                                          clinicaIMSS = e.clinicaIMSS,
                                          colonia = e.colonia,
                                          correoElectronico = e.correoElectronico,
                                          //cp = e.cp,
                                          cp_ID = e.cp_ID,
                                          ///creditoPorEmpleado = e.creditoPorEmpleado,
                                          CURP = e.CURP,
                                          // detalleAsistencia = e.detalleAsistencia,
                                          documentacion = e.documentacion,
                                          domicilio = e.domicilio,
                                          estadoCivil = e.estadoCivil,
                                          //estadoNacimiento = e.estadoNacimiento,
                                          estadoNacimiento_ID = e.estadoNacimiento_ID,
                                          // estados = e.estados,
                                          estados_ID = e.estados_ID,
                                          //experienciaLaboralExterna = e.experienciaLaboralExterna,
                                          //experienciaLaboralInterna = e.experienciaLaboralInterna,
                                          //familiares = e.familiares,
                                          fechaIngresoEmpresa = e.fechaIngresoEmpresa,
                                          fechaNacimiento = e.fechaNacimiento,
                                          //finiquitosLiquida = e.finiquitosLiquida,
                                          //formacionEconomica = e.formacionEconomica,
                                          foto = e.foto,
                                          // genero = e.genero,
                                          genero_ID = e.genero_ID,
                                          id = e.id,
                                          IMSS = e.IMSS,
                                          //ingReingresosBajas = e.ingReingresosBajas,
                                          lugarNacimiento = e.lugarNacimiento,
                                          // movNomConcep = e.movNomConcep,
                                          // municipios = e.municipios,
                                          municipios_ID = e.municipios_ID,
                                          nacionalidad = e.nacionalidad,
                                          nombre = e.nombre,
                                          nombreAbreviado = e.nombreAbreviado,
                                          numeroExt = e.numeroExt,
                                          numeroInt = e.numeroInt,
                                          //paises = e.paises,
                                          paises_ID = e.paises_ID,
                                          // paisOrigen = e.paisOrigen,
                                          paisOrigen_ID = e.paisOrigen_ID,
                                          // plazasPorEmpleado = e.plazasPorEmpleado,
                                          // ptuEmpleados = e.ptuEmpleados,
                                          // razonesSociales = e.razonesSociales,
                                          razonesSociales_ID = e.razonesSociales_ID,
                                          //registroIncapacidad = e.registroIncapacidad,
                                          RFC = e.RFC,
                                          // salariosIntegrados = e.salariosIntegrados,
                                          status = e.status,
                                          telefono = e.telefono,
                                          // vacacionesDisfrutadas = e.vacacionesDisfrutadas

                                      }).ToList();
                mensajeResultado.resultado = listaEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("EmpleadosAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje guardarEmpleado(Empleados entity, List<Familiares> agregaModFam, object[] eliminadosFam,
            List<FormacionAcademica> agregaModForAc, object[] eliminadosForAc,
            List<Capacitaciones> agregaModCap, object[] eliminadosCap,
            List<ExperienciaLaboralExterna> agregaModExLbExt, object[] eliminadosExLbExt,
            List<ExperienciaLaboralInterna> agregaModExLbInt, object[] eliminadosExLbInt,
            List<Documentacion> agregaModDoc, object[] eliminadosDoc,
            DBContextAdapter dbContext)
        {
            DbContext dbContextSimple;
            DbContextTransaction transacion;
            using (dbContextSimple = dbContext.context)
            {
                using (transacion = dbContextSimple.Database.BeginTransaction())
                {
                    try
                    {
                        inicializaVariableMensaje();
                        dbContextSimple.Set<Empleados>().AddOrUpdate(entity);
                        dbContextSimple.SaveChanges();
                        int i;

                        //Familiares
                        for (i = 0; i < agregaModFam.Count(); i++)
                        {
                            agregaModFam[i].empleados_ID = entity.id;
                            dbContextSimple.Set<Familiares>().AddOrUpdate(agregaModFam[i]);
                        }
                        dbContextSimple.SaveChanges();
                        foreach (object addFam in eliminadosFam)
                        {
                            decimal idGet = Convert.ToDecimal(addFam);
                            var getObj = new Familiares { id = idGet };
                            dbContextSimple.Set<Familiares>().Attach(getObj);
                            dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                        }
                        dbContextSimple.SaveChanges();

                        //Estudios
                        for (i = 0; i < agregaModForAc.Count(); i++)
                        {
                            agregaModForAc[i].empleados_ID = entity.id;
                            dbContextSimple.Set<FormacionAcademica>().AddOrUpdate(agregaModForAc[i]);
                        }

                        dbContextSimple.SaveChanges();
                        foreach (object getFORAC in eliminadosForAc)
                        {
                            decimal idGet = Convert.ToDecimal(getFORAC);
                            var getObj = new FormacionAcademica { id = idGet };
                            dbContextSimple.Set<FormacionAcademica>().Attach(getObj);
                            dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                        }
                        dbContextSimple.SaveChanges();

                        //Capacitaciones
                        for (i = 0; i < agregaModCap.Count(); i++)
                        {
                            agregaModCap[i].empleados_ID = entity.id;
                            dbContextSimple.Set<Capacitaciones>().AddOrUpdate(agregaModCap[i]);
                        }
                        dbContextSimple.SaveChanges();

                        foreach (object getElimCap in eliminadosCap)
                        {
                            decimal idGet = Convert.ToDecimal(getElimCap);
                            var getObj = new Capacitaciones { id = idGet };
                            dbContextSimple.Set<Capacitaciones>().Attach(getObj);
                            dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                        }
                        dbContextSimple.SaveChanges();

                        //Experiencia Laboral Externa
                        for (i = 0; i < agregaModExLbExt.Count(); i++)
                        {
                            agregaModExLbExt[i].empleados_ID = entity.id;
                            dbContextSimple.Set<ExperienciaLaboralExterna>().AddOrUpdate(agregaModExLbExt[i]);
                        }
                        dbContextSimple.SaveChanges();

                        foreach (object getElimExLbExt in eliminadosExLbExt)
                        {
                            decimal idGet = Convert.ToDecimal(getElimExLbExt);
                            var getObj = new ExperienciaLaboralExterna { id = idGet };
                            dbContextSimple.Set<ExperienciaLaboralExterna>().Attach(getObj);
                            dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                        }
                        dbContextSimple.SaveChanges();

                        //Experiencia Laboral Interna
                        for (i = 0; i < agregaModExLbInt.Count(); i++)
                        {
                            agregaModExLbInt[i].empleados_ID = entity.id;
                            dbContextSimple.Set<ExperienciaLaboralInterna>().AddOrUpdate(agregaModExLbInt[i]);
                        }
                        dbContextSimple.SaveChanges();

                        foreach (object getElimExLbInt in eliminadosExLbInt)
                        {
                            decimal idGet = Convert.ToDecimal(getElimExLbInt);
                            var getObj = new ExperienciaLaboralInterna { id = idGet };
                            dbContextSimple.Set<ExperienciaLaboralInterna>().Attach(getObj);
                            dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                        }
                        dbContextSimple.SaveChanges();

                        //Documentacion
                        for (i = 0; i < agregaModDoc.Count(); i++)
                        {
                            agregaModDoc[i].empleados_ID = entity.id;
                            dbContextSimple.Set<Documentacion>().AddOrUpdate(agregaModDoc[i]);
                        }
                        dbContextSimple.SaveChanges();

                        foreach (object getElimDoc in eliminadosDoc)
                        {
                            decimal idGet = Convert.ToDecimal(getElimDoc);
                            var getObj = new Documentacion { id = idGet };
                            dbContextSimple.Set<Documentacion>().Attach(getObj);
                            dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                        }
                        dbContextSimple.SaveChanges();

                        if (mensajeResultado.noError == 0)
                        {
                            mensajeResultado.resultado = true;
                            mensajeResultado.noError = 0;
                        }
                        transacion.Commit();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("guardarEmpleado()1_Error: ").Append(ex));
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = null;
                        transacion.Rollback();
                    }
                }
            }

            return mensajeResultado;
        }

        public Mensaje eliminarEmpleado(Empleados entity, DBContextAdapter dbContext)
        {
            DbContext dbContextSimple;
            DbContextTransaction transacion;
            using (dbContextSimple = dbContext.context)
            {
                using (transacion = dbContextSimple.Database.BeginTransaction())
                {
                    try
                    {
                        inicializaVariableMensaje();
                        //Delete Familiares
                        dbContextSimple.Set<Familiares>().
                            RemoveRange(dbContextSimple.Set<Familiares>().Where(x => x.empleados_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Delete Formacion Academica
                        dbContextSimple.Set<FormacionAcademica>().
                            RemoveRange(dbContextSimple.Set<FormacionAcademica>().Where(x => x.empleados_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Delete Capacitaciones
                        dbContextSimple.Set<Capacitaciones>().
                           RemoveRange(dbContextSimple.Set<Capacitaciones>().Where(x => x.empleados_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Delete Experiencia Laboral Externa
                        dbContextSimple.Set<ExperienciaLaboralExterna>().
                           RemoveRange(dbContextSimple.Set<ExperienciaLaboralExterna>().Where(x => x.empleados_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Delete Experiencia Laboral Interna
                        dbContextSimple.Set<ExperienciaLaboralInterna>().
                           RemoveRange(dbContextSimple.Set<ExperienciaLaboralInterna>().Where(x => x.empleados_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Documetacion
                        dbContextSimple.Set<Documentacion>().
                           RemoveRange(dbContextSimple.Set<Documentacion>().Where(x => x.empleados_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Validaciones
                        var canSave = 1;
                        bool exiteIngresos = dbContextSimple.Set<IngresosBajas>().Where(x => x.empleados_ID == entity.id).Select(c => c.id).Any();
                        if (exiteIngresos)
                        {
                            canSave = 2;
                        }
                        bool existeCreditos = dbContextSimple.Set<CreditoPorEmpleado>().Where(x => x.empleados_ID == entity.id).Select(c => c.id).Any();
                        if (existeCreditos)
                        {
                            canSave = 3;
                        }

                        if (canSave == 1)
                        {

                            //Delete Empleado
                            dbContextSimple.Set<Empleados>().Attach(entity);
                            dbContextSimple.Set<Empleados>().Remove(entity);
                            dbContextSimple.SaveChanges();

                        }


                        if (mensajeResultado.noError == 0)
                        {
                            if (canSave == 1)
                            {
                                mensajeResultado.resultado = true;
                            }
                            else
                            {
                                mensajeResultado.resultado = canSave;
                            }

                            mensajeResultado.noError = 0;
                        }
                        transacion.Commit();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminarEmpleado()1_Error: ").Append(ex));
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = null;
                        transacion.Rollback();
                    }
                }
            }

            return mensajeResultado;
        }

        public Mensaje SaveEmpleado(List<Familiares> agrega, object[] eliminados, List<FormacionAcademica> agregaFE, object[] eliminadosFE, List<Capacitaciones> agregaCap, object[] eliminadosCap, List<ExperienciaLaboralExterna> agregaELE, object[] eliminadosELE, List<ExperienciaLaboralInterna> agregaELI, object[] eliminadosELI, List<Documentacion> agregaDoc, object[] eliminadosDoc, List<VacacionesAplicacion> agregaVac, Empleados empleados, List<PlazasPorEmpleado> listPlazasPorEmpleados, List<PlazasPorEmpleadosMov> listPlazasPorEmpleadoMov, IngresosBajas ingresosReingresosBajas, SalariosIntegrados salariosIntegrados, object[] eliminadosVac, object[] eliminadosVacDis, object[] eliminadosVacDev, DBContextAdapter dbContext)
        {
            DateTime fechasistemas = new DateTime();
            inicializaVariableMensaje();
            setSession(dbContext.context);
            try
            {
                getSession().Database.BeginTransaction();
                if (eliminados.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "Familiares", "id", eliminados, "");
                    deleteListQuery("Familiares", new CamposWhere("Familiares.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosFE.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "FormacionEconomica", "id", eliminadosFE, "");
                    deleteListQuery("FormacionEconomica", new CamposWhere("FormacionEconomica.id", eliminadosFE, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosCap.Count() > 0)
                {
                    //deleteListQueryIn(getSession(), "Capacitaciones", "id", eliminadosCap, "");
                    deleteListQuery("Capacitaciones", new CamposWhere("Capacitaciones.id", eliminadosCap, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosELE.Count() > 0)
                {
                    //deleteListQueryIn(getSession(), "ExperienciaLaboralExterna", "id", eliminadosELE, "");
                    deleteListQuery("ExperienciaLaboralExterna", new CamposWhere("ExperienciaLaboralExterna.id", eliminadosELE, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosELI.Count() > 0)
                {
                    //deleteListQueryIn(getSession(), "ExperienciaLaboralInterna", "id", eliminadosELI, "");
                    deleteListQuery("ExperienciaLaboralInterna", new CamposWhere("ExperienciaLaboralInterna.id", eliminadosELI, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosDoc.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "Documentacion", "id", eliminadosDoc, "");
                    deleteListQuery("Documentacion", new CamposWhere("Documentacion.id", eliminadosDoc, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosVac.Count() > 0)
                {
                    //deleteListQueryIn(getSession(), "VacacionesAplicacion", "id", eliminadosVac, "");
                    deleteListQuery("VacacionesAplicacion", new CamposWhere("VacacionesAplicacion.id", eliminadosVac, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosVacDis.Count() > 0)
                {
                    //deleteListQueryIn(getSession(), "VacacionesDisfrutadas", "id", eliminadosVacDis, "");
                    deleteListQuery("VacacionesDisfrutadas", new CamposWhere("VacacionesDisfrutadas.id", eliminadosVacDis, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosVacDev.Count() > 0)
                {
                    //deleteListQueryIn(getSession(), "VacacionesDevengadas", "id", eliminadosVacDev, "");
                    deleteListQuery("VacacionesDevengadas", new CamposWhere("VacacionesDevengadas.id", eliminadosVacDev, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (empleados.id == 0)
                {
                    empleados.status = true;
                    getSession().Set<Empleados>().Add(empleados);
                }
                int i;
                //            Plazas por empleado
                for (i = 0; i < listPlazasPorEmpleados.Count(); i++)
                {
                    listPlazasPorEmpleados[i].empleados = empleados;
                    getSession().Set<PlazasPorEmpleado>().Add(listPlazasPorEmpleados[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                //Plazas por empleado
                for (i = 0; i < listPlazasPorEmpleadoMov.Count(); i++)
                {
                    if (listPlazasPorEmpleadoMov[i].plazasPorEmpleado.id == 0)
                    {
                        listPlazasPorEmpleadoMov[i].plazasPorEmpleado = listPlazasPorEmpleados[0];
                    }
                    getSession().Set<PlazasPorEmpleadosMov>().Add(listPlazasPorEmpleadoMov[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                if (ingresosReingresosBajas != null)
                {
                    getSession().Set<IngresosBajas>().Add(ingresosReingresosBajas);
                    getSession().SaveChanges();
                }
                if (salariosIntegrados != null)
                {
                    salariosIntegrados.empleados = empleados;
                    getSession().Set<SalariosIntegrados>().Add(salariosIntegrados);
                    // getSession().SaveChanges();
                }
                //FAMILIARES
                for (i = 0; i < agrega.Count(); i++)
                {
                    agrega[i].empleados = empleados;
                    getSession().Set<Familiares>().Add(agrega[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                //FORMACION ECONOMICA
                for (i = 0; i < agregaFE.Count(); i++)
                {
                    agregaFE[i].empleados = empleados;
                    getSession().Set<FormacionAcademica>().Add(agregaFE[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                //CAPACITACIONES
                for (i = 0; i < agregaCap.Count(); i++)
                {
                    agregaCap[i].empleados = empleados;
                    getSession().Set<Capacitaciones>().Add(agregaCap[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                //EXPERIENCIA LABORAL EXTERNA
                for (i = 0; i < agregaELE.Count(); i++)
                {
                    agregaELE[i].empleados = empleados;
                    getSession().Set<ExperienciaLaboralExterna>().Add(agregaELE[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                //EXPERIENCIA LABORAL INTERNA
                for (i = 0; i < agregaELI.Count(); i++)
                {
                    agregaELI[i].empleados = empleados;
                    getSession().Set<ExperienciaLaboralInterna>().Add(agregaELI[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                //DOCUMENTACION
                for (i = 0; i < agregaDoc.Count(); i++)
                {
                    agregaDoc[i].empleados = empleados;
                    getSession().Set<Documentacion>().Add(agregaDoc[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                if (agregaVac != null)
                {
                    for (i = 0; i < agregaVac.Count(); i++)
                    {
                        agregaVac[i].vacacionesDisfrutadas.empleados = empleados;
                        DateTime? fecha = DateTime.Now;//agregaVac[i].vacacionesDevengadas.empleados;Pendiente de corregir
                        int? anio = agregaVac[i].vacacionesDevengadas.ejercicio;
                        DateTime? cal = new DateTime?();
                        cal = fecha;
                        cal.Value.AddYears(cal.Value.Year + (int)anio);
                        if (fechasistemas.Year == cal.Value.Year)
                        {
                            getSession().Set<VacacionesDevengadas>().AddOrUpdate(agregaVac[i].vacacionesDevengadas);
                        }
                        else
                        {
                            getSession().Set<VacacionesAplicacion>().Add(agregaVac[i]);
                        }
                        if (i % 100 == 0 && i > 0)
                        {
                            getSession().SaveChanges();
                        }

                    }
                }
                mensajeResultado.resultado = empleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("SaveEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        /* private void deleteListQueryIn(Session session, String tabla, String campo, Object[] valores, DbContext dbContext)
         {
             consulta = new StringBuilder("delete ");
             consulta.append(tabla).append(" where ").append(campo).append(" in(:valores)");
             query = session.createQuery(consulta.toString());
             query.setParameterList("valores", valores);
             query.executeUpdate();
         }*/

        public Mensaje UpdateEmpleado(List<Familiares> agrega, object[] eliminados, List<FormacionAcademica> agregaFE, object[] eliminadosFE, List<Capacitaciones> agregaCap, object[] eliminadosCap, List<ExperienciaLaboralExterna> agregaELE, object[] eliminadosELE, List<ExperienciaLaboralInterna> agregaELI, object[] eliminadosELI, List<Documentacion> agregaDoc, object[] eliminadosDoc, List<VacacionesAplicacion> agregaVac, Empleados empleados, List<PlazasPorEmpleadosMov> listPlazasPorEmpleadoMov, IngresosBajas ingresosReingresosBajas, bool calcularSDI, SalariosIntegrados salariosIntegrados, List<VacacionesAplicacion> deleteVacaciones, DBContextAdapter dbContext)
        {
            inicializaVariableMensaje();
            setSession(dbContext.context);
            DateTime fechasistemas = new DateTime();
            Object[] eliminadosVac = new Object[deleteVacaciones.Count()];
            Object[] eliminadosVacDis = new Object[deleteVacaciones.Count()];
            Object[] eliminadosVacDev = new Object[deleteVacaciones.Count()];
            try
            {
                getSession().Database.BeginTransaction();
                if (eliminados.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "Familiares", "id", eliminados, "");
                    deleteListQuery("Familiares", new CamposWhere("Familiares.id", eliminados, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosFE.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "FormacionEconomica", "id", eliminadosFE, "");
                    deleteListQuery("FormacionEconomica", new CamposWhere("FormacionEconomica.id", eliminadosFE, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosCap.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "Capacitaciones", "id", eliminadosCap, "");
                    deleteListQuery("Capacitaciones", new CamposWhere("Capacitaciones.id", eliminadosCap, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosELE.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "ExperienciaLaboralExterna", "id", eliminadosELE, "");
                    deleteListQuery("ExperienciaLaboralExterna", new CamposWhere("ExperienciaLaboralExterna.id", eliminadosELE, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosELI.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "ExperienciaLaboralInterna", "id", eliminadosELI, "");
                    deleteListQuery("ExperienciaLaboralInterna", new CamposWhere("ExperienciaLaboralInterna.id", eliminadosELI, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosDoc.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "Documentacion", "id", eliminadosDoc, "");
                    deleteListQuery("Documentacion", new CamposWhere("Documentacion.id", eliminadosDoc, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (empleados != null & (listPlazasPorEmpleadoMov == null ? true : listPlazasPorEmpleadoMov.Any())) ;
                {
                    getSession().Set<Empleados>().AddOrUpdate(empleados);
                    getSession().SaveChanges();
                }
                int i;
                if (listPlazasPorEmpleadoMov != null)
                {
                    for (i = 0; i < listPlazasPorEmpleadoMov.Count(); i++)
                    {
                        getSession().Set<PlazasPorEmpleadosMov>().AddOrUpdate(listPlazasPorEmpleadoMov[i]);
                        if (i % 100 == 0 && i > 0)
                        {
                            getSession().SaveChanges();
                        }
                    }
                    if (empleados == null)
                    {
                        empleados = listPlazasPorEmpleadoMov[0].plazasPorEmpleado.empleados;

                    }
                }
                if (ingresosReingresosBajas != null)
                {
                    if (listPlazasPorEmpleadoMov != null)
                    {
                        for (i = 0; i < listPlazasPorEmpleadoMov.Count(); i++)
                        {
                            //if (listPlazasPorEmpleadoMov[i].plazasPorEmpleado.referencia.Equals(ingresosReingresosBajas.plazasPorEmpleado.referencia, StringComparison.InvariantCultureIgnoreCase))
                            //{
                            //    ingresosReingresosBajas.plazasPorEmpleado = listPlazasPorEmpleadoMov[i].plazasPorEmpleado;
                            //    break;
                            //}
                        }
                    }
                    getSession().Set<IngresosBajas>().Add(ingresosReingresosBajas);
                    getSession().SaveChanges();
                }
                if (deleteVacaciones != null || !deleteVacaciones.Any())
                {
                    MovimientosNominaDAO MovNomina = new MovimientosNominaDAO();
                    for (int k = 0; k < deleteVacaciones.Count(); k++)
                    {
                        if (deleteVacaciones[k].vacacionesDisfrutadas.statusVacaciones == StatusVacaciones.CALCULADA)
                        {
                            List<Object> listdelvaca = new List<Object>();
                            listdelvaca.Add(deleteVacaciones[k].vacacionesDisfrutadas.empleados);
                            listdelvaca.Add(deleteVacaciones[k].vacacionesDisfrutadas.periodoAplicacion);
                            Mensaje mensaje = MovNomina.buscaMovimientosNominaFiltrado(listdelvaca, dbContext, false);
                            if (mensaje.noError == 0)
                            {
                                List<MovNomConcep> movVaca = (List<MovNomConcep>)mensaje.resultado;
                                Object[] eliminadosMoviVaca = new Object[movVaca.Count()];
                                for (int j = 0; j < movVaca.Count(); j++)
                                {
                                    eliminadosMoviVaca[j] = movVaca[j].id;
                                }
                                if (eliminadosMoviVaca.Count() > 0)
                                {
                                    MovNomina.deleteListQueryMov(typeof(MovNomConcep).Name, "id", eliminadosMoviVaca, null, null, null, true, dbContext);
                                }
                            }
                            else
                            {
                                return mensaje;
                            }
                            eliminadosVac[k] = deleteVacaciones[k].id;
                            eliminadosVacDis[k] = deleteVacaciones[k].vacacionesDisfrutadas.id;
                            eliminadosVacDev[k] = deleteVacaciones[k].vacacionesDevengadas.id;
                        }
                        else
                        {
                            eliminadosVac[k] = deleteVacaciones[k].id;
                            eliminadosVacDis[k] = deleteVacaciones[k].vacacionesDisfrutadas.id;
                            eliminadosVacDev[k] = deleteVacaciones[k].vacacionesDevengadas.id;
                        }
                    }
                }
                if (eliminadosVac.Count() > 0)
                {
                    //deleteListQueryIn(getSession(), "VacacionesAplicacion", "id", eliminadosVac, "");
                    deleteListQuery("VacacionesAplicacion", new CamposWhere("VacacionesAplicacion.id", eliminadosVac, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosVacDis.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "VacacionesDisfrutadas", "id", eliminadosVacDis, "");
                    deleteListQuery("VacacionesDisfrutadas", new CamposWhere("VacacionesDisfrutadas.id", eliminadosVacDis, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }
                if (eliminadosVacDev.Count() > 0)
                {
                    // deleteListQueryIn(getSession(), "VacacionesDevengadas", "id", eliminadosVacDev, "");
                    deleteListQuery("VacacionesDevengadas", new CamposWhere("VacacionesDevengadas.id", eliminadosVacDev, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                }

                if (salariosIntegrados != null)
                {
                    salariosIntegrados.empleados = empleados;
                    getSession().Set<SalariosIntegrados>().AddOrUpdate(salariosIntegrados);
                    //getSession().SaveChanges();
                }
                for (i = 0; i < agrega.Count(); i++)
                {
                    agrega[i].empleados = empleados;
                    getSession().Set<Familiares>().AddOrUpdate(agrega[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                for (i = 0; i < agregaFE.Count(); i++)
                {
                    agregaFE[i].empleados = empleados;
                    getSession().Set<FormacionAcademica>().AddOrUpdate(agregaFE[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                for (i = 0; i < agregaCap.Count(); i++)
                {
                    agregaCap[i].empleados = empleados;
                    getSession().Set<Capacitaciones>().AddOrUpdate(agregaCap[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                for (i = 0; i < agregaELE.Count(); i++)
                {
                    agregaELE[i].empleados = empleados;
                    getSession().Set<ExperienciaLaboralExterna>().AddOrUpdate(agregaELE[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                for (i = 0; i < agregaELI.Count(); i++)
                {
                    agregaELI[i].empleados = empleados;
                    getSession().Set<ExperienciaLaboralInterna>().AddOrUpdate(agregaELI[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                for (i = 0; i < agregaDoc.Count(); i++)
                {
                    agregaDoc[i].empleados = empleados;
                    getSession().Set<Documentacion>().AddOrUpdate(agregaDoc[i]);
                    if (i % 100 == 0 && i > 0)
                    {
                        getSession().SaveChanges();
                    }
                }
                if (agregaVac != null)
                {
                    for (i = 0; i < agregaVac.Count(); i++)
                    {
                        agregaVac[i].vacacionesDisfrutadas.empleados = empleados;
                        DateTime? fecha = DateTime.Now; //agregaVac[i].vacacionesDevengadas.plazasPorEmpleado.fechaPrestaciones;Pendiente de corregir
                        int? anio = agregaVac[i].vacacionesDevengadas.ejercicio;
                        DateTime cal = new DateTime();
                        cal.AddYears(cal.Year + (int)anio);
                        if (fechasistemas.Year == cal.Year)
                        {
                            getSession().Set<VacacionesDevengadas>().AddOrUpdate(agregaVac[i].vacacionesDevengadas);
                        }
                        else
                        {
                            getSession().Set<VacacionesAplicacion>().AddOrUpdate(agregaVac[i]);
                        }
                        if (i % 100 == 0 && i > 0)
                        {
                            getSession().SaveChanges();
                        }
                    }
                }
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("UpdateEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }

        public Mensaje getPorIdEmpleado(decimal? idEmpl, string claveRazon, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var listaEmpleados = (from e in getSession().Set<Empleados>()
                                      where e.id == idEmpl && e.razonesSociales.clave == claveRazon
                                      select new
                                      {
                                          id = e.id,
                                          clave = e.clave,
                                          nombre = e.nombre,
                                          apellidoPaterno = e.apellidoPaterno,
                                          apellidoMaterno = e.apellidoMaterno,
                                          nombreAbreviado = e.nombreAbreviado,
                                          fechaNacimiento = e.fechaNacimiento,
                                          estadoNacimiento_ID = e.estadoNacimiento_ID,
                                          lugarNacimiento = e.lugarNacimiento,
                                          foto = e.foto,
                                          paisOrigen_ID = e.paisOrigen_ID,
                                          nacionalidad = e.nacionalidad,
                                          genero_ID = e.genero_ID,
                                          estadoCivil = e.estadoCivil,
                                          RFC = e.RFC,
                                          CURP = e.CURP,
                                          IMSS = e.IMSS,
                                          clinicaIMSS = e.clinicaIMSS,
                                          correoElectronico = e.correoElectronico,
                                          telefono = e.telefono,
                                          domicilio = e.domicilio,
                                          numeroExt = e.numeroExt,
                                          numeroInt = e.numeroInt,
                                          colonia = e.colonia,
                                          cp_ID = e.cp_ID,
                                          ciudades_ID = e.ciudades_ID,
                                          municipios_ID = e.municipios_ID,
                                          estados_ID = e.estados_ID,
                                          paises_ID = e.paises_ID,
                                          fechaIngresoEmpresa = e.fechaIngresoEmpresa,
                                          status = e.status,
                                          razonesSociales_ID = e.razonesSociales_ID,
                                      }).SingleOrDefault();
                mensajeResultado.resultado = listaEmpleados;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorIdEmpleadoYComplementos(decimal? idEmpl, string claveRazon, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                object[] complementos = new object[7];

                var getEmpleado = (from e in dbContext.context.Set<Empleados>()
                                   where e.id == idEmpl && e.razonesSociales.clave == claveRazon
                                   select new
                                   {
                                       id = e.id,
                                       clave = e.clave,
                                       nombre = e.nombre,
                                       apellidoPaterno = e.apellidoPaterno,
                                       apellidoMaterno = e.apellidoMaterno,
                                       nombreAbreviado = e.nombreAbreviado,
                                       fechaNacimiento = e.fechaNacimiento,
                                       estadoNacimiento_ID = e.estadoNacimiento_ID,
                                       lugarNacimiento = e.lugarNacimiento,
                                       foto = e.foto,
                                       paisOrigen_ID = e.paisOrigen_ID,
                                       nacionalidad = e.nacionalidad,
                                       genero_ID = e.genero_ID,
                                       estadoCivil = e.estadoCivil,
                                       RFC = e.RFC,
                                       CURP = e.CURP,
                                       IMSS = e.IMSS,
                                       clinicaIMSS = e.clinicaIMSS,
                                       correoElectronico = e.correoElectronico,
                                       telefono = e.telefono,
                                       domicilio = e.domicilio,
                                       numeroExt = e.numeroExt,
                                       numeroInt = e.numeroInt,
                                       colonia = e.colonia,
                                       cp_ID = e.cp_ID,
                                       ciudades_ID = e.ciudades_ID,
                                       municipios_ID = e.municipios_ID,
                                       estados_ID = e.estados_ID,
                                       paises_ID = e.paises_ID,
                                       fechaIngresoEmpresa = e.fechaIngresoEmpresa,
                                       status = e.status,
                                       razonesSociales_ID = e.razonesSociales_ID,
                                   }).SingleOrDefault();

                complementos[0] = getEmpleado;
                //dbContextSimple.Set<Empleados>().Add(entity);
                if (getEmpleado != null)
                {
                    var getFamiliares = (from f in dbContext.context.Set<Familiares>()
                                         where f.empleados_ID == getEmpleado.id
                                         select new
                                         {
                                             id = f.id,
                                             isDependiente = f.dependiente,
                                             isEmpleado = f.empleado,
                                             fechaNacimiento = f.fechaNacimiento,
                                             isFinado = f.finado,
                                             nombre = f.nombre,
                                             isSexo =
                                             new
                                             {
                                                 id = f.sexo,
                                                 clave = f.sexo == true ? "001" : "002",
                                                 descripcion = f.sexo == true ? "Femenino" : "Masculino"
                                             },
                                             empleados_ID = f.empleados_ID,
                                             parentesco_ID = f.parentesco_ID,
                                             parentesco_Clave = f.parentesco.clave,
                                             descripcionParentesco = f.parentesco.descripcion,
                                         }).ToList();
                    complementos[1] = getFamiliares;

                    var getFormacionAcademica = (from fo in dbContext.context.Set<FormacionAcademica>()
                                                 where fo.empleados_ID == getEmpleado.id
                                                 select new
                                                 {
                                                     id = fo.id,
                                                     estudios_ID = fo.estudios_ID,
                                                     estudios_Clave = fo.estudios.clave,
                                                     descripcionEstudios = fo.estudios.descripcion,
                                                     institucion = fo.institucion,
                                                     comentarios = fo.comentarios,
                                                     fechaInicio = fo.fechaInicio,
                                                     fechaFin = fo.fechaFin,
                                                     empleados_ID = fo.empleados_ID,

                                                 }).ToList();
                    complementos[2] = getFormacionAcademica;

                    var getCapacitaciones = (from ca in dbContext.context.Set<Capacitaciones>()
                                             where ca.empleados_ID == getEmpleado.id
                                             select new
                                             {
                                                 id = ca.id,
                                                 cursos_ID = ca.cursos_ID,
                                                 cursos_Clave = ca.cursos.clave,
                                                 descripcionCursos = ca.cursos.descripcion,
                                                 comentariosCapacitacion = ca.comentarios,
                                                 fechaInicioCapacitacion = ca.fechaInicio,
                                                 fechaFinCapacitacion = ca.fechaFin,
                                                 empleados_ID = ca.empleados_ID,

                                             }).ToList();
                    complementos[3] = getCapacitaciones;

                    var getExpLabExterna = (from expEx in dbContext.context.Set<ExperienciaLaboralExterna>()
                                            where expEx.empleados_ID == getEmpleado.id
                                            select new
                                            {
                                                id = expEx.id,
                                                puesto = expEx.puesto,
                                                descripcionPuesto = expEx.descripcion,
                                                fechaInicioExpLabExt = expEx.fechaInicio,
                                                fechaFinExpLabExt = expEx.fechaFin,
                                                empresa = expEx.empresa,
                                                jefeInmediato = expEx.jefeInmediato,
                                                comentarios = expEx.comentarios,
                                                empleados_ID = expEx.empleados_ID,

                                            }).ToList();
                    complementos[4] = getExpLabExterna;

                    var getExpLabInterna = (from expEx in dbContext.context.Set<ExperienciaLaboralInterna>()
                                            where expEx.empleados_ID == getEmpleado.id
                                            select new
                                            {
                                                id = expEx.id,
                                                puestos_ID = expEx.puestos_ID,
                                                puestos_Clave = expEx.puestos.clave,
                                                descripcionPuestos = expEx.puestos.descripcion,
                                                centroCostos_ID = expEx.centroCostos_ID,
                                                centroDeCosto_Clave = expEx.centroDeCosto.clave,
                                                descripcionCentroDeCosto = expEx.centroDeCosto.descripcion,
                                                fechaInicioExpLabInt = expEx.fechaInicio,
                                                fechaFinExpLabInt = expEx.fechaFin,
                                                empleados_ID = expEx.empleados_ID,
                                            }).ToList();
                    complementos[5] = getExpLabInterna;

                    var getDocumentacion = (from doc in dbContext.context.Set<Documentacion>()
                                            where doc.empleados_ID == getEmpleado.id
                                            select new
                                            {
                                                id = doc.id,
                                                documento = doc.documento,
                                                descripcionDocumentacion = doc.descripcion,
                                                entrego = doc.entrego,
                                                fechaEntrega = doc.fechaEntrega,
                                                fechaDevolucion = doc.fechaDevolucion,
                                                archivo = doc.archivo
                                            }).ToList();

                    complementos[6] = getDocumentacion;
                }

                mensajeResultado.resultado = complementos;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getEmpleadoPorClave(string claveEmpleado, string claveRazon, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var empleado = (from e in getSession().Set<Empleados>()
                                where e.clave.Equals(claveEmpleado) && e.razonesSociales.clave.Equals(claveRazon)
                                select new
                                {

                                    apellidoMaterno = e.apellidoMaterno,
                                    apellidoPaterno = e.apellidoPaterno,
                                    ciudades_ID = e.ciudades_ID,
                                    clave = e.clave,
                                    clinicaIMSS = e.clinicaIMSS,
                                    colonia = e.colonia,
                                    correoElectronico = e.correoElectronico,
                                    cp_ID = e.cp_ID,
                                    CURP = e.CURP,
                                    domicilio = e.domicilio,
                                    estadoCivil = e.estadoCivil,
                                    estadoNacimiento_ID = e.estadoNacimiento_ID,
                                    estados_ID = e.estados_ID,
                                    fechaIngresoEmpresa = e.fechaIngresoEmpresa,
                                    fechaNacimiento = e.fechaNacimiento,
                                    genero_ID = e.genero_ID,
                                    id = e.id,
                                    IMSS = e.IMSS,
                                    lugarNacimiento = e.lugarNacimiento,
                                    municipios_ID = e.municipios_ID,
                                    nacionalidad = e.nacionalidad,
                                    nombre = e.nombre,
                                    nombreAbreviado = e.nombreAbreviado,
                                    numeroExt = e.numeroExt,
                                    numeroInt = e.numeroInt,
                                    paises_ID = e.paises_ID,
                                    paisOrigen_ID = e.paisOrigen_ID,
                                    razonesSociales_ID = e.razonesSociales_ID,
                                    RFC = e.RFC,
                                    status = e.status,
                                    telefono = e.telefono,

                                }).SingleOrDefault();
                mensajeResultado.resultado = empleado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

    }
}