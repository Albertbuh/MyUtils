namespace Core.Exceptions;

public class DependencyImplementationException: Exception
{
  public DependencyImplementationException() { }

  public DependencyImplementationException(string message)
    : base(message) { }

  public DependencyImplementationException(string message, Exception e)
    : base(message, e) { } 
}
