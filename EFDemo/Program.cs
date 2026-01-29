using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

            GetActors(context);

            int actorId = InsertActor(context);

            GetActors(context, "Collins");

            UpdateActor(context, actorId);

            GetActors(context, "NewLastName");

            DeleteActor(context, actorId);
        }

        static void GetActors(PagilaContext context)
        {
            Console.WriteLine("First 5 Actors:");

            var actors = context.Actors
                .OrderBy(a => a.ActorId)
                .Take(5)
                .ToList();

            foreach (var actor in actors)
            {
                Console.WriteLine($"{actor.ActorId}: {actor.FirstName} {actor.LastName}");
            }

            Console.WriteLine();
        }

        static void GetActors(PagilaContext context, string lastName)
        {
            Console.WriteLine($"Actors with last name of {lastName}:");

            var actors = context.Actors
                .Where(a => a.LastName == lastName)
                .OrderBy(a => a.ActorId)
                .ToList();

            foreach (var actor in actors)
            {
                Console.WriteLine($"{actor.ActorId}: {actor.FirstName} {actor.LastName}");
            }

            Console.WriteLine();
        }

        static int InsertActor(PagilaContext context)
        {
            var actor = new Actor
            {
                FirstName = "Kimberly",
                LastName = "Collins",
                LastUpdate = DateTime.UtcNow
            };

            context.Actors.Add(actor);
            context.SaveChanges();

            Console.WriteLine($"Inserted actor with ID {actor.ActorId}");
            Console.WriteLine();

            return actor.ActorId;
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
