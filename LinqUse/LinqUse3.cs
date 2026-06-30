public class LinqUse3
{
	public LinqUse3()
	{
        List<Film> films = new List<Film>()
            {
              new Film { Name = "The Silence of the Lambs", Director ="Jonathan Demme" },
              new Film { Name = "The World's Fastest Indian", Director ="Roger Donaldson" },
              new Film { Name = "The Recruit", Director ="Roger Donaldson" }
            };

        List<Director> directors = new List<Director>()
            {
              new Director {Name="Jonathan Demme", Country="USA"},
              new Director {Name="Roger Donaldson", Country="New Zealand"},
            };

        List<string> strings = new List<string>() { "I", "like", "play", "board", "games", "!" };

        Console.WriteLine(strings.Aggregate((f, s) => f += " " + s));

        Console.WriteLine(strings.Aggregate((f, s) => f += s));

        Console.WriteLine(string.Join("\n", films.GroupJoin(directors, s => s.Director, d => d.Name, (s, d) => new
        {
            Film = s.Name,
            Name = s.Director,
            d.FirstOrDefault().Country
        })));

        Console.WriteLine(string.Join("\n", directors.GroupJoin(films, d => d.Name, f => f.Director, (d, f) => new
        {
            Director = d.Name,
            Films = string.Join(", ", f.Select(s => s.Name))
        })));

    }
    class Film
    {
        public string Name { get; set; }
        public string Director { get; set; }
    }
    class Director
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
