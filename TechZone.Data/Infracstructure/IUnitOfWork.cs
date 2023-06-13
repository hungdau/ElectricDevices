namespace TechZone.Data.Infracstructure
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}