namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Boardgames.Helper;
    using Newtonsoft.Json;
    using System.CodeDom.Compiler;
    using System.Text.Json;
    using System.Xml;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            var creatorsToExport = context.Creators
                .Where(c => c.Boardgames.Any())
                .Select(c => new ExportCreatorDto()
                {
                    BoardGameCount = c.Boardgames.Count(),
                    CreatorName = $"{c.FirstName} {c.LastName}",
                    Boardgames = c.Boardgames
                        .Select(bg => new ExportBoardgameDto()
                        {
                            BoardgameName = bg.Name,
                            BoardgameYearPublished = bg.YearPublished,
                        })
                        .ToArray()
                        .OrderBy(bg => bg.BoardgameName)
                        .ToArray()
                })
                .ToArray()
                .OrderByDescending(c => c.BoardGameCount)
                .ThenBy(c => c.CreatorName)
                .ToArray();

            return XmlSerializationHelper.Serialize(creatorsToExport, "Creators", false);
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellers = context.Sellers
                .Where(s => s.BoardgamesSellers.Any(bg => bg.Boardgame.YearPublished >= year))
                .Where(s => s.BoardgamesSellers.Any(bg => bg.Boardgame.Rating <= rating))
                .Select(s => new
                {
                    s.Name,
                    s.Website,
                    Boardgames = s.BoardgamesSellers
                    .Where(s => s.Boardgame.YearPublished >= year &&
                     s.Boardgame.Rating <= rating)
                        .OrderByDescending(bg => bg.Boardgame.Rating)
                        .ThenBy(bg => bg.Boardgame.Name)
                        .Select(bg => new
                        {
                            bg.Boardgame.Name,
                            bg.Boardgame.Rating,
                            bg.Boardgame.Mechanics,
                            Category = bg.Boardgame.CategoryType.ToString(),
                        })
                        .ToArray()
                })
                .OrderByDescending(s => s.Boardgames.Count())
                .ThenBy(s => s.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(sellers, Newtonsoft.Json.Formatting.Indented);
        }
    }
}