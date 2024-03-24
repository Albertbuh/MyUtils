namespace Core.Models;

public class FieldModel : TypeMember, IBrowserModel
{
	FieldInfo field;
	public override string Name => field.Name;
	public override Type Type => field.FieldType;
  public string ShortTypeName => GetShortName(Type);
  public override string Modificator => GetModificator();

	public FieldModel(FieldInfo info)
	{
		this.field = info;
	}

	public override string ToString()
	{
		return $"{Modificator}{ShortTypeName} {Name}";
	}

  public string GetModificator()
  {
    string modificator = "";

    if (field.IsPublic)
        modificator += "public ";
    else if (field.IsPrivate)
        modificator += "private ";
    else if (field.IsAssembly)
        modificator += "internal ";
    else if (field.IsFamily)
        modificator += "protected ";
    else if (field.IsFamilyAndAssembly)
        modificator += "private protected ";
    else if (field.IsFamilyOrAssembly)
        modificator += "protected internal ";

    if(field.IsStatic)
      modificator += "static ";

    return modificator;
  }
}
