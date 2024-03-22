namespace Core.Generators;

public class StringGenerator : IGenerator
{
	const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
	Random random = new();

	public object Generate(Type typeToGenerate)
	{
		if (typeToGenerate == typeof(char))
			return chars[random.Next(chars.Length)];
		if (typeToGenerate != typeof(string))
			throw new InvalidOperationException($"Sended type is not string {typeToGenerate}");

		var length = random.Next(5, 15);
		return new string(
			Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()
		);
	}
}
