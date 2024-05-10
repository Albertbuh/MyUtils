namespace Tracer.Core.Models
{
    ///<summary>
    ///Save diagnostic data for certain method
    ///</summary>
    internal record struct TraceDiagnostic(StackFrame Frame);
}
