
using Exitosw.Payroll.Entity.entidad.cfdi;
using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using Exitosw.Payroll.Hibernate.util;
using Exitosw.Payroll.TestCompilador.funciones;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StatusTimbrado = Exitosw.Payroll.Entity.entidad.cfdi.StatusTimbrado;

namespace Exitosw.Payroll.Core.modelo
{
    public class CFDIEmpleadoDAOH : NHibernateRepository<Object>, CFDIEmpleadoDAOIFH
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        private Boolean? manejaPagosPorHora = null;
        private Boolean manejaPagoDiasNaturales = false;
        private Entity.entidad.ManejoHorasPor? manejoHorasPor = null;
        private Entity.entidad.ManejoSalarioDiario? manejoSalarioDiario = null;
        private StringBuilder strQuery = new StringBuilder();
        IQuery query;

        public Mensaje generaDatosParaTimbrado(List<object> valoresDeFiltrado, string claveRazonSocial, ISession sessionSimple, ISession sessionMaster)
        {
            List<DatosParaTimbrar> datosParaTimbrar = new List<DatosParaTimbrar>();
            valoresDeFiltrado = valoresDeFiltrado == null ? new List<Object>() : valoresDeFiltrado;
            try
            {
                /**
           * ******************************Carga datos para
           * filtrado******************************************************
           */
                Periodicidad periodicidad = null;

                int i, j;
                DatosFiltradoEmpleados datosFiltradoEmpleado = new DatosFiltradoEmpleados();
                DatosFiltradoMovNom datosFiltradoMovNom = new DatosFiltradoMovNom();
                DatosFiltradoAsistencias datosFiltradoAsistencia = new DatosFiltradoAsistencias();

                datosFiltradoEmpleado.setClaveRazonSocial(claveRazonSocial);
                datosFiltradoMovNom.setClaveRazonSocial(claveRazonSocial);
                datosFiltradoAsistencia.setClaveRazonSocial(claveRazonSocial);

                bool empleadoInicio = true;
                if (valoresDeFiltrado.Count() > 0)
                {
                    Empleados empIni = null;
                    Empleados empFin = null;
                    for (i = 0; i < valoresDeFiltrado.Count(); i++)
                    {
                        if (valoresDeFiltrado[i].GetType() == typeof(TipoNomina))
                        {
                            periodicidad = ((TipoNomina)valoresDeFiltrado[i]).periodicidad;
                            datosFiltradoEmpleado.setClaveTipoNomina(((TipoNomina)valoresDeFiltrado[i]).clave);
                            datosFiltradoMovNom.setClaveTipoNomina(((TipoNomina)valoresDeFiltrado[i]).clave);
                            datosFiltradoAsistencia.setClaveTipoNomina(((TipoNomina)valoresDeFiltrado[i]).clave);
                        }
                        else if (valoresDeFiltrado[i].GetType() == typeof(TipoCorrida))
                        {
                            datosFiltradoMovNom.setClaveTipoCorrida(((TipoCorrida)valoresDeFiltrado[i]).clave);
                            datosFiltradoEmpleado.setClaveTipoCorrida(((TipoCorrida)valoresDeFiltrado[i]).clave);
                        }
                        else if (valoresDeFiltrado[i].GetType() == typeof(PeriodosNomina))
                        {
                            datosFiltradoEmpleado.setFechaInicio(((PeriodosNomina)valoresDeFiltrado[i]).fechaInicial);
                            datosFiltradoEmpleado.setFechaFin(((PeriodosNomina)valoresDeFiltrado[i]).fechaFinal);

                            datosFiltradoMovNom.setFechaInicio(((PeriodosNomina)valoresDeFiltrado[i]).fechaInicial);
                            datosFiltradoMovNom.setFechaFin(((PeriodosNomina)valoresDeFiltrado[i]).fechaFinal);

                            datosFiltradoAsistencia.setFechaInicio(((PeriodosNomina)valoresDeFiltrado[i]).fechaInicial);
                            datosFiltradoAsistencia.setFechaFin(((PeriodosNomina)valoresDeFiltrado[i]).fechaFinal);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(RegistroPatronal))
                        {
                            datosFiltradoEmpleado.setClaveRegistroPatronal(((RegistroPatronal)valoresDeFiltrado[i]).clave);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(CentroDeCosto))
                        {
                            datosFiltradoEmpleado.setClaveCentroCosto(((CentroDeCosto)valoresDeFiltrado[i]).clave);
                            datosFiltradoMovNom.setClaveCentroCosto(((CentroDeCosto)valoresDeFiltrado[i]).clave);
                            datosFiltradoAsistencia.setClaveCentroCosto(((CentroDeCosto)valoresDeFiltrado[i]).clave);
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(Departamentos))
                        {
                            datosFiltradoEmpleado.setClaveDepartamento(((Departamentos)valoresDeFiltrado[i]).clave);
                        }
                        else if ((valoresDeFiltrado[i].GetType().BaseType == typeof(Empleados) == empleadoInicio))
                        {
                            empleadoInicio = false;
                            empIni = (Empleados)valoresDeFiltrado[i];
                            if (empIni.id > 0)
                            {
                                empIni = null;
                            }
                        }
                        else if (valoresDeFiltrado[i].GetType().BaseType == typeof(Empleados))
                        {
                            empFin = (Empleados)valoresDeFiltrado[i];
                            if (empFin.id > 0)
                            {
                                empFin = null;
                            }
                        }
                    }
                    if (empIni != null && empFin != null)
                    {
                        datosFiltradoEmpleado.setClaveInicioEmpleado(empIni.clave);
                        datosFiltradoEmpleado.setClaveFinEmpleado(empFin.clave);
                    }
                    else if (empIni != null)
                    {
                        datosFiltradoEmpleado.setClaveInicioEmpleado(empIni.clave);
                    }
                    else if (empFin != null)
                    {
                        datosFiltradoEmpleado.setClaveFinEmpleado(empFin.clave);
                    }
                }

                /**
            * **************Busca parametros**********
            */
                inicializaVariableMensaje();
                setSession(sessionMaster);
                getSession().BeginTransaction();
                obtenerFactores(claveRazonSocial, periodicidad);
                getSession().Transaction.Commit();
                /**
            * **************fin busqueda parametros**********
            */
                setSession(sessionSimple);  //crea conexion
                getSession().BeginTransaction();
                List<Object> datosEmpleado = construyeQueryDatosGlobalesEmpleados(datosFiltradoEmpleado);
                if (datosEmpleado == null)
                {
                    return mensajeResultado;
                }
                if (datosEmpleado.Count() > 0)
                {

                    Object[] valores;
                    //  List<String> clavesEmpleados = new ArrayList<String>();
                    String claveEmpleado;
                    for (i = 0; i < datosEmpleado.Count(); i++)
                    {
                        valores = (Object[])datosEmpleado[i];
                        ///claveEmpleado = (String) valores[0];
                        // clavesEmpleados.add(claveEmpleado);
                        valores[1] = calculaSueldoDiario((PlazasPorEmpleadosMov)valores[1]);
                        datosEmpleado[i] = valores;
                    }

                    List<Object> datosMovNomina = (List<Object>)construyeQueryDatosGlobalesMovNom(datosFiltradoMovNom, datosFiltradoEmpleado);
                    if (datosMovNomina == null)
                    {
                        return mensajeResultado;
                    }

                    List<Object> datosAsistencias = (List<Object>)construyeQueryDatosGlobalesAsistencias(datosFiltradoAsistencia, datosFiltradoEmpleado);

                    //****************************eliminar movimientos repetidos y suma bases afecta y resultado de movimiento*********************************************************/
                    if (datosMovNomina.Count() > 1)
                    {
                        i = 0;
                        int k, l;
                        int contEx = 0, total = datosMovNomina.Count();
                        Object[] tmp;

                        MovNomConcep mov1, mov2;
                        IList<MovNomBaseAfecta> basesAfectaMov1;
                        IList<MovNomBaseAfecta> basesAfectaMov2;
                        String claveEmp, claveEmpTemp;
                        while (i < datosMovNomina.Count())
                        {
                            valores = (Object[])datosMovNomina[i];
                            mov1 = nuevaInstanciaMovNomina((MovNomConcep)valores[1]);
                            claveEmp = (String)valores[0];
                            j = i + 1;
                            if (j < datosMovNomina.Count())
                            {
                                while (j < datosMovNomina.Count())
                                {
                                    tmp = (Object[])datosMovNomina[j];
                                    claveEmpTemp = (String)tmp[0];
                                    mov2 = (MovNomConcep)tmp[1];
                                    if (claveEmpTemp.Equals(claveEmp, StringComparison.InvariantCultureIgnoreCase) & mov1.concepNomDefi.clave.Equals(mov2.concepNomDefi.clave, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        mov1.resultado = ((mov1.resultado == null ? 0.0 : mov1.resultado) + (mov2.resultado == null ? 0.0 : mov2.resultado));
                                        mov1.calculado = ((mov1.calculado == null ? 0.0 : mov1.calculado) + (mov2.calculado == null ? 0.0 : mov2.calculado));
                                        basesAfectaMov1 = mov1.movNomBaseAfecta == null ? new List<MovNomBaseAfecta>() : mov1.movNomBaseAfecta;
                                        basesAfectaMov2 = mov2.movNomBaseAfecta == null ? new List<MovNomBaseAfecta>() : mov2.movNomBaseAfecta;
                                        /**
                                         * *suma bases afecta*
                                         */
                                        if (!basesAfectaMov1.Any())
                                        {
                                            if (basesAfectaMov2.Count() > 0)
                                            {
                                                mov1.movNomBaseAfecta = (creaMovimBaseAfectar(basesAfectaMov2, mov1));
                                            }
                                        }
                                        else
                                        {
                                            if (basesAfectaMov2.Count() > 0)
                                            {
                                                for (l = 0; l < basesAfectaMov1.Count(); l++)
                                                {
                                                    for (k = 0; k < basesAfectaMov2.Count(); k++)
                                                    {
                                                        if (basesAfectaMov1[l].baseAfecConcepNom.id == basesAfectaMov2[k].baseAfecConcepNom.id)
                                                        {
                                                            basesAfectaMov1[l].resultado = (basesAfectaMov1[l].resultado == null ? 0.0 : basesAfectaMov1[l].resultado) + (basesAfectaMov2[k].resultado == null ? 0.0 : basesAfectaMov2[k].resultado);
                                                            basesAfectaMov1[l].resultadoExento = (basesAfectaMov1[l].resultadoExento == null ? 0.0 : basesAfectaMov1[l].resultadoExento) + (basesAfectaMov2[k].resultadoExento == null ? 0.0 : basesAfectaMov2[k].resultadoExento);
                                                            break;
                                                        }
                                                    }
                                                }
                                                mov1.movNomBaseAfecta = (basesAfectaMov1);
                                            }
                                        }
                                        datosMovNomina.RemoveAt(j);
                                        valores[1] = mov1;
                                        datosMovNomina[i] = valores;
                                        contEx++;
                                    }
                                    else
                                    {
                                        j++;
                                    }
                                }
                            }
                            i++;
                        }
                        /////////// System.out.println(" cont elim " + contEx + " resultado " + datosMovNomina.size() + " total " + total);
                    }
                    //****************************agrupa informacion por empleado*********************************************************/
                    DatosParaTimbrar datoPorTimbrar;
                    List<Asistencias> listAsistencias;
                    List<MovNomConcep> listMovNom;
                    Object[] valoresComp;
                    DatosPorEmpleado dpe;
                    for (i = 0; i < datosEmpleado.Count(); i++)
                    {
                        datoPorTimbrar = new DatosParaTimbrar();
                        valores = (Object[])datosEmpleado[i];
                        dpe = new DatosPorEmpleado();
                        dpe.plazasPorEmpleadosMov = (convertirPlaza((PlazasPorEmpleadosMov)valores[1]));
                        dpe.salarioDiarioIntegrado = ((Double)valores[2]);
                        dpe.fechaIngreso = ((DateTime)valores[3]);
                        dpe.detalleReciboPeriodo = ((String)valores[4]);
                        dpe.detalleReciboCorrida = ((String)valores[5]);
                        dpe.detalleReciboNomina = ((String)valores[6]);
                        datoPorTimbrar.datosPorEmpleado = (dpe);
                        claveEmpleado = (String)valores[0];
                        if (datosMovNomina.Count() > 0)
                        {
                            j = 0;
                            listMovNom = new List<MovNomConcep>();
                            while (j < datosMovNomina.Count())
                            {
                                valoresComp = (Object[])datosMovNomina[j];
                                if (claveEmpleado.Equals(valoresComp[0].ToString(), StringComparison.InvariantCultureIgnoreCase))
                                {
                                    listMovNom.Add((MovNomConcep)valoresComp[1]);
                                    datosMovNomina.RemoveAt(j);
                                }
                                else
                                {
                                    j++;
                                }
                            }
                            datoPorTimbrar.movimientos = (convertirlistaMov(listMovNom));
                        }

                        if (datosAsistencias.Count() > 0)
                        {
                            j = 0;
                            listAsistencias = new List<Asistencias>();
                            while (j < datosAsistencias.Count())
                            {
                                valoresComp = (Object[])datosAsistencias[j];
                                if (claveEmpleado.Equals(valoresComp[0].ToString(), StringComparison.InvariantCultureIgnoreCase))
                                {
                                    listAsistencias.Add((Asistencias)valoresComp[1]);
                                    datosAsistencias.RemoveAt(j);
                                }
                                else
                                {
                                    j++;
                                }
                            }
                            datoPorTimbrar = agregaIncapacidadesHorasExtras(listAsistencias, datoPorTimbrar);
                            //////////datosParaTimbrar.setDatosIncapacidades(listAsistencias);
                        }
                        if (datoPorTimbrar.movimientos == null ? false : datoPorTimbrar.movimientos.Any())
                        {
                            datosParaTimbrar.Add(datoPorTimbrar);
                        }
                    }
                }
                nullVariablesGlobales();
                mensajeResultado.resultado = (datosParaTimbrar);
                mensajeResultado.noError = (0);
                mensajeResultado.error = ("");
                getSession().Transaction.Commit();
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

        private Entity.entidad.PlazasPorEmpleadosMov convertirPlaza(PlazasPorEmpleadosMov plMov)
        {
            Entity.entidad.PlazasPorEmpleadosMov plazasMov = new Entity.entidad.PlazasPorEmpleadosMov();
            // plazasMov.bancos_ID = plMov.bancos.id;
            // plazasMov.cambioBanco = plMov.cambioBanco;
            plazasMov.cambioCentroDeCostos = plMov.cambioCentroDeCostos;
            // plazasMov.cambioClabe = plMov.cambioClabe;
            // plazasMov.cambioCuentaBancaria = plMov.cambioCuentaBancaria;
            plazasMov.cambioDepartamento = plMov.cambioDepartamento;
           // plazasMov.cambioFormaDePago = plMov.cambioFormaDePago;
            plazasMov.cambioHoras = plMov.cambioHoras;
            plazasMov.cambioPlazasPosOrganigrama = plMov.cambioPlazasPosOrganigrama;
            plazasMov.cambioPuestos = plMov.cambioPuestos;
            plazasMov.cambioRegimenContratacion = plMov.cambioRegimenContratacion;
            // plazasMov.cambioSalarioPor = plMov.cambioSalarioPor;
            plazasMov.cambioTipoContrato = plMov.cambioTipoContrato;
            plazasMov.cambioTipoDeNomina = plMov.cambioTipoDeNomina;
            plazasMov.cambioTipoRelacionLaboral = plMov.cambioTipoRelacionLaboral;
            plazasMov.cambioTurno = plMov.cambioTurno;
            // plazasMov.cambioZonaGeografica = plMov.cambioZonaGeografica;
            plazasMov.centroDeCosto_ID = plMov.centroDeCosto.id;
            //plazasMov.clabe = plMov.clabe;
            //  plazasMov.cuentaBancaria = plMov.cuentaBancaria;
            plazasMov.departamentos_ID = plMov.departamentos.id;
            plazasMov.fechaIMSS = plMov.fechaIMSS;
            plazasMov.fechaInicial = plMov.fechaInicial;
           /* plazasMov.formasDePago_ID = plMov.formasDePago.id;*/
            plazasMov.importe = plMov.importe;
            plazasMov.plazasPorEmpleado_ID = plMov.plazasPorEmpleado.id;
            //  plazasMov.plazas_ID = plMov.plazas.id;
            plazasMov.puestos_ID = plMov.puestos.id;
            plazasMov.regimenContratacion = plMov.regimenContratacion;
            //  plazasMov.salarioPor = plMov.salarioPor;
            plazasMov.sueldoDiario = plMov.sueldoDiario;
            plazasMov.tipoContrato_ID = plMov.tipoContrato.id;
            plazasMov.tipoNomina_ID = plMov.tipoNomina.id;
            plazasMov.tipoRelacionLaboral = plMov.tipoRelacionLaboral;
            plazasMov.turnos_ID = plMov.turnos.id;
            //plazasMov.zonaGeografica = plMov.zonaGeografica;
            return plazasMov;
        }

        private List<Entity.entidad.MovNomConcep> convertirlistaMov(List<MovNomConcep> listaMov)
        {
            List<Entity.entidad.MovNomConcep> listMovNom = new List<Entity.entidad.MovNomConcep>();
            for (int i = 0; i < listaMov.Count(); i++)
            {
                Entity.entidad.MovNomConcep mov = new Entity.entidad.MovNomConcep();
                mov.calculado = listaMov[i].calculado;
                mov.centroDeCosto_ID = listaMov[i].centroDeCosto.id;
                mov.concepNomDefi_ID = listaMov[i].concepNomDefi.id;
                // mov.creditoMovimientos_ID = listaMov[i].creditoMovimientos.id;
                mov.ejercicio = listaMov[i].ejercicio;
                mov.empleado_ID = listaMov[i].empleados.id;
                mov.fechaCierr = listaMov[i].fechaCierr;
                mov.fechaIni = listaMov[i].fechaIni;
                //mov.finiqLiquidCncNom_ID = listaMov[i].finiqLiquidCncNom.id;
                mov.id = listaMov[i].id;
                mov.IsEnBD = listaMov[i].IsEnBD;
                mov.mes = listaMov[i].mes;
                mov.numero = listaMov[i].numero;
                mov.numMovParticion = listaMov[i].numMovParticion;
                mov.ordenId = listaMov[i].ordenId;
                mov.periodosNomina_ID = listaMov[i].periodosNomina.id;
                mov.plazasPorEmpleado_ID = listaMov[i].plazasPorEmpleado.id;
                mov.razonesSociales_ID = listaMov[i].razonesSociales.id;
                mov.resultado = listaMov[i].resultado;
                mov.tipoCorrida_ID = listaMov[i].tipoCorrida.id;
                mov.tipoNomina_ID = listaMov[i].tipoNomina.id;
                mov.tipoPantalla = listaMov[i].tipoPantalla;
                mov.uso = listaMov[i].uso;
                listMovNom.Add(mov);
            }


            return listMovNom;
        }

        private MovNomConcep nuevaInstanciaMovNomina(MovNomConcep movNomConcep)
        {
            MovNomConcep nueva = new MovNomConcep();
            nueva.calculado = (movNomConcep.calculado);
            nueva.centroDeCosto = (movNomConcep.centroDeCosto);
            nueva.concepNomDefi = (movNomConcep.concepNomDefi);
            nueva.ejercicio = (movNomConcep.ejercicio);
            nueva.empleados = (movNomConcep.empleados);
            nueva.fechaCierr = (movNomConcep.fechaCierr);
            nueva.fechaIni = (movNomConcep.fechaIni);
            nueva.finiqLiquidCncNom = (movNomConcep.finiqLiquidCncNom);
            nueva.id = 0;
            nueva.IsEnBD = (movNomConcep.IsEnBD);
            nueva.mes = (movNomConcep.mes);
            nueva.movNomBaseAfecta = (creaMovimBaseAfectar(movNomConcep.movNomBaseAfecta, nueva));
            nueva.movNomConceParam = (creaMovNomConceParam(nueva, movNomConcep.movNomConceParam));
            nueva.numMovParticion = (movNomConcep.numMovParticion);
            nueva.numero = (movNomConcep.numero);
            nueva.ordenId = (movNomConcep.ordenId);
            nueva.periodosNomina = (movNomConcep.periodosNomina);
            nueva.plazasPorEmpleado = (movNomConcep.plazasPorEmpleado);
            nueva.razonesSociales = (movNomConcep.razonesSociales);
            nueva.resultado = (movNomConcep.resultado);
            nueva.tipoCorrida = (movNomConcep.tipoCorrida);
            nueva.tipoNomina = (movNomConcep.tipoNomina);
            nueva.uso = (movNomConcep.uso);
            return nueva;
        }

        private List<MovNomConceParam> creaMovNomConceParam(MovNomConcep mnc, IList<MovNomConceParam> param)
        {
            if (param == null)
            {
                return null;
            }
            List<MovNomConceParam> movparametros = new List<MovNomConceParam>();
            MovNomConceParam m;
            foreach (MovNomConceParam movParam in param)
            {
                m = new MovNomConceParam();
                m.movNomConcep = (mnc);
                m.paraConcepDeNom = (movParam.paraConcepDeNom);
                m.valor = (movParam.valor);
                movparametros.Add(m);
            }
            return movparametros;
        }

        private List<MovNomBaseAfecta> creaMovimBaseAfectar(IList<MovNomBaseAfecta> baseAfecConcepNominas, MovNomConcep mnc)
        {
            if (baseAfecConcepNominas == null)
            {
                return null;
            }
            List<MovNomBaseAfecta> movNominaBaseAfectas = new List<MovNomBaseAfecta>(0);
            MovNomBaseAfecta m;
            foreach (MovNomBaseAfecta afecConcepNom in baseAfecConcepNominas)
            {
                m = new MovNomBaseAfecta();
                m.baseAfecConcepNom = (afecConcepNom.baseAfecConcepNom);
                m.movNomConcep = (mnc);
                m.uso = (afecConcepNom.uso);
                m.resultado = (afecConcepNom.resultado);
                m.resultadoExento = (afecConcepNom.resultadoExento);
                movNominaBaseAfectas.Add(m);
            }
            return movNominaBaseAfectas;
        }

        private List<Object> construyeQueryDatosGlobalesEmpleados(DatosFiltradoEmpleados datosFiltradoEmpleado)
        {
            List<Object> datosEmpleado = new List<object>();
            StringBuilder select = new StringBuilder();
            StringBuilder from = new StringBuilder();
            StringBuilder where = new StringBuilder();
            StringBuilder orden = new StringBuilder();
            List<String> camposFiltrado = new List<String>();
            List<Object> valoresWhere = new List<Object>();
            try
            {
                select.Append("SELECT DISTINCT  CASE WHEN (empleado IS NULL) THEN '' ELSE CASE WHEN (empleado.clave IS NULL) THEN '' ELSE empleado.clave END END, ppm, ");
                select.Append("CASE WHEN (sdi IS NULL) THEN 0.0 ELSE CASE WHEN (sdi.salarioDiarioIntegrado IS NULL) THEN 0.0 ELSE sdi.salarioDiarioIntegrado END END, ");
                select.Append("(SELECT CASE WHEN (ing.fechaIngreso IS NULL) THEN cast('1900-01-01' as date) ELSE ing.fechaIngreso END ");
                select.Append("FROM IngReingresosBajas ing  RIGHT OUTER JOIN ing.empleados emp LEFT OUTER JOIN ing.plazasPorEmpleado ingPP WHERE emp.id = empleado.id AND ingPP.id = pp.id  AND  ing.fechaBaja > :fechaFinal AND ing.fechaIngreso <= :fechaFinal), ");
                select.Append("CASE WHEN (periodo IS NULL) THEN '' ELSE CASE WHEN (periodo.detalleConceptoRecibo IS NULL) THEN '' ELSE periodo.detalleConceptoRecibo END END, ");
                select.Append("CASE WHEN (corrida IS NULL) THEN '' ELSE CASE WHEN (corrida.detalleConceptoRecibo IS NULL) THEN '' ELSE corrida.detalleConceptoRecibo END END, ");
                select.Append("CASE WHEN (nomina IS NULL) THEN '' ELSE CASE WHEN (nomina.detalleConceptoRecibo IS NULL) THEN '' ELSE nomina.detalleConceptoRecibo END END ");
                camposFiltrado.Add("fechaFinal");
                valoresWhere.Add(datosFiltradoEmpleado.getFechaFin().GetValueOrDefault());
                from.Append("FROM PlazasPorEmpleadosMov ppm LEFT OUTER JOIN ppm.plazasPorEmpleado pp LEFT OUTER JOIN pp.empleados empleado, SalariosIntegrados sdi, MovNomConcep mvn ");
                from.Append("LEFT OUTER JOIN mvn.periodosNomina periodo LEFT OUTER JOIN mvn.tipoCorrida corrida LEFT OUTER JOIN ppm.tipoNomina nomina LEFT OUTER JOIN pp.razonesSociales razonSocial ");
                from.Append("RIGHT OUTER JOIN mvn.empleados mvmEmp RIGHT OUTER JOIN mvn.periodosNomina periodo RIGHT OUTER JOIN sdi.empleados sdiEmp ");

                where.Append("WHERE mvmEmp.id = empleado.id AND sdiEmp.id = empleado.id ");
                where.Append("AND empleado.id NOT IN (SELECT CASE WHEN (COUNT(cfdiEmp) = 0) THEN 0 ELSE em.id END FROM CFDIEmpleado cfdiEmp RIGHT OUTER JOIN cfdiEmp.plazasPorEmpleadosMov cfdiPPM LEFT OUTER JOIN cfdiEmp.cfdiRecibo recibo ");
                where.Append("LEFT OUTER JOIN cfdiEmp.periodosNomina cfdiPeriodo LEFT OUTER JOIN cfdiPeriodo.tipoCorrida cfdiCorrida LEFT OUTER JOIN cfdiPPM.plazasPorEmpleado cfdiPP LEFT OUTER JOIN cfdiPP.empleados em ");
                where.Append("WHERE (recibo.statusTimbrado = :statusTimbre) AND ((cfdiPeriodo.fechaInicial BETWEEN :fechaPeriodoInicio AND :fechaPeriodoFinal) OR (cfdiPeriodo.fechaFinal BETWEEN :fechaPeriodoInicio AND :fechaPeriodoFinal)) ");
                camposFiltrado.Add("statusTimbre");
                valoresWhere.Add(StatusTimbrado.TIMBRADO);
                camposFiltrado.Add("fechaPeriodoInicio");
                valoresWhere.Add(datosFiltradoEmpleado.getFechaInicio().GetValueOrDefault());
                camposFiltrado.Add("fechaPeriodoFinal");
                valoresWhere.Add(datosFiltradoEmpleado.getFechaFin().GetValueOrDefault());
                if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
                {
                    where.Append("AND cfdiCorrida.clave =:tipoCorrida ");
                    //camposFiltrado.Add("tipoCorrida");
                    //valoresWhere.Add(datosFiltradoEmpleado.getClaveTipoCorrida());
                }
                where.Append("AND em.razonesSociales.id = razonSocial.id GROUP By  em.id) ");
                where.Append("AND sdi.fecha IN (SELECT MAX(s0.fecha) FROM SalariosIntegrados s0 LEFT OUTER JOIN s0.empleados s0Emp WHERE s0.fecha <= :fechaFinal AND s0Emp.id = empleado.id) ");
                where.Append("AND ppm.id IN (SELECT MAX(pem.id) FROM PlazasPorEmpleadosMov pem INNER JOIN pem.plazasPorEmpleado pe INNER JOIN pe.empleados e INNER JOIN pem.tipoNomina nn ");
                where.Append("WHERE e.id = empleado.id AND :fechaFinal >= pem.fechaInicial AND pe.fechaFinal > :fechaFinal ");



                if (datosFiltradoEmpleado.getClaveTipoNomina().Any())
                {
                    where.Append("AND nn.clave = :tipoNomina) "); ///para busqueda maximo id de plaza empleado mov
                    where.Append("AND nomina.clave = :tipoNomina ");
                    camposFiltrado.Add("tipoNomina");
                    valoresWhere.Add(datosFiltradoEmpleado.getClaveTipoNomina());
                }
                else
                {
                    where.Append(") "); //cierra la busqusqueda max plaza por empleado id 
                }

                if (datosFiltradoEmpleado.getClaveRazonSocial().Any())
                {
                    where.Append("AND razonSocial.clave = :razonSocial ");
                    camposFiltrado.Add("razonSocial");
                    valoresWhere.Add(datosFiltradoEmpleado.getClaveRazonSocial());
                }

                if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
                {
                    where.Append("AND corrida.clave = :tipoCorrida AND periodo.tipoCorrida.clave = :tipoCorrida ");
                    camposFiltrado.Add("tipoCorrida");
                    valoresWhere.Add(datosFiltradoEmpleado.getClaveTipoCorrida());
                }
                if (datosFiltradoEmpleado.getFechaFin() != null & datosFiltradoEmpleado.getFechaInicio() != null)
                {
                    where.Append("AND ((periodo.fechaInicial BETWEEN :fechaPeriodoInicio AND :fechaPeriodoFinal) OR (periodo.fechaFinal BETWEEN :fechaPeriodoInicio AND :fechaPeriodoFinal)) ");
                    camposFiltrado.Add("fechaPeriodoInicio");
                    valoresWhere.Add(datosFiltradoEmpleado.getFechaInicio());
                    camposFiltrado.Add("fechaPeriodoFinal");
                    valoresWhere.Add(datosFiltradoEmpleado.getFechaFin());
                }
                if (datosFiltradoEmpleado.getClaveCentroCosto().Any())
                {
                    from.Append("LEFT OUTER JOIN ppm.centroDeCosto cc ");
                    where.Append("AND cc.clave = :centroCosto ");
                    camposFiltrado.Add("centroCosto");
                    valoresWhere.Add(datosFiltradoEmpleado.getClaveCentroCosto());
                }
                if (datosFiltradoEmpleado.getClaveDepartamento().Any())
                {
                    from.Append("LEFT OUTER JOIN ppm.departamentos depto ");
                    where.Append("AND depto.clave = :departamento ");
                    camposFiltrado.Add("departamento");
                    valoresWhere.Add(datosFiltradoEmpleado.getClaveDepartamento());
                }
                if (datosFiltradoEmpleado.getClaveRegistroPatronal().Any())
                {
                    from.Append("LEFT OUTER JOIN pp.registroPatronal rp ");
                    where.Append("AND rp.clave = :registroPatronal ");
                    camposFiltrado.Add("registroPatronal");
                    valoresWhere.Add(datosFiltradoEmpleado.getClaveRegistroPatronal());
                }
                if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any() && datosFiltradoEmpleado.getClaveFinEmpleado().Any())
                {
                    where.Append("AND (empleado.clave >= :claveEmpIni AND empleado.clave <= :claveEmpFin) ");
                    camposFiltrado.Add("claveEmpIni");
                    valoresWhere.Add(datosFiltradoEmpleado.getClaveInicioEmpleado());
                    camposFiltrado.Add("claveEmpFin");
                    valoresWhere.Add(datosFiltradoEmpleado.getClaveFinEmpleado());
                }
                else if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any())
                {
                    camposFiltrado.Add("claveEmpIni");
                    valoresWhere.Add(datosFiltradoEmpleado.getClaveInicioEmpleado());
                    where.Append("AND empleado.clave >= :claveEmpIni ");
                }
                else if (datosFiltradoEmpleado.getClaveFinEmpleado().Any())
                {
                    camposFiltrado.Add("claveEmpIni");
                    valoresWhere.Add(datosFiltradoEmpleado.getClaveFinEmpleado());
                    where.Append("AND empleado.clave <= :claveEmpIni ");
                }
                orden.Append("ORDER BY CASE WHEN (empleado IS NULL) THEN '' ELSE CASE WHEN (empleado.clave IS NULL) THEN '' ELSE empleado.clave END END");
                select.Append(from).Append(where);//.Append(orden);
                //from.Append(where);
                query = getSession().CreateQuery(select.ToString());
                for (int i = 0; i < valoresWhere.Count(); i++)
                {

                    query.SetParameter(camposFiltrado[i], valoresWhere[i]);

                }
                var datosConcep = query.List<object>();
                datosEmpleado = datosConcep.ToList();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getMovimientosNomina()1_Error: ").Append(ex));
                if (getSession().Transaction.IsActive)
                {
                    getSession().Transaction.Rollback();
                }
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
            }


            return datosEmpleado;
        }

        private List<Object> construyeQueryDatosGlobalesMovNom(DatosFiltradoMovNom datosFiltradoMovNom, DatosFiltradoEmpleados datosFiltradoEmpleado)
        {
            List<Object> datosMovimientos;

            StringBuilder select = new StringBuilder();
            StringBuilder from = new StringBuilder();
            StringBuilder where = new StringBuilder();
            StringBuilder orden = new StringBuilder();

            select.Append("SELECT DISTINCT  CASE WHEN (x1 IS NULL) THEN '' ELSE CASE WHEN (x1.clave IS NULL) THEN '' ELSE x1.clave END END, x0, CASE WHEN (x6 IS NULL) THEN '' ELSE CASE WHEN (x6.clave IS NULL) THEN '' ELSE x6.clave END END ");

            from.Append("FROM MovNomConcep x0 LEFT OUTER JOIN x0.empleados x1 LEFT OUTER JOIN x0.razonesSociales x2 LEFT OUTER JOIN x0.tipoNomina x3 LEFT OUTER JOIN x0.tipoCorrida x4 ");
            from.Append("LEFT OUTER JOIN x0.periodosNomina x5 LEFT OUTER JOIN x0.concepNomDefi x6  RIGHT OUTER JOIN x0.empleados x1  RIGHT OUTER JOIN x0.periodosNomina x5 ");

            from.Append(", PlazasPorEmpleadosMov ppm LEFT OUTER JOIN ppm.plazasPorEmpleado pp LEFT OUTER JOIN pp.empleados empleado, SalariosIntegrados sdi ");
            from.Append("LEFT OUTER JOIN ppm.tipoNomina nomina LEFT OUTER JOIN pp.razonesSociales razonSocial RIGHT OUTER JOIN sdi.empleados sdiEmp ");
            List<String> camposFiltrado = new List<String>();
            List<Object> valoresWhere = new List<Object>();

            where.Append("WHERE x1.id = empleado.id AND sdiEmp.id = empleado.id ");
            where.Append("AND x1.id NOT IN (SELECT CASE WHEN (COUNT(cfdiEmp) = 0) THEN 0 ELSE em.id END FROM CFDIEmpleado cfdiEmp RIGHT OUTER JOIN cfdiEmp.plazasPorEmpleadosMov cfdiPPM LEFT OUTER JOIN cfdiEmp.cfdiRecibo recibo ");
            where.Append("LEFT OUTER JOIN cfdiEmp.periodosNomina cfdiPeriodo LEFT OUTER JOIN cfdiPPM.plazasPorEmpleado cfdiPP LEFT OUTER JOIN cfdiPP.empleados em ");
            where.Append("WHERE (recibo.statusTimbrado = :statusTimbre) AND ((cfdiPeriodo.fechaInicial BETWEEN :fechaPeriodoInicio AND :fechaPeriodoFinal) OR (cfdiPeriodo.fechaFinal BETWEEN :fechaPeriodoInicio AND :fechaPeriodoFinal)) ");
            if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
            {
                where.Append("AND cfdiPeriodo.tipoCorrida.clave =:tipoCorrida ");
            }
            where.Append("AND em.razonesSociales.id = razonSocial.id GROUP By  em.id) ");
            where.Append("AND sdi.fecha IN (SELECT MAX(s0.fecha) FROM SalariosIntegrados s0 LEFT OUTER JOIN s0.empleados s0Emp WHERE s0.fecha <= :fechaFinal AND s0Emp.id = empleado.id) ");
            where.Append("AND ppm.id IN (SELECT MAX(pem.id) FROM PlazasPorEmpleadosMov pem INNER JOIN pem.plazasPorEmpleado pe INNER JOIN pe.empleados e INNER JOIN pem.tipoNomina nn ");

            where.Append("WHERE e.id = empleado.id AND :fechaFinal >= pem.fechaInicial AND pe.fechaFinal > :fechaFinal ");
            camposFiltrado.Add("statusTimbre");
            valoresWhere.Add(StatusTimbrado.TIMBRADO);
            camposFiltrado.Add("fechaFinal");
            valoresWhere.Add(datosFiltradoEmpleado.getFechaFin());

            if (datosFiltradoEmpleado.getClaveTipoNomina().Any())
            {
                where.Append("AND nn.clave = :tipoNomina) "); ///para busqueda maximo id de plaza empleado mov
                where.Append("AND nomina.clave = :tipoNomina ");
                camposFiltrado.Add("tipoNomina");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveTipoNomina());
            }
            else
            {
                where.Append(") "); //cierra la busqusqueda max plaza por empleado id 
            }

            if (datosFiltradoEmpleado.getClaveRazonSocial().Any())
            {
                where.Append("AND razonSocial.clave = :razonSocial ");
                camposFiltrado.Add("razonSocial");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveRazonSocial());
            }

            if (datosFiltradoEmpleado.getClaveCentroCosto().Any())
            {
                from.Append("LEFT OUTER JOIN ppm.centroDeCosto cc ");
                where.Append("AND cc.clave = :centroCosto ");
                camposFiltrado.Add("centroCosto");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveCentroCosto());
            }
            if (datosFiltradoEmpleado.getClaveDepartamento().Any())
            {
                from.Append("LEFT OUTER JOIN ppm.departamentos depto ");
                where.Append("AND depto.clave = :departamento ");
                camposFiltrado.Add("departamento");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveDepartamento());
            }
            if (datosFiltradoEmpleado.getClaveRegistroPatronal().Any())
            {
                from.Append("LEFT OUTER JOIN pp.registroPatronal rp ");
                where.Append("AND rp.clave = :registroPatronal ");
                camposFiltrado.Add("registroPatronal");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveRegistroPatronal());
            }
            if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any() && datosFiltradoEmpleado.getClaveFinEmpleado().Any())
            {
                where.Append("AND (empleado.clave >= :claveEmpIni AND empleado.clave <= :claveEmpFin) ");
                camposFiltrado.Add("claveEmpIni");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveInicioEmpleado());
                camposFiltrado.Add("claveEmpFin");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveFinEmpleado());
            }
            else if (!datosFiltradoEmpleado.getClaveInicioEmpleado().Any())
            {
                camposFiltrado.Add("claveEmpIni");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveInicioEmpleado());
                where.Append("AND empleado.clave >= :claveEmpIni ");
            }
            else if (datosFiltradoEmpleado.getClaveFinEmpleado().Any())
            {
                camposFiltrado.Add("claveEmpIni");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveFinEmpleado());
                where.Append("AND empleado.clave <= :claveEmpIni ");
            }

            where.Append("AND (x6.naturaleza  = :percepcion OR x6.naturaleza  = :deduccion) ");
            camposFiltrado.Add("percepcion");
            valoresWhere.Add(Entity.entidad.Naturaleza.PERCEPCION);
            camposFiltrado.Add("deduccion");
            valoresWhere.Add(Entity.entidad.Naturaleza.DEDUCCION);
            if (datosFiltradoMovNom.getClaveRazonSocial().Any())
            {
                where.Append("AND x2.clave = :razonSocial ");
                camposFiltrado.Add("razonSocial");
                valoresWhere.Add(datosFiltradoMovNom.getClaveRazonSocial());
            }
            if (datosFiltradoMovNom.getClaveTipoNomina().Any())
            {
                where.Append("AND x3.clave = :tipoNomina ");
                camposFiltrado.Add("tipoNomina");
                valoresWhere.Add(datosFiltradoMovNom.getClaveTipoNomina());
            }
            if (datosFiltradoMovNom.getClaveTipoCorrida().Any())
            {
                where.Append("AND x4.clave = :tipoCorrida ");
                camposFiltrado.Add("tipoCorrida");
                valoresWhere.Add(datosFiltradoMovNom.getClaveTipoCorrida());
            }
            if (datosFiltradoMovNom.getFechaFin() != null & datosFiltradoMovNom.getFechaInicio() != null)
            {
                where.Append("AND ((x5.fechaInicial BETWEEN :fechaPeriodoInicio AND  :fechaPeriodoFinal) OR (x5.fechaFinal BETWEEN :fechaPeriodoInicio AND  :fechaPeriodoFinal)) ");
                camposFiltrado.Add("fechaPeriodoInicio");
                valoresWhere.Add(datosFiltradoMovNom.getFechaInicio());
                camposFiltrado.Add("fechaPeriodoFinal");
                valoresWhere.Add(datosFiltradoMovNom.getFechaFin());
                if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
                {
                    where.Append("AND x5.tipoCorrida.clave = :tipoCorrida ");
                }
            }
            if (datosFiltradoMovNom.getClaveCentroCosto().Any())
            {
                from.Append("LEFT OUTER JOIN x0.centroDeCosto x8 ");
                where.Append("AND x8.clave = :centroDeCosto ");
                camposFiltrado.Add("centroDeCosto");
                valoresWhere.Add(datosFiltradoMovNom.getClaveCentroCosto());
            }

            //        if (datosFiltradoMovNom.getClavesEmpleados() != null) {
            //            if (datosFiltradoMovNom.getClavesEmpleados().length > 0) {
            //                where.append("AND x1.clave IN (:claveEmpleados) ");
            //                camposFiltrado.add("claveEmpleados");
            //                valoresWhere.add(datosFiltradoMovNom.getClavesEmpleados());
            //            }
            //        }
            orden.Append("ORDER BY CASE WHEN (x1 IS NULL) THEN '' ELSE CASE WHEN (x1.clave IS NULL) THEN '' ELSE x1.clave END END, CASE WHEN (x6 IS NULL) THEN '' ELSE CASE WHEN (x6.clave IS NULL) THEN '' ELSE x6.clave END END");
            select.Append(from).Append(where).Append(orden);
            query = getSession().CreateQuery(select.ToString());
            for (int i = 0; i < valoresWhere.Count(); i++)
            {

                query.SetParameter(camposFiltrado[i], valoresWhere[i]);

            }
            var datosConcep = query.List<object>();
            datosMovimientos = datosConcep.ToList();
            return datosMovimientos;
        }

        private List<Object> construyeQueryDatosGlobalesAsistencias(DatosFiltradoAsistencias datosFiltradoAsistencias, DatosFiltradoEmpleados datosFiltradoEmpleado)
        {
            List<Object> datosAsistencias;

            StringBuilder select = new StringBuilder();
            StringBuilder from = new StringBuilder();
            StringBuilder where = new StringBuilder();
            StringBuilder orden = new StringBuilder();

            select.Append("SELECT DISTINCT CASE WHEN (x1 IS NULL) THEN '' ELSE CASE WHEN (x1.clave IS NULL) THEN '' ELSE x1.clave END END, x0 ");

            from.Append("FROM Asistencias x0 LEFT OUTER JOIN x0.empleados x1 LEFT OUTER JOIN x0.razonesSociales x2 LEFT OUTER JOIN x0.tipoNomina x3 LEFT OUTER JOIN x0.excepciones x4 ");
            from.Append("LEFT OUTER JOIN x0.periodosNomina x5 ");
            from.Append(", PlazasPorEmpleadosMov ppm LEFT OUTER JOIN ppm.plazasPorEmpleado pp LEFT OUTER JOIN pp.empleados empleado, SalariosIntegrados sdi ");
            from.Append("LEFT OUTER JOIN ppm.tipoNomina nomina LEFT OUTER JOIN pp.razonesSociales razonSocial RIGHT OUTER JOIN sdi.empleados sdiEmp ");

            List<String> camposFiltrado = new List<String>();
            List<Object> valoresWhere = new List<Object>();

            where.Append("WHERE x1.id = empleado.id AND sdiEmp.id = empleado.id ");
            where.Append("AND x1.id NOT IN (SELECT CASE WHEN (COUNT(cfdiEmp) = 0) THEN 0 ELSE em.id END FROM CFDIEmpleado cfdiEmp RIGHT OUTER JOIN cfdiEmp.plazasPorEmpleadosMov cfdiPPM LEFT OUTER JOIN cfdiEmp.cfdiRecibo recibo ");
            where.Append("LEFT OUTER JOIN cfdiEmp.periodosNomina cfdiPeriodo LEFT OUTER JOIN cfdiPPM.plazasPorEmpleado cfdiPP LEFT OUTER JOIN cfdiPP.empleados em ");
            where.Append("WHERE (recibo.statusTimbrado = :statusTimbre) AND ((cfdiPeriodo.fechaInicial BETWEEN :fechaPeriodoInicio AND :fechaPeriodoFinal) OR (cfdiPeriodo.fechaFinal BETWEEN :fechaPeriodoInicio AND :fechaPeriodoFinal)) ");
            if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
            {
                where.Append("AND cfdiPeriodo.tipoCorrida.clave =:tipoCorrida ");
            }
            where.Append("AND em.razonesSociales.id = razonSocial.id GROUP By  em.id) ");
            where.Append("AND sdi.fecha IN (SELECT MAX(s0.fecha) FROM SalariosIntegrados s0 LEFT OUTER JOIN s0.empleados s0Emp WHERE s0.fecha <= :fechaFinal AND s0Emp.id = empleado.id) ");
            where.Append("AND ppm.id IN (SELECT MAX(pem.id) FROM PlazasPorEmpleadosMov pem INNER JOIN pem.plazasPorEmpleado pe INNER JOIN pe.empleados e INNER JOIN pem.tipoNomina nn ");

            where.Append("WHERE e.id = empleado.id AND :fechaFinal >= pem.fechaInicial AND pe.fechaFinal > :fechaFinal ");
            camposFiltrado.Add("statusTimbre");
            valoresWhere.Add(StatusTimbrado.TIMBRADO);
            camposFiltrado.Add("fechaFinal");
            valoresWhere.Add(datosFiltradoEmpleado.getFechaFin());

            if (datosFiltradoEmpleado.getClaveTipoNomina().Any())
            {
                where.Append("AND nn.clave = :tipoNomina) "); ///para busqueda maximo id de plaza empleado mov
                where.Append("AND nomina.clave = :tipoNomina ");
                camposFiltrado.Add("tipoNomina");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveTipoNomina());
            }
            else
            {
                where.Append(") "); //cierra la busqusqueda max plaza por empleado id 
            }

            if (datosFiltradoEmpleado.getClaveRazonSocial().Any())
            {
                where.Append("AND razonSocial.clave = :razonSocial ");
                camposFiltrado.Add("razonSocial");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveRazonSocial());
            }

            if (datosFiltradoEmpleado.getClaveCentroCosto().Any())
            {
                from.Append("LEFT OUTER JOIN ppm.centroDeCosto cc ");
                where.Append("AND cc.clave = :centroCosto ");
                camposFiltrado.Add("centroCosto");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveCentroCosto());
            }
            if (datosFiltradoEmpleado.getClaveDepartamento().Any())
            {
                from.Append("LEFT OUTER JOIN ppm.departamentos depto ");
                where.Append("AND depto.clave = :departamento ");
                camposFiltrado.Add("departamento");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveDepartamento());
            }
            if (datosFiltradoEmpleado.getClaveRegistroPatronal().Any())
            {
                from.Append("LEFT OUTER JOIN pp.registroPatronal rp ");
                where.Append("AND rp.clave = :registroPatronal ");
                camposFiltrado.Add("registroPatronal");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveRegistroPatronal());
            }
            if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any() && datosFiltradoEmpleado.getClaveFinEmpleado().Any())
            {
                where.Append("AND (empleado.clave >= :claveEmpIni AND empleado.clave <= :claveEmpFin) ");
                camposFiltrado.Add("claveEmpIni");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveInicioEmpleado());
                camposFiltrado.Add("claveEmpFin");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveFinEmpleado());
            }
            else if (datosFiltradoEmpleado.getClaveInicioEmpleado().Any())
            {
                camposFiltrado.Add("claveEmpIni");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveInicioEmpleado());
                where.Append("AND empleado.clave >= :claveEmpIni ");
            }
            else if (datosFiltradoEmpleado.getClaveFinEmpleado().Any())
            {
                camposFiltrado.Add("claveEmpIni");
                valoresWhere.Add(datosFiltradoEmpleado.getClaveFinEmpleado());
                where.Append("AND empleado.clave <= :claveEmpIni ");
            }

            where.Append(" AND (x4.clave = :exepcionExtraDoble OR x4.clave = :exepcionExtraTriple OR x4.clave = :excepcionAccidente  OR x4.clave  = :excepcionEnfermedad  OR x4.clave  = :excepcionMaternidad) ");
            camposFiltrado.Add("exepcionExtraDoble");
            valoresWhere.Add(ClavesParametrosModulos.claveExcepcionExtraDoble);
            camposFiltrado.Add("exepcionExtraTriple");
            valoresWhere.Add(ClavesParametrosModulos.claveExcepcionExtraTriple);
            camposFiltrado.Add("excepcionAccidente");
            valoresWhere.Add(ClavesParametrosModulos.claveExcepcionIncapacidadPorAccidente);
            camposFiltrado.Add("excepcionEnfermedad");
            valoresWhere.Add(ClavesParametrosModulos.claveExcepcionIncapacidadPorEnfermedad);
            camposFiltrado.Add("excepcionMaternidad");
            valoresWhere.Add(ClavesParametrosModulos.claveExcepcionIncapacidadPorMaternidad);
            if (datosFiltradoAsistencias.getClaveRazonSocial().Any())
            {
                where.Append("AND x2.clave = :razonSocial ");
                camposFiltrado.Add("razonSocial");
                valoresWhere.Add(datosFiltradoAsistencias.getClaveRazonSocial());
            }
            if (!datosFiltradoAsistencias.getClaveTipoNomina().Any())
            {
                where.Append("AND x3.clave = :tipoNomina ");
                camposFiltrado.Add("tipoNomina");
                valoresWhere.Add(datosFiltradoAsistencias.getClaveTipoNomina());
            }
            if (datosFiltradoAsistencias.getFechaFin() != null & datosFiltradoAsistencias.getFechaInicio() != null)
            {
                where.Append("AND ((x5.fechaInicial BETWEEN :fechaPeriodoInicio AND  :fechaPeriodoFinal) OR (x5.fechaFinal BETWEEN :fechaPeriodoInicio AND  :fechaPeriodoFinal)) ");
                camposFiltrado.Add("fechaPeriodoInicio");
                valoresWhere.Add(datosFiltradoAsistencias.getFechaInicio());
                camposFiltrado.Add("fechaPeriodoFinal");
                valoresWhere.Add(datosFiltradoAsistencias.getFechaFin());
                if (datosFiltradoEmpleado.getClaveTipoCorrida().Any())
                {
                    where.Append("AND x5.tipoCorrida.clave = :tipoCorrida ");
                    camposFiltrado.Add("tipoCorrida");
                    valoresWhere.Add("PER");
                }
            }
            if (datosFiltradoAsistencias.getClaveCentroCosto().Any())
            {
                from.Append("LEFT OUTER JOIN x0.centroDeCosto x8 ");
                where.Append("AND x8.clave = :centroDeCosto ");
                camposFiltrado.Add("centroDeCosto");
                valoresWhere.Add(datosFiltradoAsistencias.getClaveCentroCosto());
            }
            //        if (datosFiltradoAsistencias.getClavesEmpleados() != null) {
            //            if (datosFiltradoAsistencias.getClavesEmpleados().length > 0) {
            //                where.append("AND x1.clave IN (:claveEmpleados) ");
            //                camposFiltrado.add("claveEmpleados");
            //                valoresWhere.add(datosFiltradoAsistencias.getClavesEmpleados());
            //            }
            //        }
            orden.Append("ORDER BY CASE WHEN (x1 IS NULL) THEN '' ELSE CASE WHEN (x1.clave IS NULL) THEN '' ELSE x1.clave END END");
            select.Append(from).Append(where).Append(orden);
            query = getSession().CreateQuery(select.ToString());
            for (int i = 0; i < valoresWhere.Count(); i++)
            {

                query.SetParameter(camposFiltrado[i], valoresWhere[i]);

            }
            var datosConcep = query.List<object>();
            datosAsistencias = datosConcep.ToList();
            return datosAsistencias;
        }

        private void obtenerFactores(Object clavesElementoAplicacion, Periodicidad periodicidadTipoNomina)
        {
            String valor = null;

            try
            {
                #region Maneja pagos por hora
                if (manejaPagosPorHora == null)
                {
                    strQuery.Remove(0, strQuery.Length).Append("SELECT cr.valor");
                    strQuery.Append(" FROM Cruce cr ");
                    strQuery.Append(" INNER JOIN cr.parametros pr ");
                    strQuery.Append(" INNER JOIN cr.elementosAplicacion ea ");
                    strQuery.Append(" WHERE pr.clave = :parametro ");
                    strQuery.Append(" and ea.clave = :elementoAplicacion ");
                    strQuery.Append(" and cr.claveElemento = :claveElemento");
                    query = getSession().CreateQuery(strQuery.ToString());
                    query.SetDecimal("parametro", Convert.ToDecimal(ClavesParametrosModulos.claveParametroPagosPorHora));
                    query.SetString("elementoAplicacion", ClavesParametrosModulos.claveElementoAplicacionRazonSocial.ToString());
                    query.SetString("claveElemento", clavesElementoAplicacion.ToString());
                    query.SetMaxResults(1);
                    valor = (string)query.UniqueResult();
                    valor = (valor == null ? "" : valor);
                    if (valor.Equals(""))
                    {
                        strQuery.Remove(0, strQuery.Length).Append("SELECT pr.valor ");
                        strQuery.Append(" FROM Parametros pr");
                        strQuery.Append(" INNER JOIN pr.modulo m ");
                        strQuery.Append(" WHERE pr.clave = :parametro ");
                        strQuery.Append(" AND m.clave =:modulo");
                        query = getSession().CreateQuery(strQuery.ToString());
                        query.SetDecimal("parametro", Convert.ToDecimal(ClavesParametrosModulos.claveParametroManejarHorasPor));
                        query.SetString("modulo", ClavesParametrosModulos.claveModuloGlobal.ToString());
                        query.SetMaxResults(1);
                        valor = (string)query.UniqueResult();
                    }

                    if (valor.Equals(ClavesParametrosModulos.opcionParametroHorasNaturales))
                    {
                        manejoHorasPor = Entity.entidad.ManejoHorasPor.HORASNATURALES;
                    }
                    else if (valor.Equals(ClavesParametrosModulos.opcionParametroHorasHSM))
                    {
                        manejoHorasPor = Entity.entidad.ManejoHorasPor.HSM;
                    }

                }
                #endregion

                #region Manejo horas Por
                if (manejoHorasPor == null)
                {
                    strQuery.Remove(0, strQuery.Length).Append("SELECT cr.valor ");
                    strQuery.Append(" FROM Cruce cr ");
                    strQuery.Append(" INNER JOIN cr.parametros pr ");
                    strQuery.Append(" INNER JOIN cr.elementosAplicacion ea ");
                    strQuery.Append(" WHERE pr.clave = :parametro ");
                    strQuery.Append(" and ea.clave = :elementoAplicacion ");
                    strQuery.Append(" and cr.claveElemento = :claveElemento");
                    query = getSession().CreateQuery(strQuery.ToString());
                    query.SetDecimal("parametro", Convert.ToDecimal(ClavesParametrosModulos.claveParametroManejarHorasPor));
                    query.SetString("elementoAplicacion", ClavesParametrosModulos.claveElementoAplicacionRazonSocial.ToString());
                    query.SetString("claveElemento", clavesElementoAplicacion.ToString());
                    query.SetMaxResults(1);
                    valor = (string)query.UniqueResult();

                    valor = (valor == null ? "" : valor);
                    if (valor.Equals(""))
                    {
                        strQuery.Remove(0, strQuery.Length).Append("SELECT pr.valor ");
                        strQuery.Append(" FROM Parametros pr");
                        strQuery.Append(" INNER JOIN pr.modulo m ");
                        strQuery.Append(" WHERE pr.clave = :parametro ");
                        strQuery.Append(" AND m.clave =:modulo");

                        query = getSession().CreateQuery(strQuery.ToString());
                        query.SetDecimal("parametro", Convert.ToDecimal(ClavesParametrosModulos.claveParametroManejarHorasPor));
                        query.SetString("modulo", ClavesParametrosModulos.claveModuloGlobal.ToString());
                        query.SetMaxResults(1);
                        valor = (string)query.UniqueResult();
                    }
                    if (valor.Equals(ClavesParametrosModulos.opcionParametroHorasNaturales))
                    {
                        manejoHorasPor = Entity.entidad.ManejoHorasPor.HORASNATURALES;
                    }
                    else if (valor.Equals(ClavesParametrosModulos.opcionParametroHorasHSM))
                    {
                        manejoHorasPor = Entity.entidad.ManejoHorasPor.HSM;
                    }
                }
                #endregion

                #region Manejo de pagos dias naturales
                if (!manejaPagoDiasNaturales)
                {//BUSQUEDA POR PERIODICIDAD
                    strQuery.Remove(0, strQuery.Length).Append("SELECT cr.valor");
                    strQuery.Append(" FROM Cruce cr ");
                    strQuery.Append(" INNER JOIN cr.parametros pr ");
                    strQuery.Append(" INNER JOIN cr.elementosAplicacion ea ");
                    strQuery.Append(" WHERE pr.clave = :parametro ");
                    strQuery.Append(" and ea.clave = :elementoAplicacion ");
                    strQuery.Append(" and cr.claveElemento = :claveElemento");
                    if (periodicidadTipoNomina != null)
                    {

                        query = getSession().CreateQuery(strQuery.ToString());
                        query.SetDecimal("parametro", Convert.ToDecimal(ClavesParametrosModulos.clavePagarNominaDiasNaturales));
                        query.SetString("elementoAplicacion", ClavesParametrosModulos.claveElementoAplicacionPeriodicidad.ToString());
                        query.SetString("claveElemento", periodicidadTipoNomina.clave);
                        query.SetMaxResults(1);
                        valor = (string)query.UniqueResult();

                    }
                    valor = (valor == null ? "" : valor);
                    if (valor.Equals(""))
                    {//BUSQUEDA POR RAZON SOCIAL
                        strQuery.Remove(0, strQuery.Length).Append("SELECT cr.valor");
                        strQuery.Append(" FROM Cruce cr ");
                        strQuery.Append(" INNER JOIN cr.parametros pr ");
                        strQuery.Append(" INNER JOIN cr.elementosAplicacion ea ");
                        strQuery.Append(" WHERE pr.clave = :parametro ");
                        strQuery.Append(" and ea.clave = :elementoAplicacion ");
                        strQuery.Append(" and cr.claveElemento = :claveElemento");
                        query = getSession().CreateQuery(strQuery.ToString());
                        query.SetDecimal("parametro", Convert.ToDecimal(ClavesParametrosModulos.clavePagarNominaDiasNaturales));
                        query.SetString("elementoAplicacion", ClavesParametrosModulos.claveElementoAplicacionRazonSocial.ToString());
                        query.SetString("claveElemento", clavesElementoAplicacion.ToString());
                        query.SetMaxResults(1);
                        valor = (string)query.UniqueResult();
                        valor = (valor == null ? "" : valor);
                    }
                    if (valor.Equals(""))
                    {//BUSQUEDA GLOBAL
                        strQuery.Remove(0, strQuery.Length).Append("SELECT pr.valor ");
                        strQuery.Append(" FROM Parametros pr ");
                        strQuery.Append(" INNER JOIN pr.modulo m ");
                        strQuery.Append(" WHERE pr.clave = :parametro ");
                        strQuery.Append(" AND m.clave =:modulo");
                        query = getSession().CreateQuery(strQuery.ToString());
                        query.SetDecimal("parametro", Convert.ToDecimal(ClavesParametrosModulos.clavePagarNominaDiasNaturales));
                        query.SetString("modulo", ClavesParametrosModulos.claveModuloGlobal.ToString());
                        query.SetMaxResults(1);
                        valor = (string)query.UniqueResult();

                    }
                    if (valor.Equals(ClavesParametrosModulos.opcionParametroPagarPorDiaNatural))
                    {
                        manejaPagoDiasNaturales = true;
                    }
                    else
                    {
                        manejaPagoDiasNaturales = false;
                    }
                }
                #endregion

                #region Manejo de Salario Diario
                if (manejaPagoDiasNaturales)
                {
                    manejoSalarioDiario = Entity.entidad.ManejoSalarioDiario.DIARIO;
                }
                else
                {
                    if (manejoSalarioDiario == null)
                    {
                        strQuery.Remove(0, strQuery.Length).Append("SELECT cr.valor ");
                        strQuery.Append(" FROM Cruce cr ");
                        strQuery.Append(" INNER JOIN cr.parametros pr ");
                        strQuery.Append(" INNER JOIN cr.elementosAplicacion ea ");
                        strQuery.Append(" WHERE pr.clave = :parametro ");
                        strQuery.Append(" and ea.clave = :elementoAplicacion ");
                        strQuery.Append(" and cr.claveElemento = :claveElemento");
                        query = getSession().CreateQuery(strQuery.ToString());
                        query.SetDecimal("parametro", Convert.ToDecimal(ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor));
                        query.SetString("elementoAplicacion", ClavesParametrosModulos.claveElementoAplicacionRazonSocial.ToString());
                        query.SetString("claveElemento", clavesElementoAplicacion.ToString());
                        query.SetMaxResults(1);
                        valor = (string)query.UniqueResult();

                        valor = (valor == null ? "" : valor);
                        if (valor.Equals(""))
                        {
                            strQuery.Remove(0, strQuery.Length).Append("SELECT pr.valor ");
                            strQuery.Append(" FROM Parametros pr");
                            strQuery.Append(" INNER JOIN pr.modulo m ");
                            strQuery.Append(" WHERE pr.clave = :parametro ");
                            strQuery.Append(" AND m.clave =:modulo");
                            query = getSession().CreateQuery(strQuery.ToString());
                            query.SetDecimal("parametro", Convert.ToDecimal(ClavesParametrosModulos.claveParametroManejarSalarioDiarioPor));
                            query.SetString("modulo", ClavesParametrosModulos.claveModuloGlobal.ToString());
                            query.SetMaxResults(1);
                            valor = (string)query.UniqueResult();

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
                    }
                }
                #endregion
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
        }

        private PlazasPorEmpleadosMov calculaSueldoDiario(PlazasPorEmpleadosMov ppem)
        {
            double sueldoDiario = 0.0;
            //if (ppem.salarioPor == 2)
            //{
            //    sueldoDiario = ppem.importe.GetValueOrDefault();
            //}
            //else
            //{
            //    sueldoDiario = ppem.puestos.salarioTabular;
            //}


            ppem.sueldoDiario = (sueldoDiario);
            return ppem;
        }

        private DatosParaTimbrar agregaIncapacidadesHorasExtras(List<Asistencias> listAsistencias, DatosParaTimbrar datosParaTimbrar)
        {
            if (datosParaTimbrar == null)
            {
                datosParaTimbrar = new DatosParaTimbrar();
            }
            int diasIncapacidadEnfermedad = 0, diasIncapacidadAccidente = 0, diasIncapacidadMaternidad = 0, diasHorasDobles = 0, diasHorasTriples = 0, diasTiempoExtra = 0;
            Double hrsExtraDoble = 0.0, hrsExtraTriple = 0.0;
            ///tiempoExtra = 0.0;
            int x;
            Asistencias asistenciaEnfermedad = null, asistenciaAccidente = null, asistenciaMaternidad = null, asistenciaHrsDobles = null, asistenciaHrsTriples = null;
            if (listAsistencias != null)
            {
                for (x = 0; x < listAsistencias.Count(); x++)
                {
                    switch (Convert.ToInt32(listAsistencias[x].excepciones.clave))
                    {
                        case 6://IncapacidadPorEnfermedad = "6";
                            asistenciaEnfermedad = listAsistencias[x];
                            diasIncapacidadEnfermedad++;
                            break;
                        case 7://IncapacidadPorAccidente = "7";
                            asistenciaAccidente = listAsistencias[x];
                            diasIncapacidadAccidente++;
                            break;
                        case 8://IncapacidadPorMaternidad = "8";
                            asistenciaMaternidad = listAsistencias[x];
                            diasIncapacidadMaternidad++;
                            asistenciaEnfermedad = listAsistencias[x];
                            break;
                        case 14://ExtraDoble = "14";
                            asistenciaHrsDobles = listAsistencias[x];
                            diasHorasDobles++;
                            hrsExtraDoble += listAsistencias[x].cantidad.GetValueOrDefault();
                            break;
                        case 15://ExtraTriple = "15";
                            asistenciaHrsTriples = listAsistencias[x];
                            diasHorasTriples++;
                            hrsExtraTriple += listAsistencias[x].cantidad.GetValueOrDefault();
                            break;
                    }
                }
                List<DatosHorasExtras> listHrsExtras = new List<DatosHorasExtras>();
                DatosHorasExtras extra;
                if (asistenciaHrsDobles != null)
                {
                    extra = new DatosHorasExtras();
                    extra.asistencia = (converAsistencia(asistenciaHrsDobles));
                    extra.dias = (diasHorasDobles);
                    extra.hrsExtas = (Convert.ToInt32(hrsExtraDoble));
                    listHrsExtras.Add(extra);
                }
                if (asistenciaHrsTriples != null)
                {
                    extra = new DatosHorasExtras();
                    extra.asistencia = (converAsistencia(asistenciaHrsTriples));
                    extra.dias = (diasHorasTriples);
                    extra.hrsExtas = (Convert.ToInt32(hrsExtraTriple));
                    listHrsExtras.Add(extra);
                }
                List<DatosIncapacidades> listIncapacidades = new List<DatosIncapacidades>();
                DatosIncapacidades incapacidad;
                if (asistenciaAccidente != null)
                {
                    incapacidad = new DatosIncapacidades();
                    incapacidad.asistencia = (converAsistencia(asistenciaAccidente));
                    incapacidad.dias = (diasIncapacidadAccidente);
                    listIncapacidades.Add(incapacidad);
                }
                if (asistenciaEnfermedad != null)
                {
                    incapacidad = new DatosIncapacidades();
                    incapacidad.asistencia = (converAsistencia(asistenciaEnfermedad));
                    incapacidad.dias = (diasIncapacidadEnfermedad);
                    listIncapacidades.Add(incapacidad);
                }
                if (asistenciaMaternidad != null)
                {
                    incapacidad = new DatosIncapacidades();
                    incapacidad.asistencia = (converAsistencia(asistenciaMaternidad));
                    incapacidad.dias = (diasIncapacidadMaternidad);
                    listIncapacidades.Add(incapacidad);
                }
                datosParaTimbrar.datosHorasExtras = (listHrsExtras);
                datosParaTimbrar.datosIncapacidades = (listIncapacidades);
            }
            return datosParaTimbrar;
        }

        private Entity.entidad.Asistencias converAsistencia(Asistencias asistencia)
        {
            Entity.entidad.Asistencias asistenciaAux = new Entity.entidad.Asistencias();
            asistenciaAux.cantidad = asistencia.cantidad;
            // asistenciaAux.centroDeCosto_ID = asistencia.centroDeCosto.id;
            asistenciaAux.empleados_ID = asistencia.empleados.id;
            asistenciaAux.excepciones_ID = asistencia.excepciones.id;
            asistenciaAux.fecha = asistencia.fecha;
            asistenciaAux.id = asistencia.id;
            asistenciaAux.jornada = asistencia.jornada;
            asistenciaAux.ordenId = asistencia.ordenId;
            asistenciaAux.periodosNomina_ID = asistencia.periodosNomina.id;
            asistenciaAux.razonesSociales_ID = asistencia.razonesSociales.id;
            asistenciaAux.tipoNomina_ID = asistencia.tipoNomina.id;
            asistenciaAux.tipoPantalla = asistencia.tipoPantalla;
            return asistenciaAux;

        }

        private void nullVariablesGlobales()
        {
            manejaPagosPorHora = null;
            manejoHorasPor = null;
            manejoSalarioDiario = null;
        }

        private class DatosFiltradoEmpleados
        {

            private String claveRazonSocial;
            private String claveTipoNomina;
            private String claveRegistroPatronal;
            private String claveCentroCosto;
            private String claveDepartamento;
            private String claveInicioEmpleado;
            private String claveFinEmpleado;
            private String claveTipoCorrida;
            private DateTime? fechaInicio;
            private DateTime? fechaFin;

            public String getClaveRazonSocial()
            {
                return claveRazonSocial == null ? "" : claveRazonSocial;
            }

            public void setClaveRazonSocial(String claveRazonSocial)
            {
                this.claveRazonSocial = claveRazonSocial;
            }

            public String getClaveTipoNomina()
            {
                return claveTipoNomina == null ? "" : claveTipoNomina;
            }

            public void setClaveTipoNomina(String claveTipoNomina)
            {
                this.claveTipoNomina = claveTipoNomina;
            }

            public String getClaveRegistroPatronal()
            {
                return claveRegistroPatronal == null ? "" : claveRegistroPatronal;
            }

            public void setClaveRegistroPatronal(String claveRegistroPatronal)
            {
                this.claveRegistroPatronal = claveRegistroPatronal;
            }

            public String getClaveCentroCosto()
            {
                return claveCentroCosto == null ? "" : claveCentroCosto;
            }

            public void setClaveCentroCosto(String claveCentroCosto)
            {
                this.claveCentroCosto = claveCentroCosto;
            }

            public String getClaveDepartamento()
            {
                return claveDepartamento == null ? "" : claveDepartamento;
            }

            public void setClaveDepartamento(String claveDepartamento)
            {
                this.claveDepartamento = claveDepartamento;
            }

            public String getClaveInicioEmpleado()
            {
                return claveInicioEmpleado == null ? "" : claveInicioEmpleado;
            }

            public void setClaveInicioEmpleado(String claveInicioEmpleado)
            {
                this.claveInicioEmpleado = claveInicioEmpleado;
            }

            public String getClaveFinEmpleado()
            {
                return claveFinEmpleado == null ? "" : claveFinEmpleado;
            }

            public void setClaveFinEmpleado(String claveFinEmpleado)
            {
                this.claveFinEmpleado = claveFinEmpleado;
            }

            public DateTime? getFechaInicio()
            {
                return fechaInicio;
            }

            public void setFechaInicio(DateTime? fechaInicio)
            {
                this.fechaInicio = fechaInicio;
            }

            public DateTime? getFechaFin()
            {
                return fechaFin;
            }

            public void setFechaFin(DateTime? fechaFin)
            {
                this.fechaFin = fechaFin;
            }

            public String getClaveTipoCorrida()
            {
                return claveTipoCorrida == null ? "" : claveTipoCorrida;
            }

            public void setClaveTipoCorrida(String claveTipoCorrida)
            {
                this.claveTipoCorrida = claveTipoCorrida;
            }

        }

        private class DatosFiltradoMovNom
        {

            private String claveRazonSocial;
            private String claveTipoNomina;
            private String claveTipoCorrida;
            private String claveCentroCosto;
            private DateTime? fechaInicio;
            private DateTime? fechaFin;
            public String getClaveRazonSocial()
            {
                return claveRazonSocial == null ? "" : claveRazonSocial;
            }

            public void setClaveRazonSocial(String claveRazonSocial)
            {
                this.claveRazonSocial = claveRazonSocial;
            }

            public String getClaveTipoNomina()
            {
                return claveTipoNomina == null ? "" : claveTipoNomina;
            }

            public void setClaveTipoNomina(String claveTipoNomina)
            {
                this.claveTipoNomina = claveTipoNomina;
            }

            public String getClaveTipoCorrida()
            {
                return claveTipoCorrida == null ? "" : claveTipoCorrida;
            }

            public void setClaveTipoCorrida(String claveTipoCorrida)
            {
                this.claveTipoCorrida = claveTipoCorrida;
            }

            public String getClaveCentroCosto()
            {
                return claveCentroCosto == null ? "" : claveCentroCosto;
            }

            public void setClaveCentroCosto(String claveCentroCosto)
            {
                this.claveCentroCosto = claveCentroCosto;
            }

            public DateTime? getFechaInicio()
            {
                return fechaInicio;
            }

            public void setFechaInicio(DateTime? fechaInicio)
            {
                this.fechaInicio = fechaInicio;
            }

            public DateTime? getFechaFin()
            {
                return fechaFin;
            }

            public void setFechaFin(DateTime? fechaFin)
            {
                this.fechaFin = fechaFin;
            }
        }

        private class DatosFiltradoAsistencias
        {

            private String claveRazonSocial;
            private String claveTipoNomina;
            private String claveCentroCosto;
            private DateTime? fechaInicio;
            private DateTime? fechaFin;
            private String[] clavesEmpleados;

            public String getClaveRazonSocial()
            {
                return claveRazonSocial == null ? "" : claveRazonSocial;
            }

            public void setClaveRazonSocial(String claveRazonSocial)
            {
                this.claveRazonSocial = claveRazonSocial;
            }

            public String getClaveTipoNomina()
            {
                return claveTipoNomina == null ? "" : claveTipoNomina;
            }

            public void setClaveTipoNomina(String claveTipoNomina)
            {
                this.claveTipoNomina = claveTipoNomina;
            }

            public String getClaveCentroCosto()
            {
                return claveCentroCosto == null ? "" : claveCentroCosto;
            }

            public void setClaveCentroCosto(String claveCentroCosto)
            {
                this.claveCentroCosto = claveCentroCosto;
            }

            public DateTime? getFechaInicio()
            {
                return fechaInicio;
            }

            public void setFechaInicio(DateTime? fechaInicio)
            {
                this.fechaInicio = fechaInicio;
            }

            public DateTime? getFechaFin()
            {
                return fechaFin;
            }

            public void setFechaFin(DateTime? fechaFin)
            {
                this.fechaFin = fechaFin;
            }

            public String[] getClavesEmpleados()
            {
                return clavesEmpleados;
            }

            public void setClavesEmpleados(String[] clavesEmpleados)
            {
                this.clavesEmpleados = clavesEmpleados;
            }
        }
    }
}
