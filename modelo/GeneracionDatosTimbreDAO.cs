using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Entity.entidad.cfdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using System.Diagnostics;
using Exitosw.Payroll.Entity.util;
using Exitosw.Payroll.TestCompilador.funciones;
using Exitosw.Payroll.Core.CFDI;
using Exitosw.Payroll.Core.CFDI.Timbrado;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

namespace Exitosw.Payroll.Core.modelo
{
    public class GeneracionDatosTimbreDAO : GenericRepository<CFDIEmpleado>, GeneracionDatosTimbreDAOIF
    {

        private RazonesSociales razonesSocialesActual;
        private Usuario usuarioActual;
        private decimal? idTipoCorr;
        private decimal? idTipoNom;
        private decimal? idPeriodoNom;
        private decimal? idRegPatr;
        private decimal? idCenCost;
        private decimal? idDepart;
        private decimal? idEmpleadoIni;
        private decimal? idEmpleadoFin;
        private decimal? idRazSocial;
        private string claveRazSoc;
        private List<decimal?> listIdEmpl;
        private DBContextAdapter dbContSimple;
        private DBContextAdapter dbConttMaster;
        PeriodosNominaDAO perDAO = new PeriodosNominaDAO();
        CFDIEmpleadoDAO cfdiEmpleadoDAO = new CFDIEmpleadoDAO();
        CFDIEmpleadoDAOH cfdiEmpleadoDAOH = new CFDIEmpleadoDAOH();
        CertificadosDAO certificadoDAO = new CertificadosDAO();
        TipoCorridaDAO tipoCorridaDAO = new TipoCorridaDAO();
        ISession sessionSimple1;
        ISession sessionMaster1;
        BITCancelacionDAO bitCancelacionDAO = new BITCancelacionDAO();
        CFDIReciboProcCancDAO cFDIReciboProcCancDAO = new CFDIReciboProcCancDAO();
        private DateTime fechaActualPeriodo = DateTime.Now;
        private Dictionary<String, ConfigConceptosSat> mapConceptosSAT;
        private Dictionary<String, MovNomConcep> mapConceptosIncapacidades;
        private Dictionary<String, MovNomConcep> mapConceptosHrsExtras;
        private bool nombreCompletoPeriodicidad = false;
        private PeriodosNomina periodosNomina = new PeriodosNomina();
        private TipoNomina tipoNomina = new TipoNomina();
        private TipoCorrida tipoCorrida = new TipoCorrida();
        private Empleados empleado = new Empleados();
        private CentroDeCosto centroDeCostos = new CentroDeCosto();
        private Departamentos departamento = new Departamentos();
        private RegistroPatronal registroPatronal = new RegistroPatronal();
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        int error = 0;

        public GeneracionDatosTimbreDAO() { }

        public Mensaje generacionDatosTimbre(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal, decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster, ISession sessionSimple, ISession sessionMaster)
        {
            idTipoCorr = idTipoCorrida;
            idTipoNom = idTipoNomina;
            idPeriodoNom = idPeriodoNomina;
            idRegPatr = idRegPatronal;
            idCenCost = idCenCosto;
            idDepart = idDepartamento;
            idEmpleadoIni = idEmplIni;
            idEmpleadoFin = idEmplFin;
            idRazSocial = idRazonSocial;
            dbContSimple = dbContextSimple;
            dbConttMaster = dbContextMaster;
            sessionSimple1 = sessionSimple;
            sessionMaster1 = sessionMaster;
            mensajeResultado = getRazonSocialActual();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "Error consultar RazonSocial";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            mensajeResultado = getUsuarioActual();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "Error consultar Usuario";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            inicializaValores();
            mensajeResultado = getStatusPeriodoNomina();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "Error consultar periodo";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }

            bool periodoOpen = (bool)mensajeResultado.resultado;
            if (periodoOpen)
            {
                mensajeResultado.error = "Periodo esta abierto";
                mensajeResultado.noError = 6;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            bool certVigente = existeCertificadoVigente(dbContextSimple);
            if (!certVigente)
            {
                mensajeResultado.error = "no existe certificado vigente";
                mensajeResultado.noError = 10;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }

            //Stopwatch sw = new Stopwatch(); // Creación del Stopwatch.
            //sw.Start(); // Iniciar la medición.
            // haces tus cosas acá...
            //List<Object> wh = obtenerValoresFiltradoHir();
            List<Object> wh = obtenerValoresFiltrado();


            //Crea Consulta para poder generar CFDiEmpleado
            mensajeResultado = cfdiEmpleadoDAO.generaDatosParaTimbrado(wh, claveRazSoc, dbContextSimple, dbContextMaster);
            //Hibernate.entidad.Mensaje mensaja = cfdiEmpleadoDAOH.generaDatosParaTimbrado(wh, claveRazSoc, sessionSimple, sessionMaster);

            //sw.Stop(); // Detener la medición.
            //Console.WriteLine("Time elapsed: {0}", sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")); // Mostrar el tiempo transcurriodo con un formato hh:mm:ss.000
            if (mensajeResultado.noError != 0)
            {

                return mensajeResultado;
            }
            else
            {

                mensajeResultado = CrearListadoEmpleadosCFDI((List<DatosParaTimbrar>)mensajeResultado.resultado);


                if (mensajeResultado.resultado != null)
                {



                    List<CFDIEmpleado> cfdiEmpleado = (List<CFDIEmpleado>)mensajeResultado.resultado;

                    inicializaVariableMensaje();

                    //mensajeResultado = SaveCFDIInformacion(cfdiEmpleado);


                    var datos = cfdiEmpleado.Select(d => new
                    {
                        d.id,
                        d.CLABE,
                        d.CURP,
                        d.RFC,
                        d.antiguedad,
                        d.antiguedadYMD,
                        d.apellidoMaterno,
                        d.apellidoPaterno,
                        d.calle,
                        d.ciudad,
                        d.claveBancoSat,
                        d.codigoPostal,
                        d.colonia,
                        d.correoElectronico,
                        d.cuentaBancaria,
                        d.departamento,
                        d.estado,
                        d.fechaFinalPago,
                        d.fechaInicioPago,
                        d.fechaInicioRelLaboral,
                        d.fechaPago,
                        d.formaPago,
                        d.jornada,
                        d.municipio,
                        d.noExterior,
                        d.noInterior,
                        d.noRegistroPatronal,
                        d.noSeguroSocial,
                        d.nombre,
                        d.numeroDiasPago,
                        d.pais,
                        d.periodiciadadPago,
                        d.puesto,
                        d.regimenContratacion,
                        d.riesgoPuesto,
                        d.salBaseCotAport,
                        d.salIntIMSS,
                        d.tipoContrato,
                        tipoNomina_ID = d.tipoNomina.id,
                        tipoCorrida_ID = d.tipoCorrida.id,
                        razonesSociales_ID = d.razonesSociales.id,
                        periodosNomina_ID = d.periodosNomina.id,
                        plazaPorEmpleadoMov_ID = d.plazasPorEmpleadosMov.id,
                        d.statusGeneraInfo,
                        d.asignoFolio,
                        d.recienTimbrado,
                        d.mensaje,
                        clave = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                        d.cfdiRecibo_ID,
                        cfdiRecibo = new
                        {
                            d.cfdiRecibo.id,
                            d.cfdiRecibo.UUID,
                            d.cfdiRecibo.serie,
                            d.cfdiRecibo.acuse,
                            d.cfdiRecibo.UUIDRelacionado,
                            d.cfdiRecibo.cadenaCertificado,
                            d.cfdiRecibo.cadenaOriginalTimbrado,
                            d.cfdiRecibo.fechaGeneraInfo,
                            d.cfdiRecibo.fechaHoraTimCancelado,
                            d.cfdiRecibo.fechaEmision,
                            d.cfdiRecibo.folioCFDI,
                            d.cfdiRecibo.leyenda,
                            d.cfdiRecibo.motivoCancelacion,
                            d.cfdiRecibo.noCertificado,
                            d.cfdiRecibo.noCertificadoSAT,
                            d.cfdiRecibo.rfcProvCertif,
                            d.cfdiRecibo.sello,
                            d.cfdiRecibo.selloCFD,
                            d.cfdiRecibo.selloSAT,
                            d.cfdiRecibo.serieCFDI,
                            d.cfdiRecibo.statusTimbrado,
                            d.cfdiRecibo.statusXmlSat,
                            d.cfdiRecibo.total,
                            d.cfdiRecibo.version,
                            d.cfdiRecibo.xmlTimbrado,
                            d.cfdiRecibo.certificadoTimbrado,
                            d.cfdiRecibo.fechaHoraTimbrado,
                            d.cfdiRecibo.selloTimbrado,
                            d.cfdiRecibo.mensajeRec,
                            cfdiReciboConcepto = d.cfdiRecibo.cfdiReciboConcepto.Select(r => new
                            {
                                r.id,
                                r.claveConcepto,
                                r.claveSAT,
                                r.descripcionConcepto,
                                r.importeExento,
                                r.importeGravable,
                                r.otroPago,
                                r.tipoNaturaleza
                            }).ToList(),
                            cfdiReciboHrsExtras = d.cfdiRecibo.cfdiReciboHrsExtras.Select(h => new
                            {
                                h.id,
                                h.dias,
                                h.horasExtras,
                                h.importeExento,
                                h.importeGravable
                            }).ToList(),
                            cfdiReciboIncapacidad = d.cfdiRecibo.cfdiReciboIncapacidad.Select(i => new
                            {
                                i.id,
                                i.diasIncapacidad,
                                i.importeMonetario,
                                i.tipoIncapacidad
                            }).ToList()
                        }
                    }).ToList();


                    inicializaVariableMensaje();

                    mensajeResultado.resultado = datos;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                }



                return mensajeResultado;
            }

        }

        public Mensaje CrearListadoEmpleadosCFDI(List<DatosParaTimbrar> datosParaTimbrados)
        {
            try
            {
                int i;
                List<Object[]> listCFDIyEmpleados = new List<object[]>();
                List<CFDIEmpleado> listCFDIEmpleado;
                List<CFDIEmpleado> listCFDIGuardar = new List<CFDIEmpleado>();
                List<string> clavesEmpleado = new List<string>();

                if (datosParaTimbrados != null)
                {
                    if (datosParaTimbrados.Count() > 0)
                    {
                        ConstructorDatosATimbrar cdat = new ConstructorDatosATimbrar(mapConceptosSAT, mapConceptosIncapacidades, mapConceptosHrsExtras, nombreCompletoPeriodicidad);

                        mensajeResultado = cdat.validarDatosRazonSocial(razonesSocialesActual);
                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }
                        else
                        {

                            listCFDIEmpleado = cdat.generaDatosATimbrar(datosParaTimbrados, periodosNomina, tipoNomina, tipoCorrida, DateTime.Now, razonesSocialesActual);
                            listCFDIEmpleado = listCFDIEmpleado == null ? new List<CFDIEmpleado>() : listCFDIEmpleado;
                            if (listCFDIEmpleado.Count() > 0)
                            {
                                for (i = 0; i < listCFDIEmpleado.Count(); i++)
                                {
                                    if (listCFDIEmpleado[i].statusGeneraInfo)
                                    {
                                        listCFDIGuardar.Add(listCFDIEmpleado[i]);
                                        clavesEmpleado.Add(listCFDIEmpleado[i].plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave);
                                    }

                                }
                                RazonesSociales razonSocial = razonesSocialesActual;
                                mensajeResultado = cfdiEmpleadoDAO.getLimpiaConStatusErrorOEnProceso(razonSocial.clave, tipoNomina.clave, tipoCorrida.clave, periodosNomina.id, clavesEmpleado, dbContSimple);

                                if (mensajeResultado.noError == 0)
                                {
                                    mensajeResultado.resultado = listCFDIGuardar;
                                    mensajeResultado.noError = 0;
                                    mensajeResultado.error = "";
                                    return mensajeResultado;
                                }


                            }
                            else
                            {
                                mensajeResultado.noError = 90;
                                mensajeResultado.error = "No hay informacion a generar";
                                mensajeResultado.resultado = null;
                            }
                        }

                    }
                }
                else
                {
                    mensajeResultado.noError = 70;
                    mensajeResultado.error = "No hay informacion a generar";
                    mensajeResultado.resultado = null;
                }

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_doInBackground()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;

        }

        public Mensaje SaveCFDIInformacion(List<CFDIEmpleado> listCFDIGuardar, object[] eliminados, DBContextAdapter dbContextSimple)
        {
            try
            {

                if (listCFDIGuardar != null)
                {
                    if (listCFDIGuardar.Count() > 0)
                    {
                        mensajeResultado = cfdiEmpleadoDAO.saveDeleteCFDIEmpleado(listCFDIGuardar, eliminados, 50, dbContextSimple);

                        if (mensajeResultado.noError != 0)
                        {
                            return mensajeResultado;
                        }



                    }
                }
                else
                {
                    if (error == 2)
                    {
                        //util.mensajeError(getView(), Utilerias.ObtenerMensaje.getString("GeneracionDatosTimbreMsgGeneraOcupado"));
                    }
                    else if (error == 3)
                    {
                        //util.mensajeError(rootPane, Utilerias.ObtenerMensaje.getString("SemaforoTimbradoPacError3"));  //timbrando
                    }
                    else if (error == 4)
                    {
                        //util.mensajeError(rootPane, Utilerias.ObtenerMensaje.getString("SemaforoTimbradoPacError4"));  //cancelando timbres
                    }
                    else if (error == 5)
                    {
                        //util.mensajeError(rootPane, Utilerias.ObtenerMensaje.getString("SemaforoTimbradoPacError5"));  //Periodo abierto
                    }
                    else if (error == 6)
                    {
                        //util.mensajeError(rootPane, Utilerias.ObtenerMensaje.getString("GeneracionDatosTimbreMsgPeriodoAbierto"));
                    }
                    else if (error == 50)
                    {  //Error de hibernate
                        //Utilerias.mensajeErrorHibernateStatic(mensajeDB.getNoError(), mensajeDB.getError());
                    }
                    else
                    {
                        //util.mensajeError(rootPane, Utilerias.ObtenerMensaje.getString("SemaforoTimbradoPacError1"));  //error realizar operacion
                    }
                }



            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_doInBackground()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public void inicializaValores()
        {
            List<ConfigConceptosSat> listConceptosSAT = new List<ConfigConceptosSat>();
            //validacionesParametros();
            mensajeResultado = getConfigConceptosSatAll();
            if (mensajeResultado.noError != 0)
            {
                return;
            }
            else
            {
                listConceptosSAT = (List<ConfigConceptosSat>)mensajeResultado.resultado;
                int i;
                String conceptoClave;
                mapConceptosSAT = new Dictionary<string, ConfigConceptosSat>();
                for (i = 0; i < listConceptosSAT.Count(); i++)
                {

                    conceptoClave = String.Concat(listConceptosSAT[i].concepNomDefi.clave, ManejadorEnum.GetDescription(listConceptosSAT[i].concepNomDefi.naturaleza)); //listConceptosSAT[i].concepNomDefi.clave.Concat(ManejadorEnum.GetDescription(listConceptosSAT[i].concepNomDefi.naturaleza));
                    mapConceptosSAT[conceptoClave] = listConceptosSAT[i];
                }

            }

            List<Excepciones> listExcepciones = new List<Excepciones>();
            mensajeResultado = getExcepcionesAll();
            if (mensajeResultado.noError != 0)
            {
                return;
            }
            else
            {
                listExcepciones = (List<Excepciones>)mensajeResultado.resultado;
                mapConceptosIncapacidades = new Dictionary<string, MovNomConcep>();
                mapConceptosHrsExtras = new Dictionary<string, MovNomConcep>();
                ConcepNomDefi cnd;
                for (int i = 0; i < listExcepciones.Count(); i++)
                {
                    if ((string.Equals(listExcepciones[i].clave, ClavesParametrosModulos.claveExcepcionIncapacidadPorAccidente.ToString(), StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(listExcepciones[i].clave, ClavesParametrosModulos.claveExcepcionIncapacidadPorEnfermedad.ToString(), StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(listExcepciones[i].clave, ClavesParametrosModulos.claveExcepcionIncapacidadPorMaternidad.ToString(), StringComparison.OrdinalIgnoreCase)) && listExcepciones[i].concepNomDefi != null)
                    {
                        cnd = listExcepciones[i].concepNomDefi;
                        mapConceptosIncapacidades[cnd.clave] = null;
                    }
                    else if ((string.Equals(listExcepciones[i].clave, ClavesParametrosModulos.claveExcepcionTiempoExtra.ToString(), StringComparison.OrdinalIgnoreCase) ||
                      string.Equals(listExcepciones[i].clave, ClavesParametrosModulos.claveExcepcionExtraDoble.ToString(), StringComparison.OrdinalIgnoreCase) ||
                      string.Equals(listExcepciones[i].clave, ClavesParametrosModulos.claveExcepcionExtraTriple.ToString(), StringComparison.OrdinalIgnoreCase)) && listExcepciones[i].concepNomDefi != null)
                    {
                        cnd = listExcepciones[i].concepNomDefi;
                        mapConceptosHrsExtras[cnd.clave] = null;
                    }
                }

            }


        }

        private void validacionesParametros()
        {
            bool manejarDepartamento = true, manejarCentroCostos = true;
            decimal[] valores = new decimal[]{
            (decimal) ClavesParametrosModulos.claveParametroPermiteDepartamento,
            (decimal) ClavesParametrosModulos.claveParametroPermiteCentroCostos,
            (decimal) ClavesParametrosModulos.claveParametroNombreCompletoDetalleRecibo,
            (decimal) ClavesParametrosModulos.claveParametroAñoActivoPeriodo};

            // fechaActualPeriodo.setTime(Calendar.getInstance().getTime());
            List<Object[]> listParametros = obtenerParametrosYListCruces((String)ClavesParametrosModulos.claveModuloGlobal, valores);
            for (int i = 0; i < listParametros.Count(); i++)
            {
                if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroPermiteDepartamento))
                {
                    manejarDepartamento = validadorParametros.getInstance().parametroPermiteDepartamento((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], razonesSocialesActual, usuarioActual);
                }
                else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroPermiteCentroCostos))
                {
                    manejarCentroCostos = validadorParametros.getInstance().parametroPermiteDepartamento((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], razonesSocialesActual, usuarioActual);
                }
                else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroNombreCompletoDetalleRecibo))
                {
                    nombreCompletoPeriodicidad = validadorParametros.getInstance().parametroNombreCompletoDetalleRecibo((Parametros)listParametros[i][0], (List<Cruce>)listParametros[i][1], razonesSocialesActual, usuarioActual);
                }
                else if (((Parametros)(listParametros[i])[0]).clave.Equals((decimal)ClavesParametrosModulos.claveParametroAñoActivoPeriodo))
                {
                    //fechaActualPeriodo.set(Calendar.YEAR, validadorParametros.getInstance().yearPeriodoActivo((Parametros)listParametros.get(i)[0], (List<Cruce>)listParametros.get(i)[1]));
                }
            }

        }

        public List<Object[]> obtenerParametrosYListCruces(String claveModulo, decimal[] clavesParametros)
        {
            List<Object[]> listParametrosYListCruce = new List<object[]>();
            List<Parametros> listparametros;

            try
            {
                listparametros = (from p in dbConttMaster.context.Set<Parametros>()
                                  where p.modulo.clave == claveModulo && clavesParametros.Contains(p.clave)
                                  select p).ToList();

                if (listparametros.Any())
                {
                    for (int i = 0; i < listparametros.Count(); i++)
                    {
                        List<Cruce> values;//Si el parametro no tiene seleccionado elementos de aplicacion quiere decir que no se va filtrar o profuncidar por algun elemento de aplicacion
                        if (listparametros[i].elementosAplicacion == null ? false : listparametros[i].elementosAplicacion.Any())
                        {
                            decimal clave = listparametros[i].clave;
                            List<decimal> valore = new List<decimal>();
                            for (int j = 0; j < listparametros[i].elementosAplicacion.Count(); j++)
                            {
                                valore.Add(listparametros[i].elementosAplicacion[j].id);
                            }
                            values = (from c in dbConttMaster.context.Set<Cruce>()
                                      where c.parametros.clave == clave && valore.Contains(c.elementosAplicacion.id)
                                      orderby c.elementosAplicacion.ordenId descending
                                      select c).ToList();
                        }
                        else
                        {
                            values = new List<Cruce>();
                        }
                        Object[] objects = new Object[2];
                        objects[0] = listparametros[i];
                        objects[1] = values;
                        listParametrosYListCruce.Add(objects);
                        values = null;
                    }
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getConfigConceptosSatAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                listParametrosYListCruce = null;
            }


            return listParametrosYListCruce;
        }

        public Mensaje getConfigConceptosSatAll()
        {
            List<ConfigConceptosSat> listConceptosSAT2 = new List<ConfigConceptosSat>();
            try
            {
                inicializaVariableMensaje();
                listConceptosSAT2 = (from csat in dbContSimple.context.Set<ConfigConceptosSat>() select csat).ToList();

                mensajeResultado.resultado = listConceptosSAT2;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getConfigConceptosSatAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        public Mensaje getExcepcionesAll()
        {
            List<Excepciones> excepciones = new List<Excepciones>();
            try
            {
                inicializaVariableMensaje();

                excepciones = (from ex in dbContSimple.context.Set<Excepciones>() select ex).ToList();

                mensajeResultado.resultado = excepciones;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getExcepcionesAll()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        public List<Object> obtenerValoresFiltrado()
        {
            List<Object> wh = new List<Object>();

            if (idRazSocial != null)
            {
                RazonesSociales tipoRazonSocial = (RazonesSociales)(getRazonSocialActual()).resultado;
                wh.Add(tipoRazonSocial);

            }


            if (idTipoCorr != null)
            {
                TipoCorrida tipoCorrida = (TipoCorrida)(getTipoCorrida()).resultado;
                wh.Add(tipoCorrida);
            }

            if (idTipoNom != null)
            {
                TipoNomina tipoNomina = getTipoNomina();
                wh.Add(tipoNomina);
            }

            if (idPeriodoNom != null)
            {
                PeriodosNomina periodoNomina = getPeriodosNomina();
                wh.Add(periodoNomina);
            }

            mensajeResultado = getRazonSocialActual();

            if (idRegPatr != null)
            {
                RegistroPatronal registroPatronal = getRegistroPatronal(idRegPatr, razonesSocialesActual.id);
                wh.Add(registroPatronal);
            }



            if (idDepart != null)
            {
                Departamentos departamentos = getDepartamentos(idDepart, razonesSocialesActual.id);
                wh.Add(departamentos);
            }




            if (idCenCost != null)
            {
                CentroDeCosto centroDeCosto = getCentroDeCostos(idCenCost, razonesSocialesActual.id);
                wh.Add(centroDeCosto);
            }

            if (listIdEmpl != null)
            {

                for (int i = 0; i < listIdEmpl.Count(); i++)
                {
                    var idem = Convert.ToDecimal(listIdEmpl[i]);
                    Empleados empleado = getEmpleadoId(idem, razonesSocialesActual.id);
                    wh.Add(new Empleados());
                    wh.Add(empleado);
                }                 //--
            }
            else
            {
                if (idEmpleadoIni != null && idEmpleadoFin != null)
                {

                    if (idEmpleadoIni == idEmpleadoFin)
                    {
                        Empleados empleado = getEmpleadoId(idEmpleadoIni, razonesSocialesActual.id);
                        wh.Add(empleado);
                        wh.Add(empleado);
                    }
                    else
                    {
                        Empleados empleadoIni = getEmpleadoId(idEmpleadoIni, razonesSocialesActual.id);
                        Empleados empleadoFin = getEmpleadoId(idEmpleadoFin, razonesSocialesActual.id);
                        wh.Add(empleadoIni);
                        wh.Add(empleadoFin);

                    }

                }
                else if (idEmpleadoFin != null)
                {
                    Empleados empleado = getEmpleadoId(idEmpleadoFin, razonesSocialesActual.id);
                    wh.Add(new Empleados());
                    wh.Add(empleado);
                }
                else if (idEmpleadoIni != null)
                {
                    Empleados empleado = getEmpleadoId(idEmpleadoIni, razonesSocialesActual.id); wh.Add(empleado);
                    wh.Add(new Empleados());
                }
            }



            return wh;

        }

        public Mensaje getStatusPeriodoNomina()
        {
            try
            {
                inicializaVariableMensaje();
                mensajeResultado = perDAO.getStatusPeriodo(idPeriodoNom.GetValueOrDefault(), dbContSimple);

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getStatusPeriodoNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                //getSession().Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }

        public Mensaje getTipoCorrida()
        {
            // TipoCorrida tipoCorrida = new TipoCorrida();
            try
            {


                inicializaVariableMensaje();
                tipoCorrida = (from t in dbContSimple.context.Set<TipoCorrida>()
                               where t.id == idTipoCorr
                               select t).Single();
                mensajeResultado.resultado = tipoCorrida;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getTipoCorrida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        public TipoNomina getTipoNomina()
        {
            //TipoNomina tipoNomina = new TipoNomina();
            try
            {
                tipoNomina = (from tn in dbContSimple.context.Set<TipoNomina>()
                              where tn.id == idTipoNom
                              select tn).SingleOrDefault();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getTipoNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                tipoNomina = null;
            }


            return tipoNomina;

        }

        public PeriodosNomina getPeriodosNomina()
        {
            // PeriodosNomina periodosNomina = new PeriodosNomina();
            try
            {
                periodosNomina = (from per in dbContSimple.context.Set<PeriodosNomina>()
                                  where per.id == idPeriodoNom
                                  select per).SingleOrDefault();


            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getTipoNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                periodosNomina = null;
            }


            return periodosNomina;

        }

        public Mensaje getRazonSocialActual()
        {

            try
            {

                claveRazSoc = "";
                inicializaVariableMensaje();
                razonesSocialesActual = (from t in dbContSimple.context.Set<RazonesSociales>()
                                         where t.id == idRazSocial
                                         select t).Single();
                claveRazSoc = razonesSocialesActual.clave;
                mensajeResultado.resultado = razonesSocialesActual;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getRazonSocialActual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        public Departamentos getDepartamentos(decimal? idDepartam, decimal? idRazon)
        {

            try
            {



                departamento = (from d in dbContSimple.context.Set<Departamentos>()
                                where d.id == idDepartam && d.razonesSociales_ID == idRazon
                                select d).SingleOrDefault();


            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getDepartamentos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return departamento;

        }

        public CentroDeCosto getCentroDeCostos(decimal? idCentCost, decimal? idRazon)
        { //*
            try
            {



                centroDeCostos = (from c in dbContSimple.context.Set<CentroDeCosto>()
                                  where c.id == idCentCost && c.razonesSociales_ID == idRazon
                                  select c).SingleOrDefault();


            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getCentroDeCostos()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return centroDeCostos;
        }

        public RegistroPatronal getRegistroPatronal(decimal? idRegisP, decimal? idRazon)
        {
            try
            {



                registroPatronal = (from r in dbContSimple.context.Set<RegistroPatronal>()
                                    where r.id == idRegisP && r.razonesSociales_ID == idRazon
                                    select r).SingleOrDefault();


            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getRegistroPatronal()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return registroPatronal;
        }

        public Empleados getEmpleadoId(decimal? idEmpl, decimal? idRazon)
        {
            try
            {



                empleado = (from e in dbContSimple.context.Set<Empleados>()
                            where e.id == idEmpl && e.razonesSociales_ID == idRazon
                            select e).SingleOrDefault();


            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getEmpleadoId()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return empleado;
        }

        public Mensaje getUsuarioActual()
        {

            try
            {


                inicializaVariableMensaje();
                usuarioActual = (from t in dbConttMaster.context.Set<Usuario>()
                                 where t.id == 1
                                 select t).Single();
                mensajeResultado.resultado = usuarioActual;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getRazonSocialActual()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        public Mensaje getPorIdRazonesSociales(decimal? id, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                var razonSocial = (from rs in getSession().Set<RazonesSociales>()
                                   where rs.id == id
                                   select new
                                   {
                                       rs.clave,
                                       rs.id
                                   }).SingleOrDefault();
                mensajeResultado.resultado = razonSocial;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getPorIdRazonesSociales()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }


        //metodos para hibernate
        public List<Object> obtenerValoresFiltradoHir()
        {
            List<Object> wh = new List<Object>();

            if (idTipoCorr != null)
            {
                Hibernate.entidad.TipoCorrida tipoCorrida = (Hibernate.entidad.TipoCorrida)(getTipoCorridaH()).resultado;
                wh.Add(tipoCorrida);
            }

            if (idTipoNom != null)
            {
                Hibernate.entidad.TipoNomina tipoNomina = getTipoNominaH();
                wh.Add(tipoNomina);
            }

            if (idPeriodoNom != null)
            {
                Hibernate.entidad.PeriodosNomina periodoNomina = getPeriodosNominaH();
                wh.Add(periodoNomina);
            }


            if (idRegPatr != null)
            {
                RegistroPatronal registroPatronal = null;
                wh.Add(registroPatronal);
            }

            if (idCenCost != null)
            {
                CentroDeCosto centroDeCosto = null;
                wh.Add(centroDeCosto);
            }

            if (idDepart != null)
            {
                Departamentos departamentos = null;
                wh.Add(departamentos);
            }

            if (idEmpleadoIni != null && idEmpleadoFin != null)
            {

                if (idEmpleadoIni == idEmpleadoFin)
                {
                    Empleados empleado = null;
                    wh.Add(empleado);
                    wh.Add(empleado);
                }
                else
                {
                    Empleados empleadoIni = null;
                    Empleados empleadoFin = null;
                    wh.Add(empleadoIni);
                    wh.Add(empleadoFin);

                }

            }
            else if (idEmpleadoFin != null)
            {
                Empleados empleado = null;
                wh.Add(new Empleados());
                wh.Add(empleado);
            }
            else if (idEmpleadoIni != null)
            {
                Empleados empleado = null;
                wh.Add(empleado);
                wh.Add(new Empleados());
            }

            return wh;

        }
        public Hibernate.entidad.PeriodosNomina getPeriodosNominaH()
        {
            Hibernate.entidad.PeriodosNomina periodosNomina = new Hibernate.entidad.PeriodosNomina();
            try
            {
                IQuery query;
                inicializaVariableMensaje();
                string querystr = "From PeriodosNomina  tc where tc.id=:id";
                query = sessionSimple1.CreateQuery(querystr);
                query.SetParameter("id", idPeriodoNom);
                query.SetMaxResults(1);
                periodosNomina = (Hibernate.entidad.PeriodosNomina)query.UniqueResult();
                mensajeResultado.resultado = periodosNomina;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;


            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getTipoNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                periodosNomina = null;
            }


            return periodosNomina;

        }

        public Hibernate.entidad.TipoNomina getTipoNominaH()
        {
            Hibernate.entidad.TipoNomina tipoNomina = new Hibernate.entidad.TipoNomina();
            try
            {
                IQuery query;
                inicializaVariableMensaje();
                string querystr = "From TipoNomina  tc where tc.id=:id";
                query = sessionSimple1.CreateQuery(querystr);
                query.SetParameter("id", idTipoNom);
                query.SetMaxResults(1);
                tipoNomina = (Hibernate.entidad.TipoNomina)query.UniqueResult();
                mensajeResultado.resultado = tipoNomina;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getTipoNomina()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                tipoNomina = null;
            }


            return tipoNomina;

        }

        public Mensaje getTipoCorridaH()
        {
            Hibernate.entidad.TipoCorrida tipoCorrida = new Hibernate.entidad.TipoCorrida();
            try
            {
                IQuery query;
                inicializaVariableMensaje();
                string querystr = "From TipoCorrida  tc where tc.id=:id";
                query = sessionSimple1.CreateQuery(querystr);
                query.SetParameter("id", idTipoCorr);
                query.SetMaxResults(1);
                tipoCorrida = (Hibernate.entidad.TipoCorrida)query.UniqueResult();
                mensajeResultado.resultado = tipoCorrida;
                mensajeResultado.error = "";
                mensajeResultado.noError = 0;

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getTipoCorrida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mensajeResultado;
        }

        public Mensaje generaTimbrado(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal, decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, string ruta, List<decimal?> listIdEmpleados, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster)
        {
            int numError = 0;
            string mensajeError = "";
            idTipoCorr = idTipoCorrida;
            idTipoNom = idTipoNomina;
            idPeriodoNom = idPeriodoNomina;
            idRegPatr = idRegPatronal;
            idCenCost = idCenCosto;
            idDepart = idDepartamento;
            idEmpleadoIni = idEmplIni;
            idEmpleadoFin = idEmplFin;
            idRazSocial = idRazonSocial;
            listIdEmpl = listIdEmpleados;

            dbContSimple = dbContextSimple;
            dbConttMaster = dbContextMaster;
            mensajeResultado = getRazonSocialActual();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar RazonSocial";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            mensajeResultado = getUsuarioActual();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar Usuario";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            inicializaValores();
            mensajeResultado = getStatusPeriodoNomina();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar periodo";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }

            bool periodoOpen = (bool)mensajeResultado.resultado;
            if (periodoOpen)
            {
                mensajeResultado.error = "periodo esta abierto";
                mensajeResultado.noError = 6;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            bool certVigente = existeCertificadoVigente(dbContextSimple);
            if (!certVigente)
            {
                mensajeResultado.error = "no existe certificado vigente";
                mensajeResultado.noError = 10;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }

            List<Object> wh = obtenerValoresFiltrado();
            wh.Add(StatusTimbrado.EN_PROCESO);
            wh.Add(StatusTimbrado.ERROR);
            List<CFDIEmpleado> listCFDIEmpleadoAux = null;
            mensajeResultado = cfdiEmpleadoDAO.buscaCFDIEmpleadosFiltrado(wh, listIdEmpl, dbContSimple);
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error al consultar cfdiEmpleados";
                mensajeResultado.noError = 6;
                mensajeResultado.resultado = null;
                return mensajeResultado;
            }
            else
            {
                listCFDIEmpleadoAux = (List<CFDIEmpleado>)mensajeResultado.resultado;

                listCFDIEmpleadoAux = addDatosTimbradoTabla(listCFDIEmpleadoAux, tipoNomina, razonesSocialesActual);
                for (int i = 0; i < listCFDIEmpleadoAux.Count; i++)
                {
                    listCFDIEmpleadoAux[i].statusGeneraInfo = true;
                }
            }
            Dictionary<string, string> mapClaveConceptosISR = crearMapsConceptoISr();
            string path = ruta;//ConfigurationManager.AppSettings["routeAccesXslt"];//@"D:\FuentesNomina\NomRH\Exitosw.Payroll.Core\";
            string pathxsl = path + @"\recursos\xsltLocal\cadenaoriginal_3_3.xslt";
            System.Xml.Xsl.XslCompiledTransform transformerCadenaOriginal = new System.Xml.Xsl.XslCompiledTransform(true);
            transformerCadenaOriginal.Load(pathxsl);



            List<CFDIEmpleado> addFolioCfdiEmpleado = new List<CFDIEmpleado>();
            List<CFDIEmpleado> listCFDIEmpleado = new List<CFDIEmpleado>();

            for (int i = 0; i < listCFDIEmpleadoAux.Count; i++)
            {
                if (listCFDIEmpleadoAux[i].statusGeneraInfo)
                {
                    listCFDIEmpleado.Add(listCFDIEmpleadoAux[i]);
                }
                else if (listCFDIEmpleadoAux[i].asignoFolio)
                {
                    addFolioCfdiEmpleado.Add(listCFDIEmpleadoAux[i]);
                }
            }

            if (listCFDIEmpleado.Count > 0)
            {
                decimal[] idsTimbrados = new decimal[listCFDIEmpleado.Count()];
                for (int j = 0; j < listCFDIEmpleado.Count; j++)
                {
                    idsTimbrados[j] = listCFDIEmpleado[j].id;
                }

                mensajeResultado = cfdiEmpleadoDAO.getCFDIEmpleadoTimbrados(idsTimbrados, dbContextSimple);
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.error = "error al consultar cfdiEmpleados";
                    mensajeResultado.noError = 6;
                    mensajeResultado.resultado = null;
                    return mensajeResultado;
                }
                else
                {
                    List<CFDIEmpleado> cfdiTimbrados = mensajeResultado.resultado == null ? new List<CFDIEmpleado>() : (List<CFDIEmpleado>)mensajeResultado.resultado;
                    if (cfdiTimbrados.Any())
                    {
                        for (int j = 0; j < listCFDIEmpleado.Count(); j++)
                        {
                            for (int k = 0; k < cfdiTimbrados.Count(); k++)
                            {
                                if (listCFDIEmpleado[j].id.Equals(cfdiTimbrados[k].id))
                                {
                                    listCFDIEmpleado.Insert(j, cfdiTimbrados[k]);
                                    // listCFDIEmpleado.set(j, cfdiTimbrados.get(k));
                                    cfdiTimbrados.RemoveAt(k);
                                    //cfdiTimbrados.remove(k);
                                    break;
                                }
                            }
                            if (!cfdiTimbrados.Any())
                            {
                                break;
                            }
                        }
                    }
                }


                ConstruyeComprobanteV33 comprobanteV33 = new ConstruyeComprobanteV33(mapClaveConceptosISR, ruta);
                comprobanteV33.setServicioCFDIEmpleado(cfdiEmpleadoDAO);
                comprobanteV33.setservicioCertificado(certificadoDAO);
                ConfiguraTimbrado configuraTimbrado = razonesSocialesActual.configuraTimbrado;
                bool valido = true;
                if (configuraTimbrado == null)
                {
                    valido = false;
                }
                else
                {
                    if (configuraTimbrado.contraseña == null ? true : configuraTimbrado.contraseña.Trim().Length == 0)
                    {
                        valido = false;
                    }
                    else if (configuraTimbrado.URL == null ? true : configuraTimbrado.URL.Trim().Length == 0)
                    {
                        valido = false;
                    }
                    else if (configuraTimbrado.usuario == null ? true : configuraTimbrado.usuario.Trim().Length == 0)
                    {
                        valido = false;
                    }
                }

                if (valido)
                {
                    listCFDIEmpleado = comprobanteV33.generaComprobanteV33(tipoNomina, periodosNomina, listCFDIEmpleado, configuraTimbrado, transformerCadenaOriginal, dbContextSimple);
                    if (comprobanteV33.mensaje.noError > 0)
                    {
                        numError = comprobanteV33.mensaje.noError;
                        mensajeError = comprobanteV33.mensaje.error;
                    }
                }
                else
                {
                    numError = 2;
                }

                if (listCFDIEmpleado.Count > 0)
                {
                    List<CFDIEmpleado> listCFDIGuardar = new List<CFDIEmpleado>();
                    int i = 0;
                    if (addFolioCfdiEmpleado != null && addFolioCfdiEmpleado.Any())
                    {
                        listCFDIGuardar.AddRange(addFolioCfdiEmpleado);
                        addFolioCfdiEmpleado = null;
                    }
                    while (i < listCFDIEmpleado.Count)
                    {
                        listCFDIEmpleado[i].statusGeneraInfo = false;
                        if (listCFDIEmpleado[i].cfdiRecibo.statusTimbrado == StatusTimbrado.EN_PROCESO | listCFDIEmpleado[i].asignoFolio)
                        {
                            if (listCFDIEmpleado[i].recienTimbrado)
                            {
                                listCFDIGuardar.Add(listCFDIEmpleado[i]);
                                //listCFDIEmpleado.RemoveAt(i);
                                i++;
                            }
                            else
                            {
                                i++;
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }

                    if (listCFDIGuardar.Count > 0)
                    {
                        mensajeResultado = cfdiEmpleadoDAO.saveDeleteCFDIEmpleado(listCFDIGuardar, null, 50, dbContextSimple);
                        if (mensajeResultado.noError > 0)
                        {
                            //pendiente
                        }
                        // listCFDIEmpleado.AddRange(listCFDIGuardar);
                    }

                    //aqui

                    if (mensajeResultado.noError == 0)
                    {
                        List<CFDIEmpleado> cfdiEmpleado = listCFDIEmpleado;

                        var datos = cfdiEmpleado.Select(d => new
                        {
                            d.id,
                            d.CLABE,
                            d.CURP,
                            d.RFC,
                            d.antiguedad,
                            d.antiguedadYMD,
                            d.apellidoMaterno,
                            d.apellidoPaterno,
                            d.calle,
                            d.fechaFinalPago,
                            d.fechaInicioPago,
                            d.fechaInicioRelLaboral,
                            d.fechaPago,
                            d.formaPago,

                            d.nombre,
                            d.mensaje,

                            tipoNomina_ID = d.tipoNomina.id,
                            tipoCorrida_ID = d.tipoCorrida.id,
                            razonesSociales_ID = d.razonesSociales.id,
                            periodosNomina_ID = d.periodosNomina.id,
                            plazaPorEmpleadoMov_ID = d.plazasPorEmpleadosMov.id,
                            d.statusGeneraInfo,
                            d.asignoFolio,
                            d.recienTimbrado,
                            clave = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                            d.cfdiRecibo_ID,
                            cfdiRecibo = new
                            {
                                d.cfdiRecibo.id,
                                d.cfdiRecibo.UUID,
                                d.cfdiRecibo.serie,
                                d.cfdiRecibo.acuse,
                                d.cfdiRecibo.UUIDRelacionado,
                                d.cfdiRecibo.cadenaCertificado,
                                d.cfdiRecibo.cadenaOriginalTimbrado,
                                d.cfdiRecibo.fechaGeneraInfo,
                                d.cfdiRecibo.fechaHoraTimCancelado,
                                d.cfdiRecibo.fechaEmision,
                                d.cfdiRecibo.folioCFDI,
                                d.cfdiRecibo.leyenda,
                                d.cfdiRecibo.motivoCancelacion,
                                d.cfdiRecibo.noCertificado,
                                d.cfdiRecibo.noCertificadoSAT,
                                d.cfdiRecibo.rfcProvCertif,
                                d.cfdiRecibo.sello,
                                d.cfdiRecibo.selloCFD,
                                d.cfdiRecibo.selloSAT,
                                d.cfdiRecibo.serieCFDI,
                                d.cfdiRecibo.statusTimbrado,
                                d.cfdiRecibo.statusXmlSat,
                                d.cfdiRecibo.total,
                                d.cfdiRecibo.version,
                                d.cfdiRecibo.xmlTimbrado,
                                d.cfdiRecibo.certificadoTimbrado,
                                d.cfdiRecibo.fechaHoraTimbrado,
                                d.cfdiRecibo.selloTimbrado,
                                d.cfdiRecibo.mensajeRec,
                                /* cfdiReciboConcepto = d.cfdiRecibo.cfdiReciboConcepto.Select(r => new
                                 {
                                     r.id,
                                     r.claveConcepto,
                                     r.claveSAT,
                                     r.descripcionConcepto,
                                     r.importeExento,
                                     r.importeGravable,
                                     r.otroPago,
                                     r.tipoNaturaleza
                                 }).ToList(),
                                 cfdiReciboHrsExtras = d.cfdiRecibo.cfdiReciboHrsExtras.Select(h => new
                                 {
                                     h.id,
                                     h.dias,
                                     h.horasExtras,
                                     h.importeExento,
                                     h.importeGravable
                                 }).ToList(),
                                 cfdiReciboIncapacidad = d.cfdiRecibo.cfdiReciboIncapacidad.Select(g => new
                                 {
                                     g.id,
                                     g.diasIncapacidad,
                                     g.importeMonetario,
                                     g.tipoIncapacidad
                                 }).ToList()*/
                            }
                        }).ToList();



                        //inicializaVariableMensaje();

                        mensajeResultado.resultado = datos;
                        mensajeResultado.noError = numError;
                        mensajeResultado.error = mensajeError;
                    }



                }

            }



            return mensajeResultado;
        }

        public Dictionary<string, string> crearMapsConceptoISr()
        {
            Dictionary<string, string> mapClaveConceptosISR = null;
            List<ConcepNomDefi> listConcepNomDefi = null;
            try
            {
                if (tipoCorrida != null)
                {
                    listConcepNomDefi = (from cdn in dbContSimple.context.Set<ConcepNomDefi>()
                                         from ctc in dbContSimple.context.Set<ConceptoPorTipoCorrida>()
                                         join tc in dbContSimple.context.Set<TipoCorrida>() on ctc.tipoCorrida_ID equals tc.id into join_tc
                                         from tc in join_tc.DefaultIfEmpty()
                                         where ctc.concepNomDefi_ID == cdn.id && tc.clave == tipoCorrida.clave
                                         && cdn.activado == true && cdn.formulaConcepto.Contains("CalculoISR")
                                         && cdn.fecha == ((from c in dbContSimple.context.Set<ConcepNomDefi>()
                                                           where c.formulaConcepto.Contains("CalculoISR")
                                                           select new { c.fecha }
                                                         ).Max(p => p.fecha))
                                         select cdn).ToList();
                }
                else
                {

                    listConcepNomDefi = (from cdn in dbContSimple.context.Set<ConcepNomDefi>()
                                         from ctc in dbContSimple.context.Set<ConceptoPorTipoCorrida>()
                                         join tc in dbContSimple.context.Set<TipoCorrida>() on ctc.tipoCorrida_ID equals tc.id into join_tc
                                         from tc in join_tc.DefaultIfEmpty()
                                         where ctc.concepNomDefi_ID == cdn.id
                                         && cdn.activado == true && cdn.formulaConcepto.Contains("CalculoISR")
                                         && cdn.fecha == ((from c in dbContSimple.context.Set<ConcepNomDefi>()
                                                           where c.formulaConcepto.Contains("CalculoISR")
                                                           select new { c.fecha }
                                                         ).Max(p => p.fecha))
                                         select cdn).ToList();
                }

                if (listConcepNomDefi != null)
                {
                    mapClaveConceptosISR = new Dictionary<string, string>();
                    for (int i = 0; i < listConcepNomDefi.Count; i++)
                    {
                        mapClaveConceptosISR[listConcepNomDefi[i].clave] = listConcepNomDefi[i].clave;
                    }
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("generacionDatosTimbre_getTipoCorrida()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }

            return mapClaveConceptosISR;

        }

        public Mensaje buscarTimbres(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal, decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster)
        {
            idTipoCorr = idTipoCorrida;
            idTipoNom = idTipoNomina;
            idPeriodoNom = idPeriodoNomina;
            idRegPatr = idRegPatronal;
            idCenCost = idCenCosto;
            idDepart = idDepartamento;
            idEmpleadoIni = idEmplIni;
            idEmpleadoFin = idEmplFin;
            idRazSocial = idRazonSocial;
            dbContSimple = dbContextSimple;
            dbConttMaster = dbContextMaster;
            mensajeResultado = getRazonSocialActual();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar RazonSocial";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            mensajeResultado = getUsuarioActual();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar Usuario";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            inicializaValores();
            mensajeResultado = getStatusPeriodoNomina();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar periodo";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }

            bool periodoOpen = (bool)mensajeResultado.resultado;
            if (periodoOpen)
            {
                mensajeResultado.error = "periodo esta abierto";
                mensajeResultado.noError = 6;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            bool certVigente = existeCertificadoVigente(dbContextSimple);
            if (!certVigente)
            {
                mensajeResultado.error = "no existe certificado vigente";
                mensajeResultado.noError = 10;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }

            List<Object> wh = obtenerValoresFiltrado();
            wh.Add(StatusTimbrado.EN_PROCESO);
            wh.Add(StatusTimbrado.ERROR);

            mensajeResultado = cfdiEmpleadoDAO.buscaCFDIEmpleadosFiltrado(wh, null, dbContSimple);



            if (mensajeResultado.noError != 0)
            {

                return mensajeResultado;
            }
            else

            {
                if (mensajeResultado.resultado != null)
                {
                    List<CFDIEmpleado> cfdiEmpleado = (List<CFDIEmpleado>)mensajeResultado.resultado;

                    inicializaVariableMensaje();

                    //mensajeResultado = SaveCFDIInformacion(cfdiEmpleado);


                    var datos = cfdiEmpleado.Select(d => new
                    {
                        d.id,
                        d.CLABE,
                        d.CURP,
                        d.RFC,
                        d.antiguedad,
                        d.antiguedadYMD,
                        d.apellidoMaterno,
                        d.apellidoPaterno,
                        d.calle,
                        d.ciudad,
                        d.claveBancoSat,
                        d.codigoPostal,
                        d.colonia,
                        d.correoElectronico,
                        d.cuentaBancaria,
                        d.departamento,
                        d.estado,
                        d.fechaFinalPago,
                        d.fechaInicioPago,
                        d.fechaInicioRelLaboral,
                        d.fechaPago,
                        d.formaPago,
                        d.jornada,
                        d.municipio,
                        d.noExterior,
                        d.noInterior,
                        d.noRegistroPatronal,
                        d.noSeguroSocial,
                        d.nombre,
                        d.numeroDiasPago,
                        d.pais,
                        d.periodiciadadPago,
                        d.puesto,
                        d.regimenContratacion,
                        d.riesgoPuesto,
                        d.salBaseCotAport,
                        d.salIntIMSS,
                        d.tipoContrato,
                        tipoNomina_ID = d.tipoNomina.id,
                        tipoCorrida_ID = d.tipoCorrida.id,
                        razonesSociales_ID = d.razonesSociales.id,
                        periodosNomina_ID = d.periodosNomina.id,
                        plazaPorEmpleadoMov_ID = d.plazasPorEmpleadosMov.id,
                        d.statusGeneraInfo,
                        d.asignoFolio,
                        d.recienTimbrado,
                        clave = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                        d.cfdiRecibo_ID,
                        cfdiRecibo = new
                        {
                            d.cfdiRecibo.id,
                            d.cfdiRecibo.UUID,
                            d.cfdiRecibo.serie,
                            d.cfdiRecibo.acuse,
                            d.cfdiRecibo.UUIDRelacionado,
                            d.cfdiRecibo.cadenaCertificado,
                            d.cfdiRecibo.cadenaOriginalTimbrado,
                            d.cfdiRecibo.fechaGeneraInfo,
                            d.cfdiRecibo.fechaHoraTimCancelado,
                            d.cfdiRecibo.fechaEmision,
                            d.cfdiRecibo.folioCFDI,
                            d.cfdiRecibo.leyenda,
                            d.cfdiRecibo.motivoCancelacion,
                            d.cfdiRecibo.noCertificado,
                            d.cfdiRecibo.noCertificadoSAT,
                            d.cfdiRecibo.rfcProvCertif,
                            d.cfdiRecibo.sello,
                            d.cfdiRecibo.selloCFD,
                            d.cfdiRecibo.selloSAT,
                            d.cfdiRecibo.serieCFDI,
                            d.cfdiRecibo.statusTimbrado,
                            d.cfdiRecibo.statusXmlSat,
                            d.cfdiRecibo.total,
                            d.cfdiRecibo.version,
                            d.cfdiRecibo.xmlTimbrado,
                            d.cfdiRecibo.certificadoTimbrado,
                            d.cfdiRecibo.fechaHoraTimbrado,
                            d.cfdiRecibo.selloTimbrado,
                            d.cfdiRecibo.mensajeRec,
                            cfdiReciboConcepto = d.cfdiRecibo.cfdiReciboConcepto.Select(r => new
                            {
                                r.id,
                                r.claveConcepto,
                                r.claveSAT,
                                r.descripcionConcepto,
                                r.importeExento,
                                r.importeGravable,
                                r.otroPago,
                                r.tipoNaturaleza
                            }).ToList(),
                            cfdiReciboHrsExtras = d.cfdiRecibo.cfdiReciboHrsExtras.Select(h => new
                            {
                                h.id,
                                h.dias,
                                h.horasExtras,
                                h.importeExento,
                                h.importeGravable
                            }).ToList(),
                            cfdiReciboIncapacidad = d.cfdiRecibo.cfdiReciboIncapacidad.Select(i => new
                            {
                                i.id,
                                i.diasIncapacidad,
                                i.importeMonetario,
                                i.tipoIncapacidad
                            }).ToList()
                        }
                    }).ToList();


                    inicializaVariableMensaje();

                    mensajeResultado.resultado = datos;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                }
            }

            return mensajeResultado;
        }

        private List<CFDIEmpleado> addDatosTimbradoTabla(List<CFDIEmpleado> listCFDIEmpleados, TipoNomina nomina, RazonesSociales razonSocial)
        {
            listCFDIEmpleados = listCFDIEmpleados == null ? new List<CFDIEmpleado>() : listCFDIEmpleados;
            Object valor;
            if (listCFDIEmpleados.Count() > 0)
            {
                /* String serie = nomina.series.serie;

                 int folioIni = nomina.series.folioInicial;

                 int longitudFolio = nomina.series.longitudFolio;

                 if (serie == null ? true : serie.Trim().Length == 0)
                 {
                     serie = razonSocial.series.serie;
                     folioIni = razonSocial.series.folioInicial;
                     longitudFolio = razonSocial.series.longitudFolio;
                 }

                 if (serie == null ? true : serie.Trim().Length == 0)
                 {
                     valor = (from o in dbContSimple.context.Set<CFDIRecibo>()
                              select new { o.folioCFDI }).Max(p => p.folioCFDI);

                 }
                 else
                 {
                     valor = (from o in dbContSimple.context.Set<CFDIRecibo>()
                              where o.serieCFDI == serie
                              select new { o.folioCFDI }).Max(p => p.folioCFDI);
                 }
                 GeneradorClave generadorClave = new GeneradorClave();
                 String folio;
                 string mascara = "";
                 for (int k = 0; k < longitudFolio; k++)
                 {
                     mascara = mascara + "#";
                 }
                 if (valor == null)
                 {
                     folio = nomina.folio;
                     if (folio == null ? true : folio.Trim().Length == 0)
                     {
                         folio = razonSocial.folio;

                     }
                 }
                 else
                 {

                     folio = generadorClave.generaClaveMax(valor.ToString(), mascara);
                 }

                */

                String serieN = tipoNomina.series.serie;
                String folio;
                int folioIniN = tipoNomina.series.folioInicial;

                int longitudFolioN = tipoNomina.series.longitudFolio;
                int ultifolio = ultimofolio(serieN, folioIniN, longitudFolioN);

               /* for (int l = 0; l < cfdiEmpleado.Count; l++)
                {

                    cfdiEmpleado[l].cfdiRecibo.serie = serieN;
                    cfdiEmpleado[l].cfdiRecibo.folioCFDI = datoMascara(serieN, ultifolio, longitudFolioN);
                    ultifolio++;
                }*/

                int i;
                for (i = 0; i < listCFDIEmpleados.Count; i++)
                {
                    if (listCFDIEmpleados[i].cfdiRecibo.folioCFDI == null ? true : listCFDIEmpleados[i].cfdiRecibo.folioCFDI.Trim().Length == 0)
                    {
                        folio = (datoMascara(serieN, ultifolio, longitudFolioN));
                        if (folio == null ? false : folio.Trim().Length > 0)
                        {
                            listCFDIEmpleados[i].cfdiRecibo.folioCFDI = (folio);
                            listCFDIEmpleados[i].asignoFolio = (true);
                            folio = listCFDIEmpleados[i].cfdiRecibo.folioCFDI; // generadorClave.generaClaveMax(folio, mascara);
                        }
                        else
                        {
                            listCFDIEmpleados[i].asignoFolio = (false);
                        }
                    }
                    else
                    {
                        listCFDIEmpleados[i].asignoFolio = (false);
                    }
                    listCFDIEmpleados[i].cfdiRecibo.serieCFDI = (serieN);

                    ultifolio++;
                }

            }
            return listCFDIEmpleados;
        }

        public Mensaje cancelarTimbres(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal, decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, List<decimal?> listIdEmpleados, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster)
        {
            try
            {
                int numError = 0;
                idTipoCorr = idTipoCorrida;
                idTipoNom = idTipoNomina;
                idPeriodoNom = idPeriodoNomina;
                idRegPatr = idRegPatronal;
                idCenCost = idCenCosto;
                idDepart = idDepartamento;
                idEmpleadoIni = idEmplIni;
                idEmpleadoFin = idEmplFin;
                idRazSocial = idRazonSocial;
                dbContSimple = dbContextSimple;
                dbConttMaster = dbContextMaster;
                listIdEmpl = listIdEmpleados;
                mensajeResultado = getRazonSocialActual();
                if (mensajeResultado.noError > 0)
                {
                    mensajeResultado.error = "error consultar RazonSocial";
                    mensajeResultado.noError = 50;
                    mensajeResultado.resultado = null;

                    return mensajeResultado;
                }

                mensajeResultado = getUsuarioActual();
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.error = "error consultar Usuario";
                    mensajeResultado.noError = 50;
                    mensajeResultado.resultado = null;

                    return mensajeResultado;
                }

                mensajeResultado = getStatusPeriodoNomina();
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.error = "error consultar periodo";
                    mensajeResultado.noError = 50;
                    mensajeResultado.resultado = null;

                    return mensajeResultado;
                }

                bool periodoOpen = (bool)mensajeResultado.resultado;
                if (periodoOpen)
                {
                    mensajeResultado.error = "periodo esta abierto";
                    mensajeResultado.noError = 6;
                    mensajeResultado.resultado = null;

                    return mensajeResultado;
                }
                bool certVigente = existeCertificadoVigente(dbContextSimple);
                if (!certVigente)
                {
                    mensajeResultado.error = "no existe certificado vigente";
                    mensajeResultado.noError = 10;
                    mensajeResultado.resultado = null;

                    return mensajeResultado;
                }

                List<Object> wh = obtenerValoresFiltrado();
                wh.Add(StatusTimbrado.TIMBRADO);

                //List<CFDIEmpleado> listCFDIEmpleadoAux = null;
                mensajeResultado = cfdiEmpleadoDAO.buscaCFDIEmpleadosFiltrado(wh, listIdEmpl, dbContSimple);
                if (mensajeResultado.noError > 0)
                {
                    mensajeResultado.error = "error al consultar cfdiEmpleados";
                    mensajeResultado.noError = 6;
                    mensajeResultado.resultado = null;
                    return mensajeResultado;
                }
                else
                {
                    List<CFDIEmpleado> listCFDIEmpleados = null;
                    listCFDIEmpleados = (List<CFDIEmpleado>)mensajeResultado.resultado;
                    listCFDIEmpleados = listCFDIEmpleados == null ? new List<CFDIEmpleado>() : listCFDIEmpleados;
                    InfoATimbrar infoATimbrar = new InfoATimbrar();
                    byte[] archivoPfx;
                    byte[] archivoKey;
                    if (razonesSocialesActual.certificadoSAT == null)
                    {
                        archivoPfx = System.IO.File.ReadAllBytes(razonesSocialesActual.rutaCert);
                    }
                    else
                    {
                        archivoPfx = razonesSocialesActual.certificadoSAT;
                    }

                    if (razonesSocialesActual.llaveSAT == null)
                    {
                        archivoKey = System.IO.File.ReadAllBytes(razonesSocialesActual.rutaLlave);
                    }
                    else
                    {
                        archivoKey = razonesSocialesActual.llaveSAT;
                    }

                    if (archivoPfx == null)
                    {
                        numError = 2;
                    }
                    else
                    {
                        infoATimbrar.archivoPfx = archivoPfx;
                    }

                    if (archivoKey == null)
                    {
                        numError = 2;
                    }
                    else
                    {
                        infoATimbrar.archivoKey = archivoKey;
                    }


                    ConfiguraTimbrado configuraTimbrado = razonesSocialesActual.configuraTimbrado;
                    bool valido = true;
                    if (configuraTimbrado == null)
                    {
                        valido = false;
                    }
                    else
                    {
                        if (configuraTimbrado.contraseña == null ? false : configuraTimbrado.contraseña.Trim().Length == 0)
                        {
                            valido = false;
                        }
                        else if (configuraTimbrado.URL == null ? false : configuraTimbrado.URL.Trim().Length == 0)
                        {
                            valido = false;
                        }
                        else if (configuraTimbrado.usuario == null ? false : configuraTimbrado.usuario.Trim().Length == 0)
                        {
                            valido = false;
                        }

                        if (valido)
                        {
                            infoATimbrar.password = configuraTimbrado.contraseña;
                            infoATimbrar.urlWebServices = configuraTimbrado.URL;
                            infoATimbrar.usuario = configuraTimbrado.usuario;

                        }
                        else
                        {
                            numError = 3;
                        }


                        if (numError == 0)
                        {
                            infoATimbrar.passwordKey = razonesSocialesActual.password;
                            infoATimbrar.tipoOperacion = TipoOperacionWS.CANCELAR;
                            InfoExtra infoExtra;
                            List<InfoExtra> listInfoEmpleado = new List<InfoExtra>();
                            Comprobante comprobante;
                            TimbreFiscalDigital timbreFiscalDigital;


                            for (int i = 0; i < listCFDIEmpleados.Count; i++)
                            {
                                if (listCFDIEmpleados[i].cfdiRecibo.statusTimbrado == StatusTimbrado.TIMBRADO)
                                {
                                    timbreFiscalDigital = null;
                                    XmlDocument doc = new XmlDocument();
                                    MemoryStream ms = new MemoryStream(listCFDIEmpleados[i].cfdiRecibo.xmlTimbrado);
                                    doc.Load(ms);
                                    XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));
                                    using (XmlReader writer = new XmlNodeReader(doc))
                                    {
                                        comprobante = (Comprobante)oXmlSerializar.Deserialize(writer);
                                    }
                                    if (comprobante == null)
                                    {
                                        listCFDIEmpleados[i].mensaje = ("error al abrir archivo|");
                                    }
                                    else
                                    {

                                        infoExtra = new InfoExtra();
                                        infoExtra.folio = comprobante.Folio;
                                        infoExtra.serie = comprobante.Serie;
                                        infoExtra.cfdirecibo_id = listCFDIEmpleados[i].cfdiRecibo_ID;
                                        infoExtra.usuario = usuarioActual.nombre;
                                        infoExtra.total = comprobante.Total;
                                        infoExtra.rfcEmisor = comprobante.Emisor.Rfc == null ? "" : comprobante.Emisor.Rfc;
                                        infoExtra.rfcReceptor = comprobante.Receptor.Rfc == null ? "" : comprobante.Receptor.Rfc;
                                        XmlElement[] listaComplementos = comprobante.Complemento[0].Any;
                                        if (listaComplementos != null)
                                        {
                                            for (int j = 0; j < listaComplementos.Length; j++)
                                            {
                                                if (listaComplementos[j].LocalName.Equals("TimbreFiscalDigital"))
                                                {
                                                    XmlSerializer serializer = new XmlSerializer(typeof(TimbreFiscalDigital));
                                                    timbreFiscalDigital = (TimbreFiscalDigital)serializer.Deserialize(new XmlNodeReader(listaComplementos[j]));
                                                    break;
                                                }
                                            }
                                            if (timbreFiscalDigital != null)
                                            {
                                                infoExtra.UUID = (timbreFiscalDigital.UUID);
                                            }
                                        }
                                        listInfoEmpleado.Add(infoExtra);
                                    }



                                }
                            }

                            if (listInfoEmpleado.Count > 0)
                            {
                                infoATimbrar.infoExtras = listInfoEmpleado;
                                TimbrarXmlSat timbrarXmlSat = new TimbrarXmlSat();
                                List<object> timbreFiscalDigitales = timbrarXmlSat.generaTimbres(infoATimbrar, dbContSimple);
                                if (timbrarXmlSat.error != null)
                                {
                                    if (timbrarXmlSat.error.Trim().Length > 0)
                                    {
                                        mensajeResultado.noError = (100);
                                        mensajeResultado.error = (timbrarXmlSat.error);
                                        //util.mensajeError(rootPane, Utilerias.ObtenerMensaje.getString("CancelaTimbradoMsgErrorWebService"));
                                    }
                                }

                                if (timbreFiscalDigitales == null)
                                {
                                    timbreFiscalDigitales = new List<object>();
                                }
                                BITCancelacion bITCancelacion = new BITCancelacion();
                                CFDIReciboProcCanc CFDIReciboProcCanc = new CFDIReciboProcCanc();
                                int j = 0;
                                for (int i = 0; i < listCFDIEmpleados.Count(); i++)
                                {
                                    while (j < timbreFiscalDigitales.Count())
                                    {
                                        cUUIDCancelado infoCancelado = (cUUIDCancelado)timbreFiscalDigitales[i];
                                        //listCFDIEmpleados.get(i).getCfdiRecibo().getFolioCFDI().equalsIgnoreCase(timbreFiscalDigitales.get(j).getFolio())
                                        if (string.Equals(listCFDIEmpleados[i].cfdiRecibo.UUID, infoCancelado.UUID, StringComparison.OrdinalIgnoreCase))
                                        {
                                            //timbreFiscalDigitales.get(j).getStatus().trim().equalsIgnoreCase("100") || timbreFiscalDigitales.get(j).getStatus().trim().equalsIgnoreCase("201")
                                            if (string.Equals(infoCancelado.status, "100", StringComparison.OrdinalIgnoreCase) || string.Equals(infoCancelado.status, "201", StringComparison.OrdinalIgnoreCase))
                                            {

                                                if (infoCancelado.Acuse == null)
                                                {
                                                    listCFDIEmpleados[i].cfdiRecibo.statusTimbrado = (StatusTimbrado.EN_PROCESO_CANCELACION);
                                                    listCFDIEmpleados[i].mensaje = "EN PROCESO DE CANCELACION";
                                                    bITCancelacion.fechaYHora = DateTime.Now;
                                                    bITCancelacion.usuario = usuarioActual.nombre;
                                                    bITCancelacion.proceso = ProcesoBitcancelacion.MODIFICACION_CFDIRECIBO;
                                                    bITCancelacion.status = StatusBitcancelacion.BDProcesoCancelacion;
                                                    bITCancelacion.cfdiRecibo_ID = listCFDIEmpleados[i].cfdiRecibo_ID;
                                                    bITCancelacion.statusMsj = "EN PROCESO DE CANCELACION";
                                                    bitCancelacionDAO.agregar(bITCancelacion, dbContSimple);

                                                    CFDIReciboProcCanc.serie = listCFDIEmpleados[i].cfdiRecibo.serie;
                                                    CFDIReciboProcCanc.folio = listCFDIEmpleados[i].cfdiRecibo.folioCFDI;
                                                    CFDIReciboProcCanc.fechaIntento = DateTime.Now;
                                                    CFDIReciboProcCanc.cfdiRecibo_ID = listCFDIEmpleados[i].cfdiRecibo_ID;
                                                    cFDIReciboProcCancDAO.agregar(CFDIReciboProcCanc, dbContSimple);


                                                }
                                                else
                                                {
                                                    listCFDIEmpleados[i].cfdiRecibo.acuse = (infoCancelado.Acuse);
                                                    listCFDIEmpleados[i].cfdiRecibo.statusTimbrado = (StatusTimbrado.CANCELADO);
                                                    listCFDIEmpleados[i].cfdiRecibo.fechaHoraTimCancelado = DateTime.Now;//leeracuse y tomarfecha cancelado
                                                    listCFDIEmpleados[i].mensaje = "CANCELADO";


                                                    bITCancelacion.fechaYHora = DateTime.Now;
                                                    bITCancelacion.usuario = usuarioActual.nombre;
                                                    bITCancelacion.proceso = ProcesoBitcancelacion.MODIFICACION_CFDIRECIBO;
                                                    bITCancelacion.status = StatusBitcancelacion.BdCancelado;
                                                    bITCancelacion.cfdiRecibo_ID = listCFDIEmpleados[i].cfdiRecibo_ID;
                                                    bITCancelacion.statusMsj = "CANCELADO";
                                                    bitCancelacionDAO.agregar(bITCancelacion, dbContSimple);


                                                }

                                            }
                                            else
                                            {
                                                listCFDIEmpleados[i].mensaje = (infoCancelado.observaciones);
                                            }

                                            timbreFiscalDigitales.RemoveAt(j);
                                            break;
                                        }
                                        else
                                        {
                                            j++;
                                        }
                                    }
                                    j = 0;
                                    if (!timbreFiscalDigitales.Any())
                                    {
                                        break;
                                    }
                                }
                            }

                        }


                        if (listCFDIEmpleados != null)
                        {
                            if (listCFDIEmpleados.Count > 0)
                            {
                                List<CFDIEmpleado> listCFDIGuardar = new List<CFDIEmpleado>();
                                int i = 0;
                                while (i < listCFDIEmpleados.Count)
                                {
                                    listCFDIEmpleados[i].statusGeneraInfo = false;
                                    if (listCFDIEmpleados[i].cfdiRecibo.statusTimbrado == StatusTimbrado.CANCELADO)
                                    {
                                        listCFDIGuardar.Add(listCFDIEmpleados[i]);
                                        listCFDIEmpleados.RemoveAt(i);
                                    }
                                    i++;

                                }

                                if (listCFDIGuardar.Count > 0)
                                {
                                    mensajeResultado = cfdiEmpleadoDAO.saveDeleteCFDIEmpleado(listCFDIGuardar, null, 50, dbContSimple);
                                    if (mensajeResultado.noError != 0)
                                    {
                                        //util.mensajeError(getView(), mensajeDB.getError());
                                        //return;
                                        return mensajeResultado;
                                    }
                                    listCFDIEmpleados.AddRange(listCFDIGuardar);
                                }
                            }
                        }

                        if (mensajeResultado.resultado != null)
                        {
                            if (mensajeResultado.noError == 0)
                            {

                                List<CFDIEmpleado> cfdiEmpleado = listCFDIEmpleados;
                                var datos = cfdiEmpleado.Select(d => new
                                {
                                    d.id,
                                    d.CLABE,
                                    d.CURP,
                                    d.RFC,
                                    d.antiguedad,
                                    d.antiguedadYMD,
                                    d.apellidoMaterno,
                                    d.apellidoPaterno,
                                    d.calle,
                                    d.ciudad,
                                    d.claveBancoSat,
                                    d.codigoPostal,
                                    d.colonia,
                                    d.correoElectronico,
                                    d.cuentaBancaria,
                                    d.departamento,
                                    d.estado,
                                    d.fechaFinalPago,
                                    d.fechaInicioPago,
                                    d.fechaInicioRelLaboral,
                                    d.fechaPago,
                                    d.formaPago,
                                    d.jornada,
                                    d.municipio,
                                    d.noExterior,
                                    d.noInterior,
                                    d.noRegistroPatronal,
                                    d.noSeguroSocial,
                                    d.nombre,
                                    d.numeroDiasPago,
                                    d.pais,
                                    d.periodiciadadPago,
                                    d.puesto,
                                    d.regimenContratacion,
                                    d.riesgoPuesto,
                                    d.salBaseCotAport,
                                    d.salIntIMSS,
                                    d.tipoContrato,
                                    tipoNomina_ID = d.tipoNomina.id,
                                    tipoCorrida_ID = d.tipoCorrida.id,
                                    razonesSociales_ID = d.razonesSociales.id,
                                    periodosNomina_ID = d.periodosNomina.id,
                                    plazaPorEmpleadoMov_ID = d.plazasPorEmpleadosMov.id,
                                    d.statusGeneraInfo,
                                    d.asignoFolio,
                                    d.recienTimbrado,
                                    d.mensaje,
                                    clave = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                                    d.cfdiRecibo_ID,
                                    cfdiRecibo = new
                                    {
                                        d.cfdiRecibo.id,
                                        d.cfdiRecibo.UUID,
                                        d.cfdiRecibo.serie,
                                        d.cfdiRecibo.acuse,
                                        d.cfdiRecibo.UUIDRelacionado,
                                        d.cfdiRecibo.cadenaCertificado,
                                        d.cfdiRecibo.cadenaOriginalTimbrado,
                                        d.cfdiRecibo.fechaGeneraInfo,
                                        d.cfdiRecibo.fechaHoraTimCancelado,
                                        d.cfdiRecibo.fechaEmision,
                                        d.cfdiRecibo.folioCFDI,
                                        d.cfdiRecibo.leyenda,
                                        d.cfdiRecibo.motivoCancelacion,
                                        d.cfdiRecibo.noCertificado,
                                        d.cfdiRecibo.noCertificadoSAT,
                                        d.cfdiRecibo.rfcProvCertif,
                                        d.cfdiRecibo.sello,
                                        d.cfdiRecibo.selloCFD,
                                        d.cfdiRecibo.selloSAT,
                                        d.cfdiRecibo.serieCFDI,
                                        d.cfdiRecibo.statusTimbrado,
                                        d.cfdiRecibo.statusXmlSat,
                                        d.cfdiRecibo.total,
                                        d.cfdiRecibo.version,
                                        d.cfdiRecibo.xmlTimbrado,
                                        d.cfdiRecibo.certificadoTimbrado,
                                        d.cfdiRecibo.fechaHoraTimbrado,
                                        d.cfdiRecibo.selloTimbrado,
                                        d.cfdiRecibo.mensajeRec,
                                        cfdiReciboConcepto = d.cfdiRecibo.cfdiReciboConcepto.Select(r => new
                                        {
                                            r.id,
                                            r.claveConcepto,
                                            r.claveSAT,
                                            r.descripcionConcepto,
                                            r.importeExento,
                                            r.importeGravable,
                                            r.otroPago,
                                            r.tipoNaturaleza
                                        }).ToList(),
                                        cfdiReciboHrsExtras = d.cfdiRecibo.cfdiReciboHrsExtras.Select(h => new
                                        {
                                            h.id,
                                            h.dias,
                                            h.horasExtras,
                                            h.importeExento,
                                            h.importeGravable
                                        }).ToList(),
                                        cfdiReciboIncapacidad = d.cfdiRecibo.cfdiReciboIncapacidad.Select(i => new
                                        {
                                            i.id,
                                            i.diasIncapacidad,
                                            i.importeMonetario,
                                            i.tipoIncapacidad
                                        }).ToList()
                                    }
                                }).ToList();


                                //inicializaVariableMensaje();

                                mensajeResultado.resultado = datos;
                                mensajeResultado.noError = 0;
                                mensajeResultado.error = "";





                            }
                            else
                            {
                                List<CFDIEmpleado> cfdiEmpleado = (List<CFDIEmpleado>)mensajeResultado.resultado;

                                var datos = cfdiEmpleado.Select(d => new
                                {
                                    d.id,
                                    d.CLABE,
                                    d.CURP,
                                    d.RFC,
                                    d.antiguedad,
                                    d.antiguedadYMD,
                                    d.apellidoMaterno,
                                    d.apellidoPaterno,
                                    d.calle,
                                    d.ciudad,
                                    d.claveBancoSat,
                                    d.codigoPostal,
                                    d.colonia,
                                    d.correoElectronico,
                                    d.cuentaBancaria,
                                    d.departamento,
                                    d.estado,
                                    d.fechaFinalPago,
                                    d.fechaInicioPago,
                                    d.fechaInicioRelLaboral,
                                    d.fechaPago,
                                    d.formaPago,
                                    d.jornada,
                                    d.municipio,
                                    d.noExterior,
                                    d.noInterior,
                                    d.noRegistroPatronal,
                                    d.noSeguroSocial,
                                    d.nombre,
                                    d.numeroDiasPago,
                                    d.pais,
                                    d.periodiciadadPago,
                                    d.puesto,
                                    d.regimenContratacion,
                                    d.riesgoPuesto,
                                    d.salBaseCotAport,
                                    d.salIntIMSS,
                                    d.tipoContrato,
                                    tipoNomina_ID = d.tipoNomina.id,
                                    tipoCorrida_ID = d.tipoCorrida.id,
                                    razonesSociales_ID = d.razonesSociales.id,
                                    periodosNomina_ID = d.periodosNomina.id,
                                    plazaPorEmpleadoMov_ID = d.plazasPorEmpleadosMov.id,
                                    d.statusGeneraInfo,
                                    d.asignoFolio,
                                    d.recienTimbrado,
                                    d.mensaje,
                                    clave = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                                    d.cfdiRecibo_ID,
                                    cfdiRecibo = new
                                    {
                                        d.cfdiRecibo.id,
                                        d.cfdiRecibo.UUID,
                                        d.cfdiRecibo.serie,
                                        d.cfdiRecibo.acuse,
                                        d.cfdiRecibo.UUIDRelacionado,
                                        d.cfdiRecibo.cadenaCertificado,
                                        d.cfdiRecibo.cadenaOriginalTimbrado,
                                        d.cfdiRecibo.fechaGeneraInfo,
                                        d.cfdiRecibo.fechaHoraTimCancelado,
                                        d.cfdiRecibo.fechaEmision,
                                        d.cfdiRecibo.folioCFDI,
                                        d.cfdiRecibo.leyenda,
                                        d.cfdiRecibo.motivoCancelacion,
                                        d.cfdiRecibo.noCertificado,
                                        d.cfdiRecibo.noCertificadoSAT,
                                        d.cfdiRecibo.rfcProvCertif,
                                        d.cfdiRecibo.sello,
                                        d.cfdiRecibo.selloCFD,
                                        d.cfdiRecibo.selloSAT,
                                        d.cfdiRecibo.serieCFDI,
                                        d.cfdiRecibo.statusTimbrado,
                                        d.cfdiRecibo.statusXmlSat,
                                        d.cfdiRecibo.total,
                                        d.cfdiRecibo.version,
                                        d.cfdiRecibo.xmlTimbrado,
                                        d.cfdiRecibo.certificadoTimbrado,
                                        d.cfdiRecibo.fechaHoraTimbrado,
                                        d.cfdiRecibo.selloTimbrado,
                                        d.cfdiRecibo.mensajeRec,
                                        cfdiReciboConcepto = d.cfdiRecibo.cfdiReciboConcepto.Select(r => new
                                        {
                                            r.id,
                                            r.claveConcepto,
                                            r.claveSAT,
                                            r.descripcionConcepto,
                                            r.importeExento,
                                            r.importeGravable,
                                            r.otroPago,
                                            r.tipoNaturaleza
                                        }).ToList(),
                                        cfdiReciboHrsExtras = d.cfdiRecibo.cfdiReciboHrsExtras.Select(h => new
                                        {
                                            h.id,
                                            h.dias,
                                            h.horasExtras,
                                            h.importeExento,
                                            h.importeGravable
                                        }).ToList(),
                                        cfdiReciboIncapacidad = d.cfdiRecibo.cfdiReciboIncapacidad.Select(i => new
                                        {
                                            i.id,
                                            i.diasIncapacidad,
                                            i.importeMonetario,
                                            i.tipoIncapacidad
                                        }).ToList()
                                    }
                                }).ToList();




                                mensajeResultado.resultado = datos;
                                mensajeResultado.noError = 0;
                                mensajeResultado.error = "";
                            }
                        }


                    }




                }
            }
            catch (Exception)
            {

                throw;
            }




            return mensajeResultado;
        }

        public Mensaje buscarTimbresId(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal, decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, int tipoNodoConsulta, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster)
        {
            idTipoCorr = idTipoCorrida;
            idTipoNom = idTipoNomina;
            idPeriodoNom = idPeriodoNomina;
            idRegPatr = idRegPatronal;
            idCenCost = idCenCosto;
            idDepart = idDepartamento;
            idEmpleadoIni = idEmplIni;
            idEmpleadoFin = idEmplFin;
            idRazSocial = idRazonSocial;
            dbContSimple = dbContextSimple;
            dbConttMaster = dbContextMaster;
            mensajeResultado = getRazonSocialActual();



            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar RazonSocial";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            mensajeResultado = getUsuarioActual();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar Usuario";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            inicializaValores();
            mensajeResultado = getStatusPeriodoNomina();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar periodo";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }



            if (tipoNodoConsulta == 0)
            {
                bool periodoOpen = (bool)mensajeResultado.resultado;
                if (periodoOpen)
                {
                    mensajeResultado.error = "periodo esta abierto";
                    mensajeResultado.noError = 6;
                    mensajeResultado.resultado = null;

                    return mensajeResultado;
                }
                bool certVigente = existeCertificadoVigente(dbContextSimple);
                if (!certVigente)
                {
                    mensajeResultado.error = "no existe certificado vigente";
                    mensajeResultado.noError = 10;
                    mensajeResultado.resultado = null;

                    return mensajeResultado;
                }
            }

            List<Object> wh = obtenerValoresFiltrado();
            wh.Add(StatusTimbrado.EN_PROCESO);
            wh.Add(StatusTimbrado.ERROR);

            mensajeResultado = cfdiEmpleadoDAO.buscaCFDIEmpleadosFiltrado(wh, null, dbContSimple);



            if (mensajeResultado.noError != 0)
            {

                return mensajeResultado;
            }
            else

            {
                if (mensajeResultado.resultado != null)
                {
                    List<CFDIEmpleado> cfdiEmpleado = (List<CFDIEmpleado>)mensajeResultado.resultado;

                    inicializaVariableMensaje();


                    if (tipoNodoConsulta == 0)
                    {
                        var datos = cfdiEmpleado.Select(d => new
                        {
                            d.id
                        }).ToList();


                        inicializaVariableMensaje();

                        mensajeResultado.resultado = datos;
                        mensajeResultado.noError = 0;
                        mensajeResultado.error = "";
                    }
                    else if (tipoNodoConsulta == 1)
                    {
                        inicializaVariableMensaje();

                        mensajeResultado.resultado = cfdiEmpleado;
                        mensajeResultado.noError = 0;
                        mensajeResultado.error = "";
                    }



                }
            }

            return mensajeResultado;
        }

        public Mensaje buscarParaTimbrar(decimal? idTipoCorrida, decimal? idTipoNomina, decimal? idPeriodoNomina, decimal? idRegPatronal, decimal? idCenCosto, decimal? idDepartamento, decimal? idEmplIni, decimal? idEmplFin, decimal? idRazonSocial, bool desdeCancelado, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster)
        {
            idTipoCorr = idTipoCorrida;
            idTipoNom = idTipoNomina;
            idPeriodoNom = idPeriodoNomina;
            idRegPatr = idRegPatronal;
            idCenCost = idCenCosto;
            idDepart = idDepartamento;
            idEmpleadoIni = idEmplIni;
            idEmpleadoFin = idEmplFin;
            idRazSocial = idRazonSocial;

            dbContSimple = dbContextSimple;
            dbConttMaster = dbContextMaster;
            mensajeResultado = getRazonSocialActual();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar RazonSocial";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            mensajeResultado = getUsuarioActual();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar Usuario";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            inicializaValores();
            mensajeResultado = getStatusPeriodoNomina();
            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar periodo";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }

            bool periodoOpen = (bool)mensajeResultado.resultado;
            if (periodoOpen)
            {
                mensajeResultado.error = "periodo esta abierto";
                mensajeResultado.noError = 6;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }
            bool certVigente = existeCertificadoVigente(dbContextSimple);
            if (!certVigente)
            {
                mensajeResultado.error = "no existe certificado vigente";
                mensajeResultado.noError = 10;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }

            List<Object> wh = obtenerValoresFiltrado();

            if (desdeCancelado)
            {
                wh.Add(StatusTimbrado.TIMBRADO);
            }
            else
            {
                wh.Add(StatusTimbrado.EN_PROCESO);
                wh.Add(StatusTimbrado.ERROR);
            }



            mensajeResultado = cfdiEmpleadoDAO.buscaCFDIEmpleadosFiltrado(wh, null, dbContSimple);

            //asignarle serie y folio

            if (mensajeResultado.noError != 0)
            {

                return mensajeResultado;
            }
            else

            {
                if (mensajeResultado.resultado != null)
                {
                    List<CFDIEmpleado> cfdiEmpleado = (List<CFDIEmpleado>)mensajeResultado.resultado;

                    inicializaVariableMensaje();

                    String serieN = tipoNomina.series.serie;

                    int folioIniN = tipoNomina.series.folioInicial;

                    int longitudFolioN = tipoNomina.series.longitudFolio;
                    int ultifolio = ultimofolio(serieN, folioIniN, longitudFolioN);

                    for (int l = 0; l < cfdiEmpleado.Count; l++)
                    {
                       
                        cfdiEmpleado[l].cfdiRecibo.serie = serieN;
                        cfdiEmpleado[l].cfdiRecibo.folioCFDI = datoMascara(serieN, ultifolio, longitudFolioN);
                        ultifolio++;
                    }



                    //mensajeResultado = SaveCFDIInformacion(cfdiEmpleado);


                    var datos = cfdiEmpleado.Select(d => new
                    {
                        d.id,
                        d.CLABE,
                        d.CURP,
                        d.RFC,
                        d.antiguedad,
                        d.antiguedadYMD,
                        d.apellidoMaterno,
                        d.apellidoPaterno,
                        d.calle,
                        d.ciudad,
                        d.claveBancoSat,
                        d.codigoPostal,
                        d.colonia,
                        d.correoElectronico,
                        d.cuentaBancaria,
                        d.departamento,
                        d.estado,
                        d.fechaFinalPago,
                        d.fechaInicioPago,
                        d.fechaInicioRelLaboral,
                        d.fechaPago,
                        d.formaPago,
                        d.jornada,
                        d.municipio,
                        d.noExterior,
                        d.noInterior,
                        d.noRegistroPatronal,
                        d.noSeguroSocial,
                        d.nombre,
                        d.numeroDiasPago,
                        d.pais,
                        d.periodiciadadPago,
                        d.puesto,
                        d.regimenContratacion,
                        d.riesgoPuesto,
                        d.salBaseCotAport,
                        d.salIntIMSS,
                        d.tipoContrato,
                        tipoNomina_ID = d.tipoNomina.id,
                        tipoCorrida_ID = d.tipoCorrida.id,
                        razonesSociales_ID = d.razonesSociales.id,
                        periodosNomina_ID = d.periodosNomina.id,
                        plazaPorEmpleadoMov_ID = d.plazasPorEmpleadosMov.id,
                        d.statusGeneraInfo,
                        d.asignoFolio,
                        d.recienTimbrado,
                        idTabla = 0,
                        seleccionado = 0,
                        clave = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                        idEmpleadoTabla = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.id,
                        d.cfdiRecibo_ID,
                        cfdiRecibo = new
                        {
                            d.cfdiRecibo.id,
                            d.cfdiRecibo.UUID,
                            d.cfdiRecibo.serie,
                            d.cfdiRecibo.acuse,
                            d.cfdiRecibo.UUIDRelacionado,
                            d.cfdiRecibo.cadenaCertificado,
                            d.cfdiRecibo.cadenaOriginalTimbrado,
                            d.cfdiRecibo.fechaGeneraInfo,
                            d.cfdiRecibo.fechaHoraTimCancelado,
                            d.cfdiRecibo.fechaEmision,
                            d.cfdiRecibo.folioCFDI,
                            d.cfdiRecibo.leyenda,
                            d.cfdiRecibo.motivoCancelacion,
                            d.cfdiRecibo.noCertificado,
                            d.cfdiRecibo.noCertificadoSAT,
                            d.cfdiRecibo.rfcProvCertif,
                            d.cfdiRecibo.sello,
                            d.cfdiRecibo.selloCFD,
                            d.cfdiRecibo.selloSAT,
                            d.cfdiRecibo.serieCFDI,
                            d.cfdiRecibo.statusTimbrado,
                            d.cfdiRecibo.statusXmlSat,
                            d.cfdiRecibo.total,
                            d.cfdiRecibo.version,
                            d.cfdiRecibo.xmlTimbrado,
                            d.cfdiRecibo.certificadoTimbrado,
                            d.cfdiRecibo.fechaHoraTimbrado,
                            d.cfdiRecibo.selloTimbrado,
                            d.cfdiRecibo.mensajeRec,
                            cfdiReciboConcepto = d.cfdiRecibo.cfdiReciboConcepto.Select(r => new
                            {
                                r.id,
                                r.claveConcepto,
                                r.claveSAT,
                                r.descripcionConcepto,
                                r.importeExento,
                                r.importeGravable,
                                r.otroPago,
                                r.tipoNaturaleza
                            }).ToList(),
                            cfdiReciboHrsExtras = d.cfdiRecibo.cfdiReciboHrsExtras.Select(h => new
                            {
                                h.id,
                                h.dias,
                                h.horasExtras,
                                h.importeExento,
                                h.importeGravable
                            }).ToList(),
                            cfdiReciboIncapacidad = d.cfdiRecibo.cfdiReciboIncapacidad.Select(i => new
                            {
                                i.id,
                                i.diasIncapacidad,
                                i.importeMonetario,
                                i.tipoIncapacidad
                            }).ToList()
                        }
                    }).ToList();



                    inicializaVariableMensaje();

                    mensajeResultado.resultado = datos;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                }
            }

            return mensajeResultado;
        }

        public Mensaje buscarEnProcesoCanc(decimal? idRazonSocial, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster)
        {

            idTipoCorr = null;
            idTipoNom = null;
            idPeriodoNom = null;
            idRegPatr = null;
            idCenCost = null;
            idDepart = null;
            idEmpleadoIni = null;
            idEmpleadoFin = null;
            idRazSocial = idRazonSocial;

            dbContSimple = dbContextSimple;
            dbConttMaster = dbContextMaster;

            mensajeResultado = getRazonSocialActual();


            if (mensajeResultado.noError != 0)
            {
                mensajeResultado.error = "error consultar RazonSocial";
                mensajeResultado.noError = 50;
                mensajeResultado.resultado = null;

                return mensajeResultado;
            }

            List<Object> wh = obtenerValoresFiltrado();


            mensajeResultado = cfdiEmpleadoDAO.buscarCFDIEmpleadosEnProceso(wh, dbContSimple);



            if (mensajeResultado.noError != 0)
            {

                return mensajeResultado;
            }
            else

            {
                if (mensajeResultado.resultado != null)
                {
                    List<CFDIEmpleado> cfdiEmpleado = (List<CFDIEmpleado>)mensajeResultado.resultado;

                    inicializaVariableMensaje();


                    var datos = cfdiEmpleado.Select(d => new
                    {
                        d.id,
                        d.CLABE,
                        d.CURP,
                        d.RFC,
                        d.antiguedad,
                        d.antiguedadYMD,
                        d.apellidoMaterno,
                        d.apellidoPaterno,
                        d.calle,
                        d.ciudad,
                        d.claveBancoSat,
                        d.codigoPostal,
                        d.colonia,
                        d.correoElectronico,
                        d.cuentaBancaria,
                        d.departamento,
                        d.estado,
                        d.fechaFinalPago,
                        d.fechaInicioPago,
                        d.fechaInicioRelLaboral,
                        d.fechaPago,
                        d.formaPago,
                        d.jornada,
                        d.municipio,
                        d.noExterior,
                        d.noInterior,
                        d.noRegistroPatronal,
                        d.noSeguroSocial,
                        d.nombre,
                        d.numeroDiasPago,
                        d.pais,
                        d.periodiciadadPago,
                        d.puesto,
                        d.regimenContratacion,
                        d.riesgoPuesto,
                        d.salBaseCotAport,
                        d.salIntIMSS,
                        d.tipoContrato,
                        tipoNominaID = d.tipoNomina.id,
                        tipoNominaClave = d.tipoNomina.clave,
                        tipoCorrida_ID = d.tipoCorrida.id,
                        tipoCorridaClave = d.tipoCorrida.clave,
                        razonesSociales_ID = d.razonesSociales.id,
                        periodosNomina_ID = d.periodosNomina.id,
                        periodoNominaClave = d.periodosNomina.clave,
                        plazaPorEmpleadoMov_ID = d.plazasPorEmpleadosMov.id,
                        d.statusGeneraInfo,
                        d.asignoFolio,
                        d.recienTimbrado,
                        clave = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                        idEmpleadoTabla = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.id,
                        d.cfdiRecibo_ID,
                        cfdiRecibo = new
                        {
                            d.cfdiRecibo.id,
                            d.cfdiRecibo.UUID,
                            d.cfdiRecibo.serie,
                            d.cfdiRecibo.acuse,
                            d.cfdiRecibo.UUIDRelacionado,
                            d.cfdiRecibo.cadenaCertificado,
                            d.cfdiRecibo.cadenaOriginalTimbrado,
                            d.cfdiRecibo.fechaGeneraInfo,
                            d.cfdiRecibo.fechaHoraTimCancelado,
                            d.cfdiRecibo.fechaEmision,
                            d.cfdiRecibo.folioCFDI,
                            d.cfdiRecibo.leyenda,
                            d.cfdiRecibo.motivoCancelacion,
                            d.cfdiRecibo.noCertificado,
                            d.cfdiRecibo.noCertificadoSAT,
                            d.cfdiRecibo.rfcProvCertif,
                            d.cfdiRecibo.sello,
                            d.cfdiRecibo.selloCFD,
                            d.cfdiRecibo.selloSAT,
                            d.cfdiRecibo.serieCFDI,
                            d.cfdiRecibo.statusTimbrado,
                            d.cfdiRecibo.statusXmlSat,
                            d.cfdiRecibo.total,
                            d.cfdiRecibo.version,
                            d.cfdiRecibo.xmlTimbrado,
                            d.cfdiRecibo.certificadoTimbrado,
                            d.cfdiRecibo.fechaHoraTimbrado,
                            d.cfdiRecibo.selloTimbrado,
                            d.cfdiRecibo.mensajeRec,
                            cfdiReciboConcepto = d.cfdiRecibo.cfdiReciboConcepto.Select(r => new
                            {
                                r.id,
                                r.claveConcepto,
                                r.claveSAT,
                                r.descripcionConcepto,
                                r.importeExento,
                                r.importeGravable,
                                r.otroPago,
                                r.tipoNaturaleza
                            }).ToList(),
                            cfdiReciboHrsExtras = d.cfdiRecibo.cfdiReciboHrsExtras.Select(h => new
                            {
                                h.id,
                                h.dias,
                                h.horasExtras,
                                h.importeExento,
                                h.importeGravable
                            }).ToList(),
                            cfdiReciboIncapacidad = d.cfdiRecibo.cfdiReciboIncapacidad.Select(i => new
                            {
                                i.id,
                                i.diasIncapacidad,
                                i.importeMonetario,
                                i.tipoIncapacidad
                            }).ToList()
                        }
                    }).ToList();



                    inicializaVariableMensaje();

                    mensajeResultado.resultado = datos;
                    mensajeResultado.noError = 0;
                    mensajeResultado.error = "";
                }
            }

            return mensajeResultado;

        }

        public Mensaje recuperarAcuse(decimal? idRazonSocial, List<decimal?> listIdEmpleados, DBContextAdapter dbContextSimple, DBContextAdapter dbContextMaster)
        {
            int numError = 0;


            try
            { //--
                idTipoCorr = null;
                idTipoNom = null;
                idPeriodoNom = null;
                idRegPatr = null;
                idCenCost = null;
                idDepart = null;
                idEmpleadoIni = null;
                idEmpleadoFin = null;
                idRazSocial = idRazonSocial;
                dbContSimple = dbContextSimple;
                dbConttMaster = dbContextMaster;
                listIdEmpl = listIdEmpleados;
                mensajeResultado = getRazonSocialActual();
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.error = "error consultar RazonSocial";
                    mensajeResultado.noError = 50;
                    mensajeResultado.resultado = null;

                    return mensajeResultado;
                }
                mensajeResultado = getUsuarioActual();
                if (mensajeResultado.noError != 0)
                {
                    mensajeResultado.error = "Error consultar Usuario";
                    mensajeResultado.noError = 50;
                    mensajeResultado.resultado = null;

                    return mensajeResultado;
                }

                List<Object> wh = obtenerValoresFiltrado();

                mensajeResultado = cfdiEmpleadoDAO.buscarCFDIEmpleadosEnProceso(wh, dbContSimple);
                if (mensajeResultado.noError > 0)
                {
                    mensajeResultado.error = "error al consultar recibos en proceso de cancelacin";
                    mensajeResultado.noError = 6;
                    mensajeResultado.resultado = null;
                    return mensajeResultado;
                }
                else
                {
                    List<CFDIEmpleado> listCFDIEmpleados = null;
                    listCFDIEmpleados = (List<CFDIEmpleado>)mensajeResultado.resultado;
                    listCFDIEmpleados = listCFDIEmpleados == null ? new List<CFDIEmpleado>() : listCFDIEmpleados;
                    InfoATimbrar infoATimbrar = new InfoATimbrar();
                    byte[] archivoPfx;
                    byte[] archivoKey;
                    if (razonesSocialesActual.certificadoSAT == null)
                    {
                        archivoPfx = System.IO.File.ReadAllBytes(razonesSocialesActual.rutaCert);
                    }
                    else
                    {
                        archivoPfx = razonesSocialesActual.certificadoSAT;
                    }

                    if (razonesSocialesActual.llaveSAT == null)
                    {
                        archivoKey = System.IO.File.ReadAllBytes(razonesSocialesActual.rutaLlave);
                    }
                    else
                    {
                        archivoKey = razonesSocialesActual.llaveSAT;
                    }

                    if (archivoPfx == null)
                    {
                        numError = 2;
                    }
                    else
                    {
                        infoATimbrar.archivoPfx = archivoPfx;
                    }

                    if (archivoKey == null)
                    {
                        numError = 2;
                    }
                    else
                    {
                        infoATimbrar.archivoKey = archivoKey;
                    }


                    ConfiguraTimbrado configuraTimbrado = razonesSocialesActual.configuraTimbrado;
                    bool valido = true;
                    if (configuraTimbrado == null)
                    {
                        valido = false;
                    }
                    else
                    {
                        if (configuraTimbrado.contraseña == null ? false : configuraTimbrado.contraseña.Trim().Length == 0)
                        {
                            valido = false;
                        }
                        else if (configuraTimbrado.URL == null ? false : configuraTimbrado.URL.Trim().Length == 0)
                        {
                            valido = false;
                        }
                        else if (configuraTimbrado.usuario == null ? false : configuraTimbrado.usuario.Trim().Length == 0)
                        {
                            valido = false;
                        }

                        if (valido)
                        {
                            infoATimbrar.password = configuraTimbrado.contraseña;
                            infoATimbrar.urlWebServices = configuraTimbrado.URL;
                            infoATimbrar.usuario = configuraTimbrado.usuario;

                        }
                        else
                        {
                            numError = 3;
                        }


                        if (numError == 0)
                        {
                            infoATimbrar.passwordKey = razonesSocialesActual.password;
                            infoATimbrar.tipoOperacion = TipoOperacionWS.CANCELARSTATUS;
                            InfoExtra infoExtra;
                            List<InfoExtra> listInfoEmpleado = new List<InfoExtra>();
                            Comprobante comprobante;
                            TimbreFiscalDigital timbreFiscalDigital;


                            for (int i = 0; i < listCFDIEmpleados.Count; i++)
                            {
                                //if (listCFDIEmpleados[i].cfdiRecibo.statusTimbrado == StatusTimbrado.TIMBRADO)

                                timbreFiscalDigital = null;
                                XmlDocument doc = new XmlDocument();
                                MemoryStream ms = new MemoryStream(listCFDIEmpleados[i].cfdiRecibo.xmlTimbrado);
                                doc.Load(ms);
                                XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));
                                using (XmlReader writer = new XmlNodeReader(doc))
                                {
                                    comprobante = (Comprobante)oXmlSerializar.Deserialize(writer);
                                }
                                if (comprobante == null)
                                {
                                    listCFDIEmpleados[i].mensaje = ("error al abrir archivo|");
                                }
                                else
                                {

                                    infoExtra = new InfoExtra();
                                    infoExtra.folio = comprobante.Folio;
                                    infoExtra.serie = comprobante.Serie;
                                    infoExtra.cfdirecibo_id = listCFDIEmpleados[i].cfdiRecibo_ID;
                                    infoExtra.usuario = usuarioActual.nombre;
                                    infoExtra.total = comprobante.Total;
                                    infoExtra.rfcEmisor = comprobante.Emisor.Rfc == null ? "" : comprobante.Emisor.Rfc;
                                    infoExtra.rfcReceptor = comprobante.Receptor.Rfc == null ? "" : comprobante.Receptor.Rfc;
                                    XmlElement[] listaComplementos = comprobante.Complemento[0].Any;
                                    if (listaComplementos != null)
                                    {
                                        for (int j = 0; j < listaComplementos.Length; j++)
                                        {
                                            if (listaComplementos[j].LocalName.Equals("TimbreFiscalDigital"))
                                            {
                                                XmlSerializer serializer = new XmlSerializer(typeof(TimbreFiscalDigital));
                                                timbreFiscalDigital = (TimbreFiscalDigital)serializer.Deserialize(new XmlNodeReader(listaComplementos[j]));
                                                break;
                                            }
                                        }
                                        if (timbreFiscalDigital != null)
                                        {
                                            infoExtra.UUID = (timbreFiscalDigital.UUID);
                                        }
                                    }
                                    listInfoEmpleado.Add(infoExtra);
                                }




                            }

                            if (listInfoEmpleado.Count > 0)
                            {
                                infoATimbrar.infoExtras = listInfoEmpleado;
                                TimbrarXmlSat timbrarXmlSat = new TimbrarXmlSat();
                                List<object> timbreFiscalDigitales = timbrarXmlSat.generaTimbres(infoATimbrar, dbContSimple);
                                if (timbrarXmlSat.error != null)
                                {
                                    if (timbrarXmlSat.error.Trim().Length > 0)
                                    {
                                        mensajeResultado.noError = (100);
                                        mensajeResultado.error = (timbrarXmlSat.error);
                                        //util.mensajeError(rootPane, Utilerias.ObtenerMensaje.getString("CancelaTimbradoMsgErrorWebService"));
                                    }
                                }

                                if (timbreFiscalDigitales == null)
                                {
                                    timbreFiscalDigitales = new List<object>();
                                }
                                BITCancelacion bITCancelacion = new BITCancelacion();
                                CFDIReciboProcCanc CFDIReciboProcCanc = new CFDIReciboProcCanc();
                                int j = 0;
                                for (int i = 0; i < listCFDIEmpleados.Count(); i++)
                                {
                                    while (j < timbreFiscalDigitales.Count())
                                    {
                                        cUUIDCancelado infoCancelado = (cUUIDCancelado)timbreFiscalDigitales[i];
                                        //listCFDIEmpleados.get(i).getCfdiRecibo().getFolioCFDI().equalsIgnoreCase(timbreFiscalDigitales.get(j).getFolio())
                                        if (string.Equals(listCFDIEmpleados[i].cfdiRecibo.UUID, infoCancelado.UUID, StringComparison.OrdinalIgnoreCase))
                                        {

                                            if (string.Equals(infoCancelado.status, "100", StringComparison.OrdinalIgnoreCase) || string.Equals(infoCancelado.status, "201", StringComparison.OrdinalIgnoreCase))
                                            {

                                                if (infoCancelado.Acuse == null)
                                                {
                                                    listCFDIEmpleados[i].cfdiRecibo.statusTimbrado = (StatusTimbrado.EN_PROCESO_CANCELACION);
                                                    listCFDIEmpleados[i].mensaje = "EN PROCESO DE CANCELACION";
                                                    bITCancelacion.fechaYHora = DateTime.Now;
                                                    bITCancelacion.usuario = usuarioActual.nombre;
                                                    bITCancelacion.proceso = ProcesoBitcancelacion.MODIFICACION_CFDIRECIBO;
                                                    bITCancelacion.status = StatusBitcancelacion.BDProcesoCancelacion;
                                                    bITCancelacion.cfdiRecibo_ID = listCFDIEmpleados[i].cfdiRecibo_ID;
                                                    bITCancelacion.statusMsj = "EN PROCESO DE CANCELACION";
                                                    bitCancelacionDAO.agregar(bITCancelacion, dbContSimple);

                                                    /*CFDIReciboProcCanc.serie = listCFDIEmpleados[i].cfdiRecibo.serie;
                                                    CFDIReciboProcCanc.folio = listCFDIEmpleados[i].cfdiRecibo.folioCFDI;
                                                    CFDIReciboProcCanc.fechaIntento = DateTime.Now;
                                                    CFDIReciboProcCanc.cfdiRecibo_ID = listCFDIEmpleados[i].cfdiRecibo_ID;
                                                    cFDIReciboProcCancDAO.agregar(CFDIReciboProcCanc, dbContSimple);*/


                                                }
                                                else
                                                {
                                                    listCFDIEmpleados[i].cfdiRecibo.acuse = (infoCancelado.Acuse);
                                                    listCFDIEmpleados[i].cfdiRecibo.statusTimbrado = (StatusTimbrado.CANCELADO);
                                                    listCFDIEmpleados[i].cfdiRecibo.fechaHoraTimCancelado = DateTime.Now;//leeracuse y tomarfecha cancelado
                                                    listCFDIEmpleados[i].mensaje = "CANCELADO";


                                                    bITCancelacion.fechaYHora = DateTime.Now;
                                                    bITCancelacion.usuario = usuarioActual.nombre;
                                                    bITCancelacion.proceso = ProcesoBitcancelacion.MODIFICACION_CFDIRECIBO;
                                                    bITCancelacion.status = StatusBitcancelacion.BdCancelado;
                                                    bITCancelacion.cfdiRecibo_ID = listCFDIEmpleados[i].cfdiRecibo_ID;
                                                    bITCancelacion.statusMsj = "CANCELADO";
                                                    bitCancelacionDAO.agregar(bITCancelacion, dbContSimple);

                                                    CFDIReciboProcCanc lCfdiRecProcCanc = listCFDIEmpleados[i].cfdiRecibo.cfdiReciboProcCanc[0];
                                                    cFDIReciboProcCancDAO.eliminar(lCfdiRecProcCanc, dbContSimple);



                                                }

                                            }
                                            else
                                            {
                                                listCFDIEmpleados[i].mensaje = (infoCancelado.observaciones);
                                            }

                                            timbreFiscalDigitales.RemoveAt(j);
                                            break;
                                        }
                                        else
                                        {
                                            j++;
                                        }
                                    }
                                    j = 0;
                                    if (!timbreFiscalDigitales.Any())
                                    {
                                        break;
                                    }
                                }
                            }

                        }


                        if (listCFDIEmpleados != null)
                        {
                            if (listCFDIEmpleados.Count > 0)
                            {
                                List<CFDIEmpleado> listCFDIGuardar = new List<CFDIEmpleado>();
                                int i = 0;
                                while (i < listCFDIEmpleados.Count)
                                {
                                    listCFDIEmpleados[i].statusGeneraInfo = false;
                                    if (listCFDIEmpleados[i].cfdiRecibo.statusTimbrado == StatusTimbrado.CANCELADO)
                                    {
                                        listCFDIGuardar.Add(listCFDIEmpleados[i]);
                                        listCFDIEmpleados.RemoveAt(i);
                                    }
                                    i++;

                                }

                                if (listCFDIGuardar.Count > 0)
                                {
                                    mensajeResultado = cfdiEmpleadoDAO.saveDeleteCFDIEmpleado(listCFDIGuardar, null, 50, dbContSimple);
                                    if (mensajeResultado.noError != 0)
                                    {
                                        //util.mensajeError(getView(), mensajeDB.getError());
                                        //return;
                                        return mensajeResultado;
                                    }
                                    listCFDIEmpleados.AddRange(listCFDIGuardar);
                                }
                            }
                        }

                        if (mensajeResultado.resultado != null)
                        {
                            if (mensajeResultado.noError == 0)
                            {

                                List<CFDIEmpleado> cfdiEmpleado = listCFDIEmpleados;
                                var datos = cfdiEmpleado.Select(d => new
                                {
                                    d.id,
                                    d.CLABE,
                                    d.CURP,
                                    d.RFC,
                                    d.antiguedad,
                                    d.antiguedadYMD,
                                    d.apellidoMaterno,
                                    d.apellidoPaterno,
                                    d.calle,
                                    d.ciudad,
                                    d.claveBancoSat,
                                    d.codigoPostal,
                                    d.colonia,
                                    d.correoElectronico,
                                    d.cuentaBancaria,
                                    d.departamento,
                                    d.estado,
                                    d.fechaFinalPago,
                                    d.fechaInicioPago,
                                    d.fechaInicioRelLaboral,
                                    d.fechaPago,
                                    d.formaPago,
                                    d.jornada,
                                    d.municipio,
                                    d.noExterior,
                                    d.noInterior,
                                    d.noRegistroPatronal,
                                    d.noSeguroSocial,
                                    d.nombre,
                                    d.numeroDiasPago,
                                    d.pais,
                                    d.periodiciadadPago,
                                    d.puesto,
                                    d.regimenContratacion,
                                    d.riesgoPuesto,
                                    d.salBaseCotAport,
                                    d.salIntIMSS,
                                    d.tipoContrato,
                                    tipoNominaID = d.tipoNomina.id,
                                    tipoNominaClave = d.tipoNomina.clave,
                                    tipoCorrida_ID = d.tipoCorrida.id,
                                    tipoCorridaClave = d.tipoCorrida.clave,
                                    razonesSociales_ID = d.razonesSociales.id,
                                    periodosNomina_ID = d.periodosNomina.id,
                                    periodoNominaClave = d.periodosNomina.clave,
                                    plazaPorEmpleadoMov_ID = d.plazasPorEmpleadosMov.id,
                                    d.statusGeneraInfo,
                                    d.asignoFolio,
                                    d.recienTimbrado,
                                    d.mensaje,
                                    clave = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                                    d.cfdiRecibo_ID,
                                    cfdiRecibo = new
                                    {
                                        d.cfdiRecibo.id,
                                        d.cfdiRecibo.UUID,
                                        d.cfdiRecibo.serie,
                                        d.cfdiRecibo.acuse,
                                        d.cfdiRecibo.UUIDRelacionado,
                                        d.cfdiRecibo.cadenaCertificado,
                                        d.cfdiRecibo.cadenaOriginalTimbrado,
                                        d.cfdiRecibo.fechaGeneraInfo,
                                        d.cfdiRecibo.fechaHoraTimCancelado,
                                        d.cfdiRecibo.fechaEmision,
                                        d.cfdiRecibo.folioCFDI,
                                        d.cfdiRecibo.leyenda,
                                        d.cfdiRecibo.motivoCancelacion,
                                        d.cfdiRecibo.noCertificado,
                                        d.cfdiRecibo.noCertificadoSAT,
                                        d.cfdiRecibo.rfcProvCertif,
                                        d.cfdiRecibo.sello,
                                        d.cfdiRecibo.selloCFD,
                                        d.cfdiRecibo.selloSAT,
                                        d.cfdiRecibo.serieCFDI,
                                        d.cfdiRecibo.statusTimbrado,
                                        d.cfdiRecibo.statusXmlSat,
                                        d.cfdiRecibo.total,
                                        d.cfdiRecibo.version,
                                        d.cfdiRecibo.xmlTimbrado,
                                        d.cfdiRecibo.certificadoTimbrado,
                                        d.cfdiRecibo.fechaHoraTimbrado,
                                        d.cfdiRecibo.selloTimbrado,
                                        d.cfdiRecibo.mensajeRec,
                                        cfdiReciboConcepto = d.cfdiRecibo.cfdiReciboConcepto.Select(r => new
                                        {
                                            r.id,
                                            r.claveConcepto,
                                            r.claveSAT,
                                            r.descripcionConcepto,
                                            r.importeExento,
                                            r.importeGravable,
                                            r.otroPago,
                                            r.tipoNaturaleza
                                        }).ToList(),
                                        cfdiReciboHrsExtras = d.cfdiRecibo.cfdiReciboHrsExtras.Select(h => new
                                        {
                                            h.id,
                                            h.dias,
                                            h.horasExtras,
                                            h.importeExento,
                                            h.importeGravable
                                        }).ToList(),
                                        cfdiReciboIncapacidad = d.cfdiRecibo.cfdiReciboIncapacidad.Select(i => new
                                        {
                                            i.id,
                                            i.diasIncapacidad,
                                            i.importeMonetario,
                                            i.tipoIncapacidad
                                        }).ToList()
                                    }
                                }).ToList();


                                //inicializaVariableMensaje();

                                mensajeResultado.resultado = datos;
                                mensajeResultado.noError = 0;
                                mensajeResultado.error = "";





                            }
                            else
                            {
                                List<CFDIEmpleado> cfdiEmpleado = (List<CFDIEmpleado>)mensajeResultado.resultado;

                                var datos = cfdiEmpleado.Select(d => new
                                {
                                    d.id,
                                    d.CLABE,
                                    d.CURP,
                                    d.RFC,
                                    d.antiguedad,
                                    d.antiguedadYMD,
                                    d.apellidoMaterno,
                                    d.apellidoPaterno,
                                    d.calle,
                                    d.ciudad,
                                    d.claveBancoSat,
                                    d.codigoPostal,
                                    d.colonia,
                                    d.correoElectronico,
                                    d.cuentaBancaria,
                                    d.departamento,
                                    d.estado,
                                    d.fechaFinalPago,
                                    d.fechaInicioPago,
                                    d.fechaInicioRelLaboral,
                                    d.fechaPago,
                                    d.formaPago,
                                    d.jornada,
                                    d.municipio,
                                    d.noExterior,
                                    d.noInterior,
                                    d.noRegistroPatronal,
                                    d.noSeguroSocial,
                                    d.nombre,
                                    d.numeroDiasPago,
                                    d.pais,
                                    d.periodiciadadPago,
                                    d.puesto,
                                    d.regimenContratacion,
                                    d.riesgoPuesto,
                                    d.salBaseCotAport,
                                    d.salIntIMSS,
                                    d.tipoContrato,
                                    tipoNominaID = d.tipoNomina.id,
                                    tipoNominaClave = d.tipoNomina.clave,
                                    tipoCorrida_ID = d.tipoCorrida.id,
                                    tipoCorridaClave = d.tipoCorrida.clave,
                                    razonesSociales_ID = d.razonesSociales.id,
                                    periodosNomina_ID = d.periodosNomina.id,
                                    periodoNominaClave = d.periodosNomina.clave,
                                    plazaPorEmpleadoMov_ID = d.plazasPorEmpleadosMov.id,
                                    d.statusGeneraInfo,
                                    d.asignoFolio,
                                    d.recienTimbrado,
                                    d.mensaje,
                                    clave = d.plazasPorEmpleadosMov.plazasPorEmpleado.empleados.clave,
                                    d.cfdiRecibo_ID,
                                    cfdiRecibo = new
                                    {
                                        d.cfdiRecibo.id,
                                        d.cfdiRecibo.UUID,
                                        d.cfdiRecibo.serie,
                                        d.cfdiRecibo.acuse,
                                        d.cfdiRecibo.UUIDRelacionado,
                                        d.cfdiRecibo.cadenaCertificado,
                                        d.cfdiRecibo.cadenaOriginalTimbrado,
                                        d.cfdiRecibo.fechaGeneraInfo,
                                        d.cfdiRecibo.fechaHoraTimCancelado,
                                        d.cfdiRecibo.fechaEmision,
                                        d.cfdiRecibo.folioCFDI,
                                        d.cfdiRecibo.leyenda,
                                        d.cfdiRecibo.motivoCancelacion,
                                        d.cfdiRecibo.noCertificado,
                                        d.cfdiRecibo.noCertificadoSAT,
                                        d.cfdiRecibo.rfcProvCertif,
                                        d.cfdiRecibo.sello,
                                        d.cfdiRecibo.selloCFD,
                                        d.cfdiRecibo.selloSAT,
                                        d.cfdiRecibo.serieCFDI,
                                        d.cfdiRecibo.statusTimbrado,
                                        d.cfdiRecibo.statusXmlSat,
                                        d.cfdiRecibo.total,
                                        d.cfdiRecibo.version,
                                        d.cfdiRecibo.xmlTimbrado,
                                        d.cfdiRecibo.certificadoTimbrado,
                                        d.cfdiRecibo.fechaHoraTimbrado,
                                        d.cfdiRecibo.selloTimbrado,
                                        d.cfdiRecibo.mensajeRec,
                                        cfdiReciboConcepto = d.cfdiRecibo.cfdiReciboConcepto.Select(r => new
                                        {
                                            r.id,
                                            r.claveConcepto,
                                            r.claveSAT,
                                            r.descripcionConcepto,
                                            r.importeExento,
                                            r.importeGravable,
                                            r.otroPago,
                                            r.tipoNaturaleza
                                        }).ToList(),
                                        cfdiReciboHrsExtras = d.cfdiRecibo.cfdiReciboHrsExtras.Select(h => new
                                        {
                                            h.id,
                                            h.dias,
                                            h.horasExtras,
                                            h.importeExento,
                                            h.importeGravable
                                        }).ToList(),
                                        cfdiReciboIncapacidad = d.cfdiRecibo.cfdiReciboIncapacidad.Select(i => new
                                        {
                                            i.id,
                                            i.diasIncapacidad,
                                            i.importeMonetario,
                                            i.tipoIncapacidad
                                        }).ToList()
                                    }
                                }).ToList();




                                mensajeResultado.resultado = datos;
                                mensajeResultado.noError = 0;
                                mensajeResultado.error = "";
                            }
                        }


                    }




                }

            }
            catch (Exception)
            {

                throw;
            }









            return mensajeResultado;
        }

        private bool existeCertificadoVigente(DBContextAdapter dbContextSimple)
        {
            CertificadosDAO cert = new CertificadosDAO();
            bool resultado = false;

            try
            {
                inicializaVariableMensaje();

                mensajeResultado = cert.certificadoActual(idRazSocial, dbContextSimple);

                if (mensajeResultado.resultado != null)
                {
                    resultado = true;
                }
                else
                {
                    if (mensajeResultado.noError == 10)
                        resultado = false;
                }




            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("existeCertificadoVigente()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }







            return resultado;
        }

        private int ultimofolio(string serie, int folioIni, int longitudFolio)
        {

            Object valor;
            int folio = 0;

            RazonesSociales razonSocial = razonesSocialesActual;

            if (serie == null ? true : serie.Trim().Length == 0)
            {


                serie = razonSocial.series.serie;
                folioIni = razonSocial.series.folioInicial;
                longitudFolio = razonSocial.series.longitudFolio;
            }

            if (serie == null ? true : serie.Trim().Length == 0)
            {
                valor = (from o in dbContSimple.context.Set<CFDIRecibo>()
                         select new { o.folioCFDI }).Max(p => p.folioCFDI);

            }
            else
            {
                valor = (from o in dbContSimple.context.Set<CFDIRecibo>()
                         where o.serieCFDI == serie
                         select new { o.folioCFDI }).Max(p => p.folioCFDI);
            }

            if (valor == null)
            {
                folio = tipoNomina.series.folioInicial;
                if (folio == 0)
                {
                    folio = 1;

                }
            }
            else
            {
                folio = int.Parse(valor.ToString());
               
            }
            

            return folio;
        }

        private string datoMascara(string serie, int folioIni, int longitudFolio)
        {
            string folio = "";
            string mascara = "";
            string mascaraFin = "";
            for (int k = 0; k < longitudFolio; k++)
            {
                mascara = mascara + "#";
            }
            if (folioIni.ToString().Length < mascara.Length)
            {
                string sub = mascara.Substring(0,( mascara.Length -folioIni.ToString().Length));
                sub = sub.Replace('#', '0');

                mascaraFin = sub;
                for (int k = 0; k < (mascara.Length - sub.Length); k++)
                {
                    mascaraFin = mascaraFin +  "#";
                }
                

                folio = String.Format("{0:" + mascaraFin + "}", folioIni);


            }



            


            return folio;
        }

        

    }

    

    


}
