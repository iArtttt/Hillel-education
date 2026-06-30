using System.Numerics;

namespace TreadsTask.Strategy
{
    internal class AverageThreadStrategy<T> : AvaregeSumStrutegy<T, decimal> where T : struct, INumber<T>
    {
        protected override decimal FinalAction(decimal result, ThreadParam<T, decimal> param) => result + param.Result;

        protected override decimal FinalResult(decimal result, ThreadParam<T, decimal>[] param) => result / param.Length;

        protected override decimal ResultMethod(decimal result, ThreadParam<T, decimal> param) => result / param.Data.Span.Length;
    }
}
