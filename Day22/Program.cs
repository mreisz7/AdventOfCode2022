// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Map map = new Map();

for (int row = 1; row <= inputData.Length - 2; row++)
{
    for (int column = 1; column <= inputData[row - 1].Length; column++)
    {
        bool isWall = inputData[row - 1][column - 1] == '#';
        bool isCoord = inputData[row - 1][column - 1] == '.';
        if (isWall || isCoord)
        {
            MapCoordinate _ = new MapCoordinate(row, column, isWall, map); 
        }
    }
}

foreach (MapCoordinate mapCoordinate in map.Coordinates)
{
    mapCoordinate.MapAdjacentCoordinates();
}

List<(int steps, char? turnDirection)> directions = new();
string instructionSet = inputData[inputData.Length - 1];

for(int i = 0; i < instructionSet.Length; i++)
{
    string integerString = "";

    while (i < instructionSet.Length && instructionSet[i] != 'R' && instructionSet[i] != 'L')
    {
        if (int.TryParse(instructionSet[i].ToString(), out int _))
        {
            integerString += instructionSet[i].ToString();
            i++;
        }
    }

    int numberOfSteps = int.Parse(integerString);
    char? direction = (i < instructionSet.Length && (instructionSet[i] == 'L' || instructionSet[i] == 'R')) ? instructionSet[i] : null;

    directions.Add((numberOfSteps, direction));
}

foreach ((int steps, char? turnDirection) direction in directions)
{
    for (int i = 0; i < direction.steps; i++)
    {
        map.CurrentCoordinate = map.CurrentCoordinate.GetNextCoordinate();
    }
    if (direction.turnDirection is not null)
    {
        map.SetNewDirection((char)direction.turnDirection);
    }
}

int Challenge1Answer = map.DecodePassword();

Console.WriteLine($"Challenge 1 Answer: {Challenge1Answer}");

class Map
{
    public HashSet<MapCoordinate> Coordinates { get; set; } = new();

    public MapCoordinate? CurrentCoordinate { get; set; } = null;

    public MapCoordinate? StartingCoordinate { get; set; } = null;

    public Direction CurrentDirection { get; private set; } = Direction.Right;

    public void ResetMap()
    {
        CurrentCoordinate = StartingCoordinate;
        CurrentDirection = Direction.Right;
    }

    public int DecodePassword()
    {
        if (CurrentCoordinate is not null)
        {
            int rowComponent = 1000 * CurrentCoordinate.Row;
            int columnComponent = 4 * CurrentCoordinate.Column;
            int headingComponent = 0;

            switch (CurrentDirection)
            {
                case Direction.Right:
                    headingComponent = 0;
                    break;
                case Direction.Down:
                    headingComponent = 1;
                    break;
                case Direction.Left:
                    headingComponent = 2;
                    break;
                case Direction.Up:
                    headingComponent = 3;
                    break;
            }

            return rowComponent + columnComponent + headingComponent;
        }

        return int.MinValue;
    }

    public void SetNewDirection(char turnDirection)
    {
        if (turnDirection == 'R')
        {
            switch (CurrentDirection)
            {
                case Direction.Up:
                    CurrentDirection = Direction.Right; break;
                case Direction.Down:
                    CurrentDirection = Direction.Left; break;
                case Direction.Left:
                    CurrentDirection = Direction.Up; break;
                case Direction.Right:
                    CurrentDirection = Direction.Down; break;
            }
        }
        if (turnDirection == 'L')
        {
            switch (CurrentDirection)
            {
                case Direction.Up:
                    CurrentDirection = Direction.Left; break;
                case Direction.Down:
                    CurrentDirection = Direction.Right; break;
                case Direction.Left:
                    CurrentDirection = Direction.Down; break;
                case Direction.Right:
                    CurrentDirection = Direction.Up; break;
            }
        }
    }
}

class MapCoordinate
{
    public int Row { get; init; }

    public int Column { get; init; }

    public bool IsWall { get; init; }

    public Map Map { get; init; }

    public MapCoordinate? UpCoordinate { get; set; } = null;

    public MapCoordinate? DownCoordinate { get; set; } = null;

    public MapCoordinate? LeftCoordinate { get; set; } = null;

    public MapCoordinate? RightCoordinate { get; set; } = null;

    public MapCoordinate? GetNextCoordinate()
    {
        switch (Map.CurrentDirection)
        {
            case Direction.Up:
                if (!UpCoordinate.IsWall)
                    return UpCoordinate;
                break;
            case Direction.Down:
                if (!DownCoordinate.IsWall)
                    return DownCoordinate;
                break;
            case Direction.Left:
                if (!LeftCoordinate.IsWall)
                    return LeftCoordinate;
                break;
            case Direction.Right: 
                if (!RightCoordinate.IsWall)
                    return RightCoordinate;
                break;
            default: throw new NotImplementedException("Something went horribly wrong. :(");
        }
        return this;
    }

    public void MapAdjacentCoordinates()
    {
        // Map the UpCoordinate
        if (Map.Coordinates.Where(c => c.Column == this.Column && c.Row == this.Row - 1).Count() == 0)
        {
            UpCoordinate = Map.Coordinates.Where(c => c.Column == this.Column).MaxBy(c => c.Row);
        }
        else
        {
            UpCoordinate = Map.Coordinates.Where(c => c.Column == this.Column && c.Row == this.Row - 1).First();
        }

        // Map the DownCoordinate
        if (Map.Coordinates.Where(c => c.Column == this.Column && c.Row == this.Row + 1).Count() == 0)
        {
            DownCoordinate = Map.Coordinates.Where(c => c.Column == this.Column).MinBy(c => c.Row);
        }
        else
        {
            DownCoordinate = Map.Coordinates.Where(c => c.Column == this.Column && c.Row == this.Row + 1).First();
        }

        // Map the LeftCoordinate
        if (Map.Coordinates.Where(c => c.Column == this.Column - 1 && c.Row == this.Row).Count() == 0)
        {
            LeftCoordinate = Map.Coordinates.Where(c => c.Row == this.Row).MaxBy(c => c.Column);
        }
        else
        {
            LeftCoordinate = Map.Coordinates.Where(c => c.Column == this.Column - 1 && c.Row == this.Row).First();
        }

        // Map the RightCoordinate
        if (Map.Coordinates.Where(c => c.Column == this.Column + 1 && c.Row == this.Row).Count() == 0)
        {
            RightCoordinate = Map.Coordinates.Where(c => c.Row == this.Row).MinBy(c => c.Column);
        }
        else
        {
            RightCoordinate = Map.Coordinates.Where(c => c.Column == this.Column + 1 && c.Row == this.Row).First();
        }
    }

    public MapCoordinate(int row, int column, bool isWall, Map map)
    {
        Row = row;
        Column = column;
        IsWall = isWall;
        Map = map;
        Map.Coordinates.Add(this);

        // If this is the first coordinate added to the Map, then it should be the starting coordinate
        if (Map.Coordinates.Count == 1)
        {
            Map.CurrentCoordinate = this;
            Map.StartingCoordinate = this;
        }
    }
}

enum Direction
{
    Up,
    Down,
    Left,
    Right,
}