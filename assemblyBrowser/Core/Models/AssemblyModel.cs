namespace Core.Models;

public class AssemblyModel : IBrowserModel
{
	private Assembly assembly;
	private HashSet<NamespaceModel> members;

	public AssemblyModel(Assembly assembly)
	{
		this.assembly = assembly;
		members = FillMembers(assembly);
	}

	private HashSet<NamespaceModel> FillMembers(Assembly assembly)
	{
		var types = assembly.GetTypes();
		var dict = new Dictionary<string, List<Type>>();
		foreach (var type in types)
		{
			if (type.Namespace != null)
			{
				if (!dict.ContainsKey(type.Namespace))
					dict.Add(type.Namespace, new List<Type>());

				dict[type.Namespace].Add(type);
			}
		}
		return dict.Select(p => new NamespaceModel(p.Key, p.Value)).ToHashSet();
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		var name = assembly.FullName ?? "";
		sb.AppendLine($"{name.Substring(0, name.IndexOf(","))} DLL");
		foreach (var member in members)
			sb.Append($"  {member}");

		return sb.ToString();
	}
}
