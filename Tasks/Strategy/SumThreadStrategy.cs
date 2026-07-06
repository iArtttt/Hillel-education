using System.Numerics;

namespace TreadsTask.Strategy
{
    internal class SumThreadStrategy<T> : AvaregeSumStrutegy<T, ulong> where T : struct, INumber<T>
    {
        protected override ulong ResultMethod(ulong result, TaskParam<T, ulong> param) => result;
        protected override ulong FinalResult(ulong result, TaskParam<T, ulong>[] param) => result;
        protected override ulong FinalAction(ulong result, TaskParam<T, ulong> param) => result + param.Result;
    }
}
