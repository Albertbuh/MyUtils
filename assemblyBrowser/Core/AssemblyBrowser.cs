namespace Core;

public class AssemblyBrowser
{
	AssemblyModel? assembly;

	public void LoadFrom(string filepath)
	{
		assembly = new AssemblyModel(Assembly.LoadFrom(filepath));
	}

	public string? GetAssemblyInfo()
	{
		return assembly?.ToString();
	}
}
