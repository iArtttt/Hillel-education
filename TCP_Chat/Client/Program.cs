namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Client client = new Client())
            {
                client.Start();
            }
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
