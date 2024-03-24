namespace Core.Tests;

[TestFixture]
public class AssemblyBrowserTests
{
  AssemblyBrowser browser = new();
  Models.AssemblyModel? assembly;

  [Test, Order(1)]
  public void SmokeReadCurrentAssembly()
  {
    assembly = browser.LoadFrom(Assembly.GetExecutingAssembly().Location);

    Assert.NotNull(assembly);
    Assert.AreEqual(assembly!.Members.Count, 2);
  }

  [Test]
  public void CheckFooAndBooClassesInCurrentAssembly()
  {
    var namespaces = assembly!.Members;
    var namesOfTypes = namespaces.Select(n => n.Types).SelectMany(t => t).Select(t => t.Name);

    Assert.True(namesOfTypes.Contains("Foo"), "Unable to find type Foo");
    Assert.True(namesOfTypes.Contains("Boo"), "Unable to find type Boo");
  }

  [Test]
  public void TestClassWithPrivateMethods()
  {
    var types = assembly!.Members.Select(n => n.Types).SelectMany(t => t);
    var boo = types.FirstOrDefault(t => t.Name.Equals("Boo"));

    Assert.NotNull(boo);
    Assert.AreEqual(Boo.Amount, boo!.Members.Count, $"Amount of members in Boo is {boo!.Members.Count}");
    
    var modifiers = new string[] {"private", "protected", "private protected", ""};
    var privateMembers = boo!.Members.Where(m => !m.Modificator.Contains("static"));
    foreach(var member in privateMembers)
      Assert.Contains(member.Modificator.Trim(), modifiers, $"Some field is not private: {member.ToString()}");
  }
}
