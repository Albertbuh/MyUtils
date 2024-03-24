namespace Core.Models;

public class MethodModel : TypeMember, IBrowserModel
{
	MethodInfo method;
	public override string Name => method.Name;
	public override Type Type => method.ReturnType;
  public override string Modificator => GetModificator();

	public MethodModel(MethodInfo info)
	{
		method = info;
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.Append($"{Modificator}{GetShortName(Type)} {Name}");
		var parameters = method.GetParameters();

		sb.Append("(");
		sb.AppendJoin(',', parameters.Select(p => $"{GetShortName(p.ParameterType)} {p.Name}"));
		sb.Append(")");
		return sb.ToString();
	}

  public string GetModificator()
  {
    string modificator = "";

    if (method.IsPublic)
        modificator += "public ";
    else if (method.IsPrivate)
        modificator += "private ";
    else if (method.IsAssembly)
        modificator += "internal ";
    else if (method.IsFamily)
        modificator += "protected ";
    else if (method.IsFamilyAndAssembly)
        modificator += "private protected ";
    else if (method.IsFamilyOrAssembly)
        modificator += "protected internal ";

    if(method.IsStatic)
      modificator += "static ";

    return modificator;
  }
}
