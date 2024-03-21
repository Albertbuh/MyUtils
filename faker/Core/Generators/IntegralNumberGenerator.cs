namespace Core.Generators;

public class IntegralNumberGenerator : NumberGenerator
{
  public IntegralNumberGenerator()
  {
    generationHandler.Add(typeof(sbyte), () => GenerateSByte());
    generationHandler.Add(typeof(byte), () => GenerateByte());
    generationHandler.Add(typeof(short), () => GenerateShort());
    generationHandler.Add(typeof(ushort), () => GenerateUShort());
    generationHandler.Add(typeof(int), () => GenerateInt());
    generationHandler.Add(typeof(uint), () => GenerateUInt());
    generationHandler.Add(typeof(long), () => GenerateLong());
    generationHandler.Add(typeof(ulong), () => GenerateULong());
  }
  
  private sbyte GenerateSByte() => (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue + 1);
  private byte GenerateByte() => (byte)random.Next(byte.MinValue, byte.MaxValue + 1);
  private short GenerateShort() => (short)random.Next(short.MinValue, short.MaxValue + 1);
  private ushort GenerateUShort() => (ushort)random.Next(ushort.MinValue, ushort.MaxValue + 1);
  private int GenerateInt() => random.Next(int.MinValue, int.MaxValue);
  private uint GenerateUInt() => (uint)random.NextInt64(0, uint.MaxValue);
  private long GenerateLong() => random.NextInt64(long.MinValue, long.MaxValue);
  private ulong GenerateULong() 
  {
    var highbits = (uint)random.Next();
    var lowbits = (uint)random.Next();
    return ((ulong)highbits << 32) | lowbits;
  }
}
