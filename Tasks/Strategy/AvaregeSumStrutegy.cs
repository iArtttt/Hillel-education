using System.Numerics;
using TreadsTask.Interface;
using TreadsTask.Static;

namespace TreadsTask.Strategy
{
    internal abstract class AvaregeSumStrutegy<T, TResult> : IThreadStrategy<T, TResult> where T : struct, INumber<T> where TResult : struct, INumber<TResult>
    {
        protected abstract TResult ResultMethod(TResult result, TaskParam<T, TResult> param);
        protected abstract TResult FinalAction(TResult result, TaskParam<T, TResult> param);
        protected abstract TResult FinalResult(TResult result, TaskParam<T, TResult>[] param);
        public void ThreadMethod(object? obj)
        {
            TaskParam<T, TResult> param = (TaskParam<T, TResult>)obj!;
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


        public TResult ThreadResult(TaskParam<T, TResult>[] threadParams)
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
