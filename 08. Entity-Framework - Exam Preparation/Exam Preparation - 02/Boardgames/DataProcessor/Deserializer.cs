namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.DataProcessor.ImportDto;
    using Boardgames.Helper;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            var deserializedObjects = XmlSerializationHelper
                .Deserialize<ImportCreatorDto[]>(xmlString, "Creators");

            ICollection<Creator> creatorsToImport = new HashSet<Creator>();

            var sb = new StringBuilder();

            foreach (var dto in deserializedObjects)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Creator newCreator = new Creator()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                };

                foreach (var game in dto.Boardgames)
                {
                    if (!IsValid(game))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Boardgame newGame = new Boardgame()
                    {
                        Name = game.Name,
                        Rating = game.Rating,
                        YearPublished = game.YearPublished,
                        CategoryType = (CategoryType)game.CategoryType,
                        Mechanics = game.Mechanics,
                        Creator = newCreator,
                        CreatorId = newCreator.Id,
                    };

                    newCreator.Boardgames.Add(newGame);
                }

                sb.AppendLine(String
                    .Format(SuccessfullyImportedCreator
                    , newCreator.FirstName, newCreator.LastName, newCreator.Boardgames.Count()));

                creatorsToImport.Add(newCreator);
            }

            context.Creators.AddRange(creatorsToImport);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            var sellers = JsonConvert.DeserializeObject<ImportSellerDto[]>(jsonString);

            var sb = new StringBuilder();

            List<Seller> sellersToImport = new List<Seller>();

            List<int> activeBoardgames = context.Boardgames
                .Select(b => b.Id)
                .ToList();

            foreach (var dto in sellers)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Seller newSeller = new Seller() 
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    Country = dto.Country,
                    Website = dto.Website
                };

                foreach (var gameId in dto.BoardgamesIds.Distinct())
                {
                    if (!activeBoardgames.Contains(gameId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    BoardgameSeller boardGameSeller = new BoardgameSeller()
                    {
                        Seller = newSeller,
                        BoardgameId = gameId,
                    };

                    newSeller.BoardgamesSellers.Add(boardGameSeller);
                }

                sellersToImport.Add(newSeller);
                sb.AppendLine(String.Format(SuccessfullyImportedSeller, newSeller.Name, newSeller.BoardgamesSellers.Count()));
            }

                context.Sellers.AddRange(sellersToImport);
                context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
