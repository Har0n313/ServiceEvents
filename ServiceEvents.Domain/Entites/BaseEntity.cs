namespace ServiceEvents.Domain.Entites
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity entity)
                return false;

            return Id == entity.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
