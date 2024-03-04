namespace Tracer.Core.Serialization;

public class XmlSerializer : ITraceResultSerializer
{
  public string Format => "xml";
  Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
  
  public void Serialize(TraceResult traceResult, string path)
  {
    if(!path.Contains('.'))
      path = $"{path}.{Format}";

    var json = JsonConvert.SerializeObject(traceResult);
    var node = JsonConvert.DeserializeXNode(json, "root");
    using(StreamWriter stream = new StreamWriter(path))
    {
      stream.Write(node?.ToString());
    }
  }
}
