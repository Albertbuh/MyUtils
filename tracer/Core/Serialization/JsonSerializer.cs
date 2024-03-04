namespace Tracer.Core.Serialization;

public class JsonSerializer : ITraceResultSerializer
{
  public string Format => "json";
  Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

  public void Serialize(TraceResult traceResult, string path)
  {
    if(!path.Contains('.'))
      path = $"{path}.{Format}";
    
    using(StreamWriter stream = new StreamWriter(path))
    using(JsonWriter writer = new JsonTextWriter(stream))
    {
      writer.Formatting = Formatting.Indented;
      serializer.Serialize(writer, traceResult);
    }  
  }
}
