using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exitosw.Payroll.Hibernate.entidad;
using System.Reflection;
using NHibernate;
using Exitosw.Payroll.Hibernate.modelo;
using Exitosw.Payroll.Hibernate.util;
using Exitosw.Payroll.Hibernate;
using System.Data.Common;
using NHibernate.Tool.hbm2ddl;
using Exitosw.Payroll.TestCompilador.funciones;
using NHibernate.Dialect;

namespace Exitosw.Payroll.Core.modelo
{
    public class ConsultaGenericaEspecialesHDAO : NHibernateRepository<Object>, ConsultaGenericaEspecialesHDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private Dictionary<String, String[]> aliasTablaExtras;
        private int contTablaOuter;
        List<String> ordenTablasOuter;
        StringBuilder consulta;
        IQuery q;

        public Mensaje existeClave(string identificador, string[] campowhere, object[] valoreswhere, ISession sessionSimple, ISession sessionSimpleMaster)
        {
            throw new NotImplementedException();
        }

        public Mensaje consultaPorRangosFiltro(string identificador, int inicio, int rango, string[] camposWhere, object[] valoresWhere, ISession sessionSimple, ISession sessionSimpleMaster)
        {
            return consultaGenericaPorRangos(identificador, inicio, rango, camposWhere, valoresWhere, sessionSimple, sessionSimpleMaster);
        }

        private Mensaje consultaGenericaPorRangos(String identificador, int inicio, int rango, String[] camposWhere, Object[] valoresWhere, ISession sessionSimple, ISession sessionSimpleMaster)
        {
            return generarQuery(identificador, inicio, rango, camposWhere, valoresWhere, false, sessionSimple, sessionSimpleMaster);

        }

        private Mensaje generarQuery(String identificador, int inicio, int rango, String[] camposWhere, Object[] valoresWhere, bool uniqueResult, ISession sessionSimple, ISession sessionSimpleMaster)
        {
            int i = -1;
            try
            {
                inicializaVariableMensaje();
                mensajeResultado = ObtenerQueryPerzonalizado(identificador, camposWhere, valoresWhere, sessionSimple, sessionSimpleMaster);
                if (mensajeResultado.noError == 0)
                {
                    List<object> Objetos = (List<object>)mensajeResultado.resultado;
                    consulta = new StringBuilder(Objetos[0].ToString());
                    camposWhere = (string[])Objetos[1];
                    getSession().BeginTransaction();
                    q = getSession().CreateQuery(consulta.ToString());
                    if (camposWhere != null)
                    {
                        for (i = 0; i < camposWhere.Length; i++)
                        {
                            if (string.Equals(identificador, "QueryEmpleadoEspecial", StringComparison.OrdinalIgnoreCase)
                                || string.Equals(identificador, "QueryEmpleadoEspecialMovimientosNomina", StringComparison.OrdinalIgnoreCase))
                            {
                                if (valoresWhere[i] != null)
                                {//caso especial por los finiquitos-liquidacion pueda ser que la corrida venga NULL
                                    q.SetParameter(string.Concat("parametros", Convert.ToString(i)), valoresWhere[i]);
                                }
                            }
                            else if (string.Equals(identificador, "QueryEmpleadoFiniquito", StringComparison.OrdinalIgnoreCase))
                            {
                                if (valoresWhere[i] != null)
                                {
                                    q.SetParameter(string.Concat("parametros", Convert.ToString(i)), valoresWhere[i]);
                                }
                            }
                            else
                            {
                                q.SetParameter(string.Concat("parametros", Convert.ToString(i)), valoresWhere[i]);
                            }
                        }
                    }

                    if (inicio >= 0 && rango > 0)
                    {
                        q.SetFirstResult(inicio);
                        q.SetMaxResults(rango);
                    }

                    if (uniqueResult)
                    {
                        mensajeResultado.resultado = q.UniqueResult();

                    }
                    else
                    {
                        mensajeResultado.resultado = q.List();
                    }
                }

            }
            catch (HibernateException ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generarQuery()1_Error: ").Append(ex));
                try
                {
                    if (getSession().Transaction.IsActive)
                    {
                        getSession().Transaction.Rollback();
                    }
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                }
                catch (HibernateException exx)
                {

                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(exx);
                    mensajeResultado.error = exx.GetBaseException().ToString();
                }
                mensajeResultado.resultado = null;


            }
            return mensajeResultado;
        }

        private Mensaje ObtenerQueryPerzonalizado(String identificador, String[] camwhere, Object[] valoresWhere, ISession sessionSimple, ISession sessionSimpleMaster)
        {
            List<Object> camposWhere = new List<Object>();
            int i = -1;
            String nombreTabla = "";
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
            try
            {
                if (string.Equals(identificador, "QueryEmpleadoFiniquito", StringComparison.OrdinalIgnoreCase))
                {
                    #region QueryEmpleadoFiniquito
                    consulta = new StringBuilder("select pe.empleados from FiniqLiquidPlazas fp RIGHT OUTER JOIN fp.finiquitosLiquida fin RIGHT OUTER JOIN fp.plazasPorEmpleado pe  RIGHT OUTER JOIN pe.empleados emp RIGHT OUTER JOIN pe.razonesSociales rs ");
                    consulta.Append("where ");
                    if (camwhere == null ? false : camwhere.Length > 0)
                    {
                        consulta.Append("emp.clave=:parametros").Append(++i).Append(" and");

                    }
                    int valwer = 0;
                    if (valoresWhere == null ? false : valoresWhere.Length > 0)
                    {
                        consulta.Append(" ").Append(" rs.clave = :parametros").Append(++i);
                        valwer++;
                    }

                    if (valoresWhere == null ? false : valoresWhere.Length > 0)
                    {
                        int val = ++i;
                        String comp = " != ";
                        valwer++;
                        //                    if (valoresWhere[val] == ModoBaja.NORMAL) {
                        //                        comp = " = ";
                        //                    } else {
                        valoresWhere[val] = Entity.entidad.ModoBaja.NORMAL;
                        ////                    }
                        consulta.Append(" ").Append("and (fin.modoBaja").Append(comp).Append(":parametros").Append(val).Append(" or fp is null)");
                    }
                    consulta.Append(" order by pe.empleados.clave");
                    ///agregar otra razon social para probar busqueda
                    camposWhere.Add(consulta.ToString());
                    if (valoresWhere == null)
                    {
                        camposWhere.Add(null);
                    }
                    else
                    {
                        camposWhere.Add(new String[valwer + (camwhere == null ? 0 : camwhere.Length)]);
                    }
                    #endregion
                }
                else if (string.Equals(identificador, "QueryEmpleadoEspecial", StringComparison.OrdinalIgnoreCase)
                  || string.Equals(identificador, "QueryEmpleadoEspecialMovimientosNomina", StringComparison.OrdinalIgnoreCase))
                {
                    #region QueryEmpleadoEspecial y  QueryEmpleadoEspecialMovimientosNomina 
                    consulta = new StringBuilder();
                    consulta.Append(string.Equals(identificador, "QueryEmpleadoEspecial", StringComparison.OrdinalIgnoreCase) ?
                        "select o.tipoNomina,o.plazasPorEmpleado.empleados"
                        : "select o, o.plazasPorEmpleado.empleados");
                    consulta.Append(" from PlazasPorEmpleadosMov o where o.id IN ( ");
                    consulta.Append(" Select MAX(m.id) from PlazasPorEmpleadosMov m ");
                    consulta.Append(" where ");
                    if (camwhere == null ? false : camwhere.Length > 0)
                    {
                        List<String> lstDatosGlobal = new List<String>();
                        String campoGlobal = "";
                        consulta.Append(" ( ");
                        for (int j = 0; j < camwhere.Length; j++)
                        {
                            String[] y = null;
                            if (camwhere.Contains("#"))
                            {
                                string x = camwhere[j].Substring(camwhere[j].IndexOf("#"));
                                camwhere[j] = camwhere[j].Substring(0, camwhere[j].IndexOf("#"));
                                if (x[0] == '#')
                                {
                                    x = x.Replace("#", "");
                                    y = x.Split('|');
                                }
                            }
                            else
                            {
                                if (camwhere[j].Split(',').Length == 1)
                                {
                                    campoGlobal = string.Concat("o.", camwhere[j]);
                                }
                            }

                            if (camwhere[j].Split(',').Length > 1)
                            {
                                consulta.Append(construyeConcatString(" m.", camwhere[j]));
                            }
                            else
                            {
                                consulta.Append(" m.").Append(camwhere[j]);
                            }


                            if (y != null)
                            {
                                if (y[0].Trim().Any())
                                {
                                    consulta.Append(" " + y[0] + " ");
                                }
                                else
                                {
                                    consulta.Append(" = ");
                                }
                            }
                            else
                            {
                                consulta.Append(" = ");
                            }

                            consulta.Append(":parametros").Append(++i);
                            if (campoGlobal.Length > 0)
                            {
                                //  lstDatosGlobal.Add(campoGlobal.concat("=:parametros").concat(String.valueOf(i)));
                                lstDatosGlobal.Add(string.Concat(campoGlobal, string.Concat("=:parametros", Convert.ToString(i))));
                            }
                            if (j < camwhere.Length - 1)
                            {
                                if (y != null)
                                {
                                    if (y.Length > 2)
                                    {
                                        if (y != null)
                                        {
                                            //consulta.Append(y[2].Trim().equalsIgnoreCase("AND") ? " AND " : " or ");
                                            consulta.Append(string.Equals(y[2].Trim(), "AND", StringComparison.OrdinalIgnoreCase) ? " AND " : " or ");
                                        }
                                        else
                                        {
                                            consulta.Append(" AND ");
                                        }
                                    }
                                    else
                                    {
                                        consulta.Append(" AND ");
                                    }
                                }
                                else
                                {
                                    consulta.Append(" AND ");
                                }
                            }

                        }
                        consulta.Append(" ) ");
                        consulta.Append(" AND ");
                        for (int j = 0; j < lstDatosGlobal.Count(); j++)
                        {
                            consulta.Append(lstDatosGlobal[j]).Append(" AND ");
                        }
                    }
                    consulta.Append(" m.plazasPorEmpleado.razonesSociales.clave =:parametros").Append(++i);
                    bool aplicarRestriccionEmpleadoDadoBaja = true;
                    i++;
                    if (valoresWhere[i] != null)
                    {
                        if (valoresWhere[i].GetType() == typeof(TipoCorrida))
                        {

                            if (string.Equals(((TipoCorrida)valoresWhere[i]).clave, "FIN", StringComparison.OrdinalIgnoreCase) || string.Equals(((TipoCorrida)valoresWhere[i]).clave, "LIQ", StringComparison.OrdinalIgnoreCase))
                            {
                                aplicarRestriccionEmpleadoDadoBaja = false;
                            }
                        }
                        consulta.Append(" AND :parametros").Append(i).Append("= :parametros").Append(i);
                    }

                    int parametroFechaInicial = ++i, parametroFechaFinal = ++i;
                    if (aplicarRestriccionEmpleadoDadoBaja)
                    {
                        consulta.Append(" AND ( ( m.fechaInicial <= :parametros").Append(parametroFechaInicial);
                        consulta.Append(" ) OR ( m.fechaInicial  between :parametros").Append(parametroFechaInicial);
                        consulta.Append(" AND :parametros").Append(parametroFechaFinal).Append(" ) ) ");
                        consulta.Append(" AND ( ( m.plazasPorEmpleado.fechaFinal >= :parametros").Append(parametroFechaFinal).Append(" ) ");
                        consulta.Append(" OR (m.plazasPorEmpleado.fechaFinal  between :parametros").Append(parametroFechaInicial);
                        consulta.Append(" AND :parametros").Append(parametroFechaFinal).Append(" ) ) ");
                    }
                    else
                    {
                        consulta.Append(" AND :parametros").Append(parametroFechaInicial).Append("= :parametros").Append(parametroFechaInicial);
                        consulta.Append(" AND :parametros").Append(parametroFechaFinal).Append("= :parametros").Append(parametroFechaFinal);
                    }

                    consulta.Append(" group by m.plazasPorEmpleado.empleados.clave ) ");
                    consulta.Append(" order by o.plazasPorEmpleado.empleados.clave, o.fechaInicial ");
                    camposWhere.Add(consulta.ToString());
                    if (camwhere != null)
                    {//JSA03
                        camposWhere.Add(new String[4 + camwhere.Length]);
                    }
                    else
                    {
                        camposWhere.Add(new String[4]);
                    }
                    nombreTabla = "PlazasPorEmpleadosMov";
                    #endregion
                }
                else if (string.Equals(identificador, "queryPlazasEmpleadoEspecial", StringComparison.OrdinalIgnoreCase))
                {
                    #region queryPlazasEmpleadoEspecial
                    nombreTabla = typeof(Parametros).Name;
                    if (existeTablaMEFMaster(sessionSimpleMaster, nombreTabla))
                    {
                        setSession(sessionSimpleMaster);
                    }
                    else
                    {
                        setSession(sessionSimple);
                    }
                    getSession().BeginTransaction();
                    StringBuilder strQuery = new StringBuilder();
                    #region manejaPagosPorHora
                    strQuery.Append("SELECT cr.valor FROM Cruce cr ");
                    strQuery.Append(" INNER JOIN cr.parametros pr ");
                    strQuery.Append(" INNER JOIN cr.elementosaplicacion ea ");
                    strQuery.Append(" WHERE pr.clave = :parametro ");
                    strQuery.Append(" and ea.clave = :elementoAplicacion ");
                    strQuery.Append(" and cr.claveElemento = :claveElemento");
                    bool manejaPagosPorHora;
                    Entity.entidad.ManejoHorasPor manejoHorasPor = Entity.entidad.ManejoHorasPor.HSM;
                    Entity.entidad.ManejoSalarioDiario manejoSalarioDiario = Entity.entidad.ManejoSalarioDiario.QUINCENAL;
                    String valor = (String)ejecutaQueryUnico(strQuery.ToString(), new String[] { "parametro", "elementoAplicacion", "claveElemento" }, new Object[] { ClavesParametrosModulos.claveParametroPagosPorHora, ClavesParametrosModulos.claveElementoAplicacionRazonSocial, valoresWhere[valoresWhere.Length - 1] });

                    valor = (valor == null ? "" : valor);
                    if (!valor.Any())
                    {
                        strQuery.Remove(0, strQuery.Length).Append("SELECT pr.valor ");
                        strQuery.Append(" FROM Parametros pr ");
                        strQuery.Append(" INNER JOIN pr.modulo m ");
                        strQuery.Append(" WHERE pr.clave = :parametro ");
                        strQuery.Append(" AND m.clave =:modulo");
                        valor = (String)ejecutaQueryUnico(strQuery.ToString(), new String[] { "parametro", "modulo" }, new Object[] { ClavesParametrosModulos.claveParametroPagosPorHora, ClavesParametrosModulos.claveModuloGlobal });
                    }

                    if (valor.Equals(ClavesParametrosModulos.opcionParametroPagarPorHoras))
                    {
                        manejaPagosPorHora = true;
                    }
                    else
                    {
                        manejaPagosPorHora = false;
                    }
                    #endregion
                    #region manejoHorasPor
                    strQuery.Remove(0, strQuery.Length).Append("SELECT cr.valor ");
                    strQuery.Append(" FROM Cruce cr ");
                    strQuery.Append(" INNER JOIN cr.parametros pr ");
                    strQuery.Append(" INNER JOIN cr.elementosaplicacion ea ");
                    strQuery.Append(" WHERE pr.clave = :parametro ");
                    strQuery.Append(" and ea.clave = :elementoAplicacion ");
                    strQuery.Append(" and cr.claveElemento = :claveElemento");
                    valor = (String)ejecutaQueryUnico(strQuery.ToString(), new String[] { "parametro", "elementoAplicacion", "claveElemento" }, new Object[] { ClavesParametrosModulos.claveParametroManejarHorasPor, ClavesParametrosModulos.claveElementoAplicacionRazonSocial, valoresWhere[valoresWhere.Length - 1] });
                    valor = (valor == null ? "" : valor);
                    if (!valor.Any())
                    {
                        strQuery.Remove(0, strQuery.Length).Append("SELECT pr.valor ");
                        strQuery.Append(" FROM Parametros pr");
                        strQuery.Append(" INNER JOIN pr.modulo m ");
                        strQuery.Append(" WHERE pr.clave = :parametro ");
                        strQuery.Append(" AND m.clave =:modulo");
                        valor = (String)ejecutaQueryUnico(strQuery.ToString(), new String[] { "parametro", "modulo" }, new Object[] { ClavesParametrosModulos.claveParametroManejarHorasPor, ClavesParametrosModulos.claveModuloGlobal });
                    }
                    if (valor.Equals(ClavesParametrosModulos.opcionParametroHorasNaturales))
                    {
                        manejoHorasPor = Entity.entidad.ManejoHorasPor.HORASNATURALES;
                    }
                    else
                    {
                        manejoHorasPor = Entity.entidad.ManejoHorasPor.HSM;
                    }
                    #endregion
                    #region manejoSalarioDiario
                    strQuery.Remove(0, strQuery.Length).Append("SELECT cr.valor ");
                    strQuery.Append(" FROM Cruce cr ");
                    strQuery.Append(" INNER JOIN cr.parametros pr ");
                    strQuery.Append(" INNER JOIN cr.elementosaplicacion ea ");
                    strQuery.Append(" WHERE pr.clave = :parametro ");
                    strQuery.Append(" and ea.clave = :elementoAplicacion ");
                    strQuery.Append(" and cr.claveElemento = :claveElemento");
                    valor = (String)ejecutaQueryUnico(strQuery.ToString(), new String[] { "parametro", "elementoAplicacion", "claveElemento" }, new Object[] { ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor, ClavesParametrosModulos.claveElementoAplicacionRazonSocial, valoresWhere[valoresWhere.Length - 1] });
                    valor = (valor == null ? "" : valor);
                    if (!valor.Any())
                    {
                        strQuery.Remove(0, strQuery.Length).Append("SELECT pr.valor ");
                        strQuery.Append(" FROM Parametros pr");
                        strQuery.Append(" INNER JOIN pr.modulo m ");
                        strQuery.Append(" WHERE pr.clave = :parametro ");
                        strQuery.Append(" AND m.clave =:modulo");
                        valor = (String)ejecutaQueryUnico(strQuery.ToString(), new String[] { "parametro", "modulo" }, new Object[] { ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor, ClavesParametrosModulos.claveModuloGlobal });
                    }
                    if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioDiario))
                    {
                        manejoSalarioDiario = Entity.entidad.ManejoSalarioDiario.DIARIO;
                    }
                    else if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioSemanal))
                    {
                        manejoSalarioDiario = Entity.entidad.ManejoSalarioDiario.SEMANAL;
                    }
                    else if (valor.Equals(ClavesParametrosModulos.opcionParametroSalarioQuincenal))
                    {
                        manejoSalarioDiario = Entity.entidad.ManejoSalarioDiario.QUINCENAL;
                    }
                    else
                    {
                        manejoSalarioDiario = Entity.entidad.ManejoSalarioDiario.MENSUAL;
                    }
                    #endregion

                    if (!getSession().Transaction.WasRolledBack)
                    {
                        try
                        {
                            if (getSession().Transaction.IsActive)
                            {
                                getSession().Transaction.Rollback();
                            }
                        }
                        catch (HibernateException exc)
                        { //cuando marca error al hacer rollback pro ejemplo se cae conexion servidor
                            mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(exc);
                            mensajeResultado.error = exc.GetBaseException().ToString();
                            mensajeResultado.resultado = null;
                            return mensajeResultado;
                        }
                    }

                    nombreTabla = typeof(DatosPlazasEmpleado).Name;
                    consulta = new StringBuilder("SELECT new DatosPlazasEmpleado( ");
                    consulta.Append("p, ");
                    consulta.Append("(SELECT ingresos.fechaIngreso FROM IngresosReingresosBajas ingresos WHERE ingresos.id = ");
                    consulta.Append("(SELECT MAX(o1.id) FROM IngresosReingresosBajas o1 Where  o1.fechaIngreso <= ");
                    consulta.Append("(SELECT MAX(o2.fechaIngreso) FROM IngresosReingresosBajas o2 Where o2.fechaBaja >= current_date() ");
                    consulta.Append("and o2.empleados.clave = p.plazasPorEmpleado.empleados.clave))) ");
                    consulta.Append(", ").Append(manejaPagosPorHora ? 0 : 1);
                    consulta.Append(", ").Append(manejoHorasPor);
                    consulta.Append(", ").Append(manejoSalarioDiario);
                    consulta.Append(" ) ");
                    consulta.Append("from PlazasPorEmpleadosMov p ");
                    consulta.Append(" where ");
                    if (valoresWhere == null ? false : (valoresWhere.Length > 1 ? true : false))
                    {
                        for (int j = 0; j < camwhere.Length; j++)
                        {
                            String[] y = null;
                            if (camwhere[j].Contains("#"))
                            {
                                String x = camwhere[j].Substring(camwhere[j].IndexOf("#"));
                                camwhere[j] = camwhere[j].Substring(0, camwhere[j].IndexOf("#"));
                                if (x[0] == '#' & x[x.Length - 1] == '#')
                                {
                                    x = x.Replace("#", "").Trim();
                                    y = x.Split('|');
                                    if (y[0].Trim().Any())
                                    {
                                        consulta.Append(" not ");
                                    }
                                }
                            }

                            //ClaveEmpleado,Nombre,ClavePlazaEmpleado,FechaInicial,FechaFinal,FechaIngreso,FechaFinUltReingreso,Horas,
                            //Importe,DescripcionCentroCosto,DescripcionPuesto,ClavePlaza
                            String texto = camwhere[j].Trim();
                            if (string.Equals(texto, "ClaveEmpleado", StringComparison.OrdinalIgnoreCase))
                            {
                                consulta.Append("p.plazasPorEmpleado.empleados.clave");
                            }
                            else if (texto.ToLower().Contains("nombre".ToLower()))
                            {
                                texto = "plazasPorEmpleado.empleados." + texto;
                                texto = texto.Replace(",", ",plazasPorEmpleado.empleados.");
                                //    consulta.append("p.plazasPorEmpleado.empleados.nombre");
                                consulta.Append(construyeConcatString(" p.", texto));
                            }
                            else if (string.Equals(texto, "ClavePlazaEmpleado", StringComparison.OrdinalIgnoreCase))
                            {
                                consulta.Append("p.plazasPorEmpleado.clave");
                            }
                            else if (string.Equals(texto, "FechaInicial", StringComparison.OrdinalIgnoreCase))
                            {
                                consulta.Append("p.fechaInicial");
                            }
                            else if (string.Equals(texto, "FechaFinal", StringComparison.OrdinalIgnoreCase))
                            {
                                consulta.Append("p.plazasPorEmpleado.fechaFinal");
                            }
                            else if (string.Equals(texto, "FechaIngreso", StringComparison.OrdinalIgnoreCase))
                            {
                                //consulta.append("p.plazasPorEmpleado.empleados.nombre");
                            }
                            else if (string.Equals(texto, "FechaFinUltReingreso", StringComparison.OrdinalIgnoreCase))
                            {
                                //consulta.append("p.plazasPorEmpleado.empleados.nombre");
                            }
                            else if (string.Equals(texto, "Horas", StringComparison.OrdinalIgnoreCase))
                            {
                                consulta.Append("p.horas");
                            }
                            else if (string.Equals(texto, "Importe", StringComparison.OrdinalIgnoreCase))
                            {
                                if (valoresWhere[j] != null)
                                {
                                    if (valoresWhere[j].GetType() == typeof(Double) | valoresWhere[j].GetType() == typeof(double))
                                    {
                                        valoresWhere[j] = ((Double)valoresWhere[j]);
                                    }
                                }
                                consulta.Append("p.importe");
                            }
                            else if (string.Equals(texto, "DescripcionCentroCosto", StringComparison.OrdinalIgnoreCase))
                            {
                                consulta.Append("p.centroDeCosto.descripcion");
                            }
                            else if (string.Equals(texto, "DescripcionPuesto", StringComparison.OrdinalIgnoreCase))
                            {
                                consulta.Append("p.puestos.descripcion");
                            }
                            else if (string.Equals(texto, "ClavePlaza", StringComparison.OrdinalIgnoreCase))
                            {
                                consulta.Append("p.plazas.clave");
                            }
                            if (y != null)
                            {
                                if (y[1].Trim().Any())
                                {
                                    consulta.Append(" like ");
                                }
                                else
                                {
                                    consulta.Append(" = ");
                                }
                            }
                            else
                            {
                                consulta.Append(" = ");
                            }
                            consulta.Append(":parametros").Append(++i);
                            if (j < camwhere.Length - 1)
                            {
                                if (y != null)
                                {
                                    if (y.Length > 2)
                                    {
                                        if (y != null)
                                        {
                                            //consulta.Append(y[2].trim().equalsIgnoreCase("AND") ? " AND " : " or ");
                                            consulta.Append(string.Equals(y[2].Trim(), "AND", StringComparison.OrdinalIgnoreCase) ? " AND " : " or ");
                                        }
                                        else
                                        {
                                            consulta.Append(" AND ");
                                        }
                                    }
                                    else
                                    {
                                        consulta.Append(" AND ");
                                    }
                                }
                                else
                                {
                                    consulta.Append(" AND ");
                                }
                            }
                        }
                        consulta.Append(" and ");
                    }

                    consulta.Append(" p.id IN ( ");
                    consulta.Append(" SELECT MAX(m.id) FROM PlazasPorEmpleadosMov m WHERE  m.plazasPorEmpleado.razonesSociales.clave = :parametros").Append(++i);
                    consulta.Append(" AND current_date()  BETWEEN  m.fechaInicial AND m.plazasPorEmpleado.fechaFinal + 1 ");
                    consulta.Append(" GROUP BY m.plazasPorEmpleado.clave ) ");
                    consulta.Append(" ORDER BY p.plazasPorEmpleado.empleados.clave, p.fechaInicial");
                    camposWhere.Add(consulta.ToString());
                    if (camwhere != null)
                    {
                        camposWhere.Add(new String[1 + camwhere.Length]);
                    }
                    else
                    {
                        camposWhere.Add(new String[1]);
                    }
                    #endregion
                }

                consulta = null;
                if (existeTablaMEFMaster(sessionSimpleMaster, nombreTabla))
                {
                    setSession(sessionSimpleMaster);
                }
                else
                {
                    setSession(sessionSimple);
                }
            }
            catch (HibernateException ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ObtenerQueryPerzonalizado()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                return mensajeResultado;
            }

            mensajeResultado.resultado = camposWhere;
            return mensajeResultado;
        }

        public Mensaje obtenerDatosCriterioConsulta(string[] tablas, string[] camposMostrar, string[] camposWhere, object[] valoresWhere, string[] camposOrden, ISession sessionSimple)
        {
            throw new NotImplementedException();
        }

        private String construyeConcatString(String alias, String campo)
        {
            String[] campos = campo.Split(',');
            if (campos.Length > 0)
            {
                StringBuilder sb = new StringBuilder(" CONCAT(");
                int i;
                for (i = 0; i < campos.Length; i++)
                {
                    sb.Append("CASE WHEN (").Append(alias).Append(campos[i]).Append(" IS NULL) THEN '' ELSE ").Append(campos[i]).Append(" END").Append("@");
                }
                campo = sb.ToString().Substring(0, sb.Length - 1);
                return string.Concat(campo.Replace("@", ",' ',"), ")");// campo.Replace("@", ",' ',").Concat(")");
            }
            return "";
        }

        private Boolean existeTablaMEFMaster(ISession sesion, String tabla)
        {//JSA01

            Boolean existeTabla = false;
            Boolean continuar = true;

            var a = sesion.GetSessionImplementation().Factory.OpenSession();
            DatabaseMetadata meta = new DatabaseMetadata((DbConnection)a.Connection, new NHibernate.Dialect.MsSql2012Dialect());
            if (meta.IsTable(tabla))
            {
                existeTabla = true;
            }
            return existeTabla;
        }

        private Object ejecutaQueryUnico(String strQuery, String[] camposParametro, Object[] valoresParametro)
        {
            Object result = null;
            try
            {
                IQuery query = getSession().CreateQuery(strQuery);
                int i;
                if (camposParametro != null & valoresParametro != null)
                {
                    for (i = 0; i < camposParametro.Length; i++)
                    {
                        query.SetParameter(camposParametro[i], valoresParametro[i]);
                    }
                }
                query.SetMaxResults(1);//JSA02
                result = query.UniqueResult();
            }
            catch (HibernateException ex)
            {
                //            mensajeRespuesta.setError(ex.getMessage());
                //            mensajeRespuesta.setNoError(-100);
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ejecutaQueryUnico()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            catch (Exception e)
            {
                //            mensajeRespuesta.setError(e.getMessage());
                //            mensajeRespuesta.setNoError(-100);
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("ejecutaQueryUnico()1_Error: ").Append(e));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(e);
                mensajeResultado.error = e.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return result;
        }

        public Mensaje consultaPorFiltros(string query, object[] campos, object[] valores, int inicio, int rango, ISession sessionSimple, ISession sessionSimpleMaster)
        {
            try
            {
                setSession(sessionSimple);
                IQuery q = getSession().CreateQuery(query);
                for (int i = 0; i < campos.Length; i++)
                {
                    //if (valores[i].GetType() == typeof(object[])) {
                    q.SetParameter("parametro"+i, valores[i]);
                    //}
                }
                if (inicio >= 0 && rango > 0)
                {
                    q.SetFirstResult(inicio);
                    q.SetMaxResults(rango);
                }

                mensajeResultado.resultado = q.List();
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

            }
            catch (HibernateException ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltros()1_Error: ").Append(ex));
                try
                {
                    if (getSession().Transaction.IsActive)
                    {
                        getSession().Transaction.Rollback();
                    }
                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                    mensajeResultado.error = ex.GetBaseException().ToString();
                }
                catch (HibernateException exx)
                {

                    mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(exx);
                    mensajeResultado.error = exx.GetBaseException().ToString();
                }
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }
    }
}
