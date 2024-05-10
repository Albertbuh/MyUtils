namespace Tracer.Core.Models;

public struct MethodInfo
{
    [JsonProperty(Order = 1)]
    [YamlMember(Order = 1)]
    public string ClassName { get; private set; }
    
    [JsonProperty(Order = 2)]
    [YamlMember(Order = 2)]
    public string Name { get; private set; }

    private long _time { get; set; }
    [JsonProperty(Order = 3)]
    [YamlMember(Order = 4)]
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
