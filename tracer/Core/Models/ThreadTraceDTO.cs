namespace Tracer.Core.Models;

public struct ThreadTraceDTO
{
  internal readonly int Id;
  internal readonly long Time;
  internal IEnumerable<Queue<MethodInfo>> Methods;

  internal ThreadTraceDTO(int id, long time, IEnumerable<Queue<MethodInfo>> methods)
  {
    Id = id;
    Time = time;
    Methods = methods;
  }

  public override string ToString()
  {
    var sb = new System.Text.StringBuilder();
    sb.AppendLine($"Thread: {Id}");
    sb.AppendLine($"Time: {Time}");
    foreach(var q in Methods)
    {
      System.Console.WriteLine(q.Count);
      var depth = 0;
      while(q.Count != 0)
      {
        var cur = q.Dequeue();
        foreach(var child in cur.ChildMethods)
          q.Enqueue(child);
        
        sb.Append($"Tracing for method {cur.Name} (thread: {this.Id}) (depth: {depth}) ");
        sb.Append($"of class {cur.ClassName}. ");
        sb.Append($"Execution time - {cur.TimeString}");
        sb.AppendLine();
        if(cur.ChildMethods.Any())
          depth++;
      }
    }
    return sb.ToString();
  }
}
