namespace Tracer.Core.Serialization;

public class YamlSerializer : ITraceResultSerializer
{
  public string Format => "yaml";
  ISerializer serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

  public void Serialize(TraceResult traceResult, string path)
  {
    if(!path.Contains('.'))
      path = $"{path}.{Format}";
    using(StreamWriter writer = new StreamWriter(path))
    {
      writer.Write(serializer.Serialize(traceResult));
    }
  }
}
