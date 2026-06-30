namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "D:\\iArttt\\Visual Studio Projects\\Code Remember\\Generator\\Generated_Text.txt";
            Parser parser = new Parser(path);

            parser.ParseLocations();
        }
    }
}
