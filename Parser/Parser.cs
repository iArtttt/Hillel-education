using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Parser
{
    public class Parser
    {
        private string Path { get; set; } = string.Empty;

        public Parser(string path)
        {
            Path = path;
        }

        public void ParseLocations()
        {
            //List<Location> locations = new List<Location>();
            //List<string> json = new List<string>();
            Stopwatch timer = new Stopwatch();
            string newPath = string.Join(string.Empty, Path.SkipLast(Path.Length - Path.LastIndexOf('.'))) + ".json";
            timer.Start();
            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true,
            };



            var text = File.ReadAllText(Path);

            var splited = text.Split(')');
            using (var sw = File.OpenWrite(newPath))
            {
                foreach (var line in splited)
                {
                    var spLoc = line.Split(';', '(', ':');

                    if (spLoc.Length == 5)
                    {
                        string towrite = JsonSerializer.Serialize(new Location(spLoc[3], spLoc[4], spLoc[0], double.Parse(spLoc[1]), int.Parse(spLoc[2])), jsonOptions);
                        sw.Write(Encoding.UTF8.GetBytes(towrite));
                    }

                }
            }


            timer.Stop();
            Console.WriteLine(timer);

        }

    }
}
