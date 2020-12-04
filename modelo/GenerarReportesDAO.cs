using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using Exitosw.Payroll.Hibernate.util;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modelo
{
    public class GenerarReportesDAO : NHibernateRepository<Object>, GenerarReportesDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Server").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        //List<Cursos> listaCursos = new List<Cursos>();
        IQuery query;
        #region reporte de nomina
        public Mensaje getMovimientosNomina(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session)
        {
            try
            {

                inicializaVariableMensaje();
                setSession(session);
                List<object> datosReporte = new List<object>();
                //Datos generales del empleado 
                String genQueryCount = construirQueryDatosEmpleados(values["filtrosPersonalizados"].ToString(), values["filtrosOrden"].ToString());
                query = getSession().CreateQuery(genQueryCount);
                query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                query.SetString("fechaInicial", values["fechaInicial"].ToString());
                query.SetString("fechaFinal", values["fechaFinal"].ToString());
                var datosemp = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                //var datosemp = query.List<object>();
                Dictionary<string, object> reporte = new Dictionary<string, object>();
                Dictionary<string, object> reporteFinal = new Dictionary<string, object>();
                //  reporte["DatosEmpleado"] = datosemp;
                //obtener los dias trabajados
                for (int i = 0; i < datosemp.Count; i++)
                {
                    reporte = new Dictionary<string, object>();
                    Dictionary<string, object> emp = (Dictionary<string, object>)datosemp[i];
                    string queryDias = construirQueryDatosConcep(values["prioridadNaturaleza"].ToString());
                    query = getSession().CreateQuery(queryDias);
                    query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                    query.SetString("fechaInicial", values["fechaInicial"].ToString());
                    query.SetString("fechaFinal", values["fechaFinal"].ToString());
                    query.SetParameterList("meses", (List<int>)values["meses"]);
                    query.SetString("claveEmpleado", emp["claveEmpleado"].ToString());
                    var datosConcep = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                    //var datosConcep = query.List<object>();

                    string querySubtotal = construirQuerySubTotalConcep(values["filtrosPersonalizados"].ToString());
                    query = getSession().CreateQuery(querySubtotal);
                    query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                    query.SetString("fechaInicial", values["fechaInicial"].ToString());
                    query.SetString("fechaFinal", values["fechaFinal"].ToString());
                    query.SetParameterList("meses", (List<int>)values["meses"]);
                    query.SetString("claveDepartamentos", emp["claveDepartamentos"].ToString());
                    var datosSubConcep = query.SetResultTransformer(new DictionaryResultTransformer()).List();

                    reporte["DatosEmpleado"] = datosemp[i];
                    reporte["DatosConceptos"] = datosConcep;
                    reporte["DatosSubTotConcep"] = datosSubConcep;

                    datosReporte.Add(reporte);

                }
                string querytotales = construirQueryTotalesConcep(values["filtrosPersonalizados"].ToString());
                query = getSession().CreateQuery(querytotales);
                query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                query.SetString("fechaInicial", values["fechaInicial"].ToString());
                query.SetString("fechaFinal", values["fechaFinal"].ToString());
                //query.SetParameterList("meses",(List<int>)values["meses"]);
                //query.SetString("claveDepartamentos", "001");
                var datosTotalConcep = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                //var datosTotalConcep = query.List<object>();
                reporteFinal["empleados"] = datosReporte;
                reporteFinal["totalConcep"] = datosTotalConcep;
                //reporte["DatosConceptos"] = datosConcep;
                mensajeResultado.resultado = reporteFinal;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Transaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMovimientosNomina()1_Error: ").Append(ex));
                //if (getSession().Transaction.IsActive)
                //{
                //    getSession().Transaction.Rollback();
                //}
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        public string construirQueryDatosEmpleados(string filtrosPersonalizados, string filtrosOrden)
        {
            concatena.Remove(0, concatena.Length);
            concatena.Append("select  DISTINCT ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.clave IS NULL) THEN '' ELSE emp.clave END END) as claveEmpleado, ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.nombre IS NULL) THEN '' ELSE emp.nombre END END) as nombreEmpleado, ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoPaterno IS NULL) THEN '' ELSE emp.apellidoPaterno END END)as apelliPaternoEmpleado, ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoMaterno IS NULL) THEN '' ELSE emp.apellidoMaterno END END)as apelliMaternoEmpleado, ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.IMSS IS NULL) THEN '' ELSE emp.IMSS END END)as IMMSEmpleado, ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.RFC IS NULL) THEN '' ELSE emp.RFC END END)as RFCEmpleado, ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.CURP IS NULL) THEN '' ELSE emp.CURP END END)as CURPEmpleado, ");
            concatena.Append("(select CASE WHEN (emp2 IS NULL) THEN cast('1900-01-01' as date) ELSE CASE WHEN (ing.fechaIngreso IS NULL) THEN cast('1900-01-01' as date) ELSE ing.fechaIngreso END END ");
            concatena.Append("from IngresosBajas ing inner join ing.empleados emp2 inner join ing.razonesSociales rs2 ");
            concatena.Append("where emp2.id=emp.id and rs2.clave=rs.clave) as fechaIngresoEmpleado, ");
            concatena.Append("(CASE WHEN (pm.salarioPor IS 1) THEN pm.puestos.salarioTabular ELSE pm.importe  END) as salarioFijo, ");
            concatena.Append("(CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.clave IS NULL) THEN '' ELSE dep.clave END END)as claveDepartamentos, ");
            concatena.Append("(CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.descripcion IS NULL) THEN '' ELSE dep.descripcion END END)as descripcionDepartamentos, ");
            concatena.Append("(CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.clave IS NULL) THEN '' ELSE cent.clave END END)as claveCentrodeCostos, ");
            concatena.Append("(CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.descripcion IS NULL) THEN '' ELSE cent.descripcion END END)as descripcionCentrodeCostos, ");
            concatena.Append("(CASE WHEN (pm.salarioPor IS 1) THEN pm.puestos.salarioTabular ELSE pm.importe  END) as salarioDiarioFijo,sdi.salarioDiarioIntegrado as salarioDiarioIntegral ");

            concatena.Append("FROM PlazasPorEmpleadosMov pm INNER JOIN pm.plazasPorEmpleado pl RIGHT OUTER JOIN pl.empleados emp ");
            concatena.Append("INNER JOIN emp.razonesSociales rs  LEFT OUTER JOIN pm.departamentos dep LEFT OUTER JOIN pm.centroDeCosto cent, ");
            concatena.Append("MovNomConcep mov RIGHT OUTER JOIN mov.concepNomDefi cnc RIGHT OUTER JOIN mov.tipoCorrida tipcorr RIGHT OUTER JOIN mov.periodosNomina per ");
            concatena.Append("RIGHT OUTER JOIN mov.empleados empMov RIGHT OUTER JOIN mov.tipoNomina tipNom RIGHT OUTER JOIN pl.registroPatronal reg ");
            concatena.Append("LEFT  OUTER JOIN pm.plazas plaz   LEFT  OUTER JOIN plaz.categoriasPuestos cat LEFT  OUTER JOIN plaz.puestos pue ");
            concatena.Append("LEFT  OUTER JOIN plaz.turnos tur    LEFT  OUTER JOIN pm.tipoContrato tipCon, ");
            concatena.Append("SalariosIntegrados sdi ");

            concatena.Append("where sdi.empleados.id= emp.id ");
            concatena.Append("AND sdi.fecha = (SELECT MAX(s.fecha) FROM SalariosIntegrados s WHERE s.fecha <= GETDATE() ");
            concatena.Append("AND s.empleados.id = sdi.empleados.id AND s.empleados.id = pl.empleados.id) ");
            concatena.Append("AND rs.clave = :claveRazonsocial ");
            concatena.Append("AND empMov.id  = emp.id ");
            concatena.Append("AND ((per.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per.fechaFinal BETWEEN :fechaInicial AND  :fechaFinal)) ");

            concatena.Append("AND ").Append(filtrosPersonalizados);
            concatena.Append(" AND pm.id IN (");
            concatena.Append("SELECT x0 FROM PlazasPorEmpleadosMov x0 LEFT OUTER JOIN x0.plazasPorEmpleado x1 LEFT OUTER JOIN ");
            concatena.Append("x1.razonesSociales x2 LEFT OUTER JOIN x1.empleados x3 WHERE x2.clave = :claveRazonsocial ");
            concatena.Append("AND x0.fechaInicial IN (SELECT max(x0X.fechaInicial) FROM PlazasPorEmpleadosMov x0X LEFT OUTER JOIN x0X.plazasPorEmpleado x1X ");
            concatena.Append("LEFT OUTER JOIN x1X.empleados x3X WHERE (x0X.fechaInicial <= :fechaFinal ) AND x3.id = x3X.id))");
            concatena.Append(" ").Append(filtrosOrden);

            return concatena.ToString();
        }

        public string construirQueryDatosConcep(string prioridadNaturaleza)
        {
            concatena.Remove(0, concatena.Length);
            concatena.Append("SELECT cnc.clave as claveConcepNomDefi, cnc.descripcion as descripcionConcepNomDefi,cnc.naturaleza as naturalezaConcepNomDefi, ");
            concatena.Append("CASE WHEN (cnc.naturaleza IS NULL) THEN 0.0 ELSE CASE WHEN (cnc.naturaleza = 1) THEN sum(mov.resultado) ELSE 0.0 END END as Percepciones, ");
            concatena.Append("CASE WHEN (cnc.naturaleza IS NULL) THEN 0.0 ELSE CASE WHEN (cnc.naturaleza = 2) THEN sum(mov.resultado) ELSE 0.0 END END as Deducciones, ");
            concatena.Append("CASE WHEN (cnc.naturaleza IS NULL) THEN 0.0 ELSE (CASE WHEN (cnc.naturaleza =3) THEN sum(mov.resultado) ELSE ");
            concatena.Append("CASE WHEN (cnc.naturaleza = 4) THEN sum(mov.resultado) ELSE CASE WHEN (cnc.naturaleza = 5) THEN ");
            concatena.Append("sum(mov.resultado) ELSE 0.0 END END END) END as Dato, ");

            concatena.Append("(SELECT  CASE WHEN(Hx13 IS NULL) THEN  '0.0' ELSE CASE WHEN (Hx13.tipo = 'INTEGER' AND Hx13.clasificadorParametro = '0') ");
            concatena.Append("THEN CAST(SUM(CAST(CASE WHEN (Hx6 IS NULL) THEN '0.0' ELSE CASE WHEN (Hx6.valor IS NULL) THEN '0.0' ELSE Hx6.valor END END as float)) as string) ");
            concatena.Append("ELSE  MAX(CASE WHEN (Hx6 IS NULL) THEN '0.0' ELSE CASE WHEN (Hx6.valor IS NULL) THEN '0.0' ELSE Hx6.valor END END)  END END ");
            concatena.Append("FROM MovNomConcep Hx3 LEFT OUTER JOIN Hx3.tipoNomina Hx9 LEFT OUTER JOIN Hx3.tipoCorrida Hx8 LEFT OUTER JOIN Hx3.periodosNomina Hx5 ");
            concatena.Append("LEFT OUTER JOIN Hx3.concepNomDefi Hx4 LEFT OUTER JOIN Hx3.empleados Hx10 LEFT OUTER JOIN Hx3.plazasPorEmpleado Hx11 LEFT OUTER JOIN ");
            concatena.Append("Hx3.razonesSociales Hx12 LEFT OUTER JOIN Hx3.movNomConceParam Hx6 LEFT OUTER JOIN Hx6.paraConcepDeNom Hx13 ");
            concatena.Append("WHERE Hx9.clave = mov.tipoNomina.clave AND Hx4.clave = cnc.clave AND Hx10.clave = :claveEmpleado ");
            concatena.Append("AND Hx12.clave = :claveRazonsocial AND Hx8.clave = tipcorr.clave ");
            concatena.Append("AND ((Hx5.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (Hx5.fechaFinal BETWEEN :fechaInicial AND :fechaFinal)) ");
            concatena.Append("AND Hx13.id= paCncNom.id AND Hx3.mes IN(:meses) ");
            concatena.Append("group by Hx13.id,Hx13.tipo,Hx13.clasificadorParametro) as valorParam ");

            concatena.Append("From MovNomConcep mov RIGHT OUTER JOIN mov.concepNomDefi cnc ");
            concatena.Append("LEFT OUTER JOIN mov.movNomConceParam movCncPam LEFT OUTER JOIN movCncPam.paraConcepDeNom paCncNom RIGHT OUTER JOIN mov.tipoCorrida tipcorr ");
            concatena.Append("RIGHT OUTER JOIN mov.periodosNomina per RIGHT OUTER JOIN ");
            concatena.Append("mov.tipoNomina tipNom RIGHT OUTER JOIN mov.empleados x9 RIGHT OUTER JOIN mov.plazasPorEmpleado x10 ");
            concatena.Append("RIGHT OUTER JOIN mov.razonesSociales x11 ");

            concatena.Append("where (paCncNom.numero = null or  paCncNom.numero = 1) ");
            concatena.Append("AND ((per.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per.fechaFinal BETWEEN :fechaInicial AND :fechaFinal)) ");
            concatena.Append("AND x11.clave = :claveRazonsocial AND x9.razonesSociales.clave = :claveRazonsocial ");
            concatena.Append("AND mov.mes IN (:meses) ").Append(prioridadNaturaleza).Append(" ");
            concatena.Append("AND x9.clave= :claveEmpleado ");

           concatena.Append("group by cnc.clave,cnc.descripcion,mov.tipoNomina.id,mov.tipoNomina.clave,tipcorr.clave,cnc.naturaleza,paCncNom.id,paCncNom.tipo,paCncNom.clasificadorParametro ORDER BY  cast(cnc.clave as integer)");

            //concatena.Append("order by cnc.clave ");
            return concatena.ToString();
        }

        public string construirQuerySubTotalConcep(string filtrosPersonalizados)
        {

            concatena.Remove(0, concatena.Length);
            concatena.Append("SELECT  DISTINCT  cnc.clave as claveConcepNomDefi, cnc.descripcion as descripcionConcepNomDefi, cnc.naturaleza as naturalezaConcepNomDefi, ");
            concatena.Append("CASE WHEN (cnc.naturaleza IS NULL) THEN 0.0 ELSE CASE WHEN (cnc.naturaleza = 1) THEN sum(mov.resultado) ELSE 0.0 END END as subTotalPercepcion, ");
            concatena.Append("CASE WHEN (cnc.naturaleza IS NULL) THEN 0.0 ELSE CASE WHEN (cnc.naturaleza = 2) THEN sum(mov.resultado) ELSE 0.0 END END as subTotalDeduccion ");

            concatena.Append("From PlazasPorEmpleadosMov pm ");
            concatena.Append("LEFT JOIN pm.plazasPorEmpleado pl LEFT OUTER JOIN pl.empleados emp, ");
            concatena.Append("MovNomConcep mov RIGHT OUTER JOIN mov.concepNomDefi cnc LEFT OUTER JOIN pl.razonesSociales rs ");
            concatena.Append("RIGHT OUTER JOIN pl.registroPatronal reg RIGHT OUTER JOIN mov.tipoCorrida tipcorr ");
            concatena.Append("RIGHT OUTER JOIN mov.periodosNomina per RIGHT OUTER JOIN ");
            concatena.Append("mov.tipoNomina tipNom RIGHT OUTER JOIN mov.empleados x9 RIGHT OUTER JOIN mov.plazasPorEmpleado x10 ");
            concatena.Append("RIGHT OUTER JOIN mov.razonesSociales x11 RIGHT OUTER JOIN mov.empleados x9 ");
            concatena.Append("LEFT OUTER JOIN pm.departamentos dep LEFT OUTER JOIN pm.centroDeCosto cent LEFT  OUTER JOIN pm.plazas plaz ");
            concatena.Append("LEFT  OUTER JOIN plaz.categoriasPuestos cat LEFT  OUTER JOIN plaz.puestos pue  ");
            concatena.Append("LEFT  OUTER JOIN plaz.turnos tur   LEFT OUTER JOIN pm.tipoContrato tipCon, ");
            concatena.Append("SalariosIntegrados sdi ");

            concatena.Append("where sdi.empleados.id= emp.id ");
            concatena.Append("AND sdi.fecha = (SELECT MAX (s.fecha) FROM SalariosIntegrados s WHERE s.fecha <= GETDATE() ");
            concatena.Append("AND s.empleados.id = sdi.empleados.id AND s.empleados.id = pl.empleados.id) ");
            concatena.Append("AND x9.id  = emp.id ");
            concatena.Append("AND ((per.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per.fechaFinal BETWEEN :fechaInicial AND :fechaFinal)) ");
            concatena.Append("AND cnc.naturaleza <=1 AND rs.clave = :claveRazonsocial ");
            concatena.Append("AND ").Append(filtrosPersonalizados).Append(" ");
            concatena.Append("AND mov.mes IN (:meses) AND dep.clave = :claveDepartamentos ");
            concatena.Append("AND pm.id IN (");
            concatena.Append("SELECT pm.id FROM PlazasPorEmpleadosMov pm LEFT OUTER JOIN pm.plazasPorEmpleado pl LEFT OUTER JOIN ");
            concatena.Append("pl.razonesSociales rs LEFT OUTER JOIN pl.empleados x3 WHERE rs.clave = :claveRazonsocial ");
            concatena.Append("AND pm.fechaInicial IN (SELECT max(x0X.fechaInicial) FROM PlazasPorEmpleadosMov x0X LEFT OUTER JOIN x0X.plazasPorEmpleado x1X ");
            concatena.Append("LEFT OUTER JOIN x1X.empleados x3X WHERE (x0X.fechaInicial <= :fechaFinal ) AND emp.id = x3X.id)) ");
            concatena.Append("group by cnc.clave,cnc.descripcion,mov.tipoNomina, cnc.naturaleza  ");

            return concatena.ToString();
        }

        public string construirQueryTotalesConcep(string filtrosPersonalizados)
        {
            concatena.Remove(0, concatena.Length);
            concatena.Append("SELECT  DISTINCT  cnc.clave as claveConcepNomDefi, cnc.descripcion as descripcionConcepNomDefi, cnc.naturaleza as naturalezaConcepNomDefi, ");
            concatena.Append("CASE WHEN (cnc.naturaleza IS NULL) THEN 0.0 ELSE CASE WHEN (cnc.naturaleza = 1) THEN sum(mov.resultado) ELSE 0.0 END END as subTotalPercepcion, ");
            concatena.Append("CASE WHEN (cnc.naturaleza IS NULL) THEN 0.0 ELSE CASE WHEN (cnc.naturaleza = 2) THEN sum(mov.resultado) ELSE 0.0 END END as subTotalDeduccion ");

            concatena.Append("From PlazasPorEmpleadosMov pm ");
            concatena.Append("LEFT JOIN pm.plazasPorEmpleado pl LEFT OUTER JOIN pl.empleados emp, ");
            concatena.Append("MovNomConcep mov RIGHT OUTER JOIN mov.concepNomDefi cnc LEFT OUTER JOIN pl.razonesSociales rs ");
            concatena.Append("RIGHT OUTER JOIN pl.registroPatronal reg RIGHT OUTER JOIN mov.tipoCorrida tipcorr ");
            concatena.Append("RIGHT OUTER JOIN mov.periodosNomina per RIGHT OUTER JOIN ");
            concatena.Append("mov.tipoNomina tipNom RIGHT OUTER JOIN mov.empleados x9 RIGHT OUTER JOIN mov.plazasPorEmpleado x10 ");
            concatena.Append("RIGHT OUTER JOIN mov.razonesSociales x11 RIGHT OUTER JOIN mov.empleados x9 ");
            concatena.Append("LEFT OUTER JOIN pm.departamentos dep LEFT OUTER JOIN pm.centroDeCosto cent LEFT  OUTER JOIN pm.plazas plaz ");
            concatena.Append("LEFT OUTER JOIN plaz.categoriasPuestos cat LEFT  OUTER JOIN plaz.puestos pue  LEFT OUTER JOIN plaz.turnos tur ");
            concatena.Append(" LEFT OUTER JOIN pm.tipoContrato tipCon, ");
            concatena.Append("SalariosIntegrados sdi ");

            concatena.Append("where sdi.empleados.id = emp.id ");
            concatena.Append("AND sdi.fecha = (SELECT MAX (s.fecha) FROM SalariosIntegrados s WHERE s.fecha <= GETDATE() ");
            concatena.Append("AND s.empleados.id = sdi.empleados.id AND s.empleados.id = pl.empleados.id) ");
            concatena.Append("AND x9.id = emp.id ");
            concatena.Append("AND ((per.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per.fechaFinal BETWEEN :fechaInicial AND  :fechaFinal))");
            concatena.Append("AND cnc.naturaleza <=2 AND rs.clave = :claveRazonsocial ");
            concatena.Append("AND ").Append(filtrosPersonalizados).Append(" ");
            concatena.Append("AND pm.id IN (");
            concatena.Append("SELECT pm.id FROM PlazasPorEmpleadosMov pm LEFT OUTER JOIN pm.plazasPorEmpleado pl LEFT OUTER JOIN ");
            concatena.Append("pl.razonesSociales rs LEFT OUTER JOIN pl.empleados x3 WHERE rs.clave = :claveRazonsocial ");
            concatena.Append("AND pm.fechaInicial IN (SELECT max(x0X.fechaInicial) FROM PlazasPorEmpleadosMov x0X LEFT OUTER JOIN x0X.plazasPorEmpleado x1X ");
            concatena.Append("LEFT OUTER JOIN x1X.empleados x3X WHERE (x0X.fechaInicial <= :fechaFinal ) AND emp.id = x3X.id)) ");
            concatena.Append("group by cnc.clave,cnc.descripcion,mov.tipoNomina, cnc.naturaleza ORDER BY cnc.clave");

            return concatena.ToString();
        }

        #endregion

        public Mensaje getMovimientosReciboNomina(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session)
        {
            ///---
            throw new NotImplementedException();
        }

        #region Base Gravables
        public Mensaje getReportesBaseGravables(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session)
        {
            try
            {
                setSession(session);
                Dictionary<string, object> reporteFinal = new Dictionary<string, object>();
                String genQueryCount = construirQueryMovBaseGravables(values["filtrosPersonalizados"].ToString(), values["filtrosPersonalizados2"].ToString(), values["filtrosOrden"].ToString());
                query = getSession().CreateQuery(genQueryCount);
                query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                query.SetString("fechaInicial", values["fechaInicial"].ToString());
                query.SetString("fechaFinal", values["fechaFinal"].ToString());
                //var datosemp = query.List();
                var datosemp = query.SetResultTransformer(new DictionaryResultTransformer()).List();

                String genQueryCountOtros = construirQueryMovBaseGravablesOtros(values["filtrosPersonalizados"].ToString(), values["filtrosOrden"].ToString());
                query = getSession().CreateQuery(genQueryCountOtros);
                query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                query.SetString("fechaInicial", values["fechaInicial"].ToString());
                query.SetString("fechaFinal", values["fechaFinal"].ToString());

                var datosempOtros = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                reporteFinal["baseGravables"] = datosemp;
                reporteFinal["baseGravablesOtros"] = datosempOtros;
                mensajeResultado.resultado = reporteFinal;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                // getSession().Transaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReportesMovimientos()1_Error: ").Append(ex));
                //if (getSession().Transaction.IsActive)
                //{
                //    getSession().Transaction.Rollback();
                //}
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        private string construirQueryMovBaseGravables(string filtrosPersonalizados, string filtrosPersonalizados2, string filtrosOrden) {
            concatena.Remove(0, concatena.Length);
            concatena.Append("SELECT ");
            concatena.Append("(CASE WHEN (cnc IS NULL) THEN '' ELSE CASE WHEN (cnc.clave IS NULL) THEN '' ELSE cnc.clave END END) as claveConcepNomDefi, ");
            concatena.Append("(CASE WHEN  (cnc IS NULL) THEN '' ELSE CASE WHEN (cnc.clave IS NULL) THEN '' ELSE cnc.descripcion ");
            concatena.Append("END END)as descripcionConcepNomDefi,cnc.naturaleza as naturalezaConcepNomDefi, ");
            concatena.Append("CASE WHEN (SUM(mov.resultado) IS NULL) THEN 0.0 ELSE SUM(mov.resultado) END  as importe, ");
            concatena.Append("(SELECT CASE WHEN (COUNT(baseAfectmov)=0) THEN 0.0 ELSE (SUM(CASE WHEN (baseNom.clave IS NULL) THEN 0.0 ELSE CASE WHEN ");
            concatena.Append("(baseNom.clave ='01')  THEN baseAfectmov.resultado ELSE 0.0  END END))END  as gravadoIS ");
            concatena.Append("From MovNomBaseAfecta baseAfectmov LEFT OUTER  JOIN baseAfectmov.baseAfecConcepNom baseAfectConce LEFT OUTER ");
            concatena.Append("JOIN  baseAfectConce.baseNomina baseNom ");
            concatena.Append("LEFT OUTER JOIN  baseAfectmov.movNomConcep mov1  LEFT OUTER JOIN mov1.concepNomDefi cnc1 ");
            concatena.Append(" RIGHT OUTER JOIN mov1.tipoNomina tipNom1 ");
            concatena.Append("RIGHT OUTER JOIN mov1.tipoCorrida tipcorr1 RIGHT OUTER JOIN mov1.periodosNomina per1 ");
            concatena.Append("where ((per1.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per1.fechaFinal BETWEEN :fechaInicial AND  :fechaFinal)) ");
            concatena.Append("AND (cnc1.naturaleza = 1 or  cnc1.naturaleza =2) ");
            concatena.Append("AND cnc1.clave=cnc.clave ");
            concatena.Append(filtrosPersonalizados2.ToString()).Append(" ");
            concatena.Append(")as gravadoISR, ");
            concatena.Append("(SELECT CASE WHEN (COUNT(baseAfectmov)=0) THEN 0.0 ELSE (SUM(CASE WHEN (baseNom.clave IS NULL) THEN 0.0 ELSE CASE WHEN ");
            concatena.Append("(baseNom.clave ='01')  THEN baseAfectmov.resultadoExento ELSE 0.0  END END))END  as exentoISR ");
            concatena.Append(" From MovNomBaseAfecta baseAfectmov LEFT OUTER  JOIN baseAfectmov.baseAfecConcepNom baseAfectConce LEFT OUTER ");
            concatena.Append("JOIN  baseAfectConce.baseNomina baseNom LEFT OUTER JOIN  baseAfectmov.movNomConcep mov1  LEFT OUTER JOIN mov1.concepNomDefi cnc1 ");
            concatena.Append("RIGHT OUTER JOIN mov1.tipoNomina tipNom1 ");
            concatena.Append("RIGHT OUTER JOIN mov1.tipoCorrida tipcorr1 RIGHT OUTER JOIN mov1.periodosNomina per1 ");
            concatena.Append("where ((per1.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per1.fechaFinal BETWEEN :fechaInicial AND  :fechaFinal)) ");
            concatena.Append("AND (cnc1.naturaleza = 1 or  cnc1.naturaleza =2) ");
            concatena.Append("AND cnc1.clave=cnc.clave ");
            concatena.Append(filtrosPersonalizados2.ToString()).Append(" ");
            concatena.Append(")as exentoISR, ");
            concatena.Append("(SELECT CASE WHEN (COUNT(baseAfectmov)=0) THEN 0.0 ELSE (SUM(CASE WHEN (baseNom.clave IS NULL) THEN 0.0 ELSE CASE WHEN ");
            concatena.Append("(baseNom.clave ='02')  THEN baseAfectmov.resultado ELSE 0.0  END END))END  as gravadoIMSS ");
            concatena.Append(" From MovNomBaseAfecta baseAfectmov LEFT OUTER  JOIN baseAfectmov.baseAfecConcepNom baseAfectConce LEFT OUTER ");
            concatena.Append("JOIN  baseAfectConce.baseNomina baseNom LEFT OUTER JOIN  baseAfectmov.movNomConcep mov1  LEFT OUTER JOIN mov1.concepNomDefi cnc1 ");
            concatena.Append(" RIGHT OUTER JOIN mov1.tipoNomina tipNom1 RIGHT OUTER JOIN mov1.tipoCorrida tipcorr1 RIGHT OUTER JOIN mov1.periodosNomina per1 ");
            concatena.Append("where ((per1.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per1.fechaFinal BETWEEN :fechaInicial AND  :fechaFinal)) ");
            concatena.Append("AND (cnc1.naturaleza = 1 or  cnc1.naturaleza =2) AND cnc1.clave=cnc.clave ");
            concatena.Append(filtrosPersonalizados2.ToString()).Append(" ");
            concatena.Append(")as gravadoIMSS, ");
            concatena.Append("(SELECT CASE WHEN (COUNT(baseAfectmov)=0) THEN 0.0 ELSE (SUM(CASE WHEN (baseNom.clave IS NULL) THEN 0.0 ELSE CASE WHEN ");
            concatena.Append("(baseNom.clave ='02')  THEN baseAfectmov.resultadoExento ELSE 0.0  END END))END  as exentoIMSS ");
            concatena.Append(" From MovNomBaseAfecta baseAfectmov LEFT OUTER  JOIN baseAfectmov.baseAfecConcepNom baseAfectConce LEFT OUTER ");
            concatena.Append("JOIN  baseAfectConce.baseNomina baseNom LEFT OUTER JOIN  baseAfectmov.movNomConcep mov1  LEFT OUTER JOIN mov1.concepNomDefi cnc1 ");
            concatena.Append(" RIGHT OUTER JOIN mov1.tipoNomina tipNom1 RIGHT OUTER JOIN mov1.tipoCorrida tipcorr1 RIGHT OUTER JOIN mov1.periodosNomina per1 ");
            concatena.Append("where ((per1.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per1.fechaFinal BETWEEN :fechaInicial AND  :fechaFinal)) ");
            concatena.Append("AND (cnc1.naturaleza = 1 or  cnc1.naturaleza =2) AND cnc1.clave=cnc.clave ");
            concatena.Append(filtrosPersonalizados2.ToString()).Append(" ");
            concatena.Append(") as exentoIMSS ");

            concatena.Append("From PlazasPorEmpleadosMov pm LEFT JOIN pm.plazasPorEmpleado pl LEFT OUTER JOIN pl.empleados emp, MovNomConcep mov ");
            concatena.Append("RIGHT OUTER JOIN mov.concepNomDefi cnc LEFT OUTER JOIN pl.razonesSociales rs LEFT OUTER  JOIN  mov.concepNomDefi cnc ");
            concatena.Append("RIGHT OUTER JOIN mov.tipoNomina tipNom RIGHT OUTER JOIN mov.tipoCorrida tipcorr RIGHT OUTER JOIN mov.periodosNomina per RIGHT OUTER JOIN ");
            concatena.Append("pl.registroPatronal reg LEFT OUTER JOIN mov.empleados empMov LEFT OUTER JOIN pm.departamentos dep LEFT OUTER JOIN pm.centroDeCosto cent ");
            concatena.Append("where empMov.id = emp.id AND (cnc.naturaleza = 1 or  cnc.naturaleza =2) AND rs.clave =  :claveRazonsocial ");
            concatena.Append("AND((per.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per.fechaFinal BETWEEN :fechaInicial AND  :fechaFinal)) ");
            concatena.Append(filtrosPersonalizados.ToString()).Append(" ");

            concatena.Append(" AND pm.id IN (");
            concatena.Append("SELECT x0 FROM PlazasPorEmpleadosMov x0 LEFT OUTER JOIN x0.plazasPorEmpleado x1 LEFT OUTER JOIN ");
            concatena.Append("x1.razonesSociales x2 LEFT OUTER JOIN x1.empleados x3 WHERE x2.clave = :claveRazonsocial ");
            concatena.Append("AND x0.fechaInicial IN (SELECT max(x0X.fechaInicial) FROM PlazasPorEmpleadosMov x0X LEFT OUTER JOIN x0X.plazasPorEmpleado x1X ");
            concatena.Append("LEFT OUTER JOIN x1X.empleados x3X WHERE (x0X.fechaInicial <= :fechaFinal ) AND x3.id = x3X.id) GROUP BY x0.id) ");

            concatena.Append("GROUP BY  cnc.id,cnc.naturaleza, cnc.clave,cnc.descripcion ");
            concatena.Append("ORDER BY cnc.naturaleza,cnc.clave ");

            concatena.Append(filtrosOrden.ToString()).Append(" ");


            return concatena.ToString();
        }
        private string construirQueryMovBaseGravablesOtros(string filtrosPersonalizados, string filtrosOrden) {
            concatena.Remove(0, concatena.Length);
            concatena.Append("SELECT ");
            concatena.Append("(CASE WHEN (cnc IS NULL) THEN '' ELSE CASE WHEN (cnc.clave IS NULL) THEN '' ELSE cnc.clave END END) as claveConcepNomDefi, ");
            concatena.Append("(CASE WHEN  (cnc IS NULL) THEN '' ELSE CASE WHEN (cnc.clave IS NULL) THEN '' ELSE cnc.descripcion ");
            concatena.Append("END END)as descripcionConcepNomDefi,  ");
            concatena.Append("CASE WHEN (SUM(mov.resultado) IS NULL) THEN 0.0 ELSE SUM(mov.resultado) END  as importe ");


            concatena.Append("From PlazasPorEmpleadosMov pm LEFT JOIN pm.plazasPorEmpleado pl LEFT OUTER JOIN pl.empleados emp, MovNomConcep mov ");
            concatena.Append("RIGHT OUTER JOIN mov.concepNomDefi cnc LEFT OUTER JOIN pl.razonesSociales rs LEFT OUTER  JOIN  mov.concepNomDefi cnc ");
            concatena.Append("RIGHT OUTER JOIN mov.tipoNomina tipNom RIGHT OUTER JOIN mov.tipoCorrida tipcorr RIGHT OUTER JOIN mov.periodosNomina per RIGHT OUTER JOIN ");
            concatena.Append("pl.registroPatronal reg LEFT OUTER JOIN mov.empleados empMov LEFT OUTER JOIN pm.departamentos dep LEFT OUTER JOIN pm.centroDeCosto cent ");

            concatena.Append("where empMov.id  = emp.id ");
            concatena.Append("AND(cnc.formulaConcepto = 'CALCULOISR' or cnc.formulaConcepto = 'ISRSUBSIDIO' or cnc.formulaConcepto = 'ISRACARGO' or cnc.formulaConcepto = 'SUBSEEMPLEOCALCULADO' ");
            concatena.Append("or cnc.formulaConcepto = 'AJUSTESUBCAUSADO' or cnc.formulaConcepto = 'AJUSTEISRMES' or cnc.formulaConcepto = 'AJUSTESUBPAGADO') ");
            concatena.Append("AND rs.clave = :claveRazonsocial ");
            concatena.Append("AND ((per.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per.fechaFinal BETWEEN :fechaInicial AND :fechaFinal)) ");
            concatena.Append(filtrosPersonalizados.ToString()).Append(" ");

            concatena.Append(" AND pm.id IN (");
            concatena.Append("SELECT x0 FROM PlazasPorEmpleadosMov x0 LEFT OUTER JOIN x0.plazasPorEmpleado x1 LEFT OUTER JOIN ");
            concatena.Append("x1.razonesSociales x2 LEFT OUTER JOIN x1.empleados x3 WHERE x2.clave = :claveRazonsocial ");
            concatena.Append("AND x0.fechaInicial IN (SELECT max(x0X.fechaInicial) FROM PlazasPorEmpleadosMov x0X LEFT OUTER JOIN x0X.plazasPorEmpleado x1X ");
            concatena.Append("LEFT OUTER JOIN x1X.empleados x3X WHERE (x0X.fechaInicial <= :fechaFinal ) AND x3.id = x3X.id) GROUP BY x0.id) ");

            concatena.Append("GROUP BY cnc.id, cnc.clave, ");
            concatena.Append("cnc.descripcion ");

            concatena.Append("ORDER BY (CASE WHEN (cnc IS NULL) THEN '' ELSE CASE WHEN (cnc.clave IS NULL) THEN '' ELSE cnc.clave END END)");

            return concatena.ToString();

        }
        #endregion

        #region reporte de asimilidos
        public Mensaje getReportesDeAsimilados(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Reporte de movimientos nomina
        public Mensaje getReportesMovimientos(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session)
        {
            try
            {
                setSession(session);
                Dictionary<string, object> reporteFinal = new Dictionary<string, object>();
                String genQueryCount = construirQueryMovNom(values["filtrosPersonalizados"].ToString(), values["filtrosOrden"].ToString());
                query = getSession().CreateQuery(genQueryCount);
                query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                query.SetString("fechaInicial", values["fechaInicial"].ToString());
                query.SetString("fechaFinal", values["fechaFinal"].ToString());
                //var datosemp = query.List();
                var datosemp = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                mensajeResultado.resultado = datosemp;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                // getSession().Transaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReportesMovimientos()1_Error: ").Append(ex));
                //if (getSession().Transaction.IsActive)
                //{
                //    getSession().Transaction.Rollback();
                //}
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        private string construirQueryMovNom(string filtrosPersonalizados, string filtrosOrden)
        {

            concatena.Remove(0, concatena.Length);
            concatena.Append("SELECT ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.clave IS NULL) THEN '' ELSE emp.clave END END  as claveEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.nombre IS NULL) THEN '' ELSE emp.nombre END END  as nombreEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoPaterno IS NULL) THEN '' ELSE emp.apellidoPaterno END END  as apelliPaternoEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoMaterno IS NULL) THEN '' ELSE emp.apellidoMaterno END END  as apelliMaternoEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.RFC IS NULL) THEN '' ELSE emp.RFC END END as empleadoRFC, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.IMSS IS NULL) THEN '' ELSE emp.IMSS END END as empleadoIMSS, ");
            concatena.Append("cnc.descripcion as descripcionConcepNomDefi,cnc.naturaleza as naturalezaConcepNomDefi, ");
            concatena.Append("(CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.clave IS NULL) THEN '' ELSE dep.clave END END) as claveDepartamentos, ");
            concatena.Append("(CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.descripcion IS NULL) THEN '' ELSE dep.descripcion END END) as descripcionDepartamentos, ");
            concatena.Append("(CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.clave IS NULL) THEN '' ELSE cent.clave END END) as claveCentrodeCostos, ");
            concatena.Append("(CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.descripcion IS NULL) THEN '' ELSE cent.descripcion END END) as descripcionCentrodeCostos, ");
            concatena.Append("CASE WHEN (cnc.naturaleza IS NULL) THEN 0.0 ELSE sum(mov.resultado)  END as importeConcepto ");

            concatena.Append("FROM PlazasPorEmpleadosMov pm INNER JOIN pm.plazasPorEmpleado pl RIGHT OUTER JOIN pl.empleados emp ");
            concatena.Append("INNER JOIN emp.razonesSociales rs  LEFT OUTER JOIN pm.departamentos dep LEFT OUTER JOIN pm.centroDeCosto cent, ");
            concatena.Append("MovNomConcep mov RIGHT OUTER JOIN mov.concepNomDefi cnc RIGHT OUTER JOIN mov.tipoCorrida tipcorr RIGHT OUTER JOIN mov.periodosNomina per ");
            concatena.Append("RIGHT OUTER JOIN mov.empleados empMov RIGHT OUTER JOIN mov.tipoNomina tipNom RIGHT OUTER JOIN pl.registroPatronal reg ");
            concatena.Append("LEFT  OUTER JOIN pm.plazas plaz   LEFT  OUTER JOIN plaz.categoriasPuestos cat LEFT  OUTER JOIN plaz.puestos pue ");
            concatena.Append("LEFT  OUTER JOIN plaz.turnos tur    LEFT  OUTER JOIN pm.tipoContrato tipCon, ");
            concatena.Append("SalariosIntegrados sdi ");

            concatena.Append("where sdi.empleados.id= emp.id ");
            concatena.Append("AND sdi.fecha = (SELECT MAX(s.fecha) FROM SalariosIntegrados s WHERE s.fecha <= GETDATE() ");
            concatena.Append("AND s.empleados.id = sdi.empleados.id AND s.empleados.id = pl.empleados.id) ");
            concatena.Append("AND rs.clave = :claveRazonsocial ");
            concatena.Append("AND empMov.id  = emp.id ");
            concatena.Append("AND ((per.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per.fechaFinal BETWEEN :fechaInicial AND  :fechaFinal)) ");

            concatena.Append(filtrosPersonalizados);
            concatena.Append(" AND pm.id IN (");
            concatena.Append("SELECT x0 FROM PlazasPorEmpleadosMov x0 LEFT OUTER JOIN x0.plazasPorEmpleado x1 LEFT OUTER JOIN ");
            concatena.Append("x1.razonesSociales x2 LEFT OUTER JOIN x1.empleados x3 WHERE x2.clave = :claveRazonsocial ");
            concatena.Append("AND x0.fechaInicial IN (SELECT max(x0X.fechaInicial) FROM PlazasPorEmpleadosMov x0X LEFT OUTER JOIN x0X.plazasPorEmpleado x1X ");
            concatena.Append("LEFT OUTER JOIN x1X.empleados x3X WHERE (x0X.fechaInicial <= :fechaFinal ) AND x3.id = x3X.id))");


            concatena.Append("group by ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.clave IS NULL) THEN '' ELSE emp.clave END END), ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.nombre IS NULL) THEN '' ELSE emp.nombre END END), ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoPaterno IS NULL) THEN '' ELSE emp.apellidoPaterno END END), ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoMaterno IS NULL) THEN '' ELSE emp.apellidoMaterno END END), ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.RFC IS NULL) THEN '' ELSE emp.RFC END END), ");
            concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.IMSS IS NULL) THEN '' ELSE emp.IMSS END ");
            concatena.Append("END),cnc.descripcion,mov.tipoNomina, cnc.naturaleza,(CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.clave ");
            concatena.Append("IS NULL) THEN '' ELSE dep.clave END END),(CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.descripcion IS ");
            concatena.Append("NULL) THEN '' ELSE dep.descripcion END END), ");
            concatena.Append("(CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.clave IS NULL) THEN '' ELSE cent.clave END END), ");
            concatena.Append("(CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.descripcion IS NULL) THEN '' ELSE cent.descripcion END END) ");

            concatena.Append(filtrosOrden.ToString()).Append(" ");

            //concatena.Append(" (CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.clave IS NULL) THEN '' ELSE dep.clave END END) ");
            return concatena.ToString();
        }

        #endregion

        #region Reporte Credito Ahorro
        public Mensaje getReportesCreditosAhorro(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session)
        {
            try
            {
                bool capturaCredito = Convert.ToBoolean(values["capturarCredito"].ToString()); ;
                bool isFechaInicial = false;
                bool isIncluirSinSaldo = Convert.ToBoolean(values["IncluirSinSaldo"].ToString());
                if (values.ContainsKey("fechaInicial"))
                {
                    isFechaInicial = true;
                }
                setSession(session);
                Dictionary<string, object> reporteFinal = new Dictionary<string, object>();
                List<object> datosReporte = new List<object>();
                //String genQueryCount = construirQueryCredAhorro(values["filtrosPersonalizados"].ToString(), values["filtrosOrden"].ToString(),isFechaInicial);
                String genQueryCount = construirQueryDatosEmp(values["filtrosPersonalizados"].ToString(), "", isFechaInicial);
                query = getSession().CreateQuery(genQueryCount);
                query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                query.SetString("claveCredito", values["claveCredito"].ToString());
                query.SetString("tipoConfiguracion", values["tipoConfiguracion"].ToString());
                //if (isFechaInicial)
                //{
                //    query.SetString("fechaInicial", values["fechaInicial"].ToString());
                //}
                //query.SetString("fechaFinal", values["fechaFinal"].ToString());
                var datosemp = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                Dictionary<string, object> reporte = new Dictionary<string, object>();
                Dictionary<string, object> reporteMov = new Dictionary<string, object>();
                // Dictionary<string, object> reporteFinal = new Dictionary<string, object>();

                for (int i = 0; i < datosemp.Count; i++)
                {
                    reporte = new Dictionary<string, object>();
                    Dictionary<string, object> emp = (Dictionary<string, object>)datosemp[i];
                    string creditoPorEmpleado = construirQueryCredPorEmp(values["filtrosPersonalizados"].ToString(), "", isFechaInicial);
                    query = getSession().CreateQuery(creditoPorEmpleado);
                    query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                    query.SetString("claveCredito", values["claveCredito"].ToString());
                    query.SetString("tipoConfiguracion", values["tipoConfiguracion"].ToString());
                    //if (isFechaInicial)
                    //{
                    //    query.SetString("fechaInicial", values["fechaInicial"].ToString());
                    //}
                    //query.SetString("fechaFinal", values["fechaFinal"].ToString());
                    query.SetString("claveEmpleado", emp["claveEmpleado"].ToString());
                    var datosCredPorEmpleado = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                    List<object> datosMov = new List<object>();

                    for (int j = 0; j < datosCredPorEmpleado.Count; j++)
                    {
                        
                        reporteMov = new Dictionary<string, object>();
                        Dictionary<string, object> Mov = (Dictionary<string, object>)datosCredPorEmpleado[j];
                      //  bool capturaCredito = Convert.ToBoolean(Mov["capturaCredito"]);
                        string creditoMov = construirQueryCredMov(values["filtrosPersonalizados"].ToString(), "", isFechaInicial);
                        query = getSession().CreateQuery(creditoMov);
                        query.SetString("numeroCredito", Mov["numeroCredito"].ToString());
                        if (isFechaInicial)
                        {
                            query.SetString("fechaInicial", values["fechaInicial"].ToString());
                        }
                        query.SetString("fechaFinal", values["fechaFinal"].ToString());
                        query.SetString("tipoConfiguracion", values["tipoConfiguracion"].ToString());
                        var datosCredMov = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                        double credito = Convert.ToDouble(Mov["totalCredito"].ToString());
                        double cargos = credito;
                        double Abonos = 0.00;
                        for (int k = 0; k < datosCredMov.Count; k++)
                        {
                            Dictionary<string, object> Mov2 = (Dictionary<string, object>)datosCredMov[k];
                            double importe = Convert.ToDouble(Mov2["importe"].ToString());
                            int tipoMov = Convert.ToInt32(Mov2["tiposMovimiento"].ToString());
                            string tipoConfig = Mov2["tipoConfiguracion"].ToString();
                            if (tipoConfig.Equals("1"))
                            {
                                if (tipoMov == 1 || tipoMov == 5)
                                {
                                    if (capturaCredito)
                                    {
                                        Abonos = Abonos + importe;
                                        credito = credito - importe;
                                        Mov2["saldoActual"] = credito;
                                    }
                                    else {
                                        Abonos = Abonos + importe;
                                        credito = credito + importe;
                                        Mov2["saldoActual"] = credito;
                                    }
                                }
                                else if (tipoMov == 2 || tipoMov == 4)
                                {
                                    Mov2["saldoActual"] = credito;
                                }
                                else if (tipoMov == 3)
                                {
                                    cargos = cargos + importe;
                                    credito = credito + importe;
                                    Mov2["saldoActual"] = credito;
                                }
                            }
                            else if (tipoConfig.Equals("2"))
                            {
                                if (tipoMov == 1 || tipoMov == 5)
                                {
                                    cargos = cargos + importe;
                                    credito = credito + importe;
                                    Mov2["saldoActual"] = credito;
                                }
                                else if (tipoMov == 2 || tipoMov == 4)
                                {
                                    Mov2["saldoActual"] = credito;
                                }
                                else if (tipoMov == 6)
                                {
                                    Abonos = Abonos + importe;
                                    credito = credito - importe;
                                    Mov2["saldoActual"] = credito;
                                }
                            }

                            datosCredMov[k] = Mov2;
                        }
                        //reporteMov["DatosEmpl"] = datosCredPorEmpleado[j];
                        //reporteMov["DatosMov"] = datosCredMov;
                        if (credito > 0)
                        {
                            Dictionary<string, object> datosMov2 = new Dictionary<string, object>();
                            datosMov2["Datos_Mov"] = datosCredMov;
                            datosMov2["subTotalCargos"] = cargos;
                            datosMov2["subTotalAbonos"] = Abonos;
                            if (capturaCredito)
                            {
                                datosMov2["subTotal"] = cargos - Abonos;
                            }
                            else {
                                datosMov2["subTotal"] = cargos + Abonos;
                            }
                            
                            Mov["DatosMov"] = datosMov2;
                            datosMov.Add(Mov);
                        }
                        else
                        {
                            if (isIncluirSinSaldo)
                            {
                                Dictionary<string, object> datosMov2 = new Dictionary<string, object>();
                                datosMov2["Datos_Mov"] = datosCredMov;
                                datosMov2["subTotalCargos"] = cargos;
                                datosMov2["subTotalAbonos"] = Abonos;
                                if (capturaCredito)
                                {
                                    datosMov2["subTotal"] = cargos - Abonos;
                                }
                                else
                                {
                                    datosMov2["subTotal"] = cargos + Abonos;
                                }
                                Mov["DatosMov"] = datosMov2;
                                datosMov.Add(Mov);
                            }
                        }

                    }
                    if (datosMov.Count > 0)
                    {
                        double totalCargos = 0.00;
                        double totalAbonos = 0.00;
                        for (int j = 0; j < datosMov.Count; j++)
                        {
                            Dictionary<string, object> datosub = (Dictionary<string, object>)datosMov[j];
                            Dictionary<string, object> movDatos = (Dictionary<string, object>)datosub["DatosMov"];
                            totalCargos = totalCargos + Convert.ToDouble(movDatos["subTotalCargos"]);
                            totalAbonos = totalAbonos + Convert.ToDouble(movDatos["subTotalAbonos"]);
                        }
                        emp["DatosCredPorEmpleado"] = datosMov;
                        emp["totalCargos"] = totalCargos;
                        emp["totalAbonos"] = totalAbonos;
                        if (capturaCredito)
                        {
                            emp["total"] = totalCargos - totalAbonos;
                        }
                        else {
                            emp["total"] = totalCargos + totalAbonos;
                        }
                        reporte["DatosEmpleado"] = emp;
                    }
                    //reporte["DatosCredPorEmpleado"] = datosMov;
                    // reporte["DatosCredMov"] = datosMov;



                    datosReporte.Add(reporte);
                }
                double totalGenCargos = 0.00;
                double totalGenAbonos = 0.00;
                for (int i = 0; i < datosReporte.Count; i++)
                {
                    Dictionary<string, object> datosub = (Dictionary<string, object>)datosReporte[i];
                    Dictionary<string, object> movDatos = (Dictionary<string, object>)datosub["DatosEmpleado"];
                    totalGenAbonos = totalGenAbonos + Convert.ToDouble(movDatos["totalAbonos"]);
                    totalGenCargos = totalGenCargos + Convert.ToDouble(movDatos["totalCargos"]);
                }

                reporteFinal["Empleado"] = datosReporte;
                reporteFinal["totalGenCargos"] = totalGenCargos;
                reporteFinal["totalGenAbonos"] = totalGenAbonos;
                if (capturaCredito)
                {
                    reporteFinal["totalGen"] = totalGenCargos - totalGenAbonos;
                }
                else {
                    reporteFinal["totalGen"] = totalGenCargos + totalGenAbonos;
                }
                //  var datosemp = query.List();
                mensajeResultado.resultado = reporteFinal;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                // getSession().Transaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReportesMovimientos()1_Error: ").Append(ex));
                //if (getSession().Transaction.IsActive)
                //{
                //    getSession().Transaction.Rollback();
                //}
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }


        private string construirQueryDatosEmp(string filtrosPersonalizados, string filtrosOrden, bool isFechaInicial)
        {
            concatena.Remove(0, concatena.Length);
            concatena.Append("Select  DISTINCT  ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.clave IS NULL) THEN '' ELSE emp.clave END END  as claveEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.nombre IS NULL) THEN '' ELSE emp.nombre END END  as nombreEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoPaterno IS NULL) THEN '' ELSE emp.apellidoPaterno END END  as apelliPaternoEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoMaterno IS NULL) THEN '' ELSE emp.apellidoMaterno END END  as apelliMaternoEmpleado, ");
            //concatena.Append("CASE WHEN (credE IS NULL) THEN '' ELSE CASE WHEN (credE.numeroCredito IS NULL) THEN '' ELSE credE.numeroCredito END END  as numeroCredito, ");
            //concatena.Append("CASE WHEN (credE IS NULL) THEN '' ELSE CASE WHEN (credE.montoDescuento IS NULL) THEN '' ELSE credE.montoDescuento END END  as montoDescuento, ");
            //concatena.Append("CASE WHEN (credE IS NULL) THEN '' ELSE CASE WHEN (credE.totalCredito IS NULL) THEN '' ELSE credE.totalCredito END END  as totalCredito, ");
            //concatena.Append("CASE WHEN (credE IS NULL) THEN '' ELSE CASE WHEN (credE.fechaCredito IS NULL) THEN '' ELSE credE.fechaCredito END END  as fechaCredito ");
            concatena.Append("CASE WHEN (credAho IS NULL) THEN '' ELSE CASE WHEN (credAho.tipoConfiguracion IS NULL) THEN '' ELSE credAho.tipoConfiguracion END END  as tipoConfiguracion ");


            //concatena.Append("FROM CreditoMovimientos credM INNER JOIN credM.creditoPorEmpleado credE RIGHT OUTER JOIN credE.razonesSociales rs ");
            concatena.Append("FROM CreditoPorEmpleado credE INNER JOIN  credE.creditoAhorro credAho LEFT OUTER JOIN credE.razonesSociales rs ");
            concatena.Append("LEFT OUTER JOIN credE.empleados emp ");

            concatena.Append("where rs.clave = :claveRazonsocial AND credAho.clave=:claveCredito AND credAho.tipoConfiguracion=:tipoConfiguracion ");
            //if (isFechaInicial)
            //{
            //    concatena.Append("AND ((credE.fechaCredito BETWEEN :fechaInicial AND :fechaFinal)) ");
            //}
            //else {
            //    concatena.Append("AND credE.fechaCredito<=:fechaFinal ");
            //}

            concatena.Append(filtrosPersonalizados.ToString()).Append(" ");

            //concatena.Append(filtrosOrden.ToString()).Append(" ");
            return concatena.ToString();
        }

        private string construirQueryCredPorEmp(string filtrosPersonalizados, string filtrosOrden, bool isFechaInicial)
        {
            concatena.Remove(0, concatena.Length);
            concatena.Append("Select  ");
            //concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.clave IS NULL) THEN '' ELSE emp.clave END END  as claveEmpleado, ");
            //concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.nombre IS NULL) THEN '' ELSE emp.nombre END END  as nombreEmpleado, ");
            //concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoPaterno IS NULL) THEN '' ELSE emp.apellidoPaterno END END  as apelliPaternoEmpleado, ");
            //concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoMaterno IS NULL) THEN '' ELSE emp.apellidoMaterno END END  as apelliMaternoEmpleado ");
            concatena.Append("CASE WHEN (credE IS NULL) THEN '' ELSE CASE WHEN (credE.numeroCredito IS NULL) THEN '' ELSE credE.numeroCredito END END  as numeroCredito, ");
            concatena.Append("CASE WHEN (credE IS NULL) THEN '' ELSE CASE WHEN (credE.montoDescuento IS NULL) THEN '' ELSE credE.montoDescuento END END  as montoDescuento, ");
            concatena.Append("CASE WHEN (credE IS NULL) THEN '' ELSE CASE WHEN (credE.totalCredito IS NULL) THEN '' ELSE credE.totalCredito END END  as totalCredito, ");
            concatena.Append("CASE WHEN (credE IS NULL) THEN '' ELSE CASE WHEN (credE.fechaCredito IS NULL) THEN '' ELSE credE.fechaCredito END END  as fechaCredito, ");
            concatena.Append("CASE WHEN (credAho IS NULL) THEN '' ELSE CASE WHEN (credAho.tipoConfiguracion IS NULL) THEN '' ELSE credAho.tipoConfiguracion END END  as tipoConfiguracion, ");
            concatena.Append("CASE WHEN (credAho IS NULL) THEN '' ELSE CASE WHEN (credAho.capturarCreditoTotal IS NULL) THEN '' ELSE credAho.capturarCreditoTotal END END as capturaCredito ");
            //concatena.Append("FROM CreditoMovimientos credM INNER JOIN credM.creditoPorEmpleado credE RIGHT OUTER JOIN credE.razonesSociales rs ");
            concatena.Append("FROM CreditoPorEmpleado credE INNER JOIN  credE.creditoAhorro credAho LEFT OUTER JOIN credE.razonesSociales rs ");
            concatena.Append("LEFT OUTER JOIN credE.empleados emp ");

            concatena.Append("where rs.clave = :claveRazonsocial AND credAho.clave=:claveCredito AND emp.clave=:claveEmpleado AND credAho.tipoConfiguracion=:tipoConfiguracion ");
            //if (isFechaInicial)
            //{
            //    concatena.Append("AND ((credE.fechaCredito BETWEEN :fechaInicial AND :fechaFinal)) ");
            //}
            //else
            //{
            //    concatena.Append("AND credE.fechaCredito<=:fechaFinal ");
            //}

            // concatena.Append(filtrosPersonalizados.ToString()).Append(" ");

            //concatena.Append(filtrosOrden.ToString()).Append(" ");
            return concatena.ToString();
        }

        private string construirQueryCredMov(string filtrosPersonalizados, string filtrosOrden, bool isFechaInicial)
        {
            concatena.Remove(0, concatena.Length);
            concatena.Append("Select  ");

            concatena.Append("CASE WHEN (credM IS NULL) THEN '' ELSE CASE WHEN (credM.fecha IS NULL) THEN '' ELSE credM.fecha END END  as fecha, ");
            concatena.Append("CASE WHEN (credM IS NULL) THEN '' ELSE CASE WHEN (credM.tiposMovimiento IS NULL) THEN '' ELSE credM.tiposMovimiento END END  as tiposMovimiento, ");
            concatena.Append("CASE WHEN (credM IS NULL) THEN '' ELSE CASE WHEN (credM.importe IS NULL) THEN '' ELSE credM.importe END END  as importe, ");
            concatena.Append("(Select CONCAT(SUBSTRING(tn.descripcion,1,1),'-',per.clave,'-',SUBSTRING(cast(mov.ejercicio as string),3,2)) ");
            concatena.Append("FROM MovNomConcep  mov INNER JOIN mov.creditoMovimientos credMov  LEFT OUTER JOIN mov.tipoNomina tn LEFT OUTER JOIN mov.periodosNomina per ");
            concatena.Append("where credMov.id = credM.id ) as Referencia, ");
            concatena.Append("CASE WHEN (credAho IS NULL) THEN '' ELSE CASE WHEN (credAho.tipoConfiguracion IS NULL) THEN '' ELSE credAho.tipoConfiguracion END END  as tipoConfiguracion ");
            //concatena.Append("(select CASE WHEN (cr.tipoConfiguracion = '1') then (totalCredito + (select CASE WHEN (COUNT(*) = 0 ) THEN 0.0 ELSE  ");
            //concatena.Append("(SUM(CASE WHEN (c.importe is null) then 0.0 ELSE c.importe END)) END FROM CreditoMovimientos c where c.creditoPorEmpleado.id = credE.id AND c.id = credM.id ");
            //concatena.Append("AND c.tiposMovimiento = 3)) - (select CASE WHEN (COUNT(*) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (c.importe is null) then 0.0 ELSE c.importe END)) ");
            //concatena.Append("END FROM CreditoMovimientos c where c.creditoPorEmpleado.id = credE.id AND c.id = credM.id AND (c.tiposMovimiento = 1 or c.tiposMovimiento = 5)) else ");
            //concatena.Append("((select CASE WHEN (COUNT(*) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (c.importe is null) then 0.0 ELSE c.importe END)) END ");
            //concatena.Append("FROM CreditoMovimientos c where c.creditoPorEmpleado.id = credE.id AND c.id = credM.id AND (c.tiposMovimiento = 1 or c.tiposMovimiento = 5))) - ");
            //concatena.Append("(select CASE WHEN (COUNT(*) = 0 ) THEN 0.0 ELSE (SUM(CASE WHEN (c.importe is null) then 0.0 ELSE c.importe END)) END ");
            //concatena.Append("FROM CreditoMovimientos c where c.creditoPorEmpleado.id = credE.id AND c.id = credM.id AND c.tiposMovimiento = 3) end ");
            //concatena.Append("FROM CreditoPorEmpleado  c, CreditoAhorro cr where c.creditoAhorro.id=cr.id and c.id = credE.id ) as totalCreditoMov ");
            //concatena.Append("CASE WHEN (credE IS NULL) THEN '' ELSE CASE WHEN (credE.fechaCredito IS NULL) THEN '' ELSE credE.fechaCredito END END  as fechaCredito ");


            //concatena.Append("FROM CreditoMovimientos credM INNER JOIN credM.creditoPorEmpleado credE RIGHT OUTER JOIN credE.razonesSociales rs ");
            concatena.Append("FROM CreditoMovimientos credM INNER JOIN  credM.creditoPorEmpleado credE RIGHT OUTER JOIN credE.creditoAhorro credAho ");


            concatena.Append("where credE.numeroCredito =:numeroCredito AND credAho.tipoConfiguracion=:tipoConfiguracion ");
            if (isFechaInicial)
            {
                concatena.Append("AND ((credM.fecha BETWEEN :fechaInicial AND :fechaFinal)) ");
            }
            else
            {
                concatena.Append("AND credM.fecha<=:fechaFinal ");
            }
            concatena.Append("ORDER BY CASE WHEN (credM IS NULL) THEN '' ELSE CASE WHEN (credM.fecha IS NULL) THEN '' ELSE credM.fecha END END ");
            // concatena.Append(filtrosPersonalizados.ToString()).Append(" ");

            //concatena.Append(filtrosOrden.ToString()).Append(" ");
            return concatena.ToString();
        }

        #endregion

        #region reporte de integrados
        public Mensaje getReporteIntegrados(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session)
        {
            try
            {
                setSession(session);
                Dictionary<string, object> reporteFinal = new Dictionary<string, object>();
                DateTime fechaFinal = Convert.ToDateTime(values["fechaFinal"].ToString(), CultureInfo.InvariantCulture); //DateTime.Parse(values["fechaFinal"].ToString());
                String genQueryCount = construirQueryintegrados(values["filtrosPersonalizados"].ToString(), values["filtrosOrden"].ToString());
                query = getSession().CreateQuery(genQueryCount);
                query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                query.SetString("fechaInicial", values["fechaInicial"].ToString());
                query.SetString("fechaFinal", values["fechaFinal"].ToString());
                query.SetString("claveBaseNomina", values["claveBaseNomina"].ToString());
                query.SetInt32("tipoAfectaVariable",Convert.ToInt32(values["tipoAfectaVariable"].ToString()));
                query.SetParameterList("claveExcepcion", (List<string>)values["claveExcepcion"]);
                string tipoReporte = values["tipoReporte"].ToString();
               // var datosTotalConcep = query.List<object>();
                var datosintegrados = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                if (tipoReporte.Equals("Resumen"))
                {
                    for (int i = 0; i < datosintegrados.Count; i++)
                    {
                        Dictionary<string, object> integrado = (Dictionary<string, object>)datosintegrados[i];
                        DateTime fechaIngresoEmpleado = Convert.ToDateTime(integrado["fechaIngresoEmpleado"].ToString());
                        DateTime fechaMovEmpleado = Convert.ToDateTime(integrado["fechaMovEmpleado"].ToString());
                        int dias = Convert.ToInt32(integrado["dias"].ToString());
                        int ant = (fechaFinal.Date.Month - fechaIngresoEmpleado.Date.Month < 0 || (fechaFinal.Date.Month - fechaIngresoEmpleado.Date.Month == 0 &&
                            (fechaFinal - fechaIngresoEmpleado).TotalDays < 0)) ? fechaFinal.Date.Year - fechaIngresoEmpleado.Year - 1 :
                            fechaFinal.Date.Year - fechaIngresoEmpleado.Year;
                        integrado["Ant"] = ant;


                        if (integrado["importeSalarioDetalle"] == null)
                        {
                            integrado["importeSalarioDetalle"] = 0.0;
                        }
                        double importeSalarioDetalle = Convert.ToDouble(integrado["importeSalarioDetalle"].ToString());
                        int diasTrabajado = (fechaMovEmpleado.Month == 1 ? 61
                            : fechaMovEmpleado.Month == 2 ? 61
                            : fechaMovEmpleado.Month == 3 ?
                             fechaMovEmpleado.Year % 4 == 0 ? 60 : 59
                             : fechaMovEmpleado.Month == 4 ?
                             fechaMovEmpleado.Year % 4 == 0 ? 60 : 59
                             : fechaMovEmpleado.Month == 5 ? 61
                             : fechaMovEmpleado.Month == 6 ? 61
                             : fechaMovEmpleado.Month == 7 ? 61
                             : fechaMovEmpleado.Month == 8 ? 61
                             : fechaMovEmpleado.Month == 9 ? 62
                             : fechaMovEmpleado.Month == 10 ? 62
                             : fechaMovEmpleado.Month == 11 ? 61 : 61) - dias;
                        integrado["diasTrabajados"] = diasTrabajado;

                        double variableDiario = importeSalarioDetalle / diasTrabajado;
                        integrado["variableDiario"] = variableDiario;

                        datosintegrados[i] = integrado;
                    }
                }
                else
                {
                    for (int i = 0; i < datosintegrados.Count; i++)
                    {
                        Dictionary<string, object> integrado = (Dictionary<string, object>)datosintegrados[i];
                        DateTime fechaIngresoEmpleado = Convert.ToDateTime(integrado["fechaIngresoEmpleado"].ToString());
                        
                        int ant = (fechaFinal.Date.Month - fechaIngresoEmpleado.Date.Month < 0 || (fechaFinal.Date.Month - fechaIngresoEmpleado.Date.Month == 0 &&
                            (fechaFinal - fechaIngresoEmpleado).TotalDays < 0)) ? fechaFinal.Date.Year - fechaIngresoEmpleado.Year - 1 :
                            fechaFinal.Date.Year - fechaIngresoEmpleado.Year;
                        integrado["Ant"] = ant;


                        if (integrado["importeSalarioDetalle"] == null)
                        {
                            integrado["importeSalarioDetalle"] = 0.0;
                        }
                        double importeSalarioDetalle = Convert.ToDouble(integrado["importeSalarioDetalle"].ToString());
                        //string queriFijo = construirQueryConcepFijos();
                        //query = getSession().CreateQuery(queriFijo);
                        //query.SetDecimal("claveSalarioDiarioIntegrado", Convert.ToDecimal(integrado["claveSalarioDiarioIntegrado"].ToString()));
                        //query.SetString("fechaInicial", values["fechaInicial"].ToString());
                        //query.SetString("fechaFinal", values["fechaFinal"].ToString());
                        //query.SetString("claveBaseNomina", values["claveBaseNomina"].ToString());
                        //query.SetInt32("tipoAfecta", Convert.ToInt32(values["tipoAfectaFijo"].ToString()));
                        //var datosintegradosFijos = query.SetResultTransformer(new DictionaryResultTransformer()).List();

                        //string queriVariable = construirQueryConcepVariables();
                        //query = getSession().CreateQuery(queriVariable);
                        //query.SetDecimal("claveSalarioDiarioIntegrado", Convert.ToDecimal(integrado["claveSalarioDiarioIntegrado"].ToString()));
                        //query.SetString("fechaInicial", values["fechaInicial"].ToString());
                        //query.SetString("fechaFinal", values["fechaFinal"].ToString());
                        //query.SetString("claveBaseNomina", values["claveBaseNomina"].ToString());
                        //query.SetInt32("tipoAfecta", Convert.ToInt32(values["tipoAfectaVariable"].ToString()));
                        //var datosintegradosVariable = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                        //integrado["concepFijos"] = datosintegradosFijos;
                        //integrado["concepVariable"] = datosintegradosVariable;

                        string queriVariable = construirQueryConcepALl();
                        query = getSession().CreateQuery(queriVariable);
                        query.SetDecimal("claveSalarioDiarioIntegrado", Convert.ToDecimal(integrado["claveSalarioDiarioIntegrado"].ToString()));
                        query.SetString("fechaInicial", values["fechaInicial"].ToString());
                        query.SetString("fechaFinal", values["fechaFinal"].ToString());
                        query.SetString("claveBaseNomina", values["claveBaseNomina"].ToString());
                        //query.SetInt32("tipoAfecta", Convert.ToInt32(values["tipoAfectaVariable"].ToString()));
                        var datosintegradosVariable = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                        integrado["conceptos"] = datosintegradosVariable;
                        datosintegrados[i] = integrado;
                        
                    }
                }
                reporteFinal["DatosIntegrados"] = datosintegrados;
                //  var datosemp = query.List();
                mensajeResultado.resultado = reporteFinal;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                // getSession().Transaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReportesMovimientos()1_Error: ").Append(ex));
                //if (getSession().Transaction.IsActive)
                //{
                //    getSession().Transaction.Rollback();
                //}
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        private string construirQueryintegrados(string filtrosPersonalizados, string filtrosOrden) {
            concatena.Remove(0, concatena.Length);

            concatena.Append("SELECT DISTINCT sdi.id as claveSalarioDiarioIntegrado,emp.id as clveEmpleados, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.clave IS NULL) THEN '' ELSE emp.clave END END as claveEmpleado, ");
            concatena.Append("CONCAT(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoPaterno IS NULL) THEN '' ELSE emp.apellidoPaterno END ");
            concatena.Append("END, ' ',CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoMaterno IS NULL) THEN '' ELSE emp.apellidoMaterno END ");
            concatena.Append("END, ' ',CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.nombre IS NULL) THEN '' ELSE emp.nombre END END) as nombreEmpleado, ");
            concatena.Append("(SELECT DISTINCT ");
            concatena.Append("CASE WHEN (ing1.fechaBaja  < :fechaFinal) THEN 'B' ELSE CASE WHEN (COUNT(sdi1) = 1) THEN 'A' ELSE 'M' END END ");
            concatena.Append("FROM SalariosIntegrados sdi1 INNER JOIN sdi1.empleados emp1 LEFT OUTER JOIN emp1.razonesSociales rs3, PlazasPorEmpleadosMov pm1 ");
            concatena.Append("INNER JOIN pm1.plazasPorEmpleado pl1 INNER JOIN pl1.empleados e1 LEFT OUTER JOIN pl1.razonesSociales rs1, ");
            concatena.Append("IngresosBajas ing1 INNER JOIN ing1.empleados ee1 WHERE ");
            concatena.Append("emp1.id = e1.id AND ee1.id = emp1.id AND rs1.clave = :claveRazonsocial AND rs3.clave = :claveRazonsocial ");
            concatena.Append("AND emp1.id = emp.id ");
            concatena.Append("AND (sdi1.fecha BETWEEN :fechaInicial AND :fechaFinal) ");
            concatena.Append("GROUP BY emp1.clave ,ing1.fechaBaja) as TM, CASE WHEN (sdi IS NULL) THEN cast('1900-01-01' as date) ELSE CASE WHEN (sdi.fecha IS NULL) THEN ");
            concatena.Append("cast('1900-01-01' as date) ELSE sdi.fecha END END as fechaMovEmpleado, ");
            concatena.Append("CASE WHEN (ing IS NULL) THEN cast('1900-01-01' as date) ELSE CASE WHEN (ing.fechaIngreso IS NULL) THEN cast('1900-01-01' as date) ");
            concatena.Append("ELSE ing.fechaIngreso END END as fechaIngresoEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.IMSS IS NULL) THEN '' ELSE emp.IMSS END END as IMSSEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.CURP IS NULL) THEN '' ELSE emp.CURP END END as CURPEmpleado, ");
            concatena.Append("CASE WHEN (pm.salarioPor IS 1) THEN pm.puestos.salarioTabular ELSE pm.importe END as salarioDiarioFijo, ");
            concatena.Append("CASE WHEN (sdi IS NULL) THEN 0.0 ELSE CASE WHEN (sdi.factorIntegracion IS NULL) THEN 0.0 ELSE sdi.factorIntegracion END END as factorIntegracion, ");
            concatena.Append("CASE WHEN (sdi IS NULL) THEN 0.0  ELSE CASE WHEN (sdi.salarioDiarioVariable IS NULL) THEN 0.0 ELSE sdi.salarioDiarioVariable END END as salarioDiarioIntegralVariable, ");
            concatena.Append("CASE WHEN (sdi IS NULL) THEN 0.0  ELSE CASE WHEN (sdi.salarioDiarioFijo IS NULL) THEN 0.0 ELSE sdi.salarioDiarioFijo END END as salarioDiarioIntegralFijo, ");
            concatena.Append("CASE WHEN (sdi IS NULL) THEN 0.0  ELSE CASE WHEN (sdi.salarioDiarioIntegrado IS NULL) THEN 0.0 ELSE sdi.salarioDiarioIntegrado END END as salarioDiarioIntegral, ");
            concatena.Append("CASE WHEN (sdi IS NULL)THEN '' ELSE CASE WHEN (sdi.tipoDeSalario IS NULL)THEN '' ELSE sdi.tipoDeSalario END END as tipoSalario, ");
            concatena.Append("CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.clave IS NULL) THEN '' ELSE dep.clave END END as claveDepartamentos, ");
            concatena.Append("CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.descripcion IS NULL) THEN '' ELSE dep.descripcion END END as descripcionDepartamentos, ");
            concatena.Append("CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.clave IS NULL) THEN '' ELSE cent.clave END END as claveCentroCosto, ");
            concatena.Append("CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.descripcion IS NULL) THEN '' ELSE cent.descripcion END END as descripcionCentroCosto, ");
            concatena.Append("CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.descripcionPrevia IS NULL) THEN '' ELSE cent.descripcionPrevia END END as descripcionPreviaCentroCosto, ");
            concatena.Append("(Select  SUM(CASE WHEN(sddet IS NULL)THEN 0.0 ELSE CASE WHEN(sddet.importe IS NULL)THEN 0.0 ELSE sddet.importe END END) as importeSalarioDetalle ");
            concatena.Append("FROM SalariosIntegradosDet sddet INNER JOIN sddet.salariosIntegrados sdi1 LEFT OUTER JOIN sddet.concepNomDefi con ");
            concatena.Append("LEFT OUTER JOIN con.baseAfecConcepNom bac  LEFT OUTER JOIN bac.baseNomina bn ");
            concatena.Append("WHERE bn.clave=:claveBaseNomina AND (bac.tipoAfecta=:tipoAfectaVariable) AND sdi1.id=sdi.id ");
            concatena.Append("AND sddet.fechaCambio BETWEEN :fechaInicial AND :fechaFinal )as importeSalarioDetalle, ");
            concatena.Append("(SELECT COUNT(a)as dias FROM Asistencias a INNER JOIN a.excepciones ex INNER JOIN a.empleados em1 ");
            concatena.Append("INNER JOIN a.razonesSociales rs INNER JOIN a.tipoNomina t INNER JOIN a.periodosNomina p Left Outer Join a.centroDeCosto cc ");
            concatena.Append("WHERE emp.id=em1.id  AND rs.clave = :claveRazonsocial AND t.id = tipNom.id ");
            concatena.Append("AND ex.clave IN (:claveExcepcion) AND a.fecha BETWEEN :fechaInicial AND :fechaFinal) as dias ");

            concatena.Append("FROM SalariosIntegrados sdi INNER JOIN sdi.empleados emp  LEFT OUTER JOIN emp.razonesSociales rs2 , PlazasPorEmpleadosMov pm  ");
            concatena.Append("INNER JOIN pm.plazasPorEmpleado pl INNER JOIN pl.empleados e INNER JOIN pm.tipoNomina tipNom ");
            concatena.Append("LEFT OUTER JOIN pm.centroDeCosto cent LEFT OUTER JOIN pm.departamentos dep LEFT OUTER JOIN pl.razonesSociales rs, IngresosBajas ing ");
            concatena.Append("INNER JOIN ing.empleados ee ");

            concatena.Append("WHERE ");
            concatena.Append("emp.id = e.id AND ee.id = emp.id AND rs.clave = :claveRazonsocial AND rs2.clave = :claveRazonsocial ");
            concatena.Append("AND pm.id IN (SELECT x0.id FROM PlazasPorEmpleadosMov x0 LEFT OUTER JOIN x0.plazasPorEmpleado x1 LEFT OUTER JOIN ");
            concatena.Append("x1.razonesSociales x2  LEFT OUTER JOIN x1.empleados x3 WHERE x2.id = rs.id AND x0.fechaInicial IN (SELECT MAX(x0X.fechaInicial) FROM ");
            concatena.Append("PlazasPorEmpleadosMov x0X LEFT OUTER JOIN x0X.plazasPorEmpleado x1X  LEFT OUTER JOIN x1X.empleados x3X WHERE (:fechaFinal >= x0X.fechaInicial) AND x3.clave = x3X.clave)) ");

            concatena.Append(filtrosPersonalizados.ToString()).Append(" ");

            concatena.Append(filtrosOrden.ToString()).Append(" ");

            return concatena.ToString();
        }

        private string construirQueryConcepFijos() {
            concatena.Remove(0, concatena.Length);
            concatena.Append("Select CASE WHEN (sddet IS NULL) THEN '' ELSE CASE WHEN (con IS NULL) THEN '' ELSE con.descripcion END END as descripcionConcepNomDefi, ");
            concatena.Append("CASE WHEN (sddet IS NULL) THEN '' ELSE CASE WHEN (sddet.importe IS NULL) THEN '' ELSE  sddet.importe END END as importeSalarioDetalle, ");
            concatena.Append("CASE WHEN (sddet IS NULL) THEN '' ELSE CASE WHEN (sddet.fechaCambio IS NULL) THEN '' ELSE sddet.fechaCambio END END  as fechaCambioSalarioDetall ");

            concatena.Append("FROM SalariosIntegradosDet sddet INNER JOIN sddet.salariosIntegrados sdi ");
            concatena.Append("INNER JOIN sddet.concepNomDefi con LEFT ");
            concatena.Append("OUTER JOIN con.baseAfecConcepNom bac  LEFT OUTER JOIN bac.baseNomina bn ");

            concatena.Append("WHERE bn.clave = :claveBaseNomina AND bac.tipoAfecta = :tipoAfecta  AND sdi.id = ");
            concatena.Append(":claveSalarioDiarioIntegrado AND sddet.fechaCambio BETWEEN :fechaInicial AND :fechaFinal ");

            concatena.Append("ORDER BY sddet.fechaCambio ");

            return concatena.ToString();
        }

        private string construirQueryConcepVariables()
        {
            concatena.Remove(0, concatena.Length);
            concatena.Append("Select con.descripcion as descripcionConcepNomDefi, ");
            concatena.Append("sddet.importe as importeSalarioDetalle, ");
            concatena.Append("sddet.fechaCambio as fechaCambioSalarioDetall ");

            concatena.Append("FROM SalariosIntegradosDet sddet INNER JOIN sddet.salariosIntegrados sdi ");
            concatena.Append("INNER JOIN sddet.concepNomDefi con LEFT ");
            concatena.Append("OUTER JOIN con.baseAfecConcepNom bac  LEFT OUTER JOIN bac.baseNomina bn ");

            concatena.Append("WHERE bn.clave = :claveBaseNomina AND bac.tipoAfecta = :tipoAfecta  AND sdi.id = ");
            concatena.Append(":claveSalarioDiarioIntegrado AND sddet.fechaCambio BETWEEN :fechaInicial AND :fechaFinal ");

            concatena.Append("ORDER BY sddet.fechaCambio ");

            return concatena.ToString();
        }


        private string construirQueryConcepALl()
        {
            concatena.Remove(0, concatena.Length);
            concatena.Append("Select con.descripcion as descripcionConcepNomDefi, ");
            concatena.Append("sddet.importe as importeSalarioDetalle, ");
            concatena.Append("sddet.fechaCambio as fechaCambioSalarioDetall, ");
            concatena.Append("bac.tipoAfecta as tipoAfecta ");

            concatena.Append("FROM SalariosIntegradosDet sddet INNER JOIN sddet.salariosIntegrados sdi ");
            concatena.Append("INNER JOIN sddet.concepNomDefi con LEFT ");
            concatena.Append("OUTER JOIN con.baseAfecConcepNom bac  LEFT OUTER JOIN bac.baseNomina bn ");

            concatena.Append("WHERE bn.clave = :claveBaseNomina  AND sdi.id = ");
            concatena.Append(":claveSalarioDiarioIntegrado AND sddet.fechaCambio BETWEEN :fechaInicial AND :fechaFinal ");

            concatena.Append("ORDER BY sddet.fechaCambio ");

            return concatena.ToString();
        }





        #endregion

        #region reporte listado CFDI
        public Mensaje getReporteListadoCFDI(Dictionary<string, object> values, Dictionary<string, object> valuesAgrupacion, ISession session)
        {
            try
            {
                setSession(session);
                Dictionary<string, object> reporteFinal = new Dictionary<string, object>();
                String genQueryCount = construirQueryListCFDI(values["filtrosPersonalizados"].ToString(), values["filtrosOrden"].ToString());
                query = getSession().CreateQuery(genQueryCount);
                query.SetString("claveRazonsocial", values["claveRazonsocial"].ToString());
                query.SetString("fechaInicial", values["fechaInicial"].ToString());
                query.SetString("fechaFinal", values["fechaFinal"].ToString());
                //var datosemp = query.List();
                var datosemp = query.SetResultTransformer(new DictionaryResultTransformer()).List();
                mensajeResultado.resultado = datosemp;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                // getSession().Transaction.Commit();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getReportesMovimientos()1_Error: ").Append(ex));
                //if (getSession().Transaction.IsActive)
                //{
                //    getSession().Transaction.Rollback();
                //}
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        private string construirQueryListCFDI(string filtrosPersonalizados, string filtrosOrden)
        {

            concatena.Remove(0, concatena.Length);
            concatena.Append("SELECT ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.clave IS NULL) THEN '' ELSE emp.clave END END  as claveEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.nombre IS NULL) THEN '' ELSE emp.nombre END END  as nombreEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoPaterno IS NULL) THEN '' ELSE emp.apellidoPaterno END END  as apelliPaternoEmpleado, ");
            concatena.Append("CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoMaterno IS NULL) THEN '' ELSE emp.apellidoMaterno END END  as apelliMaternoEmpleado, ");
            concatena.Append("CASE WHEN (cfdiRec IS NULL) THEN '' ELSE CASE WHEN (cfdiRec.statusTimbrado IS NULL) THEN '' ELSE cfdiRec.statusTimbrado END END as status, ");
            concatena.Append("CASE WHEN (cfdiRec IS NULL) THEN '' ELSE CASE WHEN (cfdiRec.fechaHoraTimbrado IS NULL) THEN '' ELSE cfdiRec.fechaHoraTimbrado END END as fechaHoraTimbrado, ");
            concatena.Append("CASE WHEN (cfdiRec IS NULL) THEN '' ELSE CASE WHEN (cfdiRec.fechaEmision IS NULL) THEN '' ELSE cfdiRec.fechaEmision END END as fechaTimbrado, ");
            concatena.Append("CASE WHEN (cfdiRec IS NULL) THEN '' ELSE CASE WHEN (cfdiRec.total IS NULL) THEN '' ELSE cfdiRec.total END END as netoAPagar  ");
        

            concatena.Append("FROM CFDIEmpleado cfdiEmp INNER JOIN cfdiEmp.cfdiRecibo cfdiRec INNER JOIN cfdiEmp.tipoNomina tipNom ");
            concatena.Append("INNER JOIN  cfdiEmp.tipoCorrida tipcorr INNER JOIN cfdiEmp.razonesSociales rs INNER JOIN cfdiEmp.periodosNomina per ");
            concatena.Append("INNER JOIN cfdiEmp.plazasPorEmpleadosMov pm RIGHT OUTER JOIN pm.plazasPorEmpleado pl LEFT OUTER JOIN pl.empleados emp ");
            concatena.Append("LEFT OUTER JOIN pm.departamentos dep LEFT OUTER JOIN pm.centroDeCosto cent ");

         



            concatena.Append("where rs.clave=:claveRazonsocial ");
            concatena.Append("AND ((per.fechaInicial BETWEEN :fechaInicial AND :fechaFinal) OR (per.fechaFinal BETWEEN :fechaInicial AND  :fechaFinal)) ");

            concatena.Append(filtrosPersonalizados);
            //concatena.Append(" AND pm.id IN (");
            //concatena.Append("SELECT x0 FROM PlazasPorEmpleadosMov x0 LEFT OUTER JOIN x0.plazasPorEmpleado x1 LEFT OUTER JOIN ");
            //concatena.Append("x1.razonesSociales x2 LEFT OUTER JOIN x1.empleados x3 WHERE x2.clave = :claveRazonsocial ");
            //concatena.Append("AND x0.fechaInicial IN (SELECT max(x0X.fechaInicial) FROM PlazasPorEmpleadosMov x0X LEFT OUTER JOIN x0X.plazasPorEmpleado x1X ");
            //concatena.Append("LEFT OUTER JOIN x1X.empleados x3X WHERE (x0X.fechaInicial <= :fechaFinal ) AND x3.id = x3X.id))");


            //concatena.Append("group by ");
            //concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.clave IS NULL) THEN '' ELSE emp.clave END END), ");
            //concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.nombre IS NULL) THEN '' ELSE emp.nombre END END), ");
            //concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoPaterno IS NULL) THEN '' ELSE emp.apellidoPaterno END END), ");
            //concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.apellidoMaterno IS NULL) THEN '' ELSE emp.apellidoMaterno END END), ");
            //concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.RFC IS NULL) THEN '' ELSE emp.RFC END END), ");
            //concatena.Append("(CASE WHEN (emp IS NULL) THEN '' ELSE CASE WHEN (emp.IMSS IS NULL) THEN '' ELSE emp.IMSS END ");
            //concatena.Append("END),cnc.descripcion,mov.tipoNomina, cnc.naturaleza,(CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.clave ");
            //concatena.Append("IS NULL) THEN '' ELSE dep.clave END END),(CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.descripcion IS ");
            //concatena.Append("NULL) THEN '' ELSE dep.descripcion END END), ");
            //concatena.Append("(CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.clave IS NULL) THEN '' ELSE cent.clave END END), ");
            //concatena.Append("(CASE WHEN (cent IS NULL) THEN '' ELSE CASE WHEN (cent.descripcion IS NULL) THEN '' ELSE cent.descripcion END END) ");

            concatena.Append(filtrosOrden.ToString()).Append(" ");

            //concatena.Append(" (CASE WHEN (dep IS NULL) THEN '' ELSE CASE WHEN (dep.clave IS NULL) THEN '' ELSE dep.clave END END) ");
            return concatena.ToString();
        }

        #endregion
    }
}
