namespace Core.Models;

public class TypeModel : IBrowserModel
{
	string name;
	List<IBrowserModel> members;

	public TypeModel(string name, List<MemberInfo> membersInfo)
	{
		this.name = name;
		this.members = FillMembers(membersInfo);
	}

	public List<IBrowserModel> FillMembers(List<MemberInfo> membersInfo)
	{
		var result = new List<IBrowserModel>();
		foreach (var member in membersInfo)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					FieldInfo field = (FieldInfo)member;
					result.Add(new FieldModel(field));
					break;
				case MemberTypes.Property:
					var prop = (PropertyInfo)member;
					result.Add(new PropertyModel(prop));
					break;
				case MemberTypes.Method:
					var method = (MethodInfo)member;
					if (!method.IsSpecialName)
						result.Add(new MethodModel(method));
					break;
			}
		}
		return result;
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{name}");
		foreach (var member in members)
			sb.AppendLine($"      {member.ToString()}");
		return sb.ToString();
	}
}
