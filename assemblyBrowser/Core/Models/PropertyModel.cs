namespace Core.Models;

public class PropertyModel : TypeMember, IBrowserModel
{
	PropertyInfo prop;
	public override string Name => prop.Name;
	public override Type Type => prop.PropertyType;
  public override string Modificator => GetModificator();

	public PropertyModel(PropertyInfo info)
	{
		this.prop = info;
	}

	public override string ToString()
	{
		return $"{Modificator}{GetShortName(Type)} {Name}";
	}

  public string GetModificator()
  {
    string modificator = "";
    var propGet = prop.GetGetMethod();
    if(propGet != null)
    {
      if (propGet.IsPublic)
          modificator += "public ";
      else if (propGet.IsPrivate)
          modificator += "private ";
      else if (propGet.IsAssembly)
          modificator += "internal ";
      else if (propGet.IsFamily)
          modificator += "protected ";
      else if (propGet.IsFamilyAndAssembly)
          modificator += "private protected ";
      else if (propGet.IsFamilyOrAssembly)
          modificator += "protected internal ";

      if(propGet.IsStatic)
        modificator += "static ";
    }
    

    return modificator;
  }
}
