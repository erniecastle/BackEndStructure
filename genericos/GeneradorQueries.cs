using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.genericos.campos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Exitosw.Payroll.Entity.util;
using Exitosw.Payroll.Core.genericos;
using Exitosw.Payroll.Core.campos;
using Exitosw.Payroll.Core.servicios.extras;
using System.Configuration;

namespace Exitosw.Payroll.Entity.genericos
{
    public class GeneradorQueries
    {

        //Contiene todos campos que van en la parte from 
        private Dictionary<string, CamposJoin> aliasTablaJoiner;

        //contiene los valores y nombres de los parametros de la parte where 
        public Dictionary<string, object> valoresParametrosQuery { get; set; }

        //Contiene los nombres de las tablas campos join
        private List<string> tablasJoin;

        /* Contiene campos qeu estan en la consulta select que deben agregarse al 
         * group by cuando se requiera ejemplo select vaya un COUNT, MAX, SUM etc. */
        private List<CamposGrupo> camposGrupoExtras;

        //Contiene tipo de datos regresados en la parte select
        public List<Type> tipoDatosSelect { get; set; }

        //Contador de alias para las tablas
        private int countTablasJoin = 0;

        private Mensaje mensaje = null;

        //Base de datos manejada por default
        private TypeDB tipoBD = TypeDB.SQLServer;

        private string nombreAlias = "x";

        private string nameProject = "";

        public GeneradorQueries()
        {
            init();
        }

        public GeneradorQueries(string nombreAlias)
        {
            init();
            this.nombreAlias = nombreAlias;
        }

        #region inicializador de variables
        private void init()
        {
            aliasTablaJoiner = new Dictionary<string, CamposJoin>();
            valoresParametrosQuery = new Dictionary<string, object>();
            tablasJoin = new List<string>();
            tipoDatosSelect = new List<Type>();
            camposGrupoExtras = new List<CamposGrupo>();
            nameProject = ConfigurationManager.AppSettings["routeEntitiesEF"];
            // HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly.GetName().Name;
        }
        #endregion

        /*
         * @1 Tabla join
         * @2 alias tabla
         * @3 id comparador tabla principal
         * @4 id tabla join 
         */
        private const string TABLA_JOIN = "@1 @2 ON @3 = @4";

        private const string RANGOS_SQL = "OFFSET @inicio ROWS FETCH NEXT @cantidad ROWS ONLY";

        private const string RANGOS_MySQL = "LIMIT @inicio, @cantidad";

        //WHERE
        private const string RANGOS_ORACLE = "(rownum >= @inicio and rownum < @cantidad);";

        private const string RANGOS_POSTGRESQL = "LIMIT @cantidad OFFSET @inicio";

        #region  Metodo principal que genera la consulta
        public string construyeQuery(TypeDB tipoBD, TipoOperacion tipoOperacion, string tabla, OperadorSelect operadorSelect, List<CamposFrom> listCamposFrom, List<CamposWhere> listCamposWhere, List<CamposGrupo> listCamposGrupo, List<CamposOrden> listCamposOrden, ValoresRango valoresRango)
        {
            string rangoQuery = "";
            init();
            this.tipoBD = tipoBD;
            List<CamposSelect> listCamposSelect;
            if (tipoOperacion == TipoOperacion.SELECT)
            {
                listCamposSelect = operadorSelect.listCamposSelect;
            }
            else
            {
                listCamposSelect = new List<CamposSelect>();
            }
            generaListaTablasMapeadas(listCamposSelect, listCamposFrom, listCamposWhere, listCamposGrupo, listCamposOrden);
            #region  agregado rangos en la consultas
            /////////////////////////Rangos Consultas////////////////////////////////////////////
            if (valoresRango != null & tipoOperacion == TipoOperacion.SELECT)
            {   ///@inicio, @cantidad
                if (tipoBD == TypeDB.SQLServer)
                {
                    rangoQuery = RANGOS_SQL.Replace("@inicio", valoresRango.inicio.ToString()).Replace("@cantidad", valoresRango.cantidad.ToString());
                    listCamposOrden = listCamposOrden == null ? new List<CamposOrden>() : listCamposOrden;
                    CamposOrden campoOrdenRango = null;
                    if (operadorSelect.todosDatos)
                    {
                        string value = aliasTablaJoiner.Keys.ElementAt(0);
                        campoOrdenRango = new CamposOrden(String.Concat(value, ".id"), TipoOrden.ASCENDENTE);
                    }
                    else
                    {
                        if (listCamposSelect != null)
                        {
                            foreach (var item in listCamposSelect)
                            {
                                Mensaje m = buscarTipoDatoCampo(item.campoMostrar);
                                if (m.noError != 0)
                                {
                                    break;
                                }
                                Type tipoDato = (Type)m.resultado;
                                if (tipoDato.Namespace.Equals(nameProject + ".entidad"))
                                {
                                    campoOrdenRango = new CamposOrden(String.Concat(item.campoMostrar, ".id"), TipoOrden.ASCENDENTE);
                                }
                                else
                                {
                                    string value = item.campoMostrar.Substring(0, item.campoMostrar.LastIndexOf('.'));
                                    campoOrdenRango = new CamposOrden(String.Concat(value, ".id"), TipoOrden.ASCENDENTE);
                                }
                                break;
                            }
                        }
                    }

                    if (listCamposOrden.Count > 0)
                    {
                        listCamposOrden.Insert(0, campoOrdenRango);
                    }
                    else
                    {
                        listCamposOrden.Add(campoOrdenRango);
                    }
                }
                else if (tipoBD == TypeDB.MySQL)
                {
                    rangoQuery = RANGOS_MySQL.Replace("@inicio", valoresRango.inicio.ToString()).Replace("@cantidad", valoresRango.cantidad.ToString());
                }
                else if (tipoBD == TypeDB.PostgreSQL)
                {
                    rangoQuery = RANGOS_POSTGRESQL.Replace("@inicio", valoresRango.inicio.ToString()).Replace("@cantidad", valoresRango.cantidad.ToString());
                }
                else if (tipoBD == TypeDB.Oracle)
                {
                    rangoQuery = RANGOS_ORACLE.Replace("@inicio", valoresRango.inicio.ToString()).Replace("@cantidad", valoresRango.cantidad.ToString());
                }

            }
            #endregion 
            /////////////////////////////////////////////////////////////////////////////////
            StringBuilder query = new StringBuilder();
            if (tipoOperacion == TipoOperacion.SELECT)
            {
                query.Append(construyeSelectDatos(operadorSelect, tipoOperacion));
            }
            else
            {
                query.Append(getTipoOperacion(tipoOperacion)).Append(" ");
            }

            if (tipoOperacion == TipoOperacion.UPDATE)
            {
                query.Append(" ").Append(tabla).Append(" ").Append(contruyeSetUpdate(listCamposSelect));
            }
            else
            {
                query.Append(" ").Append(construyeFromConsulta(TipoJoin.LEFT_JOIN));
            }

            query.Append(" ").Append(construyeCamposWhere(listCamposWhere));
            if (tipoBD == TypeDB.Oracle)
            {
                listCamposWhere = listCamposWhere == null ? new List<CamposWhere>() : listCamposWhere;
                if (listCamposWhere.Count == 0)
                {
                    query.Append(" WHERE ");
                }
                else
                {
                    query.Append(" AND ");
                }
                query.Append(rangoQuery);
            }
            if (tipoOperacion == TipoOperacion.SELECT)
            {
                query.Append(" ").Append(construyeCamposGroupBY(listCamposGrupo));
                query.Append(" ").Append(construyeCamposOrden(listCamposOrden));
            }
            if (tipoBD != TypeDB.Oracle)
            {
                query.Append(" ").Append(rangoQuery);
            }

            return query.ToString();
        }
        #endregion


        private string getTipoOperacion(TipoOperacion tipoOperacion)
        {
            if (tipoOperacion == TipoOperacion.SELECT)
            {
                return "SELECT";
            }
            else if (tipoOperacion == TipoOperacion.UPDATE)
            {

                return "UPDATE";
            }
            else if (tipoOperacion == TipoOperacion.DELETE)
            {
                string value = aliasTablaJoiner.Keys.ElementAt(0);
                CamposJoin campoJoin = null;
                campoJoin = aliasTablaJoiner[value];
                return "DELETE " + campoJoin.alias;
            }
            else
            {
                return "SELECT";
            }
        }

        private string contruyeSetUpdate(List<CamposSelect> listCamposSelect)
        {
            StringBuilder set = new StringBuilder("SET");
            listCamposSelect = listCamposSelect == null ? new List<CamposSelect>() : listCamposSelect;
            for (int i = 0; i < listCamposSelect.Count; i++)
            {
                set.Append(" ").Append(listCamposSelect[i].campoMostrar).Append(" = @valorSet").Append(i);
                valoresParametrosQuery.Add(String.Concat("@valorSet", i), listCamposSelect[i].valor);
                if (i < listCamposSelect.Count - 1)
                {
                    set.Append(",");
                }
            }
            return set.ToString();
        }

        #region  Genera estructura del la parte select de la consulta
        /*
            Restricciones Select al usar funciones (count, sum, max, min, etc) 
            count(*),campo,campo2   valido
            count(p.*), campo       invalido
            count(campo1), campo2   valido
            count(p.clave), campo2  valido

        */

        private string construyeSelectDatos(OperadorSelect operadorSelect, TipoOperacion tipoOperacion)
        {
            StringBuilder select = new StringBuilder(getTipoOperacion(tipoOperacion));

            if (operadorSelect.usaDistinct)
            {
                select.Append(" DISTINCT ");
            }
            else
            {
                select.Append(" ");
            }
            List<CamposSelect> listCamposSelect = operadorSelect.listCamposSelect;
            listCamposSelect = listCamposSelect == null ? new List<CamposSelect>() : listCamposSelect;
            #region  Parte select muestra todos los datos si esta activado boleano todosDatos
            if (operadorSelect.todosDatos)
            {
                if (operadorSelect.tipoFuncion == TipoFuncion.NINGUNO)
                {
                    //Sin TipoFuncion
                    select.Append("*");
                    foreach (var item in tablasJoin)
                    {
                        if (!item.Contains("."))
                        {
                            Mensaje m = buscarTipoDatoCampo(item);
                            tipoDatosSelect.Add((Type)m.resultado);
                        }
                    }
                }
                else
                {
                    //Agrega TipoFuncion
                    select.Append(ManejadorEnum.GetDescription(operadorSelect.tipoFuncion)).Append("(*)");
                    if (operadorSelect.tipoFuncion == TipoFuncion.CONTAR)
                    {
                        tipoDatosSelect.Add(typeof(Int32));
                    }
                }
            }
            #endregion  
            else
            {
                string[] rutas;
                string path;
                //Busca si alguno de los campos a mostrar en la consulta tiene alguna funcion max, min, sum, count etc
                var result = (from val in listCamposSelect where val.tipoFuncion != TipoFuncion.NINGUNO select val).FirstOrDefault();

                bool usaFuncion = result == null ? false : true;
                Type tipoDato = null;
                string valorCaseWhen;

                foreach (CamposSelect item in listCamposSelect)
                {
                    ///pendiente codigo evalua si campo tiene 2 movimientos 
                    ///camposGrupoExtras
                    if (item.campoMostrar != null)
                    {
                        path = item.campoMostrar;
                        List<CamposSelect> subCampos = item.subCampos;
                        rutas = path.Split('.');
                        CamposJoin campoJoin;
                        #region  Parte CamposSelect que son de tipo Tabla ejemplo, Paises, Estados etc.
                        if (rutas.Length == 1)
                        {
                            if (aliasTablaJoiner.ContainsKey(path))
                            {
                                campoJoin = aliasTablaJoiner[path];
                                if (item.tipoFuncion == TipoFuncion.NINGUNO)
                                {
                                    select.Append(campoJoin.alias).Append(".*").Append(",");
                                    tipoDatosSelect.Add(campoJoin.campo);
                                }
                                else
                                {
                                    if (item.tipoFuncion != TipoFuncion.NINGUNO)
                                    {
                                        select.Append(ManejadorEnum.GetDescription(item.tipoFuncion)).Append("(");
                                        select.Append("*").Append("),");
                                        if (item.tipoFuncion == TipoFuncion.CONTAR)
                                        {
                                            tipoDatosSelect.Add(typeof(Int32));
                                        }
                                    }
                                    else if (operadorSelect.tipoFuncion == TipoFuncion.NINGUNO)
                                    {
                                        select.Append(campoJoin.alias).Append(".*").Append(",");
                                    }
                                    else
                                    {
                                        select.Append(ManejadorEnum.GetDescription(operadorSelect.tipoFuncion)).Append("(");
                                        select.Append(campoJoin.alias).Append(".*").Append("),");
                                        if (item.tipoFuncion == TipoFuncion.CONTAR)
                                        {
                                            tipoDatosSelect.Add(typeof(Int32));
                                        }
                                    }
                                }

                            }
                        }
                        #endregion  
                        #region  Parte CamposSelect que son de tipo Tabla ejemplo, Paises.clave, Estados.descripcion etc.
                        else
                        {
                            //Si hay campo con funcion se agrega a campos agrupar las que no tengan funcion
                            if (item.tipoFuncion == TipoFuncion.NINGUNO & usaFuncion)
                            {
                                camposGrupoExtras.Add(new CamposGrupo(item.campoMostrar));
                            }
                            path = path.Substring(0, path.Length - rutas[rutas.Length - 1].Length - 1);

                            if (aliasTablaJoiner.ContainsKey(path))
                            {
                                campoJoin = aliasTablaJoiner[path];
                                #region  Parte CamposSelect activado case when 
                                if (item.usaCaseWhen)
                                {
                                    tipoDato = campoJoin.campo;
                                    valorCaseWhen = valorCaseWhenPorTipoDato(tipoDato);

                                    select.Append(construyeCaseWhenCampos(campoJoin.alias, rutas[rutas.Length - 1], valorCaseWhen));
                                    Mensaje m = buscarTipoDatoCampo(item.campoMostrar);
                                    tipoDatosSelect.Add((Type)m.resultado);
                                }
                                #endregion 
                                else
                                {
                                    /*compara si campoMostrar es una entidad para agregarle asterisco quedaria alias+* == (x0.*) */
                                    #region  Parte CamposSelect que campo a mostrar puede ser una entidad(toda la tabla)
                                    if (aliasTablaJoiner.ContainsKey(item.campoMostrar))
                                    {
                                        campoJoin = aliasTablaJoiner[item.campoMostrar];


                                        if (item.tipoFuncion == TipoFuncion.NINGUNO)
                                        {
                                            //select.Append(campoJoin.alias).Append(".*");
                                            select.Append(generaSintaxisCampo(campoJoin.alias, "*", item.tipoFuncion, ""));
                                            tipoDatosSelect.Add(campoJoin.campo);
                                        }
                                        else
                                        {
                                            select.Append(generaSintaxisCampo("", "*", item.tipoFuncion, ""));
                                            ////select.Append(ManejadorEnum.GetDescription(item.tipoFuncion)).Append("(");
                                            ////select.Append(campoJoin.alias).Append(".*").Append(")");
                                            ////if (item.tipoFuncion == TipoFuncion.CONTAR)
                                            ////{
                                            ////    tipoDatosSelect.Add(typeof(Int32));
                                            ////}
                                        }
                                    } ///end  aliasTablaJoiner.ContainsKey(item.campoMostrar)
                                    #endregion
                                    #region  Parte CamposSelect que campo a mostrar pueden ser campos de la tabla paises.clave, estados.descripcion
                                    else
                                    {
                                        select.Append(generaSintaxisCampo(campoJoin.alias, rutas[rutas.Length - 1], item.tipoFuncion, item.campoMostrar));

                                        ////if (item.tipoFuncion == TipoFuncion.NINGUNO) {
                                        ////    select.Append(generaSintaxisCampo(campoJoin.alias, rutas[rutas.Length - 1], item.tipoFuncion, item.campoMostrar));
                                        ////    //select.Append(campoJoin.alias).Append(".").Append(rutas[rutas.Length - 1]);
                                        ////    //Mensaje m = buscarTipoDatoCampo(item.campoMostrar);
                                        ////    //tipoDatosSelect.Add((Type)m.resultado);
                                        ////} else {
                                        ////    select.Append(generaSintaxisCampo(campoJoin.alias, rutas[rutas.Length - 1], item.tipoFuncion, item.campoMostrar));
                                        ////    //select.Append(ManejadorEnum.GetDescription(item.tipoFuncion)).Append("(");
                                        ////    //select.Append(campoJoin.alias).Append(".").Append(rutas[rutas.Length - 1]).Append(")");
                                        ////    //if (item.tipoFuncion == TipoFuncion.CONTAR)
                                        ////    //{
                                        ////    //    tipoDatosSelect.Add(typeof(Int32));
                                        ////    //}
                                        ////    //else
                                        ////    //{
                                        ////    //    Mensaje m = buscarTipoDatoCampo(item.campoMostrar);
                                        ////    //    tipoDatosSelect.Add((Type)m.resultado);
                                        ////    //}
                                        ////}
                                    }
                                    #endregion
                                }
                                select.Append(",");
                            } //end aliasTablaJoiner.ContainsKey(path) 
                        }
                        #endregion
                    }
                }
            }
            select.Replace(",", "", select.Length - 1, 1);
            return select.ToString();
        }
        #endregion

        private string generaSintaxisCampo(string alias, string campo, TipoFuncion tipoFuncion, string pathCampo)
        {
            if (tipoFuncion == TipoFuncion.CONTAR)
            {
                tipoDatosSelect.Add(typeof(Int32));
            }
            else
            {
                if (!String.IsNullOrEmpty(pathCampo))
                {
                    Mensaje m = buscarTipoDatoCampo(pathCampo);
                    tipoDatosSelect.Add((Type)m.resultado);
                }
            }

            if (tipoFuncion == TipoFuncion.NINGUNO)
            {
                return String.Concat(alias, ".", campo);
            }
            else
            {
                string mascaraFuncion = "(@)";
                string punto = String.IsNullOrEmpty(pathCampo) ? "" : ".";
                string campoSelect = String.Concat(alias, punto, campo);
                mascaraFuncion = mascaraFuncion.Replace("@", campoSelect);
                return String.Concat(ManejadorEnum.GetDescription(tipoFuncion), mascaraFuncion);
            }
        }

        #region  Genera estructura del campo (case when then else) en caso de usarlo
        private string construyeCaseWhenCampos(string aliasTabla, string campo, string valorCaseWhen)
        {
            StringBuilder caseWhen = new StringBuilder("CASE WHEN ");
            caseWhen.Append(aliasTabla).Append(" IS NULL THEN ").Append(valorCaseWhen);
            caseWhen.Append(" ELSE CASE WHEN ").Append(aliasTabla).Append(".").Append(campo).Append(" IS NULL THEN ").Append(valorCaseWhen);
            caseWhen.Append(" ELSE ").Append(aliasTabla).Append(".").Append(campo).Append(" END END ");
            return caseWhen.ToString();
        }

        private string valorCaseWhenPorTipoDato(Type tipoDato)
        {
            if (tipoDato.IsPrimitive)
            {
                if (tipoDato.Equals(typeof(int)) | tipoDato.Equals(typeof(decimal)) | tipoDato.Equals(typeof(byte))
                    | tipoDato.Equals(typeof(short)) | tipoDato.Equals(typeof(bool)) | tipoDato.Equals(typeof(long)))
                {
                    return "0";
                }
                else if (tipoDato.Equals(typeof(float)) | tipoDato.Equals(typeof(double)))
                {
                    return "0.0";
                }
            }
            else
            {
                if (tipoDato.Equals(typeof(Int16)) | tipoDato.Equals(typeof(Int32)) | tipoDato.Equals(typeof(Int64))
                   | tipoDato.Equals(typeof(Decimal)) | tipoDato.Equals(typeof(Byte)) | tipoDato.Equals(typeof(Boolean)))
                {
                    return "0";
                }
                else if (tipoDato.Equals(typeof(Double)))
                {
                    return "0.0";
                }
                else if (tipoDato.Equals(typeof(DateTime)))
                {
                    if (tipoBD == TypeDB.Oracle | tipoBD == TypeDB.PostgreSQL)
                    {
                        return "TO_DATE('1900-01-01', 'YYYY-MM-DD')";
                    }
                    return "CAST('1900-01-01' AS DATE)";

                }
            }
            return "";
        }
        #endregion

        #region  Genera estructura de la parte From de la consulta
        private string construyeFromConsulta(TipoJoin tipoJoin)
        {
            string joinUsado = ManejadorEnum.GetDescription(tipoJoin);
            StringBuilder from = new StringBuilder("FROM ");
            string[] ruta;
            string tabla;
            for (int i = 0; i < tablasJoin.Count; i++)
            {
                tabla = tablasJoin[i];

                ruta = tabla.Split('.');
                CamposJoin campoJoin = null;
                if (aliasTablaJoiner.ContainsKey(tabla))
                {
                    campoJoin = aliasTablaJoiner[tabla];
                }
                String campo;
                if (ruta.Length == 1)
                {
                    if (i > 0)
                    {
                        from.Append(",");
                    }

                    campo = String.Concat(campoJoin.campo.Name, " ", campoJoin.alias);
                    //ruta[ruta.Length - 1] + " " + aliasTablaJoiner[ruta[ruta.Length - 1]]
                }
                else
                {
                    CamposJoin joinTabla = aliasTablaJoiner[tabla];
                    CamposJoin joinPadre = aliasTablaJoiner[tabla.Substring(0, tabla.Length - ruta[ruta.Length - 1].Length - 1)];
                    if (joinTabla.tipoJoin != null)
                    {
                        joinUsado = ManejadorEnum.GetDescription(joinTabla.tipoJoin);
                    }
                    //"@1 @2 ON @3 = @4";
                    string join = TABLA_JOIN.Replace("@1", campoJoin.campo.Name).Replace("@2", campoJoin.alias)
                        .Replace("@3", String.Concat(joinPadre.alias, ".", campoJoin.campoId)).Replace("@4", String.Concat(campoJoin.alias, ".id"));
                    campo = String.Concat(joinUsado, " ", join);
                    ///campo =  (aliasTablaJoiner[tabla.Substring(0, tabla.Length - ruta[ruta.Length-1].Length -1)]) + "." + ruta[ruta.Length - 1] + " " + aliasTablaJoiner[ruta[ruta.Length - 1]]; ;
                }
                from.Append(campo).Append(" ");
            }
            return from.ToString();
        }
        #endregion

        #region  Genera estructura de la parte where de la consulta
        private string construyeCamposWhere(List<CamposWhere> listCamposWhere)
        {
            listCamposWhere = listCamposWhere == null ? new List<CamposWhere>() : listCamposWhere;
            if (listCamposWhere.Count == 0)
            {
                return "";
            }
            StringBuilder where = new StringBuilder("WHERE ");
            string[] ruta;
            string path;
            int cont = 0, index = 0;
            foreach (CamposWhere item in listCamposWhere)
            {
                ruta = item.campo.Split('.');
                path = item.campo.Substring(0, item.campo.Length - ruta[ruta.Length - 1].Length - 1);
                CamposJoin campoJoin = null;
                if (aliasTablaJoiner.ContainsKey(path))
                {
                    if (item.listCamposAgrupados.Count > 0)
                    {
                        where.Append(" (");
                    }
                    campoJoin = aliasTablaJoiner[path];
                    //if (item.operadorLogico == OperadorLogico.NOT_AND | item.operadorLogico == OperadorLogico.NOT_OR) {
                    //    where.Append("NOT").Append(" ");
                    //}
                    where.Append(" ").Append(campoJoin.alias).Append(".").Append(ruta[ruta.Length - 1]).Append(" ");
                    if (item.operadorComparacion == OperadorComparacion.DIFERENTE & tipoBD == TypeDB.PostgreSQL)
                    {
                        where.Append("<>").Append(" "); ;
                    }
                    else
                    {
                        where.Append(ManejadorEnum.GetDescription(item.operadorComparacion)).Append(" ");
                    }

                    if (item.operadorComparacion == OperadorComparacion.BETWEEN | item.operadorComparacion == OperadorComparacion.NOT_BETWEEN)
                    {
                        string parametro = String.Concat("@parametro", cont.ToString());
                        // where.Append("(")
                        where.Append(parametro);
                        cont++;
                        string parametro2 = String.Concat("@parametro", cont.ToString());


                        if (item.valor.GetType().Namespace.Equals("System.Collections.Generic"))
                        {
                            IList datos = (IList)item.valor;
                            valoresParametrosQuery.Add(parametro, datos[0]);
                            valoresParametrosQuery.Add(parametro2, datos[1]);
                        }
                        else if (item.valor.GetType().BaseType.Name.Equals("Array"))
                        {
                            object[] datos = (object[])item.valor;
                            valoresParametrosQuery.Add(parametro, datos[0]);
                            valoresParametrosQuery.Add(parametro2, datos[1]);
                        }
                        else
                        {
                            valoresParametrosQuery.Add(parametro, item.valor);
                            valoresParametrosQuery.Add(parametro2, item.valor);
                        }
                        where.Append(" AND ").Append(parametro2);
                        //.Append(")");
                    }
                    else if (item.operadorComparacion == OperadorComparacion.IN | item.operadorComparacion == OperadorComparacion.NOT_IN)
                    {
                        where.Append("(");
                        string parametro;
                        if (item.subConsulta != null)
                        {
                            where.Append(construyeSubConsulta(item.subConsulta));
                        }
                        else if (item.valor.GetType().Namespace.Equals("System.Collections.Generic"))
                        {
                            List<object> datos = (List<object>)item.valor;
                            int i;
                            for (i = 0; i < datos.Count; i++)
                            {
                                parametro = String.Concat("@parametro", cont.ToString());
                                valoresParametrosQuery.Add(parametro, datos[i]);
                                where.Append(parametro);
                                if (i < datos.Count - 1)
                                {
                                    where.Append(",");
                                }
                                cont++;
                            }
                        }
                        else if (item.valor.GetType().BaseType.Name.Equals("Array"))
                        {
                            object[] datos = (object[])item.valor;
                            int i;
                            for (i = 0; i < datos.Length; i++)
                            {
                                parametro = String.Concat("@parametro", cont.ToString());
                                valoresParametrosQuery.Add(parametro, datos[i]);
                                where.Append(parametro);
                                if (i < datos.Length - 1)
                                {
                                    where.Append(",");
                                }
                                cont++;
                            }

                        }
                        where.Append(")");

                        //valoresParametrosQuery.Add(parametro, item.valor);
                    }
                    else if (item.operadorComparacion == OperadorComparacion.LIKE | item.operadorComparacion == OperadorComparacion.NOT_LIKE)
                    {
                        string parametro = String.Concat("@parametro", cont.ToString());
                        where.Append(parametro);
                        string valor = String.Concat("%", item.valor, "%");
                        valoresParametrosQuery.Add(parametro, valor);
                    }
                    else
                    {  //=====
                        if (item.subConsulta != null)
                        {
                            where.Append(construyeSubConsulta(item.subConsulta));
                        }
                        else if (item.operadorComparacion != OperadorComparacion.IS_NOT_NULL & item.operadorComparacion != OperadorComparacion.IS_NULL)
                        {
                            string parametro = String.Concat("@parametro", cont.ToString());
                            where.Append(parametro);
                            valoresParametrosQuery.Add(parametro, item.valor);
                        }
                    }
                    index++;
                    if (index <= listCamposWhere.Count - 1)
                    {
                        where.Append(" ").Append(ManejadorEnum.GetDescription(item.operadorLogico));
                    }
                    cont++;
                    if (item.listCamposAgrupados.Count > 0)
                    {
                        string subWhere = construyeCamposWhere(item.listCamposAgrupados);
                        where.Append(" ").Append(ManejadorEnum.GetDescription(item.operadorLogico)).Append(subWhere.Replace("WHERE", " ")).Append(")");
                    }
                }
            }
            return where.ToString();
        }
        #endregion

        #region Construye Sub consulta en los campos where
        private string construyeSubConsulta(SubConsulta subConsulta)
        {
            GeneradorQueries subGenerador = new GeneradorQueries("y");
            string subQuery = subGenerador.construyeQuery(tipoBD, TipoOperacion.SELECT, "", subConsulta.operadorSelect, subConsulta.listCamposFrom, subConsulta.listCamposWhere, subConsulta.listCamposGrupo, subConsulta.listCamposOrden, subConsulta.valoresRango);
            foreach (var dic in subGenerador.valoresParametrosQuery)
                valoresParametrosQuery.Add(dic.Key, dic.Value);
            return subQuery;
        }
        #endregion

        #region  Genera estructura de la parte OrderBy de la consulta
        private string construyeCamposOrden(List<CamposOrden> listCamposOrden)
        {
            listCamposOrden = listCamposOrden == null ? new List<CamposOrden>() : listCamposOrden;
            if (listCamposOrden.Count == 0)
            {
                return "";
            }
            StringBuilder orderBy = new StringBuilder("ORDER BY ");
            string[] ruta;
            string path;
            foreach (CamposOrden item in listCamposOrden)
            {
                ruta = item.campo.Split('.');
                path = item.campo.Substring(0, item.campo.Length - ruta[ruta.Length - 1].Length - 1);
                CamposJoin campoJoin = null;
                if (aliasTablaJoiner.ContainsKey(path))
                {
                    campoJoin = aliasTablaJoiner[path];
                    orderBy.Append(campoJoin.alias).Append(".").Append(ruta[ruta.Length - 1]).Append(" ");
                    if (item.tipoOrden == TipoOrden.DESCENDENTE)
                    {
                        orderBy.Append(ManejadorEnum.GetDescription(item.tipoOrden));
                    }
                }
            }
            return orderBy.ToString();
        }
        #endregion

        #region  Genera estructura de la parte GroupBY de la consulta
        private string construyeCamposGroupBY(List<CamposGrupo> listCamposGrupo)
        {
            listCamposGrupo = listCamposGrupo == null ? new List<CamposGrupo>() : listCamposGrupo;
            foreach (CamposGrupo item in camposGrupoExtras)
            {
                CamposGrupo result = listCamposGrupo.Find(s => s.campo == item.campo);
                if (result == null)
                {
                    listCamposGrupo.Add(item);
                }
            }
            if (listCamposGrupo.Count == 0)
            {
                return "";
            }

            StringBuilder groupBy = new StringBuilder("GROUP BY ");
            string[] ruta;
            string path;

            foreach (CamposGrupo item in listCamposGrupo)
            {
                ruta = item.campo.Split('.');
                path = item.campo.Substring(0, item.campo.Length - ruta[ruta.Length - 1].Length - 1);
                CamposJoin campoJoin = null;
                if (aliasTablaJoiner.ContainsKey(path))
                {
                    campoJoin = aliasTablaJoiner[path];
                    groupBy.Append(campoJoin.alias).Append(".").Append(ruta[ruta.Length - 1]).Append(" ");
                }
            }
            return groupBy.ToString();
        }
        #endregion


        #region  Parte de Pruebas
        public void pruebas()
        {
            try
            {
                //Code 12630
                ConnectionDB cxn = new ConnectionDB();
                cxn.usuario = "sa";
                cxn.password = "adminadmin";
                cxn.puertoServer = "1433";
                cxn.tipoServer = TypeDB.SQLServer;
                cxn.server = "localhost";
                cxn.nombreBd = "DBSimple";//DBMaster

                using (SqlConnection conn = (SqlConnection)EntityFrameworkCxn.createDbConnection(cxn))
                {
                    conn.Open();
                    //oa
                    string query = "SELECT * FROM Estados x0  WHERE x0.descripcion LIKE @params";
                    SqlCommand command = new SqlCommand(query, conn);

                    command.Parameters.Add(new SqlParameter("@params", "%oa%"));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("FirstColumn\tSecond Column\t\tThird Column\t\tForth Column\t");

                        while (reader.Read())
                        {
                            Console.WriteLine(reader["clave"]);
                            Console.WriteLine(reader["Descripcion"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void provadorMetodos()
        {
            List<object> resultado = new List<object>();
            try
            {
                //List<CamposGrupo> camposGrupo= new List<CamposGrupo>() {new CamposGrupo("Estados.clave"), new CamposGrupo("Estados.paises.descripcion") };
                //camposGrupoExtras = new List<CamposGrupo>() { new CamposGrupo("Estados.clave"), new CamposGrupo("Estados.descripcion") };
                //construyeCamposGroupBY(camposGrupo);
                //List<CamposSelect> campos = new List<CamposSelect>() { new CamposSelect("CentroDeCosto", null) };
                //List<string> valores = new List<string>() { "0001", "0002" };
                //List<CamposWhere> camposWhere = new List<CamposWhere>() { new CamposWhere("CentroDeCosto.razonesSociales.clave", valores, OperadorComparacion.BETWEEN, OperadorLogico.AND) };
                ///List<CamposWhere> camposWhere = new List<CamposWhere>() { new CamposWhere("CentroDeCosto.razonesSociales.clave", valor, OperadorComparacion.IGUAL, OperadorLogico.AND) };
                //  List<CamposWhere> camposWhere = new List<CamposWhere>() { new CamposWhere("Estados.paises.clave", "me", OperadorComparacion.LIKE, OperadorLogico.AND) };

                ////////////////List<CamposWhere> camposWhere = new List<CamposWhere>();
                ////////////////List<CamposFrom> camposFrom = new List<CamposFrom>() { new CamposFrom("Estados.paises", TipoJoin.LEFT_JOIN) };
                // List<CamposWhere> camposWhere = new List<CamposWhere>() { new CamposWhere("Estados.paises.descripcion", "me", OperadorComparacion.LIKE, OperadorLogico.AND) };
                // List<CamposSelect> campos = new List<CamposSelect>() { new CamposSelect("Estados", null), new CamposSelect("Estados.paises", null), new CamposSelect("Estados.paises.clave", null) };
                ////////////////List<CamposSelect> campos = new List<CamposSelect>() { new CamposSelect("Estados", TipoFuncion.CONTAR), new CamposSelect("Estados.paises", TipoFuncion.CONTAR), new CamposSelect("Estados.clave", TipoFuncion.NINGUNO) };
                //var result = (from val in campos where val.tipoFuncion != TipoFuncion.NINGUNO & val.tipoFuncion != null select val).FirstOrDefault();
                //string[] valores2 = new string[] { "MEX", "CAN" };
                // List<CamposOrden> camposOrden = new List<CamposOrden>() { new CamposOrden("Estados.clave", TipoOrden.DESCENDENTE) };
                //////////////List<CamposOrden> camposOrden = new List<CamposOrden>() { };
                //////////////OperadorSelect operadorSelect = new OperadorSelect(campos);
                //operadorSelect.todosDatos = true;
                //operadorSelect.tipoFuncion = TipoFuncion.CONTAR;
                CamposSelect campoSelect = new CamposSelect("FiniqLiquidPlazas.plazasPorEmpleado.empleados", TipoFuncion.NINGUNO);
                OperadorSelect operadorSelect = new OperadorSelect(new List<CamposSelect>() { campoSelect });
                List<CamposFrom> listCamposFrom = new List<CamposFrom>() { new CamposFrom("FiniqLiquidPlazas", TipoJoin.LEFT_JOIN), new CamposFrom("FiniqLiquidPlazas.finiquitosLiquida", TipoJoin.RIGHT_JOIN),
                    new CamposFrom("FiniqLiquidPlazas.plazasPorEmpleado", TipoJoin.RIGHT_JOIN), new CamposFrom("FiniqLiquidPlazas.plazasPorEmpleado.empleados", TipoJoin.RIGHT_JOIN),
                    new CamposFrom("FiniqLiquidPlazas.plazasPorEmpleado.razonesSociales", TipoJoin.RIGHT_JOIN)};
                List<CamposWhere> listCamposWhere = new List<CamposWhere>() { new CamposWhere("FiniqLiquidPlazas.plazasPorEmpleado.empleados.clave", "00001", OperadorComparacion.IGUAL, OperadorLogico.AND),
                    new CamposWhere("", "0001", OperadorComparacion.IGUAL, OperadorLogico.AND), new CamposWhere("", 0, OperadorComparacion.DIFERENTE, OperadorLogico.AND)};
                if (listCamposWhere.Count > 0)
                {
                    listCamposWhere[listCamposWhere.Count - 2].campo = "FiniqLiquidPlazas.plazasPorEmpleado.razonesSociales.clave";
                    listCamposWhere[listCamposWhere.Count - 1].campo = "FiniqLiquidPlazas.finiquitosLiquida.modoBaja";
                    listCamposWhere[listCamposWhere.Count - 1].operadorComparacion = OperadorComparacion.DIFERENTE;
                    listCamposWhere[listCamposWhere.Count - 1].operadorLogico = OperadorLogico.OR;
                    List<CamposWhere> listSubCamposWhere = new List<CamposWhere>() { new CamposWhere("FiniqLiquidPlazas.id", 0, OperadorComparacion.IS_NULL, OperadorLogico.AND) };
                    listCamposWhere[listCamposWhere.Count - 1].listCamposAgrupados = listSubCamposWhere;
                }
                CamposOrden campoOrden = new CamposOrden("FiniqLiquidPlazas.plazasPorEmpleado.empleados.clave");
                List<CamposOrden> orden = new List<CamposOrden>() { campoOrden };

                ///SUbquery pruebas 
                campoSelect = new CamposSelect("Paises", TipoFuncion.NINGUNO);
                operadorSelect = new OperadorSelect(new List<CamposSelect>() { campoSelect });
                CamposWhere campoWhere = new CamposWhere("Paises.id", null, OperadorComparacion.IN, OperadorLogico.AND);

                CamposSelect campoSelectSub = new CamposSelect("Paises.id", TipoFuncion.NINGUNO);
                OperadorSelect operadorSelectSub = new OperadorSelect(new List<CamposSelect>() { campoSelectSub });
                List<CamposWhere> listCamposWhereSub = new List<CamposWhere>() { new CamposWhere("Paises.clave", "mex", OperadorComparacion.IGUAL, OperadorLogico.AND) };
                SubConsulta subConsulta = new SubConsulta();
                subConsulta.operadorSelect = operadorSelectSub;
                subConsulta.listCamposWhere = listCamposWhereSub;
                campoWhere.subConsulta = subConsulta;

                listCamposWhere = new List<CamposWhere>() { campoWhere };

                listCamposFrom = null;
                orden = null;

                string query = construyeQuery(TypeDB.SQLServer, TipoOperacion.SELECT, "tabla", operadorSelect, listCamposFrom, listCamposWhere, null, orden, null);

                Type clasePrincipal = null;

                object instancia = null;

                foreach (var item in tipoDatosSelect)
                {
                    clasePrincipal = item;
                    break;

                }
                PropertyInfo[] propertie;
                if (clasePrincipal != null)
                {
                    instancia = Activator.CreateInstance(clasePrincipal);
                    propertie = clasePrincipal.GetProperties();
                }
                else
                {
                    propertie = new PropertyInfo[] { };
                }

                //Code 12630
                ConnectionDB cxn = new ConnectionDB();
                cxn.usuario = "sa";
                cxn.password = "adminadmin";
                cxn.puertoServer = "1433";
                cxn.tipoServer = TypeDB.SQLServer;
                cxn.server = "localhost";
                cxn.nombreBd = "DBSimple";//DBMaster

                using (SqlConnection conn = (SqlConnection)EntityFrameworkCxn.createDbConnection(cxn))
                {
                    // Create the connectionString
                    // Trusted_Connection is used to denote the connection uses Windows Authentication
                    ////// conn.ConnectionString = "Server=[server_name];Database=[database_name];Trusted_Connection=true";
                    conn.Open();
                    // Create the command
                    // query = "SELECT * FROM Estados x0  WHERE x0.descripcion LIKE '%oa%'";
                    SqlCommand command = new SqlCommand(query, conn);
                    // Add the parameters.

                    foreach (var item in valoresParametrosQuery)
                    {
                        command.Parameters.Add(new SqlParameter(item.Key, item.Value));

                    }
                    ///command.Parameters.Add(new SqlParameter("0", 1));

                    /* Get the rows and display on the screen! 
                     * This section of the code has the basic code
                     * that will display the content from the Database Table
                     * on the screen using an SqlDataReader. */


                    //var serializer = new WestwindJsonSerializer
                    //{
                    //    DateSerializationMode = JsonDateEncodingModes.Iso
                    //};
                    object[] valores;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("FirstColumn\tSecond Column\t\tThird Column\t\tForth Column\t");
                        int cont = 0, columnas = reader.FieldCount;
                        while (reader.Read())
                        {
                            valores = new object[tipoDatosSelect.Count];
                            foreach (var item in tipoDatosSelect)
                            {
                                instancia = generaInstanciaValores(reader, item, cont);
                                valores[cont] = instancia;
                                cont++;
                                Console.WriteLine(instancia);
                            }
                            resultado.Add(valores);
                            cont = 0;
                            //instancia = generaInstanciaValores(reader, clasePrincipal, cont);
                            //resultado.Add(instancia);

                            ////////foreach (PropertyInfo item in propertie)
                            ////////{
                            ////////    if (!item.PropertyType.FullName.Equals("System.Collections.Generic"))
                            ////////    {
                            ////////        if (item.PropertyType.FullName.StartsWith("Modelo.entidad"))
                            ////////        {

                            ////////        }
                            ////////        PropertyInfo numberPropertyInfo = clasePrincipal.GetProperty(item.Name);
                            ////////        object val = reader[cont];
                            ////////        /// convertirObjectTipoDato(numberPropertyInfo.PropertyType, val)
                            ////////        numberPropertyInfo.SetValue(instancia, val, null);
                            ////////        cont++;
                            ////////        countCol++;
                            ////////        if (countCol >= columnas)
                            ////////        {

                            ////////        }
                            ////////        Console.WriteLine(item.Name + " " + item.PropertyType);
                            ////////    }
                            ////////}
                            //   Console.WriteLine(Convert.ToString(reader("nombrecampo")));
                            ////Console.WriteLine(reader["clave"]);
                            ////Console.WriteLine(reader["Descripcion"]);
                            ///Console.WriteLine(reader[0]);
                        }
                    }
                }
                Console.WriteLine(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private object generaInstanciaValores(SqlDataReader reader, Type clasePrincipal, int index)
        {
            object instancia = null;
            PropertyInfo[] propertie;
            if (clasePrincipal != null)
            {
                if (clasePrincipal.FullName.StartsWith("System"))
                {
                    instancia = reader[index];
                    return instancia;
                }
                else
                {
                    instancia = Activator.CreateInstance(clasePrincipal);
                    propertie = clasePrincipal.GetProperties();
                }
            }
            else
            {
                propertie = new PropertyInfo[] { };
            }

            foreach (PropertyInfo item in propertie)
            {
                if (!item.PropertyType.FullName.StartsWith("System.Collections.Generic"))
                {
                    PropertyInfo numberPropertyInfo = clasePrincipal.GetProperty(item.Name);
                    if (item.PropertyType.FullName.StartsWith(nameProject + ".entidad"))
                    {
                        numberPropertyInfo.SetValue(instancia, generaInstanciaValores(reader, item.PropertyType, index), null);
                    }
                    else
                    {
                        // object val = reader[index];
                        Console.WriteLine(reader[index]);
                        /// convertirObjectTipoDato(numberPropertyInfo.PropertyType, val)
                       // numberPropertyInfo.SetValue(instancia, val, null);
                        if (!Convert.IsDBNull(reader[index]))
                            numberPropertyInfo.SetValue(instancia, reader[index], null);
                        index++;
                    }

                }
            }
            return instancia;
        }

        private object convertirObjectTipoDato(Type tipoDato, object valor)
        {
            if (tipoDato.Equals(typeof(Decimal)) | tipoDato.Equals(typeof(decimal)))
            {
                return Convert.ToDecimal(valor);
            }
            return valor;
        }
        #endregion

        #region genera mapeo de tablas usadas para la creacion de la consulta
        private void generaListaTablasMapeadas(List<CamposSelect> listCamposSelect, List<CamposFrom> listCamposFrom, List<CamposWhere> listCamposWhere, List<CamposGrupo> listCamposGrupo, List<CamposOrden> listCamposOrden)
        {
            List<string> tablasMapeadas = new List<string>();
            listCamposSelect = listCamposSelect == null ? new List<CamposSelect>() : listCamposSelect;
            listCamposFrom = listCamposFrom == null ? new List<CamposFrom>() : listCamposFrom;
            listCamposWhere = listCamposWhere == null ? new List<CamposWhere>() : listCamposWhere;
            listCamposGrupo = listCamposGrupo == null ? new List<CamposGrupo>() : listCamposGrupo;
            listCamposOrden = listCamposOrden == null ? new List<CamposOrden>() : listCamposOrden;

            if (listCamposFrom.Count > 0)
            {
                foreach (CamposFrom item in listCamposFrom)
                {
                    tablasMapeadas.Add(item.campo);
                }
            }

            if (listCamposSelect.Count > 0)
            {
                foreach (CamposSelect item in listCamposSelect)
                {
                    tablasMapeadas.Add(item.campoMostrar);
                }
            }

            if (listCamposWhere.Count > 0)
            {
                foreach (CamposWhere item in listCamposWhere)
                {
                    tablasMapeadas.Add(item.campo);
                }
            }

            if (listCamposGrupo.Count > 0)
            {
                foreach (CamposGrupo item in listCamposGrupo)
                {
                    tablasMapeadas.Add(item.campo);
                }
            }

            if (listCamposOrden.Count > 0)
            {
                foreach (CamposOrden item in listCamposOrden)
                {
                    tablasMapeadas.Add(item.campo);
                }
            }

            string[] ruta;
            int pos = 1, numAlias = countTablasJoin;
            bool existeTabla = false;
            StringBuilder rutaTabla = new StringBuilder();
            foreach (string item in tablasMapeadas)
            {
                Type tipoDato;
                Mensaje m;
                try
                {
                    ruta = item.Split('.');
                    if (ruta.Length == 1)
                    {
                        tipoDato = null;
                        if (ruta[0].Trim().Length > 0)
                        {
                            m = buscarTipoDatoCampo(ruta[0]);
                            if (m.noError != 0)
                            {
                                break;
                            }
                            tipoDato = (Type)m.resultado;
                            if (tipoDato.Namespace.Equals(nameProject + ".entidad"))
                            {
                                pos = 0;
                            }
                            else
                            {
                                pos = 1;
                            }
                        }
                    }
                    else
                    {
                        pos = 1;
                    }
                    pos = 0; ///por pruebas
                    for (int i = 0; i < ruta.Length - pos; i++)  ////pendiente
                    {
                        rutaTabla.Append(ruta[i]);
                        existeTabla = tablasJoin.Contains(rutaTabla.ToString());
                        if (!existeTabla)
                        {
                            m = buscarTipoDatoCampo(rutaTabla.ToString()); //.Replace('_', '.').
                            if (m.noError != 0)
                            {
                                break;
                            }
                            tipoDato = (Type)m.resultado;
                            if (tipoDato.FullName.StartsWith(nameProject + ".entidad") & !tipoDato.IsEnum)
                            {
                                aliasTablaJoiner.Add(rutaTabla.ToString(), new CamposJoin(tipoDato, string.Concat(nombreAlias, numAlias.ToString()), ruta[i], String.Concat(ruta[i], "_ID")));
                                numAlias++;
                                tablasJoin.Add(rutaTabla.ToString());
                            }
                            //tipoDatosSelect.Add(tipoDato);
                        }
                        existeTabla = false;
                        rutaTabla.Append(".");
                    }
                    rutaTabla.Clear();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            //agrega joins a campos
            if (listCamposFrom.Count > 0)
            {
                foreach (CamposFrom item in listCamposFrom)
                {
                    if (aliasTablaJoiner.ContainsKey(item.campo))
                    {
                        CamposJoin c = aliasTablaJoiner[item.campo];
                        c.tipoJoin = item.tipoJoin;
                    }
                }
            }
            countTablasJoin = numAlias;
        }
        #endregion

        private Mensaje buscarTipoDatoCampo(string campo)
        {
            mensaje = inicializaMensaje();
            try
            {
                Type tipoDato = null;
                string[] path = campo.Split('.');
                Assembly asm = Assembly.Load(nameProject);
                if ((campo.ToUpper().StartsWith("CFDI")) || (campo.ToUpper().StartsWith("CERT")))
                {
                    tipoDato = asm.GetType(nameProject + ".entidad.cfdi." + (path[0]));
                }
                else
                {
                    tipoDato = asm.GetType(nameProject + ".entidad." + (path[0]));
                }
                mensaje.resultado = tipoDato;
                if (path.Length > 1)
                {
                    //List<FieldInfo> fields = tipoDato.GetRuntimeFields().ToList();
                    //var a = (from fiel in fields where fiel.Name.Contains(path[1]) select fiel);
                    //FieldInfo field = null;
                    PropertyInfo field = tipoDato.GetProperty(path[1]);
                    if (field.PropertyType.Namespace.Equals("System.Collections.Generic"))
                    {
                        tipoDato = field.PropertyType.GenericTypeArguments[0];
                    }
                    else
                    {
                        tipoDato = field.PropertyType;
                    }
                    mensaje.resultado = tipoDato;
                    if (path.Length > 2)
                    {
                        int i;
                        StringBuilder ruta = new StringBuilder(tipoDato.Name);
                        for (i = 2; i < path.Length; i++)
                        {
                            ruta.Append(".").Append(path[i]);
                        }
                        mensaje = buscarTipoDatoCampo(ruta.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                mensaje.noError = 1;
                mensaje.error = e.Message;
            }

            return mensaje;
        }
        public DatosQuery construyeQueryDatos(OperadorSelect campo, List<CamposFrom> listCamposFrom, List<CamposWhere> listCamposWhere, List<CamposWhere> camposWhereExtras, List<CamposGrupo> listCamposGrupo, List<CamposOrden> listCamposOrden)
        {
            aliasTablaJoiner = new Dictionary<string, CamposJoin>();
            valoresParametrosQuery = new Dictionary<string, object>();
            DatosQuery datosQuery = new DatosQuery();
            StringBuilder consulta = new StringBuilder();
            generaListaTablasMapeadas(campo.listCamposSelect, listCamposFrom, listCamposWhere, listCamposGrupo, listCamposOrden);
            consulta.Append(construyeSelectDatos(campo, TipoOperacion.SELECT)).Append(" ");
            consulta.Append(construyeFromConsulta(TipoJoin.LEFT_JOIN)).Append(" ");
            listCamposWhere.AddRange(camposWhereExtras);
            consulta.Append(construyeCamposWhere(listCamposWhere)).Append(" ");

            datosQuery.aliasTablas = aliasTablaJoiner;
            datosQuery.parametrosCampos = valoresParametrosQuery;
            datosQuery.query = consulta.ToString();
            datosQuery.conParametros = true;

            return datosQuery;
        }
        private Mensaje inicializaMensaje()
        {
            Mensaje m = new Mensaje();
            m.noError = 0;
            return m;
        }

        #region Crea conexion con la base de datos (no va aqui este)
        //private static DbConnection genereDbConnection(Conexion conexion)
        //{
        //    DbConnection dbConnection = null;
        //    String stringConexion = "";
        //    if (conexion.TipoServidor == TipoBD.SQLServer)
        //    {
        //        stringConexion = @"Data Source=.\" + conexion.NombreServidor + "," + conexion.PuertoServidor + ";Initial Catalog=" + conexion.NombreBD + ";User ID=" + conexion.Usuario
        //            + ";Password=" + conexion.Password + ";Trusted_Connection=False;";
        //        SqlConnection connection = new SqlConnection(stringConexion);
        //        dbConnection = connection;
        //    }
        //    else if (conexion.TipoServidor == TipoBD.MYSql)
        //    {
        //        ////stringConexion = "server=" + conexion.NombreServidor + ";port=" + conexion.PuertoServidor + ";database=" + conexion.NombreBD + ";uid=" + conexion.Usuario
        //        ////    + ";pwd=" + conexion.Password;
        //        ////stringConexion = "Data Source=" + conexion.NombreServidor + ";Port=" + conexion.PuertoServidor + ";Database=" + conexion.NombreBD + ";UID=" + conexion.Usuario
        //        ////    + ";PWD=" + conexion.Password + ";";
        //        ////MySqlConnection connection = new MySqlConnection(stringConexion);
        //        ////dbConnection = connection;
        //    }

        //    //connectionString="server=localhost;user id=user;password=mypass;database=mydb"     mysql
        //    //////string connectionString = "server=localhost;port=3305;database=parking;uid=root";    mysql
        //    // //@"Data Source=.\SQLEXPRESS;Initial Catalog=mindala;User ID=sa;Password=adminadmin;Trusted_Connection=False;"   sql
        //    return dbConnection;
        //}
        #endregion
    }
}
