

using Exitosw.Payroll.Hibernate.entidad;
/**
* @author: Ernesto Castillo 
* Fecha de Creación: 11/04/2019
* Compañía: Macropro
* Descripción del programa: Intereface para llamados a metodos de Hibernate request
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

namespace Exitosw.Payroll.Hibernate.modelo
{
    public interface GenericQueriesDAOIF : IRepository<Object>
    {
        Mensaje consultaPorFiltros(string fuentePrincipal, object[] tablasRelacionadas, object[] camposMostrar, object[] camposWhere, object[] valoresWhere, object[] camposGroup, object[] camposOrden, bool withCount, int? inicio, int? fin, bool activarAlias, string tipoOrden, ISession session);
    }
}
