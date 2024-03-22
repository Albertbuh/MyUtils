namespace Core.Models;

public class FieldModel : TypeMember, IBrowserModel
{
	FieldInfo info;
	public string Name => info.Name;
	public Type Type => info.FieldType;
  public string ShortTypeName => GetShortName(Type);

	public FieldModel(FieldInfo info)
	{
		this.info = info;
	}

	public override string ToString()
	{
		return $"{Name}: {ShortTypeName}";
	}
}
