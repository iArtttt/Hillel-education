namespace TreadsTask
{
    internal class ThreadParam<T, TResult>
    {
        public int NumnerOfThread { get; }
        public Memory<T> Data { get; }
        public TResult? Result { get; set; }
        public CancellationToken Token { get; }
        public ThreadParam(Memory<T> data, CancellationToken token)
        {
            Data = data;
            Token = token;
        }
        public ThreadParam(Memory<T> data, int numnerOfThread, CancellationToken token)
        {
            Data = data;
            NumnerOfThread = numnerOfThread;
            Token = token;
        }
        public static ThreadParam<T, TResult> Create(Memory<T> data, int numnerOfThread, CancellationToken token) => new ThreadParam<T, TResult>(data, numnerOfThread, token);
        public static ThreadParam<T, TResult> Create(Memory<T> data, CancellationToken token) => new ThreadParam<T, TResult>(data, token);

    }
}
