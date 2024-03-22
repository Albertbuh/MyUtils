namespace Core.Models;

public class MethodModel : TypeMember, IBrowserModel
{
	MethodInfo method;
	public string Name => method.Name;
	public Type Type => method.ReturnType;

	public MethodModel(MethodInfo info)
	{
		method = info;
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.Append($"{GetShortName(Type)} {Name}");
		var parameters = method.GetParameters();

		sb.Append("(");
		sb.AppendJoin(',', parameters.Select(p => $"{GetShortName(p.ParameterType)} {p.Name}"));
		sb.Append(")");
		return sb.ToString();
	}
}
