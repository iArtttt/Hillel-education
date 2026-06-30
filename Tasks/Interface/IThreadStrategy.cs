namespace TreadsTask.Interface
{
    internal interface IThreadStrategy<T, TResult>
    {
        public void ThreadMethod(object? obj);
        public TResult ThreadResult(ThreadParam<T, TResult>[] threadParams);
    }
}
