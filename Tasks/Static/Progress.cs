namespace TreadsTask.Static
{
    internal static class Progress
    {
        public static void ProgressUpdate<T, TResult>(this TaskParam<T, TResult> param, int progress)
        {
            if ((progress + 1) % (param.Data.Length / 100) == 0
                || (progress + 1) % param.Data.Length == 0)
            {
                Console.SetCursorPosition(0, param.NumnerOfThread + 6);
                Console.WriteLine($"Thread {param.NumnerOfThread + 1} Progress: {(int)((decimal)(progress + 1) / param.Data.Span.Length * 100)}% of 100%");
            }
        }
    }
}
