namespace TreadsTask.Interface
{
    internal interface IInitParams
    {
        public ThreadParam<T, TResult>[] Init<T, TResult>(Memory<T> data, int threadCount, CancellationToken token);
    }
}
