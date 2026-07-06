namespace Server.Lib
{
    public static class Writer
    {
        internal static void WriteLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void Writeline(string message) => WriteLine(message, ConsoleColor.Gray);
        public static void ErrorWriteline(string message) => WriteLine(message, ConsoleColor.Red);
        public static void InfoWriteline(string message) => WriteLine(message, ConsoleColor.Yellow);
        public static void SuccessWriteline(string message) => WriteLine(message, ConsoleColor.Green);
        public static void DarkBlueWriteline(string message) => WriteLine(message, ConsoleColor.DarkBlue);
        public static void BlueWriteline(string message) => WriteLine(message, ConsoleColor.Blue);
        public static void DarkYellowWriteline(string message) => WriteLine(message, ConsoleColor.DarkYellow);
    }
}
