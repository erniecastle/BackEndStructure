/**
* @author: Ernesto Castillo
* Fecha de Creación: 17/01/2018
* Compañía: Macropro
* Descripción del programa: Entidad para llamados a eventos genericos
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
using Exitosw.Payroll.Core.servicios.extras;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Exitosw.Payroll.Entity.util;
using Exitosw.Payroll.Core.genericos.campos;
using Exitosw.Payroll.Core.campos;
using Exitosw.Payroll.Core.util;

namespace Exitosw.Payroll.Core.modelo
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class
    {

        private DbContext dbContext;
        public Mensaje mensajeResultado = new Mensaje();
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public StringBuilder consulta;
        public ConectorQuerysGenericos conectorQuerysGenericos = new ConectorQuerysGenericos();
        private string nameProject = ConfigurationManager.AppSettings["routeEntitiesEF"];

        public void setSession(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DbContext getSession()
        {
            if (dbContext == null)
            {
               // Console.WriteLine("El contexto de conexión no ha sido inicializada en el DAO antes de usarse.");
               //System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("actualizar()1_Error: ").Append(ex));

            }
            return this.dbContext;
        }

        public Task Create(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(decimal id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetById(decimal id)
        {
            throw new NotImplementedException();
        }

        public Task Update(decimal id, TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Mensaje obtenerIdMax(DBContextAdapter dbContext)
        {
            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(String.Concat(typeof(TEntity).Name, ".id"), TipoFuncion.MAXIMO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.SELECT,
                typeof(TEntity).Name, select, null, null, null, null, null);

            return mensajeResultado;
        }

        public Mensaje obtenerClaveMax(DBContextAdapter dbContext)
        {
            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(String.Concat(typeof(TEntity).Name, ".clave"), TipoFuncion.MAXIMO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.SELECT,
                typeof(TEntity).Name, select, null, null, null, null, null);

            return mensajeResultado;
        }

        //string
        public Mensaje getClaveStringMax(string tabla, List<CamposWhere> camposWhere, string campoBuscar, DBContextAdapter dbContext)
        {
            if (String.IsNullOrEmpty(tabla))
            {
                tabla = typeof(TEntity).Name;
            }
            CamposSelect campoSelect;
            if ((String.IsNullOrEmpty(campoBuscar)))
            {
                campoSelect = new CamposSelect(String.Concat(tabla, ".clave"), TipoFuncion.MAXIMO);
            }
            else
            {
                campoSelect = new CamposSelect(campoBuscar, TipoFuncion.MAXIMO);
            }
            List<CamposSelect> camposSelect = new List<CamposSelect>() { campoSelect };
            OperadorSelect select = new OperadorSelect(camposSelect);
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);

            return mensajeResultado;
        }

        public Mensaje obtenerClaveStringMax(string tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext)
        {
            return getClaveStringMax(tabla, camposWhere, null, dbContext);
        }

        public Mensaje obtenerClaveStringMax(List<CamposWhere> camposWhere, DBContextAdapter dbContext)
        {
            return getClaveStringMax(null, camposWhere, null, dbContext);
        }

        public Mensaje obtenerClaveStringMax(string tabla, List<CamposWhere> camposWhere, string campoBuscar, DBContextAdapter dbContext)
        {
            return getClaveStringMax(tabla, camposWhere, campoBuscar, dbContext);
        }

        //////public List<object> selectIdNombreEntidad()
        //////{
        //////    throw new NotImplementedException();  ///sin uso
        //////}

        // List<object>
        public Mensaje consultaPorFiltros(string tabla, List<CamposWhere> camposWhere, ValoresRango valoresRango, DBContextAdapter dbContext)
        {
            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, valoresRango);
            return mensajeResultado;
        }

        // List<object>
        public Mensaje consultaPorRangos(ValoresRango valoresRango, List<CamposWhere> camposWhere, DBContextAdapter dbContext)
        {
            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect((typeof(TEntity)).Name, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Lista, TipoOperacion.SELECT,
                (typeof(TEntity)).Name, select, null, camposWhere, null, null, valoresRango);
            return mensajeResultado;
        }

        //bool
        public Mensaje existeDato(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.CONTAR) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            List<CamposWhere> listaCamposWhere = new List<CamposWhere>();
            if (campoWhere != null)
            {
                listaCamposWhere.Add(campoWhere);
            }
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.SELECT,
                 tabla, select, null, listaCamposWhere, null, null, null);
            if (mensajeResultado.noError == 0)
            {
                if ((int)mensajeResultado.resultado > 0)
                {
                    mensajeResultado.resultado = true;
                }
                else
                {
                    mensajeResultado.resultado = false;
                }
            }
            return mensajeResultado;
        }

        //object
        public Mensaje existeClave(string tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext)
        {
            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);
            return mensajeResultado;
        }

        // List<object>
        public Mensaje existenClaves(string tabla, List<CamposWhere> camposWhere, DBContextAdapter dbContext)
        {
            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.NINGUNO) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Lista, TipoOperacion.SELECT,
                tabla, select, null, camposWhere, null, null, null);
            return mensajeResultado;
        }

        //bool
        public Mensaje existeClave(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            List<CamposSelect> camposSelect = new List<CamposSelect>() { new CamposSelect(tabla, TipoFuncion.CONTAR) };
            OperadorSelect select = new OperadorSelect(camposSelect);
            List<CamposWhere> listaCamposWhere = new List<CamposWhere>();
            if (campoWhere != null)
            {
                listaCamposWhere.Add(campoWhere);
            }
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.SELECT,
                 tabla, select, null, listaCamposWhere, null, null, null);
            if (mensajeResultado.noError == 0)
            {
                if ((int)mensajeResultado.resultado > 0)
                {
                    mensajeResultado.resultado = true;
                }
                else
                {
                    mensajeResultado.resultado = false;
                }
            }
            return mensajeResultado;
        }

        public Mensaje deleteListQuery(string tabla, CamposWhere campoWhere, DBContextAdapter dbContext)
        {
            List<CamposWhere> listaCamposWhere = new List<CamposWhere>();
            if (campoWhere != null)
            {
                listaCamposWhere.Add(campoWhere);
            }
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.DELETE,
                 tabla, null, null, listaCamposWhere, null, null, null);
            if (mensajeResultado.noError == 0)
            {
                mensajeResultado.resultado = true;
            }
            return mensajeResultado;

        }

        public Mensaje deleteListQueryConFiltrado(string tabla, List<CamposWhere> listaCamposWhere, DBContextAdapter dbContext)
        {
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.DELETE,
                 tabla, null, null, listaCamposWhere, null, null, null);
            if (mensajeResultado.noError == 0)
            {
            }
            return mensajeResultado;
        }

        public Mensaje updateListQuery(string tabla, List<CamposSelect> listaCamposActualizar, List<CamposWhere> listaCamposWhere, DBContextAdapter dbContext)
        {
            OperadorSelect select = new OperadorSelect(listaCamposActualizar);
            mensajeResultado = conectorQuerysGenericos.consultaGenerica(dbContext, TipoResultado.Unico, TipoOperacion.UPDATE,
                 tabla, select, null, listaCamposWhere, null, null, null);
            if (mensajeResultado.noError == 0)
            {
            }
            return mensajeResultado;
        }

        public void inicializaVariableMensaje()
        {
            if (mensajeResultado == null)
            {
                mensajeResultado = new Mensaje();

            }
            mensajeResultado.resultado = null;
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";
        }

        public Boolean existeTablaDBContextMaster(String tabla)
        {
            bool existeTabla = false;
            DBContextMaster dbContextSimple = new DBContextMaster();
            IObjectContextAdapter adapter = dbContextSimple as IObjectContextAdapter;
            ObjectContext objectContext = adapter.ObjectContext;
            ReadOnlyCollection<EntityType> allTypes = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace);
            foreach (EntityType item in allTypes)
            {
                if (tabla.Equals(item.Name))
                {
                    existeTabla = true;
                    break;
                }
            }
            return existeTabla;
        }

        public DateTime getFechaDelSistema()
        {
            return DateTime.Now;
        }

        public object castTiposDatos(Type tipo, object valor)
        {
            object valor2 = null;
            try
            {
                if (tipo == typeof(Decimal?) || tipo == typeof(Decimal) || tipo == typeof(decimal?) || tipo == typeof(decimal))
                {
                    if (!valor.ToString().Equals(""))
                    {
                        valor2 = Convert.ToDecimal(valor);
                    }

                }
                else if (tipo == typeof(Double?) || tipo == typeof(Double) || tipo == typeof(double?) || tipo == typeof(double))
                {
                    valor2 = Convert.ToDouble(valor);
                }
                else if (tipo == typeof(int?) || tipo == typeof(int))
                {
                    valor2 = int.Parse(valor.ToString());
                }
                else if (tipo == typeof(Boolean?) || tipo == typeof(Boolean) || tipo == typeof(bool?) || tipo == typeof(bool))
                {
                    valor2 = Convert.ToBoolean(valor);
                }
                else if (tipo == typeof(short?) || tipo == typeof(short))
                {
                    valor2 = short.Parse(valor.ToString());
                }
                else if (tipo == typeof(Int16?) || tipo == typeof(Int16))
                {
                    valor2 = Convert.ToInt16(valor);
                }
                else if (tipo == typeof(Int32?) || tipo == typeof(Int32))
                {
                    valor2 = Convert.ToInt32(valor);
                }
                else if (tipo == typeof(Int64?) || tipo == typeof(Int64))
                {
                    valor2 = Convert.ToInt64(valor);
                }
                else if (tipo == typeof(long?) || tipo == typeof(long))
                {
                    valor2 = long.Parse(valor.ToString());
                }
                else if (tipo == typeof(String) || tipo == typeof(string))
                {
                    valor2 = valor.ToString();
                }
                else
                {

                    valor2 = valor;
                }
            }
            catch (Exception ex)
            {
                valor2 = valor;
            }
            return valor2;
        }
        public object creaInstanciaDao(String nombreDao)
        {
            object instancia = null;
            try
            {
                Type calcType = Type.GetType("Exitosw.Payroll.Core.modelo." + nombreDao);
                // create instance of class Calculator
                instancia = Activator.CreateInstance(calcType);
            }
            catch (Exception)
            {
                throw;
            }

            return instancia;
        }
        public object creaInstancia(string tabla)
        {
            object instacia = null;
            try
            {
                Type calcType = Type.GetType(nameProject + ".entidad." + tabla);
                instacia = Activator.CreateInstance(calcType);
            }
            catch (Exception)
            {
                throw;
            }
            return instacia;
        }

        public object crearobjeto(Dictionary<string, object> data)
        {
            string nametable = "";
            nametable = data["Tabla"].ToString();
            data.Remove("Tabla");
            object entidad = creaInstancia(nametable);

            Type claseInstancia = entidad.GetType();

            foreach (var item in data)
            {
                MemberInfo info = claseInstancia.GetField(item.Key) as MemberInfo ??
                    claseInstancia.GetProperty(item.Key) as MemberInfo;
                if (info != null)
                {
                    switch (info.MemberType)
                    {

                        case MemberTypes.Field:
                            ((FieldInfo)info).SetValue(entidad, item.Value);
                            break;
                        case MemberTypes.Property:
                            ((PropertyInfo)info).SetValue(entidad, castTiposDatos(((PropertyInfo)info).PropertyType, item.Value), null);
                            break;
                        default:
                            throw new ArgumentException("MemberInfo must be if type FieldInfo or PropertyInfo", "member");
                    }
                }
            }
            return entidad;
        }
    }
}