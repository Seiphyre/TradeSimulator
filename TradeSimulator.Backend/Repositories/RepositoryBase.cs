using TradeSimulator.Shared.Models;

namespace TradeSimulator.Backend.Repositories
{
    public class RepositoryBase<TEntity> where TEntity : IEntity<string>
    {
        protected List<TEntity> Entities = new ();



        /* ------------------------------------------------------------ */

        public virtual TEntity GetById(string id)
        {
            if (Entities == null || Entities.Count == 0)
                return default;

            return Entities.FirstOrDefault(entity => entity.Id == id);
        }

        public virtual List<TEntity> GetAll()
        {
            if (Entities == null || Entities.Count == 0)
                return new ();

            return new List<TEntity>(Entities);
        }

        public virtual TEntity Create(TEntity entity)
        {
            if (EqualityComparer<TEntity>.Default.Equals(entity, default))
                return default;

            if (Entities == null)
                Entities = new();

            Entities.Add(entity);

            return entity;
        }

        public virtual void Update(TEntity entity)
        {
            if (EqualityComparer<TEntity>.Default.Equals(entity, default))
                return;

            var cachedEntity = GetById(entity.Id);

            if (!EqualityComparer<TEntity>.Default.Equals(cachedEntity, default))
                Delete(cachedEntity.Id);

            Create(entity);
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
