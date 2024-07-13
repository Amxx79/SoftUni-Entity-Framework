namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            //DbInitializer.ResetDatabase(context);
            using var context = new BookShopContext();

            //Console.WriteLine(GetMostRecentBooks(context));

            RemoveBooks(context);
        }

        //01

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            Enum.TryParse(command, true, out AgeRestriction ageRestriction);

            var bookTitles = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(x => x.Title)
                .OrderBy(t => t)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //02

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context.Books
                .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold)
                .Select(b => new { b.Title, b.BookId })
                .OrderBy(b => b.BookId)
                .ToArray();

            return string.Join(Environment.NewLine, goldenBooks.Select(b => b.Title));
        }

        //03

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new { b.Title, b.Price })
                .OrderByDescending(b => b.Price)
                .ToArray();

            return string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - ${b.Price:F2}"));
        }

        //04

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var booksNotInYear = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .Select(b => new { b.Title, b.BookId })
                .OrderBy(b => b.BookId)
                .ToArray();


            return string.Join(Environment.NewLine, booksNotInYear.Select(b => b.Title));
        }

        //05

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var booksFromCategory = context.BooksCategories
                .Where(bc => categories.Contains(bc.Category.Name))
                .Select(bc => bc.Book.Title)
                .OrderBy(bc => bc)
                .ToArray();

            return string.Join(Environment.NewLine, booksFromCategory);
        }

        //06

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime dt = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate < dt)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:F2}")
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        //07

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new { a.FirstName, a.LastName })
                .OrderBy(a => a.FirstName)
                .ToArray();

            return string.Join(Environment.NewLine
                , authors.Select(a => $"{a.FirstName} {a.LastName}"));
        }

        //08

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string loweredInput = input.ToLower();

            var books = context.Books
                .Where(b => b.Title.Contains(loweredInput))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        //09
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var loweredInput = input.ToLower();

            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(loweredInput))
                .OrderBy(b => b.BookId)
                .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        //10

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                .Where(b => b.Title.Length >= lengthCheck).Count();
        }

        //11

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorsCopies = context.Authors
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                    Copies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(ac => ac.Copies)
                .ToArray();

            return string.Join(Environment.NewLine,
               authorsCopies.Select(a => $"{a.FirstName} {a.LastName} - {a.Copies}"));
        }

        //12

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categoriesByProfit = context.Categories
                .Select(c => new
                {
                    c.Name,
                    Profit = c.CategoryBooks.Sum(b => b.Book.Price * b.Book.Copies)

                })
                .OrderByDescending(c => c.Profit) 
                .ThenBy(c => c.Name)
                .ToArray();

            return string.Join(Environment.NewLine, categoriesByProfit.Select(a =>
            $"{a.Name} ${a.Profit:F2}"));
        }

        //13

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    c.Name,
                    RecentBooks = c.CategoryBooks
                    .OrderByDescending(b => b.Book.ReleaseDate)
                    .Select(b => $"{b.Book.Title} ({b.Book.ReleaseDate.Value.Year})")
                    .Take(3)
                })
                .OrderBy(c => c.Name)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.Name}");
                foreach (var book in category.RecentBooks)
                {
                    sb.AppendLine(book);
                }
            }

            return sb.ToString().Trim();
        }

        //14

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksToDelete = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            context.RemoveRange(booksToDelete);
            context.SaveChanges();

            return booksToDelete.Length;
        }
    }
}