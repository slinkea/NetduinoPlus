using System;
using System.Collections.Generic;

namespace BookShop
{
    public class DataBank
    {
        private readonly static List<string> FirstNames = new List<string>() { "Sergio", "Daniel", "Carolina", "David", "Reina", "Saul", "Bernard", "Danny", "Dimas", "Yuri", "Ivan", "Laura" };
        private readonly static List<string> LastNamesA = new List<string>() { "Tapia", "Gutierrez", "Rueda", "Galviz", "Yuli", "Rivera", "Mamami", "Saucedo", "Dominguez", "Escobar", "Martin", "Crespo" };
        private readonly static List<string> LastNamesB = new List<string>() { "Johnson", "Williams", "Jones", "Brown", "David", "Miller", "Wilson", "Anderson", "Thomas", "Jackson", "White", "Robinson" };
        private readonly static List<string> FirstBookName = new List<string>() { "The", "One", "In a", "After the", "And another", "Really", "Seven", "From", "Forcefully", "Cleverly", "Created" };
        private readonly static List<string> SecondBookName = new List<string>() { "Hated", "Scared", "Feared", "Loved", "Entertaining"};
        private readonly static List<string> ThirdBookName = new List<string>() { "Bird", "Man", "Woman", "Hero", "Cat" };

        public static List<string> GenerateClientNames()
        {
            var permutations = new List<Tuple<int, int, int>>();
            List<string> generatedNames = new List<string>();

            Random random = new Random();
            int a, b, c;

            //We want to generate 50 names.
            while (permutations.Count < 50)
            {
                a = random.Next(0, FirstNames.Count);
                b = random.Next(0, FirstNames.Count);
                c = random.Next(0, FirstNames.Count);

                Tuple<int, int, int> tuple = new Tuple<int, int, int>(a, b, c);

                if (!permutations.Contains(tuple))
                {
                    permutations.Add(tuple);
                }
            }

            foreach (var tuple in permutations)
            {
                generatedNames.Add(string.Format("{0} {1} {2}", FirstNames[tuple.Item1],
                                                                LastNamesA[tuple.Item2],
                                                                LastNamesB[tuple.Item3])
                );
            }

            return generatedNames;
        }

        public static List<Book> GenerateBooks()
        {
            var permutations = new List<Tuple<int, int, int>>();
            List<Book> generatedBooks = new List<Book>();

            Random random = new Random();
            int a, b, c;

            //We want to generate 50 names.
            while (permutations.Count < 50)
            {
                a = random.Next(0, FirstBookName.Count);
                b = random.Next(0, SecondBookName.Count);
                c = random.Next(0, ThirdBookName.Count);

                Tuple<int, int, int> tuple = new Tuple<int, int, int>(a, b, c);

                if (!permutations.Contains(tuple))
                {
                    permutations.Add(tuple);
                }
            }
            Random rnd = new Random(Environment.TickCount);
            foreach (var tuple in permutations)
            {
                generatedBooks.Add(new Book(string.Format("{0} {1} {2}", FirstBookName[tuple.Item1],
                                                                SecondBookName[tuple.Item2],
                                                                ThirdBookName[tuple.Item3]), (decimal)Math.Round(rnd.NextDouble() * (10D - 1D) + 1D, 2)));
            }

            return generatedBooks;
        }
    }
}