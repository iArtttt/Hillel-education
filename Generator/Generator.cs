using System.Diagnostics;

namespace Generator
{
    public class Generator
    {
        private string Path { get; set; }

        public Generator()
        {
            Path = string.Empty;
        }
        public Generator(string path)
        {
            Path = path;
        }
        public void ChangePath(string newPath)
        {
            Path = Directory.Exists(newPath) ? newPath : Path;
        }

        public void Generate(int countToGenerate)
        {

            Stopwatch timer = new Stopwatch();
            timer.Start();
            Random rand = new Random();
            using (StreamWriter sw = new StreamWriter(Path, true))
            {
                string[] countrys = Countries.Locations.Keys.ToArray();
                Dictionary<string, string[]> districts = Countries.Locations.ToDictionary(d => d.Key, d => d.Value.Keys.ToArray());


                for (int i = 0; i < countToGenerate; i++)
                {
                    string country = countrys[rand.Next(countrys.Length)];
                    string district = districts[country][rand.Next(districts[country].Length)];
                    string[] cityes = Countries.Locations[country][district];
                    string city = cityes[rand.Next(cityes.Length)];

                    //Location location = new Location(country, district, city, Math.Round(rand.NextDouble() * rand.Next(100000000), 2), rand.Next(20000000));

                    //sw.WriteLine(location.ToString());  //$"{City}:{Area};{Population};{Country}({District})";

                    sw.Write(country);
                    sw.Write(":");
                    sw.Write(Math.Round(rand.NextDouble() * rand.Next(100000000)));
                    sw.Write(";");
                    sw.Write(rand.Next(20000000));
                    sw.Write(";");
                    sw.Write(country);
                    sw.Write("(");
                    sw.Write(district);
                    sw.Write(")");


                }

            }

            timer.Stop();
            Console.WriteLine(timer);

        }
    }
}
