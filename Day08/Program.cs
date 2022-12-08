// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

int xLength = inputData.Length;
int yLength = inputData[0].Length;

int[,] treeGrid = new int[xLength, yLength];
int maxValue = 0;

for (int x = 0; x < xLength; x++)
{
    for (int y = 0; y < yLength; y++)
    {
        _ = int.TryParse(inputData[x][y].ToString(), out treeGrid[x, y]);
        if (treeGrid[x, y] > maxValue)
        {
            maxValue = treeGrid[x, y];
        }
    }
}

HashSet<(int x, int y)> allVisibleTreeCoordinates = new();

allVisibleTreeCoordinates.UnionWith(TraverseTreeline(Direction.Up, maxValue, treeGrid));
allVisibleTreeCoordinates.UnionWith(TraverseTreeline(Direction.Down, maxValue, treeGrid));
allVisibleTreeCoordinates.UnionWith(TraverseTreeline(Direction.Left, maxValue, treeGrid));
allVisibleTreeCoordinates.UnionWith(TraverseTreeline(Direction.Right, maxValue, treeGrid));

Console.WriteLine($"Challenge 1 Answer: {allVisibleTreeCoordinates.Count}");

int bestTreeScenicScore = 0;
for (int x = 0; x < xLength; x++)
{
    for (int y = 0; y < yLength; y++)
    {
        int scenicScore = CalculateScenicScore((x, y), treeGrid);
        if (scenicScore > bestTreeScenicScore)
        {
            bestTreeScenicScore = scenicScore;
        }
    }
}

Console.WriteLine($"Challeng 2 Answer: {bestTreeScenicScore}");

HashSet<(int x, int y)> TraverseTreeline(Direction direction, int maxHeight, int[,] treeGrid)
{
    // Set the default start/end/increment for down and right
    // When the direction is left then it should start at the end and move towards the beginning, for all other cases it should increase
    int xStart      = direction == Direction.Left ? treeGrid.GetLength(0) - 1 : 0;
    int xEnd        = direction == Direction.Left ? 0 : treeGrid.GetLength(0);
    int xIncrement  = direction == Direction.Left ? -1 : 1;
    // When the direction is up then it should start at the end and move towards the beginning, for all other cases it should increase
    int yStart      = direction == Direction.Up ? treeGrid.GetLength(1) - 1 : 0;
    int yEnd        = direction == Direction.Up ? 0 : treeGrid.GetLength(1);
    int yIncrement  = direction == Direction.Up ? -1 : 1;


    // When the direction is vertical (down or up) the inner loop should be x
    // When the direction is horizontal (left or right) the inner loop should be y
    int outerStart     = direction == Direction.Up || direction == Direction.Down ? xStart     : yStart;
    int outerEnd       = direction == Direction.Up || direction == Direction.Down ? xEnd       : yEnd;
    int outerIncrement = direction == Direction.Up || direction == Direction.Down ? xIncrement : yIncrement;
    int innerStart     = direction == Direction.Up || direction == Direction.Down ? yStart     : xStart;
    int innerEnd       = direction == Direction.Up || direction == Direction.Down ? yEnd       : xEnd;
    int innerIncrement = direction == Direction.Up || direction == Direction.Down ? yIncrement : xIncrement;

    // Initialize the empty set that will collect the visibile tree coordinates
    HashSet<(int x, int y)> visibleTreeCoordinates = new();

    for (int i = outerStart; i != outerEnd; i += outerIncrement)
    {
        // vertical (down or up) means i is y and j is x
        int VisibilityHeight = direction == Direction.Up || direction == Direction.Down ? treeGrid[innerStart, i] : treeGrid[i, innerStart];
        for (int j = innerStart; j != innerEnd; j += innerIncrement)
        {
            int treeInQuestion = direction == Direction.Up || direction == Direction.Down ? treeGrid[j, i] : treeGrid[i, j];
            (int x, int y) treeToAdd = direction == Direction.Up || direction == Direction.Down ? (j, i) : (i, j);
            if (treeInQuestion >= VisibilityHeight)
            {
                visibleTreeCoordinates.Add(treeToAdd);
                if (treeInQuestion == maxHeight)
                {
                    break;
                }
                VisibilityHeight = treeInQuestion + 1;
            }
        }
    }
    
    return visibleTreeCoordinates;
}

int CalculateScenicScore((int x, int y) tree, int[,] treeGrid)
{
    int scenicScore = 1;
    int targetTreeHeight = treeGrid[tree.x, tree.y];
    int treeHeightInQuestion;

    if (tree.x == 0 || tree.x == treeGrid.GetLength(0) || tree.y == 0 || tree.y == treeGrid.GetLength(1))
        return 0;

    // Calculate up
    int treesVisibleUp = 0;
    for (int x = tree.x - 1; x >= 0; x--)
    {
        treeHeightInQuestion = treeGrid[x, tree.y];
        if (treeHeightInQuestion < targetTreeHeight)
        {
            treesVisibleUp++;
        }
        if (treeHeightInQuestion >= targetTreeHeight)
        {
            treesVisibleUp++;
            break;
        }
    }
    scenicScore *= treesVisibleUp;

    // Calculate down
    int treesVisibleDown = 0;
    for (int x = tree.x + 1; x < treeGrid.GetLength(0); x++)
    {
        treeHeightInQuestion = treeGrid[x, tree.y];
        if (treeHeightInQuestion < targetTreeHeight)
        {
            treesVisibleDown++;
        }
        if (treeHeightInQuestion >= targetTreeHeight)
        {
            treesVisibleDown++;
            break;
        }
    }
    scenicScore *= treesVisibleDown;

    // Calculate left
    int treesVisibleLeft = 0;
    for (int y = tree.y - 1; y >= 0; y--)
    {
        treeHeightInQuestion = treeGrid[tree.x, y];
        if (treeHeightInQuestion < targetTreeHeight)
        {
            treesVisibleLeft++;
        }
        if (treeHeightInQuestion >= targetTreeHeight)
        {
            treesVisibleLeft++;
            break;
        }
    }
    scenicScore *= treesVisibleLeft;

    // Calculate right
    int treesVisibleRight = 0;
    for (int y = tree.y + 1; y < treeGrid.GetLength(1); y++)
    {
        treeHeightInQuestion = treeGrid[tree.x, y];
        if (treeHeightInQuestion < targetTreeHeight)
        {
            treesVisibleRight++;
        }
        if (treeHeightInQuestion >= targetTreeHeight)
        {
            treesVisibleRight++;
            break;
        }
    }
    scenicScore *= treesVisibleRight;

    return scenicScore;
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}
