// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Dictionary<(int x, int y), int> gridSquares = new();

(int x, int y) startingGridSquare = (-1, -1);
(int x, int y) destinationGridSquare = (-1, -1);


for (int x = 0; x < inputData[0].Length; x++)
{
    for (int y = 0; y < inputData.Length; y++)
    {
        (int x, int y) coordinate = (x, y);
        gridSquares.Add(coordinate, GetGridSquareHeight(inputData[y][x]));
        if (inputData[y][x] == 'S')
            startingGridSquare = coordinate;
        if (inputData[y][x] == 'E')
            destinationGridSquare = coordinate;
    }
}

int CalculateShortestPath((int x, int y) startingPoint)
{
    Queue<(int x, int y, int distance)> bfsQueue = new();
    HashSet<(int x, int y)> visitedSquares = new();
    int shortestPath = -1;

    // Enqueue the starting point
    bfsQueue.Enqueue((startingPoint.x, startingPoint.y, 0));
    visitedSquares.Add((startingPoint.x, startingPoint.y));

    while (bfsQueue.Any() && shortestPath < 0)
    {
        (int x, int y, int distance) lastGridSquare = bfsQueue.Dequeue();

        CheckNewPath(lastGridSquare, "up");
        CheckNewPath(lastGridSquare, "down");
        CheckNewPath(lastGridSquare, "left");
        CheckNewPath(lastGridSquare, "right");
    }


    void CheckNewPath((int x, int y, int distance) lastGridSquare, string direction)
    {
        (int x, int y, int distance) nextGridSquare = lastGridSquare;
        nextGridSquare.distance++;

        switch (direction)
        {
            case "up":
                nextGridSquare.y -= 1;
                break;
            case "down":
                nextGridSquare.y += 1;
                break;
            case "left":
                nextGridSquare.x -= 1;
                break;
            case "right":
                nextGridSquare.x += 1;
                break;
        }

        if (visitedSquares.Contains((nextGridSquare.x, nextGridSquare.y)))
            return;

        if (gridSquares.ContainsKey((nextGridSquare.x, nextGridSquare.y)) &&
            gridSquares[(nextGridSquare.x, nextGridSquare.y)] - gridSquares[(lastGridSquare.x, lastGridSquare.y)] <= 1)
        {
            if ((nextGridSquare.x, nextGridSquare.y) == destinationGridSquare)
            {
                shortestPath = nextGridSquare.distance;
            }
            else
            {
                visitedSquares.Add((nextGridSquare.x, nextGridSquare.y));
                bfsQueue.Enqueue(nextGridSquare);
            }
        }
    }

    return shortestPath;
}
 
Console.WriteLine($"Challenge 1 Answer: {CalculateShortestPath(startingGridSquare)}");

List<int> shortestPathsFromLowlands = new();

foreach ((int x, int y) coordinates in gridSquares.Keys)
{
    if (gridSquares[coordinates] == 0)
    {
        shortestPathsFromLowlands.Add(CalculateShortestPath(coordinates));
    }
}

Console.WriteLine($"Challenge 1 Answer: {shortestPathsFromLowlands.Where(x => x > 0).Min()}");

int GetGridSquareHeight(char gridSquareHeight) => gridSquareHeight switch
{
    'S' => 0,
    'a' => 0,
    'b' => 1,
    'c' => 2,
    'd' => 3,
    'e' => 4,
    'f' => 5,
    'g' => 6,
    'h' => 7,
    'i' => 8,
    'j' => 9,
    'k' => 10,
    'l' => 11,
    'm' => 12,
    'n' => 13,
    'o' => 14,
    'p' => 15,
    'q' => 16,
    'r' => 17,
    's' => 18,
    't' => 19,
    'u' => 20,
    'v' => 21,
    'w' => 22,
    'x' => 23,
    'y' => 24,
    'z' => 25,
    'E' => 25,
    _ => throw new NotImplementedException("Unrecognized character provided for Grid Height!")
};
