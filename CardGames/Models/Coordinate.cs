using CardGames.Interfaces;
namespace CardGames.Models
{
    public class Coordinate : ICoordinate
    {
        public string CoordinateName { get; init; }
        public int X { get; init; }
        public int Y { get; init; }

        public Coordinate(string coordinateName, int x, int y){
            this.CoordinateName = coordinateName;
            this.X = x;
            this.Y = y;
        }
    }
}