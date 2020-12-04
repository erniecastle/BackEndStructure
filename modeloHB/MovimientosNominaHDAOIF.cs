using Exitosw.Payroll.Hibernate.entidad;
using Exitosw.Payroll.Hibernate.modelo;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exitosw.Payroll.Core.modeloHB
{
    public interface MovimientosNominaHDAOIF : IRepository<MovNomConcep>
    {
        Mensaje getMovimientosNominaAll(ISession uuidCxn);

        Mensaje getMovimientosNominaAsc(ISession uuidCxn);

        Mensaje getMaxNumeroMovimientoPorTipoNominaYPeriodo(String claveTipoNomina, decimal idPeriodo, ISession uuidCxn);

        Mensaje saveDeleteMovimientosNomina(List<MovNomConcep> AgreModif, Object[] clavesDelete, Object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, ISession uuidCxn);

        Mensaje getMovimientosPorPlazaEmpleado(Object[] clavesPlazasPorEmpleado, String claveTipoCorrida, String claveRazonSocial, ISession uuidCxn);

        Mensaje eliminaListaMovimientos(String campo, Object[] valores, List<CFDIEmpleado> valoresCFDI, Object[] valoresCalculoUnidades, Object[] valoresReestablecer, bool incluirEliminadoDiferenteTipoPantalla100, ISession uuidCxn);

        Mensaje getMovimientosPorFiltro(String[] camposWhere, Object[] valoresWhere, ISession uuidCxn);

        Mensaje getMovimientosPorFiltroEspecifico(String[] camposWhere, Object[] valoresWhere, ISession uuidCxn);

        Mensaje getCalculosUnidadesPorFiltroEspecifico(String[] camposWhere, Object[] valoresWhere, List<CFDIEmpleado> listCFDIEmpleado, ISession uuidCxn);

        Mensaje buscaMovimientosNominaFiltrado(List<Object> valoresDeFiltrado, ISession uuidCxn);
    }
}
