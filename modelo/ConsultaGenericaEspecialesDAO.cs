/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Clase ConsultaGenericaEspecialesDAO para llamados a metodos de Entity
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
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.servicios.extras;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.TestCompilador.funciones;
using System.Data.Entity;
using Exitosw.Payroll.Core.campos;

namespace Exitosw.Payroll.Core.modelo
{
    public class ConsultaGenericaEspecialesDAO : GenericRepository<Object>, ConsultaGenericaEspecialesDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private ConectorQuerysGenericos conectorQuerysGenericos = new ConectorQuerysGenericos();

        public Mensaje consultaPorRangosFiltro(string identificador, ValoresRango valoresRango, List<CamposWhere> listCamposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            return consultGenericaPorRangos(identificador, valoresRango, listCamposWhere, dbContext, dbContextMaestra);
        }

        private Mensaje consultGenericaPorRangos(string identificador, ValoresRango valoresRango, List<CamposWhere> listCamposWhere, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            return generarQuery(identificador, valoresRango, listCamposWhere, false, dbContext, dbContextMaestra);
        }


        public Mensaje existeClave(string identificador, List<CamposWhere> listCamposWhere, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaestra)
        {
            return generarQuery(identificador, null, listCamposWhere, true, dbContextSimple, dbContextMaestra);
        }

        //sin uso queda 
        public Mensaje obtenerDatosCriterioConsulta(string[] tablas, List<CamposSelect> listCamposSelect, List<CamposWhere> listCamposWhere, List<CamposOrden> listCamposOrden, DBContextAdapter dbContext)
        {
            throw new NotImplementedException();
        }

        ////private List<object> generaQueryCriterioConsulta(string[] tablas, List<CamposSelect> listCamposSelect, List<CamposWhere> listCamposWhere, List<CamposOrden> listCamposOrden) {

        ////    return null;
        ////}

        ////private List<object> construyeQueryConsultaCriterioOuter(string[] tablas, List<CamposSelect> listCamposSelect, List<CamposWhere> listCamposWhere, List<CamposOrden> listCamposOrden)
        ////{

        ////    return null;
        ////}
        private Mensaje generarQuery(string identificador, ValoresRango valoresRango, List<CamposWhere> listCamposWhere, bool uniqueResult, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaestra)
        {
            inicializaVariableMensaje();
            mensajeResultado = obtenerQueryPerzonalizado(identificador, listCamposWhere, uniqueResult, dbContextSimple, dbContextMaestra);
            return mensajeResultado;
        }

        private Mensaje obtenerQueryPerzonalizado(string identificador, List<CamposWhere> listCamposWhere, bool uniqueResult, DBContextAdapter dbContext, DBContextAdapter dbContextMaestra)
        {
            List<object> camposWhere = new List<object>();
            mensajeResultado.error = "";
            mensajeResultado.noError = 0;
            CamposSelect campoSelect;
            OperadorSelect operadorSelect;
            List<CamposFrom> listCamposFrom;
            CamposOrden campoOrden;
            

            //Code 12630
            DBContextAdapter conexionUsada;
            if (dbContext != null)
            {
                conexionUsada = dbContext;
            }
            else {
                conexionUsada = dbContextMaestra;
            }


            TipoResultado tipoResultado = TipoResultado.Lista;
            if (uniqueResult)
            {
                tipoResultado = TipoResultado.Unico;
            }
            if (identificador.Equals("QueryEmpleadoFiniquito"))
            {
                campoSelect = new CamposSelect("FiniqLiquidPlazas.plazasPorEmpleado.empleados", TipoFuncion.NINGUNO);
                operadorSelect = new OperadorSelect(new List<CamposSelect>() { campoSelect });
                listCamposFrom = new List<CamposFrom>() { new CamposFrom("FiniqLiquidPlazas", TipoJoin.LEFT_JOIN), new CamposFrom("FiniqLiquidPlazas.finiquitosLiquida", TipoJoin.RIGHT_JOIN),
                    new CamposFrom("FiniqLiquidPlazas.plazasPorEmpleado", TipoJoin.RIGHT_JOIN), new CamposFrom("FiniqLiquidPlazas.plazasPorEmpleado.empleados", TipoJoin.RIGHT_JOIN),
                    new CamposFrom("FiniqLiquidPlazas.plazasPorEmpleado.razonesSociales", TipoJoin.RIGHT_JOIN)};
                listCamposWhere = listCamposWhere == null ? new List<CamposWhere>() : listCamposWhere;
                if (listCamposWhere.Count > 0)
                {
                    listCamposWhere[listCamposWhere.Count - 2].campo = "FiniqLiquidPlazas.plazasPorEmpleado.razonesSociales.clave";
                    listCamposWhere[listCamposWhere.Count - 1].campo = "FiniqLiquidPlazas.finiquitosLiquida.modoBaja";
                    listCamposWhere[listCamposWhere.Count - 1].operadorComparacion = OperadorComparacion.DIFERENTE;
                    listCamposWhere[listCamposWhere.Count - 1].operadorLogico = OperadorLogico.OR;
                    List<CamposWhere> listSubCamposWhere = new List<CamposWhere>() { new CamposWhere("FiniqLiquidPlazas.id", 0, OperadorComparacion.IS_NULL, OperadorLogico.AND) };
                    listCamposWhere[listCamposWhere.Count - 1].listCamposAgrupados = listSubCamposWhere;
                }
                campoOrden = new CamposOrden("FiniqLiquidPlazas.plazasPorEmpleado.empleados.clave");

                mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, tipoResultado, TipoOperacion.SELECT,
                    "", operadorSelect, listCamposFrom, listCamposWhere, null, new List<CamposOrden>() { campoOrden }, null);
                return mensajeResultado;
            }
            else if (identificador.Equals("QueryEmpleadoEspecial") | identificador.Equals("QueryEmpleadoEspecialMovimientosNomina"))
            {
                if (identificador.ToUpper().Equals("QueryEmpleadoEspecial"))
                {
                    operadorSelect = new OperadorSelect(new List<CamposSelect>() { new CamposSelect("PlazasPorEmpleadosMov.tipoNomina", TipoFuncion.NINGUNO),
                        new CamposSelect("PlazasPorEmpleadosMov.plazasPorEmpleado.empleados", TipoFuncion.NINGUNO) });
                }
                else
                {
                    operadorSelect = new OperadorSelect(new List<CamposSelect>() { new CamposSelect("PlazasPorEmpleadosMov", TipoFuncion.NINGUNO),
                        new CamposSelect("PlazasPorEmpleadosMov.plazasPorEmpleado.empleados", TipoFuncion.NINGUNO) });
                }

                CamposWhere campoWherePrincipal = new CamposWhere("PlazasPorEmpleadosMov.id", null, OperadorComparacion.IN, OperadorLogico.AND);

                SubConsulta subConsulta = new SubConsulta();
                CamposSelect campoSelectSub = new CamposSelect("PlazasPorEmpleadosMov.id", TipoFuncion.MAXIMO);
                OperadorSelect operadorSelectSub = new OperadorSelect(new List<CamposSelect>() { campoSelectSub });

                if (listCamposWhere.Count > 0)
                {

                    int cont = 0, index = 0;
                    if (listCamposWhere.Count > 4)
                    {
                        index = 1;
                    }
                    bool aplicarRestriccionEmpleadoDadoDeBaja = false;
                    foreach (var campoWhere in listCamposWhere)
                    {
                        if (String.IsNullOrEmpty(campoWhere.campo))
                        {
                            if (cont == 0 + index)
                            {
                                campoWhere.campo = "PlazasPorEmpleadosMov.plazasPorEmpleado.razonesSociales.clave";

                            }
                            else if (cont == (1 + index) & campoWhere.valor != null)
                            {
                                if (campoWhere.valor.GetType().Equals(typeof(TipoCorrida)))
                                {
                                    TipoCorrida corrida = (TipoCorrida)campoWhere.valor;
                                    if (corrida.clave.ToUpper().Equals("FIN") || corrida.clave.ToUpper().Equals("LIQ"))
                                    {
                                        aplicarRestriccionEmpleadoDadoDeBaja = false;
                                    }
                                    //consulta and parametros i++ =  parametro i ++ sabe em que se use mal codigo
                                }
                            }
                            else if (cont == 2 + index)
                            {
                                if (aplicarRestriccionEmpleadoDadoDeBaja)
                                {
                                    campoWhere.campo = "PlazasPorEmpleadosMov.fechaInicial";
                                    campoWhere.operadorComparacion = OperadorComparacion.MENOR_IGUAL;
                                    campoWhere.operadorLogico = OperadorLogico.OR;

                                    CamposWhere campoWhereSub = new CamposWhere("PlazasPorEmpleadosMov.fechaInicial", campoWhere.valor, OperadorComparacion.BETWEEN, OperadorLogico.AND);
                                    campoWhere.listCamposAgrupados = new List<CamposWhere>() { campoWhereSub };
                                }
                                else
                                {
                                    campoWhere.campo = "PlazasPorEmpleadosMov.fechaInicial";
                                    campoWhere.operadorComparacion = OperadorComparacion.IGUAL;
                                    campoWhere.operadorLogico = OperadorLogico.AND;
                                }
                            }
                            else if (cont == 3 + index)
                            {
                                if (aplicarRestriccionEmpleadoDadoDeBaja)
                                {
                                    campoWhere.campo = "PlazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal";
                                    campoWhere.operadorComparacion = OperadorComparacion.MAYOR_IGUAL;
                                    campoWhere.operadorLogico = OperadorLogico.OR;

                                    CamposWhere campoWhereSub = new CamposWhere("PlazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal", campoWhere.valor, OperadorComparacion.BETWEEN, OperadorLogico.AND);
                                    campoWhere.listCamposAgrupados = new List<CamposWhere>() { campoWhereSub };
                                }
                                else
                                {
                                    campoWhere.campo = "PlazasPorEmpleadosMov.plazasPorEmpleado.fechaFinal";
                                    campoWhere.operadorComparacion = OperadorComparacion.IGUAL;
                                    campoWhere.operadorLogico = OperadorLogico.AND;
                                }
                            }
                            cont++;
                        }
                        else
                        {
                            if (!campoWhere.campo.Contains(typeof(PlazasPorEmpleadosMov).Name))
                            {
                                campoWhere.campo = String.Concat(typeof(PlazasPorEmpleadosMov).Name, ".", campoWhere.campo);
                            }
                        }
                    }
                }
                CamposGrupo campoGrupo = new CamposGrupo("PlazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave");
                campoOrden = new CamposOrden("PlazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave");

                mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionUsada, tipoResultado, TipoOperacion.SELECT,
                    "", operadorSelect, null, listCamposWhere, new List<CamposGrupo>() { campoGrupo }, new List<CamposOrden>() { campoOrden, new CamposOrden("PlazasPorEmpleadosMov.fechaInicial") }, null);

            }
            else if (identificador.Equals("queryPlazasEmpleadoEspecial"))
            {
                bool manejoPagoPorHora = false;
                ManejoHorasPor manejoPorHoras = ManejoHorasPor.HSM;
                ManejoSalarioDiario manejoSalarioDiario = ManejoSalarioDiario.QUINCENAL;
                try
                {
                    setSession(dbContext.context);
                    getSession().Database.BeginTransaction();

                    #region  Maneja pago por horas

                    operadorSelect = new OperadorSelect(new List<CamposSelect>() { new CamposSelect("Cruce.valor", TipoFuncion.NINGUNO) });

                    var query = (from cr in getSession().Set<Cruce>()
                                 join pr in getSession().Set<Parametros>() on cr.parametros_ID equals pr.id
                                 join ea in getSession().Set<ElementosAplicacion>() on cr.elementosAplicacion_ID equals ea.id
                                 where pr.clave == (decimal)ClavesParametrosModulos.claveParametroPagosPorHora && ea.clave.Equals(ClavesParametrosModulos.claveElementoAplicacionRazonSocial) && cr.claveElemento.Equals(listCamposWhere[listCamposWhere.Count - 1].valor)
                                 select cr.valor
                    );

                    string valor = query.FirstOrDefault();

                    //List<CamposWhere> listSubCamposWhere = new List<CamposWhere>() { new CamposWhere("Cruce.parametros.clave", claveParametroPagosPorHora, OperadorComparacion.IGUAL, OperadorLogico.AND),
                    //    new CamposWhere("Cruce.elementosAplicacion.clave", claveElementoAplicacionRazonSocial, OperadorComparacion.IGUAL, OperadorLogico.AND),
                    //    new CamposWhere("Cruce.claveElemento", listCamposWhere[listCamposWhere.Count - 1].valor, OperadorComparacion.IGUAL, OperadorLogico.AND)};

                    //mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionPendiente, TipoResultado.Unico, TipoOperacion.SELECT,
                    //   "", operadorSelect, null, listSubCamposWhere, null, null, null);
                    //if (mensajeResultado.noError != 0) {
                    //    return mensajeResultado;
                    //}
                    //string valor = (string) mensajeResultado.resultado;
                    if (String.IsNullOrEmpty(valor))
                    {
                        // operadorSelect = new OperadorSelect(new List<CamposSelect>() { new CamposSelect("Parametros.valor", TipoFuncion.NINGUNO) });
                        // listSubCamposWhere = new List<CamposWhere>() { new CamposWhere("Parametros.clave", claveParametroPagosPorHora, OperadorComparacion.IGUAL, OperadorLogico.AND),
                        // new CamposWhere("Parametros.modulo.clave", claveModuloGlobal, OperadorComparacion.IGUAL, OperadorLogico.AND)};

                        // mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionPendiente, TipoResultado.Unico, TipoOperacion.SELECT,
                        //"", operadorSelect, null, listSubCamposWhere, null, null, null);
                        // if (mensajeResultado.noError != 0)
                        // {
                        //     return mensajeResultado;
                        // }

                        // valor = (string)mensajeResultado.resultado;    
                        query = (from pr in getSession().Set<Parametros>()
                                 join m in getSession().Set<Modulo>() on pr.modulo_ID equals m.id
                                 where pr.clave == (decimal)ClavesParametrosModulos.claveParametroPagosPorHora && m.clave.Equals(ClavesParametrosModulos.claveModuloGlobal)
                                 select pr.valor
                       );
                        valor = query.FirstOrDefault();
                    }

                    if (valor.Equals(ClavesParametrosModulos.opcionParametroPagarPorHoras))
                    {
                        manejoPagoPorHora = true;
                    }
                    else
                    {
                        manejoPagoPorHora = false;
                    }

                    #endregion

                    #region  Manejo horas por
                    ////operadorSelect = new OperadorSelect(new List<CamposSelect>() { new CamposSelect("Cruce.valor", TipoFuncion.NINGUNO) });
                    query = (from cr in getSession().Set<Cruce>()
                             join pr in getSession().Set<Parametros>() on cr.parametros_ID equals pr.id
                             join ea in getSession().Set<ElementosAplicacion>() on cr.elementosAplicacion_ID equals ea.id
                             where pr.clave == (decimal)ClavesParametrosModulos.claveParametroManejarHorasPor && ea.clave.Equals(ClavesParametrosModulos.claveElementoAplicacionRazonSocial) && cr.claveElemento.Equals(listCamposWhere[listCamposWhere.Count - 1].valor)
                             select cr.valor
                    );

                    valor = query.FirstOrDefault();
                    ////listSubCamposWhere = new List<CamposWhere>() { new CamposWhere("Cruce.parametros.clave", claveParametroManejarHorasPor, OperadorComparacion.IGUAL, OperadorLogico.AND),
                    ////    new CamposWhere("Cruce.elementosAplicacion.clave", claveElementoAplicacionRazonSocial, OperadorComparacion.IGUAL, OperadorLogico.AND),
                    ////    new CamposWhere("Cruce.claveElemento", listCamposWhere[listCamposWhere.Count - 1].valor, OperadorComparacion.IGUAL, OperadorLogico.AND)};

                    ////mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionPendiente, TipoResultado.Unico, TipoOperacion.SELECT,
                    ////   "", operadorSelect, null, listSubCamposWhere, null, null, null);
                    ////if (mensajeResultado.noError != 0)
                    ////{
                    ////    return mensajeResultado;
                    ////}
                    ////valor = (string)mensajeResultado.resultado;
                    if (String.IsNullOrEmpty(valor))
                    {
                        // operadorSelect = new OperadorSelect(new List<CamposSelect>() { new CamposSelect("Parametros.valor", TipoFuncion.NINGUNO) });
                        // listSubCamposWhere = new List<CamposWhere>() { new CamposWhere("Parametros.clave", claveParametroManejarHorasPor, OperadorComparacion.IGUAL, OperadorLogico.AND),
                        // new CamposWhere("Parametros.modulo.clave", claveModuloGlobal, OperadorComparacion.IGUAL, OperadorLogico.AND)};

                        // mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionPendiente, TipoResultado.Unico, TipoOperacion.SELECT,
                        //"", operadorSelect, null, listSubCamposWhere, null, null, null);
                        // if (mensajeResultado.noError != 0)
                        // {
                        //     return mensajeResultado;
                        // }

                        // valor = (string)mensajeResultado.resultado;
                        query = (from pr in getSession().Set<Parametros>()
                                 join m in getSession().Set<Modulo>() on pr.modulo_ID equals m.id
                                 where pr.clave == (decimal)ClavesParametrosModulos.claveParametroManejarHorasPor && m.clave.Equals(ClavesParametrosModulos.claveModuloGlobal)
                                 select pr.valor
                        );
                        valor = query.FirstOrDefault();
                    }
                    string opcionParametroHorasNaturales = "1";
                    if (valor.Equals(opcionParametroHorasNaturales))
                    {
                        manejoPorHoras = ManejoHorasPor.HORASNATURALES;
                    }
                    else
                    {
                        manejoPorHoras = ManejoHorasPor.HSM;
                    }
                    #endregion

                    #region  Manejo salario diario
                    ////operadorSelect = new OperadorSelect(new List<CamposSelect>() { new CamposSelect("Cruce.valor", TipoFuncion.NINGUNO) });

                    query = (from cr in getSession().Set<Cruce>()
                             join pr in getSession().Set<Parametros>() on cr.parametros_ID equals pr.id
                             join ea in getSession().Set<ElementosAplicacion>() on cr.elementosAplicacion_ID equals ea.id
                             where pr.clave == (decimal)ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor && ea.clave.Equals(ClavesParametrosModulos.claveElementoAplicacionRazonSocial) && cr.claveElemento.Equals(listCamposWhere[listCamposWhere.Count - 1].valor)
                             select cr.valor
                    );

                    valor = query.FirstOrDefault();
                    ////listSubCamposWhere = new List<CamposWhere>() { new CamposWhere("Cruce.parametros.clave", claveParametroManejarSalarioDiarioPor, OperadorComparacion.IGUAL, OperadorLogico.AND),
                    ////    new CamposWhere("Cruce.elementosAplicacion.clave", claveElementoAplicacionRazonSocial, OperadorComparacion.IGUAL, OperadorLogico.AND),
                    ////    new CamposWhere("Cruce.claveElemento", listCamposWhere[listCamposWhere.Count - 1].valor, OperadorComparacion.IGUAL, OperadorLogico.AND)};

                    ////mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionPendiente, TipoResultado.Unico, TipoOperacion.SELECT,
                    ////   "", operadorSelect, null, listSubCamposWhere, null, null, null);
                    ////if (mensajeResultado.noError != 0)
                    ////{
                    ////    return mensajeResultado;
                    ////}
                    ////valor = (string)mensajeResultado.resultado;
                    if (String.IsNullOrEmpty(valor))
                    {
                        // operadorSelect = new OperadorSelect(new List<CamposSelect>() { new CamposSelect("Parametros.valor", TipoFuncion.NINGUNO) });
                        // listSubCamposWhere = new List<CamposWhere>() { new CamposWhere("Parametros.clave", claveParametroManejarSalarioDiarioPor, OperadorComparacion.IGUAL, OperadorLogico.AND),
                        // new CamposWhere("Parametros.modulo.clave", claveModuloGlobal, OperadorComparacion.IGUAL, OperadorLogico.AND)};

                        // mensajeResultado = conectorQuerysGenericos.consultaGenerica(conexionPendiente, TipoResultado.Unico, TipoOperacion.SELECT,
                        //"", operadorSelect, null, listSubCamposWhere, null, null, null);
                        // if (mensajeResultado.noError != 0)
                        // {
                        //     return mensajeResultado;
                        // }

                        // valor = (string)mensajeResultado.resultado;
                        query = (from pr in getSession().Set<Parametros>()
                                 join m in getSession().Set<Modulo>() on pr.modulo_ID equals m.id
                                 where pr.clave == (decimal)ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor && m.clave.Equals(ClavesParametrosModulos.claveModuloGlobal)
                                 select pr.valor
                         );
                        valor = query.FirstOrDefault();
                    }

                    if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioDiario))
                    {
                        manejoSalarioDiario = ManejoSalarioDiario.DIARIO;
                    }
                    else if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioSemanal))
                    {
                        manejoSalarioDiario = ManejoSalarioDiario.SEMANAL;
                    }
                    else if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioQuincenal))
                    {
                        manejoSalarioDiario = ManejoSalarioDiario.QUINCENAL;
                    }
                    else
                    {
                        manejoSalarioDiario = ManejoSalarioDiario.MENSUAL;
                    }
                    #endregion
                    getSession().Database.CurrentTransaction.Commit();

                    setSession(dbContext.context);
                    getSession().Database.BeginTransaction();
                    var queryPrincipal = (from p in getSession().Set<PlazasPorEmpleadosMov>()
                                          join pe in getSession().Set<PlazasPorEmpleado>() on p.plazasPorEmpleado_ID equals pe.id into pe_join
                                          from pe in pe_join.DefaultIfEmpty()
                                          join emp2 in getSession().Set<Empleados>() on pe.empleados_ID equals emp2.id into emp2_join
                                          from emp2 in emp2_join.DefaultIfEmpty()
                                          where
                                             (from m in getSession().Set<PlazasPorEmpleadosMov>()
                                              join pe2 in getSession().Set<PlazasPorEmpleado>() on m.plazasPorEmpleado_ID equals pe2.id into pe2_join
                                              from pe2 in pe2_join.DefaultIfEmpty()
                                              join rs in getSession().Set<RazonesSociales>() on pe2.razonesSociales_ID equals rs.id into rs_join
                                              from rs in rs_join.DefaultIfEmpty()
                                              join emp3 in getSession().Set<Empleados>() on pe2.empleados_ID equals emp3.id into emp3_join
                                              from emp3 in emp3_join.DefaultIfEmpty()
                                              where
                                                rs.clave.Equals(listCamposWhere[listCamposWhere.Count - 1].valor) &&
                                                (DateTime.Now >= m.fechaInicial && DateTime.Now <= pe2.fechaFinal)
                                              group new { pe2, m } by new
                                              {
                                                  pe2.referencia
                                              } into g
                                              select new
                                              {
                                                  Column1 = g.Max(pp => pp.m.id)
                                              }).Contains(new { Column1 = p.id })
                                          select new { p, pe, emp2 }

                            );

                    for (int i = 0; i < listCamposWhere.Count - 2; i++)
                    {
                        if (!String.IsNullOrEmpty(listCamposWhere[i].campo))
                        {
                            if (listCamposWhere[i].campo.Equals("ClaveEmpleado"))
                            {
                                queryPrincipal = (from subquery in queryPrincipal where subquery.emp2.clave.Equals(listCamposWhere[i].valor) select subquery);
                            }

                            if (listCamposWhere[i].campo.Equals("ClavePlazaEmpleado"))
                            {
                                queryPrincipal = (from subquery in queryPrincipal where subquery.pe.referencia.Equals(listCamposWhere[i].valor) select subquery);
                            }

                            if (listCamposWhere[i].campo.Equals("FechaInicial"))
                            {
                                queryPrincipal = (from subquery in queryPrincipal where subquery.p.fechaInicial == (DateTime)listCamposWhere[i].valor select subquery);
                            }

                            if (listCamposWhere[i].campo.Equals("FechaFinal"))
                            {
                                queryPrincipal = (from subquery in queryPrincipal where subquery.pe.fechaFinal == (DateTime)listCamposWhere[i].valor select subquery);
                            }

                            if (listCamposWhere[i].campo.Equals("Horas"))
                            {
                                queryPrincipal = (from subquery in queryPrincipal where subquery.p.horas == (int)listCamposWhere[i].valor select subquery);
                            }

                            if (listCamposWhere[i].campo.Equals("Importe"))
                            {
                                queryPrincipal = (from subquery in queryPrincipal where subquery.p.importe == (double)listCamposWhere[i].valor select subquery);
                            }

                            if (listCamposWhere[i].campo.Equals("DescripcionCentroCosto"))
                            {
                                queryPrincipal = (from subquery in queryPrincipal
                                                  join cc in getSession().Set<CentroDeCosto>() on subquery.p.centroDeCosto_ID equals cc.id into cc_join
                                                  from cc in cc_join.DefaultIfEmpty()
                                                  where cc.descripcion.Equals(listCamposWhere[i].valor)
                                                  select subquery);
                            }

                            if (listCamposWhere[i].campo.Equals("DescripcionPuesto"))
                            {
                                queryPrincipal = (from subquery in queryPrincipal
                                                  join pt in getSession().Set<Puestos>() on subquery.p.puestos_ID equals pt.id into pt_join
                                                  from pt in pt_join.DefaultIfEmpty()
                                                  where pt.descripcion.Equals(listCamposWhere[i].valor)
                                                  select subquery);
                            }

                            if (listCamposWhere[i].campo.Equals("ClavePlaza"))
                            {
                                queryPrincipal = (from subquery in queryPrincipal
                                                  join pz in getSession().Set<Plazas>() on subquery.p.plazas_ID equals pz.id into pz_join
                                                  from pz in pz_join.DefaultIfEmpty()
                                                  where pz.clave.Equals(listCamposWhere[i].valor)
                                                  select subquery);
                            }
                        }
                    }

                    var queryFinal = (from subquery in queryPrincipal
                                      orderby
                                           subquery.emp2.clave,
                                           subquery.p.fechaInicial
                                      select new DatosPlazasEmpleado()
                                      {
                                          plazasPorEmpleadosMov = subquery.p,
                                          fechaIngreso =
                                          ((from ingresos in getSession().Set<IngresosBajas>()
                                            where
                                            ingresos.id ==
                                            (from ol in getSession().Set<IngresosBajas>()
                                             where
                                                     ol.fechaIngreso <=
                                                     (from o2 in getSession().Set<IngresosBajas>()
                                                      join emp in getSession().Set<Empleados>() on ol.empleados_ID equals emp.id into emp_join
                                                      from emp in emp_join.DefaultIfEmpty()
                                                      where
                                                      o2.fechaBaja >= DateTime.Now &&
                                                      emp.clave == subquery.emp2.clave
                                                      select new
                                                      {
                                                          o2.fechaIngreso
                                                      }).Max(pp => pp.fechaIngreso)
                                             select new
                                             {
                                                 ol.id
                                             }).Max(pp => pp.id)
                                            select new
                                            {
                                                ingresos.fechaIngreso
                                            }).First().fechaIngreso),
                                          manejaPagosPorHora = manejoPagoPorHora ? 0 : 1,
                                          manejoHorasPor = (int)manejoPorHoras,
                                          manejoSalarioDiario = (int)manejoSalarioDiario
                                      });

                    if (uniqueResult)
                    {
                        mensajeResultado.noError = 0;
                        mensajeResultado.resultado = queryFinal.SingleOrDefault();
                    }
                    else
                    {
                        mensajeResultado.noError = 0;
                        mensajeResultado.resultado = queryFinal.ToList();
                    }
                    getSession().Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("buscaCFDIEmpleadosFiltrado()1_Error: ").Append(ex));
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                    mensajeResultado.resultado = null;
                    getSession().Database.CurrentTransaction.Rollback();
                }

            }

            return mensajeResultado;
        }

    }
}