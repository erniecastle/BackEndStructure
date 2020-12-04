/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase FiniquitosLiquidacionesDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Entity.entidad.cfdi;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;
using Exitosw.Payroll.Core.servicios.extras;

namespace Exitosw.Payroll.Core.modelo
{
    public class FiniquitosLiquidacionesDAO : GenericRepository<FiniquitosLiquida>, FiniquitosLiquidacionesDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<FiniquitosLiquida> listFiniquitosLiquidaciones = new List<FiniquitosLiquida>();

        public Mensaje agregar(FiniquitosLiquida entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<FiniquitosLiquida>().Add(entity);
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
        public Mensaje actualizar(FiniquitosLiquida entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<FiniquitosLiquida>().AddOrUpdate(entity);
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
        public Mensaje eliminar(FiniquitosLiquida entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<FiniquitosLiquida>().Attach(entity);
                getSession().Set<FiniquitosLiquida>().Remove(entity);
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

        public Mensaje guardarFiniquitos(FiniquitosLiquida entity,
            List<FiniqLiquidPlazas> agregaModFiniqLiquidPlazas, object[] eliminadosFiniqLiquidPlazas,
            List<FiniqLiquidCncNom> agregaModFiniqLiquidCncNom, object[] eliminadosFiniqLiquidCncNom,
            DBContextAdapter dbContext, DBContextAdapter dbContextMaster)
        {
            DbContext dbContextSimple;
            DbContextTransaction transacion;
            //List<PlazasPorEmpleadosMov> getPlazas = new List<PlazasPorEmpleadosMov>();
            using (dbContextSimple = dbContext.context)
            {
                using (transacion = dbContextSimple.Database.BeginTransaction())
                {
                    try
                    {
                        inicializaVariableMensaje();
                        dbContextSimple.Set<FiniquitosLiquida>().AddOrUpdate(entity);
                        dbContextSimple.SaveChanges();

                        int i;
                        int totalPlazas = agregaModFiniqLiquidPlazas.Count();

                        //FiniquitosLiquidaciones Plazas
                        for (i = 0; i < totalPlazas; i++)
                        {
                            agregaModFiniqLiquidPlazas[i].finiquitosLiquida_ID = entity.id;

                            //Si incluir para baja modificar fecha de plaza (FechaFinal)
                            decimal? idPlazaEmpleado = agregaModFiniqLiquidPlazas[i].plazasPorEmpleado_ID;
                            PlazasPorEmpleado plazaRecord =
                            (from plazasPorEmpleado in dbContextSimple.Set<PlazasPorEmpleado>()
                             where plazasPorEmpleado.id == idPlazaEmpleado
                             select plazasPorEmpleado).FirstOrDefault();

                            if (agregaModFiniqLiquidPlazas[i].incluir)
                            {
                                DateTime getFecha = (DateTime)plazaRecord.fechaFinal;
                                DateTime? fechaFinalDePlaza = new DateTime(getFecha.Year, getFecha.Month, getFecha.Day, 0, 0, 0);
                                plazaRecord.fechaFinal = entity.fechaBaja;
                                dbContextSimple.Set<PlazasPorEmpleado>().AddOrUpdate(plazaRecord);
                                dbContextSimple.SaveChanges();
                                if (agregaModFiniqLiquidPlazas[i].fechaFinal == null)
                                {
                                    agregaModFiniqLiquidPlazas[i].fechaFinal = fechaFinalDePlaza;
                                }
                            }
                            else
                            {
                                if (agregaModFiniqLiquidPlazas[i].fechaFinal != null)
                                {
                                    plazaRecord.fechaFinal = agregaModFiniqLiquidPlazas[i].fechaFinal;
                                    dbContextSimple.Set<PlazasPorEmpleado>().AddOrUpdate(plazaRecord);
                                    dbContextSimple.SaveChanges();
                                    agregaModFiniqLiquidPlazas[i].fechaFinal = null;
                                }

                            }
                            dbContextSimple.Set<FiniqLiquidPlazas>().AddOrUpdate(agregaModFiniqLiquidPlazas[i]);
                        }

                        dbContextSimple.SaveChanges();


                        #region Verifica si exiten plazas activas y revierte fecha baja de Ingreso o modifica
                        //Modificar el Ingreso (Verificar que no tenga ninguna plaza Activa)
                        DateTime fechaActual = getFechaDelSistema();
                        var lisPlazasActivas = (from o in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                where (from m in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                       where m.plazasPorEmpleado.empleados.id == entity.empleados_ID &&
                                                           m.plazasPorEmpleado.razonesSociales.id == entity.razonesSociales_ID &&
                                                           fechaActual >= m.fechaInicial && fechaActual <= m.plazasPorEmpleado.fechaFinal
                                                       group new { m.plazasPorEmpleado, m } by new
                                                       {
                                                           m.plazasPorEmpleado.referencia
                                                       } into g
                                                       select new
                                                       {
                                                           Column1 = g.Max(p => p.m.id)
                                                       }).Contains(new { Column1 = o.id })
                                                orderby o.plazasPorEmpleado.fechaFinal descending
                                                select new
                                                {
                                                    o,
                                                    o.id,
                                                    o.plazasPorEmpleado.referencia,
                                                    o.plazasPorEmpleado.fechaFinal,
                                                    o.plazasPorEmpleado.registroPatronal_ID,
                                                    //claveEmp = o.plazasPorEmpleado.empleados.clave,
                                                    //claveTipoNomina = o.tipoNomina.clave,
                                                    //claveRazonSocial = o.plazasPorEmpleado.razonesSociales.clave,
                                                    //claveRegPatronal = o.plazasPorEmpleado.registroPatronal.clave,
                                                    //tipoContrato = o.tipoContrato.clave,
                                                    //fechaImss = o.fechaIMSS
                                                }).ToList();


                        //Sino hay plazas activas afectar (FechaDeBaja) maxima de las plazas involucradas
                        decimal? idEmpleado = entity.empleados_ID;
                        IngresosBajas ingresoRecord =
                        (from ingresosBajas in dbContextSimple.Set<IngresosBajas>()
                         where ingresosBajas.empleados_ID == idEmpleado
                         && ingresosBajas.razonesSociales_ID == entity.razonesSociales_ID
                         //&& ingresosBajas.fechaBaja >= fechaActual
                         select ingresosBajas).FirstOrDefault();

                        if (ingresoRecord != null)
                        {
                            if (lisPlazasActivas.Count() == 0)
                            {
                                ingresoRecord.fechaBaja = entity.fechaBaja;
                                dbContextSimple.Set<IngresosBajas>().AddOrUpdate(ingresoRecord);
                                dbContextSimple.SaveChanges();
                            }
                            else
                            {
                                ingresoRecord.fechaBaja = lisPlazasActivas[0].fechaFinal;
                                dbContextSimple.Set<IngresosBajas>().AddOrUpdate(ingresoRecord);
                                dbContextSimple.SaveChanges();
                            }
                        }

                        #endregion


                        #region Calcula el SDI de las plazas involucradas
                        Dictionary<object, object> paramExtra = new Dictionary<object, object>();
                       // paramExtra.Add("peticionDesdeFiniquitos", true);
                        paramExtra.Add("idRegistroPatronal", ingresoRecord.registroPatronal_ID);
                        paramExtra.Add("idFiniquito", entity.id);

                        Mensaje mensajeSalario = new CalculaNominaDAO().calculoSDI(ingresoRecord.empleados.clave,
                                ingresoRecord.razonesSociales.clave,
                               (DateTime)entity.fechaBaja, paramExtra, dbContext, dbContextMaster);

                        if (mensajeSalario.noError == 0)
                        {
                            SalariosIntegrados salary = (SalariosIntegrados)mensajeSalario.resultado;
                            dbContextSimple.Set<SalariosIntegrados>().AddOrUpdate(salary);
                            dbContextSimple.SaveChanges();

                        }

                        #endregion


                        #region  Recalcula el SDI de cada una de las plazas activas a ese momento
                        //var valSDI = new Dictionary<string, object>();
                        //foreach (var plazaActiva in lisPlazasActivas)
                        //{
                        //    DbContext dbcontexSala = new DBContextSimple(EntityFrameworkCxn.createDbConnection(dbContext.connectionDB), false);
                        //    DBContextAdapter dbConSalary = new DBContextAdapter(dbcontexSala, dbContext.connectionDB);
                        //    List<PlazasPorEmpleadosMov> listaPlazasPorEmpleadosMov = new List<PlazasPorEmpleadosMov>();
                        //    listaPlazasPorEmpleadosMov.Add(plazaActiva.o);

                        //    //Parameters of SDI
                        //    valSDI.Add("claveEmpIni", plazaActiva.o.plazasPorEmpleado.empleados.clave);
                        //    valSDI.Add("claveEmpFin", plazaActiva.o.plazasPorEmpleado.empleados.clave);
                        //    valSDI.Add("claveTipoNomina", plazaActiva.o.tipoNomina.clave);
                        //    valSDI.Add("claveTipoCorrida", "PER");
                        //    valSDI.Add("claveRazonSocial", plazaActiva.o.plazasPorEmpleado.razonesSociales.clave);
                        //    valSDI.Add("claveRegPatronal", plazaActiva.o.plazasPorEmpleado.registroPatronal.clave);
                        //    valSDI.Add("tipoContrato", plazaActiva.o.tipoContrato.clave);
                        //    valSDI.Add("controlador", "RazonesSociales" + plazaActiva.o.plazasPorEmpleado.razonesSociales.clave);
                        //    valSDI.Add("soloCalculo", true);
                        //    ParametrosExtra parametrosExtra = new ParametrosExtra();
                        //    parametrosExtra.fechaCalculoSDI = entity.fechaBaja;
                        //    List<Object> valoresExtras = new List<Object>();
                        //    valoresExtras.Add(listaPlazasPorEmpleadosMov);
                        //    parametrosExtra.valoresExtras = valoresExtras;
                        //    valSDI.Add("parametrosExtra", parametrosExtra);

                        //    Mensaje mensajeSalario = new CalculaNominaDAO().calculaSalarioDiarioIntegerado((string)valSDI["claveEmpIni"],
                        //            (string)valSDI["claveEmpFin"], (string)valSDI["claveTipoNomina"], (string)valSDI["claveTipoCorrida"],
                        //         null, "", "", "", (string)valSDI["claveRazonSocial"],
                        //       (string)valSDI["claveRegPatronal"], "", "", "", null, (string)valSDI["tipoContrato"],
                        //         null, (string)valSDI["controlador"], 0, parametrosExtra, (bool)valSDI["soloCalculo"],
                        //          false, dbConSalary, dbContextMaster);

                        //    valSDI.Clear();

                        //    if (mensajeSalario.noError == 0)
                        //    {
                        //        SalariosIntegrados salary = (SalariosIntegrados)mensajeSalario.resultado;
                        //        dbContextSimple.Set<SalariosIntegrados>().AddOrUpdate(salary);
                        //        dbContextSimple.SaveChanges();

                        //    }

                        //}

                        #endregion

                        foreach (object addPar in eliminadosFiniqLiquidPlazas)
                        {
                            decimal idGet = Convert.ToDecimal(addPar);
                            var getObj = new FiniqLiquidPlazas { id = idGet };
                            dbContextSimple.Set<FiniqLiquidPlazas>().Attach(getObj);
                            dbContextSimple.Entry(getObj).State = EntityState.Deleted;
                        }
                        dbContextSimple.SaveChanges();

                        //Finiquitos Liquidaciones Conceptos
                        for (i = 0; i < agregaModFiniqLiquidCncNom.Count(); i++)
                        {
                            agregaModFiniqLiquidCncNom[i].finiquitosLiquid_ID = entity.id;
                            dbContextSimple.Set<FiniqLiquidCncNom>().AddOrUpdate(agregaModFiniqLiquidCncNom[i]);
                        }

                        dbContextSimple.SaveChanges();

                        foreach (object addTipCorr in eliminadosFiniqLiquidCncNom)
                        {
                            decimal idGet = Convert.ToDecimal(addTipCorr);
                            var getObj = new FiniqLiquidCncNom { id = idGet };
                            dbContextSimple.Set<FiniqLiquidCncNom>().Attach(getObj);
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
                        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("guardarFiniquitos()1_Error: ").Append(ex));
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = null;
                        transacion.Rollback();
                    }
                }

            }

            return mensajeResultado;
        }

        public Mensaje eliminarFiniquito(FiniquitosLiquida entity, DBContextAdapter dbContext)
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

                        //Get FiniqLiquidPlazas
                        var getFiniqLiquidPlazas = (from o in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                    join ple in dbContextSimple.Set<PlazasPorEmpleado>()
                                                    on o.plazasPorEmpleado_ID equals ple.id
                                                    join finiPl in dbContextSimple.Set<FiniqLiquidPlazas>()
                                                    on ple.id equals finiPl.plazasPorEmpleado_ID
                                                    where (from m in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                           join plazaEmpleado in dbContextSimple.Set<PlazasPorEmpleado>()
                                                           on m.plazasPorEmpleado_ID equals plazaEmpleado.id
                                                           join finiqPlazas in dbContextSimple.Set<FiniqLiquidPlazas>()
                                                           on plazaEmpleado.id equals finiqPlazas.plazasPorEmpleado_ID
                                                           where finiqPlazas.finiquitosLiquida_ID == entity.id
                                                           group new { m.plazasPorEmpleado, m } by new
                                                           {
                                                               m.plazasPorEmpleado.referencia
                                                           } into g
                                                           select new
                                                           {
                                                               Column1 = g.Max(p => p.m.id)
                                                           }).Contains(new { Column1 = o.id })

                                                    orderby o.plazasPorEmpleado.referencia
                                                    select new
                                                    {
                                                        id = finiPl.id,
                                                        incluir = finiPl.incluir,
                                                        finiquitosLiquida_ID = finiPl.finiquitosLiquida_ID,
                                                        plazasPorEmpleado_ID = finiPl.plazasPorEmpleado_ID,
                                                        idPlazaEmpleado = finiPl.plazasPorEmpleado_ID,
                                                        fechaFinal = finiPl.fechaFinal,
                                                        referencia = o.plazasPorEmpleado.referencia,
                                                        descripcionPuesto = o.puestos.descripcion,
                                                        horas = o.horas,
                                                        importe = o.importe,
                                                        fechaInicial = o.fechaInicial
                                                    }).ToList();


                        //Revert all dates of plazas
                        foreach (var finiLiquid in getFiniqLiquidPlazas)
                        {
                            decimal? idPlazaEmpleado = finiLiquid.plazasPorEmpleado_ID;
                            PlazasPorEmpleado plazaRecord =
                            (from plazasPorEmpleado in dbContextSimple.Set<PlazasPorEmpleado>()
                             where plazasPorEmpleado.id == idPlazaEmpleado
                             select plazasPorEmpleado).FirstOrDefault();
                            if (finiLiquid.fechaFinal != null)
                            {
                                plazaRecord.fechaFinal = finiLiquid.fechaFinal;
                                dbContextSimple.Set<PlazasPorEmpleado>().AddOrUpdate(plazaRecord);
                                dbContextSimple.SaveChanges();
                            }
                        }

                        #region Verifica si exiten plazas activas y revierte fecha baja de Ingreso o modifica
                        //Modificar el Ingreso (Verificar que no tenga ninguna plaza Activa)
                        DateTime fechaActual = getFechaDelSistema();
                        var lisPlazasActivas = (from o in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                where (from m in dbContextSimple.Set<PlazasPorEmpleadosMov>()
                                                       where m.plazasPorEmpleado.empleados.id == entity.empleados_ID &&
                                                           m.plazasPorEmpleado.razonesSociales.id == entity.razonesSociales_ID &&
                                                           fechaActual >= m.fechaInicial && fechaActual <= m.plazasPorEmpleado.fechaFinal
                                                       group new { m.plazasPorEmpleado, m } by new
                                                       {
                                                           m.plazasPorEmpleado.referencia
                                                       } into g
                                                       select new
                                                       {
                                                           Column1 = g.Max(p => p.m.id)
                                                       }).Contains(new { Column1 = o.id })
                                                orderby o.plazasPorEmpleado.fechaFinal descending
                                                select new
                                                {
                                                    o.id,
                                                    o.plazasPorEmpleado.referencia,
                                                    o.plazasPorEmpleado.fechaFinal

                                                }).ToList();



                        decimal? idEmpleado = entity.empleados_ID;
                        IngresosBajas ingresoRecord =
                        (from ingresosBajas in dbContextSimple.Set<IngresosBajas>()
                         where ingresosBajas.empleados_ID == idEmpleado
                         && ingresosBajas.razonesSociales_ID == entity.razonesSociales_ID
                         //&& ingresosBajas.fechaBaja >= fechaActual
                         select ingresosBajas).FirstOrDefault();

                        if (ingresoRecord != null)
                        {
                            //Obtiene la fecha mas alta de las plazas a reactivar
                            ingresoRecord.fechaBaja = lisPlazasActivas[0].fechaFinal;
                            dbContextSimple.Set<IngresosBajas>().AddOrUpdate(ingresoRecord);
                            dbContextSimple.SaveChanges();
                        }

                        #endregion

                        //Delete Salarios Integrados de Finiquitos
                        dbContextSimple.Set<SalariosIntegrados>().
                            RemoveRange(dbContextSimple.Set<SalariosIntegrados>().Where(x => x.finiquitosLiquidaciones_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Delete FiniqLiquidPlazas
                        dbContextSimple.Set<FiniqLiquidPlazas>().
                            RemoveRange(dbContextSimple.Set<FiniqLiquidPlazas>().Where(x => x.finiquitosLiquida_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Delete FiniqLiquidCncNom
                        dbContextSimple.Set<FiniqLiquidCncNom>().
                            RemoveRange(dbContextSimple.Set<FiniqLiquidCncNom>().Where(x => x.finiquitosLiquid_ID == entity.id));
                        dbContextSimple.SaveChanges();

                        //Validaciones
                        //var canSave = 1;
                        //bool exiteIngresos = dbContextSimple.Set<IngresosBajas>().Where(x => x.empleados_ID == entity.id).Select(c => c.id).Any();
                        //if (exiteIngresos)
                        //{
                        //    canSave = 2;,canSave = 3; etc
                        //}

                        //if (canSave == 1)
                        //{

                        //Delete Empleado
                        dbContextSimple.Set<FiniquitosLiquida>().Attach(entity);
                        dbContextSimple.Set<FiniquitosLiquida>().Remove(entity);
                        dbContextSimple.SaveChanges();
                        //  }

                        if (mensajeResultado.noError == 0)
                        {
                            //if (canSave == 1)
                            //{
                            mensajeResultado.resultado = true;
                            //}
                            //else
                            //{
                            //    mensajeResultado.resultado = canSave;
                            //}
                            mensajeResultado.noError = 0;
                        }
                        transacion.Commit();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminarFiniquito()1_Error: ").Append(ex));
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = null;
                        transacion.Rollback();
                    }
                }
            }

            return mensajeResultado;
        }

        public Mensaje getPorIdFiniquitosYComplementos(decimal? idFiniquitos, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                object[] complementos = new object[3];

                var getFiniquito = (from f in dbContext.context.Set<FiniquitosLiquida>()
                                    where f.id == idFiniquitos
                                    select new
                                    {
                                        id = f.id,
                                        bajaPorRiesgo = f.bajaPorRiesgo,
                                        calculado = f.calculado,
                                        causaBaja = f.causaBaja,
                                        referencia = f.referencia,
                                        contImpreso = f.contImpreso,
                                        descripcionBaja = f.descripcionBaja,
                                        fechaBaja = f.fechaBaja,
                                        fechaCalculo = f.fechaCalculo,
                                        modoBaja = f.modoBaja,
                                        observaciones = f.observaciones,
                                        status = f.status,
                                        tipoBaja = f.tipoBaja,
                                        empleados_ID = f.empleados_ID,
                                        claveEmpleado = f.empleados.clave,
                                        finiquitosComplementario_ID = f.finiquitosComplementario_ID,
                                        razonesSociales_ID = f.razonesSociales_ID,
                                    }).SingleOrDefault();

                complementos[0] = getFiniquito;

                if (getFiniquito != null)
                {

                    var getFiniqLiquidPlazas = (from o in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                                join ple in dbContext.context.Set<PlazasPorEmpleado>()
                                                on o.plazasPorEmpleado_ID equals ple.id
                                                join finiPl in dbContext.context.Set<FiniqLiquidPlazas>()
                                                on ple.id equals finiPl.plazasPorEmpleado_ID
                                                where (from m in dbContext.context.Set<PlazasPorEmpleadosMov>()
                                                       join plazaEmpleado in dbContext.context.Set<PlazasPorEmpleado>()
                                                       on m.plazasPorEmpleado_ID equals plazaEmpleado.id
                                                       join finiqPlazas in dbContext.context.Set<FiniqLiquidPlazas>()
                                                       on plazaEmpleado.id equals finiqPlazas.plazasPorEmpleado_ID
                                                       where finiqPlazas.finiquitosLiquida_ID == getFiniquito.id
                                                       group new { m.plazasPorEmpleado, m } by new
                                                       {
                                                           m.plazasPorEmpleado.referencia
                                                       } into g
                                                       select new
                                                       {
                                                           Column1 = g.Max(p => p.m.id)
                                                       }).Contains(new { Column1 = o.id })

                                                orderby o.plazasPorEmpleado.referencia
                                                select new
                                                {
                                                    id = finiPl.id,
                                                    incluir = finiPl.incluir,
                                                    finiquitosLiquida_ID = finiPl.finiquitosLiquida_ID,
                                                    plazasPorEmpleado_ID = finiPl.plazasPorEmpleado_ID,
                                                    idPlazaEmpleado = finiPl.plazasPorEmpleado_ID,
                                                    fechaFinal = finiPl.fechaFinal,
                                                    referencia = o.plazasPorEmpleado.referencia,
                                                    descripcionPuesto = o.puestos.descripcion,
                                                    horas = o.horas,
                                                    importe = o.importe,
                                                    fechaInicial = o.fechaInicial

                                                }).ToList();




                    //var getFiniqLiquidPlazas = (from f in dbContext.context.Set<FiniqLiquidPlazas>()
                    //                            where f.finiquitosLiquida_ID == getFiniquito.id
                    //                            select new
                    //                            {
                    //                                id = f.id,
                    //                                incluir = f.incluir,
                    //                                finiquitosLiquida_ID = f.finiquitosLiquida_ID,
                    //                                plazasPorEmpleado_ID = f.plazasPorEmpleado_ID,
                    //                            }).ToList();

                    complementos[1] = getFiniqLiquidPlazas;

                    var getFiniqLiquidCncNom = (from fo in dbContext.context.Set<FiniqLiquidCncNom>()
                                                where fo.finiquitosLiquid_ID == getFiniquito.id
                                                select new
                                                {
                                                    id = fo.id,
                                                    clave = fo.conceptoPorTipoCorrida.concepNomDefi.clave,
                                                    descripcion = fo.conceptoPorTipoCorrida.concepNomDefi.descripcion,
                                                    cantidad = fo.cantidad,
                                                    importe = fo.importe,
                                                    aplicar = fo.aplicar,
                                                    conceptoPorTipoCorrida_ID = fo.conceptoPorTipoCorrida_ID,
                                                    finiquitosLiquid_ID = fo.finiquitosLiquid_ID
                                                }).ToList();
                    complementos[2] = getFiniqLiquidCncNom;

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

        public Mensaje consultaPorFiltrosFiniquitosLiquida(Dictionary<string, object> campos, string operador, Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listFiniquitosLiquidaciones = new List<FiniquitosLiquida>();
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
                        campo.campo = "FiniquitosLiquida." + item.Key.ToString();
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
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltrosFiniquitos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje consultaPorRangosFiniquitosLiquida(Int64 inicio, Int64 rango, DBContextAdapter dbContext)
        {
            listFiniquitosLiquidaciones = new List<FiniquitosLiquida>();
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

        public Mensaje EliminarFiniquitosLiquidacion(FiniquitosLiquida finiquitosLiquidaciones, List<PlazasPorEmpleadosMov> plazasPorEmpleados, DBContextAdapter dbContext)
        {

            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i < plazasPorEmpleados.Count(); i++)
                {
                    PeriodosNomina periodosNomina = new PeriodosNomina();
                    CFDIEmpleado cfdiEmpleado = new CFDIEmpleado();
                    TipoNomina tiponom = plazasPorEmpleados[i].tipoNomina;
                    DateTime? fecha = finiquitosLiquidaciones.fechaCalculo;
                    periodosNomina = (from p in getSession().Set<PeriodosNomina>()
                                      join tc in getSession().Set<TipoCorrida>() on p.tipoCorrida.id equals tc.id
                                      where p.fechaInicial >= fecha ||
                                      p.fechaFinal >= fecha &&
                                      p.tipoNomina.clave.Equals(tiponom.clave) &&
                                      tc.clave.Equals("FIN")
                                      select p).SingleOrDefault();

                    cfdiEmpleado = (from o in getSession().Set<CFDIEmpleado>()
                                    join cr in getSession().Set<CFDIRecibo>() on o.cfdiRecibo.id equals cr.id
                                    where o.periodosNomina.id == periodosNomina.id &&
                                    o.plazasPorEmpleadosMov.id == plazasPorEmpleados[i].id &&
                                    o.tipoCorrida.clave.Equals("FIN") &&
                                    o.tipoNomina.id == tiponom.id &&
                                    cr.statusTimbrado == 0
                                    select o).SingleOrDefault();
                    if (cfdiEmpleado != null)
                    {
                        mensajeResultado.resultado = "Timbrado";
                        getSession().Database.CurrentTransaction.Rollback();
                    }
                    else if (!periodosNomina.status)
                    {
                        mensajeResultado.resultado = "Periodo";
                        getSession().Database.CurrentTransaction.Rollback();
                    }
                    else
                    {
                        bool exito = eliminarMovFinLiq(plazasPorEmpleados[i], finiquitosLiquidaciones);
                        if (exito)
                        {
                            DateTime? calendar = new DateTime();
                            calendar = plazasPorEmpleados[i].plazasPorEmpleado.fechaFinal;
                            if (calendar.Value.Year < 2100)
                            {
                                calendar = calendar.Value.AddYears(calendar.Value.Year + 100);
                            }
                            //Session session = getSession().getSessionFactory().openSession();
                            plazasPorEmpleados[i].plazasPorEmpleado.fechaFinal = calendar.Value.ToUniversalTime();
                            getSession().Database.BeginTransaction();
                            getSession().Set<PlazasPorEmpleadosMov>().AddOrUpdate(plazasPorEmpleados[i]);
                            getSession().SaveChanges();
                            getSession().Database.CurrentTransaction.Commit();
                            mensajeResultado.resultado = "Eliminado";
                            //session.close();
                            getSession().Database.CurrentTransaction.Commit();

                        }
                        else
                        {
                            getSession().Database.CurrentTransaction.Rollback();
                        }
                    }
                }
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("EliminarFiniquitosLiquidacion()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        private bool eliminarMovFinLiq(PlazasPorEmpleadosMov plazasPorEmpleados, FiniquitosLiquida finiquitosLiquidaciones)
        {
            bool result = false;
            int exito = 0;
            MovNomBaseAfecta movNomBaseAfecta = new MovNomBaseAfecta();
            MovNomConceParam movNomConceParam = new MovNomConceParam();
            try
            {
                List<FiniqLiquidCncNom> finLiqCncNom = new List<FiniqLiquidCncNom>();
                List<MovNomConcep> movNomConcep = new List<MovNomConcep>();
                finLiqCncNom = (from o in getSession().Set<FiniqLiquidCncNom>()
                                join f in getSession().Set<FiniquitosLiquida>() on o.finiquitosLiquida.id equals f.id
                                where f.id == finiquitosLiquidaciones.id
                                select o).ToList();
                for (int i = 0; i < finLiqCncNom.Count(); i++)
                {
                    movNomConcep = (from m in getSession().Set<MovNomConcep>()
                                    where m.finiqLiquidCncNom.id == finLiqCncNom[i].id &&
                                    m.plazasPorEmpleado.id == plazasPorEmpleados.id &&
                                    m.tipoCorrida.clave.Equals("FIN") &&
                                    m.tipoNomina.id == plazasPorEmpleados.tipoNomina.id
                                    select m).ToList();
                }
                for (int i = 0; i < movNomConcep.Count(); i++)
                {
                    exito = getSession().Database.ExecuteSqlCommand("delete m from MovNomBaseAfecta AS m where m.movNomConcep_id >= @@movNomConcep_id", new SqlParameter("@movNomConcep_id", movNomConcep[i].id));
                    exito = getSession().Database.ExecuteSqlCommand("delete m from MovNomConceParam  AS m where m.movNomConcep_id >= @@movNomConcep_id", new SqlParameter("@movNomConcep_id", movNomConcep[i].id));
                    exito = getSession().Database.ExecuteSqlCommand("delete m from" + typeof(CalculoISR).Name + "AS m where m.movNomConcep_id >= @@movNomConcep_id", new SqlParameter("@movNomConcep_id", movNomConcep[i].id));
                    exito = getSession().Database.ExecuteSqlCommand("delete m from" + typeof(CalculoIMSS).Name + "AS m where m.movNomConcep_id >= @@movNomConcep_id", new SqlParameter("@movNomConcep_id", movNomConcep[i].id));
                    exito = getSession().Database.ExecuteSqlCommand("delete m from" + typeof(CalculoIMSSPatron).Name + "AS m where m.movNomConcep_id >= @@movNomConcep_id", new SqlParameter("@movNomConcep_id", movNomConcep[i].id));
                    exito = getSession().Database.ExecuteSqlCommand("delete m from MovNomConcep AS m where m.id >= @@movNomConcepid", new SqlParameter("@movNomConcepid", movNomConcep[i].id));

                }
                IngresosBajas ingReinBajas = new IngresosBajas();
                ingReinBajas = (from m in getSession().Set<IngresosBajas>()
                                    /* where m.finiquitosLiquida.id == finiquitosLiquidaciones.id &&
                                     m.plazasPorEmpleado.id == plazasPorEmpleados.id*/
                                select m).SingleOrDefault();
                if (ingReinBajas != null)
                {
                    // ingReinBajas.finiquitosLiquida = null;
                    DateTime? calendar = new DateTime();
                    calendar = ingReinBajas.fechaBaja;
                    if (calendar.Value.Year < 2100)
                    {
                        calendar = calendar.Value.AddYears(calendar.Value.Year + 100);
                    }
                    ingReinBajas.fechaBaja = calendar.Value.ToUniversalTime();
                    getSession().Set<IngresosBajas>().AddOrUpdate(ingReinBajas);
                    getSession().SaveChanges();
                }
                exito = getSession().Database.ExecuteSqlCommand("delete f from FiniqLiquidCncNom AS f where f.finiquitosLiquid_ID >= @@idFinLiq", new SqlParameter("@idFinLiq", finiquitosLiquidaciones.id));
                exito = getSession().Database.ExecuteSqlCommand("delete f from FiniqLiquidPlazas AS f where f.finiquitosLiquid_ID >= @@idFinLiq", new SqlParameter("@idFinLiq", finiquitosLiquidaciones.id));
                SalariosIntegrados salarioInt = new SalariosIntegrados();
                salarioInt = (from s in getSession().Set<SalariosIntegrados>()
                              where s.finiquitosLiquida.id == finiquitosLiquidaciones.id
                              select s).SingleOrDefault();
                if (salarioInt != null)
                {
                    exito = getSession().Database.ExecuteSqlCommand("delete s  from SalariosIntegradosDet AS s where s.salarioIntegrado_ID >= @@idSalario", new SqlParameter("@idSalario", salarioInt.id));
                }
                exito = getSession().Database.ExecuteSqlCommand("delete s  from SalariosIntegradosDet AS s where s.finiquitosLiquid_ID >= @@idFinLiq", new SqlParameter("@idFinLiq", finiquitosLiquidaciones.id));
                exito = getSession().Database.ExecuteSqlCommand("delete f from FiniquitosLiquidaciones AS f where f.id >= @@idFinLiq", new SqlParameter("@idFinLiq", finiquitosLiquidaciones.id));
                result = true;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminarMovFinLiq()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return result;
        }

        public Mensaje getCancelarFiniquito(object[] eliminadoMovNomConceps, List<PlazasPorEmpleado> listPlazasPorEmpleado, IngresosBajas ingresosReingresosBajas, SalariosIntegrados salariosIntegrado, FiniquitosLiquida finiquitosLiquidaciones, DBContextAdapter dbContext)
        {
            listFiniquitosLiquidaciones = new List<FiniquitosLiquida>();
            int i, contador = 0;
            bool commit = true;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (i = 0; i < listPlazasPorEmpleado.Count(); i++)
                {
                    getSession().Set<PlazasPorEmpleado>().AddOrUpdate(listPlazasPorEmpleado[i]);
                    getSession().SaveChanges();
                }
                getSession().Set<FiniquitosLiquida>().AddOrUpdate(finiquitosLiquidaciones);
                getSession().SaveChanges();

                if (ingresosReingresosBajas != null)
                {
                    // ingresosReingresosBajas.plazasPorEmpleado = listPlazasPorEmpleado[0];
                    getSession().Set<IngresosBajas>().AddOrUpdate(ingresosReingresosBajas);
                    getSession().SaveChanges();
                }

                if (eliminadoMovNomConceps != null)
                {
                    commit = deleteListMovimientosNomina(eliminadoMovNomConceps, dbContext);
                }
                contador++;
                if (salariosIntegrado != null)
                {
                    //deleteListQuery( typeof(SalariosIntegrados).Name, "id", new Object[]{salariosIntegrado.getId()});
                    deleteListQuery("SalariosIntegrados", new CamposWhere("SalariosIntegrados.id", salariosIntegrado.id, OperadorComparacion.IGUAL, OperadorLogico.AND), dbContext);
                }
                if (commit)
                {
                    mensajeResultado.resultado = finiquitosLiquidaciones;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    mensajeResultado.error = "ERROR en algun proceso del finiquito";
                    mensajeResultado.resultado = "ERROR en algun proceso del finiquito";
                    mensajeResultado.resultado = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getCancelarFiniquito()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = ex;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private bool deleteListMovimientosNomina(Object[] clavesDeleteMovimientos, DBContextAdapter dbContext)
        {
            bool committ = true;
            try
            {
                //deleteListQuery("MovNomBaseAfecta ", "movNomConcep.id", clavesDeleteMovimientos);
                deleteListQuery("MovNomBaseAfecta", new CamposWhere("MovNomBaseAfecta.movNomConcep.id", clavesDeleteMovimientos, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                //deleteListQuery(CalculoISR.class.getSimpleName(), "movNomConcep.id", clavesDeleteMovimientos);//JSA02
                deleteListQuery("CalculoISR", new CamposWhere("CalculoISR.movNomConcep.id", clavesDeleteMovimientos, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                //deleteListQuery("CalculoIMSS", "movNomConcep.id", clavesDeleteMovimientos);
                deleteListQuery("CalculoIMSS", new CamposWhere("CalculoIMSS.movNomConcep.id", clavesDeleteMovimientos, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                //deleteListQuery("MovNomConceParam ", "movNomConcep.id", clavesDeleteMovimientos);
                deleteListQuery("MovNomConceParam", new CamposWhere("MovNomConceParam.movNomConcep.id", clavesDeleteMovimientos, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
                //deleteListQuery("MovNomConcep", "id", clavesDeleteMovimientos);
                deleteListQuery("MovNomConcep", new CamposWhere("MovNomConcep.id", clavesDeleteMovimientos, OperadorComparacion.IN, OperadorLogico.AND), dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListMovimientosNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return committ;
        }

        public Mensaje getFiniquitosLiquidacionesAll(string claveRazonesSociales, TipoBaja tipoBaja, ModoBaja modoBaja, DBContextAdapter dbContext)
        {
            listFiniquitosLiquidaciones = new List<FiniquitosLiquida>();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                listFiniquitosLiquidaciones = (from a in getSession().Set<FiniquitosLiquida>()
                                               where a.razonesSociales.clave.Equals(claveRazonesSociales) &&
                                               a.tipoBaja == (int)tipoBaja &&
                                               a.modoBaja == modoBaja
                                               orderby a.referencia
                                               select a).ToList();
                mensajeResultado.resultado = listFiniquitosLiquidaciones;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("FiniquitosLiquidacionesAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getFiniquitosLiquidacionesGuardarModificar(FiniquitosLiquida finiquitosLiquidaciones, object[] clavesDeleteMovimientos, List<MovNomConcep> AgreModifMovimientos, List<FiniqLiquidPlazas> finiqLiquidPlazas, object[] eliminadosfiniqLiquidPlazas, List<FiniqLiquidCncNom> listFiniqLiquidCncNom, List<FiniqLiquidCncNom> deleteFiniqLiquidCncNom, int cantPlazasFiniquitadas, int cantPlazasEmpleado, IngresosBajas ingresosReingresosBajas, List<PlazasPorEmpleado> cerrarPlazaEmpleado, SalariosIntegrados salariosIntegrado, int rango, DBContextAdapter dbContext)
        {
            int i, contador = 0;
            bool commit = true;
            try
            {

                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<FiniquitosLiquida>().Add(finiquitosLiquidaciones);
                getSession().SaveChanges();
                contador++;
                try
                {
                    #region guardando-modificando-eliminando movimientos
                    if (clavesDeleteMovimientos != null)
                    {
                        commit = deleteListMovimientosNomina(clavesDeleteMovimientos, dbContext);
                    }
                    AgreModifMovimientos = (AgreModifMovimientos == null ? new List<MovNomConcep>() : AgreModifMovimientos);
                    if (commit && !AgreModifMovimientos.Any())
                    {
                        //Guardado de modificados de los movimientos
                        foreach (MovNomConcep movNomConcep in AgreModifMovimientos)
                        {
                            contador++;
                            getSession().Set<MovNomConcep>().AddOrUpdate(movNomConcep);
                            if (contador % rango == 0 & contador > 0)
                            {
                                getSession().SaveChanges();
                            }
                        }
                    }
                    #endregion
                    try
                    {
                        #region guardado-modificado-eliminado finiquitosPlaza
                        if (eliminadosfiniqLiquidPlazas != null)
                        {
                            commit = deleteListFiniqLiquidPlazas("FiniqLiquidPlazas", new CamposWhere("FiniqLiquidPlazas.id", eliminadosfiniqLiquidPlazas, OperadorComparacion.IN, OperadorLogico.AND), dbContext);


                            //deleteListFiniqLiquidPlazas("FiniqLiquidPlazas", "id", eliminadosfiniqLiquidPlazas, "");
                        }
                        finiqLiquidPlazas = (finiqLiquidPlazas == null ? new List<FiniqLiquidPlazas>() : finiqLiquidPlazas);
                        if (commit && !finiqLiquidPlazas.Any())
                        {
                            for (i = 0; i < finiqLiquidPlazas.Count(); i++)
                            {
                                contador++;
                                finiqLiquidPlazas[i].finiquitosLiquida = finiquitosLiquidaciones;
                                getSession().Set<FiniqLiquidPlazas>().AddOrUpdate(finiqLiquidPlazas[i]);
                                if (contador % rango == 0 & contador > 0)
                                {
                                    getSession().SaveChanges();
                                }
                            }
                        }
                        #endregion
                        try
                        {
                            #region guardar-modificar-eliminar FiniquitosLiquidaConcepto
                            deleteFiniqLiquidCncNom = (deleteFiniqLiquidCncNom == null ? new List<FiniqLiquidCncNom>() : deleteFiniqLiquidCncNom);
                            if (commit && !deleteFiniqLiquidCncNom.Any())
                            {
                                for (i = 0; i < deleteFiniqLiquidCncNom.Count(); i++)
                                {
                                    getSession().Set<FiniqLiquidCncNom>().Attach(deleteFiniqLiquidCncNom[i]);
                                    getSession().Set<FiniqLiquidCncNom>().Remove(deleteFiniqLiquidCncNom[i]);
                                }
                            }
                            listFiniqLiquidCncNom = (listFiniqLiquidCncNom == null ? new List<FiniqLiquidCncNom>() : listFiniqLiquidCncNom);
                            if (commit && !listFiniqLiquidCncNom.Any())
                            {
                                for (i = 0; i < listFiniqLiquidCncNom.Count(); i++)
                                {
                                    contador++;
                                    listFiniqLiquidCncNom[i].finiquitosLiquida = finiquitosLiquidaciones;
                                    getSession().Set<FiniqLiquidCncNom>().AddOrUpdate(listFiniqLiquidCncNom[i]);
                                    if (contador % rango == 0 & contador > 0)
                                    {
                                        getSession().SaveChanges();
                                    }

                                }
                            }
                            #endregion
                            try
                            {
                                #region Modificacion de Plaza por empleado
                                PlazasPorEmpleado plazaEmpCerrada = null;
                                if (finiquitosLiquidaciones.modoBaja != ModoBaja.PROYECCION & cerrarPlazaEmpleado != null)
                                {
                                    for (i = 0; i < cerrarPlazaEmpleado.Count(); i++)
                                    {
                                        contador++;
                                        plazaEmpCerrada = finiqLiquidPlazas[i].plazasPorEmpleado;
                                        plazaEmpCerrada.fechaFinal = finiquitosLiquidaciones.fechaBaja;
                                        getSession().Set<PlazasPorEmpleado>().AddOrUpdate(cerrarPlazaEmpleado[i]);
                                        if (contador % rango == 0 & contador > 0)
                                        {
                                            getSession().SaveChanges();
                                        }
                                    }
                                }
                                #endregion
                                try
                                {
                                    #region Ingresos y Reingresos, salariosIntegrado
                                    if (finiquitosLiquidaciones.modoBaja != ModoBaja.PROYECCION)
                                    {
                                        if (cerrarPlazaEmpleado != null)
                                        {
                                            if (cantPlazasEmpleado <= cantPlazasFiniquitadas + cerrarPlazaEmpleado.Count())
                                            {
                                                if (ingresosReingresosBajas != null)
                                                {
                                                    ingresosReingresosBajas.fechaBaja = finiquitosLiquidaciones.fechaBaja;
                                                    //ingresosReingresosBajas.finiquitosLiquida = finiquitosLiquidaciones;
                                                    //ingresosReingresosBajas.plazasPorEmpleado = plazaEmpCerrada;
                                                    getSession().Set<IngresosBajas>().AddOrUpdate(ingresosReingresosBajas);
                                                }
                                                DateTime? cal = new DateTime?();
                                                cal = finiquitosLiquidaciones.fechaCalculo;
                                                cal.Value.AddMonths(cal.Value.Month + 1);
                                                if (salariosIntegrado == null)
                                                {
                                                    salariosIntegrado = new SalariosIntegrados();
                                                }
                                                else if (salariosIntegrado.finiquitosLiquida == null)
                                                {
                                                    salariosIntegrado.id = 0;
                                                }
                                                salariosIntegrado.empleados = plazaEmpCerrada.empleados;
                                                salariosIntegrado.registroPatronal = plazaEmpCerrada.registroPatronal;
                                                salariosIntegrado.fecha = finiquitosLiquidaciones.fechaCalculo;
                                                salariosIntegrado.tipoDeSalario = 1;
                                                salariosIntegrado.salarioDiarioFijo = 0;
                                                salariosIntegrado.salarioDiarioIntegrado = 0;
                                                salariosIntegrado.salarioDiarioVariable = 0;
                                                salariosIntegrado.factorIntegracion = 0;
                                                salariosIntegrado.finiquitosLiquida = finiquitosLiquidaciones;
                                                getSession().Set<SalariosIntegrados>().AddOrUpdate(salariosIntegrado);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    commit = false;
                                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getFiniquitosLiquidacionesGuardarModificar()1_Error: ").Append(ex));
                                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                                    mensajeResultado.error = ex.GetBaseException().ToString();
                                    mensajeResultado.resultado = ex;
                                }
                            }
                            catch (Exception ex)
                            {
                                commit = false;
                                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getFiniquitosLiquidacionesGuardarModificar()2_Error: ").Append(ex));
                                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                                mensajeResultado.error = ex.GetBaseException().ToString();
                                mensajeResultado.resultado = ex;
                            }
                        }
                        catch (Exception ex)
                        {
                            commit = false;
                            System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getFiniquitosLiquidacionesGuardarModificar()3_Error: ").Append(ex));
                            mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                            mensajeResultado.error = ex.GetBaseException().ToString();
                            mensajeResultado.resultado = ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        commit = false;
                        System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getFiniquitosLiquidacionesGuardarModificar()4_Error: ").Append(ex));
                        mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                        mensajeResultado.error = ex.GetBaseException().ToString();
                        mensajeResultado.resultado = ex;
                    }
                }
                catch (Exception ex)
                {
                    commit = false;
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getFiniquitosLiquidacionesGuardarModificar()5_Error: ").Append(ex));
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                    mensajeResultado.resultado = ex;
                }
                if (commit)
                {
                    mensajeResultado.resultado = finiquitosLiquidaciones;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    mensajeResultado.error = "ERROR en algun proceso del finiquito";
                    mensajeResultado.resultado = "ERROR en algun proceso del finiquito";
                    mensajeResultado.resultado = null;
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getFiniquitosLiquidacionesGuardarModificar()5_Error: "));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getFiniquitosLiquidacionesGuardarModificar()6_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }
        private bool deleteListFiniqLiquidPlazas(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            bool result = true;
            try
            {
                // deleteListQuery(tabla, campo, valores);
                deleteListQuery(tabla, campoWhere, dbContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("deleteListFiniqLiquidPlazas()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                result = false;
            }
            return result;

        }
        public Mensaje getFiniquitosLiquidacionesPorCamposClave(string referencia, RazonesSociales razonSocial, ModoBaja modoBaja, TipoBaja tipoBaja, DBContextAdapter dbContext)
        {
            FiniquitosLiquida finiquitosLiquida = new FiniquitosLiquida();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                finiquitosLiquida = (from o in getSession().Set<FiniquitosLiquida>()
                                     where o.referencia.Equals(referencia) &&
                                     o.razonesSociales.id == razonSocial.id &&
                                     o.modoBaja == modoBaja &&
                                     o.tipoBaja == (int)tipoBaja
                                     select o).SingleOrDefault();
                mensajeResultado.resultado = finiquitosLiquida;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("FiniquitosLiquidacionesPorCamposClave()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getPorEmpleado(string claveEmpleado, string claveRazonSocial, ModoBaja modoBaja, TipoBaja tipoBaja, DBContextAdapter dbContext)
        {
            FiniquitosLiquida Xempleado = new FiniquitosLiquida();
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                Xempleado = (from o in getSession().Set<FiniquitosLiquida>()
                             where o.empleados.clave.Equals(claveEmpleado) &&
                             o.razonesSociales.clave.Equals(claveRazonSocial) &&
                             o.modoBaja == modoBaja &&
                             o.tipoBaja == (int)tipoBaja
                             select o).SingleOrDefault();
                mensajeResultado.resultado = Xempleado;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("PorEmpleado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getSaveFinLiqModifConceptos(FiniquitosLiquida finiquitosLiquidaciones, object[] clavesDeleteMovimientos, List<MovNomConcep> agreModifMovimientos, List<FiniqLiquidCncNom> listFiniqLiquidCncNom, List<FiniqLiquidCncNom> deleteFiniqLiquidCncNom, int rango, DBContextAdapter dbContext)
        {
            int i, contador = 0;
            bool commit = true;
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<FiniquitosLiquida>().AddOrUpdate(finiquitosLiquidaciones);
                getSession().SaveChanges();
                contador++;
                try
                {
                    #region guardando-modificando-eliminando movimientos
                    if (clavesDeleteMovimientos != null)
                    {
                        commit = deleteListMovimientosNomina(clavesDeleteMovimientos, dbContext);
                    }
                    agreModifMovimientos = (agreModifMovimientos == null ? new List<MovNomConcep>() : agreModifMovimientos);
                    if (commit && !agreModifMovimientos.Any())
                    {
                        //Guardado de modificados de los movimientos
                        foreach (MovNomConcep movNomConcep in agreModifMovimientos)
                        {
                            contador++;
                            getSession().Set<MovNomConcep>().AddOrUpdate(movNomConcep);
                            if (contador % rango == 0 & contador > 0)
                            {
                                getSession().SaveChanges();
                            }
                        }
                    }
                    #endregion
                    deleteFiniqLiquidCncNom = (deleteFiniqLiquidCncNom == null ? new List<FiniqLiquidCncNom>() : deleteFiniqLiquidCncNom);
                    if (commit && !deleteFiniqLiquidCncNom.Any())
                    {
                        for (i = 0; i < deleteFiniqLiquidCncNom.Count(); i++)
                        {
                            getSession().Set<FiniqLiquidCncNom>().Attach(deleteFiniqLiquidCncNom[i]);
                            getSession().Set<FiniqLiquidCncNom>().Remove(deleteFiniqLiquidCncNom[i]);
                            getSession().SaveChanges();
                        }
                    }
                    listFiniqLiquidCncNom = (listFiniqLiquidCncNom == null ? new List<FiniqLiquidCncNom>() : listFiniqLiquidCncNom);
                    if (commit && !listFiniqLiquidCncNom.Any())
                    {
                        for (i = 0; i < listFiniqLiquidCncNom.Count(); i++)
                        {
                            contador++;
                            listFiniqLiquidCncNom[i].finiquitosLiquida = finiquitosLiquidaciones;
                            getSession().Set<FiniqLiquidCncNom>().AddOrUpdate(listFiniqLiquidCncNom[i]);
                            if (contador % rango == 0 & contador > 0)
                            {
                                getSession().SaveChanges();
                            }
                        }
                    }
                    mensajeResultado.resultado = finiquitosLiquidaciones;

                }
                catch (Exception ex)
                {
                    commit = false;
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getFiniquitosLiquidacionesGuardarModificar()5_Error: ").Append(ex));
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                    mensajeResultado.resultado = null;
                }
                if (commit)
                {
                    mensajeResultado.resultado = finiquitosLiquidaciones;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                    getSession().Database.CurrentTransaction.Commit();
                }
                else
                {
                    mensajeResultado.error = "ERROR en algun proceso del finiquito";
                    mensajeResultado.resultado = "ERROR en algun proceso del finiquito";
                    mensajeResultado.resultado = null;
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getFiniquitosLiquidacionesGuardarModificar()5_Error: "));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getFiniquitosLiquidacionesGuardarModificar()6_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = ex;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje validaFiniquitosLiquidacionTimbrados(DateTime fechaCalculo, List<PlazasPorEmpleadosMov> plazasPorEmpleados, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                for (int i = 0; i < plazasPorEmpleados.Count(); i++)
                {
                    PeriodosNomina per = new PeriodosNomina();
                    TipoNomina tn = plazasPorEmpleados[i].tipoNomina;
                    per = (from p in getSession().Set<PeriodosNomina>()
                           join tc in getSession().Set<TipoCorrida>() on p.tipoCorrida.id equals tc.id
                           where p.fechaInicial >= fechaCalculo ||
                           p.fechaFinal > fechaCalculo &&
                           p.tipoCorrida.clave.Equals(tn.clave) &&
                           p.tipoCorrida.Equals("FIN")
                           select p).Take(1).SingleOrDefault();
                    if (per != null)
                    {
                        CFDIEmpleado cfdiEmpleado = new CFDIEmpleado();
                        cfdiEmpleado = (from o in getSession().Set<CFDIEmpleado>()
                                        join cr in getSession().Set<CFDIRecibo>() on o.cfdiRecibo.id equals cr.id
                                        where o.periodosNomina.id == per.id &&
                                        o.plazasPorEmpleadosMov.id == plazasPorEmpleados[i].id &&
                                        o.tipoCorrida.clave.Equals("FIN") &&
                                        o.tipoNomina.id == tn.id &&
                                        cr.statusTimbrado == 0
                                        select o).SingleOrDefault();
                        if (cfdiEmpleado != null)
                        {
                            mensajeResultado.resultado = "Timbrado";
                            getSession().Database.CurrentTransaction.Commit();
                        }
                        else if (!per.status)
                        {
                            mensajeResultado.resultado = "Periodo";
                            getSession().Database.CurrentTransaction.Commit();
                        }
                        else
                        {
                            mensajeResultado.resultado = "Periodo";
                            getSession().Database.CurrentTransaction.Commit(); getSession().Database.CurrentTransaction.Commit();
                        }
                    }
                    else
                    {
                        mensajeResultado.resultado = "NoPeriodoFiniquito";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("validaFiniquitosLiquidacionTimbrados()1_Error: ").Append(ex));
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


