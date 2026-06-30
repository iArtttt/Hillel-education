using System.Diagnostics;
using TreadsTask.Interface;

namespace TreadsTask
{
    internal class ThreadsUse<T, TResult>
    {
        private readonly IThreadStrategy<T, TResult> _strategy;
        private readonly T[] _array;
        private readonly Stopwatch _timer = new();
        private ThreadParam<T, TResult>[]? _threadParams;
        private Thread[] Threads { get; set; }
        public TResult? Result { get; private set; } = default;
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        public ThreadsUse(int threads, T[] array, IThreadStrategy<T, TResult> strategy)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(threads);
            ArgumentNullException.ThrowIfNull(array);
            ArgumentNullException.ThrowIfNull(strategy);

            _array = array;
            Threads = new Thread[threads];
            _strategy = strategy;
        }

        public void ThreadStart()
        {
            CreateThreads();
            if(_threadParams == null || _strategy == null) return; 

            if (_strategy is IInitParams init)
            {
                _threadParams = init.Init<T, TResult>(_array.AsMemory(), Threads.Length, _cancellationToken.Token);
            }
            else
            {
                var data = _array.AsMemory();
                var itemsCount = data.Length / Threads.Length;

                for (var i = 0; i < _threadParams.Length; i++)
                {
                    _threadParams[i] = ThreadParam<T, TResult>.Create(data.Slice(i * itemsCount, itemsCount), i, _cancellationToken.Token);
                }
            }

            try
            {
                _timer.Start();
                for (int i = 0; i < Threads.Length; i++)
                    Threads[i].Start(_threadParams[i]);

                new Thread(Interrupt).Start();

                for (int i = 0; i < Threads.Length; i++)
                    Threads[i].Join();

                Result = _strategy.ThreadResult(_threadParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n{ex.Message}\n");
            }
            finally { _timer.Stop(); }
        }

        private void Interrupt(object? obj)
        {
            var key = Console.ReadKey().Key;

            if (key == ConsoleKey.Escape)
            {
                _cancellationToken.Cancel();
                Console.WriteLine("Is Canseled!");
            }
        }

        private void CreateThreads()
        {
            for (int i = 0; i < Threads.Length; i++)
            {
                Threads[i] = new Thread(_strategy.ThreadMethod);
            }
            _threadParams = new ThreadParam<T, TResult>[Threads.Length]; 
        }
        public void Print()
        {
            Console.WriteLine($"Result is {Result}; Time is {_timer.Elapsed}");
        }


    }
}
