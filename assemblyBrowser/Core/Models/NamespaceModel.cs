namespace Core.Models;

public class NamespaceModel : IBrowserModel, IComparable
{
	private string name;
	private List<TypeModel> types;

	public string Name => name;
	public List<TypeModel> Types => types;

	public NamespaceModel(string name, List<Type> types)
	{
		this.name = name;
		this.types = FillTypes(types);
	}

	public List<TypeModel> FillTypes(List<Type> types)
	{
		var flags =
			BindingFlags.Instance
			| BindingFlags.Public
			| BindingFlags.NonPublic
			| BindingFlags.Static
			| BindingFlags.DeclaredOnly;
		return types
			.Where(t => !IsCompilerGenerated(t))
			.Select(t => new TypeModel(t.Name, t.GetMembers(flags).ToList()))
			.ToList();
	}

	bool IsCompilerGenerated(MemberInfo member)
	{
		return member
				.GetCustomAttributes(
					typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
					true
				)
				.Length > 0;
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine(name);
		foreach (var t in types)
			sb.Append($"    {t.ToString()}");
		return sb.ToString();
	}

	public int CompareTo(object? obj)
	{
		if (obj is NamespaceModel n)
			return string.Compare(name, n.Name, true);
		else
			throw new ArgumentException("Invalid Arguments. Argument has to be NamespaceModel");
	}
}
