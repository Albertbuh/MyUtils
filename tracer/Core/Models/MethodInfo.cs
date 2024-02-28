namespace Tracer.Core.Models;

internal struct MethodInfo
{
	internal string Name {get; set;}
  internal string ClassName {get; set;}
  private long Time {get; set;}
  internal string TimeString => $"{Time}ms";

  internal IEnumerable<MethodInfo> ChildMethods = new Queue<MethodInfo>();

  internal MethodInfo(string name, string className, long time)
  {
    Name = name;
    ClassName = className;
    Time = time;
  }

}
