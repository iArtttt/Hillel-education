namespace TreadsTask
{
    internal class TaskParam<T, TResult>
    {
        public int NumnerOfThread { get; }
        public Memory<T> Data { get; }
        public TResult? Result { get; set; }
        public CancellationToken Token { get; }
        public TaskParam(Memory<T> data, CancellationToken token)
        {
            Data = data;
            Token = token;
        }
        public TaskParam(Memory<T> data, int numnerOfThread, CancellationToken token)
        {
            Data = data;
            NumnerOfThread = numnerOfThread;
            Token = token;
        }
        public static TaskParam<T, TResult> Create(Memory<T> data, int numnerOfThread, CancellationToken token) => new TaskParam<T, TResult>(data, numnerOfThread, token);
        public static TaskParam<T, TResult> Create(Memory<T> data, CancellationToken token) => new TaskParam<T, TResult>(data, token);

    }
}
