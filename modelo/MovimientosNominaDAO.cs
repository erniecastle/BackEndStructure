/**
* @author: Daniel Ruelas 
* Fecha de Creación: 17/01/2018
* Compañía: Exito Software
* Descripción del programa: Clase MovimientosNominaDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad.cfdi;
using System.Text;
using System.Reflection;
using Exitosw.Payroll.Core.util;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Data.Entity;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
   
    public class MovimientosNominaDAO : GenericRepository<MovNomConcep>, MovimientosNominaDAOIF
    {
       
        GeneracionDatosTimbreDAO generardatos = new GeneracionDatosTimbreDAO();
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private String[] camposConsulta = new String[]{"razonesSociales", "empleado",
        "tipoNomina", "periodosNomina", "tipoCorrida", "concepNomDefi", "numero", "ejercicio", "mes", "uso"};
        public Mensaje buscaMovimientosNominaFiltrado(List<object> valoresDeFiltrado, DBContextAdapter dbContext)
        {
            return buscaMovimientosNominaFiltrado(valoresDeFiltrado, dbContext, true);
        }

        public Mensaje buscaMovimientosNominaFiltrado(List<object> valoresDeFiltrado, DBContextAdapter dbContext, bool usaCommit)
        {
          
            List<MovNomConcep> movNomConceptos = new List<MovNomConcep>();
            valoresDeFiltrado = valoresDeFiltrado == null ? new List<Object>() : valoresDeFiltrado;
            var query = (from pm in getSession().Set<PlazasPorEmpleadosMov>()
                         join pl in getSession().Set<PlazasPorEmpleado>() on pm.plazasPorEmpleado_ID equals pl.id into pmpl
                         from pl in pmpl.DefaultIfEmpty()
                         join emp in getSession().Set<Empleados>() on pl.empleados_ID equals emp.id into plemp
                         from emp in plemp.DefaultIfEmpty()
                         join rs in getSession().Set<RazonesSociales>() on pl.razonesSociales_ID equals rs.id into plrs
                         from rs in plrs.DefaultIfEmpty()
                         join dep in getSession().Set<Departamentos>() on pm.departamentos_ID equals dep.id into pmdep
                         from dep in pmdep.DefaultIfEmpty()
                         join cent in getSession().Set<CentroDeCosto>() on pm.centroDeCosto_ID equals cent.id into pmcent
                         from cent in pmcent.DefaultIfEmpty()
                         join reg in getSession().Set<RegistroPatronal>() on pl.registroPatronal_ID equals reg.id into plreg
                         from reg in plreg.DefaultIfEmpty()
                         join plaz in getSession().Set<Plazas>() on pm.plazas_ID equals plaz.id into pmplaz
                         from plaz in pmplaz.DefaultIfEmpty()
                         from mov in getSession().Set<MovNomConcep>()
                         join cnc in getSession().Set<ConcepNomDefi>() on mov.concepNomDefi_ID equals cnc.id into movcnc
                         from cnc in movcnc.DefaultIfEmpty()
                         join tipcorr in getSession().Set<TipoCorrida>() on mov.tipoCorrida_ID equals tipcorr.id into movtipcorr
                         from tipcorr in movtipcorr.DefaultIfEmpty()
                         join per in getSession().Set<PeriodosNomina>() on mov.periodosNomina_ID equals per.id into movper
                         from per in movper.DefaultIfEmpty()
                         join tipNom in getSession().Set<TipoNomina>() on mov.tipoNomina_ID equals tipNom.id into movtipNom
                         from tipNom in movtipNom.DefaultIfEmpty()
                         join x9 in getSession().Set<Empleados>() on mov.empleado_ID equals x9.id into movx9
                         from x9 in movx9.DefaultIfEmpty()
                         join x10 in getSession().Set<PlazasPorEmpleado>() on mov.plazasPorEmpleado_ID equals x10.id into movx10
                         from x10 in movx10.DefaultIfEmpty()
                         join x11 in getSession().Set<RazonesSociales>() on mov.razonesSociales_ID equals x11.id into movx11
                         from x11 in movx11.DefaultIfEmpty()
                         select new
                         {
                             pl,
                             rs,
                             tipcorr,
                             per,
                             pm,
                             mov
                         });
            int i;
            bool empleadoInicio = true;
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
            mensajeResultado.resultado = movNomConceptos;
            if (valoresDeFiltrado.Count() > 0)
            {
                Empleados empIni = null;
                Empleados empFin = null;
                try
                {
                    query = (from subquery in query
                             where subquery.pl.id == subquery.mov.plazasPorEmpleado_ID
                             select subquery);
                    for (i = 0; i < valoresDeFiltrado.Count(); i++)
                    {
                        if (valoresDeFiltrado[i].GetType() == typeof(Empleados) && empleadoInicio)
                        {
                            empleadoInicio = false;
                            empIni = (Empleados)valoresDeFiltrado[i];
                            if (empIni.id == 0)
                            {
                                empIni = null;
                            }
                        }
                        else if (valoresDeFiltrado[i].GetType() == typeof(Empleados))
                        {
                            empFin = (Empleados)valoresDeFiltrado[i];
                            if (empFin.id == 0)
                            {
                                empFin = null;
                            }
                        }
                        if (valoresDeFiltrado[i].GetType() == typeof(RazonesSociales))
                        {
                            query = (from subquery in query
                                     where subquery.rs.clave.Equals(((RazonesSociales)valoresDeFiltrado[i]).clave)
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType() == typeof(TipoCorrida))
                        {
                            query = (from subquery in query
                                     where subquery.tipcorr.clave.Equals(((TipoCorrida)valoresDeFiltrado[i]).clave) &&
                                     subquery.per.tipoCorrida.clave.Equals(((TipoCorrida)valoresDeFiltrado[i]).clave)
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType() == typeof(PeriodosNomina))
                        {
                            query = (from subquery in query
                                     where subquery.per.fechaInicial >= ((PeriodosNomina)valoresDeFiltrado[i]).fechaInicial &&
                                     subquery.per.fechaInicial <= ((PeriodosNomina)valoresDeFiltrado[i]).fechaFinal
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType() == typeof(RazonesSociales))
                        {
                            query = (from subquery in query
                                     where (from pm1 in getSession().Set<PlazasPorEmpleadosMov>()
                                            join pl1 in getSession().Set<PlazasPorEmpleado>() on pm1.plazasPorEmpleado_ID equals pl1.id into pmpl1
                                            from pl1 in pmpl1.DefaultIfEmpty()
                                            join rs1 in getSession().Set<RazonesSociales>() on pl1.razonesSociales_ID equals rs1.id into plrs1
                                            from rs1 in plrs1.DefaultIfEmpty()
                                            join mov1 in getSession().Set<Empleados>() on pl1.empleados_ID equals mov1.id into plmov1
                                            from mov1 in plmov1.DefaultIfEmpty()
                                            where rs1.clave.Equals(((RazonesSociales)valoresDeFiltrado[i]).clave)
                                            select new { pm1.id }).Contains(new
                                            {
                                                subquery.pm.id
                                            })
                                     select subquery);
                        }
                        else if (valoresDeFiltrado[i].GetType() == typeof(PeriodosNomina))
                        {
                            query = (from subquery in query
                                     where subquery.pm.fechaInicial == (from x0X in getSession().Set<PlazasPorEmpleadosMov>()
                                                                        join x1X in getSession().Set<PlazasPorEmpleado>() on x0X.plazasPorEmpleado_ID equals x1X.id into x0Xx1X
                                                                        from x1X in x0Xx1X.DefaultIfEmpty()
                                                                        join x3X in getSession().Set<Empleados>() on x1X.empleados_ID equals x3X.id into x1Xx3X
                                                                        from x3X in x1Xx3X.DefaultIfEmpty()
                                                                        where (x0X.fechaInicial <= ((PeriodosNomina)valoresDeFiltrado[i]).fechaFinal) &&
                                                                        subquery.mov.id == x3X.id
                                                                        select new { x0X.fechaInicial }).Max(p => p.fechaInicial)
                                     select subquery);
                        }
                    }
                    if (empIni != null && empFin != null)
                    {
                        query = (from subquery in query
                                 where Convert.ToInt32(subquery.pl.empleados.clave) >= Convert.ToInt32(empIni.clave) &&
                                 Convert.ToInt32(subquery.pl.empleados.clave) <= Convert.ToInt32(empFin)
                                 select subquery);
                    }
                    else if (empIni != null)
                    {
                        query = (from subquery in query
                                 where Convert.ToInt32(subquery.pl.empleados.clave) >= Convert.ToInt32(empIni)
                                 select subquery);
                    }
                    else if (empFin != null)
                    {
                        query = (from subquery in query
                                 where Convert.ToInt32(subquery.pl.empleados.clave) <= Convert.ToInt32(empFin.clave)
                                 select subquery);
                    }
                    var query2 = (from subquery in query
                                  select subquery.mov);
                    inicializaVariableMensaje();
                    setSession(dbContext.context);
                    getSession().Database.BeginTransaction();
                    movNomConceptos = query2.ToList<MovNomConcep>();
                    mensajeResultado.resultado = movNomConceptos;
                    if (usaCommit)
                    {
                        getSession().Database.CurrentTransaction.Commit();
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaMovimientosNominaFiltrado()1_Error: ").Append(ex));
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                    mensajeResultado.resultado = null;
                    getSession().Database.CurrentTransaction.Rollback();
                }
            }

            return mensajeResultado;
        }

        public Mensaje eliminaListaMovimientos(string campo, object[] valores, List<CFDIEmpleado> valoresCFDI, object[] valoresCalculoUnidades, object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, DBContextAdapter dbContext)
        {
            int resultado = 0;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                resultado = deleteListQueryMov(typeof(MovNomConcep).Name, campo, valores, valoresCFDI, valoresCalculoUnidades, valoresReestablecer, incluirEliminadoDiferenteTipoPantalla100, dbContext);
                mensajeResultado.resultado = resultado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminaListaMovimientos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getCalculosUnidadesPorFiltroEspecifico(List<CamposWhere> camposwhere, List<CFDIEmpleado> listCFDIEmpleado, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                mensajeResultado = getCalculosUnidadesPorFiltroEspecificoConsulta(camposwhere, listCFDIEmpleado, dbContext);
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCalculosUnidadesPorFiltroEspecifico()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getMaxNumeroMovimientoPorTipoNominaYPeriodo(string claveTipoNomina, decimal idPeriodo, DBContextAdapter dbContext)
        {
            Object movNomConcep;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                movNomConcep = getSession().Set<MovNomConcep>().Where(m => m.tipoNomina.clave == claveTipoNomina && m.periodosNomina.id == idPeriodo).DefaultIfEmpty().Max(m => (m == null ? 0 : m.numero));
                mensajeResultado.resultado = movNomConcep;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMaxNumeroMovimientoPorTipoNominaYPeriodo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getAllMovimientosNomina(DBContextAdapter dbContext)
        {
            List<MovNomConcep> movNomConcep = new List<MovNomConcep>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                movNomConcep = (from m in getSession().Set<MovNomConcep>()
                                select m).ToList();
                mensajeResultado.resultado = movNomConcep;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMovimientosNominaAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getMovimientosNominaAsc(DBContextAdapter dbContext)
        {
            List<MovNomConcep> movNomConcep = new List<MovNomConcep>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                movNomConcep = (from m in getSession().Set<MovNomConcep>()
                                orderby m.ordenId
                                select m).ToList();
                mensajeResultado.resultado = movNomConcep;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMovimientosNominaAsc()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getMovimientosPorFiltro(List<CamposWhere> camposwhere, DBContextAdapter dbContext)
        {
            List<MovNomConcep> movNomConcep = new List<MovNomConcep>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(typeof(MovNomConcep).Name, TipoFuncion.NINGUNO) };
                OperadorSelect select = new OperadorSelect(camposSelect);
                mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.SELECT,
                    typeof(MovNomConcep).Name, select, null, camposwhere, null, null, null);
                mensajeResultado.resultado = movNomConcep;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMovimientosPorFiltro()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getMovimientosPorFiltroEspecifico(List<CamposWhere> camposwhere, DBContextAdapter dbContext)
        {
            List<MovNomConcep> movNomConcep = new List<MovNomConcep>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".id"), TipoFuncion.NINGUNO) };
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.clave"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.descripcion"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".empleado.clave"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".empleado.nombre"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".empleado.apellidoPaterno"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".empleado.apellidoMaterno"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".plazasPorEmpleado.clave"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".tipoNomina.descripcion"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".periodosNomina.descripcion"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".periodosNomina.id"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".resultado"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".periodosNomina.clave"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".periodosNomina.año"), TipoFuncion.NINGUNO));
                camposSelect.Add(new CamposSelect(String.Concat(typeof(MovNomConcep).Name, ".tipoPantalla"), TipoFuncion.NINGUNO));

                List<CamposOrden> camposorden = new List<CamposOrden>() { new CamposOrden(String.Concat(typeof(MovNomConcep).Name, ".empleado.clave"), TipoOrden.NINGUNO) };
                camposorden.Add(new CamposOrden(String.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.clave"), TipoOrden.NINGUNO));

                OperadorSelect select = new OperadorSelect(camposSelect);
                mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.SELECT,
                    typeof(CalculoUnidades).Name, select, null, camposwhere, null, camposorden, null);
                mensajeResultado.resultado = movNomConcep;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMovimientosPorFiltroEspecifico()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getMovimientosPorPlazaEmpleado(object[] clavesPlazasPorEmpleado, string claveTipoCorrida, string claveRazonSocial, DBContextAdapter dbContext)
        {
            List<MovNomConcep> movNomConcep = new List<MovNomConcep>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                movNomConcep = (from m in getSession().Set<MovNomConcep>()
                                where m.plazasPorEmpleado.referencia.Contains(clavesPlazasPorEmpleado.ToString()) &&
                                m.tipoCorrida.clave == claveTipoCorrida
                                orderby m.plazasPorEmpleado.referencia
                                select m).ToList();
                mensajeResultado.resultado = movNomConcep;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMovimientosPorPlazaEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteMovimientosNomina(List<MovNomConcep> AgreModif, object[] clavesDelete, object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, DBContextAdapter dbContext)
        {
            List<MovNomConcep> onList = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                bool commit = true;
                if (clavesDelete != null)
                {
                    int val = deleteListQueryMov("MovNomConcep", "id", clavesDelete, null, null, valoresReestablecer, incluirEliminadoDiferenteTipoPantalla100, dbContext);
                    if (val == -1)
                    {
                        commit = false;
                    }
                }
                AgreModif = (AgreModif == null ? new List<MovNomConcep>() : AgreModif);
                if (commit && AgreModif.Count > 0)
                {
                    foreach (MovNomConcep Am in AgreModif)
                    {
                        getSession().Set<MovNomConcep>().AddOrUpdate(Am);
                        getSession().SaveChanges();
                    }
                }
                if (commit)
                {
                    mensajeResultado.resultado = null;
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteMovimientosNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                Object[] valores = new Object[camposConsulta.Length];
                double suma;
                for (int i = 0; i < AgreModif.Count; i++)
                {
                    /*Solo aplica para periodo semanales y se suman las cantidades que fueron partidas
                    para reflejar en la lista el valor original del parametro con el que se intento persistir*/
                    if (AgreModif[i].numMovParticion == 2)
                    {
                        List<MovNomConceParam> movParamPrin = AgreModif[i - 1].movNomConceParam;
                        List<MovNomConceParam> movParamSec = AgreModif[i].movNomConceParam;
                        if (movParamPrin != null)
                        {
                            for (int j = 0; j < movParamPrin.Count; j++)
                            {
                                suma = Convert.ToDouble(movParamPrin[j].valor) + Convert.ToDouble(movParamSec[j].valor);
                                movParamPrin[j].valor = agregarMascaraValorNumerico(suma, movParamPrin[j].paraConcepDeNom.mascara.Replace("#", "0"));
                            }
                        }
                        AgreModif.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        valores[0] = AgreModif[i].razonesSociales;
                        valores[1] = AgreModif[i].empleados;
                        valores[2] = AgreModif[i].tipoNomina;
                        valores[3] = AgreModif[i].periodosNomina;
                        valores[4] = AgreModif[i].tipoCorrida;
                        valores[5] = AgreModif[i].concepNomDefi;
                        valores[6] = AgreModif[i].numero;
                        valores[7] = AgreModif[i].ejercicio;
                        valores[8] = AgreModif[i].mes;
                        valores[9] = AgreModif[i].uso;
                        List<CamposWhere> camposwhere = new List<CamposWhere>();
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".razonesSociales.id"), AgreModif[i].razonesSociales.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".empleado.id"), AgreModif[i].empleados.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".tipoNomina.id"), AgreModif[i].tipoNomina.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".periodosNomina.id"), AgreModif[i].periodosNomina.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".tipoCorrida.id"), AgreModif[i].tipoCorrida.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.id"), AgreModif[i].concepNomDefi.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".numero"), AgreModif[i].numero, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".ejercicio"), AgreModif[i].ejercicio, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".mes"), AgreModif[i].mes, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".uso"), AgreModif[i].uso, OperadorComparacion.IGUAL, OperadorLogico.AND));


                        mensajeResultado = existeClave("MovNomConcep", camposwhere, dbContext);
                        MovNomConcep existe = (MovNomConcep)mensajeResultado.resultado;
                        //MovNomConcep existe = (MovNomConcep)existeClave("MovNomConcep", camposConsulta, valores, null);
                        if (existe != null)
                        {//Indicar que existe en base de datos
                            AgreModif[i].IsEnBD = true;

                            //    AgreModif.get(i).setIsEnBD(true);
                            //Object[] control = AgreModif[i].;
                            //    //control[0] = true;//Para indicar que existe en la base de datos
                            //    //control[1] = existe.getId();//para respaldar el ID y poder recuperar el objeto original
                            //    //AgreModif.get(i).setIsEnBdAndBackupID(control);
                        }
                    }
                }
                onList = AgreModif;
                try
                {

                    mensajeResultado.resultado = onList;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();

                }
                catch (Exception exs)
                {
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteMovimientosNomina()1_Error: ").Append(exs));
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = exs.GetBaseException().ToString();
                    mensajeResultado.resultado = null;
                    getSession().Database.CurrentTransaction.Rollback();

                }
            }
            return mensajeResultado;
        }
        public int deleteListQueryMov(String tabla, String campo, Object[] valores, List<CFDIEmpleado> valoresCFDI, Object[] valoresCalculoUnidades, Object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, DBContextAdapter dbContext)
        {
            int exito = 0;
            List<Object> valoresx = new List<Object>();
            try
            {
                if (valoresReestablecer == null ? false : valoresReestablecer.Length > 0)
                {
                    valoresx.AddRange(valoresReestablecer);
                    valoresx.AddRange(valores);
                }
                else
                {

                    valoresx.AddRange(valores);
                }
                List<decimal?> valoresAux = new List<decimal?>();
                decimal[] valorAux = new decimal[valoresx.Count];
                for (int x = 0; x < valoresx.Count; x++)
                {
                    valoresAux.Add(Convert.ToDecimal(valoresx[x].ToString()));
                    valorAux[x] = Convert.ToDecimal(valoresx[x].ToString());
                }
                List<CreditoMovimientos> listCreditoMovimientos = (from o in getSession().Set<MovNomConcep>()
                                                                   join credMov in getSession().Set<CreditoMovimientos>()
                                                                   on o.creditoMovimientos.id equals credMov.id
                                                                   where valoresAux.Contains(o.id)
                                                                   select o.creditoMovimientos
                                                                 ).Distinct().ToList();
                //Elimina CreditoMovimientos y reestablece los importe.
                if (listCreditoMovimientos == null ? false : listCreditoMovimientos.Count > 0 ? false : true)
                {
                    int i, j, k;
                    for (i = 0; i < listCreditoMovimientos.Count; i++)
                    {
                        getSession().Set<CreditoPorEmpleado>().AddOrUpdate(listCreditoMovimientos[i].creditoPorEmpleado);
                        if (listCreditoMovimientos[i].movNomConcep != null)
                        {
                            int movimientosEliminados = 0;
                            for (j = 0; j < listCreditoMovimientos[i].movNomConcep.Count; j++)
                            {
                                listCreditoMovimientos[i].movNomConcep[j].calculado = 0.0;
                                listCreditoMovimientos[i].movNomConcep[j].resultado = 0.0;
                                listCreditoMovimientos[i].movNomConcep[j].creditoMovimientos = null;
                                if (listCreditoMovimientos[i].movNomConcep[j].movNomBaseAfecta != null)
                                {
                                    for (k = 0; k < listCreditoMovimientos[i].movNomConcep[j].movNomBaseAfecta.Count; k++)
                                    {
                                        listCreditoMovimientos[i].movNomConcep[j].movNomBaseAfecta[k].resultado = 0.0;
                                        listCreditoMovimientos[i].movNomConcep[j].movNomBaseAfecta[k].resultadoExento = 0.0;
                                    }
                                }
                                getSession().Set<MovNomConcep>().AddOrUpdate(listCreditoMovimientos[i].movNomConcep[j]);
                                if (listCreditoMovimientos[i].movNomConcep[j].tipoPantalla.Equals(100) || incluirEliminadoDiferenteTipoPantalla100)
                                {
                                    getSession().Set<MovNomConcep>().Attach(listCreditoMovimientos[i].movNomConcep[j]);
                                    getSession().Set<MovNomConcep>().Remove(listCreditoMovimientos[i].movNomConcep[j]);
                                    movimientosEliminados++;
                                }

                            }
                            List<MovNomConcep> m = listCreditoMovimientos[i].movNomConcep;
                            listCreditoMovimientos[i].movNomConcep.RemoveAll(mov => mov.id == Convert.ToDecimal(listCreditoMovimientos[i].movNomConcep.Select(p => p.id)));
                            getSession().Set<CreditoMovimientos>().Attach(listCreditoMovimientos[i]);
                            getSession().Set<CreditoMovimientos>().Remove(listCreditoMovimientos[i]);
                        }
                    }
                }

                if (valores.Length > 0)
                {
                    //getSession().Database.ExecuteSqlCommand("delete from MovNomBaseAfecta o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", valores.ToArray()));
                    Mensaje mensaje;
                    mensaje = deleteListQuery("MovNomBaseAfecta", new CamposWhere("MovNomBaseAfecta.movNomConcep.id", valores.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    if (mensaje.error.Equals(""))
                    {
                        mensaje = deleteListQuery("MovNomConceParam", new CamposWhere("MovNomConceParam.movNomConcep.id", valores.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    }
                    // getSession().Database.ExecuteSqlCommand("delete from MovNomConceParam o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", valorAux));
                    if (mensaje.error.Equals(""))
                    {
                        mensaje = deleteListQuery("CalculoISR", new CamposWhere("CalculoISR.movNomConcep.id", valores.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    }
                    //getSession().Database.ExecuteSqlCommand("delete from CalculoISR o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", valores));
                    if (mensaje.error.Equals(""))
                    {
                        mensaje = deleteListQuery("CalculoIMSS", new CamposWhere("CalculoIMSS.movNomConcep.id", valores.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    }
                    // getSession().Database.ExecuteSqlCommand("delete from CalculoIMSS o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", valores));
                    if (mensaje.error.Equals(""))
                    {
                        mensaje = deleteListQuery("CalculoIMSSPatron", new CamposWhere("CalculoIMSSPatron.movNomConcep.id", valores.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    }
                    //  getSession().Database.ExecuteSqlCommand("delete from CalculoIMSSPatron o  where o.movNomConcep.id in(@valores)", new SqlParameter(" @valores", valores));
                    if (mensaje.error.Equals(""))
                    {
                        mensaje = deleteListQuery(tabla, new CamposWhere(tabla + "." + campo, valores.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                    }
                    ///  getSession().Database.ExecuteSqlCommand("delete from " + tabla + " o  where  " + campo + "  in(@valores)", new SqlParameter(" @valores", valores));
                    

                    if (mensaje.error.Equals(""))
                    {
                        exito = 1;
                       // getSession().SaveChanges();
                    }
                    else {
                        exito = 0;
                        //getSession().Database.CurrentTransaction.Rollback();
                    }

                }
                if (valoresReestablecer == null ? false : valoresReestablecer.Length > 0)
                {
                    if (exito == 1) 
                    {
                        List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(String.Concat(typeof(MovNomConcep).Name), TipoFuncion.NINGUNO) };
                        OperadorSelect select = new OperadorSelect(camposSelect);
                        List<CamposWhere> camposwhere = new List<CamposWhere>() { new CamposWhere(String.Concat(typeof(MovNomConcep).Name, campo), valoresReestablecer, OperadorComparacion.IN, OperadorLogico.AND) };
                        mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.SELECT,
                            typeof(MovNomConcep).Name, select, null, camposwhere, null, null, null);
                        List<MovNomConcep> listMovNomConcepReestablecer = (List<MovNomConcep>)mensajeResultado.resultado;
                        if (listMovNomConcepReestablecer != null)
                        {
                            int j, k;
                            for (j = 0; j < listMovNomConcepReestablecer.Count; j++)
                            {
                                listMovNomConcepReestablecer[j].calculado = 0.0;
                                listMovNomConcepReestablecer[j].resultado = 0.0;
                                listMovNomConcepReestablecer[j].creditoMovimientos = null;
                                if (listMovNomConcepReestablecer[j].movNomBaseAfecta != null)
                                {
                                    for (k = 0; k < listMovNomConcepReestablecer[j].movNomBaseAfecta.Count; k++)
                                    {
                                        listMovNomConcepReestablecer[j].movNomBaseAfecta[k].resultado = 0.0;
                                        listMovNomConcepReestablecer[j].movNomBaseAfecta[k].resultadoExento = 0.0;
                                    }
                                }



                                getSession().Set<MovNomConcep>().AddOrUpdate(listMovNomConcepReestablecer[j]);
                               // getSession().SaveChanges();
                                if (listMovNomConcepReestablecer[j].tipoPantalla.Equals(100) || incluirEliminadoDiferenteTipoPantalla100)
                                {
                                    getSession().Set<MovNomConcep>().Attach(listMovNomConcepReestablecer[j]);
                                    getSession().Set<MovNomConcep>().Remove(listMovNomConcepReestablecer[j]);
                                   // getSession().SaveChanges();
                                }
                            }


                        }
                    }
                    

                }
                if (valoresCalculoUnidades != null ? valoresCalculoUnidades.Length > 0 : false)
                {
                    if (exito == 1) {
                        // getSession().Database.ExecuteSqlCommand("delete from CalculoUnidades o  where " + campo + " in(@valores)", new SqlParameter(" @valores", valores));
                        // getSession().SaveChanges();

                        Mensaje mensaje;
                        mensaje = deleteListQuery("CalculoUnidades", new CamposWhere("CalculoUnidades.id", valoresCalculoUnidades.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);

                        if (mensaje.error.Equals(""))
                        {
                            exito = 1;
                            //getSession().SaveChanges();
                        }
                        else
                        {
                            exito = 0;
                            //getSession().Database.CurrentTransaction.Rollback();
                        }
                    }                       
                }

                if (valoresCFDI == null ? false : valoresCFDI.Count > 0)
                {
                    if (exito == 1) {
                        Mensaje mensaje;
                        foreach (CFDIEmpleado cfdiEmpl in valoresCFDI)
                        {

                            //exito = (int)mensaje.resultado;
                            if (cfdiEmpl.id != 0)
                            {
                                decimal idrecibo = cfdiEmpl.cfdiRecibo.id;
                                List<decimal> cfdiCnc = (from o in getSession().Set<CFDIReciboConcepto>()
                                                         join cfd in getSession().Set<CFDIRecibo>() on
                                                         o.cfdiRecibo.id equals cfd.id
                                                         where cfd.id == idrecibo
                                                         select o.id).ToList();
                                if (cfdiCnc.Any())
                                {
                                    int cont = 0;
                                    List<object> datos = new List<object>();

                                    while (cont < cfdiCnc.Count)
                                    {
                                        datos.Add(cfdiCnc[cont]);

                                        cont++;
                                    }

                                    mensaje = deleteListQuery("CFDIReciboConcepto", new CamposWhere("CFDIReciboConcepto.id", datos.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                                    // exito = (int)mensaje.resultado;
                                    if (mensaje.error.Equals(""))
                                    {
                                        exito = 1;
                                        
                                    }
                                    else
                                    {
                                        exito = 0;
                                        
                                    }
                                }

                                List<decimal> cfdiInc = (from o in getSession().Set<CFDIReciboIncapacidad>()
                                                         join cfd in getSession().Set<CFDIRecibo>() on
                                                         o.cfdiRecibo.id equals cfd.id
                                                         where cfd.id == idrecibo
                                                         select o.id).ToList();
                                if (cfdiInc.Any())
                                {
                                    int cont = 0;
                                    List<object> datos = new List<object>();

                                    while (cont < cfdiInc.Count)
                                    {
                                        datos.Add(cfdiInc[cont]);

                                        cont++;
                                    }
                                    mensaje = deleteListQuery("CFDIReciboIncapacidad", new CamposWhere("CFDIReciboIncapacidad.id", datos.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                                    //exito = (int)mensaje.resultado;
                                    if (mensaje.error.Equals(""))
                                    {
                                        exito = 1;
                                        //getSession().SaveChanges();
                                    }
                                    else
                                    {
                                        exito = 0;
                                        //getSession().Database.CurrentTransaction.Rollback();
                                    }
                                }
                                List<decimal> cfdiHrs = (from o in getSession().Set<CFDIReciboHrsExtras>()
                                                         join cfd in getSession().Set<CFDIRecibo>() on
                                                         o.cfdiRecibo.id equals cfd.id
                                                         where cfd.id == idrecibo
                                                         select o.id).ToList();
                                if (cfdiHrs.Any())
                                {
                                    int cont = 0;
                                    List<object> datos = new List<object>();

                                    while (cont < cfdiHrs.Count)
                                    {
                                        datos.Add(cfdiHrs[cont]);

                                        cont++;
                                    }
                                    mensaje = deleteListQuery("CFDIReciboHrsExtras", new CamposWhere("CFDIReciboHrsExtras.id", datos.ToArray(), OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                                    // exito = (int)mensaje.resultado;
                                    if (mensaje.error.Equals(""))
                                    {
                                        exito = 1;
                                        //getSession().SaveChanges();
                                    }
                                    else
                                    {
                                        exito = 0;
                                        //getSession().Database.CurrentTransaction.Rollback();
                                    }
                                }
                                List<object> datos1 = new List<object>();
                                datos1.Add(idrecibo);
                                mensaje = deleteListQuery("CFDIRecibo", new CamposWhere("CFDIRecibo.id", datos1, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                                //exito = (int)mensaje.resultado;
                                if (mensaje.error.Equals(""))
                                {
                                    exito = 1;
                                    
                                }
                                else
                                {
                                    exito = 0;
                                    
                                }

                                
                               /* mensaje = deleteListQuery("CFDIEmpleado", new CamposWhere("CFDIEmpleado.id", cfdiEmpl.id, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                                //exito = (int)mensaje.resultado;
                                if (mensaje.error.Equals(""))
                                {
                                    exito = 1;
                                    //getSession().SaveChanges();
                                }
                                else
                                {
                                    exito = 0;
                                    //getSession().Database.CurrentTransaction.Rollback();
                                }*/


                            }
                        }
                    }
                        

                }
                if (exito ==1)
                {
                    
                    getSession().SaveChanges();
                }
                else
                {
                    
                    getSession().Database.CurrentTransaction.Rollback();
                }


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListQueryMov()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return exito;
        }
        private String agregarMascaraValorNumerico(Object valor, String formato)
        {
            String resultado;

            resultado = String.Format(formato, valor);
            //DecimalFormat decimalFormat = new DecimalFormat(formato);
            //if (formato.lastIndexOf("&") != -1)
            //{
            //    decimalFormat.setGroupingSize(3);
            //}
            //resultado = decimalFormat.format(valor);
            return resultado;
        }

        public Mensaje getCalculosUnidadesPorFiltroEspecificoConsulta(List<CamposWhere> camposwhere, List<CFDIEmpleado> listCFDIEmpleado, DBContextAdapter dbContext)
        {
            List<CalculoUnidades> listCalculoUnidades = null;
            List<decimal> idCalculoUnidades = null;
            inicializaVariableMensaje();
            try
            {
                List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(typeof(CalculoUnidades).Name, TipoFuncion.NINGUNO) };
                OperadorSelect select = new OperadorSelect(camposSelect);
                mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.SELECT,
                    typeof(CalculoUnidades).Name, select, null, camposwhere, null, null, null);
                listCalculoUnidades = (List<CalculoUnidades>)mensajeResultado.resultado;
                if (listCFDIEmpleado == null)
                {
                    listCFDIEmpleado = new List<CFDIEmpleado>();
                }
                if (listCalculoUnidades == null)
                {
                    listCalculoUnidades = new List<CalculoUnidades>();
                }
                idCalculoUnidades = new List<decimal>();
                bool encontrotimbrado = false;
                for (int i = 0; i < listCalculoUnidades.Count; i++)
                {
                    if (listCFDIEmpleado.Any())
                    {
                        idCalculoUnidades.Add(listCalculoUnidades[i].id);
                    }
                    else
                    {
                        encontrotimbrado = false;
                        for (int j = 0; j < listCFDIEmpleado.Count; j++)
                        {
                            if (listCFDIEmpleado[j].plazasPorEmpleadosMov.plazasPorEmpleado.empleados.id.Equals(listCalculoUnidades[i].empleados.id)
                                && listCFDIEmpleado[j].tipoNomina.id.Equals(listCalculoUnidades[i].tipoNomina.id)
                                && listCFDIEmpleado[j].periodosNomina.id.Equals(listCalculoUnidades[i].periodosNomina.id)
                                && listCFDIEmpleado[j].razonesSociales.id.Equals(listCalculoUnidades[i].razonesSociales.id)
                                && listCFDIEmpleado[j].tipoCorrida.id.Equals(listCalculoUnidades[i].tipoCorrida.id)
                                && listCFDIEmpleado[j].cfdiRecibo.statusTimbrado.Equals(StatusTimbrado.TIMBRADO))
                            {
                                encontrotimbrado = true;
                                break;
                            }
                        }
                        if (!encontrotimbrado)
                        {
                            idCalculoUnidades.Add(listCalculoUnidades[i].id);
                        }
                    }
                }
                mensajeResultado.resultado = idCalculoUnidades;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCalculosUnidadesPorFiltroEspecificoConsulta()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteMovNomConcep(List<object> entitysCambios, List<object> clavesDelete, Dictionary<string, object> listaParametros, int rango, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                List<MovNomConcep> agregar = new List<MovNomConcep>();
                object[] clavesDel = new object[clavesDelete.Count];

                bool commit = true;
                for (int i = 0; i < clavesDelete.Count; i++)
                {
                    clavesDel[i] = ((MovNomConcep)clavesDelete[i]).id;// clavesDelete[i].id;
                }
                if (clavesDel != null && clavesDel.Length > 0)
                {
                    //deleteListQuerys("Estados", "Clave", clavesDelete, "");
                    int val = deleteListQueryMov("MovNomConcep", "id", clavesDel, null, null, null, false, dbContext);
                    // deleteListQuerys("Estados", new CamposWhere("Estados.id", clavesDel, OperadorComparacion.IN, OperadorLogico.AND), null);
                    //getSession().SaveChanges();
                    if (val == -1)
                    {
                        commit = false;
                    }

                }
                if (commit)
                {

                    for (int i = 0; i < entitysCambios.Count; i++)
                    {
                        MovNomConcep movData = (MovNomConcep)entitysCambios[i];
                        //if (movData.id == 0)
                        //{
                        MovNomConcep mov = construirMovNomConcp(movData);
                        List<MovNomConceParam> param = new List<MovNomConceParam>();

                        if (listaParametros.Count > 0)
                        {
                            param = construirParametros(listaParametros[movData.numero.ToString()]);
                            mov.movNomConceParam = param;
                        }

                        agregar.Add(mov);
                        //}


                    }
                    agregar = generaMovsPeriodos2Meses(agregar);
                    agregar = proporcionarParametrosMovimiento(agregar );
                    for (int j = 0; j < agregar.Count; j++)
                    {
                        agregar[j].periodosNomina = null;
                        agregar[j].plazasPorEmpleado = null;
                        for (int i = 0; i < agregar[j].movNomConceParam.Count; i++)
                        {
                            agregar[j].movNomConceParam[i].paraConcepDeNom = null;
                            agregar[j].movNomConceParam[i].movNomConcep = null;
                        }
                        if (agregar[j].id == 0)
                        {
                            getSession().Set<MovNomConcep>().Add(agregar[j]);
                            getSession().SaveChanges();
                        }
                        else {
                            for (int x = 0; x < agregar[j].movNomConceParam.Count; x++)
                            {
                                getSession().Set<MovNomConceParam>().AddOrUpdate(agregar[j].movNomConceParam[x]);
                                getSession().SaveChanges();
                            }
                            getSession().Set<MovNomConcep>().AddOrUpdate(agregar[j]);
                            getSession().SaveChanges();
                        }
                    }

                }
                mensajeResultado.resultado = agregar;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                if (commit)
                {
                    getSession().Database.CurrentTransaction.Commit();
                }
                else {
                    getSession().Database.CurrentTransaction.Rollback();
                }

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteMovNomConcep()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public MovNomConcep construirMovNomConcp(MovNomConcep mData)
        {

            PeriodosNominaDAO perDao = new PeriodosNominaDAO();
            PeriodosNomina per = new PeriodosNomina();
            Mensaje mensajeper = perDao.getPeriodosNominaPorID(mData.periodosNomina_ID, null);
            per = (PeriodosNomina)mensajeper.resultado;
            DateTime fechaperiodo = per.fechaInicial.GetValueOrDefault();
            mData.ejercicio = fechaperiodo.Year;
            mData.mes = fechaperiodo.Month;
            mData.fechaIni = fechaperiodo;
            mData.fechaCierr = per.fechaCierre;
            mData.periodosNomina = per;
            PlazasPorEmpleadoDAO plDao = new PlazasPorEmpleadoDAO();
            PlazasPorEmpleado pl = new PlazasPorEmpleado();
            Mensaje mensajepl = plDao.getPlazasPorEmpleadosActivosId(mData.empleado_ID, mData.razonesSociales_ID, per.fechaInicial, per.fechaFinal, mData.tipoNomina_ID, 1, null);
            pl = (PlazasPorEmpleado)mensajepl.resultado;
            mData.plazasPorEmpleado_ID = pl.id;
            mData.plazasPorEmpleado = pl;

            return mData;
        }
        public List<MovNomConceParam> construirParametros(object param)
        {
            List<MovNomConceParam> parametros = new List<MovNomConceParam>();
            var datos = JsonConvert.DeserializeObject<object[]>(param.ToString());
            for (int i = 0; i < datos.Count(); i++)
            {
                Dictionary<string, object> valores = JsonConvert.DeserializeObject<Dictionary<string, object>>(datos[i].ToString());
                MovNomConceParam mnpara = new MovNomConceParam();
                foreach (var item in valores)
                {
                    if (item.Key.Equals("id"))
                    {
                        if (!valores.ContainsKey("paraConcepDeNom_ID"))
                        {
                            mnpara.paraConcepDeNom_ID = Convert.ToDecimal(item.Value.ToString());
                        }
                        else
                        {
                            mnpara.id = Convert.ToDecimal(item.Value.ToString());
                        }
                    }
                    else if (item.Key.Equals("movNomConcep_ID"))
                    {
                        mnpara.movNomConcep_ID = Convert.ToDecimal(item.Value.ToString());
                    }
                    else if (item.Key.Equals("paraConcepDeNom_ID"))
                    {
                        mnpara.paraConcepDeNom_ID = Convert.ToDecimal(item.Value.ToString());
                    }
                    else if (item.Key.Equals("movNomConcep"))
                    {
                        mnpara.movNomConcep = null;
                    }
                    else if (item.Key.Equals("paraConcepDeNom"))
                    {
                        mnpara.paraConcepDeNom = null;
                    }
                    else {
                        mnpara.valor = item.Value.ToString();
                    }
                }
                parametros.Add(mnpara);
            }
            return parametros;

        }

        private List<MovNomConcep> generaMovsPeriodos2Meses(List<MovNomConcep> dataSave)
        {
            int i;
            List<MovNomConcep> nuevosMov = new List<MovNomConcep>();
            for (i = 0; i < dataSave.Count; i++)
            {
                nuevosMov.Add(dataSave[i]);
                if (evaluaMesesPeriodoMovimiento(dataSave[i]))
                {
                    MovNomConcep newMov = MovNomConcep.copiaMovimiento(dataSave[i]);
                    DateTime fechaPeriodo = newMov.periodosNomina.fechaInicial.GetValueOrDefault();
                    newMov.ejercicio = fechaPeriodo.Year;
                    newMov.mes = fechaPeriodo.Month + 1;
                    newMov.numMovParticion = 2;
                    nuevosMov.Add(newMov);
                }

            }
            dataSave.Clear();
            dataSave.AddRange(nuevosMov);
            nuevosMov.Clear();
            return dataSave;
        }
        private bool evaluaMesesPeriodoMovimiento(MovNomConcep mov)
        {
            DateTime fechaInicio = mov.periodosNomina.fechaInicial.GetValueOrDefault();
            DateTime fechaFinal = mov.periodosNomina.fechaFinal.GetValueOrDefault();

            if (fechaInicio.Month != fechaFinal.Month)
            {
                return true;
            }

            return false;
        }
        private List<MovNomConcep> proporcionarParametrosMovimiento(List<MovNomConcep> dataSave)
        {
            int i, dias, diasTotales;
            double valor, proporcion;
            DateTime cFecha = DateTime.Now;
            for (i = 0; i < dataSave.Count; i++)
            {
                if (evaluaMesesPeriodoMovimiento(dataSave[i]) && dataSave[i].movNomConceParam != null)
                {
                    if (dataSave[i].movNomConceParam.Count > 0)
                    {
                        if (dataSave[i].numMovParticion == 1)
                        {
                            cFecha = dataSave[i].periodosNomina.fechaInicial.GetValueOrDefault();
                            cFecha = new DateTime(cFecha.Year, cFecha.Month, DateTime.DaysInMonth(cFecha.Year, cFecha.Month));
                            dias = (cantidadDiasEntreDosFechas(dataSave[i].periodosNomina.fechaInicial.GetValueOrDefault(), cFecha) + 1);
                        }
                        else {
                            cFecha = dataSave[i].periodosNomina.fechaFinal.GetValueOrDefault();
                            cFecha = new DateTime(cFecha.Year, cFecha.Month, 1);
                            dias = (cantidadDiasEntreDosFechas(cFecha, dataSave[i].periodosNomina.fechaFinal.GetValueOrDefault()) + 1);
                        }
                        diasTotales = dataSave[i].periodosNomina.diasPago.GetValueOrDefault();
                        foreach (MovNomConceParam movParam in dataSave[i].movNomConceParam)
                        {
                            // ParametroConceptosDeNominaDAO paraConcepDAO = new ParametroConceptosDeNominaDAO();
                            //ParaConcepDeNom paraConcep = (ParaConcepDeNom)((Mensaje)paraConcepDAO.getPorClaveParametroConceptosDeNominaID(movParam.paraConcepDeNom_ID)).resultado;
                            ParaConcepDeNom paraConcep = (from p in getSession().Set<ParaConcepDeNom>() where p.id == movParam.paraConcepDeNom_ID select p).SingleOrDefault();
                            movParam.paraConcepDeNom = paraConcep;
                            if (movParam.paraConcepDeNom.clasificadorParametro == ClasificadorParametro.ENTRADA &&
                                string.Equals(movParam.paraConcepDeNom.tipo, "Integer", StringComparison.OrdinalIgnoreCase))
                            {
                                valor = movParam.valor == null ? 0.0 : Convert.ToDouble(movParam.valor);
                                proporcion = (valor * dias) / diasTotales;
                                movParam.valor = proporcion.ToString();
                                //movParam.setValor(util.agregarMascaraValorNumerico(proporcion, movParam.getParaConcepDeNom().getMascara().replace("#", "0")));//pendiente
                            }
                        }
                    }
                }
            }
            return dataSave;

        }

        private int cantidadDiasEntreDosFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return (fechaFin - fechaInicio).Days;
            //Calendar cResultado = Calendar.getInstance();
            //Calendar cInicio = Calendar.getInstance(), cFin = Calendar.getInstance();
            //cInicio.setTime(fechaInicio);
            //cFin.setTime(fechaFin);
            //cResultado.setTimeInMillis(cFin.getTime().getTime() - cInicio.getTime().getTime());
            //return cResultado.get(Calendar.DAY_OF_YEAR);
        }

        public Mensaje getMaxNumeroMovimientoPorTipoNominaYPeriodoID(decimal? claveTipoNomina, decimal? idPeriodo, DBContextAdapter dbContext)
        {
            Object movNomConcep;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                movNomConcep = getSession().Set<MovNomConcep>().Where(m => m.tipoNomina.id == claveTipoNomina && m.periodosNomina.id == idPeriodo).DefaultIfEmpty().Max(m => (m == null ? 0 : m.numero));
                mensajeResultado.resultado = movNomConcep;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMaxNumeroMovimientoPorTipoNominaYPeriodo()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorFiltrosMovNomConcep(List<CamposWhere> camposwhere, long inicio, long rango, DBContextAdapter dbContext)
        {
            ValoresRango rangos;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
               
                List<CamposWhere> camposwheres = new List<CamposWhere>();
                //foreach (var item in campos)
                //{
                //    if (!item.Key.ToString().Equals("") && item.Value != null)
                //    {
                //        CamposWhere campo = new CamposWhere();
                //        campo.campo = "MovNomConcep." + item.Key.ToString();
                //        campo.valor = item.Value;
                //        if (operador == "=")
                //        {
                //            campo.operadorComparacion = OperadorComparacion.IGUAL;
                //        }
                //        else if (operador == "like")
                //        {
                //            campo.operadorComparacion = OperadorComparacion.LIKE;
                //        }
                //        campo.operadorLogico = OperadorLogico.AND;
                //        camposwheres.Add(campo);
                //    }

                //}
                if (Convert.ToInt32(rango) > 0)
                {
                    rangos = new ValoresRango(Convert.ToInt32(inicio), Convert.ToInt32(rango));

                }
                else {
                    rangos = null;
                }
                Mensaje mensaje = consultaPorRangos(rangos, camposwhere, dbContext);

                List<object> movNonConcep = (List<object>)mensaje.resultado;
                decimal?[] idmov = new decimal?[movNonConcep.Count];
                for (int i = 0; i < movNonConcep.Count; i++)
                {
                    object[] movaux = (object[])movNonConcep[i];
                    MovNomConcep mov = (MovNomConcep)movaux[0];
                    idmov[i] = mov.id;
                    //Mensaje m = consultaMovconcepParam(mov.id);
                    //mov.movNomConceParam = (List<MovNomConceParam>)m.resultado;
                    //movaux[0] = mov;
                    //movNonConcep.RemoveAt(i);
                    //movNonConcep.Insert(i, movaux);
                }
             

                Mensaje m = consultaMovconcepParam(idmov);
                mensajeResultado.resultado = m.resultado;
                mensajeResultado.noError = m.noError;
                mensajeResultado.error = m.error;
                getSession().Database.CurrentTransaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorFiltroMovNomConcep()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaMovconcepParam(decimal?[] idMov)
        {
            //List<MovNomConceParam> movNomConcepParam = new List<MovNomConceParam>();
            try
            {
                
                
                var movNomConcepParam = (from mp in getSession().Set<MovNomConcep>()
                                         where idMov.Contains(mp.id)
                                         select new
                                         {
                                             //finiqLiquidCncNom=  mp.finiqLiquidCncNom,
                                             finiqLiquidCncNom_ID = mp.finiqLiquidCncNom_ID,
                                             calculado = mp.calculado,
                                             // calculoIMSS=mp.calculoIMSS,
                                             // calculoIMSSPatron= mp.calculoIMSSPatron,
                                             // calculoISR=mp.calculoISR,
                                             centroDeCosto = mp.centroDeCosto == null ? null : new
                                             {
                                                 mp.centroDeCosto.clave,
                                                 mp.centroDeCosto.descripcion,
                                                 mp.centroDeCosto.id
                                             },
                                             centroDeCosto_ID = mp.centroDeCosto_ID,
                                             concepNomDefi = new
                                             {
                                                 mp.concepNomDefi.clave,
                                                 mp.concepNomDefi.descripcion,
                                                 mp.concepNomDefi.id
                                             },
                                             concepNomDefi_ID = mp.concepNomDefi_ID,
                                             // creditoMovimientos=mp.creditoMovimientos,
                                             creditoMovimientos_ID = mp.creditoMovimientos_ID,
                                             ejercicio = mp.ejercicio,
                                             empleados = new
                                             {
                                                 mp.empleados.id,
                                                 mp.empleados.clave,
                                                 mp.empleados.nombre,
                                                 mp.empleados.apellidoPaterno,
                                                 mp.empleados.apellidoMaterno
                                             },
                                             empleado_ID = mp.empleado_ID,
                                             fechaCierr = mp.fechaCierr,
                                             fechaIni = mp.fechaIni,
                                             id = mp.id,
                                             mes = mp.mes,
                                             movNomBaseAfecta = mp.movNomBaseAfecta.Select(p => new
                                             {
                                                 // baseAfecConcepNom= p.baseAfecConcepNom,
                                                 baseAfecConcepNom_ID = p.baseAfecConcepNom_ID,
                                                 id = p.id,
                                                 // movNomConcep= p.movNomConcep,
                                                 movNomConcep_ID = p.movNomConcep_ID,
                                                 resultado = p.resultado,
                                                 resultadoExento = p.resultadoExento,
                                                 uso = p.uso
                                             }).ToList(),
                                             movNomConceParam = mp.movNomConceParam.Select(p => new
                                             {
                                                 // movNomConcep=p.movNomConcep,
                                                 movNomConcep_ID = p.movNomConcep_ID,
                                                 paraConcepDeNom = new
                                                 {
                                                     p.paraConcepDeNom.clasificadorParametro,
                                                     p.paraConcepDeNom.descripcion,
                                                     p.id,
                                                     p.paraConcepDeNom.mascara,
                                                     p.paraConcepDeNom.numero,
                                                     p.paraConcepDeNom.tipo,
                                                     p.paraConcepDeNom.unidad
                                                 },
                                                 paraConcepDeNom_ID = p.paraConcepDeNom_ID,
                                                 id = p.id,
                                                 valor = p.valor
                                             }).ToList(),

                                             numero = mp.numero,
                                             numMovParticion = mp.numMovParticion,
                                             ordenId = mp.ordenId,
                                             //periodosNomina= mp.periodosNomina,
                                             periodosNomina_ID = mp.periodosNomina_ID,
                                             // plazasPorEmpleado=mp.plazasPorEmpleado,
                                             plazasPorEmpleado_ID = mp.plazasPorEmpleado_ID,
                                             // razonesSociales= mp.razonesSociales,
                                             razonesSociales_ID = mp.razonesSociales_ID,
                                             // tipoCorrida=  mp.tipoCorrida,
                                             tipoCorrida_ID = mp.tipoCorrida_ID,
                                             //tipoNomina= mp.tipoNomina,
                                             tipoNomina_ID = mp.tipoNomina_ID,
                                             tipoPantalla = mp.tipoPantalla,
                                             uso = mp.uso
                                             // vacacionesAplicacion=mp.vacacionesAplicacion




                                         }
                                    ).ToList();
                mensajeResultado.resultado = movNomConcepParam;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaMovconcepParam()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje saveDeleteMovNomina(List<MovNomConcep> AgreModif, object[] clavesDelete, object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, DBContextAdapter dbContext)
        {
            List<MovNomConcep> onList = null;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                bool commit = true;
                bool exito = true;
                if (clavesDelete != null)
                {
                    int val = deleteListQueryMov("MovNomConcep", "id", clavesDelete, null, null, valoresReestablecer, incluirEliminadoDiferenteTipoPantalla100, dbContext);
                    if (val == -1)
                    {
                        commit = false;
                    }
                }
                AgreModif = (AgreModif == null ? new List<MovNomConcep>() : AgreModif);
                if (commit && AgreModif.Count > 0)
                {
                    for (int i = 0; i < AgreModif.Count; i++)
                    {
                        mensajeResultado = buscarPeriodoNomina(AgreModif[i].periodosNomina_ID);
                        if (mensajeResultado.error.Equals(""))
                        {
                            PeriodosNomina per = new PeriodosNomina();
                            per = (PeriodosNomina)mensajeResultado.resultado;

                            AgreModif[i].periodosNomina = per;

                        }
                        else
                        {
                            exito = false;
                            break;
                        }

                    }
                    if (exito)
                    {
                        AgreModif = generaMovsPeriodos2Meses(AgreModif);
                        AgreModif = proporcionarParametrosMovimiento(AgreModif);

                        for (int i = 0; i < AgreModif.Count; i++)
                        {
                            AgreModif[i].periodosNomina = null;
                            AgreModif[i].plazasPorEmpleado = null;
                            for (int j = 0; j < AgreModif[i].movNomConceParam.Count; j++)
                            {
                                AgreModif[i].movNomConceParam[j].paraConcepDeNom = null;
                                AgreModif[i].movNomConceParam[j].movNomConcep = null;
                            }
                            if (AgreModif[i].id == 0)
                            {
                                getSession().Set<MovNomConcep>().Add(AgreModif[i]);
                                getSession().SaveChanges();
                            }
                            else
                            {
                                for (int x = 0; x < AgreModif[i].movNomConceParam.Count; x++)
                                {
                                    getSession().Set<MovNomConceParam>().AddOrUpdate(AgreModif[i].movNomConceParam[x]);
                                    getSession().SaveChanges();
                                }
                                getSession().Set<MovNomConcep>().AddOrUpdate(AgreModif[i]);
                                getSession().SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        commit = false;
                    }
                }
                if (commit)
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
                //mensajeResultado = buscarPeriodoNomina();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteMovimientosNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                Object[] valores = new Object[camposConsulta.Length];
                double suma;
                for (int i = 0; i < AgreModif.Count; i++)
                {
                    /*Solo aplica para periodo semanales y se suman las cantidades que fueron partidas
                    para reflejar en la lista el valor original del parametro con el que se intento persistir*/
                    if (AgreModif[i].numMovParticion == 2)
                    {
                        List<MovNomConceParam> movParamPrin = AgreModif[i - 1].movNomConceParam;
                        List<MovNomConceParam> movParamSec = AgreModif[i].movNomConceParam;
                        if (movParamPrin != null)
                        {
                            for (int j = 0; j < movParamPrin.Count; j++)
                            {
                                suma = Convert.ToDouble(movParamPrin[j].valor) + Convert.ToDouble(movParamSec[j].valor);
                                movParamPrin[j].valor = agregarMascaraValorNumerico(suma, movParamPrin[j].paraConcepDeNom.mascara.Replace("#", "0"));
                            }
                        }
                        AgreModif.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        valores[0] = AgreModif[i].razonesSociales;
                        valores[1] = AgreModif[i].empleados;
                        valores[2] = AgreModif[i].tipoNomina;
                        valores[3] = AgreModif[i].periodosNomina;
                        valores[4] = AgreModif[i].tipoCorrida;
                        valores[5] = AgreModif[i].concepNomDefi;
                        valores[6] = AgreModif[i].numero;
                        valores[7] = AgreModif[i].ejercicio;
                        valores[8] = AgreModif[i].mes;
                        valores[9] = AgreModif[i].uso;
                        List<CamposWhere> camposwhere = new List<CamposWhere>();
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".razonesSociales.id"), AgreModif[i].razonesSociales.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".empleado.id"), AgreModif[i].empleados.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".tipoNomina.id"), AgreModif[i].tipoNomina.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".periodosNomina.id"), AgreModif[i].periodosNomina.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".tipoCorrida.id"), AgreModif[i].tipoCorrida.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".concepNomDefi.id"), AgreModif[i].concepNomDefi.id, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".numero"), AgreModif[i].numero, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".ejercicio"), AgreModif[i].ejercicio, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".mes"), AgreModif[i].mes, OperadorComparacion.IGUAL, OperadorLogico.AND));
                        camposwhere.Add(new CamposWhere(String.Concat(typeof(MovNomConcep).Name, ".uso"), AgreModif[i].uso, OperadorComparacion.IGUAL, OperadorLogico.AND));


                        //mensajeResultado = existeClave("MovNomConcep", camposwhere, new Conexion(uuidCxn));
                        MovNomConcep existe = (MovNomConcep)mensajeResultado.resultado;
                        //MovNomConcep existe = (MovNomConcep)existeClave("MovNomConcep", camposConsulta, valores, null);
                        if (existe != null)
                        {//Indicar que existe en base de datos
                            AgreModif[i].IsEnBD = true;

                            //    AgreModif.get(i).setIsEnBD(true);
                            //Object[] control = AgreModif[i].;
                            //    //control[0] = true;//Para indicar que existe en la base de datos
                            //    //control[1] = existe.getId();//para respaldar el ID y poder recuperar el objeto original
                            //    //AgreModif.get(i).setIsEnBdAndBackupID(control);
                        }
                    }
                }
                onList = AgreModif;
                try
                {

                    mensajeResultado.resultado = onList;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();

                }
                catch (Exception exs)
                {
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("saveDeleteMovNomina()1_Error: ").Append(exs));
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = exs.GetBaseException().ToString();
                    mensajeResultado.resultado = null;
                    getSession().Database.CurrentTransaction.Rollback();

                }
            }
            return mensajeResultado;
        }

        public Mensaje buscarPeriodoNomina(decimal? idper)
        {
            PeriodosNomina per = new PeriodosNomina();

            try
            {
                inicializaVariableMensaje();


                per = (from p in getSession().Set<PeriodosNomina>()
                       where p.id == idper
                       select p).SingleOrDefault();
                mensajeResultado.resultado = per;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;


            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPeriodosNominaPorID()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }


            return mensajeResultado;
        }

        public Mensaje getIsMovPeriodoNomina(decimal idPeriodo, decimal IdtipoNomina, DBContextAdapter dbContext)
        {
            PeriodosNomina per = new PeriodosNomina();

            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var datos = (from m in getSession().Set<MovNomConcep>()
                             where m.periodosNomina_ID == idPeriodo && m.tipoNomina_ID==IdtipoNomina
                             select m).Count();
                mensajeResultado.resultado = datos;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;


            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getIsMovPeriodoNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }


            return mensajeResultado;
        }

        private List<decimal?> obtenCfdiEmpleadoPlaza(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial,  DBContextAdapter dbContext) 
        {
            List<decimal?> listPlazaXEmpleado = new List<decimal?>();
            inicializaVariableMensaje();
            setSession(dbContext.context);
            //getSession().Database.BeginTransaction();
            List<CFDIEmpleado> cfdiEmp = new List<CFDIEmpleado>();

            var query1 = (from cfd in getSession().Set<CFDIEmpleado>()
                          join cfdi in getSession().Set<CFDIRecibo>() on cfd.cfdiRecibo_ID equals cfdi.id into cfdcfdi
                          from cfdi in cfdcfdi.DefaultIfEmpty()

                          select new
                          {   cfd,
                              cfdi,
                              cfd.plazaPorEmpleadoMov_ID
                          });

            try
            {

                if (idRazonSocial != null)
                {
                    query1 = (from subquery in query1
                              where subquery.cfd.razonesSociales_ID == idRazonSocial
                              select subquery);
                }

                if (idTipoCorrida != null)
                {
                    query1 = (from subquery in query1
                              where subquery.cfd.tipoCorrida_ID == idTipoCorrida 
                                                                                
                              select subquery);
                }
                if (idTipoNomina != null)
                {
                    query1 = (from subquery in query1
                              where subquery.cfd.tipoNomina_ID == idTipoNomina 
                                                                               
                              select subquery);
                }

                if (idPeriodoNomina != null)
                {
                    query1 = (from subquery in query1
                              where subquery.cfd.periodosNomina_ID == idPeriodoNomina

                              select subquery);
                }


                
                 if (idEmplIni != null && idEmplFin != null)
                {
                    query1 = (from subquery in query1
                              where ((subquery.cfd.plazasPorEmpleadosMov.plazasPorEmpleado.empleados_ID >= idEmplIni) &&
                             (subquery.cfd.plazasPorEmpleadosMov.plazasPorEmpleado.empleados_ID <= idEmplFin))
                             select subquery);

                }
                else if (idEmplIni != null)
                {
                    query1 = (from subquery in query1
                              where (subquery.cfd.plazasPorEmpleadosMov.plazasPorEmpleado.empleados_ID >= idEmplIni)
                             select subquery);
                }
                else if (idEmplFin != null)
                {
                    query1 = (from subquery in query1
                              where (subquery.cfd.plazasPorEmpleadosMov.plazasPorEmpleado.empleados_ID <= idEmplFin)
                             select subquery);
                }
                 

               

                query1 = (from subquery in query1
                          where subquery.cfdi.statusTimbrado == StatusTimbrado.TIMBRADO
                          select subquery);

                var query3 = (from subquery in query1
                              select subquery.cfd);

                inicializaVariableMensaje();
                cfdiEmp = query3.ToList<CFDIEmpleado>();

                //listPlazaXEmpleado = query3.ToList<decimal>();
               

                if (cfdiEmp != null)
                {                   
                    if (cfdiEmp.Count() > 0)
                    {
                        int i = 0;
                        while (i < cfdiEmp.Count)
                        {
                           listPlazaXEmpleado.Add(cfdiEmp[i].plazaPorEmpleadoMov_ID);
                           i++;

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenCfdiEmpleadoPlaza()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }

            return  listPlazaXEmpleado;
        } 
        
        private List<object> buscaMovimientosNominaFi(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, DBContextAdapter dbContext)
        {

            List<decimal?> listPlazaXEmpleado = obtenCfdiEmpleadoPlaza(idTipoCorrida, idTipoNomina, idPeriodoNomina, idEmplIni, idEmplFin, idRazonSocial, dbContext);
            List<object> listMovNomConcep = new List<object>();

            inicializaVariableMensaje();
            setSession(dbContext.context);
            List<MovNomConcep> movNomConceptos = new List<MovNomConcep>();
            var query = (                       
                         from mov in getSession().Set<MovNomConcep>()
                         join cnc in getSession().Set<ConcepNomDefi>() on mov.concepNomDefi_ID equals cnc.id into movcnc
                         from cnc in movcnc.DefaultIfEmpty()
                         join tipcorr in getSession().Set<TipoCorrida>() on mov.tipoCorrida_ID equals tipcorr.id into movtipcorr
                         from tipcorr in movtipcorr.DefaultIfEmpty()
                         join per in getSession().Set<PeriodosNomina>() on mov.periodosNomina_ID equals per.id into movper
                         from per in movper.DefaultIfEmpty()
                         join tipNom in getSession().Set<TipoNomina>() on mov.tipoNomina_ID equals tipNom.id into movtipNom
                         from tipNom in movtipNom.DefaultIfEmpty()
                         join x9 in getSession().Set<Empleados>() on mov.empleado_ID equals x9.id into movx9
                         from x9 in movx9.DefaultIfEmpty()
                         join x10 in getSession().Set<PlazasPorEmpleado>() on mov.plazasPorEmpleado_ID equals x10.id into movx10
                         from x10 in movx10.DefaultIfEmpty()
                         join x11 in getSession().Set<RazonesSociales>() on mov.razonesSociales_ID equals x11.id into movx11
                         from x11 in movx11.DefaultIfEmpty()
                         
                         select new
                         {
                             mov,
                             cnc,
                             tipcorr,
                             per,
                             tipNom,
                             x9,
                             x10,
                             x11
                         });

            


            //int i;
           
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
            mensajeResultado.resultado = movNomConceptos;
            try
            {            
                
                    if (idRazonSocial != null)
                    {
                        query = (from subquery in query
                                 where subquery.x11.id == idRazonSocial
                                 select subquery);
                    }

                     if (idTipoCorrida != null){
                        query = (from subquery in query
                                 where subquery.tipcorr.id == idTipoCorrida &&
                                 subquery.per.tipoCorrida.id == idTipoCorrida
                                 select subquery);
                    }

                     if (idPeriodoNomina != null) {
                        query = (from subquery in query
                                 where subquery.per.id  == idPeriodoNomina
                                 
                                 select subquery);
                    }
                     
                    

                if (idEmplIni != null && idEmplFin != null)
                {
                    query = (from subquery in query
                             where ((subquery.x9.id >= idEmplIni) &&
                             (subquery.x9.id <= idEmplFin))
                             select subquery);

                }
                else if (idEmplIni != null)
                {
                    query = (from subquery in query
                             where (subquery.x9.id >= idEmplIni)
                             select subquery);
                }
                else if (idEmplFin != null)
                {
                    query = (from subquery in query
                             where (subquery.x9.id <= idEmplFin)
                             select subquery);
                }  



                  if (listPlazaXEmpleado != null) 
                   {
                      if (listPlazaXEmpleado.Count() > 0)
                      {
                        query = (from subquery in query

                                 where (!listPlazaXEmpleado.Contains(subquery.x10.id))
                                 select subquery);

                      }
                   }

                var query2 = (from subquery in query
                                  select subquery.mov);
                inicializaVariableMensaje();
               // setSession(dbContext.context);
               // getSession().Database.BeginTransaction();
                movNomConceptos = query2.ToList<MovNomConcep>();
               // getSession().Database.CurrentTransaction.Commit();
                if (movNomConceptos!= null) {
                    int d = 0;
                    while (d < movNomConceptos.Count)
                    {
                        listMovNomConcep.Add(movNomConceptos[d].id);
                        d++;

                    }

                }
                
                

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaMovimientosNominaFi()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return listMovNomConcep;
        }

        private List<object> buscarCalculoUnicades(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, DBContextAdapter dbContext)
        {
            List<CalculoUnidades> listCalculoUnidasdes = new List<CalculoUnidades>();
            List<object> ListCal = new List<object>();
            inicializaVariableMensaje();
            setSession(dbContext.context);

            var query1 = (from cu in getSession().Set<CalculoUnidades>()
                          select new
                          {
                              cu
                          });

            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
            mensajeResultado.resultado = listCalculoUnidasdes;

            try {

                if (idRazonSocial != null)
                {
                    query1 = (from subquery in query1
                              where subquery.cu.razonesSociales_ID == idRazonSocial
                              select subquery);
                }  
                
                if (idTipoCorrida != null)
                {
                    query1 = (from subquery in query1
                              where subquery.cu.tipoCorrida_ID == idTipoCorrida

                              select subquery);
                } 
                if (idTipoNomina != null)
                {
                    query1 = (from subquery in query1
                              where subquery.cu.tipoNomina_ID == idTipoNomina

                              select subquery);
                }

                if (idPeriodoNomina != null)
                {
                    query1 = (from subquery in query1
                              where subquery.cu.periodosNomina_ID == idPeriodoNomina

                              select subquery);
                }

                if (idEmplIni != null && idEmplFin != null)
                {
                    query1 = (from subquery in query1
                              where ((subquery.cu.plazasPorEmpleado.empleados_ID >= idEmplIni) &&
                             (subquery.cu.plazasPorEmpleado.empleados_ID <= idEmplFin))
                             select subquery);

                }
                else if (idEmplIni != null)
                {
                    query1 = (from subquery in query1
                              where (subquery.cu.plazasPorEmpleado.empleados_ID >= idEmplIni)
                             select subquery);
                }
                else if (idEmplFin != null)
                {
                    query1 = (from subquery in query1
                              where (subquery.cu.plazasPorEmpleado.empleados_ID <= idEmplFin)
                             select subquery);
                }



                
                query1 = (from subquery in query1
                          where subquery.cu.uso == 0
                          select subquery);

                var query3 = (from subquery in query1
                              select subquery.cu);

                inicializaVariableMensaje();
                
                listCalculoUnidasdes = query3.ToList<CalculoUnidades>();


                if (listCalculoUnidasdes != null)
                {
                    int d = 0;
                    while (d < listCalculoUnidasdes.Count)
                    {
                        ListCal.Add(listCalculoUnidasdes[d].id);
                        d++;

                    }

                }

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscarCalculoUnicades()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }


            return ListCal;
        }

        public Mensaje eliminarMovNomina(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal,
        decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, DBContextAdapter dbContext,  DBContextAdapter dbContextMaster)
        {
            try
            {
                inicializaVariableMensaje();
                
                List<object> listMovNomConcep = new List<object>();
                List<CFDIEmpleado> listcfdiEmp = new List<CFDIEmpleado>();
                List<object> listCalculoUnidasdes = new List<object>();

                listMovNomConcep = buscaMovimientosNominaFi(idTipoCorrida, idTipoNomina, idPeriodoNomina, idEmplIni, idEmplFin, idRazonSocial, dbContext);
                inicializaVariableMensaje();
                mensajeResultado = generardatos.buscarTimbresId(idTipoCorrida, idTipoNomina, idPeriodoNomina, idRegPatronal,
                idCenCosto, idDepartamento, idEmplIni, idEmplFin, idRazonSocial, 1, dbContext, dbContextMaster);
                if (mensajeResultado.resultado != null) {
                    List<CFDIEmpleado> cfdiEmp = (List<CFDIEmpleado>)mensajeResultado.resultado;
                    listcfdiEmp = cfdiEmp;

                }
                else {
                    listcfdiEmp = null;
                }

                listCalculoUnidasdes = buscarCalculoUnicades(idTipoCorrida, idTipoNomina, idPeriodoNomina, idEmplIni, idEmplFin, idRazonSocial, dbContext);

                inicializaVariableMensaje();

                if ( listMovNomConcep.Count > 0 )
                {
                    mensajeResultado = eliminaListaMovimientos("id", listMovNomConcep.ToArray(), listcfdiEmp, listCalculoUnidasdes.ToArray(), null, false, dbContext);
                }
                else
                {
                    mensajeResultado.noError = 5;
                    mensajeResultado.error = "No existen movimientos a eliminar";
                    mensajeResultado.resultado = null;
                }


                    


            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminarMovNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;

            }


            return mensajeResultado;
        }


    }
}  