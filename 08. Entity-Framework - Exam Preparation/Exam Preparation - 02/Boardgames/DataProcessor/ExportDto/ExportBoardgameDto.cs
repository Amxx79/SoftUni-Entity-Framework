using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Boardgame")]
    public class ExportBoardgameDto
    {
        [XmlElement]
        public string BoardgameName { get; set; }
        [XmlElement]
        public int BoardgameYearPublished { get; set; }

    }
}