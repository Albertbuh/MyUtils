namespace Core.Models;

public class PropertyModel : TypeMember, IBrowserModel
{
	PropertyInfo info;
	public string Name => info.Name;
	public Type Type => info.PropertyType;

	public PropertyModel(PropertyInfo info)
	{
		this.info = info;
	}

	public override string ToString()
	{
		return $"{Name}: {GetShortName(Type)}";
	}
}
