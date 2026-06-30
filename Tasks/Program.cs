using TreadsTask.Strategy;

namespace TreadsTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            Random random = new Random();
            int[] arr = new int[1_140_060_464];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = i + 1;
            }
            Console.Write("Please write how much threads do you want to use? --> ");
            var threadsCount = int.Parse(Console.ReadLine());
            if (threadsCount <= 0) threadsCount = 1;

            var pop = new ThreadsUse<int, int>(threadsCount, arr, new MaxStrategy<int>());
            pop.ThreadStart();
            pop.Print();

        }


    }
}
