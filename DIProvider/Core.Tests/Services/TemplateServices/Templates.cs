namespace Core.Tests.Services.Templates;

public interface IRepository
{
    public int GetSomeNumber();
}

public class SomeRepository : IRepository
{
    public int GetSomeNumber()
    {
        return 1;
    }
}

interface IService<TRepository>
    where TRepository : IRepository
{ 
    public int GetSomeNumber();
}

class ServiceImpl<TRepository> : IService<TRepository>
    where TRepository : IRepository
{
    IRepository? repository;

    public ServiceImpl() { }

    public ServiceImpl(TRepository repository)
    {
        this.repository = repository;
    }

    public int GetSomeNumber()
    {
        if(repository != null)
            return 1 + repository.GetSomeNumber();
        return 0;
    }
}
