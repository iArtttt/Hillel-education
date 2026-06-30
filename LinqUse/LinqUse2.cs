public class LinqUse2
{
	public LinqUse2()
	{
        var data = new List<object>()
            {
                "Hello",
                  new Book() { Author = "Terry Pratchett", Name = "Guards! Guards!", Pages = 810 },
                  new List<int>() {4, 6, 8, 2},
                  new string[] {"Hello inside array"},

                new Film()
                { Author = "Martin Scorsese", Name= "The Departed", Actors = new List<Actor>()
                    {
                        new Actor() { Name = "Jack Nickolson", Birthdate = new DateTime(1937, 4, 22)},
                        new Actor() { Name = "Leonardo DiCaprio", Birthdate = new DateTime(1974, 11, 11)},
                        new Actor() { Name = "Matt Damon", Birthdate = new DateTime(1970, 8, 10)}
                    }
                },
                new Film()
                {
                    Author = "Gus Van Sant", Name = "Good Will Hunting", Actors = new List<Actor>()
                    {
                        new Actor() { Name = "Matt Damon", Birthdate = new DateTime(1970, 8, 10)},
                        new Actor() { Name = "Robin Williams", Birthdate = new DateTime(1951, 8, 11)},
                    }
                },
                  new Book()
                  { Author = "Stephen King", Name="Finders Keepers", Pages = 200},
                  "Leonardo DiCaprio"
            };

        //  1
        Console.WriteLine(new string(string.Join(", ", data.Except(data.OfType<ArtObject>()).OfType<string[]>().SelectMany(s => s.Select(l => l))) + ", " +
            string.Join(", ", data.Except(data.OfType<ArtObject>()).OfType<List<int>>().SelectMany(s => s.Select(l => l))) + ", " +
            string.Join(", ", data.Except(data.OfType<ArtObject>()).OfType<string>().Select(l => l))
        ));

        //  2
        Console.WriteLine(string.Join(", ", data.OfType<Film>().SelectMany(a => a.Actors.Select(s => s.Name)).Distinct().ToArray()));

        //  3
        Console.WriteLine(data.OfType<Film>().SelectMany(a => a.Actors.Select(s => s.Birthdate)).Distinct().Count(s => s.Date.Month == 8));

        //  4
        Console.WriteLine(string.Join(", ", data.OfType<Film>().SelectMany(a => a.Actors.Select(s => s)).Distinct().OrderBy(s => s.Birthdate).Take(2).Select(a => a.Name)));

        //  5
        Console.WriteLine(new string(string.Join(" ", data.OfType<Book>().Count()) + " Books " + data.OfType<Book>().Select(a => a.Author).Count() + " Authors"));

        //  6
        Console.WriteLine(
            string.Join("\n", string.Join("\n", data.OfType<Book>()
            .Select(a => new string(a.Author + ": " + data.OfType<Book>().Where(b => b.Author.Contains(a.Author)).Select(n => n.Name).Count())).Distinct()) + "\n" +
            string.Join("\n", data.OfType<Film>()
            .Select(a => new string(a.Author + ": " + data.OfType<Film>().Where(b => b.Author.Contains(a.Author)).Select(n => n.Name).Count())).Distinct()
            )));

        //  7
        Console.WriteLine(string.Join("", data.OfType<Film>().SelectMany(a => a.Actors.SelectMany(s => s.Name.ToLower().Select(ch => ch))).Distinct().Count()));

        //  8
        Console.WriteLine(string.Join(", ", data.OfType<Book>().OrderBy(a => a.Author).ThenBy(p => p.Pages).Select(n => n.Name)));

        //  9
        Console.WriteLine(string.Join("", data.OfType<Film>().SelectMany(a => a.Actors.Select(s => s.Name)).Take(1)
            .SelectMany(s => s + ": " + string.Join(", ", data.OfType<Film>().Where((a) => a.Actors.Select(n => n.Name).Contains(s)).Select(f => f.Name).ToArray()))));

        //  10
        Console.WriteLine(string.Join(" ", data.OfType<Book>().Sum(p => p.Pages) + ", int: " + data.OfType<List<int>>().Sum(a => a.Sum())));

        //  11
        Console.WriteLine(data.OfType<Book>().Select(autor => autor.Author).Distinct()
            .ToDictionary(a => a, value => data.OfType<Book>().Where(b => b.Author.Contains(value)).Select(n => n.Name).ToArray()));

        //  12
        Console.WriteLine(string.Join(", ", data.OfType<Film>().Where((a) => a.Actors.Select(n => n.Name).Contains("Matt Damon"))
            .Except(data.OfType<Film>().Where((a) => a.Actors.Any(c => data.OfType<string>().Any(co => co == c.Name)))).Select(f => f.Name).ToArray()));
    }

    class Actor
    {
        public string Name { get; set; }
        public DateTime Birthdate { get; set; }
    }

    abstract class ArtObject
    {
        public string Author { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
    }

    class Film : ArtObject
    {
        public int Length { get; set; }
        public IEnumerable<Actor> Actors { get; set; }
    }

    class Book : ArtObject
    {
        public int Pages { get; set; }
    }
}
