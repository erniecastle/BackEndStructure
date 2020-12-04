/**
 * @author: Ernesto Castillo 
 * Fecha de Creación: 11/04/2019
 * Compañía: Macropro
 * Descripción del programa: clase GenericQueriesDAO para llamados a metodos de Hibernate request
 * -----------------------------------------------------------------------------
 * MODIFICACIONES:
 * -----------------------------------------------------------------------------
 * Clave: 
 * Autor: 
 * Fecha:
 * Descripción:
 * -----------------------------------------------------------------------------
 */

using NHibernate;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Hibernate.util;
using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using System.Collections;
using System.Text.RegularExpressions;
using System.Configuration;
using Newtonsoft.Json;

namespace Exitosw.Payroll.Core.modelo
{
    public class GenericQueriesDAO : NHibernateRepository<Object>, GenericQueriesDAOIF
    {
        private string nameProject = ConfigurationManager.AppSettings["routeEntitiesEF"];
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<Object> listaCursos = new List<Object>();
        IQuery query;
        private StringBuilder queryFrom = new StringBuilder(200);
        public String funcionLength = "length";
        private int initFrom = -1;

        public Mensaje consultaPorFiltros(string fuentePrincipal, object[] tablasRelacionadas, object[] camposMostrar,
            object[] camposWhere, object[] valoresWhere, object[] camposGroup, object[] camposOrden, bool withCount,
            int? inicio, int? fin, bool activarAlias, string tipoOrden, ISession session)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(session);
                object[] dataQuery = new object[2];
                StringBuilder obtainedQuery = new StringBuilder();
                String genQuery = createGenericQuery(fuentePrincipal, tablasRelacionadas, camposMostrar, camposWhere, valoresWhere, activarAlias);
                string applyGroupByToQuery = genGroupByQuery(fuentePrincipal, tablasRelacionadas, camposGroup);
                string applyOrderToQuery = genOrderQuery(fuentePrincipal, tablasRelacionadas, camposOrden, tipoOrden);
                obtainedQuery.Append(genQuery).Append(" ").Append(applyGroupByToQuery).Append(" ").Append(applyOrderToQuery);
                query = getSession().CreateQuery(obtainedQuery.ToString());
                if (valoresWhere != null)
                {
                    if (valoresWhere != null && valoresWhere.Length > 0)
                    {
                        int i;
                        int cmpValWhere = valoresWhere.Length;
                        for (i = 0; i < cmpValWhere; i++)
                        {
                            bool noSetParamIfSubQuery = camposWhere[i].ToString().StartsWith("@") ?
                                camposWhere[i].ToString().StartsWith("@BETWEEN") ? true : false :
                                camposWhere[i].ToString().EndsWith("@") ? false : true;
                            if (noSetParamIfSubQuery)
                            {
                                if (valoresWhere[i] is Array)
                                {
                                    object[] parValues = (object[])valoresWhere[i];
                                    query.SetParameter(parValues[1].ToString(), parValues[2]);
                                    query.SetParameter(parValues[3].ToString(), parValues[4]);

                                }
                                else
                                {
                                    query.SetParameter("param" + i, valoresWhere[i]);
                                }

                                //query.SetParameter("param" + i, "%" + valoresWhere[i] + "%");
                            }
                        }
                    }
                }

                if (inicio != null && fin != null)
                {
                    int start = inicio ?? 0;
                    int end = fin ?? 0;

                    query.SetFirstResult(start);
                    query.SetMaxResults(end);
                }

                if (activarAlias)
                {
                    dataQuery[0] = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                }
                else
                {
                    dataQuery[0] = query.List<object>();
                }
                if (withCount)
                {
                    long count = getTotalRegisters(genQuery, camposWhere, valoresWhere);
                    dataQuery[1] = count;
                }
                obtainedQuery.Clear();
                mensajeResultado.resultado = dataQuery;
                //mensajeResultado.resultado = query.List<object>();
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("consultaPorFiltros()1_Error: ").Append(ex));
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

        public Mensaje obtenerClaveStringMax(String tabla, String campo, String[] camposWhere, object[] valoresCamposWhere, ISession session)
        {
            String valor;
            try
            {
                inicializaVariableMensaje();
                setSession(session);
                getSession().BeginTransaction();
                valor = claveStringMax(tabla, campo, camposWhere, valoresCamposWhere, session);
                mensajeResultado.resultado = valor;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Transaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("obtenerClaveStringMax()1_Error: ").Append(ex));
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

        public Mensaje existeClave(String tabla, String[] campos, Object[] valores, String queryAntesDeFrom, ISession session)
        {
            object valor;
            try
            {
                inicializaVariableMensaje();
                setSession(session);
                getSession().BeginTransaction();
                valor = queryExisteClave(tabla, campos, valores, null, session);
                if (valor == null)
                {
                    valor = false;
                }
                else
                {
                    valor = true;
                }
                mensajeResultado.resultado = valor;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Transaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existeClave()1_Error: ").Append(ex));
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

        /*Methods from generics*/
        public Object queryExisteClave(String tabla, String[] campo, Object[] valores, String queryAntesDeFrom, ISession session)
        {
            StringBuilder consulta = new StringBuilder();
            setSession(session);
            /*
             * ---@ Para SubConsultas ("from tabla t where t.id = 1") ---#Para
             * Indicar como deseas Filtrar (>,<,!=,IN) Ejemplos al usar campos de
             * filtrados: 1.- NombreCampo#!=,NombreCampo#>,NombreCampo#< 2.-
             * NombreCampo@ 3.- NombreCampo#IN@ */
            /*
             * ---@ Para SubConsultas ("from tabla t where t.id = 1") ---#Para
             * Indicar como deseas Filtrar (>,<,!=,IN) Ejemplos al usar campos de
             * filtrados: 1.- NombreCampo#!=,NombreCampo#>,NombreCampo#< 2.-
             * NombreCampo@ 3.- NombreCampo#IN@
             */
            if (queryAntesDeFrom != null)
            {
                consulta = new StringBuilder(queryAntesDeFrom).Append(" ").Append("from ");
            }
            else
            {
                consulta = new StringBuilder("from ");
            }
            consulta.Append(tabla).Append(" o where ");
            int i;
            String[] campos = null;
            for (i = 0; i < campo.Length; i++)
            {
                if (i > 0)
                {
                    consulta.Append(" and ");
                }

                if (campo[i].StartsWith("@"))
                {
                    campos = campo[i].Split('#');
                    if (campos.Length > 1)
                    {
                        consulta.Append("o.").Append(campos[0].Substring(1)).Append(" ").Append(campos[1]);
                    }
                    else
                    {
                        consulta.Append("o.").Append(campo[i].Substring(1)).Append(" = ");
                    }
                    consulta.Append(valores[i]);
                }
                else
                {
                    campos = campo[i].Split('#');
                    if (campos.Length > 1)
                    {
                        if (campos[1].StartsWith("IN"))
                        {
                            consulta.Append("o.").Append(campos[0]);
                            if (campos[1].Contains("@"))
                            {
                                consulta.Append(" ").Append(campos[1].Substring(0, campos[1].LastIndexOf("@"))).Append(" (").Append(valores[i]).Append(")");
                            }
                            else
                            {

                                consulta.Append(" ").Append(campos[1]).Append(" (:").Append("parametro").Append(Convert.ToString(i)).Append(")");
                            }
                        }
                        else
                        {
                            consulta.Append("o.").Append(campos[0]);
                            consulta.Append(" ").Append(campos[1]).Append(" :").Append("parametro" + Convert.ToString(i).ToString());
                        }
                    }
                    else
                    {
                        consulta.Append("o.").Append(campos[0]);
                        consulta.Append(" = :").Append("parametro" + Convert.ToString(i));
                    }
                }
            }
            query = getSession().CreateQuery(consulta.ToString());
            for (i = 0; i < campo.Length; i++)
            {
                if (!campo[i].Contains("@"))
                {
                    if (valores[i] is Object[])
                    {
                        query.SetParameterList("parametro" + Convert.ToString(i), (Object[])valores[i]);
                    }
                    else if (valores[i] is ArrayList)
                    {
                        query.SetParameterList("parametro" + Convert.ToString(i), ((ArrayList)valores[i]).ToArray());
                    }
                    else
                    {
                        query.SetParameter("parametro" + Convert.ToString(i), valores[i]);
                    }


                }
            }
            return query.UniqueResult();
        }

        private String claveStringMax(String tabla, String campo, String[] camposWhere, Object[] valoresCamposWhere, ISession session)
        {
            setSession(session);
            StringBuilder consulta = new StringBuilder();
            consulta = new StringBuilder("Select max(o.").Append(campo).Append(") from ");
            consulta.Append(tabla).Append(" o where ");
            int i;
            if (camposWhere != null)
            {
                consulta.Append(" o.");

                consulta.Append(camposWhere[0]);
                consulta.Append(" = :").Append((camposWhere[0].IndexOf('.') > -1) ? (camposWhere[0].Substring(0, camposWhere[0].IndexOf('.'))) + "_0" : camposWhere[0]);
                for (i = 1; i < camposWhere.Length; i++)
                {
                    consulta.Append(" and ");
                    consulta.Append("o.").Append(camposWhere[i]);
                    consulta.Append(" = :").Append((camposWhere[i].IndexOf('.') > -1) ? (camposWhere[i].Substring(0, camposWhere[i].IndexOf('.'))) + "_" + i : camposWhere[i]);
                }
                consulta.Append(" and ");
            }
            consulta.Append(" ").Append(funcionLength).Append("(o.").Append(campo).Append(") = (select max(").Append(funcionLength).Append("(oo.").Append(campo).Append(")) from ").Append(tabla).Append(" oo ");



            if (camposWhere != null)
            {
                consulta.Append(" where oo.");
                consulta.Append(camposWhere[0]);
                consulta.Append(" = :").Append((camposWhere[0].IndexOf('.') > -1) ? (camposWhere[0].Substring(0, camposWhere[0].IndexOf('.'))) + "_0" : camposWhere[0]);
                for (i = 1; i < camposWhere.Length; i++)
                {
                    consulta.Append(" and ");
                    consulta.Append("oo.").Append(camposWhere[i]);
                    consulta.Append(" = :").Append((camposWhere[i].IndexOf('.') > -1) ? (camposWhere[i].Substring(0, camposWhere[i].IndexOf('.'))) + "_" + i : camposWhere[i]);
                }
            }
            consulta.Append(")");

            query = getSession().CreateQuery(consulta.ToString());

            if (camposWhere != null)
            {
                for (i = 0; i < camposWhere.Length; i++)
                {
                    query.SetParameter((camposWhere[i].IndexOf('.') > -1) ? (camposWhere[i].Substring(0, camposWhere[i].IndexOf('.'))) + "_" + i : camposWhere[i], valoresCamposWhere[i]);
                }
            }
            return Convert.ToString(query.UniqueResult());
        }

        private String createGenericQuery(string fuentePrincipal, object[] tablasRelacionadas, object[] camposMostrar, object[] camposWhere, object[] valoresWhere, bool activarAlias)
        {
            StringBuilder builder = new StringBuilder();
            if (camposMostrar != null || camposMostrar.Length > 0)
            {
                builder.Append("Select ");
                if (camposMostrar.Length < 1)
                {
                    builder.Append("o ");
                }
            }

            int i;
            int endFieldShow;
            int cmpMostrar = camposMostrar.Length;

            //Fields to show
            for (i = 0; i < cmpMostrar; i++)
            {
                Type typeField = null;
                bool reservedWord = false;

                if (camposMostrar[i].ToString().Contains("."))
                {
                    if (!camposMostrar[i].ToString().StartsWith("@"))
                    {
                        if (camposMostrar[i].ToString().StartsWith("Date:"))
                        {
                            reservedWord = true;
                        }
                        else
                        {
                            typeField = buscarTipoDatoCampo(camposMostrar[i].ToString());
                        }
                    }
                    if (activarAlias)
                    {
                        string tes = camposMostrar[i].ToString().Substring(0, 1).ToUpper() + camposMostrar[i].ToString().Substring(1);
                        builder.Append(camposMostrar[i]).Append(" as ").Append(tes.Replace(".", ""));
                    }
                    else
                    {
                        if (camposMostrar[i].ToString().StartsWith("@"))
                        {
                            builder.Append(camposMostrar[i]).Append("@");
                        }
                        else
                        {

                            if (reservedWord)
                            {
                                if (camposMostrar[i].ToString().StartsWith("Date:"))
                                {
                                    string[] dateCmp = camposMostrar[i].ToString().Split(':');
                                    builder.Append(" CASE WHEN ").Append("(").Append(dateCmp[1]).Append(" = NULL) ");
                                    builder.Append("THEN ").Append("CAST('1900-01-01' as date) ").Append("ELSE ");
                                    builder.Append(dateCmp[1]).Append(" END ");
                                }
                            }

                            else
                            {
                                builder.Append(" CASE WHEN ").Append(camposMostrar[i]).Append(" = NULL ");
                                builder.Append("THEN ").Append(Numeric.Is(typeField) ? "0 " : "'' ").Append("ELSE ");
                                builder.Append(camposMostrar[i]).Append(" END ");
                            }
                        }
                    }
                }
                else
                {
                    string field = "";
                    if (!camposMostrar[i].ToString().StartsWith("@"))
                    {
                        if (camposMostrar[i].ToString().Equals("NOTID"))
                        {
                            reservedWord = true;
                            typeField = Type.GetType("System.String");
                        }
                        else if (camposMostrar[i].ToString().StartsWith("Date:"))
                        {
                            reservedWord = true;
                        }
                        else
                        {
                            field = fuentePrincipal + "." + camposMostrar[i].ToString();
                            typeField = buscarTipoDatoCampo(field);
                        }
                    }

                    if (activarAlias)
                    {
                        string tes = camposMostrar[i].ToString().Substring(0, 1).ToUpper() + camposMostrar[i].ToString().Substring(1);
                        builder.Append("o.").Append(camposMostrar[i]).Append(" as ").Append(tes);

                    }
                    else
                    {
                        if (camposMostrar[i].ToString().StartsWith("@"))
                        {
                            builder.Append("o.").Append(camposMostrar[i]).Append("@");
                        }

                        else
                        {
                            if (reservedWord)
                            {
                                if (camposMostrar[i].ToString().StartsWith("Date:"))
                                {
                                    string[] dateCmp = camposMostrar[i].ToString().Split(':');
                                    builder.Append(" CASE WHEN ").Append("(o.").Append(dateCmp[1]).Append(" = NULL) ");
                                    builder.Append("THEN ").Append("cast('1900-01-01' as date) ").Append("ELSE ");
                                    builder.Append("o.").Append(dateCmp[1]).Append(" END ");
                                }
                                else
                                {
                                    builder.Append(" CASE WHEN ").Append("'").Append(camposMostrar[i]).Append("'").Append(" = NULL ");
                                    builder.Append("THEN ").Append(Numeric.Is(typeField) ? "0 " : "'' ").Append("ELSE ");
                                    builder.Append("'").Append(camposMostrar[i]).Append("'").Append(" END ");
                                }
                            }
                            else
                            {
                                builder.Append(" CASE WHEN ").Append("o.").Append(camposMostrar[i]).Append(" = NULL ");
                                builder.Append("THEN ").Append(Numeric.Is(typeField) ? "0 " : "'' ").Append("ELSE ");
                                builder.Append("o.").Append(camposMostrar[i]).Append(" END ");
                            }
                        }
                    }
                }
                if (i != (cmpMostrar - 1))
                {
                    builder.Append(", ");
                }
            }
            endFieldShow = builder.Length;
            object[] relationTables = tablasRelacionadas == null ? null : new object[tablasRelacionadas.Length];

            if (relationTables != null)
            {
                for (int t = 0; t < tablasRelacionadas.Length; t++)
                {
                    string[] unionPart = tablasRelacionadas[t].ToString().Split(':');

                    if (unionPart.Length > 1)
                    {
                        relationTables[t] = unionPart[1];
                    }
                    else
                    {
                        relationTables[t] = tablasRelacionadas[t];
                    }
                }
            }

            builder = setFields(builder, relationTables, 0, endFieldShow);
            initFrom = builder.Length;
            builder.Append(" from ").Append(fuentePrincipal).Append(" o");

            //Relationed tables
            if (tablasRelacionadas != null)
            {
                if (tablasRelacionadas != null || tablasRelacionadas.Length > 0)
                {
                    int numTabRela = tablasRelacionadas.Length;
                    for (i = 0; i < numTabRela; i++)
                    {
                        string union = tablasRelacionadas[i].ToString();
                        string[] unionPart = union.Split(':');

                        if (unionPart.Length > 1)
                        {
                            builder.Append(" ").Append(unionPart[0]).Append(" o.").Append(unionPart[1]).Append(" ").Append("o").Append(i);
                        }
                        else
                        {
                            builder.Append(" INNER JOIN o.").Append(tablasRelacionadas[i]).Append(" ").Append("o").Append(i);
                        }
                    }
                }
            }
            //Compare values
            int cmpWhere = 0;
            int startWhere = builder.Length;
            int endWhere;
            String[] campos;
            if (camposWhere != null)
            {
                if (camposWhere != null && camposWhere.Length > 0)
                {
                    cmpWhere = camposWhere.Length;
                    builder.Append(" WHERE ");
                    for (i = 0; i < cmpWhere; i++)
                    {
                        campos = camposWhere[i].ToString().Split('#');
                        if (i > 0)
                        {
                            if (campos.Length > 2)
                            {
                                if (campos[2].StartsWith("or"))
                                {
                                    builder.Append(" ").Append(campos[2]).Append(" ");
                                }

                                else
                                {
                                    builder.Append(" and ");
                                    builder.Append(campos[2]).Append(" ");
                                }
                            }
                            else
                            {
                                builder.Append(" AND ");
                            }
                        }
                        if (campos[0].Contains("."))
                        {
                            if (campos[0].ToString().StartsWith("@"))
                            {
                                builder.Append(campos[0].ToString().Substring(1)).Append(" = ");
                            }
                            else
                            {
                                builder.Append(campos[0]);
                            }
                        }
                        else
                        {
                            if (campos[0].ToString().StartsWith("@"))
                            {
                                if (campos.Length > 1)
                                {
                                    builder.Append("o.").Append(campos[0].Substring(1)).Append(" ").Append(campos[1]);
                                }
                                else
                                {
                                    if (campos[0].StartsWith("@BETWEEN"))
                                    {
                                        builder.Append(" ");
                                    }
                                    else
                                    {
                                        builder.Append("o.").Append(campos[0].ToString().Substring(1)).Append(" = ");
                                    }
                                }
                            }
                            else
                            {
                                builder.Append("o.").Append(campos[0]);
                            }
                        }

                        if (campos.Length > 1)
                        {
                            if (campos[1].StartsWith("IN"))
                            {
                                if (campos[1].Contains("@"))
                                {
                                    builder.Append(" ").Append(campos[1].Substring(0, campos[1].LastIndexOf("@"))).Append(" (").Append(valoresWhere[i]).Append(")");
                                }
                                else
                                {
                                    builder.Append(" ").Append(campos[1]).Append(" (:").Append("param").Append(i).Append(")");
                                }
                            }
                            else if (string.Equals(campos[1], "BETWEEN", StringComparison.OrdinalIgnoreCase))
                            {
                                builder.Append(" ").Append(campos[1]).Append(" :").Append("param").Append(i).Append("1")
                                        .Append(" ").Append("AND").Append(" :").Append("param").Append(i).Append("2");
                            }
                            else
                            {
                                builder.Append(" ").Append(campos[1]).Append(" :").Append("param").Append(i);
                            }
                        }
                        else
                        {
                            if (campos[0].ToString().StartsWith("@"))
                            {
                                if (campos[0].StartsWith("@BETWEEN"))
                                {
                                    object valBetween = valoresWhere[i];
                                    object[] values = JsonConvert.DeserializeObject<object[]>(valBetween.ToString());
                                    if (values[2] is DateTime)
                                    {
                                        values[2] = toDateOnly((DateTime)values[2]);
                                    }
                                    if (values[4] is DateTime)
                                    {
                                        values[4] = toDateOnly((DateTime)values[4]);
                                    }

                                    valoresWhere[i] = values;
                                    builder.Append("@").Append(values[0].ToString()).Append("@").Append(" ");
                                }
                                else
                                {
                                    builder.Append("@").Append(valoresWhere[i]).Append("@");
                                }
                            }
                            else
                            {
                                builder.Append(" Like ").Append(":").Append("param").Append(i);
                            }
                        }

                        //if (camposWhere[i].ToString().Contains("."))
                        //{
                        //    builder.Append(camposWhere[i]);
                        //}
                        //else
                        //{
                        //    builder.Append("o.").Append(camposWhere[i]);
                        //}

                        ////Lexicon
                        //builder.Append(" Like ").Append(":").Append("param").Append(i);

                        //if (i != (cmpWhere - 1))
                        //{
                        //    //Lexicon can change by others
                        //    builder.Append(" AND ");
                        //}
                    }

                }

                endWhere = builder.Length;

                builder = setFields(builder, relationTables, startWhere, (endWhere - startWhere));
            }

            return builder.ToString();
        }

        private string genGroupByQuery(string fuentePrincipal, object[] tablasRelacionadas, object[] camposGroup)
        {

            StringBuilder groupDataBuilder = new StringBuilder();
            if (camposGroup != null)
            {
                groupDataBuilder.Append(" GROUP BY ");

                for (int i = 0; i < camposGroup.Length; i++)
                {
                    Type typeField = null;
                    string fieldOder = "";

                    if (i > 0)
                    {
                        groupDataBuilder.Append(",");
                    }

                    if (camposGroup[i].ToString().Contains("."))
                    {
                        fieldOder = camposGroup[i].ToString();
                        typeField = buscarTipoDatoCampo(fieldOder);

                        groupDataBuilder.Append(" CASE WHEN ").Append(camposGroup[i]).Append(" = NULL ");
                        groupDataBuilder.Append("THEN ").Append(Numeric.Is(typeField) ? "0 " : "'' ").Append("ELSE ");
                        groupDataBuilder.Append(camposGroup[i]).Append(" END ");
                    }

                    else
                    {
                        fieldOder = fuentePrincipal + "." + camposGroup[i].ToString();
                        typeField = buscarTipoDatoCampo(fieldOder);
                        groupDataBuilder.Append(" CASE WHEN ").Append("o.").Append(camposGroup[i]).Append(" = NULL ");
                        groupDataBuilder.Append("THEN ").Append(Numeric.Is(typeField) ? "0 " : "'' ").Append("ELSE ");
                        groupDataBuilder.Append("o.").Append(camposGroup[i]).Append(" END ");
                    }
                }

                int endFieldGroup = groupDataBuilder.Length;
                object[] relationTables = tablasRelacionadas == null ? null : new object[tablasRelacionadas.Length];

                if (relationTables != null)
                {
                    for (int t = 0; t < tablasRelacionadas.Length; t++)
                    {
                        string[] unionPart = tablasRelacionadas[t].ToString().Split(':');

                        if (unionPart.Length > 1)
                        {
                            relationTables[t] = unionPart[1];
                        }
                        else
                        {
                            relationTables[t] = tablasRelacionadas[t];
                        }
                    }
                }

                groupDataBuilder = setFields(groupDataBuilder, relationTables, 0, endFieldGroup);

            }

            return groupDataBuilder.ToString();
        }

        private string genOrderQuery(string fuentePrincipal, object[] tablasRelacionadas, object[] camposOrden, string tipoOrden)
        {
            StringBuilder orderDataBuilder = new StringBuilder();

            orderDataBuilder.Append(" ORDER BY ");

            if (camposOrden == null)
            {
                camposOrden = new object[] { "id" };
            }

            for (int i = 0; i < camposOrden.Length; i++)
            {
                Type typeField = null;
                string fieldFilter = "";

                if (i > 0)
                {
                    orderDataBuilder.Append(",");
                }

                if (camposOrden[i].ToString().Contains("."))
                {
                    typeField = buscarTipoDatoCampo(camposOrden[i].ToString());

                    orderDataBuilder.Append(" CASE WHEN ").Append(camposOrden[i]).Append(" = NULL ");
                    orderDataBuilder.Append("THEN ").Append(Numeric.Is(typeField) ? "0 " : "'' ").Append("ELSE ");
                    orderDataBuilder.Append(camposOrden[i]).Append(" END ");
                }

                else
                {
                    fieldFilter = fuentePrincipal + "." + camposOrden[i].ToString();
                    typeField = buscarTipoDatoCampo(fieldFilter);

                    orderDataBuilder.Append(" CASE WHEN ").Append("o.").Append(camposOrden[i]).Append(" = NULL ");
                    orderDataBuilder.Append("THEN ").Append(Numeric.Is(typeField) ? "0 " : "'' ").Append("ELSE ");
                    orderDataBuilder.Append("o.").Append(camposOrden[i]).Append(" END ");

                }
            }

            int endFieldOrder = orderDataBuilder.Length;
            object[] relationTables = tablasRelacionadas == null ? null : new object[tablasRelacionadas.Length];

            if (relationTables != null)
            {
                for (int t = 0; t < tablasRelacionadas.Length; t++)
                {
                    string[] unionPart = tablasRelacionadas[t].ToString().Split(':');

                    if (unionPart.Length > 1)
                    {
                        relationTables[t] = unionPart[1];
                    }
                    else
                    {
                        relationTables[t] = tablasRelacionadas[t];
                    }
                }
            }

            orderDataBuilder = setFields(orderDataBuilder, relationTables, 0, endFieldOrder);

            if (tipoOrden.Equals("DESC"))
            {
                orderDataBuilder.Append(" DESC ");
            }
            else
            {
                orderDataBuilder.Append(" ASC ");
            }

            return orderDataBuilder.ToString();
        }

        private long getTotalRegisters(String genQuery, object[] camposWhere, object[] valoresWhere)
        {
            StringBuilder builderCount = new StringBuilder();
            builderCount.Append(genQuery);

            //MatchCollection mainFrom = Regex.Matches(builderCount.ToString(), @"(?<!\([^()])(?i)\bfrom\b(?![^()]*\))");
            //int pos = mainFrom[0].Index;
            //builderCount.Remove(0, pos);

            // int pos = builderCount.ToString().IndexOf("from");

            int pos = initFrom;
            builderCount.Remove(0, pos);
            builderCount.Insert(0, "Select count(*) ");
            String genQueryCount = builderCount.ToString();
            query = getSession().CreateQuery(genQueryCount);

            if (valoresWhere != null)
            {
                if (valoresWhere != null && valoresWhere.Length > 0)
                {
                    int i;
                    int cmpValWhere = valoresWhere.Length;
                    for (i = 0; i < cmpValWhere; i++)
                    {
                        bool noSetParamIfSubQuery = camposWhere[i].ToString().StartsWith("@") ?
                                   camposWhere[i].ToString().StartsWith("@BETWEEN") ? true : false :
                                   camposWhere[i].ToString().EndsWith("@") ? false : true;
                        if (noSetParamIfSubQuery)
                        {
                            if (valoresWhere[i] is Array)
                            {
                                object[] parValues = (object[])valoresWhere[i];
                                query.SetParameter(parValues[1].ToString(), parValues[2]);
                                query.SetParameter(parValues[3].ToString(), parValues[4]);
                            }
                            else
                            {
                                query.SetParameter("param" + i, valoresWhere[i]);
                            }
                            //query.SetParameter("param" + i, "%" + valoresWhere[i] + "%");
                        }
                    }
                }
            }

            long count = (long)query.UniqueResult();
            return count;
        }

        private StringBuilder setFields(StringBuilder builder, object[] tablasRelacionadas, int startIndex, int endIndex)
        {
            int i = 0;
            if (tablasRelacionadas != null)
            {
                //Enabled
                MatchCollection subQueries = Regex.Matches(builder.ToString(), @"\@([^\@]+)\@");
                var nSubs = 0;
                var countIndex = 0;
                if (subQueries.Count > 0)
                {
                    foreach (Match match in subQueries)
                    {
                        builder.Replace(match.Groups[0].Value, "@" + nSubs);
                        countIndex += match.Length;
                        nSubs++;
                    }
                    endIndex -= countIndex;
                }
                foreach (string k in tablasRelacionadas)
                {
                    var checkExits = builder.ToString(startIndex, (builder.Length - startIndex));


                    if (checkExits.Contains(k))
                    {
                        builder.Replace(k, "o" + i, startIndex, endIndex);
                        var actualLenght = builder.Length;
                        if (endIndex > actualLenght)
                        {
                            endIndex -= (endIndex - actualLenght);
                        }
                        else if (endIndex < actualLenght)
                        {
                            endIndex = actualLenght - startIndex;
                        }
                    }
                    i++;
                }

                //Restablish
                if (subQueries.Count > 0)
                {
                    nSubs = 0;
                    foreach (Match match in subQueries)
                    {
                        builder.Replace("@" + nSubs, match.Groups[1].Value);


                        nSubs++;
                    }
                }

            }
            return builder;
        }

        private DateTime toDateOnly(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day);
        }

        private Type buscarTipoDatoCampo(string campo)
        {

            Type tipoDato = null;
            string[] path = campo.Split('.');
            Assembly asm = Assembly.Load(nameProject);
            string fieldData = path[0].Substring(0, 1).ToUpper() + path[0].ToString().Substring(1);
            if (campo.ToUpper().StartsWith("CFDI") || campo.ToUpper().StartsWith("CERT"))
            {
                tipoDato = asm.GetType(nameProject + ".entidad.cfdi." + (fieldData));
            }
            else
            {
                tipoDato = asm.GetType(nameProject + ".entidad." + (fieldData));
            }
            if (path.Length > 1)
            {
                PropertyInfo field = tipoDato.GetProperty(path[1]);
                if (field.PropertyType.Namespace.Equals("System.Collections.Generic"))
                {
                    tipoDato = field.PropertyType.GenericTypeArguments[0];
                }
                else
                {
                    tipoDato = field.PropertyType;
                }
                if (path.Length > 2)
                {
                    int i;
                    StringBuilder ruta = new StringBuilder(tipoDato.Name);
                    for (i = 2; i < path.Length; i++)
                    {
                        ruta.Append(".").Append(path[i]);
                    }
                    tipoDato = buscarTipoDatoCampo(ruta.ToString());
                }
            }

            return tipoDato;
        }

        public sealed class Numeric
        {
            /// <summary>
            /// Determines if a type is numeric.  Nullable numeric types are considered numeric.
            /// </summary>
            /// <remarks>
            /// Boolean is not considered numeric.
            /// </remarks>
            public static bool Is(Type type)
            {
                if (type == null) return false;

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                    case TypeCode.Object:
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            return Is(Nullable.GetUnderlyingType(type));
                        }
                        return false;
                }
                return false;
            }

            /// <summary>
            /// Determines if a type is numeric.  Nullable numeric types are considered numeric.
            /// </summary>
            /// <remarks>
            /// Boolean is not considered numeric.
            /// </remarks>
            public static bool Is<T>()
            {
                return Is(typeof(T));
            }

        }


    }
}
