namespace Core.Exceptions;

public class DependencyConfigurationException: Exception
{
  public DependencyConfigurationException() { }

  public DependencyConfigurationException(string message)
    : base(message) { }

  public DependencyConfigurationException(string message, Exception e)
    : base(message, e) { } 
}
