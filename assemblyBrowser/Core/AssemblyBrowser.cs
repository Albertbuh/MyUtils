namespace Core;

public class AssemblyBrowser
{
	AssemblyModel? assembly;

	public AssemblyModel LoadFrom(string filepath)
	{
		assembly = new AssemblyModel(Assembly.LoadFrom(filepath));
		return assembly;
	}

	public string? GetAssemblyInfo()
	{
		return assembly?.ToString();
	}
}
