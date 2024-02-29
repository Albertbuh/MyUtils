namespace Tracer.Core.Models;

public struct ThreadTraceDTO
{
  public readonly int Id;
  public readonly string Time;
  public IEnumerable<Queue<MethodInfo>> Methods;

  internal ThreadTraceDTO(int id, long time, IEnumerable<Queue<MethodInfo>> methods)
  {
    Id = id;
    Time = $"{time}ms";
    Methods = methods;
  }

  public override string ToString()
  {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine($"Thread: {Id}");
    sb.AppendLine($"Time: {Time}");
    foreach(var queue in Methods)
    {
      var q = new Queue<MethodInfo>(queue);
      var depth = 0;
      while(q.Count != 0)
      {
        var cur = q.Dequeue();
        foreach(var child in cur.ChildMethods)
          q.Enqueue(child);
        
        sb.Append($"Tracing for method {cur.Name} (thread: {this.Id}) (depth: {depth}) ");
        sb.Append($"of class {cur.ClassName}. ");
        sb.Append($"Execution time - {cur.Time}");
        sb.AppendLine();
        if(cur.ChildMethods.Any())
          depth++;
      }
    }
    return sb.ToString();
  }
}
