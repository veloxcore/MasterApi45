using MasterApi45.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Data.Infrastracture
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IUnitOfWork _unitOfWork;

        public DbSet<TEntity> dbSet;
        private DbContext DatabaseContext
        {
            get { return ((UnitOfWork)_unitOfWork).DatabaseContext; }
        }

        public Repository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }
            _unitOfWork = unitOfWork;
            this.dbSet = DatabaseContext.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return dbSet.ToList();
        }

        public TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            dbSet.Remove(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (DatabaseContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public void Update(TEntity entityToUpdate)
        {
            var entry = DatabaseContext.Entry<TEntity>(entityToUpdate);

            if (entry.State == EntityState.Detached)
            {
                TEntity attachedEntity = dbSet.Find(GetKey(entityToUpdate));

                // You need to have access to key
                if ((attachedEntity != null))
                {
                    dynamic attachedEntry = DatabaseContext.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entityToUpdate);
                }
                else
                {
                    // This should attach entity
                    entry.State = EntityState.Modified;
                }
            }
        }

        public void SaveChanges()
        {
            try
            {
                DatabaseContext.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Private Methods
        protected abstract object[] GetKey(TEntity enity);
        #endregion
    }
}
