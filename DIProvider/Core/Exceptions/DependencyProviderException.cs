namespace Core.Exceptions;

public class DependencyProviderException: Exception
{
  public DependencyProviderException() { }

  public DependencyProviderException(string message)
    : base(message) { }

  public DependencyProviderException(string message, Exception e)
    : base(message, e) { } 
}
