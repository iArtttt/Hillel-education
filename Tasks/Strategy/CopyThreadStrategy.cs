using TreadsTask.Interface;
using TreadsTask.Static;

namespace TreadsTask.Strategy
{
    internal class CopyThreadStrategy<T> : IInitParams, IThreadStrategy<T, T[]>
    {
        public Range Range { get; }
        public CopyThreadStrategy(int startIndex, int lastIndex)
        {
            Range = new Range(startIndex, lastIndex);
        }

        public TaskParam<T1, TResult>[] Init<T1, TResult>(Memory<T1> data, int threadCount, CancellationToken token)
        {
            var result = new TaskParam<T1, TResult>[threadCount];
            data = data[Range];

            var itemsCount = data.Length / threadCount;

            for (int i = 0; i < threadCount; i++)
            {
                result[i] = TaskParam<T1, TResult>.Create(data.Slice(i * itemsCount, itemsCount), i, token);
            }
            return result;
        }

        public void ThreadMethod(object? obj)
        {
            var param = (TaskParam<T, T[]>)obj!;
            var span = param.Data.Span;
            T[] result = new T[span.Length];

            for (int i = 0; i < span.Length; i++)
            {
                if (param.Token.IsCancellationRequested) return;
                result[i] = span[i];
                param.ProgressUpdate(i);
            }
            param.Result = result;
        }

        public T[] ThreadResult(TaskParam<T, T[]>[] threadParams) => threadParams.SelectMany(s => s.Result!).ToArray();
    }
}
