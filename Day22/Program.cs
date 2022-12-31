using System.Text.RegularExpressions;

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

int Challenge1Answer = CalculatePassword(map.CurrentCoordinate.Row, map.CurrentCoordinate.Column, (int)map.CurrentDirection);

Console.WriteLine($"Challenge 1 Answer: {Challenge1Answer}");

Console.WriteLine($"Challenge 2 Answer: {SolveChallenge2(inputData)}");

int SolveChallenge2(string[] input)
{
    // Extract the map portion of the input
    string[] mapInput = input[0..^2];

    // Extract the instructions from the last line of input
    MatchCollection instructionPattern = Regex.Matches(input.Last(), @"(\d+)([RL]?)");
    List<(int steps, string rotateTo)> instructions = instructionPattern.Select(x => (int.Parse(x.Groups[1].Value), x.Groups[2].Value)).ToList();

    // Calculate the length of each face
    int faceSize = (int)Math.Sqrt(mapInput.Sum(x => x.Where(c => c != ' ').Count()) / 6);

    // Map out the adjacent faces of each face
    Dictionary<Face, Face[]> adjacentFaces = new()
    {
        { Face.Front,  new[] { Face.Right, Face.Bottom, Face.Left,  Face.Top   } },
        { Face.Back,   new[] { Face.Left,  Face.Bottom, Face.Right, Face.Top   } },
        { Face.Left,   new[] { Face.Front, Face.Bottom, Face.Back,  Face.Top   } },
        { Face.Right,  new[] { Face.Back,  Face.Bottom, Face.Front, Face.Top   } },
        { Face.Top,    new[] { Face.Right, Face.Front,  Face.Left,  Face.Back  } },
        { Face.Bottom, new[] { Face.Right, Face.Back,   Face.Left,  Face.Front } },
    };

    Dictionary<Face, int> faceOffset = new();
    Dictionary<Face, (int X, int Y)> faceSegment = new();
    Dictionary<(int X, int Y), Dictionary<(int X, int Y), bool>> faces = new();

    Face currentFace = Face.Front;
    (int X, int Y) currentPosition = (X: int.MinValue, Y: int.MinValue);
    (int X, int Y)[] directions = new (int X, int Y)[]
    {
        (1, 0),
        (0, 1),
        (-1, 0),
        (0, -1),
    };
    int directionIndex = 0;

    // Loop through each face group
    for (int faceRow = 0; faceRow < mapInput.Length / faceSize; faceRow++)
    {
        int firstRowOfFace = faceRow * faceSize;
        for (int faceColumn = 0; faceColumn < mapInput[firstRowOfFace].Length / faceSize; faceColumn++)
        {
            int firstColumnOfFace = faceColumn * faceSize;
            (int, int) cubeFace = (faceColumn, faceRow);
            faces[cubeFace] = new();
            for (int y = 0; y < faceSize; y++)
            {
                string mapRow = mapInput[firstRowOfFace + y];
                for (int x = 0; x < faceSize; x++)
                {
                    char mapCell = mapRow[firstColumnOfFace + x];
                    if (mapCell == ' ')
                    {
                        continue;
                    }
                    (int X, int Y) point = (X: x, Y: y);
                    switch (mapCell)
                    {
                        case '#':
                            faces[cubeFace][point] = false;
                            break;
                        case '.':
                            faces[cubeFace][point] = true;
                            if (currentPosition == (int.MinValue, int.MinValue))
                                currentPosition = (point);
                            break;
                        default:
                            continue;
                    }
                }
            }

            // If there weren't any cells in the face, then clean up the faces dictionary before moving on
            if (faces[cubeFace].Count == 0)
                faces.Remove(cubeFace);
        }
    }

    Queue<((int X, int Y) cubeFace, Face face, int fromDirection, Face fromFace)> queue = new();
    HashSet<(int X, int Y)> visited = new();
    visited.Add(faces.Keys.First());
    queue.Enqueue((visited.First(), Face.Front, 1, Face.Top));

    while (queue.Any())
    {
        ((int X, int Y) cubeFace, Face face, int fromDirection, Face fromFace) current = queue.Dequeue();
        faceSegment[current.face] = current.cubeFace;
        int relativeFrom = current.fromDirection + 2 % 4;
        int offset = (4 + relativeFrom - Array.IndexOf(adjacentFaces[current.face], current.fromFace)) % 4;
        faceOffset[current.face] = offset;

        for (int i = 0; i < 4; i++)
        {
            (int X, int Y) direction = directions[i];
            (int, int) targetCubeFace = (current.cubeFace.X + direction.X, current.cubeFace.Y + direction.Y);

            if (faces.ContainsKey(targetCubeFace) && !visited.Contains(targetCubeFace))
            {
                visited.Add(targetCubeFace);
                queue.Enqueue((targetCubeFace, adjacentFaces[current.face][(4 + i - offset) % 4], i, current.face));
            }
        }
    }

    // Now follow the instructions
    foreach ((int steps, string rotateTo) in instructions)
    {
        for (int i = 0; i < steps; i++)
        {
            (int X, int Y) direction = directions[directionIndex];
            (int X, int Y) newPosition = (X: currentPosition.X + direction.X, Y: currentPosition.Y + direction.Y);
            int newDirectionIndex = directionIndex;
            Face newFace = currentFace;

            // If the new position isn't on the current face, then find out how to translate
            if (!faces[faceSegment[currentFace]].TryGetValue(newPosition, out bool valid))
            {
                newFace = adjacentFaces[currentFace][(4 + directionIndex - faceOffset[currentFace]) % 4];
                newPosition = currentPosition;
                int relativeFrom = (directionIndex + 2) % 4;
                int positionOffset = (4 + Array.IndexOf(adjacentFaces[newFace], currentFace) - relativeFrom) % 4;
                int offset = faceOffset[newFace];
                int rotations = (positionOffset + offset) % 4;

                for (int j = 0; j < rotations; j++)
                {
                    newDirectionIndex = (newDirectionIndex + 1) % 4;
                    newPosition = (faceSize - 1 - newPosition.Y, newPosition.X);
                }

                newPosition = newDirectionIndex switch
                {
                    0 => (0, newPosition.Y),
                    1 => (newPosition.X, 0),
                    2 => (faceSize - 1, newPosition.Y),
                    3 => (newPosition.X, faceSize - 1),
                    _ => throw new NotImplementedException("An unexpected value was provided.")
                };

                valid = faces[faceSegment[newFace]][newPosition];
            }

            if (!valid)
            {
                break;
            }

            currentPosition = newPosition;
            currentFace = newFace;
            directionIndex = newDirectionIndex;
        }

        switch (rotateTo)
        {
            case "R":
                directionIndex = (directionIndex + 1) % 4;
                break;
            case "L":
                directionIndex = (directionIndex + 3) % 4;
                break;
            default:
                break;
        }
    }

    // Calculate the answer
    (int xSegment, int ySegment) = faceSegment[currentFace];
    int column = xSegment * faceSize + currentPosition.X + 1;
    int row = ySegment * faceSize + currentPosition.Y + 1;

    return CalculatePassword(row, column, directionIndex);
}

int CalculatePassword(int row, int column, int direction)
{
    return (1000 * row) + (4 * column) + direction;
}

class Map
{
    public HashSet<MapCoordinate> Coordinates { get; set; } = new();

    public MapCoordinate? CurrentCoordinate { get; set; } = null;

    public MapCoordinate? StartingCoordinate { get; set; } = null;

    public Direction CurrentDirection { get; private set; } = Direction.Right;

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

enum Face
{
    Front,
    Back,
    Top,
    Bottom,
    Left,
    Right,
}

enum Direction
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3,
}