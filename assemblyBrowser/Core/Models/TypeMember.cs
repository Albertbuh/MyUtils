namespace Core.Models;

public abstract class TypeMember
{
	protected string GetShortName(Type type)
	{
		if (type.IsGenericType)
		{
			string genericTypeName = type.Name.Split('`')[0];
			Type[] genericArguments = type.GetGenericArguments();
			string[] genericArgumentNames = new string[genericArguments.Length];

			for (int i = 0; i < genericArguments.Length; i++)
			{
				genericArgumentNames[i] = GetShortName(genericArguments[i]);
			}

			return $"{genericTypeName}<{string.Join(", ", genericArgumentNames)}>";
		}

		return type.Name;
	}
}
