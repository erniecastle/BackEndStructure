using Exitosw.Payroll.Entity.entidad;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Exitosw.Payroll.Core.servicios.extras
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T>
        where T : class, IEntityBase, new()
    {

        private DbContext  _context;
        public Mensaje mensajeResultado = new Mensaje();

        public void setSession(DbContext dbContext)
        {
            this._context = dbContext;
        }

        public DbContext getSession()
        {
            if (_context == null)
            {
                Console.WriteLine("El contexto de conexión no ha sido inicializada en el DAO antes de usarse.");

            }
            return this._context;
        }

        public EntityBaseRepository()
        {
        }
        #region Properties
        public EntityBaseRepository(DbContext context)
        {
            _context = context;
        }
        #endregion
        public virtual IEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsEnumerable();
        }

        public virtual int Count()
        {
            return _context.Set<T>().Count();
        }

        public virtual IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.AsEnumerable();
        }

        public T GetSingle(decimal id)
        {
            return _context.Set<T>().FirstOrDefault(x => x.id == id);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.Where(predicate).FirstOrDefault();
        }

        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public virtual void Add(T entity)
        {
           // EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }

        public virtual void Update(T entity)
        {
            _context.Set<T>().Add(entity);  ///pendiente
            ////EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            ////dbEntityEntry.State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Set<T>().Remove(entity);
            //EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            //dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }
        }

        public decimal obtenerIdMax(T entity)
        {
            
            var maxId = _context.Set<T>().Max(e => e.id);
            return maxId;
        }

        public virtual void Commit()
        {
            _context.SaveChanges();
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
    }
}
