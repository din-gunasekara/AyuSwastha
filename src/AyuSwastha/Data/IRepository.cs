using System.Collections.Generic;

namespace AyuSwastha.Data
{
    /// <summary>
    /// Generic CRUD contract implemented by every repository — demonstrates
    /// <b>interfaces</b> and <b>generics</b> decoupling the UI from data access.
    /// </summary>
    public interface IRepository<T>
    {
        IReadOnlyList<T> GetAll();
        T GetById(int id);
        int Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
