namespace Core.Compilers;

internal static class TestsCompilerFactory
{
  static Lazy<XUnitTestsCompiler> xunit = new Lazy<XUnitTestsCompiler>();  
  static Lazy<NUnitTestsCompiler> nunit = new Lazy<NUnitTestsCompiler>();  
  static Lazy<MSTestsCompiler> mstests = new Lazy<MSTestsCompiler>();  

  public static TestsCompiler Create(GeneratorCompilersEnum compiler)
  {
    TestsCompiler result;
    switch(compiler)
    {
      case GeneratorCompilersEnum.NUnit:
        result = nunit.Value;
        break;
      case GeneratorCompilersEnum.XUnit:
        result = xunit.Value;
        break;
      case GeneratorCompilersEnum.MSTests:
        result = mstests.Value;
        break;
      default:
        throw new ArgumentException("Unknown compiler enum");
    }
    return result;
  }
}
