using System.Reflection;
namespace Tracer.Core.Serialization;

public sealed class SerializeLoader
{
  public void Serialize(TraceResult result, string fileName)
  {
    var asm = Assembly.LoadFrom("./bin/Debug/net8.0/Core.dll");
    var type = typeof(ITraceResultSerializer);
    var types = asm.GetTypes()
    .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
    foreach(var tSerializer in types)
    {
      ITraceResultSerializer? serializer = (ITraceResultSerializer?)Activator.CreateInstance(tSerializer); 
      var method = tSerializer.GetMethod("Serialize");
      method?.Invoke(serializer, new object[] {result, fileName});
    }
  }
}
