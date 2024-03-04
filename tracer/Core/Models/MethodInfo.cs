namespace Tracer.Core.Models;

public struct MethodInfo
{
	public string Name {get; private set;}
  public string ClassName {get; private set;}
  private long _time {get; set;}
  public string Time => $"{_time}ms";

  [JsonProperty(Order = 4)]
  [YamlMember(Order = 4)]
  public IReadOnlyCollection<MethodInfo> ChildMethods = new Queue<MethodInfo>();
  
  public MethodInfo(string name, string className, long time)
  {
    Name = name;
    ClassName = className;
    _time = time;
  }

}
