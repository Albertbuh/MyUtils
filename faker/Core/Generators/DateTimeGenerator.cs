namespace Core.Generators;

public class DateTimeGenerator : IGenerator
{
	Random random = new();
	readonly DateTime startDate = new DateTime(2000, 1, 1);
	readonly DateTime endDate = DateTime.Now;

	public object Generate(Type typeOfGeneration)
	{
		if (typeOfGeneration != typeof(DateTime))
			throw new InvalidOperationException($"Sended type is not datetime {typeOfGeneration}");

		var maxTimeSpan = endDate - startDate;
		var randTimeSpan = new TimeSpan(0, random.Next(0, (int)maxTimeSpan.TotalMinutes), 0);

		return startDate + randTimeSpan;
	}
}
