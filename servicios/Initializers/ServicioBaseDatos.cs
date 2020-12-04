/**
* @author: Beatriz Baldenebro 
* Fecha de Creación: 24/01/2018
* Compañía: Macropro
* Descripción del programa: clase ServicioBaseDatos para llamados a metodos de Entity
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
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Validation;
using System.Data.Common;
using Exitosw.Payroll.Entity.entidad;
using Exitosw.Payroll.Core.util;
using Exitosw.Payroll.Entity.entidad.contabilidad;
using Exitosw.Payroll.Core.servicios.extras;

namespace Exitosw.Payroll.Core.servicios.Initializers
{
    public class ServicioBaseDatos
    {

        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<Paises> listPaises = new List<Paises>();
        List<Estados> listEstados = new List<Estados>();
        List<Municipios> listMunicipios = new List<Municipios>();
        List<Ciudades> listCiudades = new List<Ciudades>();
        List<Cp> listCp = new List<Cp>();
        private Mensaje mensaje;


        public Mensaje inicializarMaster(DbContext dbContextMaster)
        {

            mensaje = new Mensaje();
            DbContext dbContext = dbContextMaster;
            List<TipoHerramienta> listTipoHerramienta = new List<TipoHerramienta>();
            TipoHerramienta tipoHerramienta = null;
            List<TipoElemento> listTipoElementos = new List<TipoElemento>();
            TipoElemento tipoElemento = null;
            List<Perfiles> listPerfiles = new List<Perfiles>();
            Perfiles perfiles = null;
            List<Usuario> listUsuario = new List<Usuario>();
            Usuario usuario = null;
            List<Apariencia> listApariencia = new List<Apariencia>();
            Apariencia apariencia = null;
            List<RazonSocial> listRazonSocial = new List<RazonSocial>();
            RazonSocial razonSocial = null;
            List<RazonSocialConfiguracion> listRazonSocialConfiguracion = new List<RazonSocialConfiguracion>();
            RazonSocialConfiguracion razonSocialConfiguracion = null;
            RegimenFiscal regimenFiscal = null;
            List<RegimenFiscal> listRegimenFiscal = new List<RegimenFiscal>();
            List<Herramienta> listHerramienta = new List<Herramienta>();
            Herramienta herramienta = null;
            List<Contenedor> listContenedor = new List<Contenedor>();
            Contenedor contenedor = null;
            List<Sistemas> listSistemas = new List<Sistemas>();
            Sistemas sistemas = null;
            List<Modulo> listModulo = new List<Modulo>();
            Modulo modulo = null;
            List<Ventana> listVentana = new List<Ventana>();
            Ventana ventana = null;
            List<TipoTabla> listTipoTabla = new List<TipoTabla>();
            TipoTabla tipoTabla = null;
            List<Parametros> listParametros = new List<Parametros>();
            Parametros parametros = null;
            List<TablaBase> listTablaBase = new List<TablaBase>();
            TablaBase tablaBase = null;
            List<ElementosAplicacion> listElementosAplicacion = new List<ElementosAplicacion>();
            ElementosAplicacion elementosAplicacion = null;
            List<HerramientaPersonalizada> listHerramientaPer = new List<HerramientaPersonalizada>();
            HerramientaPersonalizada herramientaPer = null;
            List<ContenedorPersonalizado> listContenedorPersonalizado = new List<ContenedorPersonalizado>();
            ContenedorPersonalizado contenedorPersonalizado = null;
            List<TablaDatos> listTablaDatos = new List<TablaDatos>();
            TablaDatos tablaDatos = null;
            int i = 0, contador = 0;
            try
            {
                dbContext.Database.BeginTransaction();

                //IQueryable<TipoElemento> sentecia = (from t in dbContext.Set<TipoElemento>()
                //   where t.id == 2
                //   select t);

                /*evalua si ya esta inicializada base de datos*/
                bool existe = (from co in dbContext.Set<Contenedor>()
                               select co).Count() > 0 ? true : false;
                if (existe)
                {
                    mensaje.resultado = "This database has already been created.";
                    mensaje.noError = 0;
                    mensaje.error = "Existe";
                    dbContext.Database.CurrentTransaction.Commit();
                    return mensaje;
                }

                #region APARIENCIA
                apariencia = new Apariencia();
                apariencia.controlApariencia =  1;
                apariencia.tema = 0;
                apariencia.iconos = 4;
                apariencia.permiteUsuarioTema = false;
                apariencia.permiteUsuarioIconos = false;
                apariencia.mostrarBordes = false;

                listApariencia.Add(apariencia);

                for (i = 0; i < listApariencia.Count; i++)
                {
                    dbContext.Set<Apariencia>().Add(listApariencia[i]);
                }

                contador = contador + listApariencia.Count;
                /*if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region TIPO DE HERRAMIENTA  
                /*TIPO DE HERRAMIENTA */

                tipoHerramienta = new TipoHerramienta();
                tipoHerramienta.id = 1;
                tipoHerramienta.nombre = "Inicio";
                listTipoHerramienta.Add(tipoHerramienta);

                tipoHerramienta = new TipoHerramienta();
                tipoHerramienta.id = 2;
                tipoHerramienta.nombre = "Catalogos";
                listTipoHerramienta.Add(tipoHerramienta);

                tipoHerramienta = new TipoHerramienta();
                tipoHerramienta.id = 3;
                tipoHerramienta.nombre = "Configuración";
                listTipoHerramienta.Add(tipoHerramienta);

                for (i = 0; i < listTipoHerramienta.Count; i++)
                {

                    dbContext.Set<TipoHerramienta>().Add(listTipoHerramienta[i]);

                }

                contador = contador + listTipoHerramienta.Count;
                /* if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region TIPO ELEMENTO
                /*TIPO ELEMENTO --externo: 0=false, 1=true */

                tipoElemento = new TipoElemento();
                tipoElemento.externo = false;
                tipoElemento.nombre = "JMenu";
                listTipoElementos.Add(tipoElemento);

                tipoElemento = new TipoElemento();
                tipoElemento.externo = false;
                tipoElemento.nombre = "JMenuItem";
                listTipoElementos.Add(tipoElemento);

                tipoElemento = new TipoElemento();
                tipoElemento.externo = false;
                tipoElemento.nombre = "JRadioButtonMenuItem";
                listTipoElementos.Add(tipoElemento);

                tipoElemento = new TipoElemento();
                tipoElemento.externo = false;
                tipoElemento.nombre = "JCheckBoxMenuItem";
                listTipoElementos.Add(tipoElemento);

                tipoElemento = new TipoElemento();
                tipoElemento.externo = false;
                tipoElemento.nombre = "JSeparator";
                listTipoElementos.Add(tipoElemento);

                tipoElemento = new TipoElemento();
                tipoElemento.externo = false;
                tipoElemento.nombre = "JButton";
                listTipoElementos.Add(tipoElemento);

                tipoElemento = new TipoElemento();
                tipoElemento.externo = false;
                tipoElemento.nombre = "JCheckBox";
                listTipoElementos.Add(tipoElemento);

                tipoElemento = new TipoElemento();
                tipoElemento.externo = false;
                tipoElemento.nombre = "JRadioButton";
                listTipoElementos.Add(tipoElemento);

                tipoElemento = new TipoElemento();
                tipoElemento.externo = true;
                tipoElemento.nombre = "externo";
                listTipoElementos.Add(tipoElemento);

                for (i = 0; i < listTipoElementos.Count; i++)
                {
                    dbContext.Set<TipoElemento>().Add(listTipoElementos[i]);
                }

                contador = contador + listTipoElementos.Count;
                /* if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region PERFILES
                /*Perfiles Niveles de acceso: 0=Sistema 1=Administrador 2=Usuario */
                perfiles = new Perfiles();
                perfiles.clave = "01";
                perfiles.nombre = "Sistema";
                perfiles.nivelAccesoSistema = ((byte)0);
                listPerfiles.Add(perfiles);

                perfiles = new Perfiles();
                perfiles.clave = "02";
                perfiles.nombre = "Administrador";
                perfiles.nivelAccesoSistema = ((byte)1);
                listPerfiles.Add(perfiles);

                perfiles = new Perfiles();
                perfiles.clave = "03";
                perfiles.nombre = "Operador";
                perfiles.nivelAccesoSistema = ((byte)2);
                listPerfiles.Add(perfiles);

                for (i = 0; i < listPerfiles.Count; i++)
                {
                    dbContext.Set<Perfiles>().Add(listPerfiles[i]);
                }

                contador = contador + listPerfiles.Count;
                /* if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region PERFILES
                /*Usuarios Activa Fecha Ex False=0, True=1 */
                usuario = new Usuario();
                usuario.activaFechaEx = false;
                usuario.clave = "0001";
                usuario.nombre = "Sistemas";
                usuario.password = "827CCB0EEA8A706C4C34A16891F84E7B";
                usuario.perfiles_ID = (listPerfiles[0].id);
                listUsuario.Add(usuario);

                usuario = new Usuario();
                usuario.activaFechaEx = false;
                usuario.clave = "0002";
                usuario.nombre = "Administrador";
                usuario.password = "827CCB0EEA8A706C4C34A16891F84E7B";
                usuario.perfiles_ID = (listPerfiles[1].id);
                listUsuario.Add(usuario);

                usuario = new Usuario();
                usuario.activaFechaEx = false;
                usuario.clave = "0003";
                usuario.nombre = "Operador";
                usuario.password = "827CCB0EEA8A706C4C34A16891F84E7B";
                usuario.perfiles_ID = (listPerfiles[2].id);
                listUsuario.Add(usuario);

                for (i = 0; i < listUsuario.Count; i++)
                {

                    dbContext.Set<Usuario>().Add(listUsuario[i]);
                }

                contador = contador + listUsuario.Count;
                /*if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region RAZONSOCIAL
                razonSocial = new RazonSocial();
                razonSocial.claveRazonSocial = "0001";
                razonSocial.nombreRazonSocial = "Empresa Prueba";
                listRazonSocial.Add(razonSocial);

                listRazonSocial.Add(razonSocial);

                for (i = 0; i < listRazonSocial.Count; i++)
                {
                    dbContext.Set<RazonSocial>().Add(listRazonSocial[i]);
                }

                contador = contador + listRazonSocial.Count;
                /*if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region RazonSocialConfiguracion
                razonSocialConfiguracion = new RazonSocialConfiguracion();
                razonSocialConfiguracion.razonSocial_ID = listRazonSocial[0].id;
                razonSocialConfiguracion.permitido = false;
                razonSocialConfiguracion.usuario_ID = listUsuario[0].id;
                listRazonSocialConfiguracion.Add(razonSocialConfiguracion);

                razonSocialConfiguracion = new RazonSocialConfiguracion();
                razonSocialConfiguracion.razonSocial_ID = listRazonSocial[0].id;
                razonSocialConfiguracion.permitido = false;
                razonSocialConfiguracion.usuario_ID = listUsuario[1].id;
                listRazonSocialConfiguracion.Add(razonSocialConfiguracion);

                razonSocialConfiguracion = new RazonSocialConfiguracion();
                razonSocialConfiguracion.razonSocial_ID = listRazonSocial[0].id;
                razonSocialConfiguracion.permitido = true;
                razonSocialConfiguracion.usuario_ID = listUsuario[2].id;
                listRazonSocialConfiguracion.Add(razonSocialConfiguracion);

                for (i = 0; i < listRazonSocialConfiguracion.Count; i++)
                {
                    dbContext.Set<RazonSocialConfiguracion>().Add(listRazonSocialConfiguracion[i]);
                }

                contador = contador + listRazonSocialConfiguracion.Count;
                /*if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region REGIMEN FISCAL
                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("601");
                regimenFiscal.descripcion = ("General de Ley Personas Morales");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("603");
                regimenFiscal.descripcion = ("Personas Morales con Fines no Lucrativos");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("605");
                regimenFiscal.descripcion = ("Sueldos y Salarios e Ingresos Asimilados a Salarios");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("606");
                regimenFiscal.descripcion = ("Arrendamiento");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("607");
                regimenFiscal.descripcion = ("Régimen de Enajenación o Adquisición de Bienes");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("608");
                regimenFiscal.descripcion = ("Demás ingresos");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("609");
                regimenFiscal.descripcion = ("Consolidación");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("610");
                regimenFiscal.descripcion = ("Residentes en el Extranjero sin Establecimiento Permanente en México");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("611");
                regimenFiscal.descripcion = ("Ingresos por Dividendos (socios y accionistas)");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("612");
                regimenFiscal.descripcion = ("Personas Físicas con Actividades Empresariales y Profesionales");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("614");
                regimenFiscal.descripcion = ("Ingresos por intereses");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("615");
                regimenFiscal.descripcion = ("Régimen de los ingresos por obtención de premios");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("616");
                regimenFiscal.descripcion = ("Sin obligaciones fiscales");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("620");
                regimenFiscal.descripcion = ("Sociedades Cooperativas de Producción que optan por diferir sus ingresos");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("621");
                regimenFiscal.descripcion = ("Incorporación Fiscal");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("622");
                regimenFiscal.descripcion = ("Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("623");
                regimenFiscal.descripcion = ("Opcional para Grupos de Sociedades");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("624");
                regimenFiscal.descripcion = ("Coordinados");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("625");
                regimenFiscal.descripcion = ("Régimen de las Actividades Empresariales con ingresos a través de Plataformas Tecnológicas");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("628");
                regimenFiscal.descripcion = ("Hidrocarburos");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("629");
                regimenFiscal.descripcion = ("De los Regímenes Fiscales Preferentes y de las Empresas Multinacionales");
                listRegimenFiscal.Add(regimenFiscal);

                regimenFiscal = new RegimenFiscal();
                regimenFiscal.clave = ("630");
                regimenFiscal.descripcion = ("Enajenación de acciones en bolsa de valores");
                listRegimenFiscal.Add(regimenFiscal);

                listRegimenFiscal.Add(regimenFiscal);

                for (i = 0; i < listRegimenFiscal.Count; i++)
                {
                    dbContext.Set<RegimenFiscal>().Add(listRegimenFiscal[i]);
                }
                /* contador = contador + listModulo.Count;
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/

                contador = contador + listRegimenFiscal.Count;

                dbContext.SaveChanges();
                #endregion

                #region HERRAMIENTA
                herramienta = new Herramienta();
                herramienta.id = 1;
                herramienta.habilitado = (true);
                herramienta.nombre = ("Domicilio");
                herramienta.visible = (true);
                herramienta.tipoHerramienta_ID = (listTipoHerramienta[1].id);
                listHerramienta.Add(herramienta);

                herramienta = new Herramienta();
                herramienta.id = 2;
                herramienta.habilitado = (true);
                herramienta.nombre = ("Nómina");
                herramienta.visible = (true);
                herramienta.tipoHerramienta_ID = (listTipoHerramienta[1].id);
                listHerramienta.Add(herramienta);

                herramienta = new Herramienta();
                herramienta.id = 3;
                herramienta.habilitado = (true);
                herramienta.nombre = ("Empresas");
                herramienta.visible = (true);
                herramienta.tipoHerramienta_ID = (listTipoHerramienta[2].id);
                listHerramienta.Add(herramienta);

                for (i = 0; i < listHerramienta.Count; i++)
                {
                    dbContext.Set<Herramienta>().Add(listHerramienta[i]);
                }

                contador = contador + listHerramienta.Count;
                /* if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region SISTEMAS
                /*SISTEMAS*/
                sistemas = new Sistemas();
                sistemas.nombre = ("NRH");
                sistemas.clave = ("1");
                listSistemas.Add(sistemas);

                sistemas = new Sistemas();
                sistemas.nombre = ("ERP");
                sistemas.clave = ("2");
                listSistemas.Add(sistemas);

                for (i = 0; i < listSistemas.Count; i++)
                {
                    dbContext.Set<Sistemas>().Add(listSistemas[i]);
                }

                contador = contador + listSistemas.Count;
                /* if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region VENTANA
                ventana = new Ventana();
                ventana.clave = (1);
                ventana.nombre = ("AbrirPeriodosNomina");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (2);
                ventana.nombre = ("RegistroAhorro");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (3);
                ventana.nombre = ("Asistencia");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (4);
                ventana.nombre = ("CalculoDeNomina");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (5);
                ventana.nombre = ("CalculoSDI");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (6);
                ventana.nombre = ("CapturaMovimientosNomina");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (7);
                ventana.nombre = ("CatalogoBancos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (8);
                ventana.nombre = ("CatalogoCategoriasPuestos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (9);
                ventana.nombre = ("CatalogoCentroDeCostos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (10);
                ventana.nombre = ("CatalogoCiudades");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (11);
                ventana.nombre = ("CatalogoConceptosDeNomina");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (12);
                ventana.nombre = ("CatalogoConceptosPorTipoCorrida");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (13);
                ventana.nombre = ("CatalogoCp");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (14);
                ventana.nombre = ("DefinirParametros");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (15);
                ventana.nombre = ("CatalogoDepartamentos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (16);
                ventana.nombre = ("CatalogoEmpleados");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (17);
                ventana.nombre = ("CatalogoEstados");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);
                ventana = new Ventana();

                ventana.clave = (18);
                ventana.nombre = ("CatalogoGrupos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (19);
                ventana.nombre = ("CatalogoHorario");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (20);
                ventana.nombre = ("CatalogoModulos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (21);
                ventana.nombre = ("CatalogoMonedas");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (22);
                ventana.nombre = ("CatalogoMunicipios");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (23);
                ventana.nombre = ("CatalogoNivelesClasificacion");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (24);
                ventana.nombre = ("CatalogoPaises");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (25);
                ventana.nombre = ("CatalogoPerfiles");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGODIALOG);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (26);
                ventana.nombre = ("CatalogoPeriodicidad");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (27);
                ventana.nombre = ("CatalogoPosicionOrganigrama");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (28);
                ventana.nombre = ("CatalogoPuestos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (29);
                ventana.nombre = ("CatalogoRazonSocial");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (30);
                ventana.nombre = ("CatalogoRegistroPatronal");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (31);
                ventana.nombre = ("CatalogoSistemas");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (32);
                ventana.nombre = ("TablaCruce");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (33);
                ventana.nombre = ("CatalogoTipoCentroCostos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (34);
                ventana.nombre = ("CatalogoTipoNomina");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (35);
                ventana.nombre = ("CapturaTiposDeCambio");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (36);
                ventana.nombre = ("CatalogoTiposDeRedondeo");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (37);
                ventana.nombre = ("CatalogoTurnos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (38);
                ventana.nombre = ("CatalogoUsuarios");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (39);
                ventana.nombre = ("CerrarPeriodosNomina");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (40);
                ventana.nombre = ("ConfigFiniquitosLiquidacion");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (41);
                ventana.nombre = ("ConfiguracionAhorro");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (42);
                ventana.nombre = ("ConfiguracionAsistencia");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (43);
                ventana.nombre = ("ConfiguracionCredito");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (44);
                ventana.nombre = ("ConfiguracionDespensa");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (45);
                ventana.nombre = ("ConfiguracionElementosAplicacion");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (46);
                ventana.nombre = ("ConfiguracionesIMSS");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (47);
                ventana.nombre = ("ConfiguracionInicialModoCatalogo");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (48);
                ventana.nombre = ("ConfiguracionPeriodosNomina");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (49);
                ventana.nombre = ("ConfigurarConsultas");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();  //---no
                ventana.clave = (50);
                ventana.nombre = ("ConstruyeConsulta");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONSULTA);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (51); //--no
                ventana.nombre = ("ConstruyeReporte");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (52);
                ventana.nombre = ("RegistroCredito");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (53);
                ventana.nombre = ("GeneraTimbrado");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (54);
                ventana.nombre = ("DefinirConceptosEspeciales");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (55);   //---no
                ventana.nombre = ("DefinirParametros");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (56);
                ventana.nombre = ("CatalogoDefinirTablas");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (57);
                ventana.nombre = ("EliminaSaldos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (58);
                ventana.nombre = ("CancelaTimbrado");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (59);
                ventana.nombre = ("GridTipoTabla");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.GRID);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (60);
                ventana.nombre = ("HistorialAhorros");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (61);
                ventana.nombre = ("HistorialCreditos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (62);
                ventana.nombre = ("CatalogoTiposVacaciones");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (63);
                ventana.nombre = ("MenuDinamicoSistema");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (64);
                ventana.nombre = ("MovimientosDeNomina");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (65);
                ventana.nombre = ("NuevasPlazasYReingresos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (66);
                ventana.nombre = ("PromocionModificacionPlazas");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (67);
                ventana.nombre = ("RegistrarIncapacidades");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (68);
                ventana.nombre = ("ConfiguracionVacaciones");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();  //---no
                ventana.clave = (69);
                ventana.nombre = ("ConfiguracionTimbrado");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (70);
                ventana.nombre = ("TablaValores");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.BASEFORM);
                listVentana.Add(ventana);

                ventana = new Ventana();             //--- no
                ventana.clave = (71);
                ventana.nombre = ("ReporteNominas");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.REPORTE);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (72);
                ventana.nombre = ("FiniquitosNormal");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (73);
                ventana.nombre = ("FiniquitosComplementario");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (74);
                ventana.nombre = ("FiniquitosProyeccion");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (75);
                ventana.nombre = ("LiquidacionesNormal");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (76);
                ventana.nombre = ("LiquidacionesComplementario");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (77);
                ventana.nombre = ("LiquidacionesProyeccion");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (78);
                ventana.nombre = ("GeneracionDatosTimbre");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (79);   //---no
                ventana.nombre = ("BajaEmpleado");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (80);
                ventana.nombre = ("ConfigurarReportes");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (81);
                ventana.nombre = ("CatalogoDefinirTablasUsuario");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (82);
                ventana.nombre = ("ConfigAsistenciasExcepciones");   //--no
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (83);
                ventana.nombre = ("ConfigurarTeclasAcceso");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (84);
                ventana.nombre = ("CatalogoParentesco");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (85);
                ventana.nombre = ("CatalogoEstudios");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (86);
                ventana.nombre = ("CatalogoCursos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (87);
                ventana.nombre = ("RegistroAbono");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (88);
                ventana.nombre = ("RegistroDescuento");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (89);
                ventana.nombre = ("RegistroCargos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (90);
                ventana.nombre = ("RegistroBloqueos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (91);
                ventana.nombre = ("RegistroOtros");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (92);
                ventana.nombre = ("ConfiguracionImpresionReportes");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (93);
                ventana.nombre = ("CatalogoTipoCorrida");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (94);
                ventana.nombre = ("CatalogoFirmas");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (95);  ///-no
                ventana.nombre = ("RegistroAbonoAhorro");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana(); ///-no
                ventana.clave = (96);
                ventana.nombre = ("RegistroDescuentoAhorro");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana(); ///-no
                ventana.clave = (97);
                ventana.nombre = ("RegistroCargosAhorro");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana(); ///-no
                ventana.clave = (98);
                ventana.nombre = ("RegistroBloqueosAhorro");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana(); ///-no
                ventana.clave = (99);
                ventana.nombre = ("RegistroOtrosAhorro");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.MOVIMIENTO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (100);
                ventana.nombre = ("ConfiguracionConceptosSat");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (101);
                ventana.nombre = ("CatalogoBaseDeNomina");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (102);
                ventana.nombre = ("ConfiguracionMascaras");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (103);
                ventana.nombre = ("ConfiguracionFoliacion");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (104);
                ventana.nombre = ("ConfiguracionConceptosDIM");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (105);
                ventana.nombre = ("ConsultaRazonesSociales");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (106);
                ventana.nombre = ("ParametroCuentaContable");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (107);
                ventana.nombre = ("CatalagoCuentasContables");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (108);
                ventana.nombre = ("CatalagoFormatoCuentasContables");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (109);
                ventana.nombre = ("GeneradorPolizas");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (110);
                ventana.nombre = ("CalculoPtu");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (111);
                ventana.nombre = ("CatalogoGenero");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (112);
                ventana.nombre = ("Inasistencias");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CATALOGO);
                listVentana.Add(ventana);

                ventana = new Ventana();
                ventana.clave = (113);
                ventana.nombre = ("ConfiguracionAguinaldos");
                ventana.sistemas_ID = (listSistemas[0].id);
                ventana.tipoVentana = (TipoVentana.CONFIGURACION);
                listVentana.Add(ventana);

                for (i = 0; i < listVentana.Count; i++)
                {
                    dbContext.Set<Ventana>().Add(listVentana[i]);
                }

                /*contador = contador + listVentana.Count;
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                //byte[] byteDefaultIcon = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAIGNIUk0AAHolAACAgwAA+f8AAIDpAAB1MAAA6mAAADqYAAAXb5JfxUYAAAnISURBVHja7J1tcFRXGcd/e3OzCSmEt8ZCQqGhVYq2kAGaWqCg2EKhOgVqBZQPVZqC4Ad0+kXHmU5bp35xGD5YKZoyzuiUtwHacRSogrxTCs1owAJDFZoJIAZo3tlk3/zwnIVN9iW7uWc3d9nzn9nZ7Obuvff8//ec55znec45nqo1YVwML1CpXhOBB4AxQBkwDCgFBgGF6ng/4ANagGagCWgELgFngYvq1e3WAtsuFODLwJPATKAKGAsUazq/D2gA/gEcAQ4Dn7hJII8LakgB8FXgOWAu8Ij6LhsIAmeAD4D3gQ/Vd3kpyFjgBeC7wBSXPKB1wLvAdlWT8kKQKmAl8B1ghEub8pvANmCjat6yBiuL15oCbAY+Ala5WAzUva1S97o5mzU4G4KMAd4CjgJLo3pEuYBCdc9HVRnG5LIgnqinbLXGntJAoFiVIVK7PbkmyERgN7ABGM3dg9GqTLtVGXNCkBWqjz+PuxfzVBlXuFmQwcBvgVqXG2ydhr8W+J0qu6sEqVTVuIb8w0uq7JVuEWQa8Dfl6shXzFQcPDbQgnwD2AuMx2A8sEdxMiCCzAN25om9SMeu7HTSoemvIHMQf0+p0SAGpYqbOdkSZJq64BDDfUIMURxNy7Qg44AdpplKufnaoTjLiCAliGt6rOE6ZYxVnJVkQpD1wHTDcdqYrrjTKsgP8nTQpws1ikMtgjwMrDOcOsY6xaUjQWwkDjDU8OkYQxWXthNBavrbnzZIOH6r6a8go4HXDIfa8RpJYkTJBPk5kpBmoBdlitu0BJmEuJW1IByGUFjeDUBxOykdQX6GZBE6QigMnV0QCIHlgWAIOnzynufwKo7j9qLi1Y5FTq8YCIoIy2bD3KlQPgKaWuBAPWw/DB1d4I1zdY8HuvzgD7ib0UFFUj4HWARMBv7ZlyA/clo7giEh+80XYd7UO98/cB889iX4+mRYuxFaOsDulTTqD8CECjnWzThxHtpvgWU5qiVrgJeTCTIGWOL0Zn1++OGzPcWIxrQvwiuL4ae/B9uiR1JNhw++9Th8f667BVn8BjS3g9dZiG8J8DqSoR/XhizFYYwjGIKyUlj4RPLj5k2FylHgD8Y2Wd0ub64i9lEDShXncY26DSx3eoVgCO4thVHD+26Dy0eKrclzLCcqmzNakGplZBwjEEztKe8OSI3Ic0wCpsYTZLGOs9sFcPkGnG9MftzVm/DvK1AYp1tRYLmfRY0PkieaezvqXYsZtTxwqwve/gu8tTrxjW/cDddboaQotqCtnfC/ZncLEghqFeUZNS4JROaHPIpMVtE2xe1Wtxj2V54XmxJBW6eIsekD6RrHK1SBFdsddhu6A1o9DwFkysPpiACz0TzfcJAXdh2DUxdg+kSoGAlNrfDhWbhwRf6f6AlL1QYNJDQ3q7bS4LYgszJx0yVFcO1z2HJQel+WBUV2bDMVr30uyD9jPwv4tQ0U6epdJTLybm9+XIIqoMhCMiNMJsnA435gnAU8RG7PbrpbUAw8aJFC4N0ga3g4UkMM3IGHbKA8k1fwdfcMSFkWFBcal0kClNtkKG4eCkugqXoCPF0FFffK6PvgaTh0RvrxifrywRAEXe50LLQz8lCV2cBw3WcNh2Vwt3YhvDy/Z2Rt6WzYcRTeeBdCodgATygEI4fA0HvcLciVGxlxjg6z0ThhMYLObvje12DVgvj/f34GXG+BdbtiB4ntPlj9TVi5wN2CLHwdLlwGr95lEIZYuru8oRAMLYHlfUzsWjJL3Cnx4iF5bF+KLTQvdREISXBqXB+WadhgiZv7g/GbvDxFoYWBq2Ahy+Jpg22JQ7GhKflxzR1w6RoUFpgmKwp+C1n2Tp/CFrR0wh/2JT9u60GJLMZzPOZxk+WzgXbdZx3khW2H4b7hsd1egJ1HYcOfZYDYG4OLxV2/+5T7u72F+lesbLOBz3Wf1eORJ3/9e3D8LDxVBWPKZGB4oD75wNCy4Eab+0O4GRoYNtvIUqr6jZNHaspH50WUdFwnyUbxdzmabOBKRjvWXgzSaAkt4FPDg2vwqQWcMzy4BuciNcRnuBj4Lm+khjQAnxk+BhyfAQ0W0IUkyWUEgaBMMWjtFE+uP2CYT4A6oCsytDkKLNN9hc4uuL9MEuVGjxCX+4nzfSfKReYkuhkZ6JYfgzvZioeQdEatqaSLpvcvlTQXcrkykEp6MFqQs+r1qBYxumBOFfzyxVjCh5SISO0+2HwgNkDVdgtWPwvLXb5cwYr1cPG/2twnEf5vCxJAtmxwLEgoLIPBVQuSj8ZXzoe/1okA0dU/HIbSEvjCMHcLYhdorSF7lAY95ofs1GXEK0bChD5WSR89Ah4sj2/kc2HatEYxwsCuyIdoQU7Sa4quk6cnlarstc1iAkA9cCqeIH7gjzp6H9dbJUjVl9G/csMkYivO/fEEAdgCtDoVpKkV3jue/Li9HyujWBDbFHht97No6XG9tyrO77QuvQ5oBLbicPW44kKo3QPjR8Wfq/7xBfjVDlU7ehXsnmL40wk4fcndglxr1lK7txI1Rx3ib3k0Gdknw5HjPLK0xqIZMHeKmkHVAn+vh+2HzNIayM5w1b3tdqI9qLagYUWHUFhyewttaZqCIflc7M3bAFTv2rE0pilMcPCbaNjbz/LIwM+2RBzLI02SEYNuxTGpClKP7I2hBR6PiGEy3m+jVnGcsiAAvyBD8fY8R5PilnQFuQq8avjTjlcVt2kLArKdz37DoTbsV5zSX0ECyCJbLYZLLYPANYrTfgsCkgTxE8OnY/yYFBJKUu2Abuqrqhn02fRvSmmokMZJ16LCjAZp4ZjiDt2CdCJbbTcYjlNGg+KsMxOCgKSqfBvZ3togOW4qrtJKseqPE+MksjF9m+E8IdqQ/eJPpvvD/nqV9itRWg33cbu3LwD7+vNjJ26+vchagab56tlMLVbckG1BUE/BM8BFowUXFRf7nJxEhyP8JLLd6JE8FuOI4uCk0xNZGp+O+Wh02ecQalXZtbQSOkNF7Ugs/qU8sSs3VVlr0DhxNhOxu3eAJ50YthzAXlXGd3SfOFPB1E9UNV5NEt9/DuKqKtMCVUZyRRCQFMkNSGbFb8jtWVo+VYZqVaaMJbtmI92gEYkDzECyWfw5JIRf3fMMVYbGTF8wm/kfdcikoGrgbZcb/pvqHqvVPddl68KJ8rKygbGIv2cZsu65G1AHbAa2MUBe7YEUJAIbeBx4Dtmh4REgWynYQeAMMjfmfeAEfYRY80GQaHiBrwAzgSdUzRmHvlXvfIg7vA44rkbY/0JDUuDdKkg8gSqB8ciCz5VABbKS6jBkw99iZFW8kDLCXYjHtRnJgbqsRtHngP+ov7vdWuD/DwAcEYKkQXXFQgAAAABJRU5ErkJggg==");

                #region CONTENEDORES

                #region Menu principal
                contenedor = new Contenedor();
                contenedor.id = (1);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "paises";
                contenedor.nombre = ("Paises");
                contenedor.ordenId = (1);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[0]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);

                contenedor = new Contenedor();
                contenedor.id = (2);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "estados";
                contenedor.nombre = ("Estados");
                contenedor.ordenId = (2);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[0]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);

                contenedor = new Contenedor();
                contenedor.id = (3);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "municipios";
                contenedor.nombre = ("Municipios");
                contenedor.ordenId = (3);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[0]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);

                contenedor = new Contenedor();
                contenedor.id = (4);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "ciudades";
                contenedor.nombre = ("Ciudades");
                contenedor.ordenId = (4);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[0]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);

                contenedor = new Contenedor();
                contenedor.id = (5);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "codigo_postal";
                contenedor.nombre = ("Códigos Postales");
                contenedor.ordenId = (5);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[0]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);

                contenedor = new Contenedor();
                contenedor.id = (6);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "tipos-nominas";
                contenedor.nombre = ("Tipos de nómina");
                contenedor.ordenId = (6);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[1]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);

                contenedor = new Contenedor();
                contenedor.id = (7);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "periosidad";
                contenedor.nombre = ("Periodicidad");
                contenedor.ordenId = (7);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[1]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);

                contenedor = new Contenedor();
                contenedor.id = (8);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "";
                contenedor.nombre = ("Periodos de nómina");
                contenedor.ordenId = (8);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[1]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);

                contenedor = new Contenedor();
                contenedor.id = (9);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "razones_sociales";
                contenedor.nombre = ("Razones Sociales");
                contenedor.ordenId = (9);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[2]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);

                contenedor = new Contenedor();
                contenedor.id = (10);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "registro-patronal";
                contenedor.nombre = ("Registros Patronales");
                contenedor.ordenId = (10);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[2]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);

                contenedor = new Contenedor();
                contenedor.id = (11);
                contenedor.accion = ("---");
                contenedor.habilitado = (true);
                contenedor.nombreIcono = "firmas";
                contenedor.nombre = ("Firmas");
                contenedor.ordenId = (11);
                contenedor.tipoAcciones = (TipoAcciones.VENTANA);
                contenedor.visible = (true);
                contenedor.herramienta = (listHerramienta[2]);
                contenedor.tipoElemento = (null);
                contenedor.ventana = (null);
                listContenedor.Add(contenedor);
                #endregion

                for (i = 0; i < listContenedor.Count; i++)
                {
                    dbContext.Set<Contenedor>().Add(listContenedor[i]);
                }

                /*contador = contador + listContenedor.Count;
                  if (contador % rango == 0 & contador > 0)
                  {
                      session.flush();
                      session.clear();
                      contador = 0;
                  }*/
                dbContext.SaveChanges();
                #endregion

                #region HERRAMIENTAS PERSONALIZADAS
                herramientaPer = new HerramientaPersonalizada();
                herramientaPer.id = 1;
                herramientaPer.habilitado = (true);
                herramientaPer.nombre = ("Mis procesos");
                herramientaPer.visible = (true);
                herramientaPer.usuario_ID = (listUsuario[0].id);
                listHerramientaPer.Add(herramientaPer);

                herramientaPer = new HerramientaPersonalizada();
                herramientaPer.id = 2;
                herramientaPer.habilitado = (true);
                herramientaPer.nombre = ("Mis reportes");
                herramientaPer.visible = (true);
                herramientaPer.usuario_ID = (listUsuario[0].id);
                listHerramientaPer.Add(herramientaPer);

                herramientaPer = new HerramientaPersonalizada();
                herramientaPer.id = 3;
                herramientaPer.habilitado = (true);
                herramientaPer.nombre = ("Mi configuración");
                herramientaPer.visible = (true);
                herramientaPer.usuario_ID = (listUsuario[0].id);
                listHerramientaPer.Add(herramientaPer);

                herramientaPer = new HerramientaPersonalizada();
                herramientaPer.id = 4;
                herramientaPer.habilitado = (true);
                herramientaPer.nombre = ("Mi procesos activos");
                herramientaPer.visible = (true);
                herramientaPer.usuario_ID = (listUsuario[1].id);
                listHerramientaPer.Add(herramientaPer);

                herramientaPer = new HerramientaPersonalizada();
                herramientaPer.id = 4;
                herramientaPer.habilitado = (true);
                herramientaPer.nombre = ("Mi estimaciones");
                herramientaPer.visible = (true);
                herramientaPer.usuario_ID = (listUsuario[1].id);
                listHerramientaPer.Add(herramientaPer);

                for (i = 0; i < listHerramientaPer.Count; i++)
                {
                    dbContext.Set<HerramientaPersonalizada>().Add(listHerramientaPer[i]);
                }

                dbContext.SaveChanges();
                #endregion

                #region CONTENEDORES PERSONALIZADOS
                contenedorPersonalizado = new ContenedorPersonalizado();
                contenedorPersonalizado.id = (1);
                contenedorPersonalizado.ordenId = (1);
                contenedorPersonalizado.herramientaPersonalizada_ID = (listHerramientaPer[0].id);
                contenedorPersonalizado.contenedor_ID = (listContenedor[0].id);
                listContenedorPersonalizado.Add(contenedorPersonalizado);

                contenedorPersonalizado = new ContenedorPersonalizado();
                contenedorPersonalizado.id = (2);
                contenedorPersonalizado.ordenId = (1);
                contenedorPersonalizado.herramientaPersonalizada_ID = (listHerramientaPer[0].id);
                contenedorPersonalizado.contenedor_ID = (listContenedor[1].id);
                listContenedorPersonalizado.Add(contenedorPersonalizado);

                contenedorPersonalizado = new ContenedorPersonalizado();
                contenedorPersonalizado.id = (3);
                contenedorPersonalizado.ordenId = (1);
                contenedorPersonalizado.herramientaPersonalizada_ID = (listHerramientaPer[0].id);
                contenedorPersonalizado.contenedor_ID = (listContenedor[2].id);
                listContenedorPersonalizado.Add(contenedorPersonalizado);

                contenedorPersonalizado = new ContenedorPersonalizado();
                contenedorPersonalizado.id = (4);
                contenedorPersonalizado.ordenId = (1);
                contenedorPersonalizado.herramientaPersonalizada_ID = (listHerramientaPer[1].id);
                contenedorPersonalizado.contenedor_ID = (listContenedor[3].id);
                listContenedorPersonalizado.Add(contenedorPersonalizado);

                contenedorPersonalizado = new ContenedorPersonalizado();
                contenedorPersonalizado.id = (5);
                contenedorPersonalizado.ordenId = (1);
                contenedorPersonalizado.herramientaPersonalizada_ID = (listHerramientaPer[1].id);
                contenedorPersonalizado.contenedor_ID = (listContenedor[3].id);
                listContenedorPersonalizado.Add(contenedorPersonalizado);

                contenedorPersonalizado = new ContenedorPersonalizado();
                contenedorPersonalizado.id = (6);
                contenedorPersonalizado.ordenId = (1);
                contenedorPersonalizado.herramientaPersonalizada_ID = (listHerramientaPer[3].id);
                contenedorPersonalizado.contenedor_ID = (listContenedor[4].id);
                listContenedorPersonalizado.Add(contenedorPersonalizado);

                contenedorPersonalizado = new ContenedorPersonalizado();
                contenedorPersonalizado.id = (7);
                contenedorPersonalizado.ordenId = (1);
                contenedorPersonalizado.herramientaPersonalizada_ID = (listHerramientaPer[3].id);
                contenedorPersonalizado.contenedor_ID = (listContenedor[5].id);
                listContenedorPersonalizado.Add(contenedorPersonalizado);

                contenedorPersonalizado = new ContenedorPersonalizado();
                contenedorPersonalizado.id = (8);
                contenedorPersonalizado.ordenId = (1);
                contenedorPersonalizado.herramientaPersonalizada_ID = (listHerramientaPer[4].id);
                contenedorPersonalizado.contenedor_ID = (listContenedor[6].id);
                listContenedorPersonalizado.Add(contenedorPersonalizado);

                for (i = 0; i < listContenedorPersonalizado.Count; i++)
                {
                    dbContext.Set<ContenedorPersonalizado>().Add(listContenedorPersonalizado[i]);
                }

                dbContext.SaveChanges();
                #endregion

                #region MODULO
                modulo = new Modulo();
                modulo.clave = ("1");
                modulo.nombre = ("Globales");
                modulo.sistemas_ID = (listSistemas[0].id);
                listModulo.Add(modulo);

                modulo = new Modulo();
                modulo.clave = ("2");
                modulo.nombre = ("Nomina");
                modulo.sistemas_ID = (listSistemas[0].id);
                listModulo.Add(modulo);

                modulo = new Modulo();
                modulo.clave = ("3");
                modulo.nombre = ("Pre-Nomina");
                modulo.sistemas_ID = (listSistemas[0].id);
                listModulo.Add(modulo);

                for (i = 0; i < listModulo.Count; i++)
                {
                    dbContext.Set<Modulo>().Add(listModulo[i]);
                }
                /* contador = contador + listModulo.Count;
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region TIPO DE TABLA
                tipoTabla = new TipoTabla();
                tipoTabla.clave = ("1");
                tipoTabla.nombre = ("ISR");
                tipoTabla.sistema = (true);
                listTipoTabla.Add(tipoTabla);

                tipoTabla = new TipoTabla();
                tipoTabla.clave = ("2");
                tipoTabla.nombre = ("SUBSIDIO");
                tipoTabla.sistema = (true);
                listTipoTabla.Add(tipoTabla);

                tipoTabla = new TipoTabla();
                tipoTabla.clave = ("3");
                tipoTabla.nombre = ("SALARIOSMÍNIMOS");
                tipoTabla.sistema = (true);
                listTipoTabla.Add(tipoTabla);

                tipoTabla = new TipoTabla();
                tipoTabla.clave = ("4");
                tipoTabla.nombre = ("FACTORINTEGRACION");
                tipoTabla.sistema = (true);
                listTipoTabla.Add(tipoTabla);

                tipoTabla = new TipoTabla();
                tipoTabla.clave = ("5");
                tipoTabla.nombre = ("DIASFESTIVOS");
                tipoTabla.sistema = (true);
                listTipoTabla.Add(tipoTabla);

                tipoTabla = new TipoTabla();
                tipoTabla.clave = ("6");
                tipoTabla.nombre = ("PERSONALIZADOS");
                tipoTabla.sistema = (false);
                listTipoTabla.Add(tipoTabla);

                for (i = 0; i < listTipoTabla.Count; i++)
                {
                    dbContext.Set<TipoTabla>().Add(listTipoTabla[i]);
                }

                /*contador = contador + listTipoTabla.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/

                dbContext.SaveChanges();
                #endregion

                #region ELEMENTO APLICACION
                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (1);
                elementosAplicacion.clave = ("01");
                elementosAplicacion.entidad = ("RazonesSociales");
                elementosAplicacion.nombre = ("Empresas");
                elementosAplicacion.ordenId = (0);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (2);
                elementosAplicacion.clave = ("02");
                elementosAplicacion.entidad = ("RegistroPatronal");
                elementosAplicacion.nombre = ("Reg.Patro");
                elementosAplicacion.ordenId = (1);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (3);
                elementosAplicacion.clave = ("03");
                elementosAplicacion.entidad = ("CentroDeCosto");
                elementosAplicacion.nombre = ("Centro de costo");
                elementosAplicacion.ordenId = (2);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (4);
                elementosAplicacion.clave = ("04");
                elementosAplicacion.entidad = ("Departamentos");
                elementosAplicacion.nombre = ("Departamento");
                elementosAplicacion.ordenId = (3);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (5);
                elementosAplicacion.clave = ("05");
                elementosAplicacion.entidad = ("Empleados");
                elementosAplicacion.nombre = ("Empleado");
                elementosAplicacion.ordenId = (4);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (6);
                elementosAplicacion.clave = ("06");
                elementosAplicacion.entidad = ("TipoNomina");
                elementosAplicacion.nombre = ("Tipo de nomina");
                elementosAplicacion.ordenId = (5);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (7);
                elementosAplicacion.clave = ("07");
                elementosAplicacion.entidad = ("CategoriasPuestos");
                elementosAplicacion.nombre = ("Categoria de puesto");
                elementosAplicacion.ordenId = (6);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (8);
                elementosAplicacion.clave = ("08");
                elementosAplicacion.entidad = ("Puestos");
                elementosAplicacion.nombre = ("Puesto");
                elementosAplicacion.ordenId = (7);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (9);
                elementosAplicacion.clave = ("09");
                elementosAplicacion.entidad = ("TipoCorrida");
                elementosAplicacion.nombre = ("Tipo de corrida");
                elementosAplicacion.ordenId = (8);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (10);
                elementosAplicacion.clave = ("10");
                elementosAplicacion.entidad = ("TipoCentroCostos");
                elementosAplicacion.nombre = ("Tipo centro de costo");
                elementosAplicacion.ordenId = (10);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (11);
                elementosAplicacion.clave = ("11");
                elementosAplicacion.entidad = ("NivelesClasificacion");
                elementosAplicacion.nombre = ("Niveles de clasificacion");
                elementosAplicacion.ordenId = (11);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (12);
                elementosAplicacion.clave = ("12");
                elementosAplicacion.entidad = ("Grupo");
                elementosAplicacion.nombre = ("Grupo de nomina");
                elementosAplicacion.ordenId = (12);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (13);
                elementosAplicacion.clave = ("13");
                elementosAplicacion.entidad = ("Periodicidad");
                elementosAplicacion.nombre = ("Periodicidad");
                elementosAplicacion.ordenId = (13);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (14);
                elementosAplicacion.clave = ("14");
                elementosAplicacion.entidad = ("Usuario");
                elementosAplicacion.nombre = ("Usuario");
                elementosAplicacion.ordenId = (14);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                elementosAplicacion = new ElementosAplicacion();
                elementosAplicacion.id = (15);
                elementosAplicacion.clave = ("15");
                elementosAplicacion.entidad = ("Estados");
                elementosAplicacion.nombre = ("Estados");
                elementosAplicacion.ordenId = (15);
                elementosAplicacion.parentId = (0);
                listElementosAplicacion.Add(elementosAplicacion);

                for (i = 0; i < listElementosAplicacion.Count; i++)
                {
                    dbContext.Set<ElementosAplicacion>().Add(listElementosAplicacion[i]);
                }
                dbContext.SaveChanges();
                #endregion

                #region PARAMETROS
                /*Parametros*/
                List<ElementosAplicacion> listRazonSocialElemento;
                listRazonSocialElemento = new List<ElementosAplicacion>();
                listRazonSocialElemento.Add(listElementosAplicacion[0]);

                parametros = new Parametros();
                parametros.clave = (1);
                parametros.nombre = ("PermiteTipoCostos");
                parametros.ordenId = (1);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (2);
                parametros.nombre = ("Dia de inicio de semana.");
                parametros.ordenId = (2);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("Domingo,Lunes,Martes,Miercoles,Jueves,Viernes,Sabado");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Domingo|Lunes|Martes|Miercoles|Jueves|Viernes|Sabado");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (3);
                parametros.nombre = ("Margen de tiempo de Tolerancia para considerar como Checadas Repetidas");
                parametros.ordenId = (3);
                parametros.valor = ("10");
                parametros.opcionesParametros = ("El margen es en minutos");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (4);
                parametros.nombre = ("Tiempo de Tolerancia Antes de Hora de Entrada");
                parametros.ordenId = (4);
                parametros.valor = ("5");
                parametros.opcionesParametros = ("El margen es en minutos");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (5);
                parametros.nombre = ("Ajustar los periodos de nomina");
                parametros.ordenId = (5);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("Esto le permitira que los periodos se generen exactos al mes.");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (6);
                parametros.nombre = ("¿Manejar Organigrama por Plaza?");
                parametros.ordenId = (6);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (7);
                parametros.nombre = ("¿Tipo de tabla de ISR a utilizar?");
                parametros.ordenId = (7);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("Se puede utilizar la tabla mensual o bien por cada periodicidad un tabla correspondiente a ella.");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Mensual|Periodicidad");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.ISR);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (8);
                parametros.nombre = ("Factor de aplicación tabla mensual");
                parametros.ordenId = (8);
                parametros.valor = ("30.4");
                parametros.opcionesParametros = ("introduzca un numero ejemplo: 30 o 30.4");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|2");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.ISR);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (9);
                parametros.nombre = ("Factor de aplicación tabla anual");
                parametros.ordenId = (9);
                parametros.valor = ("365");
                parametros.opcionesParametros = ("Introduzca un numero");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("3|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.ISR);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (10);
                parametros.nombre = ("¿Desglose interno del ISR?");
                parametros.ordenId = (10);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("Normal y Anual. :: Normal, Anual y Directo. :: Tabla Anual.");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Normal y Anual|Normal, Anual y Directo|Tabla Anual");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.ISR);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (11);
                parametros.nombre = ("¿Modo para ajustar el ISR en el mes?");
                parametros.ordenId = (11);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("1=Se proporciona en cada periodo independiente :: 2=Proporciona cada periodo ajustando al mes :: 3=Proporciona cada periodo independiente según días naturales :: 4=Proporciona cada periodo independiente y ajusta el último periodo del mes :: 5=En el último periodo se ajusta sin proporcionar al mes :: 6=Proporcional con la tabla Anual");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Proporciona c/periodo independiente|Proporciona c/periodo ajustando al mes|Proporciona c/periodo independiente según días naturales|Proporciona c/periodo independiente y ajusta el último periodo del mes|Último periodo sin ajustar al mes|Proporciona con la tabla anual");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.ISR);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (12);
                parametros.nombre = ("¿Calcular en automatico el SDI?");
                parametros.ordenId = (12);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.IMSS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (13);
                parametros.nombre = ("¿Manejar pagos por hora?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (13);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.SUELDOSYSALARIOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (14);
                parametros.nombre = ("¿Manejar horas Por?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (14);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Natural|HSM");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.SUELDOSYSALARIOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (15);
                parametros.nombre = ("¿Manejar tablas de prestaciones?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (15);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.PRESTACIONES);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (16);
                parametros.nombre = ("¿Activar topes salariales?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (16);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.IMSS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (17);
                parametros.nombre = ("¿Activar control de puestos por Registro Patronal?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (17);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (18);
                parametros.nombre = ("¿Manejar salario por tabulador?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (18);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.SUELDOSYSALARIOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (19);
                parametros.nombre = ("¿Permitir manejar multiplazas?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (19);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.INFORMACIONNOMINAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (20);
                parametros.nombre = ("¿Permitir manejar Departamentos?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (20);
                parametros.valor = ("1");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (21);
                parametros.nombre = ("¿Permitir manejar Centros de costos?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (21);
                parametros.valor = ("1");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (22);
                parametros.nombre = ("¿Manejar datos de la nomina del empleado por folio o referencia?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (22);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.INFORMACIONNOMINAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (23);
                parametros.nombre = ("¿Activar funcionalidad de catalogo a Movimientos?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (23);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.MOVIMIENTOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (24);
                parametros.nombre = ("¿Minimizar en automatico datos de la nomina?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (24);
                parametros.valor = ("1");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (25);
                parametros.nombre = ("¿Forma de alimentar el sueldo o salario del empleado?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (25);
                parametros.valor = ("1");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Diario|Semanal|Quincenal|Mensual");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.SUELDOSYSALARIOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (26);
                parametros.nombre = ("¿Insercion automatica modo grid?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (26);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (27);
                parametros.nombre = ("¿Capturar datos de nomina del empleado separado?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (27);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.INFORMACIONNOMINAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (28);
                parametros.nombre = ("¿Manejar orden calculo en conceptos?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (28);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.CONCEPTOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (29);
                parametros.nombre = ("¿Pagar nomina sobre dias naturales?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (29);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.SUELDOSYSALARIOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (30);
                parametros.nombre = ("¿Mostrar conceptos con valor 0?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (30);
                parametros.valor = ("1");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.CONCEPTOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (31);
                parametros.nombre = ("¿Mostrar captura registro incapacidades?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (31);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (32);
                parametros.nombre = ("¿Mostrar busqueda en los catalogos en automatico?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (32);
                parametros.valor = ("1");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (33);
                parametros.nombre = ("Año activo para periodos");
                parametros.opcionesParametros = ("Ingresar el año del periodo a mostrar");
                parametros.ordenId = (33);
                parametros.valor = ("2018");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("4|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (34);
                parametros.nombre = ("Pais predeterminado");
                parametros.opcionesParametros = ("Mostrar pais predeterminado");
                parametros.ordenId = (34);
                parametros.valor = ("1");
                parametros.tipoConfiguracion = (3);
                parametros.propiedadConfig = ("Paises|Clave|Descripcion");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (35);
                parametros.nombre = ("Generar datos de timbrado al cerrar el periodo?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (35);
                parametros.valor = ("1");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (36);
                parametros.nombre = ("Combinar detallado del concepto para el recibo de nómina?");
                parametros.opcionesParametros = ("");
                parametros.ordenId = (35);
                parametros.valor = ("2");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (37);
                parametros.nombre = ("Tiempo de Tolerancia Antes de Hora de Entrada");
                parametros.ordenId = (37);
                parametros.valor = ("5");
                parametros.opcionesParametros = ("El tiempo es en minutos");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (38);
                parametros.nombre = ("Considerar como retardo si excede tolerancia después de Entrada");
                parametros.ordenId = (6);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (39);
                parametros.nombre = ("Activar tiempo límite en entrada para considerar como ausentismo");
                parametros.ordenId = (7);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (40);
                parametros.nombre = ("Tiempo límite para considerar como ausentismo");
                parametros.ordenId = (8);
                parametros.valor = ("5");
                parametros.opcionesParametros = ("El tiempo es en minutos");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (41);
                parametros.nombre = ("Impedir registro de entrada si llega despues del tiempo límite para ausentismo");
                parametros.ordenId = (9);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (42);
                parametros.nombre = ("Tolerancia Antes de Hora de Salida");
                parametros.ordenId = (10);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("El tiempo es en minutos");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (43);
                parametros.nombre = ("Restringir salida antes del tiempo");
                parametros.ordenId = (11);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (44);
                parametros.nombre = ("Si salida es antes de este tiempo se considera ausentismo");
                parametros.ordenId = (12);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (49);
                parametros.nombre = ("Utilizar tope diario para horas extras");
                parametros.ordenId = (17);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (50);
                parametros.nombre = ("Tope Horas Dobles Diario");
                parametros.ordenId = (18);
                parametros.valor = ("3");
                parametros.opcionesParametros = ("El tope default es 3");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("1|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (51);
                parametros.nombre = ("Tope Horas Dobles Semanal");
                parametros.ordenId = (19);
                parametros.valor = ("9");
                parametros.opcionesParametros = ("El tope default es 9");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("1|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (52);
                parametros.nombre = ("Solicitar autorización si excede cierta cantidad de hrs extras");
                parametros.ordenId = (20);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (53);
                parametros.nombre = ("Cantidad de horas extras que no requieren autorización");
                parametros.ordenId = (21);
                parametros.valor = ("0");
                parametros.opcionesParametros = ("La cantidad es de minutos");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("1|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (54);
                parametros.nombre = ("Tipo de Redondeo para horas ordinarias");
                parametros.ordenId = (22);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("El personalizado se puede establecer el redondeo y el factor del mismo, con la opcion de tabla se selecciona una opcion guardado desde el catalago de tipo de redondeos");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Personalizado|Tabla");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (57);
                parametros.nombre = ("Activar Primer Coffe Break");
                parametros.ordenId = (25);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (58);
                parametros.nombre = ("Activar checada salida y entrada para primer Coffe Break");
                parametros.ordenId = (26);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (61);
                parametros.nombre = ("Activar Tiempo de Comida");
                parametros.ordenId = (29);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (62);
                parametros.nombre = ("Modalidad del tiempo para comer");
                parametros.ordenId = (30);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("Se especifica el tiempo y un margen de tiempo para tomar comida :: Se especifica el horario");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Por tiempo y margen|Por horario");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (65);
                parametros.nombre = ("Checar salida para comer");
                parametros.ordenId = (33);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (66);
                parametros.nombre = ("Tolerancia Antes en Comida");
                parametros.ordenId = (34);
                parametros.valor = ("0");
                parametros.opcionesParametros = ("El tiempo es en minutos");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (67);
                parametros.nombre = ("Restringir salida a comer antes del tiempo");
                parametros.ordenId = (35);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (68);
                parametros.nombre = ("Tolerancia Después en Comida");
                parametros.ordenId = (36);
                parametros.valor = ("0");
                parametros.opcionesParametros = ("El tiempo es en minutos");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (69);
                parametros.nombre = ("Considerar como retardo si excede tolerancia después de Entrada");
                parametros.ordenId = (37);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (70);
                parametros.nombre = ("Tratamiento de Tiempo para comer");
                parametros.ordenId = (38);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("Incluir en el tiempo ordinario :: No considerar como tiempo ordinario");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Incluir en el tiempo ordinario|No considerar como tiempo ordinario");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (73);
                parametros.nombre = ("Activar Segundo Coffe Break");
                parametros.ordenId = (41);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (74);
                parametros.nombre = ("Activar checada salida y entrada para Segundo Coffe Break");
                parametros.ordenId = (42);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (77);
                parametros.nombre = ("El empleado tiene tiempo extra cuando llega antes de su hora");
                parametros.ordenId = (45);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (78);
                parametros.nombre = ("El empleado tiene tiempo extra si dispone de su hora de comida");
                parametros.ordenId = (46);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (79);
                parametros.nombre = ("Tratamiento para tiempo extra cuando el empleado asiste en días festivos");
                parametros.ordenId = (47);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (80);
                parametros.nombre = ("Indicar si se paga horas extras");
                parametros.ordenId = (48);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (81);
                parametros.nombre = ("Tope");
                parametros.ordenId = (49);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("Diario o Semanal");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Diario|Semanal");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (82);
                parametros.nombre = ("Indicar si se paga el dia festivo trabajado");
                parametros.ordenId = (50);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (83);
                parametros.nombre = ("Tratamiento para tiempo extra cuando el empleado asiste en días de descanso");
                parametros.ordenId = (51);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (84);
                parametros.nombre = ("Indicar si se paga horas extras");
                parametros.ordenId = (52);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (85);
                parametros.nombre = ("Tope ");
                parametros.ordenId = (53);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("Diario o Semanal");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Diario|Semanal");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (86);
                parametros.nombre = ("Indicar si se paga el descanso trabajado");
                parametros.ordenId = (54);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (87);
                parametros.nombre = ("El empleado tiene tiempo extra cuando sale después de su hora de salida");
                parametros.ordenId = (55);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (88);
                parametros.nombre = ("Usar tiempo extra para compensar tiempo ordinario no completado");
                parametros.ordenId = (56);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (89);
                parametros.nombre = ("Tiempo mínimo para considerar tiempo extra");
                parametros.ordenId = (57);
                parametros.valor = ("0");
                parametros.opcionesParametros = ("El tiempo es en minutos");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (90);
                parametros.nombre = ("Tipo de Redondeo para horas extras");
                parametros.ordenId = (58);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("El personalizado se puede establecer el redondeo y el factor del mismo, con la opcion de tabla se selecciona una opcion guardado desde el catalago de tipo de redondeos");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Personalizado|Tabla");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (93);
                parametros.nombre = ("Aplicar regla para Retardos ('x' retardos en 'y' días equivale a 'z' faltas)");
                parametros.ordenId = (61);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (94);
                parametros.nombre = ("Número de Retardos");
                parametros.ordenId = (62);
                parametros.valor = ("0");
                parametros.opcionesParametros = ("Ingresar el número de retardos");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (95);
                parametros.nombre = ("Número de dias naturales en que ocurrieron retardos");
                parametros.ordenId = (63);
                parametros.valor = ("0");
                parametros.opcionesParametros = ("Ingresar los dias");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (96);
                parametros.nombre = ("Número de faltas a considerar");
                parametros.ordenId = (64);
                parametros.valor = ("0");
                parametros.opcionesParametros = ("Ingresar las faltas");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (97);
                parametros.nombre = ("Tratamiento de Séptimo Día");
                parametros.ordenId = (65);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("Si es pagar completo ingresar el mínimo a trabajar..");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Proporcional|Pagar completo");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (100);
                parametros.nombre = ("Mínimo a trabajar para pago de séptimo día");
                parametros.ordenId = (68);
                parametros.valor = ("8");
                parametros.opcionesParametros = ("Ingresar las horas");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("2|@");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.HORARIOSTURNO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (101);
                parametros.nombre = ("¿Tipo de accion a aplicar sobre el resultado del concepto en los movimientos?");
                parametros.ordenId = (31);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Truncado|Redondeo");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.CONCEPTOS);
                listParametros.Add(parametros);
                parametros = new Parametros();

                parametros = new Parametros();
                parametros.clave = (102);
                parametros.nombre = ("Mascara para el resultado del concepto");
                parametros.ordenId = (32);
                parametros.valor = ("############.##");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (5);
                parametros.propiedadConfig = ("12|2");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.CONCEPTOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (103);
                parametros.nombre = ("Mascara para sueldos");
                parametros.ordenId = (30);
                parametros.valor = ("############.##");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (5);
                parametros.propiedadConfig = ("12|2");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.SUELDOSYSALARIOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (104);
                parametros.nombre = ("Mascara para sueldo diario");
                parametros.ordenId = (31);
                parametros.valor = ("############.##");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (5);
                parametros.propiedadConfig = ("12|2");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.SUELDOSYSALARIOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (105);
                parametros.nombre = ("Mascara para salario diario integrado");
                parametros.ordenId = (32);
                parametros.valor = ("############.##");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (5);
                parametros.propiedadConfig = ("12|2");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.SUELDOSYSALARIOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (106);
                parametros.nombre = ("Mascara para horas");
                parametros.ordenId = (33);
                parametros.valor = ("##.##");
                parametros.opcionesParametros = ("horas:minutos (hh:mm)o en numeros (##.##)");
                parametros.tipoConfiguracion = (5);
                parametros.propiedadConfig = ("2|2");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.SUELDOSYSALARIOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (107);
                parametros.nombre = ("¿Permitir manejar categoria de puesto?");
                parametros.ordenId = (36);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (108);
                parametros.nombre = ("¿Permitir manejar subcuenta en los departamentos?");
                parametros.ordenId = (36);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                //String ruta = buscaRutaArchivos(System.getProperty("user.dir"), archivosReportes);
                String ruta = "";
                if (String.IsNullOrEmpty(ruta))
                {
                    ruta = "C:\\REPORTESSISTEMAS";
                }
                parametros = new Parametros();
                parametros.clave = (109);
                parametros.nombre = ("Ubicación de los reportes del sistema");
                parametros.ordenId = (37);
                parametros.valor = (ruta);
                parametros.opcionesParametros = ("Esta es la ubicación de los reportes fijos en el servidor");
                parametros.tipoConfiguracion = (6);
                parametros.propiedadConfig = ("255");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (110);
                parametros.nombre = ("Carpeta del reporte de nomina");
                parametros.ordenId = (38);
                parametros.valor = ("REPORTENOMINA");
                parametros.opcionesParametros = ("Esta es la carpeta del reporte de nómina que se encuentra dentro de los reportes fijos del sistema.");
                parametros.tipoConfiguracion = (8);
                parametros.propiedadConfig = ("255");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (111);
                parametros.nombre = ("Logo para los reportes del sistema");
                parametros.ordenId = (38);
                parametros.valor = ("C:\\REPORTESSISTEMAS\\LOGOS\\nrh logo-03.png");
                parametros.opcionesParametros = ("Este es el logo que tendran los reportes del sistema");
                parametros.tipoConfiguracion = (7);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                String c = "3C3F786D6C2076657273696F6E3D27312E302720656E636F64696E673D275554462D38273F3E3C7461626C65206E616D653D2249535222203E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D22323722203E3C636F6C756D6E6120706F733D223122206E6F6D627265436F6C3D224C696D697465496E666572696F722220746974756C6F3D224C696D69746520496E666572696F7222207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D223822202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E4C696D69746520496E666572696F723C2F65733E3C656E206E6F6D6272653D22496E676C6573223E4C6F776572204C696D69743C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D22323722203E3C636F6C756D6E6120706F733D223222206E6F6D627265436F6C3D2243756F746146696A612220746974756C6F3D2243756F74612046696A6122207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D223822202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E43756F74612046696A613C2F65733E3C656E206E6F6D6272653D22496E676C6573223E466C6174204665653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223022203E3C636F6C756D6E6120706F733D223322206E6F6D627265436F6C3D22506F7263656E74616A652220746974756C6F3D22506F7263656E74616A6522207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D223522202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E506F7263656E74616A653C2F65733E3C656E206E6F6D6272653D22496E676C6573223E506F7263656E74616A653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C436F6E74726F6C61646F7265733E3C436F6E74726F6C61646F72207469706F3D22566967656E7465206465736465222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6775692E706572736F6E616C697A61646F732E504A58446174655069636B657222206964656E746966696361646F723D22436F6E74726F6C506F724665636861222F3E3C2F436F6E74726F6C61646F7265733E3C2F7461626C653E";
                byte[] imagen = new byte[c.Length / 2];
                for (i = 0; i < c.Length; i += 2)
                {
                    imagen[i / 2] = (byte)((Digit(c[i], 16) << 4)
                            + Digit(c[i + 1], 16));
                }
                parametros.imagen = (imagen);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (112);
                parametros.nombre = ("Tasa de impuesto estatal");
                parametros.ordenId = (112);
                parametros.valor = ("2.00");
                parametros.opcionesParametros = ("El valor de la tasa impuesto global");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("3|2");
                List<ElementosAplicacion> listElemEstado = new List<ElementosAplicacion>();
                listElemEstado.Add(listElementosAplicacion[14]); //Estados
                parametros.elementosAplicacion = (listElemEstado);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (113);
                parametros.nombre = ("¿Abrir al exportar los reportes?");
                parametros.ordenId = (113);
                parametros.valor = ("1");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (114);
                parametros.nombre = ("Incluir Concepto: Dato,Calculo ó Informativo en reporte de nómina");
                parametros.ordenId = (114);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("Dato. :: Cálculo. :: Informativo.");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Dato|Cálculo|Informativo");
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.REPORTES);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (115);
                parametros.nombre = ("¿Seleccionar texto al recibir foco?");
                parametros.ordenId = (115);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("1=Si :: 2=No");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (116);
                parametros.nombre = ("¿Mantenerse el último filtro utilizado en los movimientos de nómina?");
                parametros.ordenId = (116);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("1=Si :: 2=No");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");

                //Elemento por  razon social y usuario
                parametros.elementosAplicacion = new List<ElementosAplicacion> { listElementosAplicacion[0], listElementosAplicacion[12] };
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (117);
                parametros.nombre = ("¿Versión del cálculo para crédito y ahorro?");
                parametros.ordenId = (117);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("1|0");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.CALCULO);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (118);
                parametros.nombre = ("¿Pagar IMSS sobre días naturales?");
                parametros.ordenId = (118);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.IMSS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (119);
                parametros.nombre = ("Carpeta del recibo CFDI");
                parametros.ordenId = (119);
                parametros.valor = ("ReporteReciboNominaCFDI");
                parametros.opcionesParametros = ("Esta es la carpeta del recibo CFDI");
                parametros.tipoConfiguracion = (8);
                parametros.propiedadConfig = ("255");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (120);
                parametros.nombre = ("Carpeta del recibo de CDFI asimilado");
                parametros.ordenId = (120);
                parametros.valor = ("ReporteAsimilados");
                parametros.opcionesParametros = ("Esta es la carpeta del recibo CFDI asimilado");
                parametros.tipoConfiguracion = (8);
                parametros.propiedadConfig = ("255");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (121);
                parametros.nombre = ("Asignacion concepto despensa");
                parametros.ordenId = (121);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("Concepto despensas para cuentas contables");
                parametros.tipoConfiguracion = (3);
                parametros.propiedadConfig = ("ConcepNomDefi|Clave|Descripcion");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.CONCEPTOS);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (122);
                parametros.nombre = ("Control de Registros Patronales");
                parametros.ordenId = (122);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("Definir el Nivel de Control de Registros Patronales");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Registro Patronal único|Registro Patronal por Tipo de Nómina|Registro Patronal por Plaza");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (123);
                parametros.nombre = ("Activar Credito infonavit");
                parametros.ordenId = (123);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("Activa captura de Infonavit en catalogo de empleados");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (124);
                parametros.nombre = ("Permitir pagar prima vacacional aparte");
                parametros.ordenId = (124);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("1=Si :: 2=No");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                //Elemento por  razon social y usuario
                parametros.elementosAplicacion = (new List<ElementosAplicacion> { listElementosAplicacion[0], listElementosAplicacion[12] });
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (125);
                parametros.nombre = ("Definir días para derecho a PTU");
                parametros.ordenId = (125);
                parametros.valor = ("60");
                parametros.opcionesParametros = ("Configuración para definir dias para derecho a PTU");
                parametros.tipoConfiguracion = (4);
                parametros.propiedadConfig = ("60|@");
                //Elemento por  razon social y usuario
                parametros.elementosAplicacion = (new List<ElementosAplicacion> { listElementosAplicacion[0], listElementosAplicacion[12] });
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (126);
                parametros.nombre = ("Opción de cálculo ISR por PTU");
                parametros.ordenId = (126);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("Art. 113 LISR ó Art. 142 RLISR");
                parametros.tipoConfiguracion = (2);
                parametros.propiedadConfig = ("Art. 113 LISR |Art. 142 RLISR");
                //Elemento por  razon social y usuario
                parametros.elementosAplicacion = (new List<ElementosAplicacion> { listElementosAplicacion[0], listElementosAplicacion[12] });
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (127);
                parametros.nombre = ("Pagar PTU en el primer periodo activo después del cálculo del PTU");
                parametros.ordenId = (126);
                parametros.valor = ("1");
                parametros.opcionesParametros = ("1=Si :: 2=No");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                //Elemento por  razon social y usuario
                parametros.elementosAplicacion = (new List<ElementosAplicacion> { listElementosAplicacion[0], listElementosAplicacion[12] });
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (128);
                parametros.nombre = ("Permitir modificar variables en calculo de PTU");
                parametros.ordenId = (127);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("1=Si :: 2=No");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                //Elemento por  razon social y usuario
                parametros.elementosAplicacion = (new List<ElementosAplicacion> { listElementosAplicacion[0], listElementosAplicacion[12] });
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.GLOBAL);
                listParametros.Add(parametros);

                parametros = new Parametros();
                parametros.clave = (129);
                parametros.nombre = ("¿Descontar faltas al modo ajustar al mes?");
                parametros.ordenId = (129);
                parametros.valor = ("2");
                parametros.opcionesParametros = ("");
                parametros.tipoConfiguracion = (1);
                parametros.propiedadConfig = ("");
                parametros.elementosAplicacion = (listRazonSocialElemento);
                parametros.modulo = (listModulo[0]);
                parametros.clasificacion = (int)(Clasificacion.ISR);
                listParametros.Add(parametros);

                for (i = 0; i < listParametros.Count; i++)
                {
                    dbContext.Set<Parametros>().Add(listParametros[i]);
                }

                /*contador = contador + listParametros.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region TABLA BASE
                tablaBase = new TablaBase();
                tablaBase.clave = ("01");
                tablaBase.controladores = ("ControlPorFecha");
                tablaBase.descripcion = ("ISR");
                tablaBase.descripcionAbreviada = ("ISR");
                String s = "3C3F786D6C2076657273696F6E3D27312E302720656E636F64696E673D275554462D38273F3E3C7461626C65206E616D653D2249535222203E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D22323722203E3C636F6C756D6E6120706F733D223122206E6F6D627265436F6C3D224C696D697465496E666572696F722220746974756C6F3D224C696D69746520696E666572696F7222207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313522202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E4C696D69746520696E666572696F723C2F65733E3C656E206E6F6D6272653D22496E676C6573223E4C6F776572206C696D69743C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D22323722203E3C636F6C756D6E6120706F733D223222206E6F6D627265436F6C3D2243756F746146696A612220746974756C6F3D2243756F74612066696A6122207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E43756F74612066696A613C2F65733E3C656E206E6F6D6272653D22496E676C6573223E466C6174206665653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223022203E3C636F6C756D6E6120706F733D223322206E6F6D627265436F6C3D22506F7263656E74616A652220746974756C6F3D22506F7263656E74616A6522207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E506F7263656E74616A653C2F65733E3C656E206E6F6D6272653D22496E676C6573223E506F7263656E74616A653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C436F6E74726F6C61646F7265733E3C436F6E74726F6C61646F72207469706F3D22566967656E7465206465736465222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6775692E706572736F6E616C697A61646F732E504A58446174655069636B657222206964656E746966696361646F723D22436F6E74726F6C506F724665636861222F3E3C2F436F6E74726F6C61646F7265733E3C2F7461626C653E";
                byte[] data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {

                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaBase.fileXml = (data);
                tablaBase.renglonSeleccionado = (true);
                tablaBase.tipoTabla = (listTipoTabla[0]);
                listTablaBase.Add(tablaBase);

                tablaBase = new TablaBase();
                tablaBase.clave = ("02");
                tablaBase.controladores = ("ControlPorFecha");
                tablaBase.descripcion = ("SUBSIDIO EMPLEO");
                tablaBase.descripcionAbreviada = ("SUBSIDIO EMPLEO");
                s = "3C3F786D6C2076657273696F6E3D27312E302720656E636F64696E673D275554462D38273F3E3C7461626C65206E616D653D22535542534944494F22203E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223222203E3C636F6C756D6E6120706F733D223122206E6F6D627265436F6C3D224C696D697465496E666572696F722220746974756C6F3D224C696D69746520696E666572696F7222207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E4C696D69746520696E666572696F723C2F65733E3C656E206E6F6D6272653D22496E676C6573223E4C6F776572206C696D69743C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223222203E3C636F6C756D6E6120706F733D223222206E6F6D627265436F6C3D2243756F74612220746974756C6F3D2243756F746122207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E43756F74613C2F65733E3C656E206E6F6D6272653D22496E676C6573223E53686172653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C436F6E74726F6C61646F7265733E3C436F6E74726F6C61646F72207469706F3D22566967656E7465206465736465222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6775692E706572736F6E616C697A61646F732E504A58446174655069636B657222206964656E746966696361646F723D22436F6E74726F6C506F724665636861222F3E3C2F436F6E74726F6C61646F7265733E3C2F7461626C653E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaBase.fileXml = (data);
                tablaBase.renglonSeleccionado = (true);
                tablaBase.tipoTabla = (listTipoTabla[1]);
                listTablaBase.Add(tablaBase);

                tablaBase = new TablaBase();
                tablaBase.clave = ("03");
                tablaBase.controladores = ("ControlPorFecha");
                tablaBase.descripcion = ("DIAS FESTIVOS");
                tablaBase.descripcionAbreviada = ("DIAS FESTIVOS");
                s = "3C3F786D6C2076657273696F6E3D27312E302720656E636F64696E673D275554462D38273F3E3C7461626C65206E616D653D22444941534645535449564F5322203E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223522203E3C636F6C756D6E6120706F733D223122206E6F6D627265436F6C3D2266656368612220746974756C6F3D22466368612064C3AD61206665737469766F22207469706F3D22446174652220666F726D61746F3D2264642F4D4D2F79797979222074616D436F6C3D22313522202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E466368612064C3AD61206665737469766F3C2F65733E3C656E206E6F6D6272653D22496E676C6573223E5075626C696320686F6C69646179733C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223522203E3C636F6C756D6E6120706F733D223222206E6F6D627265436F6C3D226465736372697063696F6E2220746974756C6F3D224465736372697063696F6E22207469706F3D22537472696E672220666F726D61746F3D223130222074616D436F6C3D22333522202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E4465736372697063696F6E3C2F65733E3C656E206E6F6D6272653D22496E676C6573223E4465736372697074696F6E3C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C436F6E74726F6C61646F7265733E3C436F6E74726F6C61646F72207469706F3D22566967656E7465206465736465222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6775692E706572736F6E616C697A61646F732E504A58446174655069636B657222206964656E746966696361646F723D22436F6E74726F6C506F724665636861222F3E3C2F436F6E74726F6C61646F7265733E3C2F7461626C653E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaBase.fileXml = (data);
                tablaBase.renglonSeleccionado = (true);
                tablaBase.tipoTabla = (listTipoTabla[4]);
                listTablaBase.Add(tablaBase);

                tablaBase = new TablaBase();
                tablaBase.controladores = ("ControlPorFecha");
                tablaBase.clave = ("04");
                tablaBase.descripcion = ("SALARIOS MÍNIMOS");
                tablaBase.descripcionAbreviada = ("SALARIOS MÍNIMOS");
                s = "3C3F786D6C2076657273696F6E3D27312E302720656E636F64696E673D275554462D38273F3E3C7461626C65206E616D653D2253414C4152494F534DC38D4E494D4F5322203E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223122203E3C636F6C756D6E6120706F733D223122206E6F6D627265436F6C3D224465736372697063696F6E2220746974756C6F3D225A6F6E6122207469706F3D22537472696E672220666F726D61746F3D223130222074616D436F6C3D223822202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E5A6F6E613C2F65733E3C656E206E6F6D6272653D22496E676C6573223E5A6F6E653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223122203E3C636F6C756D6E6120706F733D223222206E6F6D627265436F6C3D2253616C6172696F2220746974756C6F3D2253616C6172696F22207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E53616C6172696F3C2F65733E3C656E206E6F6D6272653D22496E676C6573223E576167653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C436F6E74726F6C61646F7265733E3C436F6E74726F6C61646F72207469706F3D22566967656E7465206465736465222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6775692E706572736F6E616C697A61646F732E504A58446174655069636B657222206964656E746966696361646F723D22436F6E74726F6C506F724665636861222F3E3C2F436F6E74726F6C61646F7265733E3C2F7461626C653E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaBase.fileXml = (data);
                tablaBase.renglonSeleccionado = (true);
                tablaBase.tipoTabla = (listTipoTabla[2]);
                listTablaBase.Add(tablaBase);

                tablaBase = new TablaBase();
                tablaBase.clave = ("05");
                tablaBase.controladores = ("ControlPorEntidad");
                tablaBase.descripcion = ("FACTORES DE INTEGRACIÓN");
                tablaBase.descripcionAbreviada = ("FACT. DE INTEG.");
                s = "3C3F786D6C2076657273696F6E3D27312E302720656E636F64696E673D275554462D38273F3E3C7461626C65206E616D653D22464143544F52494E544547524143494F4E22203E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D22313122203E3C636F6C756D6E6120706F733D223122206E6F6D627265436F6C3D22616E74696775656461642220746974756C6F3D22416E746967756564616422207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E416E74696775656461643C2F65733E3C656E206E6F6D6272653D22496E676C6573223E45786167653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D22313122203E3C636F6C756D6E6120706F733D223222206E6F6D627265436F6C3D22666163746F722220746974756C6F3D22466163746F7220696E74656772616369C3B36E22207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E303030307C222074616D436F6C3D22313522202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E466163746F7220696E74656772616369C3B36E3C2F65733E3C656E206E6F6D6272653D22496E676C6573223E466163746F7220696E746567726174696F6E3C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D22313122203E3C636F6C756D6E6120706F733D223322206E6F6D627265436F6C3D2264696173416775696E616C646F2220746974756C6F3D224469617320616775696E616C646F22207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E4469617320616775696E616C646F3C2F65733E3C656E206E6F6D6272653D22496E676C6573223E4461797320626F6E75733C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D22313122203E3C636F6C756D6E6120706F733D223422206E6F6D627265436F6C3D22646961735661636163696F6E65732220746974756C6F3D2244696173207661636163696F6E657322207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E44696173207661636163696F6E65733C2F65733E3C656E206E6F6D6272653D22496E676C6573223E44617973207661636174696F6E733C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D22313022203E3C636F6C756D6E6120706F733D223522206E6F6D627265436F6C3D227072696D615661636163696F6E616C2220746974756C6F3D225072696D61207661636163696F6E616C22207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E5072696D61207661636163696F6E616C3C2F65733E3C656E206E6F6D6272653D22496E676C6573223E5661636174696F6E207072656D69756D3C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C436F6E74726F6C61646F7265733E3C436F6E74726F6C61646F72207469706F3D22456D707265736173222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6D6F64656C6F2E656E74696461642E52617A6F6E6573536F6369616C657322206964656E746966696361646F723D22436F6E74726F6C506F72456E7469646164222F3E3C436F6E74726F6C61646F72207469706F3D225265672E506174726F222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6D6F64656C6F2E656E74696461642E526567697374726F506174726F6E616C22206964656E746966696361646F723D22436F6E74726F6C506F72456E7469646164222F3E3C436F6E74726F6C61646F72207469706F3D225469706F206465206E6F6D696E61222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6D6F64656C6F2E656E74696461642E5469706F4E6F6D696E6122206964656E746966696361646F723D22436F6E74726F6C506F72456E7469646164222F3E3C436F6E74726F6C61646F72207469706F3D2243617465676F7269612064652070756573746F222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6D6F64656C6F2E656E74696461642E43617465676F7269617350756573746F7322206964656E746966696361646F723D22436F6E74726F6C506F72456E7469646164222F3E3C2F436F6E74726F6C61646F7265733E3C2F7461626C653E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaBase.fileXml = (data);
                tablaBase.renglonSeleccionado = (true);
                tablaBase.tipoTabla = (listTipoTabla[3]);
                listTablaBase.Add(tablaBase);

                tablaBase = new TablaBase();
                tablaBase.clave = ("06");
                tablaBase.controladores = ("ControlPorAño");
                tablaBase.descripcion = ("ISR ANUAL");
                tablaBase.descripcionAbreviada = ("ISR ANUAL 2018");
                s = "3C3F786D6C2076657273696F6E3D27312E302720656E636F64696E673D275554462D38273F3E3C7461626C65206E616D653D2249535222203E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223722203E3C636F6C756D6E6120706F733D223122206E6F6D627265436F6C3D224C696D697465496E666572696F722220746974756C6F3D224C696D69746520696E666572696F7222207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313522202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C383C692C382C2B16F6C223E4C696D69746520696E666572696F723C2F65733E3C656E206E6F6D6272653D22496E676C6573223E4C6F776572206C696D69743C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223722203E3C636F6C756D6E6120706F733D223222206E6F6D627265436F6C3D2243756F746146696A612220746974756C6F3D2243756F74612066696A6122207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C383C692C382C2B16F6C223E43756F74612066696A613C2F65733E3C656E206E6F6D6272653D22496E676C6573223E466C6174206665653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223722203E3C636F6C756D6E6120706F733D223322206E6F6D627265436F6C3D22506F7263656E74616A652220746974756C6F3D22506F7263656E74616A6522207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C383C692C382C2B16F6C223E506F7263656E74616A653C2F65733E3C656E206E6F6D6272653D22496E676C6573223E50657263656E743C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C436F6E74726F6C61646F7265733E3C436F6E74726F6C61646F72207469706F3D224361707475726120706F722061C3B16F222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6775692E706572736F6E616C697A61646F732E504A5370696E6E657222206964656E746966696361646F723D22436F6E74726F6C506F72416E696F222F3E3C2F436F6E74726F6C61646F7265733E3C2F7461626C653E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaBase.fileXml = (data);
                tablaBase.renglonSeleccionado = (true);
                tablaBase.tipoTabla = (listTipoTabla[0]);
                listTablaBase.Add(tablaBase);

                tablaBase = new TablaBase();
                tablaBase.clave = ("07");
                tablaBase.controladores = ("ControlPorAño");
                tablaBase.descripcion = ("SUBSIDIO AL EMPLEADO ANUAL 2018");
                tablaBase.descripcionAbreviada = ("SUBSIDIO EMP. ANUAL 2018");
                s = "3C3F786D6C2076657273696F6E3D27312E302720656E636F64696E673D275554462D38273F3E3C7461626C65206E616D653D22535542534944494F22203E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223422203E3C636F6C756D6E6120706F733D223122206E6F6D627265436F6C3D224C696D697465496E666572696F722220746974756C6F3D224C696D69746520696E666572696F7222207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C383C692C382C2B16F6C223E4C696D69746520696E666572696F723C2F65733E3C656E206E6F6D6272653D22496E676C6573223E4C6F776572206C696D69743C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223422203E3C636F6C756D6E6120706F733D223222206E6F6D627265436F6C3D2243756F74612220746974756C6F3D2243756F746122207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D22313022202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C383C692C382C2B16F6C223E43756F74613C2F65733E3C656E206E6F6D6272653D22496E676C6573223E53686172653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C436F6E74726F6C61646F7265733E3C436F6E74726F6C61646F72207469706F3D224361707475726120706F722061C3B16F222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6775692E706572736F6E616C697A61646F732E504A5370696E6E657222206964656E746966696361646F723D22436F6E74726F6C506F72416E696F222F3E3C2F436F6E74726F6C61646F7265733E3C2F7461626C653E";

                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaBase.fileXml = (data);
                tablaBase.renglonSeleccionado = (true);
                tablaBase.tipoTabla = (listTipoTabla[1]);
                listTablaBase.Add(tablaBase);

                tablaBase = new TablaBase();
                tablaBase.clave = ("08");
                tablaBase.controladores = ("ControlPorFecha,ControlPorEntidad");
                tablaBase.descripcion = ("ISR POR PERIODICIDAD");
                tablaBase.descripcionAbreviada = ("ISR POR PERIODICIDAD");
                s = "3C3F786D6C2076657273696F6E3D27312E302720656E636F64696E673D275554462D38273F3E3C7461626C65206E616D653D2249535222203E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223022203E3C636F6C756D6E6120706F733D223122206E6F6D627265436F6C3D224C696D697465496E666572696F722220746974756C6F3D224C696D69746520496E666572696F7222207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D223822202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E4C696D69746520496E666572696F723C2F65733E3C656E206E6F6D6272653D22496E676C6573223E4C6F776572204C696D69743C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223022203E3C636F6C756D6E6120706F733D223222206E6F6D627265436F6C3D2243756F746146696A612220746974756C6F3D2243756F74612046696A6122207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D223822202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E43756F74612046696A613C2F65733E3C656E206E6F6D6272653D22496E676C6573223E466C6174204665653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223022203E3C636F6C756D6E6120706F733D223322206E6F6D627265436F6C3D22506F7263656E74616A652220746974756C6F3D22506F7263656E74616A6522207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D223822202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E506F7263656E74616A653C2F65733E3C656E206E6F6D6272653D22496E676C6573223E50657263656E743C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C436F6E74726F6C61646F7265733E3C436F6E74726F6C61646F72207469706F3D22566967656E7465206465736465222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6775692E706572736F6E616C697A61646F732E504A58446174655069636B657222206964656E746966696361646F723D22436F6E74726F6C506F724665636861222F3E3C436F6E74726F6C61646F72207469706F3D22506572696F64696369646164222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6D6F64656C6F2E656E74696461642E506572696F6469636964616422206964656E746966696361646F723D22436F6E74726F6C506F72456E7469646164222F3E3C2F436F6E74726F6C61646F7265733E3C2F7461626C653E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaBase.fileXml = (data);
                tablaBase.renglonSeleccionado = (true);
                tablaBase.tipoTabla = (listTipoTabla[0]);
                listTablaBase.Add(tablaBase);

                tablaBase = new TablaBase();
                tablaBase.clave = ("09");
                tablaBase.controladores = ("ControlPorFecha,ControlPorEntidad");
                tablaBase.descripcion = ("SUBSIDIO POR PERIODICIDAD");
                tablaBase.descripcionAbreviada = ("SUBSIDIO POR PERIODICIDAD");
                s = "3C3F786D6C2076657273696F6E3D27312E302720656E636F64696E673D275554462D38273F3E3C7461626C65206E616D653D22535542534944494F22203E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223022203E3C636F6C756D6E6120706F733D223122206E6F6D627265436F6C3D224C696D697465496E666572696F722220746974756C6F3D224C696D69746520496E666572696F7222207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D223822202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E4C696D69746520496E666572696F723C2F65733E3C656E206E6F6D6272653D22496E676C6573223E4C6F7765204C696D69743C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C70726F70657274792073697374656D613D223122207374617475733D2230222063616E743D223022203E3C636F6C756D6E6120706F733D223222206E6F6D627265436F6C3D2243756F74612220746974756C6F3D2243756F746122207469706F3D22466C6F61742220666F726D61746F3D227C3126616D703B3233347C2E30307C222074616D436F6C3D223822202F3E3C4964696F6D61733E3C6573206E6F6D6272653D2245737061C3B16F6C223E43756F74613C2F65733E3C656E206E6F6D6272653D22496E676C6573223E53686172653C2F656E3E3C2F4964696F6D61733E3C2F70726F70657274793E3C436F6E74726F6C61646F7265733E3C436F6E74726F6C61646F72207469706F3D22566967656E7465206465736465222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6775692E706572736F6E616C697A61646F732E504A58446174655069636B657222206964656E746966696361646F723D22436F6E74726F6C506F724665636861222F3E3C436F6E74726F6C61646F72207469706F3D22506572696F64696369646164222073697374656D613D22312220656E74696461643D22636F6D2E6D65662E6572702E6D6F64656C6F2E656E74696461642E506572696F6469636964616422206964656E746966696361646F723D22436F6E74726F6C506F72456E7469646164222F3E3C2F436F6E74726F6C61646F7265733E3C2F7461626C653E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaBase.fileXml = (data);
                tablaBase.renglonSeleccionado = (true);
                tablaBase.tipoTabla = (listTipoTabla[1]);
                listTablaBase.Add(tablaBase);

                for (i = 0; i < listTablaBase.Count; i++)
                {
                    dbContext.Set<TablaBase>().Add(listTablaBase[i]);
                }

                /*contador = contador + listTablaBase.Count;
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region TABLA DATOS
                //año,mes,dia
                DateTime fecha = new DateTime(2018, 1, 1);
                tablaDatos = new TablaDatos();
                tablaDatos.controlPorFecha = (fecha);
                tablaDatos.controladores = ("");
                tablaDatos.descripcion = ("ISR 2018");
                //ISR
                s = "3C3F786D6C2076657273696F6E3D22312E302220656E636F64696E673D225554462D38223F3E0A3C7461626C613E3C6461746F2069643D2231223E3C4C696D697465496E666572696F723E302E30313C2F4C696D697465496E666572696F723E3C43756F746146696A613E302E303C2F43756F746146696A613E3C506F7263656E74616A653E312E39323C2F506F7263656E74616A653E3C2F6461746F3E3C6461746F2069643D2232223E3C4C696D697465496E666572696F723E3439362E30383C2F4C696D697465496E666572696F723E3C43756F746146696A613E392E35323C2F43756F746146696A613E3C506F7263656E74616A653E362E343C2F506F7263656E74616A653E3C2F6461746F3E3C6461746F2069643D2233223E3C4C696D697465496E666572696F723E343231302E34323C2F4C696D697465496E666572696F723E3C43756F746146696A613E3234372E32333C2F43756F746146696A613E3C506F7263656E74616A653E31302E38383C2F506F7263656E74616A653E3C2F6461746F3E3C6461746F2069643D2234223E3C4C696D697465496E666572696F723E373339392E34333C2F4C696D697465496E666572696F723E3C43756F746146696A613E3539342E32343C2F43756F746146696A613E3C506F7263656E74616A653E31362E303C2F506F7263656E74616A653E3C2F6461746F3E3C6461746F2069643D2235223E3C4C696D697465496E666572696F723E383630312E35313C2F4C696D697465496E666572696F723E3C43756F746146696A613E3738362E35353C2F43756F746146696A613E3C506F7263656E74616A653E31372E39323C2F506F7263656E74616A653E3C2F6461746F3E3C6461746F2069643D2236223E3C4C696D697465496E666572696F723E31303239382E33363C2F4C696D697465496E666572696F723E3C43756F746146696A613E313039302E36323C2F43756F746146696A613E3C506F7263656E74616A653E32312E33363C2F506F7263656E74616A653E3C2F6461746F3E3C6461746F2069643D2237223E3C4C696D697465496E666572696F723E32303737302E333C2F4C696D697465496E666572696F723E3C43756F746146696A613E333332372E34323C2F43756F746146696A613E3C506F7263656E74616A653E32332E35323C2F506F7263656E74616A653E3C2F6461746F3E3C6461746F2069643D2238223E3C4C696D697465496E666572696F723E33323733362E38343C2F4C696D697465496E666572696F723E3C43756F746146696A613E363134312E39353C2F43756F746146696A613E3C506F7263656E74616A653E33302E303C2F506F7263656E74616A653E3C2F6461746F3E3C2F7461626C613E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaDatos.fileXml = (data);
                tablaDatos.renglonSeleccionado = (true);
                tablaDatos.tablaBase = (listTablaBase[0]);
                listTablaDatos.Add(tablaDatos);

                tablaDatos = new TablaDatos();
                tablaDatos.controladores = ("");
                tablaDatos.controlPorFecha = (fecha);
                tablaDatos.descripcion = ("SUBSIDIO 2018");
                //SUBSIDIO
                s = "3C3F786D6C2076657273696F6E3D22312E302220656E636F64696E673D225554462D38223F3E0A3C7461626C613E3C6461746F2069643D2231223E3C4C696D697465496E666572696F723E302E30313C2F4C696D697465496E666572696F723E3C43756F74613E3430372E30323C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2232223E3C4C696D697465496E666572696F723E313736382E39373C2F4C696D697465496E666572696F723E3C43756F74613E3430362E38333C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2233223E3C4C696D697465496E666572696F723E323635332E33393C2F4C696D697465496E666572696F723E3C43756F74613E3430362E36323C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2234223E3C4C696D697465496E666572696F723E333437322E38353C2F4C696D697465496E666572696F723E3C43756F74613E3339322E37373C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2235223E3C4C696D697465496E666572696F723E333533372E38383C2F4C696D697465496E666572696F723E3C43756F74613E3338322E34363C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2236223E3C4C696D697465496E666572696F723E343434362E31363C2F4C696D697465496E666572696F723E3C43756F74613E3335342E32333C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2237223E3C4C696D697465496E666572696F723E343731372E31393C2F4C696D697465496E666572696F723E3C43756F74613E3332342E38373C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2238223E3C4C696D697465496E666572696F723E353333352E34333C2F4C696D697465496E666572696F723E3C43756F74613E3239342E36333C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2239223E3C4C696D697465496E666572696F723E363232342E36383C2F4C696D697465496E666572696F723E3C43756F74613E3235332E35343C2F43756F74613E3C2F6461746F3E3C6461746F2069643D223130223E3C4C696D697465496E666572696F723E373131332E39313C2F4C696D697465496E666572696F723E3C43756F74613E3231372E36313C2F43756F74613E3C2F6461746F3E3C6461746F2069643D223131223E3C4C696D697465496E666572696F723E373338322E33343C2F4C696D697465496E666572696F723E3C43756F74613E302E303C2F43756F74613E3C2F6461746F3E3C2F7461626C613E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaDatos.fileXml = (data);
                tablaDatos.renglonSeleccionado = (true);
                tablaDatos.tablaBase = (listTablaBase[1]);
                listTablaDatos.Add(tablaDatos);

                tablaDatos = new TablaDatos();
                tablaDatos.controladores = ("");
                tablaDatos.controlPorFecha = (fecha);
                tablaDatos.descripcion = ("DIAS FESTIVOS 2018");
                //DIAS FESTIVOS
                s = "3C3F786D6C2076657273696F6E3D22312E302220656E636F64696E673D225554462D38223F3E0A3C7461626C613E3C6461746F2069643D2231223E3C66656368613E30312F30312F323031333C2F66656368613E3C6465736372697063696F6E3E41C3B16F206E7565766F3C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D2232223E3C66656368613E30362F30312F323031333C2F66656368613E3C6465736372697063696F6E3E44696173206465206C6F732073616E746F73202052657965733C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D2233223E3C66656368613E30352F30322F323031333C2F66656368613E3C6465736372697063696F6E3E446961206465206C6120436F6E737469747563696F6E3C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D2234223E3C66656368613E32342F30322F323031333C2F66656368613E3C6465736372697063696F6E3E446961206465206C612062616E64657261204E6163696F6E616C3C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D2235223E3C66656368613E32312F30332F323031333C2F66656368613E3C6465736372697063696F6E3E4E6174616C6963696F2062656E69746F204A756172657A3C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D2236223E3C66656368613E32382F30332F323031333C2F66656368613E3C6465736372697063696F6E3E4A75657665732053616E746F3C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D2237223E3C66656368613E32392F30332F323031333C2F66656368613E3C6465736372697063696F6E3E566965726E65732053616E746F3C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D2238223E3C66656368613E33302F30342F323031333C2F66656368613E3C6465736372697063696F6E3E4469612064656C204E69C3B16F3C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D2239223E3C66656368613E30312F30352F323031333C2F66656368613E3C6465736372697063696F6E3E4469612064656C2074726162616A6F3C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D223130223E3C66656368613E30352F30352F323031333C2F66656368613E3C6465736372697063696F6E3E426174616C6C6120646520707565626C613C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D223131223E3C66656368613E31302F30352F323031333C2F66656368613E3C6465736372697063696F6E3E446961206465206C6173206D61647265733C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D223132223E3C66656368613E30312F30392F323031333C2F66656368613E3C6465736372697063696F6E3E496E666F726D6520507265736964656E6369616C3C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D223133223E3C66656368613E31362F30392F323031333C2F66656368613E3C6465736372697063696F6E3E446961206465206C6120696E646570656E64656E6369613C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D223134223E3C66656368613E30312F31312F323031333C2F66656368613E3C6465736372697063696F6E3E44696120646520746F646F73206C6F732073616E746F733C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D223135223E3C66656368613E30322F31312F323031333C2F66656368613E3C6465736372697063696F6E3E446961206465206C6F73206D756572746F733C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D223136223E3C66656368613E32302F31312F323031333C2F66656368613E3C6465736372697063696F6E3E5265766F6C7563696F6E206D65786963616E613C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D223137223E3C66656368613E31322F31322F323031333C2F66656368613E3C6465736372697063696F6E3E4469612064652076697267656E2064652067756164616C7570653C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D223138223E3C66656368613E32342F31322F323031333C2F66656368613E3C6465736372697063696F6E3E4E6F636865206275656E613C2F6465736372697063696F6E3E3C2F6461746F3E3C6461746F2069643D223139223E3C66656368613E32352F31322F323031333C2F66656368613E3C6465736372697063696F6E3E4E6176696461643C2F6465736372697063696F6E3E3C2F6461746F3E3C2F7461626C613E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaDatos.fileXml = (data);
                tablaDatos.renglonSeleccionado = (true);
                tablaDatos.tablaBase = (listTablaBase[2]);
                listTablaDatos.Add(tablaDatos);

                tablaDatos = new TablaDatos();
                tablaDatos.controladores = ("");
                tablaDatos.controlPorFecha = (fecha);
                tablaDatos.descripcion = ("SALARIOS MÍNIMOS 2018");
                //SALARIOS MÍNIMOS
                s = "3C3F786D6C2076657273696F6E3D22312E302220656E636F64696E673D225554462D38223F3E0A3C7461626C613E0A2020203C6461746F2069643D2231223E0A2020202020203C4465736372697063696F6E3E413C2F4465736372697063696F6E3E0A2020202020203C53616C6172696F3E37302E31303C2F53616C6172696F3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2232223E0A2020202020203C4465736372697063696F6E3E423C2F4465736372697063696F6E3E0A2020202020203C53616C6172696F3E36362E34353C2F53616C6172696F3E0A2020203C2F6461746F3E0A3C2F7461626C613E0A";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaDatos.fileXml = (data);
                tablaDatos.renglonSeleccionado = (true);
                tablaDatos.tablaBase = (listTablaBase[3]);
                listTablaDatos.Add(tablaDatos);

                tablaDatos = new TablaDatos();
                tablaDatos.controladores = ("RazonesSociales0001");
                tablaDatos.descripcion = ("FACTOR DE INTEGRACION 2018");
                //FACTOR DE INTEGRACION
                s = "3C3F786D6C2076657273696F6E3D22312E302220656E636F64696E673D225554462D38223F3E0A3C7461626C613E0A2020203C436F6E74726F6C61646F722063616D706F42757371756564613D22636C617665220A20202020202020202020202020202020656E74696461643D22636F6D2E6D65662E6572702E6D6F64656C6F2E656E74696461642E52617A6F6E6573536F6369616C6573220A202020202020202020202020202020206964656E746966696361646F723D22436F6E74726F6C506F72456E7469646164220A202020202020202020202020202020206E6F6D6272653D22456D707265736173220A2020202020202020202020202020202076616C6F7242757371756564613D2230303031223E456D7072657361205072756562613C2F436F6E74726F6C61646F723E0A2020203C436F6E74726F6C61646F722063616D706F42757371756564613D22636C617665220A20202020202020202020202020202020656E74696461643D22636F6D2E6D65662E6572702E6D6F64656C6F2E656E74696461642E526567697374726F506174726F6E616C220A202020202020202020202020202020206964656E746966696361646F723D22436F6E74726F6C506F72456E7469646164220A202020202020202020202020202020206E6F6D6272653D225265672E506174726F220A2020202020202020202020202020202076616C6F7242757371756564613D22222F3E0A2020203C436F6E74726F6C61646F722063616D706F42757371756564613D22636C617665220A20202020202020202020202020202020656E74696461643D22636F6D2E6D65662E6572702E6D6F64656C6F2E656E74696461642E5469706F4E6F6D696E61220A202020202020202020202020202020206964656E746966696361646F723D22436F6E74726F6C506F72456E7469646164220A202020202020202020202020202020206E6F6D6272653D225469706F206465206E6F6D696E61220A2020202020202020202020202020202076616C6F7242757371756564613D22222F3E0A2020203C436F6E74726F6C61646F722063616D706F42757371756564613D22636C617665220A20202020202020202020202020202020656E74696461643D22636F6D2E6D65662E6572702E6D6F64656C6F2E656E74696461642E43617465676F7269617350756573746F73220A202020202020202020202020202020206964656E746966696361646F723D22436F6E74726F6C506F72456E7469646164220A202020202020202020202020202020206E6F6D6272653D2243617465676F7269612064652070756573746F220A2020202020202020202020202020202076616C6F7242757371756564613D22222F3E0A2020203C6461746F2069643D2231223E0A2020202020203C616E74696775656461643E313C2F616E74696775656461643E0A2020202020203C666163746F723E312E303435323C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E363C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2232223E0A2020202020203C616E74696775656461643E323C2F616E74696775656461643E0A2020202020203C666163746F723E312E303436363C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E383C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2233223E0A2020202020203C616E74696775656461643E333C2F616E74696775656461643E0A2020202020203C666163746F723E312E303437393C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E31303C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2234223E0A2020202020203C616E74696775656461643E343C2F616E74696775656461643E0A2020202020203C666163746F723E312E303439333C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E31323C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2235223E0A2020202020203C616E74696775656461643E393C2F616E74696775656461643E0A2020202020203C666163746F723E312E303530373C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E31343C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2236223E0A2020202020203C616E74696775656461643E31343C2F616E74696775656461643E0A2020202020203C666163746F723E312E303532313C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E31363C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2237223E0A2020202020203C616E74696775656461642F3E0A2020202020203C666163746F723E312E303533343C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E31383C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2238223E0A2020202020203C616E74696775656461643E32343C2F616E74696775656461643E0A2020202020203C666163746F723E312E303534383C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E32303C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2239223E0A2020202020203C616E74696775656461643E32393C2F616E74696775656461643E0A2020202020203C666163746F723E312E303536323C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E32323C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D223130223E0A2020202020203C616E74696775656461643E33343C2F616E74696775656461643E0A2020202020203C666163746F723E312E303537353C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E32343C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A2020203C6461746F2069643D223131223E0A2020202020203C616E74696775656461643E33393C2F616E74696775656461643E0A2020202020203C666163746F723E312E303538393C2F666163746F723E0A2020202020203C64696173416775696E616C646F3E31353C2F64696173416775696E616C646F3E0A2020202020203C646961735661636163696F6E65733E32363C2F646961735661636163696F6E65733E0A2020202020203C7072696D615661636163696F6E616C3E32353C2F7072696D615661636163696F6E616C3E0A2020203C2F6461746F3E0A3C2F7461626C613E0A";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaDatos.fileXml = (data);
                tablaDatos.renglonSeleccionado = (true);
                tablaDatos.tablaBase = (listTablaBase[4]);
                listTablaDatos.Add(tablaDatos);

                tablaDatos = new TablaDatos();
                tablaDatos.controladores = ("");
                tablaDatos.controlPorAnio = (2018);
                tablaDatos.descripcion = ("ISR ANUAL 2018");
                //ISR ANUAL
                s = "3C3F786D6C2076657273696F6E3D22312E302220656E636F64696E673D225554462D38223F3E0A3C7461626C613E0A2020203C6461746F2069643D2231223E0A2020202020203C4C696D697465496E666572696F723E302E30313C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E302E303C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E312E39323C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2232223E0A2020202020203C4C696D697465496E666572696F723E353935322E38353C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E3131342E32393C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E362E343C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2233223E0A2020202020203C4C696D697465496E666572696F723E35303532342E39333C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E323936362E39313C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E31302E38383C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2234223E0A2020202020203C4C696D697465496E666572696F723E38383739332E30353C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E373133302E34383C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E31362E303C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2235223E0A2020202020203C4C696D697465496E666572696F723E3130333231382E30313C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E393433382E34373C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E31372E39323C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2236223E0A2020202020203C4C696D697465496E666572696F723E3132333538302E32313C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E31333038372E33373C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E32312E33363C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2237223E0A2020202020203C4C696D697465496E666572696F723E3234393234332E34393C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E33393932392E30353C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E32332E35323C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2238223E0A2020202020203C4C696D697465496E666572696F723E3339323834312E39373C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E37333730332E34313C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E33302E303C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A2020203C6461746F2069643D2239223E0A2020202020203C4C696D697465496E666572696F723E3735303030302E30313C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E3138303835302E38323C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E33323C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A2020203C6461746F2069643D223130223E0A2020202020203C4C696D697465496E666572696F723E313030303030302E30313C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E3236303835302E38313C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E33343C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A2020203C6461746F2069643D223131223E0A2020202020203C4C696D697465496E666572696F723E333030303030302E30313C2F4C696D697465496E666572696F723E0A2020202020203C43756F746146696A613E3934303835302E38313C2F43756F746146696A613E0A2020202020203C506F7263656E74616A653E33353C2F506F7263656E74616A653E0A2020203C2F6461746F3E0A3C2F7461626C613E0A";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaDatos.fileXml = (data);
                tablaDatos.renglonSeleccionado = (true);
                tablaDatos.tablaBase = (listTablaBase[5]);
                listTablaDatos.Add(tablaDatos);

                tablaDatos = new TablaDatos();
                tablaDatos.controladores = ("");
                tablaDatos.controlPorAnio = (2018);
                tablaDatos.descripcion = ("SUBSIDIO ANUAL 2018");
                //SUBSIDIO ANUAL
                s = "3C3F786D6C2076657273696F6E3D22312E302220656E636F64696E673D225554462D38223F3E0A3C7461626C613E3C6461746F2069643D2231223E3C4C696D697465496E666572696F723E302E30313C2F4C696D697465496E666572696F723E3C43756F74613E343838342E32343C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2232223E3C4C696D697465496E666572696F723E32313232372E36343C2F4C696D697465496E666572696F723E3C43756F74613E343838312E39363C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2233223E3C4C696D697465496E666572696F723E33313834302E36383C2F4C696D697465496E666572696F723E3C43756F74613E343837392E34343C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2234223E3C4C696D697465496E666572696F723E34313637342E32303C2F4C696D697465496E666572696F723E3C43756F74613E343731332E32343C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2235223E3C4C696D697465496E666572696F723E34323435342E35363C2F4C696D697465496E666572696F723E3C43756F74613E343538392E35323C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2236223E3C4C696D697465496E666572696F723E35333335332E39323C2F4C696D697465496E666572696F723E3C43756F74613E343235302E37363C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2237223E3C4C696D697465496E666572696F723E35363630362E32383C2F4C696D697465496E666572696F723E3C43756F74613E333839382E34343C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2238223E3C4C696D697465496E666572696F723E36343032352E31363C2F4C696D697465496E666572696F723E3C43756F74613E333533352E35363C2F43756F74613E3C2F6461746F3E3C6461746F2069643D2239223E3C4C696D697465496E666572696F723E37343639362E31363C2F4C696D697465496E666572696F723E3C43756F74613E333034322E34383C2F43756F74613E3C2F6461746F3E3C6461746F2069643D223130223E3C4C696D697465496E666572696F723E38353336362E39323C2F4C696D697465496E666572696F723E3C43756F74613E323631312E33323C2F43756F74613E3C2F6461746F3E3C6461746F2069643D223131223E3C4C696D697465496E666572696F723E38383538382E30383C2F4C696D697465496E666572696F723E3C43756F74613E302E303C2F43756F74613E3C2F6461746F3E3C2F7461626C613E";
                data = new byte[s.Length / 2];
                for (i = 0; i < s.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(s[i], 16) << 4)
                            + Digit(s[i + 1], 16));
                }
                tablaDatos.fileXml = (data);
                tablaDatos.renglonSeleccionado = (true);
                tablaDatos.tablaBase = (listTablaBase[6]);
                listTablaDatos.Add(tablaDatos);

                for (i = 0; i < listTablaDatos.Count; i++)
                {
                    dbContext.Set<TablaDatos>().Add(listTablaDatos[i]);
                }

                /* contador = contador + listTablaBase.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Reporte de fuente
                int j;
                List<ReporteFuenteDatos> listReportFuenteDatos = new List<ReporteFuenteDatos>();
                ReporteFuenteDatos reporteFuenteDatos = new ReporteFuenteDatos();
                reporteFuenteDatos.clave = ("1");
                reporteFuenteDatos.nombre = ("Informacion Empleado");
                reporteFuenteDatos.nombreEntidad = ("FuenteDatos_Empleados");
                reporteFuenteDatos.orden = (1);
                reporteFuenteDatos.usaFormulas = (true);
                listReportFuenteDatos.Add(reporteFuenteDatos);

                reporteFuenteDatos = new ReporteFuenteDatos();
                reporteFuenteDatos.clave = ("2");
                reporteFuenteDatos.nombre = ("Informacion de los movimientos de la nomina");
                reporteFuenteDatos.nombreEntidad = ("FuenteDatos_Movimientos");
                reporteFuenteDatos.orden = (2);
                reporteFuenteDatos.usaFormulas = (false);
                listReportFuenteDatos.Add(reporteFuenteDatos);

                reporteFuenteDatos = new ReporteFuenteDatos();
                reporteFuenteDatos.clave = ("3");
                reporteFuenteDatos.nombre = ("Informacion Centro costos");
                reporteFuenteDatos.nombreEntidad = ("FuenteDatos_CentroDeCostos");
                reporteFuenteDatos.orden = (3);
                reporteFuenteDatos.usaFormulas = (false);
                listReportFuenteDatos.Add(reporteFuenteDatos);

                reporteFuenteDatos = new ReporteFuenteDatos();
                reporteFuenteDatos.clave = ("4");
                reporteFuenteDatos.nombre = ("Informacion Conceptos de Nomina");
                reporteFuenteDatos.nombreEntidad = ("FuenteDatos_ConcepNomDefi");
                reporteFuenteDatos.orden = (4);
                reporteFuenteDatos.usaFormulas = (false);
                listReportFuenteDatos.Add(reporteFuenteDatos);

                reporteFuenteDatos = new ReporteFuenteDatos();
                reporteFuenteDatos.clave = ("5");
                reporteFuenteDatos.nombre = ("Informacion Departamentos");
                reporteFuenteDatos.nombreEntidad = ("FuenteDatos_Departamentos");
                reporteFuenteDatos.orden = (5);
                reporteFuenteDatos.usaFormulas = (false);
                listReportFuenteDatos.Add(reporteFuenteDatos);

                reporteFuenteDatos = new ReporteFuenteDatos();
                reporteFuenteDatos.clave = ("6");
                reporteFuenteDatos.nombre = ("Informacion registro incapacidades");
                reporteFuenteDatos.nombreEntidad = ("FuenteDatos_RegistroIncapacidad");
                reporteFuenteDatos.orden = (6);
                reporteFuenteDatos.usaFormulas = (false);
                listReportFuenteDatos.Add(reporteFuenteDatos);

                reporteFuenteDatos = new ReporteFuenteDatos();
                reporteFuenteDatos.clave = ("7");
                reporteFuenteDatos.nombre = ("Informacion Ahorros");
                reporteFuenteDatos.nombreEntidad = ("FuenteDatos_AhorrosMovimientos");
                reporteFuenteDatos.orden = (7);
                reporteFuenteDatos.usaFormulas = (false);
                listReportFuenteDatos.Add(reporteFuenteDatos);

                reporteFuenteDatos = new ReporteFuenteDatos();
                reporteFuenteDatos.clave = ("8");
                reporteFuenteDatos.nombre = ("Informacion Creditos");
                reporteFuenteDatos.nombreEntidad = ("FuenteDatos_CreditosMovimientos");
                reporteFuenteDatos.orden = (8);
                reporteFuenteDatos.usaFormulas = (false);
                listReportFuenteDatos.Add(reporteFuenteDatos);

                reporteFuenteDatos = new ReporteFuenteDatos();
                reporteFuenteDatos.clave = ("9");
                reporteFuenteDatos.nombre = ("Informacion Recibos CFDI");
                reporteFuenteDatos.nombreEntidad = ("FuenteDatos_Comprobante");
                reporteFuenteDatos.orden = (9);
                reporteFuenteDatos.usaFormulas = (false);
                listReportFuenteDatos.Add(reporteFuenteDatos);

                reporteFuenteDatos = new ReporteFuenteDatos();
                reporteFuenteDatos.clave = ("10");
                reporteFuenteDatos.nombre = ("Informacion Recibos CFDI desde tablas");
                reporteFuenteDatos.nombreEntidad = ("FuenteDatos_CFDIEmpleado");
                reporteFuenteDatos.orden = (10);
                reporteFuenteDatos.usaFormulas = (false);
                listReportFuenteDatos.Add(reporteFuenteDatos);

                for (j = 0; j < listReportFuenteDatos.Count; j++)
                {
                    dbContext.Set<ReporteFuenteDatos>().Add(listReportFuenteDatos[j]);
                }
                /* contador = contador + listReportFuenteDatos.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Reportes Dinamicos
                // Pendiente
                #endregion

                #region ParametrosConsulta
                // Pendiente
                #endregion

                #region PERMISOS
                Permisos permisos = null;
                for (j = 0; j < listPerfiles.Count; j++)
                {
                    for (int k = 0; k < listVentana.Count; k++)
                    {
                        if (listVentana[k].tipoVentana == TipoVentana.CATALOGO || listVentana[k].tipoVentana == TipoVentana.CATALOGODIALOG)
                        {
                            permisos = new Permisos();
                            permisos.consultar = (true);
                            permisos.eliminar = (true);
                            permisos.imprimir = (true);
                            permisos.insertar = (true);
                            permisos.modificar = (true);
                            permisos.ventana = (listVentana[k]);
                            permisos.perfiles = (listPerfiles[j]);
                            dbContext.Set<Permisos>().Add(permisos);
                            contador++;
                            /*if (contador % rango == 0 & contador > 0)
                            {
                                session.flush();
                                session.clear();
                                contador = 0;
                            }*/
                        }
                    }

                }
                dbContext.SaveChanges();
                #endregion

                mensaje.resultado = "The database master was succesful created";
                mensaje.noError = (0);
                mensaje.error = ("");

                dbContext.Database.CurrentTransaction.Commit();
                dbContext.Database.Connection.Close();
            }

            catch (DbEntityValidationException eu)
            {
                foreach (var eve in eu.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("inicializarMaster()1_Error: ").Append(ex));
            }


            return mensaje;
        }


        public Mensaje inicializarSimple(DbContext dbContextSimple)
        {
            mensaje = new Mensaje();
            DbContext dbContext = dbContextSimple;
            listPaises = new List<Paises>();
            Paises paises = null;
            listEstados = new List<Estados>();
            Estados estados = null;
            listMunicipios = new List<Municipios>();
            Municipios municipios = null;
            listCiudades = new List<Ciudades>();
            Ciudades ciudades = null;
            listCp = new List<Cp>();
            Cp cp = null;
            List<RazonesSociales> listRazonesSociales = new List<RazonesSociales>();
            RazonesSociales razonesSociales = null;
            RegimenContratacion regimenContratacion = null;
            List<RegimenContratacion> listRegimenContratacion = new List<RegimenContratacion>();
            List<Periodicidad> listPeriodicidad = new List<Periodicidad>();
            Periodicidad periodicidad = null;
            List<Monedas> listMonedas = new List<Monedas>();
            Monedas monedas = null;
            List<BaseNomina> listBaseNomina = new List<BaseNomina>();
            BaseNomina baseNomina = null;
            List<TipoCorrida> listTipoCorrida = new List<TipoCorrida>();
            TipoCorrida tipoCorrida = null;
            List<CampoDIM> listCampoDIM = new List<CampoDIM>();
            CampoDIM campoDIM = null;
            List<Incidencias> listIncidencias = new List<Incidencias>();
            Incidencias incidencias = null;
            List<Excepciones> listExcepciones = new List<Excepciones>();
            Excepciones excepciones = null;
            List<ConfiguraMovimiento> listConfiguraMovimiento = new List<ConfiguraMovimiento>();
            ConfiguraMovimiento configuraMovimiento = null;
            List<ConfigAsistencias> listConfiguraAsistencias = new List<ConfigAsistencias>();
            ConfigAsistencias configuraAsistencias = null;
            List<ConceptoDeNomina> listConceptoDeNominas = new List<ConceptoDeNomina>();
            ConceptoDeNomina conceptoDeNomina = null;
            List<ParaConcepDeNom> listParaConcepDeNom = new List<ParaConcepDeNom>();
            ParaConcepDeNom paraConcepDeNom = null;
            List<ConcepNomDefi> listConceptoDeNominaDefinicion = new List<ConcepNomDefi>();
            ConcepNomDefi conceptoDeNominaDefinicion = null;
            List<TipoNomina> listTipoNomina = new List<TipoNomina>();
            TipoNomina tipoNomina = null;
            List<RegistroPatronal> listRegistroPatronal = new List<RegistroPatronal>();
            RegistroPatronal registroPatronal = null;
            List<CentroDeCosto> listCentroDeCosto = new List<CentroDeCosto>();
            CentroDeCosto centroDeCosto = null;
            List<CategoriasPuestos> listCategoriaPuestos = new List<CategoriasPuestos>();
            CategoriasPuestos categoriasPuestos = null;
            List<Puestos> listPuestos = new List<Puestos>();
            Puestos puestos = null;
            List<Departamentos> listDepartamentos = new List<Departamentos>();
            Departamentos departamentos = null;
            List<Jornada> listJornada = new List<Jornada>();
            Jornada jornada = null;
            List<Horario> listHorario = new List<Horario>();
            Horario horario = null;
            List<Turnos> listTurnos = new List<Turnos>();
            Turnos turnos = null;
            List<Plazas> listPlazas = new List<Plazas>();
            Plazas plazas = null;
            List<Empleados> listEmpleado = new List<Empleados>();
            Empleados empleados = null;
            List<FormasDePago> listFormasDePago = new List<FormasDePago>();
            FormasDePago formasDePago = null;
            List<TipoContrato> listTipoContratos = new List<TipoContrato>();
            TipoContrato tipoContrato = null;
            List<Bancos> listBancos = new List<Bancos>();
            Bancos bancos = null;
            List<Parentesco> listParentesco = new List<Parentesco>();
            Parentesco parentesco = null;
            List<Cursos> listCursos = new List<Cursos>();
            Cursos cursos = null;
            List<Estudios> listEstudios = new List<Estudios>();
            Estudios estudios = null;
            List<Firmas> listFirmas = new List<Firmas>();
            Firmas firmas = null;
            List<PeriodosNomina> listPeriodosNominas = new List<PeriodosNomina>();
            PeriodosNomina periodoNomina;
            List<CreditoAhorro> listCreditoAhorro = new List<CreditoAhorro>();
            CreditoAhorro creditoAhorro;
            List<DatosDisponiblesCxnConta> listDatosDisponibleCxnConta = new List<DatosDisponiblesCxnConta>();
            DatosDisponiblesCxnConta datosDisponiblesCxnConta = null;
            List<TiposVacaciones> listTiposVacaciones = new List<TiposVacaciones>();
            TiposVacaciones tiposVacaciones = null;
            List<CausaDeBaja> listCausaDeBaja = new List<CausaDeBaja>();
            CausaDeBaja causaDeBaja = null;

            int i, contador = 0;
            try
            {
                dbContext.Database.BeginTransaction();

                /*evalua si ya esta inicializada base de datos*/
                bool existe = (from b in dbContext.Set<Bancos>()
                               select b).Count() > 0 ? true : false;
                if (existe)
                {
                    mensaje.resultado = "This database has already been created.";
                    mensaje.noError = 0;
                    mensaje.error = "Existe";
                    dbContext.Database.CurrentTransaction.Commit();
                    return mensaje;
                }

                #region Paises
                paises = new Paises();
                paises.clave = ("MEX");
                paises.descripcion = ("Mexico");
                paises.nacionalidad = ("Mexicana");
                listPaises.Add(paises);

                paises = new Paises();
                paises.clave = ("USA");
                paises.descripcion = ("Estados Unidos");
                paises.nacionalidad = ("Estadounidense");
                listPaises.Add(paises);

                paises = new Paises();
                paises.clave = ("CAN");
                paises.descripcion = ("Canada");
                paises.nacionalidad = ("Canadiense");
                listPaises.Add(paises);

                for (i = 0; i < listPaises.Count; i++)
                {
                    dbContext.Set<Paises>().Add(listPaises[i]);
                }

                /* contador = contador + listPaises.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Estados
                estados = new Estados();
                estados.clave = ("AGU");
                estados.descripcion = ("Aguascalientes");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("BCN");
                estados.descripcion = ("Baja California");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("BCS");
                estados.descripcion = ("Baja California Sur");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("CAM");
                estados.descripcion = ("Campeche");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("CHP");
                estados.descripcion = ("Chiapas");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("CHH");
                estados.descripcion = ("Chihuahua");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("COA");
                estados.descripcion = ("Coahuila");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("COL");
                estados.descripcion = ("Colima");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("DIF");
                estados.descripcion = ("Ciudad de México");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("DUR");
                estados.descripcion = ("Durango");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("GUA");
                estados.descripcion = ("Guanajuato");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("GRO");
                estados.descripcion = ("Guerrero");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("HID");
                estados.descripcion = ("Hidalgo");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("JAL");
                estados.descripcion = ("Jalisco");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MEX");
                estados.descripcion = ("Estado de México");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MIC");
                estados.descripcion = ("Michoacán");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MOR");
                estados.descripcion = ("Morelos");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NAY");
                estados.descripcion = ("Nayarit");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NLE");
                estados.descripcion = ("Nuevo León");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("OAX");
                estados.descripcion = ("Oaxaca");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("PUE");
                estados.descripcion = ("Puebla");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("QUE");
                estados.descripcion = ("Querétaro");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("ROO");
                estados.descripcion = ("Quintana Roo");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("SLP");
                estados.descripcion = ("San Luis Potosí");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("SIN");
                estados.descripcion = ("Sinaloa");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("SON");
                estados.descripcion = ("Sonora");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("TAB");
                estados.descripcion = ("Tabasco");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("TAM");
                estados.descripcion = ("Tamaulipas");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("TLA");
                estados.descripcion = ("Tlaxcala");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("VER");
                estados.descripcion = ("Veracruz");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("YUC");
                estados.descripcion = ("Yucatán");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("ZAC");
                estados.descripcion = ("Zacatecas");
                estados.paises_ID = (listPaises[0].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("AL");
                estados.descripcion = ("Alabama");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("Ak");
                estados.descripcion = ("Alaska");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("AZ");
                estados.descripcion = ("Arizona");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("AR");
                estados.descripcion = ("Arkansas");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("CA");
                estados.descripcion = ("California");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NC");
                estados.descripcion = ("Carolina del Norte");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("SC");
                estados.descripcion = ("Carolina del Sur");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("CO");
                estados.descripcion = ("Colorado");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("CT");
                estados.descripcion = ("Connecticut");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("ND");
                estados.descripcion = ("Dakota del Norte");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("SD");
                estados.descripcion = ("Dakota del Sur");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("DE");
                estados.descripcion = ("Delaware");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("FL");
                estados.descripcion = ("Florida");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("GA");
                estados.descripcion = ("Georgia");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("HI");
                estados.descripcion = ("Hawái");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("ID");
                estados.descripcion = ("Idaho");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("IL");
                estados.descripcion = ("Illinois");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("IN");
                estados.descripcion = ("Indiana");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("AI");
                estados.descripcion = ("Iowa");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("KS");
                estados.descripcion = ("Kansas");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("KY");
                estados.descripcion = ("Kentucky");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("LA");
                estados.descripcion = ("Luisiana");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("ME");
                estados.descripcion = ("Maine");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MD");
                estados.descripcion = ("Maryland");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MA");
                estados.descripcion = ("Massachusetts");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MI");
                estados.descripcion = ("Michigan");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MN");
                estados.descripcion = ("Minesota");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MS");
                estados.descripcion = ("Misisipi");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MO");
                estados.descripcion = ("Misuri");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MT");
                estados.descripcion = ("Montana");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NE");
                estados.descripcion = ("Nebraska");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NV");
                estados.descripcion = ("Nevada");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NJ");
                estados.descripcion = ("Nueva Jersey");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NY");
                estados.descripcion = ("Nueva York");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NH");
                estados.descripcion = ("Nuevo Hampshire");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NM");
                estados.descripcion = ("Nuevo México");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("OH");
                estados.descripcion = ("Ohio");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("OK");
                estados.descripcion = ("Oklahoma");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("OR");
                estados.descripcion = ("Oregon");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("PA");
                estados.descripcion = ("Pensilvania");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("RI");
                estados.descripcion = ("Rhode Island");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("TN");
                estados.descripcion = ("Tennessee");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("TX");
                estados.descripcion = ("Texas");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("UT");
                estados.descripcion = ("Utah");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("VT");
                estados.descripcion = ("Vermont");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("VA");
                estados.descripcion = ("Virginia");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("WV");
                estados.descripcion = ("Virginia Occidental");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("WA");
                estados.descripcion = ("Washington");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("WI");
                estados.descripcion = ("Wisconsin");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("WY");
                estados.descripcion = ("Wyoming");
                estados.paises_ID = (listPaises[1].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("ON");
                estados.descripcion = ("Ontario");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("QC");
                estados.descripcion = ("Quebec");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NS");
                estados.descripcion = ("Nueva Escocia");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NB");
                estados.descripcion = ("Nuevo Brunswick ");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("MB");
                estados.descripcion = ("Manitoba");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("BC");
                estados.descripcion = ("Columbia Británica");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("PE");
                estados.descripcion = (" Isla del Príncipe Eduardo");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("SK");
                estados.descripcion = ("Saskatchewan");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("AB");
                estados.descripcion = ("Alberta");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NL");
                estados.descripcion = ("Terranova y Labrador");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("NT");
                estados.descripcion = ("Territorios del Noroeste");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("YT");
                estados.descripcion = ("Yukón");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                estados = new Estados();
                estados.clave = ("UN");
                estados.descripcion = ("Nunavut");
                estados.paises_ID = (listPaises[2].id);
                listEstados.Add(estados);

                for (i = 0; i < listEstados.Count; i++)
                {
                    dbContext.Set<Estados>().Add(listEstados[i]);
                }

                /* contador = contador + listEstados.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Municipios
                municipios = new Municipios();
                municipios.clave = ("001");
                municipios.descripcion = ("Aguascalientes");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("002");
                municipios.descripcion = ("Asientos");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("003");
                municipios.descripcion = ("Calvillo");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("004");
                municipios.descripcion = ("Cosío");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("005");
                municipios.descripcion = ("Jesús María");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("006");
                municipios.descripcion = ("Pabellón de Arteaga");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("007");
                municipios.descripcion = ("Rincón de Romos");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("008");
                municipios.descripcion = ("San José de Gracia");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("009");
                municipios.descripcion = ("Tepezalá");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("010");
                municipios.descripcion = ("El Llano");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("011");
                municipios.descripcion = ("San Francisco de los Romo");
                municipios.estados_ID = (listEstados[0].id);
                listMunicipios.Add(municipios);

                //Municipios de Sinaloa
                municipios = new Municipios();
                municipios.clave = ("012");
                municipios.descripcion = ("Ahome");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("013");
                municipios.descripcion = ("Angostura");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("014");
                municipios.descripcion = ("Badiraguato");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("015");
                municipios.descripcion = ("Concordia");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("016");
                municipios.descripcion = ("Cosalá");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("017");
                municipios.descripcion = ("Culiacán");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("018");
                municipios.descripcion = ("Choix");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("019");
                municipios.descripcion = ("Elota");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("020");
                municipios.descripcion = ("Escuinapa");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("021");
                municipios.descripcion = ("El Fuerte");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("022");
                municipios.descripcion = ("Guasave");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("023");
                municipios.descripcion = ("Mazatlán");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("024");
                municipios.descripcion = ("Mocorito");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("025");
                municipios.descripcion = ("Rosario");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("026");
                municipios.descripcion = ("Salvador Alvarado");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("027");
                municipios.descripcion = ("San Ignacio");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("028");
                municipios.descripcion = ("Sinaloa");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                municipios = new Municipios();
                municipios.clave = ("029");
                municipios.descripcion = ("Navolato");
                municipios.estados_ID = (listEstados[24].id);
                listMunicipios.Add(municipios);

                for (i = 0; i < listMunicipios.Count; i++)
                {
                    dbContext.Set<Municipios>().Add(listMunicipios[i]);
                }
                contador = contador + listMunicipios.Count();
                /* if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Ciudades
                ciudades = new Ciudades();
                ciudades.clave = ("0001");
                ciudades.descripcion = ("Los Mochis");
                ciudades.municipios_ID = (listMunicipios[11].id);
                listCiudades.Add(ciudades);

                ciudades = new Ciudades();
                ciudades.clave = ("0002");
                ciudades.descripcion = ("Ahome");
                ciudades.municipios_ID = (listMunicipios[11].id);
                listCiudades.Add(ciudades);

                ciudades = new Ciudades();
                ciudades.clave = ("0003");
                ciudades.descripcion = ("Higuera de Zaragoza");
                ciudades.municipios_ID = (listMunicipios[11].id);
                listCiudades.Add(ciudades);

                ciudades = new Ciudades();
                ciudades.clave = ("0004");
                ciudades.descripcion = ("Topolobampo");
                ciudades.municipios_ID = (listMunicipios[11].id);
                listCiudades.Add(ciudades);

                ciudades = new Ciudades();
                ciudades.clave = ("0005");
                ciudades.descripcion = ("Culiacán Rosales");
                ciudades.municipios_ID = (listMunicipios[16].id);
                listCiudades.Add(ciudades);

                for (i = 0; i < listCiudades.Count; i++)
                {

                    dbContext.Set<Ciudades>().Add(listCiudades[i]);
                }

                /* contador = contador + listCiudades.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Cp
                String descripcion;
                cp = new Cp();
                descripcion = "81200";
                cp.clave = (descripcion);
                descripcion = cp.clave + " " + listCiudades[0].descripcion;
                if (descripcion.Length > 30)
                {
                    cp.descripcion = (descripcion.Substring(0, 30));
                }
                else
                {
                    cp.descripcion = (descripcion);
                }
                cp.ciudades_ID = (listCiudades[0].id);
                listCp.Add(cp);

                cp = new Cp();
                descripcion = "81315";
                cp.clave = (descripcion);
                descripcion = cp.clave + " " + listCiudades[1].descripcion;
                if (descripcion.Length > 30)
                {
                    cp.descripcion = (descripcion.Substring(0, 30));
                }
                else
                {
                    cp.descripcion = (descripcion);
                }
                cp.ciudades_ID = (listCiudades[1].id);
                listCp.Add(cp);

                cp = new Cp();
                descripcion = "81330";
                cp.clave = (descripcion);
                descripcion = cp.clave + " " + listCiudades[2].descripcion;
                if (descripcion.Length > 30)
                {
                    cp.descripcion = (descripcion.Substring(0, 30));
                }
                else
                {
                    cp.descripcion = (descripcion);
                }
                cp.ciudades_ID = (listCiudades[2].id);
                listCp.Add(cp);

                cp = new Cp();
                descripcion = "81370";
                cp.clave = (descripcion);
                descripcion = cp.clave + " " + listCiudades[3].descripcion;
                if (descripcion.Length > 30)
                {
                    cp.descripcion = (descripcion.Substring(0, 30));
                }
                else
                {
                    cp.descripcion = (descripcion);
                }
                cp.ciudades_ID = (listCiudades[3].id);
                listCp.Add(cp);

                cp = new Cp();
                descripcion = "80000";
                cp.clave = (descripcion);
                descripcion = cp.clave + " " + listCiudades[4].descripcion;
                if (descripcion.Length > 30)
                {
                    cp.descripcion = (descripcion.Substring(0, 30));
                }
                else
                {
                    cp.descripcion = (descripcion);
                }
                cp.ciudades_ID = (listCiudades[4].id);
                listCp.Add(cp);

                for (i = 0; i < listCp.Count; i++)
                {
                    dbContext.Set<Cp>().Add(listCp[i]);
                }

                contador = contador + listCp.Count();
                /* if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;

                 }*/
                dbContext.SaveChanges();
                #endregion

                #region ConfiguraTimbrado
                List<ConfiguraTimbrado> listConfiguraTimbrados = new List<ConfiguraTimbrado>();
                ConfiguraTimbrado configuraTimbrado = new ConfiguraTimbrado();
                configuraTimbrado.clave = ("1");
                configuraTimbrado.contraseña = ("N1hTyjCbltI=");
                configuraTimbrado.descripcion = ("BIINFORM");
                configuraTimbrado.URL = ("http://biinform.com.mx/srvCFDiprueba/service1.asmx");
                configuraTimbrado.usuario = ("prueba");
                configuraTimbrado.principal = (true);
                listConfiguraTimbrados.Add(configuraTimbrado);

                configuraTimbrado = new ConfiguraTimbrado();
                configuraTimbrado.clave = ("2");
                configuraTimbrado.contraseña = ("N1hTyjCbltI=");
                configuraTimbrado.descripcion = ("MACROPRO");
                configuraTimbrado.URL = ("http://portalmacropro.com.mx/srvCFDiprueba/service1.asmx");
                configuraTimbrado.usuario = ("prueba");
                configuraTimbrado.principal = (false);
                listConfiguraTimbrados.Add(configuraTimbrado);

                for (i = 0; i < listConfiguraTimbrados.Count; i++)
                {
                    dbContext.Set<ConfiguraTimbrado>().Add(listConfiguraTimbrados[i]);
                }

                /* contador = contador + listConfiguraTimbrados.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region RazonesSociales
                razonesSociales = new RazonesSociales();
                razonesSociales.clave = ("0001");
                razonesSociales.razonsocial = ("Empresa Prueba");
                razonesSociales.rfc = ("AAA010101AAA");
                razonesSociales.representantelegal = ("Representante Prueba");
                razonesSociales.telefono = ("9999999999");
                razonesSociales.calle = ("Calle Desconocido");
                razonesSociales.colonia = ("Colonia Desconocido");
                razonesSociales.cp_ID = (listCp[0].id);
                razonesSociales.ciudades_ID = (listCiudades[0].id);
                razonesSociales.municipios_ID = (listMunicipios[11].id);
                razonesSociales.estados_ID = (listEstados[24].id);
                razonesSociales.paises_ID = (listPaises[0].id);
                razonesSociales.numeroex = ("000");
                razonesSociales.numeroin = ("000");
                razonesSociales.folio = ("000000000001");
                razonesSociales.regimenFiscal = ("601");
                String file = "3082046130820349A00302010202143230303031303030303030323030303031343238300D06092A864886F70D01010505003082015C311A301806035504030C11412E432E20322064652070727565626173312F302D060355040A0C26536572766963696F2064652041646D696E69737472616369C3B36E205472696275746172696131383036060355040B0C2F41646D696E69737472616369C3B36E20646520536567757269646164206465206C6120496E666F726D616369C3B36E3129302706092A864886F70D010901161A617369736E657440707275656261732E7361742E676F622E6D783126302406035504090C1D41762E20486964616C676F2037372C20436F6C2E20477565727265726F310E300C06035504110C053036333030310B3009060355040613024D583119301706035504080C10446973747269746F204665646572616C3112301006035504070C09436F796F6163C3A16E3134303206092A864886F70D0109020C25526573706F6E7361626C653A2041726163656C692047616E64617261204261757469737461301E170D3133303530373136303132395A170D3137303530373136303132395A3081DB3129302706035504031320414343454D20534552564943494F5320454D50524553415249414C45532053433129302706035504291320414343454D20534552564943494F5320454D50524553415249414C455320534331293027060355040A1320414343454D20534552564943494F5320454D50524553415249414C455320534331253023060355042D131C414141303130313031414141202F2048454754373631303033345332311E301C06035504051315202F20484547543736313030334D44464E535230383111300F060355040B130870726F647563746F30819F300D06092A864886F70D010101050003818D0030818902818100A4BF6DE515CBA13768E0DA36E2DDD92DCF5DA42B7B4B46C66613794C5AE79C76B7FEFDD7DB6D80EA4A17A84F1792AF958878CEDA90A8FE32115F878C7EF2CBFD9DFFE7BBB7E27AD634EB6DD2EA4F7583D3EB397C22FA24EAC5E21AB2FFF10675FBD87ED74644DAD91FE7408D65A69764BF86B37D694C6B692E6AA8ED5BEDBB650203010001A31D301B300C0603551D130101FF04023000300B0603551D0F0404030206C0300D06092A864886F70D01010505000382010100023D7016657D83B8A89956FDF9452D5614A813DAE6D43BD0BE6F27D96A957A67360D3A20382A6A64A8753F6E4647A8A8D1F91E7D86862155D9FE0A550896265FD8284787CB346B4BC144D2772077C0D5A916773A1C6192D78F3DE6D04AF7D4163EB92DEA594B4FB0E346543C992FA21404152516879C4213FD00EDFD7ABF3321B794D9BD0C3012710ADE2F6C3285BD3F62DD5D0FBA8A3E9269A47F5E43DAB04647E76B77C1FC620F130C614A7F6CCA991B204FB9C87A8BBA5A97D8A79EF4588BCB25063E7EC329DF7E03D39529E72918480C5419ADF445D533038AF1379853F96F708565460FCF130F7DCDC39E6E35151C44EDD95151E6756446F868E195FB03";
                byte[] data = new byte[file.Length / 2];
                for (i = 0; i < file.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(file[i], 16) << 4)
                            + Digit(file[i + 1], 16));
                }
                razonesSociales.certificadoSAT = (data);
                file = "308202C6304006092A864886F70D01050D3033301B06092A864886F70D01050C300E040802010002818100A402020400301406082A864886F70D03070408308202760201003004820280FE5B4A92EDCBBAA5752A56BDF1300BDF9DF4EF99D6333C78D7878201E399B6B7AE884A1CCF690ACDCE3F2ACB50440DAA2B47FB18307E323C2F9E8C87E63FEA8BB940B1B0D69048F2DCF54624A4C3F9E4B3B97F7B2AE0229C82820C29541C84E7EF516CC5CF5E91C63588920C196EC779B47CA360C08CC19653374E66C5D00DFABF4715B181F5702512543295FE325AD7EDE1320107AC4EA623BE94A197273C9CB2D2BE49F1696FBFD1E0B61A106F15EB27467EACC36547BFF28488D91C0EED1373916A546E55CDB95B9C3FB3DCAED4166EAE3FD001B01629534D64F20271BBC53B83192B90187FDCB3A4C41C73FA13A1A33B641B6B5554982F77641E9E72D402E6EB42272414BF08F3DCCF652C3B979CD392C1009FFD7523A47F0D16442031EA0C5ED0C5B10D27DC9DDFF7CECE2B7B965ED18C085F12DCD878397D055271B9CCFEFE7664B71038C8E1E5D4C4699DDA28055BC05F74F51B18BB1DECAEC7E88A289077CFFE636C5BC20733B2207E5EC41D0FAC6C114DE4A1F259046F2CAE6CC10F2413715363991AD2247CBCAB90E7F0F766DFC8BF163B6C415F85CA0F9B237E64F29C132AA93D4A6C817485962A6CA8286B74D419B4B8015537ECE92E233BA00F0391042D0A2F50DCCC1AA5DDDAE41FF25CF1CA972C45223093D7D9D11F84475F43CCC2337EBD43A76162BDBC83CAB1E3E08F205EC0DF7EB5BA54E8DE8EFEC3E5CFEBACE37C6E8B0E2625CA8D716BAD3D8605D4D92BF4A6EE9EA30064219562ED7559AEE56A473085DE22AD63AC4B8333A56C5023003D47B3CEF6C05BD49955D51B3A7B10309B7B959C4020B386C24053157E70D89B13F4385BBE081B7FD7782910D71ADEF29EA5588A7F56F3ECDEB1C907636C04428FD36380E1164249EDAA7D";
                data = new byte[file.Length / 2];
                for (i = 0; i < file.Length; i += 2)
                {
                    data[i / 2] = (byte)((Digit(file[i], 16) << 4)
                            + Digit(file[i + 1], 16));
                }
                razonesSociales.llaveSAT = (data);
                razonesSociales.rutaCert = ("CSD01_AAA010101AAA.cer");
                razonesSociales.rutaLlave = ("CSD01_AAA010101AAA.key");
                razonesSociales.password = ("bqs/BQFE5V7lnyzo7VPN2w==");
                razonesSociales.ubicacionXML = ("C:\\Temp");
                razonesSociales.descripcionRecibo = ("Recibo por razon social");
                // razonesSociales.serie = ("PR"); /*FIX THIS BY SERIE*/
                listRazonesSociales.Add(razonesSociales);

                for (i = 0; i < listRazonesSociales.Count; i++)
                {
                    dbContext.Set<RazonesSociales>().Add(listRazonesSociales[i]);
                }

                /* contador = contador + listRazonesSociales.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region RegimenContratacion

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("02");
                regimenContratacion.descripcion = ("Sueldos");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("03");
                regimenContratacion.descripcion = ("Jubilados");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("04");
                regimenContratacion.descripcion = ("Pensionados");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("05");
                regimenContratacion.descripcion = ("Asimilados Miembros Sociedades Cooperativas Produccion");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("06");
                regimenContratacion.descripcion = ("Asimilados Integrantes Sociedades Asociaciones Civiles");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("07");
                regimenContratacion.descripcion = ("Asimilados Miembros consejos");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("08");
                regimenContratacion.descripcion = ("Asimilados comisionistas");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("09");
                regimenContratacion.descripcion = ("Asimilados Honorarios");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("10");
                regimenContratacion.descripcion = ("Asimilados acciones");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("11");
                regimenContratacion.descripcion = ("Asimilados otros");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("12");
                regimenContratacion.descripcion = ("Jubilados o Pensionados");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("13");
                regimenContratacion.descripcion = ("Indemnización o Separación");
                listRegimenContratacion.Add(regimenContratacion);

                regimenContratacion = new RegimenContratacion();
                regimenContratacion.clave = ("99");
                regimenContratacion.descripcion = ("Otro Regimen");
                listRegimenContratacion.Add(regimenContratacion);

                for (i = 0; i < listRegimenContratacion.Count; i++)
                {
                    dbContext.Set<RegimenContratacion>().Add(listRegimenContratacion[i]);
                }
                dbContext.SaveChanges();

                #endregion

                #region Periodicidad
                periodicidad = new Periodicidad();
                periodicidad.clave = ("01");
                periodicidad.descripcion = ("Diario");
                periodicidad.dias = (1L);
                listPeriodicidad.Add(periodicidad);

                periodicidad = new Periodicidad();
                periodicidad.clave = ("02");
                periodicidad.descripcion = ("Semanal");
                periodicidad.dias = (7L);
                listPeriodicidad.Add(periodicidad);

                periodicidad = new Periodicidad();
                periodicidad.clave = ("03");
                periodicidad.descripcion = ("Catorcenal");
                periodicidad.dias = (14L);
                listPeriodicidad.Add(periodicidad);

                periodicidad = new Periodicidad();
                periodicidad.clave = ("04");
                periodicidad.descripcion = ("Quincenal");
                periodicidad.dias = (15L);
                listPeriodicidad.Add(periodicidad);

                periodicidad = new Periodicidad();
                periodicidad.clave = ("05");
                periodicidad.descripcion = ("Mensual");
                periodicidad.dias = (30L);
                listPeriodicidad.Add(periodicidad);

                periodicidad = new Periodicidad();
                periodicidad.clave = ("06");
                periodicidad.descripcion = ("Bimestral");
                periodicidad.dias = (60L);
                listPeriodicidad.Add(periodicidad);

                periodicidad = new Periodicidad();
                periodicidad.clave = ("07");
                periodicidad.descripcion = ("Unidad obra");
                periodicidad.dias = (1L);
                listPeriodicidad.Add(periodicidad);

                periodicidad = new Periodicidad();
                periodicidad.clave = ("08");
                periodicidad.descripcion = ("Comisión");
                periodicidad.dias = (1L);
                listPeriodicidad.Add(periodicidad);

                periodicidad = new Periodicidad();
                periodicidad.clave = ("09");
                periodicidad.descripcion = ("Precio alzado");
                periodicidad.dias = (1L);
                listPeriodicidad.Add(periodicidad);

                periodicidad = new Periodicidad();
                periodicidad.clave = ("99");
                periodicidad.descripcion = ("Otra Periodicidad");
                periodicidad.dias = (1L);
                listPeriodicidad.Add(periodicidad);

                for (i = 0; i < listPeriodicidad.Count; i++)
                {
                    dbContext.Set<Periodicidad>().Add(listPeriodicidad[i]);
                }

                contador = contador + listPeriodicidad.Count();
                /*if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region Monedas
                monedas = new Monedas();
                monedas.clave = ("1");
                monedas.identificador = ("MXN");
                monedas.simbolo = ("$");
                monedas.centimosSingular = ("Centavo");
                monedas.centimosPlural = ("Centavos");
                monedas.generoCentimos = (false);
                monedas.monedaSingular = ("Peso");
                monedas.monedaPlural = ("Pesos");
                monedas.generoMoneda = (false);
                monedas.decimales = (3);
                listMonedas.Add(monedas);

                monedas = new Monedas();
                monedas.clave = ("2");
                monedas.identificador = ("Dollar");
                monedas.simbolo = ("$");
                monedas.centimosSingular = ("penny");
                monedas.centimosPlural = ("pennys");
                monedas.generoCentimos = (false);
                monedas.monedaSingular = ("Dollar");
                monedas.monedaPlural = ("Dollars");
                monedas.generoMoneda = (false);
                monedas.decimales = (3);
                listMonedas.Add(monedas);

                for (i = 0; i < listMonedas.Count; i++)
                {
                    dbContext.Set<Monedas>().Add(listMonedas[i]);
                }

                /*contador = contador + listMonedas.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region Base Nomina
                baseNomina = new BaseNomina();
                baseNomina.clave = ("01");
                baseNomina.descripcion = ("ISR");
                baseNomina.reservado = (true);
                listBaseNomina.Add(baseNomina);

                baseNomina = new BaseNomina();
                baseNomina.clave = ("02");
                baseNomina.descripcion = ("IMSS");
                baseNomina.reservado = (true);
                listBaseNomina.Add(baseNomina);

                baseNomina = new BaseNomina();
                baseNomina.clave = ("03");
                baseNomina.descripcion = ("INFONAVIT");
                baseNomina.reservado = (true);
                listBaseNomina.Add(baseNomina);

                baseNomina = new BaseNomina();
                baseNomina.clave = ("04");
                baseNomina.descripcion = ("PTU");
                baseNomina.reservado = (true);
                listBaseNomina.Add(baseNomina);

                baseNomina = new BaseNomina();
                baseNomina.clave = ("05");
                baseNomina.descripcion = ("ISN");
                baseNomina.reservado = (true);
                listBaseNomina.Add(baseNomina);

                baseNomina = new BaseNomina();
                baseNomina.clave = ("06");
                baseNomina.descripcion = ("Despensa");
                baseNomina.reservado = (true);
                listBaseNomina.Add(baseNomina);

                baseNomina = new BaseNomina();
                baseNomina.clave = ("07");
                baseNomina.descripcion = ("Fondo Ahorro");
                baseNomina.reservado = (true);
                listBaseNomina.Add(baseNomina);

                baseNomina = new BaseNomina();
                baseNomina.clave = ("08");
                baseNomina.descripcion = ("Aguinaldo");
                baseNomina.reservado = (true);
                listBaseNomina.Add(baseNomina);

                for (i = 0; i < listBaseNomina.Count; i++)
                {
                    dbContext.Set<BaseNomina>().Add(listBaseNomina[i]);
                }

                /*contador = contador + listBaseNomina.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region Tipos Corrida
                tipoCorrida = new TipoCorrida();
                tipoCorrida.clave = ("PER");
                tipoCorrida.descripcion = ("Periodica");
                tipoCorrida.sistema = (true);
                tipoCorrida.orden = ((short)1);
                tipoCorrida.tipoDeCalculo = ((short)1);
                tipoCorrida.usaCorrPeriodica = (true);
                tipoCorrida.mostrarMenuCalc = (true);
                listTipoCorrida.Add(tipoCorrida);

                tipoCorrida = new TipoCorrida();
                tipoCorrida.clave = ("VAC");
                tipoCorrida.descripcion = ("Vacaciones");
                tipoCorrida.sistema = (true);
                tipoCorrida.orden = ((short)2);
                tipoCorrida.tipoDeCalculo = ((short)6);
                tipoCorrida.usaCorrPeriodica = (true);
                tipoCorrida.mostrarMenuCalc = (true);
                listTipoCorrida.Add(tipoCorrida);

                tipoCorrida = new TipoCorrida();
                tipoCorrida.clave = ("AGI");
                tipoCorrida.descripcion = ("Aguinaldo");
                tipoCorrida.sistema = (true);
                tipoCorrida.orden = ((short)3);
                tipoCorrida.tipoDeCalculo = ((short)3);
                tipoCorrida.usaCorrPeriodica = (false);
                tipoCorrida.mostrarMenuCalc = (true);
                listTipoCorrida.Add(tipoCorrida);

                tipoCorrida = new TipoCorrida();
                tipoCorrida.clave = ("FDA");
                tipoCorrida.descripcion = ("Fondo de Ahorro");
                tipoCorrida.sistema = (true);
                tipoCorrida.orden = ((short)4);
                tipoCorrida.tipoDeCalculo = ((short)7);
                tipoCorrida.usaCorrPeriodica = (false);
                tipoCorrida.mostrarMenuCalc = (true);
                listTipoCorrida.Add(tipoCorrida);

                tipoCorrida = new TipoCorrida();
                tipoCorrida.clave = ("FIN");
                tipoCorrida.descripcion = ("Finiquito");
                tipoCorrida.sistema = (true);
                tipoCorrida.orden = ((short)5);
                tipoCorrida.tipoDeCalculo = ((short)4);
                tipoCorrida.usaCorrPeriodica = (true);
                tipoCorrida.mostrarMenuCalc = (false);
                listTipoCorrida.Add(tipoCorrida);

                tipoCorrida = new TipoCorrida();
                tipoCorrida.clave = ("LIQ");
                tipoCorrida.descripcion = ("Liquidaciones");
                tipoCorrida.sistema = (true);
                tipoCorrida.orden = ((short)6);
                tipoCorrida.tipoDeCalculo = ((short)5);
                tipoCorrida.usaCorrPeriodica = (true);
                tipoCorrida.mostrarMenuCalc = (false);
                listTipoCorrida.Add(tipoCorrida);

                tipoCorrida = new TipoCorrida();
                tipoCorrida.clave = ("PTU");
                tipoCorrida.descripcion = ("PTU");
                tipoCorrida.sistema = (true);
                tipoCorrida.orden = ((short)7);
                tipoCorrida.tipoDeCalculo = ((short)2);
                tipoCorrida.usaCorrPeriodica = (false);
                tipoCorrida.mostrarMenuCalc = (false);
                listTipoCorrida.Add(tipoCorrida);

                tipoCorrida = new TipoCorrida();
                tipoCorrida.clave = ("ASI");
                tipoCorrida.descripcion = ("Asimilados");
                tipoCorrida.sistema = (true);
                tipoCorrida.orden = ((short)8);
                tipoCorrida.tipoDeCalculo = ((short)8);
                tipoCorrida.usaCorrPeriodica = (false);
                tipoCorrida.mostrarMenuCalc = (true);
                listTipoCorrida.Add(tipoCorrida);

                for (i = 0; i < listTipoCorrida.Count; i++)
                {
                    dbContext.Set<TipoCorrida>().Add(listTipoCorrida[i]);
                }

                /*contador = contador + listTipoCorrida.Count;
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region DatosDisponiblesCxnConta
                datosDisponiblesCxnConta = new DatosDisponiblesCxnConta();
                datosDisponiblesCxnConta.clave = ("001");
                datosDisponiblesCxnConta.descripcion = ("Neto a pagar");
                listDatosDisponibleCxnConta.Add(datosDisponiblesCxnConta);

                datosDisponiblesCxnConta = new DatosDisponiblesCxnConta();
                datosDisponiblesCxnConta.clave = ("002");
                datosDisponiblesCxnConta.descripcion = ("Despensa");
                listDatosDisponibleCxnConta.Add(datosDisponiblesCxnConta);

                datosDisponiblesCxnConta = new DatosDisponiblesCxnConta();
                datosDisponiblesCxnConta.clave = ("003");
                datosDisponiblesCxnConta.descripcion = ("Neto sin Despensa");
                listDatosDisponibleCxnConta.Add(datosDisponiblesCxnConta);

                datosDisponiblesCxnConta = new DatosDisponiblesCxnConta();
                datosDisponiblesCxnConta.clave = ("004");
                datosDisponiblesCxnConta.descripcion = ("Conceptos");
                listDatosDisponibleCxnConta.Add(datosDisponiblesCxnConta);

                datosDisponiblesCxnConta = new DatosDisponiblesCxnConta();
                datosDisponiblesCxnConta.clave = ("005");
                datosDisponiblesCxnConta.descripcion = ("Percepciones");
                listDatosDisponibleCxnConta.Add(datosDisponiblesCxnConta);

                datosDisponiblesCxnConta = new DatosDisponiblesCxnConta();
                datosDisponiblesCxnConta.clave = ("006");
                datosDisponiblesCxnConta.descripcion = ("Deducciones");
                listDatosDisponibleCxnConta.Add(datosDisponiblesCxnConta);

                datosDisponiblesCxnConta = new DatosDisponiblesCxnConta();
                datosDisponiblesCxnConta.clave = ("007");
                datosDisponiblesCxnConta.descripcion = ("Conceptos Nomina");
                listDatosDisponibleCxnConta.Add(datosDisponiblesCxnConta);

                for (int ddc = 0; ddc < listDatosDisponibleCxnConta.Count; ddc++)
                {
                    dbContext.Set<DatosDisponiblesCxnConta>().Add(listDatosDisponibleCxnConta[ddc]);
                }

                /* contador = contador + listDatosDisponibleCxnConta.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Campos DIM
                campoDIM = new CampoDIM();
                campoDIM.clave = ("033");
                campoDIM.descripcion = ("Ingresos totales por pago en parcialidades");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("034");
                campoDIM.descripcion = ("Monto diario percibido por jubilaciones, pensiones o haberes de retiro en parcialidades");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("035");
                campoDIM.descripcion = ("Cantidad que se hubiera percibido en el periodo de no haber pago único por jubilaciones, pensiones o haberes de retiro en una sola exhibición");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("036");
                campoDIM.descripcion = ("Monto total del pago en una sola exhibición");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("037");
                campoDIM.descripcion = ("Número de días");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("038");
                campoDIM.descripcion = ("Ingresos exentos");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("039");
                campoDIM.descripcion = ("Ingresos gravables");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("040");
                campoDIM.descripcion = ("Ingresos acumulables");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("041");
                campoDIM.descripcion = ("Ingresos no acumulables");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("042");
                campoDIM.descripcion = ("Impuesto retenido");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("043");
                campoDIM.descripcion = ("Monto total pagado de otros pagos por separación");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("044");
                campoDIM.descripcion = ("Número de años de servicio del trabajador");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("045");
                campoDIM.descripcion = ("Ingresos exentos");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("046");
                campoDIM.descripcion = ("Ingresos gravados");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("047");
                campoDIM.descripcion = ("Ingresos acumulables (último sueldo mensual ordinario)");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("048");
                campoDIM.descripcion = ("Impuesto correspondiente al último sueldo mensual ordinario");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("049");
                campoDIM.descripcion = ("Ingresos no acumulables");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("050");
                campoDIM.descripcion = ("Impuesto retenido");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("051");
                campoDIM.descripcion = ("Ingresos asimilados a salarios");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("052");
                campoDIM.descripcion = ("Impuesto retenido durante el ejercicio");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("053");
                campoDIM.descripcion = ("Indique si ejerció la opción otorgada por el empleador para adquirir acciones o títulos valor");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("054");
                campoDIM.descripcion = ("Valor de mercado de las acciones o títulos valor al ejercer la opción");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("055");
                campoDIM.descripcion = ("Precio establecido al otorgarse la opción de ingresos en acciones o títulos valor");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("056");
                campoDIM.descripcion = ("Ingreso Acumulable");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("057");
                campoDIM.descripcion = ("Impuesto retenido durante el ejercicio");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("058");
                campoDIM.descripcion = ("Sueldos, salarios, rayas y jornales gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("059");
                campoDIM.descripcion = ("Sueldos, salarios, rayas y jornales exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("060");
                campoDIM.descripcion = ("Gratificación anual gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("061");
                campoDIM.descripcion = ("Gratificación anual exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("062");
                campoDIM.descripcion = ("Viáticos y gastos de viaje gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("063");
                campoDIM.descripcion = ("Viáticos y gastos de viaje exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("064");
                campoDIM.descripcion = ("Tiempo extraordinario gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("065");
                campoDIM.descripcion = ("Tiempo extraordinario exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("066");
                campoDIM.descripcion = ("Prima vacacional gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("067");
                campoDIM.descripcion = ("Prima vacacional exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("068");
                campoDIM.descripcion = ("Prima dominical gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("069");
                campoDIM.descripcion = ("Prima dominical exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("070");
                campoDIM.descripcion = ("Participación de los trabajadores en las utilidades (PTU) gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("071");
                campoDIM.descripcion = ("Participación de los trabajadores en las utilidades (PTU) exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("072");
                campoDIM.descripcion = ("Reembolso de gastos médicos, dentales y hospitalarios gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("073");
                campoDIM.descripcion = ("Reembolso de gastos médicos, dentales y hospitalarios exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("074");
                campoDIM.descripcion = ("Fondo de ahorro gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("075");
                campoDIM.descripcion = ("Fondo de ahorro exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("076");
                campoDIM.descripcion = ("Caja de ahorro gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("077");
                campoDIM.descripcion = ("Caja de ahorro exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("078");
                campoDIM.descripcion = ("Vales para despensa gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("079");
                campoDIM.descripcion = ("Vales para despensa exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("080");
                campoDIM.descripcion = ("Ayuda para gastos de funeral gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("081");
                campoDIM.descripcion = ("Ayuda para gastos de funeral exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("082");
                campoDIM.descripcion = ("Contribuciones a cargo del trabajador pagadas por el patrón gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("083");
                campoDIM.descripcion = ("Contribuciones a cargo del trabajador pagadas por el patrón exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("084");
                campoDIM.descripcion = ("Premios por puntualidad gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("085");
                campoDIM.descripcion = ("Premios por puntualidad exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("086");
                campoDIM.descripcion = ("Prima de seguro de vida gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("087");
                campoDIM.descripcion = ("Prima de seguro de vida exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("088");
                campoDIM.descripcion = ("Seguro de gastos médicos mayores gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("089");
                campoDIM.descripcion = ("Seguro de gastos médicos mayores exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("090");
                campoDIM.descripcion = ("Vales para restaurante gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("091");
                campoDIM.descripcion = ("Vales para restaurante exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("092");
                campoDIM.descripcion = ("Vales para gasolina gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("093");
                campoDIM.descripcion = ("Vales para gasolina exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("094");
                campoDIM.descripcion = ("Vales para ropa gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("095");
                campoDIM.descripcion = ("Vales para ropa exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("096");
                campoDIM.descripcion = ("Ayuda para renta gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("097");
                campoDIM.descripcion = ("Ayuda para renta exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("098");
                campoDIM.descripcion = ("Ayuda para artículos escolares gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("099");
                campoDIM.descripcion = ("Ayuda para artículos escolares exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("100");
                campoDIM.descripcion = ("Dotación o ayuda para anteojos gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("101");
                campoDIM.descripcion = ("Dotación o ayuda para anteojos exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("102");
                campoDIM.descripcion = ("Ayuda para transporte gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("103");
                campoDIM.descripcion = ("Ayuda para transporte exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("104");
                campoDIM.descripcion = ("Cuotas sindicales pagadas por el patrón gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("105");
                campoDIM.descripcion = ("Cuotas sindicales pagadas por el patrón exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("106");
                campoDIM.descripcion = ("Subsidios por incapacidad gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("107");
                campoDIM.descripcion = ("Subsidios por incapacidad exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("108");
                campoDIM.descripcion = ("Becas para trabajadores y/o sus hijos gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("109");
                campoDIM.descripcion = ("Becas para trabajadores y/o sus hijos exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("110");
                campoDIM.descripcion = ("Pagos efectuados por otros empleadores (sólo si el patrón que declara realizó cálculo anual) gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("111");
                campoDIM.descripcion = ("Pagos efectuados por otros empleadores (sólo si el patrón que declara realizó cálculo anual) exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("112");
                campoDIM.descripcion = ("Otros ingresos por salarios gravado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("113");
                campoDIM.descripcion = ("Otros ingresos por salarios exento");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("114");
                campoDIM.descripcion = ("Suma del ingreso GRAVADO por sueldos y salarios");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("115");
                campoDIM.descripcion = ("Suma del ingreso EXENTO por sueldos y salarios");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("116");
                campoDIM.descripcion = ("Impuesto retenido durante el ejercicio que declara");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("117");
                campoDIM.descripcion = ("Impuesto retenido por otro(s) patrón(es) durante el ejercicio que declara");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("118");
                campoDIM.descripcion = ("Saldo a favor determinado en el ejercicio que declara, que el patrón compensará durante el siguiente ejercicio o solicitará su devolución");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("119");
                campoDIM.descripcion = ("Saldo a favor del ejercicio anterior no compensado durante el ejercicio que declara");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("120");
                campoDIM.descripcion = ("Suma de las cantidades que por concepto de crédito al salario le correspondió al trabajador");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("121");
                campoDIM.descripcion = ("Crédito al salario entregado en efectivo al trabajador durante el ejercicio que declara");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("122");
                campoDIM.descripcion = ("Monto total de ingresos obtenidos por concepto de prestaciones de previsión social");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("123");
                campoDIM.descripcion = ("Suma de ingresos exentos por concepto de prestaciones de previsión social");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("124");
                campoDIM.descripcion = ("Suma de ingresos por sueldos y salarios");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("125");
                campoDIM.descripcion = ("Monto del impuesto local a los ingresos por sueldos, salarios y en general por la prestación de un servicio personal subordinado retenido");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("126");
                campoDIM.descripcion = ("Monto del subsidio para el empleo entregado en efectivo al trabajador durante el ejercicio que declara");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("127");
                campoDIM.descripcion = ("Total de las aportaciones voluntarias deducibles");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("128");
                campoDIM.descripcion = ("ISR conforme a la tarifa anual");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("129");
                campoDIM.descripcion = ("Subsidio acreditable");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("130");
                campoDIM.descripcion = ("Subsidio no acreditable");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("131");
                campoDIM.descripcion = ("Impuesto sobre ingresos acumulables");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("132");
                campoDIM.descripcion = ("Impuesto sobre ingresos no acumulables");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("133");
                campoDIM.descripcion = ("Impuesto local a los ingresos por sueldos, salarios y en general por la prestación de un servicio personal subordinado");
                listCampoDIM.Add(campoDIM);

                campoDIM = new CampoDIM();
                campoDIM.clave = ("134");
                campoDIM.descripcion = ("Monto del subsidio para el empleo que le correspondió al trabajador durante el ejercicio");
                listCampoDIM.Add(campoDIM);

                for (i = 0; i < listCampoDIM.Count; i++)
                {
                    dbContext.Set<CampoDIM>().Add(listCampoDIM[i]);
                }

                contador = contador + listCampoDIM.Count();
                /* if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Parametros de los conceptos
                paraConcepDeNom = new ParaConcepDeNom();
                paraConcepDeNom.descripcion = ("HORASEXTRASDOBLES");
                paraConcepDeNom.clasificadorParametro = (ClasificadorParametro.ENTRADA);
                paraConcepDeNom.mascara = ("##.##");
                paraConcepDeNom.numero = (1);
                paraConcepDeNom.tipo = ("INTEGER");
                paraConcepDeNom.unidad = ("HORAS");
                listParaConcepDeNom.Add(paraConcepDeNom);

                paraConcepDeNom = new ParaConcepDeNom();
                paraConcepDeNom.descripcion = ("HORASTIEMPOEXTRAS");
                paraConcepDeNom.clasificadorParametro = (ClasificadorParametro.ENTRADA);
                paraConcepDeNom.mascara = ("##.##");
                paraConcepDeNom.numero = (1);
                paraConcepDeNom.tipo = ("INTEGER");
                paraConcepDeNom.unidad = ("HORAS");
                listParaConcepDeNom.Add(paraConcepDeNom);

                paraConcepDeNom = new ParaConcepDeNom();
                paraConcepDeNom.descripcion = ("TIEMPOEXTRADIADESCANSO");
                paraConcepDeNom.clasificadorParametro = (ClasificadorParametro.ENTRADA);
                paraConcepDeNom.mascara = ("##.##");
                paraConcepDeNom.numero = (1);
                paraConcepDeNom.tipo = ("INTEGER");
                paraConcepDeNom.unidad = ("HORAS");
                listParaConcepDeNom.Add(paraConcepDeNom);

                paraConcepDeNom = new ParaConcepDeNom();
                paraConcepDeNom.descripcion = ("TIEMPOEXTRAFESTIVO");
                paraConcepDeNom.clasificadorParametro = (ClasificadorParametro.ENTRADA);
                paraConcepDeNom.mascara = ("##.##");
                paraConcepDeNom.numero = (1);
                paraConcepDeNom.tipo = ("INTEGER");
                paraConcepDeNom.unidad = ("HORAS");
                listParaConcepDeNom.Add(paraConcepDeNom);

                paraConcepDeNom = new ParaConcepDeNom();
                paraConcepDeNom.descripcion = ("TIEMPOEXTRADOMINGO");
                paraConcepDeNom.clasificadorParametro = (ClasificadorParametro.ENTRADA);
                paraConcepDeNom.mascara = ("##.##");
                paraConcepDeNom.numero = (1);
                paraConcepDeNom.tipo = ("INTEGER");
                paraConcepDeNom.unidad = ("HORAS");
                listParaConcepDeNom.Add(paraConcepDeNom);

                paraConcepDeNom = new ParaConcepDeNom();
                paraConcepDeNom.descripcion = ("INCENTIVO");
                paraConcepDeNom.clasificadorParametro = (ClasificadorParametro.ENTRADA);
                paraConcepDeNom.mascara = ("####.##");
                paraConcepDeNom.numero = (1);
                paraConcepDeNom.tipo = ("INTEGER");
                paraConcepDeNom.unidad = ("OTROS");
                listParaConcepDeNom.Add(paraConcepDeNom);

                paraConcepDeNom = new ParaConcepDeNom();
                paraConcepDeNom.descripcion = ("PREVISIONSOCIAL");
                paraConcepDeNom.clasificadorParametro = (ClasificadorParametro.ENTRADA);
                paraConcepDeNom.mascara = ("####.##");
                paraConcepDeNom.numero = (1);
                paraConcepDeNom.tipo = ("INTEGER");
                paraConcepDeNom.unidad = ("OTROS");
                listParaConcepDeNom.Add(paraConcepDeNom);

                paraConcepDeNom = new ParaConcepDeNom();
                paraConcepDeNom.descripcion = ("BECAS");
                paraConcepDeNom.clasificadorParametro = (ClasificadorParametro.ENTRADA);
                paraConcepDeNom.mascara = ("####.##");
                paraConcepDeNom.numero = (1);
                paraConcepDeNom.tipo = ("INTEGER");
                paraConcepDeNom.unidad = ("OTROS");
                listParaConcepDeNom.Add(paraConcepDeNom);

                paraConcepDeNom = new ParaConcepDeNom();
                paraConcepDeNom.descripcion = ("DEVOLUCION");
                paraConcepDeNom.clasificadorParametro = (ClasificadorParametro.ENTRADA);
                paraConcepDeNom.mascara = ("####.##");
                paraConcepDeNom.numero = (1);
                paraConcepDeNom.tipo = ("INTEGER");
                paraConcepDeNom.unidad = ("OTROS");
                listParaConcepDeNom.Add(paraConcepDeNom);

                paraConcepDeNom = new ParaConcepDeNom();
                paraConcepDeNom.descripcion = ("OTRASPERCEPCIONES");
                paraConcepDeNom.clasificadorParametro = ClasificadorParametro.ENTRADA;
                paraConcepDeNom.mascara = ("####.##");
                paraConcepDeNom.numero = (1);
                paraConcepDeNom.tipo = ("INTEGER");
                paraConcepDeNom.unidad = ("OTROS");
                listParaConcepDeNom.Add(paraConcepDeNom);

                for (i = 0; i < listParaConcepDeNom.Count; i++)
                {
                    dbContext.Set<ParaConcepDeNom>().Add(listParaConcepDeNom[i]);
                }

                contador = contador + listParaConcepDeNom.Count();
                /*if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/

                dbContext.SaveChanges();
                #endregion

                #region ConceptoDeNominaDefinicion
                //año,mes,dia
                DateTime dateAlta = new DateTime(2018, 1, 1, 12, 0, 0);

                List<TipoCorrida> listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[0]);
                List<ParaConcepDeNom> listParaConcepNomina = new List<ParaConcepDeNom>();

                ConceptoPorTipoCorrida ctc = new ConceptoPorTipoCorrida();
                ctc.tipoCorrida_ID = (listTipoCorrida[0].id);

                //PERCEPCIONES 001-599
                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("100");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("SUELDO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("SUELDO");
                conceptoDeNominaDefinicion.formulaConcepto = ("SUELDODIARIOFINAL*CONCEP_901");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 001");
                conceptoDeNominaDefinicion.tipo = (Tipo.AUTOMATICO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (100);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("160");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("COMISIONES");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("COMISIONES");
                conceptoDeNominaDefinicion.formulaConcepto = ("SALARIOMIN*DIASPAGO*10.0");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 008");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (160);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("120");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("TIEMPO EXTRA DOBLE");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("TIEMPOEXTDBLE");
                conceptoDeNominaDefinicion.formulaConcepto = ("(SUELDODIARIOFINAL/HRSTURNO)*HorasExtrasDobles*2");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 002");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (120);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                for (int bcn = 0; bcn < conceptoDeNominaDefinicion.baseAfecConcepNom.Count; bcn++)
                {
                    conceptoDeNominaDefinicion.baseAfecConcepNom[bcn].formulaExenta = ("IF (((SUELDODIARIOFINAL/HRSTURNO)*HorasExtrasDobles)/2)<=(SalarioMin*10.7) THEN (((SUELDODIARIOFINAL/HRSTURNO)*HorasExtrasDobles)/2) ELSE (SalarioMin*10.7)");
                }
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("122");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("TIEMPO EXTRA TRIPLE");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("TIEMPOEXTTPLE");
                conceptoDeNominaDefinicion.formulaConcepto = ("(SUELDODIARIOFINAL/HRSTURNO)*HorasExtrasTriples*3");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 002");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (122);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("124");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("TIEMPO EXTRA DIA DE DESCANSO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("TIEMPOEXTDESCAN");
                conceptoDeNominaDefinicion.formulaConcepto = ("(SUELDODIARIOFINAL/HRSTURNO)*TExtrasDiaDescanso*2");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 002");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (124);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("126");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("TIEMPO EXTRA DIA FESTIVO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("TIEMPOEXTFESTI");
                conceptoDeNominaDefinicion.formulaConcepto = ("(SUELDODIARIOFINAL/HRSTURNO)*TExtrasDiaFestivo*2");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 002");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (126);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("132");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("PRIMA DOMINICAL");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("PRIMADOMINI");
                conceptoDeNominaDefinicion.formulaConcepto = ("(SUELDODIARIOFINAL/HRSTURNO)*TExtrasDiaDomingo*0.25");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 003");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (132);

                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                for (int bcn = 0; bcn < conceptoDeNominaDefinicion.baseAfecConcepNom.Count; bcn++)
                {
                    conceptoDeNominaDefinicion.baseAfecConcepNom[bcn].formulaExenta = ("SalarioMin*(TExtrasDiaDomingo/HrsTurno)");
                }
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("200");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("VACACIONES");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("VACACIONES");
                conceptoDeNominaDefinicion.formulaConcepto = ("IF CONCEP_902>0 THEN (CONCEP_902)*SUELDODIARIOFINAL ELSE 0");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 005");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (200);
                listTipoCorrida_Concept_Nomi = null;
                listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[0]);
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[1]);

                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("202");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("PRIMA VACACIONAL");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("PRIMA");
                conceptoDeNominaDefinicion.formulaConcepto = ("(SUELDODIARIOFINAL*CONCEP_902)*CONCEP_903");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 005");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (202);

                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                for (int bcn = 0; bcn < conceptoDeNominaDefinicion.baseAfecConcepNom.Count; bcn++)
                {
                    conceptoDeNominaDefinicion.baseAfecConcepNom[bcn].formulaExenta = ("SalarioMin*15");
                    conceptoDeNominaDefinicion.baseAfecConcepNom[bcn].tipoAfecta = 2;

                }
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("210");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("AGUINALDO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("AGUINALDO");
                conceptoDeNominaDefinicion.formulaConcepto = ("SUELDODIARIOFINAL*FACTORDIASAGUINALDO");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 006");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (210);
                listTipoCorrida_Concept_Nomi = null;
                listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[0]);
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[2]);

                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                for (int bcn = 0; bcn < conceptoDeNominaDefinicion.baseAfecConcepNom.Count; bcn++)
                {
                    conceptoDeNominaDefinicion.baseAfecConcepNom[bcn].formulaExenta = ("SalarioMin*30");
                    conceptoDeNominaDefinicion.baseAfecConcepNom[bcn].tipoAfecta = 2;
                }

                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("220");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("REPARTO DE UTILIDADES");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("REPARTOUTILIDADES");
                conceptoDeNominaDefinicion.formulaConcepto = ("");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 003");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (220);
                listTipoCorrida_Concept_Nomi = null;
                listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[6]);

                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                for (int bcn = 0; bcn < conceptoDeNominaDefinicion.baseAfecConcepNom.Count; bcn++)
                {
                    conceptoDeNominaDefinicion.baseAfecConcepNom[bcn].formulaExenta = ("SalarioMin*30");
                    conceptoDeNominaDefinicion.baseAfecConcepNom[bcn].tipoAfecta = 2;
                }
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("150");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("PREMIO POR PUNTUALIDAD");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("PREMIOPUNTUA");
                conceptoDeNominaDefinicion.formulaConcepto = ("IF (RETARDOS+FALTAS+INCAPACIDADENFERMEDAD+INCAPACIDADACCIDENTE+PERMISOSINSUELDO+PERMISOCONSUELDO)=0.0 THEN CONCEP_001*0.03");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 003");
                conceptoDeNominaDefinicion.tipo = (Tipo.REPETITIVO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (150);
                listTipoCorrida_Concept_Nomi = null;
                listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[0]);

                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("165");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("INCENTIVOS");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("INCENTIVOS");
                conceptoDeNominaDefinicion.formulaConcepto = ("Param1");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 003");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (165);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listParaConcepNomina = null;
                listParaConcepNomina = new List<ParaConcepDeNom>();
                listParaConcepNomina.Add(listParaConcepDeNom[5]);
                conceptoDeNominaDefinicion.paraConcepDeNom = (listParaConcepNomina);
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("230");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("PREVISION SOCIAL");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("PREVISION");
                conceptoDeNominaDefinicion.formulaConcepto = ("Param1");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 010");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (230);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listParaConcepNomina = null;
                listParaConcepNomina = new List<ParaConcepDeNom>();
                listParaConcepNomina.Add(listParaConcepDeNom[6]);
                conceptoDeNominaDefinicion.paraConcepDeNom = (listParaConcepNomina);
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("145");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("BECAS");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("BECAS");
                conceptoDeNominaDefinicion.formulaConcepto = ("Param1");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 010");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (145);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listParaConcepNomina = null;
                listParaConcepNomina = new List<ParaConcepDeNom>();
                listParaConcepNomina.Add(listParaConcepDeNom[7]);
                conceptoDeNominaDefinicion.paraConcepDeNom = (listParaConcepNomina);
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("235");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("APORTACION PATRONAL F. AHORRO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("APORTACION");
                conceptoDeNominaDefinicion.formulaConcepto = ("CONCEP_001*0.01");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 010");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (235);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("170");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("DEVOLUCION DEL F. DE AHORRO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("DEVOLUCION");
                conceptoDeNominaDefinicion.formulaConcepto = ("Param1");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = (null);
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (170);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listParaConcepNomina = null;
                listParaConcepNomina = new List<ParaConcepDeNom>();
                listParaConcepNomina.Add(listParaConcepDeNom[8]);
                conceptoDeNominaDefinicion.paraConcepDeNom = (listParaConcepNomina);
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("180");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("OTRAS PERCEPCIONES");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("OTRASPERCEP");
                conceptoDeNominaDefinicion.formulaConcepto = ("Param1");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 009");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (180);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listParaConcepNomina = null;
                listParaConcepNomina = new List<ParaConcepDeNom>();
                listParaConcepNomina.Add(listParaConcepDeNom[9]);
                conceptoDeNominaDefinicion.paraConcepDeNom = (listParaConcepNomina);
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1031");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("SUBSIDIO AL EMPLEO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("SUBSIDIO");
                conceptoDeNominaDefinicion.formulaConcepto = ("ISRSubsidio");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("1125 999");
                conceptoDeNominaDefinicion.tipo = (Tipo.AUTOMATICO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1031);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("790");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("PRIMA DE AJUSTE POR REDONDE");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("PRIMAAJUSTEREDONDEO");
                conceptoDeNominaDefinicion.formulaConcepto = ("");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.PERCEPCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 008");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (790);

                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                //DEDUCCINES 600-900
                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("900");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("RETARDO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("RETARDO");
                conceptoDeNominaDefinicion.formulaConcepto = ("IF DiasJornada=6 THEN (SueldoDiarioFinal/HrsTurno)*Retardos*1.1666 ELSE (SueldoDiarioFinal/HrsTurno)*Retardos*1.4");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 001");
                conceptoDeNominaDefinicion.tipo = (Tipo.AUTOMATICO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (900);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                conceptoDeNominaDefinicion.baseAfecConcepNom = (buscarBaseAfecta(listBaseNomina, conceptoDeNominaDefinicion));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("905");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("FALTAS INJUSTIFICADAS");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("FALTASINJUSTI");
                conceptoDeNominaDefinicion.formulaConcepto = ("");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 001");
                conceptoDeNominaDefinicion.tipo = (Tipo.AUTOMATICO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (905);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("920");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("INCAPACIDADES POR ENFERMEDAD");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("INCAP X ENFERM");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 001");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (902);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("925");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("INCAPACIDADES POR ACCIDENTE");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("INCAP X ACCIDEN");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 001");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (925);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("930");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("INCAPACIDAD POR MATERNIDAD");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("INCAP X MATER");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (930);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("910");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("PERMISOS SIN SUELDO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("PERMISOS");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.subCuenta = ("511D 001");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (910);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1150");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("CUOTA SINDICAL");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("CUOTASINDICAL");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1150);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1130");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("CREDITO INFONAVIT");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("CRED. INFO.");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.tipo = (Tipo.AUTOMATICO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1130);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1131");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("TASA CREDITO INFONAVIT");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("TASA CRED. INFO.");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1131);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1135");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("PRESTAMOS PERSONALES");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("PREST. PERS.");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.tipo = (Tipo.REPETITIVO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1135);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("11501111");// Repetido con 1150 Linea 6405
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("FONDO DE AHORRO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("FONDOAHORRO");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.formulaConcepto = ("(TotalPercepcionesPeriodo-Concep_598)*0.14");
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1150);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1155");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("OTRAS DEDUCCIONES");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("OTRASDEDUCCIONES");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1155);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1102");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("IMSS");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("IMSS");
                conceptoDeNominaDefinicion.formulaConcepto = ("CalculoIMSS");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.subCuenta = ("2130 002");
                conceptoDeNominaDefinicion.tipo = (Tipo.AUTOMATICO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1102);
                listTipoCorrida_Concept_Nomi = null;
                listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[0]);

                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1030");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("ISR");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("ISR");
                conceptoDeNominaDefinicion.formulaConcepto = ("CalculoISR");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.subCuenta = ("2130 001");
                conceptoDeNominaDefinicion.tipo = (Tipo.AUTOMATICO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1030);
                listTipoCorrida_Concept_Nomi = null;
                listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[0]);
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[1]);
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[2]);
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[4]);
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[5]);
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[6]);

                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1200");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("AJUSTE POR REDONDEO");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("AJUSTEREDONDEO");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DEDUCCION);
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1200);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                //901-?
                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("010");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("DIAS TRABAJADOS");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("DIAS TRAB.");
                conceptoDeNominaDefinicion.formulaConcepto = ("DiasPago");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DATO);
                conceptoDeNominaDefinicion.subCuenta = ("511D 102");
                conceptoDeNominaDefinicion.tipo = (Tipo.AUTOMATICO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (10);
                listTipoCorrida_Concept_Nomi = null;
                listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[0]);
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[1]);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("016");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("DIAS VACACIONES");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("DIAS VAC.");
                conceptoDeNominaDefinicion.formulaConcepto = ("DiasVacaciones");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DATO);
                conceptoDeNominaDefinicion.subCuenta = ("511D 102");
                conceptoDeNominaDefinicion.tipo = (Tipo.AUTOMATICO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (016);
                listTipoCorrida_Concept_Nomi = null;
                listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[0]);
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[1]);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("017");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("DIAS PRIMA VACACIONAL");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("DIAS PRIMA VAC.");
                conceptoDeNominaDefinicion.formulaConcepto = ("diasPrima");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.DATO);
                conceptoDeNominaDefinicion.subCuenta = ("511D 102");
                conceptoDeNominaDefinicion.tipo = (Tipo.AUTOMATICO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (17);
                listTipoCorrida_Concept_Nomi = null;
                listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[0]);
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[1]);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                //CONCEPTOS ESPECIALES DEDUCCION 
                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1300");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("IMSS PATRONAL");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("IMSSPATRONAL");
                conceptoDeNominaDefinicion.formulaConcepto = ("CalculoIMSSPatronal");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.INFORMATIVO);
                conceptoDeNominaDefinicion.subCuenta = ("511D 101");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1300);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1302");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("5% INFONAVIT");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("5%INFONAVIT");
                conceptoDeNominaDefinicion.formulaConcepto = ("BASEINFONAVIT*0.05");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.INFORMATIVO);
                conceptoDeNominaDefinicion.subCuenta = ("511D 102");
                conceptoDeNominaDefinicion.tipo = (Tipo.REPETITIVO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1302);
                listTipoCorrida_Concept_Nomi = null;
                listTipoCorrida_Concept_Nomi = new List<TipoCorrida>();
                listTipoCorrida_Concept_Nomi.Add(listTipoCorrida[0]);

                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1304");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("2% SAR");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("2%SAR");
                conceptoDeNominaDefinicion.formulaConcepto = ("BASESAR*0.02");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.INFORMATIVO);
                conceptoDeNominaDefinicion.subCuenta = ("511D 103");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1304);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);

                conceptoDeNominaDefinicion = new ConcepNomDefi();
                conceptoDeNominaDefinicion.clave = ("1306");
                conceptoDeNominaDefinicion.fecha = (dateAlta);
                conceptoDeNominaDefinicion.comportamiento = (null);
                conceptoDeNominaDefinicion.descripcion = ("2% ESTATAL");
                conceptoDeNominaDefinicion.descripcionAbreviada = ("2%ESTATAL");
                conceptoDeNominaDefinicion.formulaConcepto = ("TOTALPERCEPCIONES*0.02");
                conceptoDeNominaDefinicion.imprimirEnListadoNomina = (false);
                conceptoDeNominaDefinicion.imprimirEnReciboNomina = (false);
                conceptoDeNominaDefinicion.naturaleza = (Naturaleza.INFORMATIVO);
                conceptoDeNominaDefinicion.subCuenta = ("511D 104");
                conceptoDeNominaDefinicion.tipo = (Tipo.PERIODO);
                conceptoDeNominaDefinicion.activado = (true);
                conceptoDeNominaDefinicion.prioridadDeCalculo = (1306);
                conceptoDeNominaDefinicion.conceptoPorTipoCorrida = (listConcep_tipCorr(conceptoDeNominaDefinicion, listTipoCorrida_Concept_Nomi));
                listConceptoDeNominaDefinicion.Add(conceptoDeNominaDefinicion);


                int j = 0;

                #region ConceptoDeNomina
                for (j = 0; j < listConceptoDeNominaDefinicion.Count; j++)
                {
                    listConceptoDeNominaDefinicion[j].condicionConcepto = ("");
                    conceptoDeNomina = new ConceptoDeNomina();
                    conceptoDeNomina.clave = (listConceptoDeNominaDefinicion[j].clave);
                    listConceptoDeNominas.Add(conceptoDeNomina);
                }

                for (i = 0; i < listConceptoDeNominas.Count; i++)
                {
                    dbContext.Set<ConceptoDeNomina>().Add(listConceptoDeNominas[i]);
                }

                /*contador = contador + listConceptoDeNominas.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/

                for (j = 0; j < listConceptoDeNominas.Count; j++)
                {
                    listConceptoDeNominaDefinicion[j].conceptoDeNomina = (listConceptoDeNominas[j]);
                }

                for (i = 0; i < listConceptoDeNominaDefinicion.Count; i++)
                {
                    dbContext.Set<ConcepNomDefi>().Add(listConceptoDeNominaDefinicion[i]);
                }

                /* contador = contador + listConceptoDeNominaDefinicion.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/

                dbContext.SaveChanges();
                #endregion

                #endregion

                #region Incidencias
                incidencias = new Incidencias();
                incidencias.descripcion = ("Faltas injustificadas");
                incidencias.descontar = (true);
                listIncidencias.Add(incidencias);

                incidencias = new Incidencias();
                incidencias.descripcion = ("Permisos sin goce de sueldo");
                incidencias.descontar = (true);
                listIncidencias.Add(incidencias);

                incidencias = new Incidencias();
                incidencias.descripcion = ("Suspensión");
                incidencias.descontar = (true);
                listIncidencias.Add(incidencias);

                incidencias = new Incidencias();
                incidencias.descripcion = ("Incapacidades por Enfermedad General");
                incidencias.descontar = (true);
                listIncidencias.Add(incidencias);

                incidencias = new Incidencias();
                incidencias.descripcion = ("Incapacidades por Maternidad");
                incidencias.descontar = (true);
                listIncidencias.Add(incidencias);

                incidencias = new Incidencias();
                incidencias.descripcion = ("Incapacidades por Riesgo de Trabajo");
                incidencias.descontar = (true);
                listIncidencias.Add(incidencias);

                for (i = 0; i < listIncidencias.Count; i++)
                {
                    dbContext.Set<Incidencias>().Add(listIncidencias[i]);
                }

                /* contador = contador + listIncidencias.Count;
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region ConfiguraMovimientos
                /*CONFIGURAMOVIMIENTO SIN RAZON SOCIAL GLOBALES*/
                configuraMovimiento = new ConfiguraMovimiento();
                configuraMovimiento.nombre = ("Movtos. masivos por concepto");
                configuraMovimiento.filtro = ("Concepto");
                configuraMovimiento.movimiento = ("Empleado");
                configuraMovimiento.movimientoExistente = (true);
                //Empleado,Concepto,Centro de costos
                configuraMovimiento.activadosFiltro = ("0,1,0"); //valores en true 1, 0 false
                                                                 //Empleado,Concepto,Centro de costos,Plazas,FechaInicio,FechaCierre
                configuraMovimiento.activadosMovimientos = ("1,0,0,0,0,0"); //valores en true 1, 0 false
                configuraMovimiento.ordenId = (1);
                configuraMovimiento.sistema = (true);
                configuraMovimiento.razonesSociales = (null);
                listConfiguraMovimiento.Add(configuraMovimiento);

                configuraMovimiento = new ConfiguraMovimiento();
                configuraMovimiento.nombre = ("Movtos. masivos por empleado");
                configuraMovimiento.filtro = ("Empleado");
                configuraMovimiento.movimiento = ("Concepto");
                configuraMovimiento.movimientoExistente = (true);
                configuraMovimiento.activadosFiltro = ("1,0,0");
                configuraMovimiento.activadosMovimientos = ("0,1,0,0,0,0");
                configuraMovimiento.ordenId = (2);
                configuraMovimiento.sistema = (true);
                configuraMovimiento.razonesSociales = (null);
                listConfiguraMovimiento.Add(configuraMovimiento);

                for (i = 0; i < listConfiguraMovimiento.Count(); i++)
                {
                    dbContext.Set<ConfiguraMovimiento>().Add(listConfiguraMovimiento[i]);
                }
                /*  contador = contador + listConfiguraMovimiento.Count();
                  if (contador % rango == 0 & contador > 0)
                  {
                      session.flush();
                      session.clear();
                      contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region Excepciones
                excepciones = new Excepciones();
                excepciones.clave = ("0");
                excepciones.orden = (0);
                excepciones.excepcion = ("Laborado");
                excepciones.naturaleza = (Naturaleza.PERCEPCION);
                excepciones.unico = (false);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("1");
                excepciones.orden = (1);
                excepciones.excepcion = ("Retardo");
                excepciones.naturaleza = (Naturaleza.DEDUCCION);
                excepciones.unico = (false);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.HORASMINUTOS);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("2");
                excepciones.orden = (2);
                excepciones.excepcion = ("Falta");
                excepciones.naturaleza = (Naturaleza.DEDUCCION);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                excepciones.unico = (true);
                excepciones.concepNomDefi_ID = (listConceptoDeNominaDefinicion[21].id);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("3");
                excepciones.orden = (3);
                excepciones.excepcion = ("Ausentismo");
                excepciones.naturaleza = (Naturaleza.DEDUCCION);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                excepciones.unico = (true);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("4");
                excepciones.orden = (4);
                excepciones.excepcion = ("Permiso con sueldo");
                excepciones.naturaleza = (Naturaleza.DEDUCCION);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                excepciones.unico = (false);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("5");
                excepciones.orden = (5);
                excepciones.excepcion = ("Permiso sin sueldo ");
                excepciones.naturaleza = (Naturaleza.DEDUCCION);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                excepciones.unico = (false);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("6");
                excepciones.orden = (6);
                excepciones.excepcion = ("Incapacidad por enfermedad");
                excepciones.naturaleza = (Naturaleza.DEDUCCION);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                excepciones.unico = (true);
                excepciones.concepNomDefi_ID = (listConceptoDeNominaDefinicion[22].id);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("7");
                excepciones.orden = (7);
                excepciones.excepcion = ("Incapacidad por accidente");
                excepciones.naturaleza = (Naturaleza.DEDUCCION);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                excepciones.unico = (true);
                excepciones.concepNomDefi_ID = (listConceptoDeNominaDefinicion[23].id);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("8");
                excepciones.orden = (8);
                excepciones.excepcion = ("Incapacidad por maternidad");
                excepciones.naturaleza = (Naturaleza.DEDUCCION);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                excepciones.unico = (true);
                excepciones.concepNomDefi_ID = (listConceptoDeNominaDefinicion[24].id);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("10");
                excepciones.orden = (10);
                excepciones.excepcion = ("Descanso laborado");
                excepciones.naturaleza = (Naturaleza.PERCEPCION);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.HORASMINUTOS);
                excepciones.unico = (false);
                excepciones.concepNomDefi_ID = (listConceptoDeNominaDefinicion[4].id);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("11");
                excepciones.orden = (11);
                excepciones.excepcion = ("Festivo laborado");
                excepciones.naturaleza = (Naturaleza.PERCEPCION);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                excepciones.unico = (false);
                excepciones.concepNomDefi_ID = (listConceptoDeNominaDefinicion[5].id);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("12");
                excepciones.orden = (12);
                excepciones.excepcion = ("Domingo laborado");
                excepciones.naturaleza = (Naturaleza.PERCEPCION);
                excepciones.unico = (false);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.HORASMINUTOS);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("13");
                excepciones.orden = (13);
                excepciones.excepcion = ("Tiempo extra");
                excepciones.naturaleza = (Naturaleza.PERCEPCION);
                excepciones.unico = (false);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.HORASMINUTOS);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("14");
                excepciones.orden = (14);
                excepciones.excepcion = ("Extra doble");
                excepciones.naturaleza = (Naturaleza.PERCEPCION);
                excepciones.unico = (false);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.HORASMINUTOS);
                excepciones.concepNomDefi_ID = (listConceptoDeNominaDefinicion[2].id);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("15");
                excepciones.orden = (15);
                excepciones.excepcion = ("Extra triple");
                excepciones.naturaleza = (Naturaleza.PERCEPCION);
                excepciones.unico = (false);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.HORASMINUTOS);
                excepciones.concepNomDefi_ID = (listConceptoDeNominaDefinicion[3].id);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("16");
                excepciones.orden = (16);
                excepciones.excepcion = ("Descanso");
                excepciones.naturaleza = (Naturaleza.PERCEPCION);
                excepciones.unico = (false);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                listExcepciones.Add(excepciones);

                excepciones = new Excepciones();
                excepciones.clave = ("17");
                excepciones.orden = (17);
                excepciones.excepcion = ("Festivo");
                excepciones.naturaleza = (Naturaleza.PERCEPCION);
                excepciones.unico = (false);
                excepciones.tipoDatoExcepcion = (TipoDatoExcepcion.SINDATO);
                listExcepciones.Add(excepciones);

                for (i = 0; i < listExcepciones.Count; i++)
                {
                    dbContext.Set<Excepciones>().Add(listExcepciones[i]);
                }

                /* contador = contador + listExcepciones.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region ConfigAsistencias
                listExcepciones.RemoveAt(13);//se remueve las excepciones de tiempo extra doble y triples ya que existe la excepcion de tiempo extra.

                configuraAsistencias = new ConfigAsistencias();
                configuraAsistencias.activadosFiltro = ("1,0,0");
                configuraAsistencias.filtro = ("Empleado");
                configuraAsistencias.activadosMovimientos = ("0,0,1");
                configuraAsistencias.movimiento = ("Excepcion");
                configuraAsistencias.nombre = ("Asistencias por empleado");
                configuraAsistencias.ordenId = (1);
                configuraAsistencias.excepciones = (listExcepciones);
                configuraAsistencias.sistema = (true);
                listConfiguraAsistencias.Add(configuraAsistencias);

                configuraAsistencias = new ConfigAsistencias();
                configuraAsistencias.activadosFiltro = ("0,0,1");
                configuraAsistencias.filtro = ("Excepcion");
                configuraAsistencias.activadosMovimientos = ("1,0,0");
                configuraAsistencias.movimiento = ("Empleado");
                configuraAsistencias.nombre = ("Asistencia por excepción");
                configuraAsistencias.ordenId = (2);
                configuraAsistencias.excepciones = (listExcepciones);
                configuraAsistencias.sistema = (true);
                listConfiguraAsistencias.Add(configuraAsistencias);

                for (i = 0; i < listConfiguraAsistencias.Count; i++)
                {
                    dbContext.Set<ConfigAsistencias>().Add(listConfiguraAsistencias[i]);
                }

                /*contador = contador + listConfiguraAsistencias.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region Configuracion IMSS
                ConfiguracionIMSS configuracionIMSS = new ConfiguracionIMSS();
                DateTime s = new DateTime(2018, 1, 1, 12, 0, 0);
                configuracionIMSS.fechaAplica = (s);
                configuracionIMSS.excedenteEspecie = (3F);
                configuracionIMSS.cuotaFijaPatron = (1F);
                #region Tasas empleado
                configuracionIMSS.tasaEspecieEnfermeMater = (0.40F);
                configuracionIMSS.tasaDineEnfermeMater = (0.25F);
                configuracionIMSS.tasaGastosPension = (0.38F);
                configuracionIMSS.tasaInvalidezVida = (0.63F);
                configuracionIMSS.tasaCesantiaVejez = (1.13F);
                #endregion

                #region Tasas Patronales
                configuracionIMSS.tasaFijaPatron = (1.10F);
                configuracionIMSS.tasaExcedentePatron = (20.40F);
                configuracionIMSS.tasaGastosPensPatron = (1.05F);
                configuracionIMSS.tasaRiesgosPatron = (0.54F);
                configuracionIMSS.tasaInvaliVidaPatron = (1.75F);
                configuracionIMSS.tasaGuarderiaPatron = (1.00F);
                configuracionIMSS.tasaPrestDinePatron = (0.70F);
                configuracionIMSS.tasaCesanVejezPatron = (3.15F);
                #endregion
                #region Topes
                configuracionIMSS.topeEnfermedadMaternidad = (25F);
                configuracionIMSS.topeRiesgoTrabajoGuarderias = (25F);
                configuracionIMSS.topeCesanVejez = (25F);
                configuracionIMSS.topeRetiro = (25F);
                configuracionIMSS.topeInfonavit = (25F);
                #endregion
                configuracionIMSS.tasaAportacionRetiroPatron = (2F);
                configuracionIMSS.tasaAportacionInfonavitPatron = (5F);
                dbContext.Set<ConfiguracionIMSS>().Add(configuracionIMSS);
                dbContext.SaveChanges();
                #endregion

                #region Tipo Nomina
                tipoNomina = new TipoNomina();
                tipoNomina.clave = ("0001");
                tipoNomina.descripcion = ("Diario");
                tipoNomina.periodicidad_ID = (listPeriodicidad[0].id);
                listTipoNomina.Add(tipoNomina);

                tipoNomina = new TipoNomina();
                tipoNomina.clave = ("0002");
                tipoNomina.descripcion = ("Semanal");
                tipoNomina.periodicidad_ID = (listPeriodicidad[1].id);
                listTipoNomina.Add(tipoNomina);

                tipoNomina = new TipoNomina();
                tipoNomina.clave = ("0003");
                tipoNomina.descripcion = ("Decenal");
                tipoNomina.periodicidad_ID = (listPeriodicidad[9].id);
                listTipoNomina.Add(tipoNomina);

                tipoNomina = new TipoNomina();
                tipoNomina.clave = ("0004");
                tipoNomina.descripcion = ("Catorcenal");
                tipoNomina.periodicidad_ID = (listPeriodicidad[2].id);
                listTipoNomina.Add(tipoNomina);

                tipoNomina = new TipoNomina();
                tipoNomina.clave = ("0005");
                tipoNomina.descripcion = ("Quincenal");
                tipoNomina.periodicidad_ID = (listPeriodicidad[3].id);
                tipoNomina.detalleConceptoRecibo = ("Quincenal");
                listTipoNomina.Add(tipoNomina);

                tipoNomina = new TipoNomina();
                tipoNomina.clave = ("0006");
                tipoNomina.descripcion = ("Mensual");
                tipoNomina.periodicidad_ID = (listPeriodicidad[4].id);
                listTipoNomina.Add(tipoNomina);

                tipoNomina = new TipoNomina();
                tipoNomina.clave = ("0007");
                tipoNomina.descripcion = ("Anual");
                tipoNomina.periodicidad_ID = (listPeriodicidad[8].id);
                listTipoNomina.Add(tipoNomina);

                for (i = 0; i < listTipoNomina.Count; i++)
                {
                    dbContext.Set<TipoNomina>().Add(listTipoNomina[i]);
                }

                contador = contador + listTipoNomina.Count();
                /* if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Registro Patronal
                registroPatronal = new RegistroPatronal();
                registroPatronal.clave = ("001");
                registroPatronal.calle = ("Conocido");
                registroPatronal.colonia = ("Centro");
                registroPatronal.convenio = (false);
                registroPatronal.nombreregtpatronal = ("Empresa pruebas");
                registroPatronal.numeroex = ("55");
                registroPatronal.numeroin = ("56");
                registroPatronal.registroPatronal = ("E4500125555");
                registroPatronal.telefono = ("0008102515");
                registroPatronal.riesgoPuesto = ("1");
                registroPatronal.cp_ID = (listCp[0].id);
                registroPatronal.ciudades_ID = (listCiudades[0].id);
                registroPatronal.municipios_ID = (listMunicipios[11].id);
                registroPatronal.estados_ID = (listEstados[24].id);
                registroPatronal.paises_ID = (listPaises[0].id);
                registroPatronal.razonesSociales_ID = (listRazonesSociales[0].id);
                listRegistroPatronal.Add(registroPatronal);
                for (i = 0; i < listRegistroPatronal.Count; i++)
                {
                    dbContext.Set<RegistroPatronal>().Add(listRegistroPatronal[i]);
                }

                /* contador = contador + listRegistroPatronal.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Centro de Costos
                centroDeCosto = new CentroDeCosto();
                centroDeCosto.clave = ("001");
                centroDeCosto.calle = ("Conocido 2");
                centroDeCosto.colonia = ("Centro");
                centroDeCosto.descripcion = ("Ventas");
                centroDeCosto.descripcionPrevia = ("V1");
                centroDeCosto.numeroExterior = ("15");
                centroDeCosto.numeroInterior = ("14");
                centroDeCosto.subCuenta = ("11125111");
                centroDeCosto.telefono = ("6681521521");
                centroDeCosto.cp_ID = (listCp[0].id);
                centroDeCosto.ciudades_ID = (listCiudades[0].id);
                centroDeCosto.municipios_ID = (listMunicipios[11].id);
                centroDeCosto.estados_ID = (listEstados[24].id);
                centroDeCosto.paises_ID = (listPaises[0].id);
                centroDeCosto.razonesSociales_ID = (listRazonesSociales[0].id);
                centroDeCosto.registroPatronal_ID = (listRegistroPatronal[0].id);
                listCentroDeCosto.Add(centroDeCosto);
                for (i = 0; i < listCentroDeCosto.Count; i++)
                {
                    dbContext.Set<CentroDeCosto>().Add(listCentroDeCosto[i]);
                }
                /* contador = contador + listCentroDeCosto.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Categoria Puesto
                categoriasPuestos = new CategoriasPuestos();
                categoriasPuestos.clave = ("001");
                categoriasPuestos.descripcion = ("Gerente");
                categoriasPuestos.estado = (true);
                categoriasPuestos.pagarPorHoras = (false);
                listCategoriaPuestos.Add(categoriasPuestos);

                for (i = 0; i < listCategoriaPuestos.Count; i++)
                {
                    dbContext.Set<CategoriasPuestos>().Add(listCategoriaPuestos[i]);
                }

                /*contador = contador + listCategoriaPuestos.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region Puestos
                puestos = new Puestos();
                puestos.clave = ("001");
                puestos.descripcion = ("Gerente");
                puestos.descripcionPrevia = ("G1");
                puestos.funciones = ("Administrar");
                puestos.maximo = (200);
                puestos.minimo = (100);
                puestos.salarioTabular = (100f);
                puestos.categoriasPuestos_ID = (listCategoriaPuestos[0].id);
                puestos.registroPatronal_ID = (listRegistroPatronal[0].id);
                listPuestos.Add(puestos);
                for (i = 0; i < listPuestos.Count; i++)
                {
                    dbContext.Set<Puestos>().Add(listPuestos[i]);
                }

                /*  contador = contador + listPuestos.Count();
                  if (contador % rango == 0 & contador > 0)
                  {
                      session.flush();
                      session.clear();
                      contador = 0;
                  }*/
                dbContext.SaveChanges();
                #endregion

                #region Departamento
                departamentos = new Departamentos();
                departamentos.clave = ("001");
                departamentos.descripcion = ("Administrativo");
                departamentos.subCuenta = ("111111012");
                departamentos.razonesSociales_ID = (listRazonesSociales[0].id);
                listDepartamentos.Add(departamentos);

                for (i = 0; i < listDepartamentos.Count; i++)
                {
                    dbContext.Set<Departamentos>().Add(listDepartamentos[i]);
                }

                /* contador = contador + listDepartamentos.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Horario
                horario = new Horario();
                horario.clave = ("001");
                horario.descripcion = ("MATUTINO");
                horario.razonesSociales_ID = (listRazonesSociales[0].id);
                s = new DateTime(2018, 12, 1, 8, 0, 0);
                horario.horaEntrada = (s);
                horario.tiempoComer = (2.0);
                s = new DateTime(2018, 12, 1, 13, 0, 0);
                horario.horaInicioComer = (s);
                s = new DateTime(2018, 12, 1, 15, 0, 0);
                horario.horaFinalComer = (s);
                s = new DateTime(2018, 12, 1, 18, 30, 0);
                horario.horaSalida = (s);
                horario.tiempo1erCoffeBreak = (10.0);
                s = new DateTime(2018, 12, 1, 12, 0, 0);
                horario.horaInicio1erCoffeBreak = (s);
                s = new DateTime(2018, 12, 1, 12, 10, 0);
                horario.horaFinal1erCoffeBreak = (s);
                horario.tiempo2doCoffeBreak = (10.0);
                s = new DateTime(2018, 12, 1, 17, 30, 0);
                horario.horaInicio2doCoffeBreak = (s);
                s = new DateTime(2018, 12, 1, 17, 40, 0);
                horario.horaFinal2doCoffeBreak = (s);
                horario.topeDiarioHrsExtras = (3.0);
                s = new DateTime(2018, 12, 1, 18, 31, 0);
                horario.horaInicioHrsExtra = (s);
                listHorario.Add(horario);
                for (i = 0; i < listHorario.Count; i++)
                {
                    dbContext.Set<Horario>().Add(listHorario[i]);
                }

                /* contador = contador + listHorario.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Jornada
                jornada = new Jornada();
                jornada.clave = ("01");
                jornada.descripcion = ("Diurna");
                listJornada.Add(jornada);

                jornada = new Jornada();
                jornada.clave = ("02");
                jornada.descripcion = ("Nocturna");
                listJornada.Add(jornada);

                jornada = new Jornada();
                jornada.clave = ("03");
                jornada.descripcion = ("Mixta");
                listJornada.Add(jornada);

                jornada = new Jornada();
                jornada.clave = ("04");
                jornada.descripcion = ("Por hora");
                listJornada.Add(jornada);

                jornada = new Jornada();
                jornada.clave = ("05");
                jornada.descripcion = ("Reducida");
                listJornada.Add(jornada);

                jornada = new Jornada();
                jornada.clave = ("06");
                jornada.descripcion = ("Continuada");
                listJornada.Add(jornada);

                jornada = new Jornada();
                jornada.clave = ("07");
                jornada.descripcion = ("Partida");
                listJornada.Add(jornada);

                jornada = new Jornada();
                jornada.clave = ("08");
                jornada.descripcion = ("Por turnos");
                listJornada.Add(jornada);

                jornada = new Jornada();
                jornada.clave = ("09");
                jornada.descripcion = ("Otra Jornada");
                listJornada.Add(jornada);

                for (i = 0; i < listJornada.Count; i++)
                {
                    dbContext.Set<Jornada>().Add(listJornada[i]);
                }

                /* contador = contador + listJornada.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Turnos
                turnos = new Turnos();
                turnos.clave = ("001");
                turnos.descripcion = ("Administrativo");
                turnos.diasJornada = (6);
                turnos.horaJornada = ((float)8.0);
                turnos.topeHorasDoblesSemanal = (9);
                turnos.Jornada_ID = (listJornada[0].id);
                turnos.primerDiaSemana = (2);
                turnos.tipoDeJornadaIMSS = (1);
                turnos.tipoDeTurno = (1);
                turnos.razonesSociales_ID = (listRazonesSociales[0].id);
                listTurnos.Add(turnos);

                for (i = 0; i < listTurnos.Count; i++)
                {
                    dbContext.Set<Turnos>().Add(listTurnos[i]);
                }

                for (i = 0; i < listTurnos.Count; i++)
                {
                    List<TurnosHorariosFijos> list = new List<TurnosHorariosFijos>();
                    TurnosHorariosFijos turnosHorariosFijos = new TurnosHorariosFijos();
                    turnosHorariosFijos.diaSemana = (DiaSemana.Lunes);
                    turnosHorariosFijos.horario = (listHorario[0]);
                    turnosHorariosFijos.ordenDia = (2);
                    turnosHorariosFijos.razonesSociales_ID = (listRazonesSociales[0].id);
                    turnosHorariosFijos.turnos_ID = (listTurnos[0].id);
                    turnosHorariosFijos.statusDia = (0);
                    list.Add(turnosHorariosFijos);

                    turnosHorariosFijos = new TurnosHorariosFijos();
                    turnosHorariosFijos.diaSemana = (DiaSemana.Martes);
                    turnosHorariosFijos.horario = (listHorario[0]);
                    turnosHorariosFijos.ordenDia = (3);
                    turnosHorariosFijos.razonesSociales_ID = (listRazonesSociales[0].id);
                    turnosHorariosFijos.turnos_ID = (listTurnos[0].id);
                    turnosHorariosFijos.statusDia = (0);
                    list.Add(turnosHorariosFijos);

                    turnosHorariosFijos = new TurnosHorariosFijos();
                    turnosHorariosFijos.diaSemana = (DiaSemana.Miercoles);
                    turnosHorariosFijos.horario = (listHorario[0]);
                    turnosHorariosFijos.ordenDia = (4);
                    turnosHorariosFijos.razonesSociales_ID = (listRazonesSociales[0].id);
                    turnosHorariosFijos.turnos_ID = (listTurnos[0].id);
                    turnosHorariosFijos.statusDia = (0);
                    list.Add(turnosHorariosFijos);

                    turnosHorariosFijos = new TurnosHorariosFijos();
                    turnosHorariosFijos.diaSemana = (DiaSemana.Jueves);
                    turnosHorariosFijos.horario = (listHorario[0]);
                    turnosHorariosFijos.ordenDia = (5);
                    turnosHorariosFijos.razonesSociales_ID = (listRazonesSociales[0].id);
                    turnosHorariosFijos.turnos_ID = (listTurnos[0].id);
                    turnosHorariosFijos.statusDia = (0);
                    list.Add(turnosHorariosFijos);

                    turnosHorariosFijos = new TurnosHorariosFijos();
                    turnosHorariosFijos.diaSemana = (DiaSemana.Viernes);
                    turnosHorariosFijos.horario = (listHorario[0]);
                    turnosHorariosFijos.ordenDia = (6);
                    turnosHorariosFijos.razonesSociales_ID = (listRazonesSociales[0].id);
                    turnosHorariosFijos.turnos_ID = (listTurnos[0].id);
                    turnosHorariosFijos.statusDia = (0);
                    list.Add(turnosHorariosFijos);

                    turnosHorariosFijos = new TurnosHorariosFijos();
                    turnosHorariosFijos.diaSemana = (DiaSemana.Sabado);
                    turnosHorariosFijos.horario = (listHorario[0]);
                    turnosHorariosFijos.ordenDia = (7);
                    turnosHorariosFijos.razonesSociales_ID = (listRazonesSociales[0].id);
                    turnosHorariosFijos.turnos_ID = (listTurnos[0].id);
                    turnosHorariosFijos.statusDia = (0);
                    list.Add(turnosHorariosFijos);
                    listTurnos[i].turnosHorariosFijos_turnos.AddRange(list);
                    dbContext.Set<Turnos>().Add(listTurnos[i]);

                }

                /*  if (contador % rango == 0 & contador > 0)
                  {
                      session.flush();
                      session.clear();
                      contador = 0;
                  }*/
                dbContext.SaveChanges();


                #endregion Tipo de Contrato

                #region Tipo de Contrato
                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("01");
                tipoContrato.descripcion = ("Contrato de trabajo por tiempo indeterminado");
                tipoContrato.esSindicalizado = (false);
                listTipoContratos.Add(tipoContrato);

                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("02");
                tipoContrato.descripcion = ("Contrato de trabajo para obra determinada");
                tipoContrato.esSindicalizado = (false);
                listTipoContratos.Add(tipoContrato);

                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("03");
                tipoContrato.descripcion = ("Contrato de trabajo por tiempo determinado");
                tipoContrato.esSindicalizado = (false);
                listTipoContratos.Add(tipoContrato);

                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("04");
                tipoContrato.descripcion = ("Contrato de trabajo por temporada");
                tipoContrato.esSindicalizado = (true);
                listTipoContratos.Add(tipoContrato);

                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("05");
                tipoContrato.descripcion = ("Contrato de trabajo sujeto a prueba");
                tipoContrato.esSindicalizado = (false);
                listTipoContratos.Add(tipoContrato);

                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("06");
                tipoContrato.descripcion = ("Contrato de trabajo con capacitación inicial");
                tipoContrato.esSindicalizado = (false);
                listTipoContratos.Add(tipoContrato);

                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("07");
                tipoContrato.descripcion = ("Modalidad de contratación por pago de hora laborada");
                tipoContrato.esSindicalizado = (false);
                listTipoContratos.Add(tipoContrato);

                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("08");
                tipoContrato.descripcion = ("Modalidad de trabajo por comisión laboral");
                tipoContrato.esSindicalizado = (false);
                listTipoContratos.Add(tipoContrato);

                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("09");
                tipoContrato.descripcion = ("Modalidades de contratación donde no existe relación de trabajo");
                tipoContrato.esSindicalizado = (false);
                listTipoContratos.Add(tipoContrato);

                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("10");
                tipoContrato.descripcion = ("Jubilación, pensión, retiro.");
                tipoContrato.esSindicalizado = (false);
                listTipoContratos.Add(tipoContrato);

                tipoContrato = new TipoContrato();
                tipoContrato.clave = ("99");
                tipoContrato.descripcion = ("Otro contrato");
                tipoContrato.esSindicalizado = (false);
                listTipoContratos.Add(tipoContrato);

                for (i = 0; i < listTipoContratos.Count; i++)
                {
                    dbContext.Set<TipoContrato>().Add(listTipoContratos[i]);
                }

                contador = contador + listTipoContratos.Count();
                /*if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region Formas de Pago
                formasDePago = new FormasDePago();
                formasDePago.id = (1L);
                formasDePago.clave = ("1");
                formasDePago.descripcion = ("Efectivo");
                listFormasDePago.Add(formasDePago);

                formasDePago = new FormasDePago();
                formasDePago.id = (2L);
                formasDePago.clave = ("2");
                formasDePago.descripcion = ("Cheque");
                listFormasDePago.Add(formasDePago);

                formasDePago = new FormasDePago();
                formasDePago.id = (3L);
                formasDePago.clave = ("3");
                formasDePago.descripcion = ("Transferencia Electronica");
                listFormasDePago.Add(formasDePago);

                for (i = 0; i < listFormasDePago.Count; i++)
                {
                    dbContext.Set<FormasDePago>().Add(listFormasDePago[i]);
                }

                /*contador = contador + listFormasDePago.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region Bancos
                bancos = new Bancos();
                bancos.clave = ("002");
                bancos.descripcion = ("BANAMEX");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Nacional de México, S.A., Institución de Banca Múltiple, Grupo Financiero Banamex");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("006");
                bancos.descripcion = ("BANCOMEXT");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Nacional de Comercio Exterior, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("009");
                bancos.descripcion = ("BANOBRAS");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Nacional de Obras y Servicios Públicos, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("012");
                bancos.descripcion = ("BBVA BANCOMER");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("BBVA Bancomer, S.A., Institución de Banca Múltiple, Grupo Financiero BBVA Bancomer");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("014");
                bancos.descripcion = ("SANTANDER");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Santander (México), S.A., Institución de Banca Múltiple, Grupo Financiero Santander");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("019");
                bancos.descripcion = ("BANJERCITO");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Nacional del Ejército, Fuerza Aérea y Armada, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("021");
                bancos.descripcion = ("HSBC");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("HSBC México, S.A., institución De Banca Múltiple, Grupo Financiero HSBC");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("030");
                bancos.descripcion = ("BAJIO");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco del Bajío, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("032");
                bancos.descripcion = ("IXE");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("IXE Banco, S.A., Institución de Banca Múltiple, IXE Grupo Financiero");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("036");
                bancos.descripcion = ("INBURSA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Inbursa, S.A., Institución de Banca Múltiple, Grupo Financiero Inbursa");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("037");
                bancos.descripcion = ("INTERACCIONES");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Interacciones, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("042");
                bancos.descripcion = ("MIFEL");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banca Mifel, S.A., Institución de Banca Múltiple, Grupo Financiero Mifel");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("044");
                bancos.descripcion = ("SCOTIABANK");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Scotiabank Inverlat, S.A.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("058");
                bancos.descripcion = ("BANREGIO");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Regional de Monterrey, S.A., Institución de Banca Múltiple, Banregio Grupo Financiero");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("059");
                bancos.descripcion = ("INVEX");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Invex, S.A., Institución de Banca Múltiple, Invex Grupo Financiero");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("060");
                bancos.descripcion = ("BANSI");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Bansi, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("062");
                bancos.descripcion = ("AFIRME");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banca Afirme, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("072");
                bancos.descripcion = ("BANORTE");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Mercantil del Norte, S.A., Institución de Banca Múltiple, Grupo Financiero Banorte");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("102");
                bancos.descripcion = ("THE ROYAL BANK");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("The Royal Bank of Scotland México, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("103");
                bancos.descripcion = ("AMERICAN EXPRESS");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("American Express Bank (México), S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("106");
                bancos.descripcion = ("BAMSA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Bank of America México, S.A., Institución de Banca Múltiple, Grupo Financiero Bank of America");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("108");
                bancos.descripcion = ("TOKYO");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Bank of Tokyo-Mitsubishi UFJ (México), S.A.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("112");
                bancos.descripcion = ("BMONEX");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Monex, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("113");
                bancos.descripcion = ("VE POR MAS");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Ve Por Mas, S.A. Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("116");
                bancos.descripcion = ("ING");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("ING Bank (México), S.A., Institución de Banca Múltiple, ING Grupo Financiero");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("124");
                bancos.descripcion = ("DEUTSCHE");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Deutsche Bank México, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("126");
                bancos.descripcion = ("CREDIT SUISSE");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Credit Suisse (México), S.A. Institución de Banca Múltiple, Grupo Financiero Credit Suisse (México)");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("127");
                bancos.descripcion = ("AZTECA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Azteca, S.A. Institución de Banca Múltiple.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("128");
                bancos.descripcion = ("AUTOFIN");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Autofin México, S.A. Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("129");
                bancos.descripcion = ("BARCLAYS");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Barclays Bank México, S.A., Institución de Banca Múltiple, Grupo Financiero Barclays México");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("130");
                bancos.descripcion = ("COMPARTAMOS");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Compartamos, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("131");
                bancos.descripcion = ("BANCO FAMSA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Ahorro Famsa, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("132");
                bancos.descripcion = ("BMULTIVA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Multiva, S.A., Institución de Banca Múltiple, Multivalores Grupo Financiero");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("133");
                bancos.descripcion = ("ACTINVER");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Actinver, S.A. Institución de Banca Múltiple, Grupo Financiero Actinver");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("134");
                bancos.descripcion = ("WAL-MART");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Wal-Mart de México Adelante, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("135");
                bancos.descripcion = ("NAFIN");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Nacional Financiera, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("136");
                bancos.descripcion = ("INTERBANCO");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Inter Banco, S.A. Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("137");
                bancos.descripcion = ("BANCOPPEL");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("BanCoppel, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("138");
                bancos.descripcion = ("ABC CAPITAL");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("ABC Capital, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("139");
                bancos.descripcion = ("UBS BANK");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("UBS Bank México, S.A., Institución de Banca Múltiple, UBS Grupo Financiero");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("140");
                bancos.descripcion = ("CONSUBANCO");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Consubanco, S.A. Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("141");
                bancos.descripcion = ("VOLKSWAGEN");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Volkswagen Bank, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("143");
                bancos.descripcion = ("CIBANCO");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("CIBanco, S.A.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("145");
                bancos.descripcion = ("BBASE");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco Base, S.A., Institución de Banca Múltiple");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("166");
                bancos.descripcion = ("BANSEFI");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Banco del Ahorro Nacional y Servicios Financieros, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("168");
                bancos.descripcion = ("HIPOTECARIA FEDERAL");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Sociedad Hipotecaria Federal, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("600");
                bancos.descripcion = ("MONEXCB");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Monex Casa de Bolsa, S.A. de C.V. Monex Grupo Financiero");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("601");
                bancos.descripcion = ("GBM");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("GBM Grupo Bursátil Mexicano, S.A. de C.V. Casa de Bolsa");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("602");
                bancos.descripcion = ("MASARI");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Masari Casa de Bolsa, S.A.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("605");
                bancos.descripcion = ("VALUE");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Value, S.A. de C.V. Casa de Bolsa");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("606");
                bancos.descripcion = ("ESTRUCTURADORES");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Estructuradores del Mercado de Valores Casa de Bolsa, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("607");
                bancos.descripcion = ("TIBER");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Casa de Cambio Tiber, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("608");
                bancos.descripcion = ("VECTOR");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Vector Casa de Bolsa, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("610");
                bancos.descripcion = ("B&B");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("B y B, Casa de Cambio, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("614");
                bancos.descripcion = ("ACCIVAL");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Acciones y Valores Banamex, S.A. de C.V., Casa de Bolsa");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("615");
                bancos.descripcion = ("MERRILL LYNCH");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Merrill Lynch México, S.A. de C.V. Casa de Bolsa");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("616");
                bancos.descripcion = ("FINAMEX");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Casa de Bolsa Finamex, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("617");
                bancos.descripcion = ("VALMEX");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Valores Mexicanos Casa de Bolsa, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("618");
                bancos.descripcion = ("UNICA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Unica Casa de Cambio, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("619");
                bancos.descripcion = ("MAPFRE");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("MAPFRE Tepeyac, S.A.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("620");
                bancos.descripcion = ("PROFUTURO");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Profuturo G.N.P., S.A. de C.V., Afore");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("621");
                bancos.descripcion = ("CB ACTINVER");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Actinver Casa de Bolsa, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("622");
                bancos.descripcion = ("OACTIN");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("OPERADORA ACTINVER, S.A. DE C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("623");
                bancos.descripcion = ("SKANDIA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Skandia Vida, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("626");
                bancos.descripcion = ("CBDEUTSCHE");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Deutsche Securities, S.A. de C.V. CASA DE BOLSA");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("627");
                bancos.descripcion = ("ZURICH");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Zurich Compañía de Seguros, S.A.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("628");
                bancos.descripcion = ("ZURICHVI");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Zurich Vida, Compañía de Seguros, S.A.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("629");
                bancos.descripcion = ("SU CASITA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Hipotecaria Su Casita, S.A. de C.V. SOFOM ENR");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("630");
                bancos.descripcion = ("CB INTERCAM");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Intercam Casa de Bolsa, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("631");
                bancos.descripcion = ("CI BOLSA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("CI Casa de Bolsa, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("632");
                bancos.descripcion = ("BULLTICK CB");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Bulltick Casa de Bolsa, S.A., de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("633");
                bancos.descripcion = ("STERLING");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Sterling Casa de Cambio, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("634");
                bancos.descripcion = ("FINCOMUN");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Fincomún, Servicios Financieros Comunitarios, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("636");
                bancos.descripcion = ("HDI SEGUROS");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("HDI Seguros, S.A. de C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("637");
                bancos.descripcion = ("ORDER");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Order Express Casa de Cambio, S.A. de C.V");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("638");
                bancos.descripcion = ("AKALA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Akala, S.A. de C.V., Sociedad Financiera Popular");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("640");
                bancos.descripcion = ("CB JPMORGAN");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("J.P. Morgan Casa de Bolsa, S.A. de C.V. J.P. Morgan Grupo Financiero");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("642");
                bancos.descripcion = ("REFORMA");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Operadora de Recursos Reforma, S.A. de C.V., S.F.P.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("646");
                bancos.descripcion = ("STP");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Sistema de Transferencias y Pagos STP, S.A. de C.V.SOFOM ENR");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("647");
                bancos.descripcion = ("TELECOMM");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Telecomunicaciones de México");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("648");
                bancos.descripcion = ("EVERCORE");
                bancos.domicilio = ("");
                bancos.notas = ("Evercore Casa de Bolsa, S.A. de C.V.");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("649");
                bancos.descripcion = ("SKANDIA");
                bancos.domicilio = ("");
                bancos.notas = ("Skandia Operadora de Fondos, S.A. de C.V.");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("651");
                bancos.descripcion = ("SEGMTY");
                bancos.domicilio = ("");
                bancos.notas = ("Seguros Monterrey New York Life, S.A de C.V");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("652");
                bancos.descripcion = ("ASEA");
                bancos.domicilio = ("");
                bancos.notas = ("Solución Asea, S.A. de C.V., Sociedad Financiera Popular");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("653");
                bancos.descripcion = ("KUSPIT");
                bancos.domicilio = ("");
                bancos.notas = ("Kuspit Casa de Bolsa, S.A. de C.V.");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("655");
                bancos.descripcion = ("SOFIEXPRESS");
                bancos.domicilio = ("");
                bancos.notas = ("J.P. SOFIEXPRESS, S.A. de C.V., S.F.P.");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("656");
                bancos.descripcion = ("UNAGRA");
                bancos.domicilio = ("");
                bancos.notas = ("UNAGRA, S.A. de C.V., S.F.P.");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("659");
                bancos.descripcion = ("OPCIONES EMPRESARIALES DEL NOROESTE");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("OPCIONES EMPRESARIALES DEL NORESTE, S.A. DE C.V., S.F.P.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("670");
                bancos.descripcion = ("LIBERTAD");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Libertad Servicios Financieros, S.A. De C.V.");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("901");
                bancos.descripcion = ("CLS");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("Cls Bank International");
                listBancos.Add(bancos);

                bancos = new Bancos();
                bancos.clave = ("902");
                bancos.descripcion = ("INDEVAL");
                bancos.domicilio = ("");
                bancos.paginaweb = ("");
                bancos.RFC = ("");
                bancos.notas = ("SD. Indeval, S.A. de C.V.");
                listBancos.Add(bancos);

                for (i = 0; i < listBancos.Count; i++)
                {
                    dbContext.Set<Bancos>().Add(listBancos[i]);
                }

                /* contador = contador + listBancos.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Parentesco
                parentesco = new Parentesco();
                parentesco.clave = ("01");
                parentesco.descripcion = ("Madre");
                listParentesco.Add(parentesco);

                parentesco = new Parentesco();
                parentesco.clave = ("02");
                parentesco.descripcion = ("Padre");
                listParentesco.Add(parentesco);

                parentesco = new Parentesco();
                parentesco.clave = ("03");
                parentesco.descripcion = ("Hermano(a)");
                listParentesco.Add(parentesco);

                parentesco = new Parentesco();
                parentesco.clave = ("04");
                parentesco.descripcion = ("Primo(a)");
                listParentesco.Add(parentesco);

                for (i = 0; i < listParentesco.Count; i++)
                {
                    dbContext.Set<Parentesco>().Add(listParentesco[i]);
                }

                /*contador = contador + listParentesco.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Cursos
                cursos = new Cursos();
                cursos.clave = ("01");
                cursos.descripcion = ("Curso1");
                listCursos.Add(cursos);

                cursos = new Cursos();
                cursos.clave = ("02");
                cursos.descripcion = ("Curso2");
                listCursos.Add(cursos);

                for (i = 0; i < listCursos.Count; i++)
                {
                    dbContext.Set<Cursos>().Add(listCursos[i]);
                }

                /* contador = contador + listCursos.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;

                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Estudios
                estudios = new Estudios();
                estudios.clave = ("01");
                estudios.descripcion = ("Primaria");
                listEstudios.Add(estudios);

                estudios = new Estudios();
                estudios.clave = ("02");
                estudios.descripcion = ("Secundaria");
                listEstudios.Add(estudios);

                estudios = new Estudios();
                estudios.clave = ("03");
                estudios.descripcion = ("Preparatoria");
                listEstudios.Add(estudios);

                estudios = new Estudios();
                estudios.clave = ("04");
                estudios.descripcion = ("Carrera Profesional");
                listEstudios.Add(estudios);

                for (i = 0; i < listEstudios.Count; i++)
                {
                    dbContext.Set<Estudios>().Add(listEstudios[i]);
                }

                /* contador = contador + listEstudios.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Firmas
                firmas = new Firmas();
                firmas.clave = ("01");
                firmas.puesto = ("Gerente");
                firmas.descripcion = ("AAAAAA");
                listFirmas.Add(firmas);

                for (i = 0; i < listFirmas.Count; i++)
                {
                    dbContext.Set<Firmas>().Add(listFirmas[i]);
                }

                /* contador = contador + listFirmas.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region Plazas
                plazas = new Plazas();
                plazas.clave = ("01");
                plazas.horas = (8d);
                plazas.importe = (200);
                plazas.tipoRelacionLaboral = (1);
                plazas.tipoSalario = (1);
                plazas.salarioPor = (2);
                plazas.tipoContrato_ID = (listTipoContratos[0].id);
                plazas.categoriasPuestos_ID = (listCategoriaPuestos[0].id);
                plazas.centroDeCosto_ID = (listCentroDeCosto[0].id);
                plazas.departamentos_ID = (listDepartamentos[0].id);
                plazas.puestos_ID = (listPuestos[0].id);
                plazas.registroPatronal_ID = (listRegistroPatronal[0].id);
                plazas.tipoNomina_ID = (listTipoNomina[4].id);
                plazas.turnos_ID = (listTurnos[0].id);
                plazas.cantidadPlazas = (1);
                plazas.razonesSociales_ID = (listRazonesSociales[0].id);
                listPlazas.Add(plazas);

                for (i = 0; i < listPlazas.Count; i++)
                {
                    dbContext.Set<Plazas>().Add(listPlazas[i]);
                }

                /*contador = contador + listPlazas.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region Genero
                List<Genero> generos = new List<Genero>();
                Genero genMujer = new Genero();
                genMujer.clave = ("001");
                genMujer.descripcion = ("Femenino");
                Genero genHombre = new Genero();
                genHombre.clave = ("002");
                genHombre.descripcion = ("Masculino");
                generos.Add(genHombre);
                generos.Add(genMujer);
                for (i = 0; i < generos.Count; i++)
                {
                    dbContext.Set<Genero>().Add(generos[i]);
                }

                dbContext.SaveChanges();
                #endregion

                #region Empleados
                empleados = new Empleados();
                empleados.clave = ("00001");
                empleados.razonesSociales_ID = (listRazonesSociales[0].id);
                empleados.apellidoPaterno = ("Lopez");
                empleados.apellidoMaterno = ("Lopez");
                empleados.nombre = ("Quintana Jesus");
                empleados.nombreAbreviado = ("Quintanilla");
                empleados.domicilio = ("Av. Velasquez");
                empleados.numeroExt = ("12");
                empleados.numeroInt = ("15");
                empleados.colonia = ("Centro");
                empleados.cp_ID = (listCp[0].id);
                empleados.ciudades_ID = (listCiudades[0].id);
                empleados.municipios_ID = (listMunicipios[11].id);
                empleados.estados_ID = (listEstados[24].id);
                empleados.paises_ID = (listPaises[0].id);
                DateTime calendar = new DateTime(1990, 3, 15);
                empleados.fechaNacimiento = (calendar);
                calendar = new DateTime(2017, 7, 9);
                empleados.fechaIngresoEmpresa = (calendar);
                empleados.paisOrigen_ID = (listPaises[0].id);
                empleados.nacionalidad = ("Mexicano");
                empleados.estadoNacimiento_ID = (listEstados[23].id);
                empleados.lugarNacimiento = ("Ahome");
                empleados.RFC = ("LOGQ1202126SA");
                empleados.CURP = ("LOGQ120212HSPPTNA2");
                empleados.IMSS = ("23152415211");
                empleados.clinicaIMSS = ("Ahome");
                empleados.estadoCivil = (1);
                empleados.genero_ID = (generos[0].id);
                empleados.status = (true);

                listEmpleado.Add(empleados);

                for (i = 0; i < listEmpleado.Count(); i++)
                {
                    dbContext.Set<Empleados>().Add(listEmpleado[i]);
                }
                dbContext.SaveChanges();
                #endregion

                #region Ingresos Y Bajas
                IngresosBajas ingresosBajas = new IngresosBajas();
                ingresosBajas.empleados_ID = (listEmpleado[0].id);
                calendar = new DateTime(2017, 7, 9, 12, 0, 0);
                ingresosBajas.fechaIngreso = (calendar);
                calendar = new DateTime(calendar.Year + 100, 7, 9, 12, 0, 0);
                ingresosBajas.fechaBaja = (calendar);
                //ingresosBajas.plazasPorEmpleado_ID = (listPlazasPorEmpleados[0].id);
                ingresosBajas.razonesSociales_ID = (listRazonesSociales[0].id);
                ingresosBajas.registroPatronal_ID = (listRegistroPatronal[0].id);
                // ingresosBajas.tipoMovimiento = (IngReingresosBajas.TipoMovimiento.I);
                dbContext.Set<IngresosBajas>().Add(ingresosBajas);
                /* contador = contador + 1;
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region PlazasPorEmpleado
                List<PlazasPorEmpleado> listPlazasPorEmpleados = new List<PlazasPorEmpleado>();
                PlazasPorEmpleado plazasPorEmpleado = new PlazasPorEmpleado();
                plazasPorEmpleado.empleados_ID = (listEmpleado[0].id);
                DateTime c = new DateTime(2017 + 100, 7, 9, 12, 0, 0);
                plazasPorEmpleado.fechaFinal = (c);
                c = new DateTime(2017, 7, 9, 12, 0, 0);
                plazasPorEmpleado.fechaPrestaciones = (calendar);
                plazasPorEmpleado.razonesSociales_ID = (listRazonesSociales[0].id);
                plazasPorEmpleado.referencia = ("000000000000001");
                plazasPorEmpleado.registroPatronal_ID = (listRegistroPatronal[0].id);
                plazasPorEmpleado.plazaPrincipal = (true);
                plazasPorEmpleado.ingresosBajas = ingresosBajas;
                listPlazasPorEmpleados.Add(plazasPorEmpleado);
                dbContext.Set<PlazasPorEmpleado>().Add(listPlazasPorEmpleados[0]);

                List<PlazasPorEmpleadosMov> listPlazasPorEmpleadosMovs = new List<PlazasPorEmpleadosMov>();
                PlazasPorEmpleadosMov plazasPorEmpleadosMov = new PlazasPorEmpleadosMov();
                c = new DateTime(2017, 7, 9, 12, 0, 0);
                plazasPorEmpleadosMov.fechaInicial = (calendar);
                plazasPorEmpleadosMov.fechaIMSS = (calendar);
                plazasPorEmpleadosMov.tipoNomina_ID = (listTipoNomina[4].id);
                plazasPorEmpleadosMov.cambioTipoDeNomina = (false);
                plazasPorEmpleadosMov.centroDeCosto_ID = (listCentroDeCosto[0].id);
                plazasPorEmpleadosMov.cambioCentroDeCostos = (false);
                plazasPorEmpleadosMov.departamentos_ID = (listDepartamentos[0].id);
                plazasPorEmpleadosMov.cambioDepartamento = (false);
                plazasPorEmpleadosMov.puestos_ID = (listPuestos[0].id);
                plazasPorEmpleadosMov.cambioPuestos = (false);
                plazasPorEmpleadosMov.turnos_ID = (listTurnos[0].id);
                plazasPorEmpleadosMov.cambioTurno = (false);
                plazasPorEmpleadosMov.tipoContrato_ID = (listTipoContratos[0].id);
                plazasPorEmpleadosMov.cambioTipoContrato = (false);
                plazasPorEmpleadosMov.horas = (null);
                plazasPorEmpleadosMov.cambioHoras = (false);
                plazasPorEmpleadosMov.cambioPlazasPosOrganigrama = (false);
                //   plazasPorEmpleadosMov.salarioPor = (2);
                //  plazasPorEmpleadosMov.cambioSalarioPor = (false);
                plazasPorEmpleadosMov.importe = (233.00F);
                plazasPorEmpleadosMov.sueldoDiario = (0.0);
                /* plazasPorEmpleadosMov.formasDePago_ID = (listFormasDePago[0].id);*/
                /* plazasPorEmpleadosMov.cambioFormaDePago = (false);*/
                //plazasPorEmpleadosMov.bancos_ID = (listBancos[0].id);
                // plazasPorEmpleadosMov.cambioBanco = (false);
                // plazasPorEmpleadosMov.cuentaBancaria = ("3333-3333-3333-3333");
                // plazasPorEmpleadosMov.clabe = ("11111111113");
                // plazasPorEmpleadosMov.cambioCuentaBancaria = (false);
                plazasPorEmpleadosMov.tipoRelacionLaboral = (1);
                plazasPorEmpleadosMov.cambioTipoRelacionLaboral = (false);
                // plazasPorEmpleadosMov.zonaGeografica = (int)(ZonaGeografica.ZonaGeograficaB);
                // plazasPorEmpleadosMov.cambioZonaGeografica = (false);
                plazasPorEmpleadosMov.plazasPorEmpleado_ID = (listPlazasPorEmpleados[0].id);
                plazasPorEmpleadosMov.regimenContratacion = ("02");
                plazasPorEmpleadosMov.cambioRegimenContratacion = (false);
                listPlazasPorEmpleadosMovs.Add(plazasPorEmpleadosMov);
                dbContext.Set<PlazasPorEmpleadosMov>().Add(listPlazasPorEmpleadosMovs[0]);

                /*contador = contador + listEmpleado.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region SalarioIntegrados
                SalariosIntegrados salariosIntegrados = new SalariosIntegrados();
                salariosIntegrados.empleados_ID = (listEmpleado[0].id);
                salariosIntegrados.factorIntegracion = (0D);
                salariosIntegrados.tipoDeSalario = (0);
                salariosIntegrados.salarioDiarioIntegrado = (243.8799);
                salariosIntegrados.salarioDiarioVariable = (0f);
                salariosIntegrados.salarioDiarioFijo = (243.8799);
                salariosIntegrados.registroPatronal_ID = (listRegistroPatronal[0].id);
                calendar = new DateTime(2017, 7, 9, 12, 0, 0);
                salariosIntegrados.fecha = (calendar);
                dbContext.Set<SalariosIntegrados>().Add(salariosIntegrados);
                /* contador = contador + 1;
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region ConfigConceptosSat
                List<ConfigConceptosSat> listConceptosSat = new List<ConfigConceptosSat>();
                /*PERCEPCIONES SAT*/
                ConfigConceptosSat conceptosSat = new ConfigConceptosSat();  //SUELDO
                conceptosSat.concepNomDefi_ID = (listConceptoDeNominaDefinicion[0].id);
                conceptosSat.conceptoSatClave = ("001");
                conceptosSat.otroPago = (false);
                listConceptosSat.Add(conceptosSat);

                conceptosSat = new ConfigConceptosSat(); //COMISIONES
                conceptosSat.concepNomDefi_ID = (listConceptoDeNominaDefinicion[1].id);
                conceptosSat.conceptoSatClave = ("028");
                conceptosSat.otroPago = (false);
                listConceptosSat.Add(conceptosSat);

                conceptosSat = new ConfigConceptosSat(); //SUBSIDIO EMPLEO
                conceptosSat.concepNomDefi_ID = (listConceptoDeNominaDefinicion[18].id);
                conceptosSat.conceptoSatClave = ("002");
                conceptosSat.otroPago = (true);
                listConceptosSat.Add(conceptosSat);

                /*DEDUCCIONES SAT*/
                conceptosSat = new ConfigConceptosSat(); //IMSS
                conceptosSat.concepNomDefi_ID = (listConceptoDeNominaDefinicion[32].id);
                conceptosSat.conceptoSatClave = ("001");
                conceptosSat.otroPago = (false);
                listConceptosSat.Add(conceptosSat);

                conceptosSat = new ConfigConceptosSat(); //ISR
                conceptosSat.concepNomDefi_ID = (listConceptoDeNominaDefinicion[33].id);
                conceptosSat.conceptoSatClave = ("002");
                conceptosSat.otroPago = (false);
                listConceptosSat.Add(conceptosSat);

                for (i = 0; i < listConceptosSat.Count; i++)
                {
                    dbContext.Set<ConfigConceptosSat>().Add(listConceptosSat[i]);
                }

                /* contador = contador + listConceptosSat.Count();
                 if (contador % rango == 0 & contador > 0)
                 {
                     session.flush();
                     session.clear();
                     contador = 0;
                 }*/
                dbContext.SaveChanges();
                #endregion

                #region CreditoAhorro
                DateTime localDate = DateTime.Now;
                creditoAhorro = new CreditoAhorro();
                creditoAhorro.clave = ("001");
                creditoAhorro.descripcion = ("PRESTAMOS PERSONALES");
                creditoAhorro.tipoConfiguracion = ("1");
                creditoAhorro.descripcionAbrev = ("PRESTAMOS");
                creditoAhorro.asignaAutoNumCredAho = (true);
                creditoAhorro.longitudNumCredAho = ("8");
                creditoAhorro.mascaraNumCredAho = ("########");
                creditoAhorro.inicioDescuento = (true);
                creditoAhorro.definirNumEmp = (false);
                creditoAhorro.longitudNumEmp = ("");
                creditoAhorro.modoDescuento = ((byte)1);
                creditoAhorro.porcentaje = (false);
                creditoAhorro.vsmg = (false);
                creditoAhorro.modoCapturaDescuento = (0);
                creditoAhorro.numDecimalesDescuento = (2);
                creditoAhorro.modoCapturaDescuentoVSMG = (null);
                creditoAhorro.numDecimalesDescuentoVSMG = (null);
                creditoAhorro.modoCapturaDescuentoPorc = (null);
                creditoAhorro.numDecimalesDescuentoPorc = (null);
                creditoAhorro.cuotaFija = (true);
                creditoAhorro.descPropDiasPer = (true);
                creditoAhorro.solicitarFecVen = (false);
                creditoAhorro.fondoAhorro = (false);
                creditoAhorro.modoAgregarCredAhoIngEmp = ((byte)0);
                creditoAhorro.modoDescontarCredAhoFini = ((byte)0);
                creditoAhorro.modoDescontarCredAhoLiqu = ((byte)0);
                creditoAhorro.tasaIntMens = ("");
                creditoAhorro.corteMesDia = (localDate);
                creditoAhorro.razonesSociales_ID = (listRazonesSociales[0].id);
                for (int k = 0; k < listConceptoDeNominaDefinicion.Count; k++)
                {
                    if (listConceptoDeNominaDefinicion[k].clave.Equals("760", StringComparison.InvariantCultureIgnoreCase))
                    {
                        creditoAhorro.concepNomiDefin_ID = (listConceptoDeNominaDefinicion[k].id);
                        break;
                    }
                }
                creditoAhorro.cNDInteresMensual = (null);
                creditoAhorro.cNDescuento = (null);
                creditoAhorro.conceptoDcto = ("");
                creditoAhorro.periodicidadDescuento = (0);
                creditoAhorro.cuandoDescontar = (-1);
                creditoAhorro.modoManejoDescuento = ((byte)2);
                creditoAhorro.importeDescuento = (null);
                creditoAhorro.activarManejoDescuento = (false);
                creditoAhorro.factorProporcional = ((byte)1);
                listCreditoAhorro.Add(creditoAhorro);

                creditoAhorro = new CreditoAhorro();
                creditoAhorro.clave = ("005");
                creditoAhorro.descripcion = ("INFONAVIT");
                creditoAhorro.tipoConfiguracion = ("1");
                creditoAhorro.descripcionAbrev = ("INFONAVIT");
                creditoAhorro.asignaAutoNumCredAho = (false);
                creditoAhorro.longitudNumCredAho = ("11");
                creditoAhorro.mascaraNumCredAho = ("###########");
                creditoAhorro.inicioDescuento = (false);
                creditoAhorro.definirNumEmp = (false);
                creditoAhorro.longitudNumEmp = ("");
                creditoAhorro.modoDescuento = ((byte)1);
                creditoAhorro.porcentaje = (true);
                creditoAhorro.vsmg = (true);
                creditoAhorro.modoCapturaDescuento = (1);
                creditoAhorro.numDecimalesDescuento = (4);
                creditoAhorro.modoCapturaDescuentoVSMG = (1);
                creditoAhorro.numDecimalesDescuentoVSMG = (4);
                creditoAhorro.modoCapturaDescuentoPorc = (0);
                creditoAhorro.numDecimalesDescuentoPorc = (4);
                creditoAhorro.cuotaFija = (true);
                creditoAhorro.descPropDiasPer = (false);
                creditoAhorro.solicitarFecVen = (false);
                creditoAhorro.fondoAhorro = (false);
                creditoAhorro.modoAgregarCredAhoIngEmp = ((byte)0);
                creditoAhorro.modoDescontarCredAhoFini = ((byte)0);
                creditoAhorro.modoDescontarCredAhoLiqu = ((byte)0);
                creditoAhorro.tasaIntMens = ("");
                creditoAhorro.corteMesDia = (localDate);
                creditoAhorro.razonesSociales_ID = (listRazonesSociales[0].id);
                for (int k = 0; k < listConceptoDeNominaDefinicion.Count(); k++)
                {
                    if (listConceptoDeNominaDefinicion[k].clave.Equals("750", StringComparison.InvariantCultureIgnoreCase))
                    {
                        creditoAhorro.concepNomiDefin_ID = (listConceptoDeNominaDefinicion[k].id);
                        break;
                    }
                }
                creditoAhorro.cNDInteresMensual = (null);
                for (int k = 0; k < listConceptoDeNominaDefinicion.Count(); k++)
                {
                    if (listConceptoDeNominaDefinicion[k].clave.Equals("755", StringComparison.InvariantCultureIgnoreCase))
                    {
                        creditoAhorro.cNDescuento = (listConceptoDeNominaDefinicion[k]);
                        break;
                    }
                }
                creditoAhorro.conceptoDcto = ("SEGURO");
                creditoAhorro.periodicidadDescuento = (0);
                creditoAhorro.cuandoDescontar = (-1);
                creditoAhorro.modoManejoDescuento = ((byte)2);
                creditoAhorro.importeDescuento = (1.875);
                creditoAhorro.activarManejoDescuento = (true);
                creditoAhorro.factorProporcional = ((byte)1);
                listCreditoAhorro.Add(creditoAhorro);
                for (i = 0; i < listCreditoAhorro.Count(); i++)
                {
                    dbContext.Set<CreditoAhorro>().Add(listCreditoAhorro[i]);
                }

                /*contador = contador + listCreditoAhorro.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region TiposVacaciones
                tiposVacaciones = new TiposVacaciones();
                tiposVacaciones.clave = ("01");
                tiposVacaciones.nombre = ("Vacaciones Disfrutadas");
                listTiposVacaciones.Add(tiposVacaciones);

                tiposVacaciones = new TiposVacaciones();
                tiposVacaciones.clave = ("02");
                tiposVacaciones.nombre = ("Vacaciones Trabajadas");
                listTiposVacaciones.Add(tiposVacaciones);

                for (i = 0; i < listTiposVacaciones.Count; i++)
                {
                    //session.save(listTiposVacaciones.get(i));
                    dbContext.Set<TiposVacaciones>().Add(listTiposVacaciones[i]);
                }

                /*contador = contador + listTiposVacaciones.Count();
                if (contador % rango == 0 & contador > 0)
                {
                    session.flush();
                    session.clear();
                    contador = 0;
                }*/
                dbContext.SaveChanges();
                #endregion

                #region CausasDeBaja
                causaDeBaja = new CausaDeBaja();
                causaDeBaja.clave = ("1");
                causaDeBaja.descripcion = ("Término de contrato");
                listCausaDeBaja.Add(causaDeBaja);

                causaDeBaja = new CausaDeBaja();
                causaDeBaja.clave = ("2");
                causaDeBaja.descripcion = ("Separación voluntaria");
                listCausaDeBaja.Add(causaDeBaja);

                causaDeBaja = new CausaDeBaja();
                causaDeBaja.clave = ("3");
                causaDeBaja.descripcion = ("Abandono de empleo");
                listCausaDeBaja.Add(causaDeBaja);

                causaDeBaja = new CausaDeBaja();
                causaDeBaja.clave = ("4");
                causaDeBaja.descripcion = ("Defunción");
                listCausaDeBaja.Add(causaDeBaja);

                causaDeBaja = new CausaDeBaja();
                causaDeBaja.clave = ("5");
                causaDeBaja.descripcion = ("Clausura");
                listCausaDeBaja.Add(causaDeBaja);

                causaDeBaja = new CausaDeBaja();
                causaDeBaja.clave = ("6");
                causaDeBaja.descripcion = ("Pensión");
                listCausaDeBaja.Add(causaDeBaja);

                causaDeBaja = new CausaDeBaja();
                causaDeBaja.clave = ("7");
                causaDeBaja.descripcion = ("Jubilación");
                listCausaDeBaja.Add(causaDeBaja);

                causaDeBaja = new CausaDeBaja();
                causaDeBaja.clave = ("8");
                causaDeBaja.descripcion = ("Liquidación");
                listCausaDeBaja.Add(causaDeBaja);

                causaDeBaja = new CausaDeBaja();
                causaDeBaja.clave = ("9");
                causaDeBaja.descripcion = ("Otras");
                listCausaDeBaja.Add(causaDeBaja);

                for (i = 0; i < listCausaDeBaja.Count; i++)
                {
                    dbContext.Set<CausaDeBaja>().Add(listCausaDeBaja[i]);
                }

          
                dbContext.SaveChanges();
                #endregion

                //dbContext.SaveChanges();
                //dbContext.Set<TipoHerramienta>().Add(listTipoHerramienta[i]);
                mensaje.resultado = "The database simple was succesful created";
                mensaje.noError = (0);
                mensaje.error = ("");

                dbContext.Database.CurrentTransaction.Commit();
                dbContext.Database.Connection.Close();
            }

            catch (DbEntityValidationException eu)
            {
                foreach (var eve in eu.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("inicializarSimple()1_Error: ").Append(ex));
            }


            return mensaje;
        }


        public Mensaje obtenerConexion(string usuario, string password, string BaseDatosConfig, string BaseDatos, string tipoServidor, string Ubicacionservidor, string puertoServidor, bool cambiarConfiguracion)
        {
            throw new NotImplementedException();
        }

        public Mensaje validaArchivoKey(DbContext dbContext, string rutaArchivoRFC)
        {
            throw new NotImplementedException();
        }

        public string getVersion()
        {
            throw new NotImplementedException();
        }

        //Some other functions convertions
        public static int Digit(char value, int radix)
        {
            if ((radix <= 0) || (radix > 36))
                return -1; // Or throw exception

            if (radix <= 10)
                if (value >= '0' && value < '0' + radix)
                    return value - '0';
                else
                    return -1;
            else if (value >= '0' && value <= '9')
                return value - '0';
            else if (value >= 'a' && value < 'a' + radix - 10)
                return value - 'a' + 10;
            else if (value >= 'A' && value < 'A' + radix - 10)
                return value - 'A' + 10;

            return -1;
        }

        private List<ConceptoPorTipoCorrida> listConcep_tipCorr(ConcepNomDefi c, List<TipoCorrida> ltc)
        {
            List<ConceptoPorTipoCorrida> lc_tc = new List<ConceptoPorTipoCorrida>();

            foreach (TipoCorrida tc in ltc)
            {
                ConceptoPorTipoCorrida c_tc = new ConceptoPorTipoCorrida();
                c_tc.concepNomDefi = (c);
                c_tc.tipoCorrida_ID = (tc.id);
                c_tc.cantidad = (0);
                c_tc.descuentoCreditos = (DescuentoCreditos.DescontarSoloMes);
                c_tc.incluir = (false);
                c_tc.modificarCantidad = (false);
                c_tc.modificarImporte = (false);
                c_tc.mostrar = (false);
                c_tc.opcional = (false);
                lc_tc.Add(c_tc);
            }
            return lc_tc;
        }

        private List<BaseAfecConcepNom> buscarBaseAfecta(List<BaseNomina> listBaseNomina, ConcepNomDefi concepNomDefi)
        {
            List<BaseAfecConcepNom> listBaseAfecta = new List<BaseAfecConcepNom>();
            BaseAfecConcepNom baseAfecConcepNom = new BaseAfecConcepNom();
            baseAfecConcepNom.baseNomina_ID = (listBaseNomina[0].id);//base ISR
            baseAfecConcepNom.concepNomDefin_ID = (concepNomDefi.id);
            baseAfecConcepNom.tipoAfecta = (0);
            listBaseAfecta.Add(baseAfecConcepNom);
            return listBaseAfecta;
        }

        //private String buscaRutaArchivos(String ruta, String pathBuscar)
        //{
        //    File file = new File(ruta);
        //    String path = "";

        //    if (File.Exists(ruta))
        //    {
        //        Path root = file.toPath();
        //        if (root.getRoot().toString().equalsIgnoreCase(ruta))
        //        {
        //            ruta = ruta.replace(System.getProperty("file.separator"), "").concat(pathBuscar);
        //        }
        //        File config = new File(ruta.concat(pathBuscar));
        //        if (config.exists())
        //        {
        //            return config.getAbsolutePath();
        //        }
        //        else {
        //            if (file.getParentFile() != null)
        //            {
        //                path = buscaRutaArchivos(file.getParentFile().getAbsolutePath(), pathBuscar);
        //            }
        //        }
        //    }
        //    return path;
        //}

    }
}
