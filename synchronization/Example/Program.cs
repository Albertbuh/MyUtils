using System.Reflection;

var assembly = Assembly.LoadFile("/home/albertbuh/projects/utils/synchronization/Core/bin/Debug/net8.0/Core.dll");
var types = assembly.GetTypes().Where(t => t.IsPublic).OrderBy(t => t.Name).OrderBy(t => t.Namespace);
foreach(var t in types)
{
    Console.WriteLine(t);
}
