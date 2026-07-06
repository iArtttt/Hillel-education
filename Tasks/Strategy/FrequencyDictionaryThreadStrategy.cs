using TreadsTask.Interface;
using TreadsTask.Static;

namespace TreadsTask.Strategy
{
    internal class FrequencyDictionaryThreadStrategy<T> : IThreadStrategy<T, Dictionary<T, int>> where T : notnull
    {
        public void ThreadMethod(object? obj)
        {
            var param = (TaskParam<T, Dictionary<T, int>>)obj!;
            var span = param.Data.Span;
            Dictionary<T, int> result = new ();

            for (int i = 0; i < span.Length; i++)
            {
                if (param.Token.IsCancellationRequested) return;
                if (result.TryGetValue(span[i], out int value))
                    result[span[i]] = ++value;
                else
                    result.Add(span[i], 1);
                param.ProgressUpdate(i);

            }
            param.Result = result;
        }

        public Dictionary<T, int> ThreadResult(TaskParam<T, Dictionary<T, int>>[] threadParams)
        {
            Dictionary<T, int> result = new();
            foreach (var thread in threadParams)
            {
                foreach (var param in thread.Result!)
                {
                    if (result.ContainsKey(param.Key))
                    {
                        result[param.Key] += param.Value;
                    }
                    else
                    {
                        result.Add(param.Key, param.Value);
                    }
                }
            }
            return result;
        }
    }
}
