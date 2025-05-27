using TradeSimulator.Backend.Models;

namespace TradeSimulator.Backend.Repositories
{
    public class RepositoryBase<TEntity> where TEntity : IEntity<string>
    {
        protected List<TEntity> Entities;



        /* ------------------------------------------------------------ */

        public virtual TEntity GetById(string id)
        {
            if (Entities == null || Entities.Count == 0)
                return default;

            return Entities.FirstOrDefault(entity => entity.Id == id);
        }

        public virtual List<TEntity> GetAll(string id)
        {
            return new List<TEntity>(Entities);
        }

        public virtual void Insert(TEntity entity)
        {
            if (EqualityComparer<TEntity>.Default.Equals(entity, default))
                return;

            if (Entities == null)
                Entities = new();

            Entities.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            if (EqualityComparer<TEntity>.Default.Equals(entity, default))
                return;

            var cachedEntity = GetById(entity.Id);

            if (!EqualityComparer<TEntity>.Default.Equals(cachedEntity, default))
                Delete(cachedEntity.Id);

            Insert(entity);
        }

        public virtual void Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            var cachedEntity = GetById(id);

            if (EqualityComparer<TEntity>.Default.Equals(cachedEntity, default))
                return;

            Entities.Remove(cachedEntity);
        }

    }
}
