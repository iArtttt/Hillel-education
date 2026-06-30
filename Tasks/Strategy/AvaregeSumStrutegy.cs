using System.Numerics;
using TreadsTask.Interface;
using TreadsTask.Static;

namespace TreadsTask.Strategy
{
    internal abstract class AvaregeSumStrutegy<T, TResult> : IThreadStrategy<T, TResult> where T : struct, INumber<T> where TResult : struct, INumber<TResult>
    {
        protected abstract TResult ResultMethod(TResult result, ThreadParam<T, TResult> param);
        protected abstract TResult FinalAction(TResult result, ThreadParam<T, TResult> param);
        protected abstract TResult FinalResult(TResult result, ThreadParam<T, TResult>[] param);
        public void ThreadMethod(object? obj)
        {
            ThreadParam<T, TResult> param = (ThreadParam<T, TResult>)obj!;
            var span = param.Data.Span;
            TResult result = default;

            for (int i = 0; i < span.Length; i++)
            {
                if (param.Token.IsCancellationRequested) return;
                result += TResult.CreateChecked(span[i]);
                param.ProgressUpdate(i);
            }

            param.Result = ResultMethod(result, param);
        }


        public TResult ThreadResult(ThreadParam<T, TResult>[] threadParams)
        {
            TResult result = default;
            foreach (var thread in threadParams)
            {
                result = FinalAction(result, thread);
            }
            return FinalResult(result, threadParams);
        }
    }
}
