using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using Exitosw.Payroll.Hibernate.util;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modeloHB
{
    public class MovimientosNominaHDAO : NHibernateRepository<MovNomConcep>, MovimientosNominaHDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private String[] camposConsulta = new String[]{"razonesSociales", "empleado",
        "tipoNomina", "periodosNomina", "tipoCorrida", "concepNomDefi", "numero", "ejercicio", "mes", "uso"};
        //plazasporempleado--nop
        //uso,mes,ejercicio
        private StringBuilder strQuery = new StringBuilder();

        public Mensaje buscaMovimientosNominaFiltrado(List<object> valoresDeFiltrado, ISession uuidCxn)
        {
            throw new NotImplementedException();
        }

        public Mensaje eliminaListaMovimientos(string campo, object[] valores, List<CFDIEmpleado> valoresCFDI, object[] valoresCalculoUnidades, object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, ISession uuidCxn)
        {
            throw new NotImplementedException();
        }

        public Mensaje getCalculosUnidadesPorFiltroEspecifico(string[] camposWhere, object[] valoresWhere, List<CFDIEmpleado> listCFDIEmpleado, ISession uuidCxn)
        {
            throw new NotImplementedException();
        }

        public Mensaje getMaxNumeroMovimientoPorTipoNominaYPeriodo(string claveTipoNomina, decimal idPeriodo, ISession uuidCxn)
        {
            throw new NotImplementedException();
        }

        public Mensaje getMovimientosNominaAll(ISession uuidCxn)
        {
            throw new NotImplementedException();
        }

        public Mensaje getMovimientosNominaAsc(ISession uuidCxn)
        {
            throw new NotImplementedException();
        }

        public Mensaje getMovimientosPorFiltro(string[] camposWhere, object[] valoresWhere, ISession uuidCxn)
        {
            throw new NotImplementedException();
        }

        public Mensaje getMovimientosPorFiltroEspecifico(string[] camposWhere, object[] valoresWhere, ISession uuidCxn)
        {
            throw new NotImplementedException();
        }

        public Mensaje getMovimientosPorPlazaEmpleado(object[] clavesPlazasPorEmpleado, string claveTipoCorrida, string claveRazonSocial, ISession uuidCxn)
        {
            throw new NotImplementedException();
        }

        public Mensaje saveDeleteMovimientosNomina(List<MovNomConcep> AgreModif, object[] clavesDelete, object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, ISession uuidCxn)
        {
            throw new NotImplementedException();
        }

        public int deleteListQueryMov(String tabla, String campo, Object[] valores, List<CFDIEmpleado> valoresCFDI, Object[] valoresCalculoUnidades, Object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100)
        {
            StringBuilder consulta = new StringBuilder();
            int exito = 0;
            try
            {
                //Deshace el movimiento del credito o ahorro
                consulta.Append("select distinct  credMov from CreditoMovimientos credMov inner join credMov.movNomConceps o where o.id in(:valores)");

                IQuery q = getSession().CreateQuery(consulta.ToString());

                if (valoresReestablecer == null ? false : valoresReestablecer.Length > 0)
                {
                    List<Object> valoresx = new List<Object>();
                    valoresx.AddRange(valores.ToList());
                    valoresx.AddRange(valoresReestablecer.ToList());
                    q.SetParameterList("valores", valoresx.ToArray());
                }
                else
                {
                    q.SetParameterList("valores", valores);
                }

                IList<CreditoMovimientos> listCreditoMovimientos2 = q.List<CreditoMovimientos>();
                List<CreditoMovimientos> listCreditoMovimientos = (List<CreditoMovimientos>)listCreditoMovimientos2;
                //Elimina CreditoMovimientos y reestablece los importe.
                if (listCreditoMovimientos == null ? false : !listCreditoMovimientos.Any() ? false : true)
                {
                    consulta.Remove(0, consulta.Length);
                    consulta.Append("delete ");
                    consulta.Append("CreditoMovimientos o where o.id = :valor");
                    int i, j, k;
                    for (i = 0; i < listCreditoMovimientos.Count; i++)
                    {
                        getSession().SaveOrUpdate(listCreditoMovimientos[i].creditoPorEmpleado);
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

                                getSession().SaveOrUpdate(listCreditoMovimientos[i].movNomConcep[j]);
                                if (listCreditoMovimientos[i].movNomConcep[j].tipoPantalla.Equals(100) || incluirEliminadoDiferenteTipoPantalla100)
                                {
                                    getSession().Delete(listCreditoMovimientos[i].movNomConcep[j]);
                                    movimientosEliminados++;
                                }
                            }
                            List<MovNomConcep> m = (List<MovNomConcep>)listCreditoMovimientos[i].movNomConcep;
                            m.RemoveAll(mov => mov.id == Convert.ToDecimal(listCreditoMovimientos[i].movNomConcep.Select(p => p.id)));
                            listCreditoMovimientos[i].movNomConcep = m;
                            //listCreditoMovimientos[i].movNomConcep.RemoveAll(mov => mov.id == Convert.ToDecimal(listCreditoMovimientos[i].movNomConcep.Select(p => p.id)));

                            getSession().Delete(listCreditoMovimientos[i]);
                        }
                    }

                }

                consulta.Remove(0, consulta.Length);
                consulta.Append("delete ");
                if (valores.Length > 0)
                {
                    //Elimina Bases Afectadas de Movimientos por Conceptos
                    consulta.Append("MovNomBaseAfecta o where o.movNomConcep.id in(:valores)");
                    q = getSession().CreateQuery(consulta.ToString());
                    q.SetParameterList("valores", valores);
                    exito = q.ExecuteUpdate();
                    /////System.out.println("exito " + exito);
                    consulta.Remove(7, consulta.Length);

                    //Elimina Movimientos Por parametros de Movimientos por Conceptos
                    consulta.Append("MovNomConceParam o where o.movNomConcep.id in(:valores)");
                    q = getSession().CreateQuery(consulta.ToString());
                    q.SetParameterList("valores", valores);
                    exito = q.ExecuteUpdate();
                    /////System.out.println("exito " + exito);
                    consulta.Remove(7, consulta.Length);

                    //Elimina Movimientos ISRRetenidos
                    consulta.Append(typeof(CalculoISR).Name).Append(" o where o.movNomConcep.id in(:valores)");//JSA06
                    q = getSession().CreateQuery(consulta.ToString());
                    q.SetParameterList("valores", valores);
                    exito = q.ExecuteUpdate();
                    //////System.out.println("exito " + exito);
                    consulta.Remove(7, consulta.Length);

                    //Elimina Movimientos CalculoIMSS
                    consulta.Append(typeof(CalculoIMSS).Name).Append(" o where o.movNomConcep.id in(:valores)");
                    q = getSession().CreateQuery(consulta.ToString());
                    q.SetParameterList("valores", valores);
                    exito = q.ExecuteUpdate();
                    ///////System.out.println("exito " + exito);
                    consulta.Remove(7, consulta.Length);

                    //Elimina Movimientos CalculoIMSSPatron
                    consulta.Append(typeof(CalculoIMSSPatron).Name).Append(" o where o.movNomConcep.id in(:valores)");
                    q = getSession().CreateQuery(consulta.ToString());
                    q.SetParameterList("valores", valores);
                    exito = q.ExecuteUpdate();
                    /////////System.out.println("exito " + exito);
                    consulta.Remove(7, consulta.Length);

                    //Elimina Movimientos por Conceptos
                    consulta.Append(tabla).Append(" where ").Append(campo).Append(" in(:valores)");
                    q = getSession().CreateQuery(consulta.ToString());
                    q.SetParameterList("valores", valores);
                    exito = q.ExecuteUpdate();
                    //////////System.out.println("exito " + exito);
                    consulta.Remove(7, consulta.Length);
                }

                if (valoresReestablecer == null ? false : valoresReestablecer.Length > 0)
                {
                    //Reestructurar Movimientos por Conceptos
                    consulta.Remove(0, consulta.Length).Append("from ");
                    consulta.Append(tabla).Append(" where ").Append(campo).Append(" in(:valores)");
                    q = getSession().CreateQuery(consulta.ToString());
                    q.SetParameterList("valores", valoresReestablecer);
                    IList<MovNomConcep> listMovNomConcepReestablecer = q.List<MovNomConcep>();
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
                            //                        if (listMovNomConcepReestablecer.get(j).getMovNomConceParam() != null) {
                            //                            for (k = 0; k < listMovNomConcepReestablecer.get(j).getMovNomConceParam().size(); k++) {
                            //                                listMovNomConcepReestablecer.get(j).getMovNomConceParam().get(k).setValor(null);
                            //                            }
                            //                        }
                            getSession().SaveOrUpdate(listMovNomConcepReestablecer[j]);
                            if (listMovNomConcepReestablecer[j].tipoPantalla.Equals(100) || incluirEliminadoDiferenteTipoPantalla100)
                            {
                                getSession().Delete(listMovNomConcepReestablecer[j]);
                            }
                        }
                    }
                    consulta.Remove(0, consulta.Length);
                    consulta.Append("delete ");
                }

                if (valoresCalculoUnidades != null ? valoresCalculoUnidades.Length > 0 : false)
                {
                    //Elimina Calculo de unidades
                    consulta.Append(typeof(CalculoUnidades).Name).Append(" where ").Append(campo).Append(" in(:valores)");
                    q = getSession().CreateQuery(consulta.ToString());
                    q.SetParameterList("valores", valoresCalculoUnidades);
                    exito = q.ExecuteUpdate();
                    ///////System.out.println("exito " + exito);
                    consulta.Remove(7, consulta.Length);
                }

                if (valoresCFDI == null ? false : valoresCFDI.Any())
                {
                    for (int i = 0; i < valoresCFDI.Count; i++)
                    {
                        exito = deleteListQuery("CFDIEmpleado", "id", new Object[] { valoresCFDI[i].id });
                        consulta.Remove(0, consulta.Length);//CFDIReciboConcepto
                        consulta.Append("select cfdiCnc.id from CFDIRecibo o INNER JOIN o.cfdiReciboConceptos cfdiCnc where o.id= :idRecibo ");
                        q = getSession().CreateQuery(consulta.ToString());
                        if (valoresCFDI[i].cfdiRecibo.id > 0)
                        {
                            decimal idRecibo = valoresCFDI[i].cfdiRecibo.id;
                            q.SetParameter("idRecibo", idRecibo);
                            IList<Object[]> cfdiCnc = q.List<object[]>();
                            if (cfdiCnc.Any())
                            {
                                exito = deleteListQuery("CFDIReciboConcepto", "id", cfdiCnc.ToArray());
                                /////////////System.out.println("exito " + exito);
                            }
                            consulta.Remove(0, consulta.Length);//CFDIReciboIncapacidad
                            consulta.Append("select cfdiInc.id from CFDIRecibo o INNER JOIN o.cfdiReciboIncapacidades cfdiInc where o.id= :idRecibo ");
                            q = getSession().CreateQuery(consulta.ToString());
                            q.SetParameter("idRecibo", idRecibo);
                            IList<Object[]> cfdiInc = q.List<object[]>();
                            if (cfdiInc.Any())
                            {
                                exito = deleteListQuery("CFDIReciboIncapacidad", "id", cfdiInc.ToArray());
                                ///////////////System.out.println("exito " + exito);
                            }
                            consulta.Remove(0, consulta.Length);//CFDIReciboHrsExtras
                            consulta.Append("select cfdiHrs.id from CFDIRecibo o INNER JOIN o.cfdiReciboHrsExtras cfdiHrs  where o.id= :idRecibo ");
                            q = getSession().CreateQuery(consulta.ToString());
                            q.SetParameter("idRecibo", idRecibo);
                            IList<Object[]> cfdiHrs = q.List<object[]>();
                            if (cfdiHrs.Any())
                            {
                                exito = deleteListQuery("CFDIReciboHrsExtras", "id", cfdiHrs.ToArray());
                                /////////////System.out.println("exito " + exito);
                            }
                            exito = deleteListQuery("CFDIRecibo", "id", new Object[] { idRecibo });
                            ///////////System.out.println("exito " + exito);
                        }
                    }
                }

            }
            catch (HibernateException ex)
            {
                mensajeResultado.noError = (ControlErroresEntity.buscaNoErrorPorExcepcion(ex));
                mensajeResultado.error = (ex.GetBaseException().Message);
                mensajeResultado.resultado = null;
                exito = -1;
            }
            return exito;
        }
    }
}
