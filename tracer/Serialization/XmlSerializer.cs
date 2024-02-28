namespace Tracer.Serialization;

public class XmlSerializer : ITraceResultSerializer
{
  public string Format => "xml";
  Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
  
  public void Serialize(TraceResult traceResult, string path)
  {
    if(!path.Contains('.'))
      path = $"{path}.{Format}";

    var json = JsonLayer(traceResult);
    var node = JsonConvert.DeserializeXNode(json, "root");
    using(StreamWriter stream = new StreamWriter(path))
    {
      stream.Write(node?.ToString());
    }
  }

  private string JsonLayer(TraceResult traceResult)
  {
    const string jsonPath = @"fasjflasjf.json";
    using(StreamWriter stream = new StreamWriter(jsonPath))
    using(JsonWriter writer = new JsonTextWriter(stream))
    {
      serializer.Serialize(writer, traceResult);
    }
    string jsonText = File.ReadAllText(jsonPath);
    Task.Run(() => File.Delete(jsonPath));
    return jsonText;
  }
}
