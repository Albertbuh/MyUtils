namespace Tracer.Core.Serialization;

public interface ITraceResultSerializer
{
  string Format {get;} 
  void Serialize(TraceResult traceResult, string path);
}
