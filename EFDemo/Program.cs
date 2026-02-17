using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace EFDemo
{
    class Program
    {
        static void Main()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<PagilaContext>()
                .UseNpgsql(config.GetConnectionString("Pagila"))
                .Options;

            using var context = new PagilaContext(options);

            //int actorId = InsertActor(context, "Kimberly", "Collins");
            //int filmId = InsertFilm(context, "Kimberly's Film");
            //int sequelId = InsertFilm(context, "Kimberly's Film - The Sequel");
            //InsertFilmActor(context, actorId, filmId);
            //InsertFilmActor(context, actorId, sequelId);
            //InsertFilmActor(context, actorId, sequelId);

            //GetActors(context, "Collins");
            //GetFilms(context, DateTime.Today.Year);

            WriteToCSV(context);
            ReadFromCSV();
        }

        private static void WriteToCSV(PagilaContext context)
        {
            using var writer = new StreamWriter("actors.csv");
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var actors = context.Actors.Take(5);
            csv.WriteRecords(actors);
        }

        private static void ReadFromCSV()
        {
            using var reader = new StreamReader("actors.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var actors = csv.GetRecords<Actor>().ToList();

            foreach (var actor in actors)
            {
                Console.WriteLine($"{actor.ActorId}: {actor.FirstName} {actor.LastName}");
            }
        }

        static void GetActors(PagilaContext context)
        {
            Console.WriteLine("First 5 Actors:");

            var actors = context.Actors
                .Include(a => a.Films)
                .OrderBy(a => a.ActorId)
                .Take(5)
                .ToList();

            foreach (var actor in actors)
            {
                Console.WriteLine($"{actor.ActorId}: {actor.FirstName} {actor.LastName} ({actor.Films.Count} films)");
            }

            Console.WriteLine();
        }

        static void GetActors(PagilaContext context, string lastName)
        {
            Console.WriteLine($"Actors with last name of {lastName}:");

            var query = context.Actors
                .Include(a => a.Films)
                .Where(a => a.LastName == lastName)
                .OrderBy(a => a.ActorId);

            Console.WriteLine(query.ToQueryString()); //Example of logging generated SQL

            var actors = query.ToList();

            foreach (var actor in actors)
            {
                Console.WriteLine($"{actor.ActorId}: {actor.FirstName} {actor.LastName} ({actor.Films.Count} films)");
            }

            Console.WriteLine();
        }

        static int InsertActor(PagilaContext context, string firstName, string lastName)
        {
            var actor = new Actor
            {
                FirstName = firstName,
                LastName = lastName
            };

            context.Actors.Add(actor);
            context.SaveChanges();

            Console.WriteLine($"Inserted actor with ID {actor.ActorId}");
            Console.WriteLine();

            return actor.ActorId;
        }

        static void GetFilms(PagilaContext context, int releaseYear)
        {
            Console.WriteLine($"Films released in {releaseYear}:");

            var films = context.Films
                .Include(f => f.Actors)
                .Where(f => f.ReleaseYear == releaseYear)
                .OrderBy(f => f.FilmId)
                .ToList();

            foreach (var film in films)
            {
                Console.WriteLine($"{film.FilmId}: {film.Title} ({film.Actors.Count} actors)");
            }

            Console.WriteLine();
        }

        static int InsertFilm(PagilaContext context, string title)
        {
            var film = new Film
            {
                Title = title,
                Description = null, //Example of inserting NULL value
                ReleaseYear = DateTime.Today.Year
            };

            context.Films.Add(film);
            context.SaveChanges();

            return film.FilmId;
        }

        private static void InsertFilmActor(PagilaContext context, int actorId, int filmId)
        {
            var actor = context.Actors
                .FirstOrDefault(a => a.ActorId == actorId);

            var film = context.Films
                .Include(f => f.Actors)
                .FirstOrDefault(f => f.FilmId == filmId);

            if (film != null && actor != null)
            {
                if (!film.Actors.Any(a => a.ActorId == actor.ActorId))
                {
                    film.Actors.Add(actor);
                    context.SaveChanges();
                    Console.WriteLine($"Added {actor.FirstName} {actor.LastName} to {film.Title}");
                }
                else
                {
                    Console.WriteLine($"Actor {actor.FirstName} {actor.LastName} is already in the film {film.Title}.");
                }
            }
        }

        static void UpdateActor(PagilaContext context, int actorId)
        {
            var actor = context.Actors.Find(actorId);

            if (actor != null)
            {
                actor.LastName = "NewLastName";
                actor.LastUpdate = DateTime.UtcNow;

                context.SaveChanges();
                Console.WriteLine($"Updated actor {actorId}");

                Console.WriteLine();
            }
        }

        static void DeleteActor(PagilaContext context, int actorId)
        {
            var actor = context.Actors.Find(actorId);

            if (actor != null)
            {
                context.Actors.Remove(actor);
                context.SaveChanges();
                Console.WriteLine($"Deleted actor {actorId}");
            }
        }
    }
}
