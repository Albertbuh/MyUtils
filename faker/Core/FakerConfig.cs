using System.Linq.Expressions;
using Core.Generators;

namespace Core;

public class FakerConfig
{
	private Dictionary<Type, IGenerator> generators = new();
	internal List<ConfigItem> items = new();

	public void Add<T, TElem, TGen>(Expression<Func<T, TElem>> expression)
		where TGen : IGenerator
	{
    if(expression.Body is MemberExpression memberExpression)
    {
      if (!generators.ContainsKey(typeof(TGen)))
        generators.Add(typeof(TGen), Activator.CreateInstance<TGen>());
      
      items.Add(
        new ConfigItem()
        {
          Expression = memberExpression,
          Generator = generators[typeof(TGen)],
          ObjectType = typeof(T)
        }
      );
    }
    else
      throw new ArgumentException("Invalid expression. Expression should be a property getter");
	}
}

internal struct ConfigItem
{
	public MemberExpression Expression;
	public IGenerator Generator;
	public Type ObjectType;
}
