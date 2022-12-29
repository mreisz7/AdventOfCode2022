// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
using System.Linq;

string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Dictionary<int, List<((int x, int y) coordinates, Direction direction)>> stormPredictor = new() { { 0, new()} };
HashSet<(int x, int y)> boundingBox = new();
(int x, int y) startingCoordinate = (-1, -1);
(int x, int y) endingCoordinate = (-1, -1);

// Read in the input
for (int y = 0; y < inputData.Length; y++)
{
    for (int x = 0; x < inputData[y].Length; x++)
    {
        // Capture the starting location
        if (y == 0 && inputData[y][x] == '.')
        {
            startingCoordinate = (x, y);
        }

        // Capture the ending location
        if (y == inputData.Length - 1 && inputData[y][x] == '.')
        {
            endingCoordinate = (x, y);
        }

        // Capture the storm location and directions
        switch (inputData[y][x])
        {
            case '^':
                stormPredictor[0].Add(((x, y), Direction.Up));
                break;
            case 'v':
                stormPredictor[0].Add(((x, y), Direction.Down));
                break;
            case '<':
                stormPredictor[0].Add(((x, y), Direction.Left));
                break;
            case '>':
                stormPredictor[0].Add(((x, y), Direction.Right));
                break;
            case '#':
                boundingBox.Add((x, y));
                break;
        }
    }
}

// Calculate bounding box min/max
int minX = boundingBox.Min(c => c.x);
int maxX = boundingBox.Max(c => c.x);
int minY = boundingBox.Min(c => c.y);
int maxY = boundingBox.Max(c => c.y);

// Predict the weather
for (int i = 1; i <= 1000; i++)
{
    stormPredictor.Add(i, new());
    foreach (((int x, int y) coordinates, Direction direction) in stormPredictor[i - 1])
    {
        switch (direction)
        {
            case Direction.Up:
                if (coordinates.y - 1 > minY)
                {
                    stormPredictor[i].Add(((coordinates.x, coordinates.y - 1), Direction.Up));
                } else
                {
                    stormPredictor[i].Add(((coordinates.x, maxY - 1), Direction.Up));
                }
                break;
            case Direction.Down:
                if (coordinates.y + 1 < maxY)
                {
                    stormPredictor[i].Add(((coordinates.x, coordinates.y + 1), Direction.Down));
                }
                else
                {
                    stormPredictor[i].Add(((coordinates.x, minY + 1), Direction.Down));
                }
                break;
            case Direction.Left:
                if (coordinates.x - 1 > minX)
                {
                    stormPredictor[i].Add(((coordinates.x - 1, coordinates.y), Direction.Left));
                }
                else
                {
                    stormPredictor[i].Add(((maxX - 1, coordinates.y), Direction.Left));
                }
                break;
            case Direction.Right:
                if (coordinates.x + 1 < maxX)
                {
                    stormPredictor[i].Add(((coordinates.x + 1, coordinates.y), Direction.Right));
                }
                else
                {
                    stormPredictor[i].Add(((minX + 1, coordinates.y), Direction.Right));
                }
                break;
        }
    }
    if (stormPredictor[i].Select(s => s.coordinates).Intersect(boundingBox).Count() > 0)
        throw new Exception("Uh Oh! Something went wrong.  :(");
}

// Now that we've predicted the storm paths, simplify for lookups during the path finding
Dictionary<int, HashSet<(int x, int y)>> stormLocation = new();
foreach (int turn in stormPredictor.Keys)
{
    stormLocation.Add(turn, stormPredictor[turn].Select(x => x.coordinates).ToHashSet());
}

Stack<((int x, int y) coordinate, int turn)> hikingQueue = new();
hikingQueue.Push((startingCoordinate, 0));
int minimumNumberOfTurns = int.MaxValue;
HashSet<((int x, int y) coordinate, int turn)> visited = new();

while (hikingQueue.Count > 0)
{
    ((int x, int y) coordinate, int turn) = hikingQueue.Pop();
    visited.Add((coordinate, turn));
    int nextTurn = turn + 1;

    if (coordinate == endingCoordinate && turn < minimumNumberOfTurns)
    {
        minimumNumberOfTurns = turn;
        continue;
    }

    // If this has taken longer than an already established minimum then don't go any further
    if (turn > minimumNumberOfTurns)
    {
        continue;
    }

    // If this path takes longer than 1,000 turns then don't bother continuing with this path
    if (nextTurn > stormPredictor.Keys.Count - 1)
    {
        continue;
    }

    // Wait a turn
    if (!stormLocation[nextTurn].Contains(coordinate) && !visited.Contains((coordinate, nextTurn)))
    {
        hikingQueue.Push((coordinate, nextTurn));
    }

    // Move up
    (int x, int y) upCoordinate = (coordinate.x, coordinate.y - 1);
    if (!stormLocation[nextTurn].Contains(upCoordinate) && !boundingBox.Contains(upCoordinate) && upCoordinate.y > 0 && !visited.Contains((upCoordinate, nextTurn)))
    {
        hikingQueue.Push((upCoordinate, nextTurn));
    }

    // Move down
    (int x, int y) downCoordinate = (coordinate.x, coordinate.y + 1);
    if (!stormLocation[nextTurn].Contains(downCoordinate) && !boundingBox.Contains(downCoordinate) && !visited.Contains((downCoordinate, nextTurn)))
    {
        hikingQueue.Push((downCoordinate, nextTurn));
    }

    // Move left
    (int x, int y) leftCoordinate = (coordinate.x - 1, coordinate.y);
    if (!stormLocation[nextTurn].Contains(leftCoordinate) && !boundingBox.Contains(leftCoordinate) && !visited.Contains((leftCoordinate, nextTurn)))
    {
        hikingQueue.Push((leftCoordinate, nextTurn));
    }

    // Move right
    (int x, int y) rightCoordinate = (coordinate.x + 1, coordinate.y);
    if (!stormLocation[nextTurn].Contains(rightCoordinate) && !boundingBox.Contains(rightCoordinate) && !visited.Contains((rightCoordinate, nextTurn)))
    {
        hikingQueue.Push((rightCoordinate, nextTurn));
    }
}

Console.WriteLine($"Challenge 1 Answer: {minimumNumberOfTurns}");

enum Direction
{
    Up,
    Down,
    Left,
    Right,
}