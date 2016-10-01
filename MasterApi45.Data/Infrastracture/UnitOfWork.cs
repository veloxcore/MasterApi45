using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Data.Infrastracture
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContextTransaction _transaction;
        /// <summary>
        /// The readonly keyword is used to declare a member variable a constant, but allows the value to be calculated at runtime.
        /// This differs from a constant declared with the const modifier, which must have its value set at compile time.
        /// Using readonly you can set the value of the field either in the declaration, or in the constructor of the object that the field is a member of.
        /// Also use it if you don't want to have to recompile external DLLs that reference the constant (since it gets replaced at compile time).
        /// </summary>
        private readonly MasterApi45Context _master45Context;
        public DbContext DatabaseContext
        {
            get { return _master45Context; }
        }

        public UnitOfWork(MasterApi45Context master45Context)
        {
            _master45Context = master45Context;
        }

        #region Public Methods
        public void CommitTransaction()
        {
            SaveChanges();
            _transaction.Commit();
            _transaction.Dispose();
        }
        public void StartTransaction()
        {
            _transaction = DatabaseContext.Database.BeginTransaction();
        }
        public void RollBackTransaction()
        {
            _transaction.Rollback();
        }
        public void SaveChanges()
        {
            _master45Context.SaveChanges();
        }
        #endregion

        #region Dispose
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    DatabaseContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
