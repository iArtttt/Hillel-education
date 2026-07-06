using System.Numerics;
using TreadsTask.Interface;
using TreadsTask.Static;

namespace TreadsTask.Strategy
{
    internal abstract class MaxMinStrategy<T> : IThreadStrategy<T, T> where T : struct, INumber<T>
    {
        protected abstract int Compare(T result, T compare);
        public void ThreadMethod(object? obj)
        {
            TaskParam<T, T> param = (TaskParam<T, T>)obj!;
            var span = param.Data.Span;
            T result = default;

            for (int i = 0; i < span.Length; i++)
            {
                if (param.Token.IsCancellationRequested) return;
                if (Compare(result, span[i]) < 0)
                    result = span[i];
                param.ProgressUpdate(i);
            }
            param.Result = result;
        }

        public T ThreadResult(TaskParam<T, T>[] threadParams)
        {
            T result = default;
            foreach (var thread in threadParams)
            {
                if (Compare(result, thread.Result) < 0)
                    result = thread.Result;
            }
            return result;
        }
    }

    internal class MaxStrategy<T> : MaxMinStrategy<T> where T : struct, INumber<T>
    {
        protected override int Compare(T result, T compare) => result > compare ? 1 : -1;
    }

    internal class MinStrategy<T> : MaxMinStrategy<T> where T : struct, INumber<T>
    {
        protected override int Compare(T result, T compare) => result < compare ? 1 : -1;
    }
}
