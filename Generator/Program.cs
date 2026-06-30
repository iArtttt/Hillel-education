namespace Generator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Generator generator = new Generator("D:\\iArttt\\Visual Studio Projects\\Code Remember\\Generator\\Generated_Text.txt");

            generator.Generate(100000);
        }
    }
}
