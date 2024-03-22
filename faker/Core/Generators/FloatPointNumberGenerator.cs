namespace Core.Generators;

public class FloatPointNumberGenerator : NumberGenerator
{
	public FloatPointNumberGenerator()
	{
		generationHandler.Add(typeof(float), () => GenerateFloat());
		generationHandler.Add(typeof(double), () => GenerateDouble());
		generationHandler.Add(typeof(decimal), () => GenerateDecimal());
	}

	private float GenerateFloat() => random.NextSingle();

	private double GenerateDouble() => random.NextDouble();

	private decimal GenerateDecimal()
	{
		int lo = random.Next(int.MinValue, int.MaxValue);
		int mid = random.Next(int.MinValue, int.MaxValue);
		int hi = random.Next(int.MinValue, int.MaxValue);
		bool isNegative = random.Next(2) == 0;
		return new decimal(lo, mid, hi, isNegative, 0);
	}
}
