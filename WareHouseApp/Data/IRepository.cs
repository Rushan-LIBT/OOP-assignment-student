using System.Collections.Generic;

namespace WareHouseApp.Data
{
    /// <summary>
    /// Generic CRUD contract every repository fulfils (interface + generics for OOP).
    /// </summary>
    public interface IRepository<T>
    {
        List<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }

    /// <summary>
    /// Shared base for repositories. Holds the <see cref="DatabaseManager"/>
    /// so concrete repositories only implement their own SQL.
    /// </summary>
    public abstract class BaseRepository<T> : IRepository<T>
    {
        protected readonly DatabaseManager Db = DatabaseManager.Instance;

        public abstract List<T> GetAll();
        public abstract T GetById(int id);
        public abstract void Add(T entity);
        public abstract void Update(T entity);
        public abstract void Delete(int id);
    }
}
