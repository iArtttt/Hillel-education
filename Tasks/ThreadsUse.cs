using System.Diagnostics;
using TreadsTask.Interface;

namespace TreadsTask
{
    internal class ThreadsUse<T, TResult>
    {
        private readonly IThreadStrategy<T, TResult> _strategy;
        private readonly T[] _array;
        private readonly Stopwatch _timer = new();
        private TaskParam<T, TResult>[]? _taskParams;
        private Task[] Tasks { get; set; }
        public TResult? Result { get; private set; } = default;
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        public ThreadsUse(int threads, T[] array, IThreadStrategy<T, TResult> strategy)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(threads);
            ArgumentNullException.ThrowIfNull(array);
            ArgumentNullException.ThrowIfNull(strategy);

            _array = array;
            Tasks = new Task[threads];
            _strategy = strategy;
        }

        public async Task<TResult?> ThreadStart()
        {
            
            _taskParams = new TaskParam<T, TResult>[Tasks.Length];

            if (_taskParams == null || _strategy == null) throw new ArgumentNullException(); 

            if (_strategy is IInitParams init)
            {
                _taskParams = init.Init<T, TResult>(_array.AsMemory(), Tasks.Length, _cancellationToken.Token);
            }
            else
            {
                CreateTask();
            }

            try
            {
                _timer.Start();

                for (int i = 0; i < Tasks.Length; i++)
                    Tasks[i].Start();
                var abortTask = Interrupt();

                await Task.WhenAll(Tasks);
               

                Result = _strategy.ThreadResult(_taskParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n{ex.Message}\n");
            }
            finally 
            { 
                _timer.Stop();
                _cancellationToken.Dispose();
            }
            return Result;
        }

        private Task Interrupt()
        {
            return Task.Run(() =>
            {
                while (!Tasks.Any(t => t.IsCompleted) && !_cancellationToken.IsCancellationRequested)
                {
                    Console.SetCursorPosition(10, 1);
                    Console.WriteLine("Press 'Escape' to interrupt threads");

                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        _cancellationToken.Cancel();
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\nTask was canseled\n");
                        Console.ResetColor();
                    }
                }
            });
        }

        private void CreateTask()
        {
            var data = _array.AsMemory();
            var itemsCount = data.Length / Tasks.Length;

            for (var i = 0; i < _taskParams.Length; i++)
            {
                _taskParams[i] = TaskParam<T, TResult>.Create(data.Slice(i * itemsCount, itemsCount), i, _cancellationToken.Token);
                Tasks[i] = new Task(() => _strategy.ThreadMethod(_taskParams[i]), _cancellationToken.Token);
            }
            
        }
        public void Print()
        {
            Console.WriteLine($"Result is {Result}; Time is {_timer.Elapsed}");
        }


    }
}
