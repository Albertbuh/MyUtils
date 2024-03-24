namespace Core.Models;

public class AssemblyModel : IBrowserModel
{
	private Assembly assembly;
	private HashSet<NamespaceModel> members;
	public List<NamespaceModel> Members => members.ToList();
	public string Name
	{
		get
		{
            var name = assembly.FullName ?? "";
            return $"{name.Substring(0, name.IndexOf(","))} DLL";
        }
    }

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
		sb.AppendLine(this.Name);
		foreach (var member in members)
			sb.Append($"  {member}");

		return sb.ToString();
	}
}
