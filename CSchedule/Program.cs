using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CSchedule
{
    internal class Program
    {
        public static DateTime DateInput(string title)
        {
            DateTime dateTime;
            Console.WriteLine("{0}\nEnter memo time(dd/mm/yyyy h:m):",title);
            while (!DateTime.TryParseExact(Console.ReadLine(), new []
            {
                "dd/MM/yyyy H:m","dd/MM/yyyy H:mm","d/MM/yyyy H:m","d/MM/yyyy H:mm",
                "dd/MM/yyyy HH:m","dd/MM/yyyy HH:mm","d/MM/yyyy HH:m","d/MM/yyyy HqH:mm"
            },CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                Console.Clear();
                Console.WriteLine("{0}\nEnter valid time.\nEnter memo time(dd/mm/yyyy h:m):",title);
            }

            return dateTime;
        }

        public static void Post(Schedule schedule)
        {
            if (!schedule.IsEmpty())
            {
                var ses = schedule.TextList();
                Console.WriteLine(ses[0]);
                Console.WriteLine("_____");
                
                foreach (var s in ses.Skip(1))
                {
                    Console.WriteLine("\n{0}",s);
                    Console.WriteLine("_____");
                }
            }
        }
        
        public static void Main(string[] args)
        {
            var schedule = new Schedule();
            var flag = true;
            while (flag)
            {
                Console.WriteLine("Press first letter of the command to perform it.\nOpen|Add|Delete|Edit|Quit\n");
                Post(schedule);
                DateTime dateTime;
                string name;
                StringBuilder content;
                ConsoleKeyInfo input;
                
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.A:
                        Console.Clear();
                        dateTime = DateInput("Add");

                        Console.Clear();
                        Console.WriteLine("Add\nEnter memo title:");
                        name = Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine("Add\nEnter memo(Esc to finish):");
                        content = new StringBuilder();
                        while ((input = Console.ReadKey()).Key != ConsoleKey.Escape)
                        {
                            content.Append(input.KeyChar);
                        }
                        schedule.Add(name, content.ToString(), dateTime);
                        break;
                    case ConsoleKey.D:
                        Console.Clear();
                        dateTime = DateInput("Delete");
                        schedule.Remove(dateTime);
                        break;
                    case ConsoleKey.E:
                        Console.Clear();
                        Post(schedule);
                        dateTime = DateInput("\nEdit");
                        
                        Console.Clear();
                        Console.WriteLine(schedule.Get(dateTime));
                        Console.WriteLine("\nEdit\nEnter memo title:");
                        name = Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine(schedule.Get(dateTime));
                        Console.WriteLine("\nEdit\nEnter memo(Esc to finish):");
                        content = new StringBuilder();
                        while ((input = Console.ReadKey()).Key != ConsoleKey.Escape)
                        {
                            content.Append(input.KeyChar);
                        }
                        schedule.Edit(name, content.ToString(), dateTime);
                        break;
                    case ConsoleKey.O:
                        Console.Clear();
                        Console.WriteLine("Open\nEnter file name:");
                        schedule.Load(Console.ReadLine());
                        break;
                    case ConsoleKey.Q:
                        Console.Clear();
                        Console.WriteLine("Enter save file name:");
                        File.Copy("temp.sch",Console.ReadLine()+".sch",true);
                        flag = false;
                        break;
                }
                Console.Clear();
            }
        }
    }
}