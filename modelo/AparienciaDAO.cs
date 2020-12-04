using Exitosw.Payroll.Entity.entidad;
using System;
using System.Text;
using Exitosw.Payroll.Core.util;
using System.Reflection;
using System.Data.Entity.Migrations;
using System.Linq;
using Newtonsoft.Json;
using System.Data.Entity.Validation;

namespace Exitosw.Payroll.Core.modelo
{
    public class AparienciaDAO : GenericRepository<Apariencia>, AparienciaDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        public Mensaje agregar(Apariencia entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Apariencia>().Add(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = entity;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("agregar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje modificar(Apariencia entity, string keyUser, int idRazonSocial, DBContextAdapter dbContextMaster, DBContextAdapter dbContextSimple)
        {
            var dbMaster = dbContextMaster.context;
            var dbSimple = dbContextSimple.context;
            try
            {
                inicializaVariableMensaje();
                dbMaster.Database.BeginTransaction();
                dbSimple.Database.BeginTransaction();
                dbMaster.Set<Apariencia>().AddOrUpdate(entity);
                //1 Usuario 2 empresa 3 Sistema
                var aparienciaCnf = new { tema = entity.temaTransient, iconos = entity.iconosTransient };
                var aparienciaJson = JsonConvert.SerializeObject(aparienciaCnf);

                int key = Int32.Parse(keyUser);
                Usuario user = dbMaster.Set<Usuario>().FirstOrDefault(u => u.id == key);
                if (entity.controlApariencia == 1)
                {
                    user.apariencia = aparienciaJson;
                }
                else
                {
                    user.apariencia = null;
                }

                dbMaster.Set<Usuario>().Attach(user);
                dbMaster.Entry(user).Property(x => x.apariencia).IsModified = true;
                RazonesSociales razonesSociales = dbSimple.Set<RazonesSociales>().FirstOrDefault(r => r.id == idRazonSocial);

                if (entity.controlApariencia == 2)
                {
                    razonesSociales.apariencia = aparienciaJson;
                }
                else
                {
                    razonesSociales.apariencia = null;
                }

                dbSimple.Set<RazonesSociales>().Attach(razonesSociales);
                dbSimple.Entry(razonesSociales).Property(x => x.apariencia).IsModified = true;

                dbMaster.SaveChanges();
                dbSimple.SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                dbMaster.Database.CurrentTransaction.Commit();
                dbMaster.Database.Connection.Close();
                dbSimple.Database.CurrentTransaction.Commit();
                dbSimple.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("actualizar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbMaster.Database.CurrentTransaction.Rollback();
                dbSimple.Database.CurrentTransaction.Rollback();
            }

            return mensajeResultado;
        }

        public Mensaje eliminar(Apariencia entity, DBContextAdapter dbContext)
        {
            try
            {
                inicializaVariableMensaje();
                setSession(dbContext.context);
                getSession().Database.BeginTransaction();
                getSession().Set<Apariencia>().Attach(entity);
                getSession().Set<Apariencia>().Remove(entity);
                getSession().SaveChanges();
                mensajeResultado.resultado = true;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                getSession().Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("eliminar()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                getSession().Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

        public Mensaje getActualApariencia(string keyUser, int idRazonSocial, DBContextAdapter dbContextMaster, DBContextAdapter dbContextSimple)
        {

            var dbMaster = dbContextMaster.context;
            var dbSimple = dbContextSimple.context;
            try
            {
                inicializaVariableMensaje();
                dbMaster.Database.BeginTransaction();
                dbSimple.Database.BeginTransaction();



                var configuracionApariencia = (from apa in dbMaster.Set<Apariencia>()
                                               select new
                                               {
                                                   apa.id,
                                                   apa.controlApariencia,
                                                   apa.tema,
                                                   apa.iconos,
                                                   apa.permiteUsuarioTema,
                                                   apa.permiteUsuarioIconos,
                                                   apa.mostrarBordes
                                               }
                                         ).SingleOrDefault();

                int key = Int32.Parse(keyUser);
                Usuario user = dbMaster.Set<Usuario>().FirstOrDefault(u => u.id == key);
                RazonesSociales razonesSociales = dbSimple.Set<RazonesSociales>().FirstOrDefault(r => r.id == idRazonSocial);
                var cnfApa = new { Apariencia = configuracionApariencia, Usuario = user.apariencia, RazonesSociales = razonesSociales.apariencia };

                mensajeResultado.resultado = cnfApa;
                mensajeResultado.noError = 0;
                mensajeResultado.error = "";
                dbMaster.Database.CurrentTransaction.Commit();
                dbMaster.Database.Connection.Close();
                dbSimple.Database.CurrentTransaction.Commit();
                dbSimple.Database.Connection.Close();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getActualApariencia()1_Error: ").Append(ex));
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
                dbMaster.Database.CurrentTransaction.Rollback();
                dbSimple.Database.CurrentTransaction.Rollback();
            }
            return mensajeResultado;
        }

    }
}
