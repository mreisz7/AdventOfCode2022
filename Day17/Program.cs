// Read in the Challenge input
//string inputData = File.ReadAllText(@".\ChallengeInput_Test.txt").Trim();
string inputData = File.ReadAllText(@".\ChallengeInput.txt").Trim();

Console.WriteLine($"Challenge 1 Answer: {SolveProblem1(2022)}");

Console.WriteLine($"Challenge 2 Answer: {SolveProblem2()}");

int SolveProblem1(int numberOfCycles)
{
    int result = 0;
    Cavern cavern = new();
    int jetCounter = 0;

    for (int i = 0; i < numberOfCycles; i++)
    {
        Boulder boulder = GetBoulder((i % 5) + 1, 3, cavern.GetTopOfBoulderPile() + 4, cavern);

        //if (i < 10)
        //{
        //    cavern.DrawCavern(boulder);
        //}

        if (i != 0 && i % 100 == 0)
        {
            cavern.ReduceBoulderPileSize();
        }

        bool boulderStillFalling = true;
        while (boulderStillFalling)
        {
            switch (inputData[jetCounter % inputData.Length])
            {
                case '>':
                    boulder.MoveRight();
                    break;
                case '<':
                    boulder.MoveLeft();
                    break;
                default:
                    throw new NotImplementedException("unhandled directional input");
            }
            jetCounter++;


            bool canMoveDown = boulder.MoveDown();

            if (!canMoveDown)
            {
                cavern.AddBoulderToPile(boulder);
                boulderStillFalling = false;
            }
        }
    }

    Console.WriteLine($"Number of Boulders: {cavern.NumberOfBouldersInPile}");
    Console.WriteLine($"Jet Pattern Location: {jetCounter % inputData.Length}");
    result = cavern.GetTopOfBoulderPile();

    return result;
}

long SolveProblem2()
{
    Cavern cavern = new();
    int jetCounter = 0;

    Dictionary<int, List<(int boulderNumber, int bouldersSinceLast, int boulderheight, int boulderHeightDeltaSinceLast)>> pattern = new();

    for (int i = 0; i < 10_000; i++)
    {
        Boulder boulder = GetBoulder((i % 5) + 1, 3, cavern.GetTopOfBoulderPile() + 4, cavern);

        if (i != 0 && i % 100 == 0)
        {
            cavern.ReduceBoulderPileSize();
        }

        if ((i % 5) + 1 == 1)
        {
            int jCounter = jetCounter % inputData.Length;
            int newBoulderHeight = cavern.GetTopOfBoulderPile();
            if (!pattern.ContainsKey(jCounter))
            {
                pattern.Add(jCounter, new());
                pattern[jCounter].Add((i, -1, newBoulderHeight, -1));
            }
            else
            {
                int numberOfBouldersSinceLast = i - pattern[jCounter].Last().boulderNumber;
                int boulderHeightDeltaSinceLast = newBoulderHeight - pattern[jCounter].Last().boulderheight;
                pattern[jCounter].Add((i, numberOfBouldersSinceLast, newBoulderHeight, boulderHeightDeltaSinceLast));
            }
        }

        bool boulderStillFalling = true;
        while (boulderStillFalling)
        {
            switch (inputData[jetCounter % inputData.Length])
            {
                case '>':
                    boulder.MoveRight();
                    break;
                case '<':
                    boulder.MoveLeft();
                    break;
                default:
                    throw new NotImplementedException("unhandled directional input");
            }
            jetCounter++;


            bool canMoveDown = boulder.MoveDown();

            if (!canMoveDown)
            {
                cavern.AddBoulderToPile(boulder);
                boulderStillFalling = false;
            }
        }
    }

    Dictionary<(int patternLength, int pileDelta), int> commonNumbers = new();

    foreach (var p in pattern)
    {
        foreach (var bp in p.Value)
        {
            if (!commonNumbers.ContainsKey((bp.bouldersSinceLast, bp.boulderHeightDeltaSinceLast)))
            {
                commonNumbers.Add((bp.bouldersSinceLast, bp.boulderHeightDeltaSinceLast), 0);
            }
            commonNumbers[(bp.bouldersSinceLast, bp.boulderHeightDeltaSinceLast)]++;
        }
    }

    (int patternLength, int pileDelta) = commonNumbers.OrderByDescending(x => x.Value).First().Key;

    long numberOfCycles = 1_000_000_000_000;
    long numberOfRepetitions = Math.DivRem(numberOfCycles, patternLength, out long remainder);

    (int boulderNumber, int bouldersSinceLast, int boulderHeight, int boulderHeightDelta) frontPad = (-1, -1, -1, -1);

    foreach (int i in pattern.Keys)
    {
        if (pattern[i].Any(x => x.boulderNumber == (int)remainder))
        {
            (frontPad.boulderNumber, _, frontPad.boulderHeight, _) = pattern[i].Where(x => x.boulderNumber == (int)remainder).First();
            break;
        }
    }

    long totalHeightOfPile = frontPad.boulderHeight + (numberOfRepetitions * pileDelta);

    return totalHeightOfPile;
}


Boulder GetBoulder(int cycleNumber, int xOffset, int yOffset, Cavern cavern) => cycleNumber switch
{
    1 => new Boulder1(xOffset, yOffset, cavern),
    2 => new Boulder2(xOffset, yOffset, cavern),
    3 => new Boulder3(xOffset, yOffset, cavern),
    4 => new Boulder4(xOffset, yOffset, cavern),
    5 => new Boulder5(xOffset, yOffset, cavern),
    _ => throw new NotImplementedException(),
};

abstract class Boulder
{
    public abstract HashSet<(int x, int y)> BoulderCoordinates { get; set; }

    public Cavern Cavern { get; init; }

    public (int top, int bottom, int left, int right) GetHorizontalBoundaries()
    {
        int topY = BoulderCoordinates.Select(b => b.y).Min();
        int bottomY = BoulderCoordinates.Select(b => b.y).Max();
        int leftX = BoulderCoordinates.Select(b => b.x).Min();
        int rightX = BoulderCoordinates.Select(b => b.x).Max();
        return (topY, bottomY, leftX, rightX);
    }

    public bool BoulderWillCollide(Direction direction)
    {
        int directionalXOffset = 0;
        int directionalYOffset = 0;
        switch (direction)
        {
            case Direction.Left:
                directionalXOffset = -1;
                break;
            case Direction.Right:
                directionalXOffset = 1;
                break;
            case Direction.Down:
                directionalYOffset = -1;
                break;
            default:
                throw new NotImplementedException("This direction is not currently supported");
        }

        HashSet<(int x, int y)> newBoulderPosition = 
            BoulderCoordinates.Select(b => (b.x + directionalXOffset, b.y + directionalYOffset)).ToHashSet();

        // Check if it will collide with the boulder pile or the floor
        if (newBoulderPosition.Intersect(Cavern.BoulderPile).Count() > 0 
            || newBoulderPosition.Select(b => b.y).Min() == 0)
        {
            return true;
        }

        // Check if it will collide with the wall
        if (newBoulderPosition.Select(b => b.x).Min() == Cavern.LeftWall
            || newBoulderPosition.Select(b => b.x).Max() == Cavern.RightWall)
        {
            return true;
        }

        return false;
    }

    public bool MoveDown()
    {
        if (!BoulderWillCollide(Direction.Down))
        {
            BoulderCoordinates = BoulderCoordinates.Select(b => (b.x, b.y - 1)).ToHashSet();
            return true;
        }
        else
        {
            //Console.WriteLine("Boulder has settled on the pile");
            return false;
        }
    }

    public bool MoveLeft()
    {
        if (!BoulderWillCollide(Direction.Left))
        {
            BoulderCoordinates = BoulderCoordinates.Select(b => (b.x - 1, b.y)).ToHashSet();
            return true;
        }
        else
        {
            //Console.WriteLine("Boulder collided with the left wall");
            return false;
        }
    }

    public bool MoveRight()
    {
        if (!BoulderWillCollide(Direction.Right))
        {
            BoulderCoordinates = BoulderCoordinates.Select(b => (b.x + 1, b.y)).ToHashSet();
            return true;
        }
        else
        {
            //Console.WriteLine("Boulder collided with the right wall");
            return false;
        }
    }

    public Boulder(Cavern cavern)
    {
        Cavern = cavern;
    }
}

class Boulder1 : Boulder
{
    public override HashSet<(int x, int y)> BoulderCoordinates { get; set; }

    public Boulder1(int xOffset, int yOffset, Cavern cavern) 
        : base(cavern)
    {
        BoulderCoordinates = new() { (0, 0), (1, 0), (2, 0), (3, 0) };
        BoulderCoordinates = BoulderCoordinates.Select(b => (b.x + xOffset, b.y + yOffset)).ToHashSet();
    }
}

class Boulder2 : Boulder
{
    public override HashSet<(int x, int y)> BoulderCoordinates { get; set; }

    public Boulder2(int xOffset, int yOffset, Cavern cavern) 
        : base(cavern)
    {
        BoulderCoordinates = new() { (1, 0), (0, 1), (1, 1), (2, 1), (1, 2) };
        BoulderCoordinates = BoulderCoordinates.Select(b => (b.x + xOffset, b.y + yOffset)).ToHashSet();
    }
}

class Boulder3 : Boulder
{
    public override HashSet<(int x, int y)> BoulderCoordinates { get; set; }

    public Boulder3(int xOffset, int yOffset, Cavern cavern) 
        : base(cavern)
    {
        BoulderCoordinates = new() { (0, 0), (1, 0), (2, 0), (2, 1), (2, 2) };
        BoulderCoordinates = BoulderCoordinates.Select(b => (b.x + xOffset, b.y + yOffset)).ToHashSet();
    }
}

class Boulder4 : Boulder
{
    public override HashSet<(int x, int y)> BoulderCoordinates { get; set; }

    public Boulder4(int xOffset, int yOffset, Cavern cavern) 
        : base(cavern)
    {
        BoulderCoordinates = new() { (0, 0), (0, 1), (0, 2), (0, 3) };
        BoulderCoordinates = BoulderCoordinates.Select(b => (b.x + xOffset, b.y + yOffset)).ToHashSet();
    }
}

class Boulder5 : Boulder
{
    public override HashSet<(int x, int y)> BoulderCoordinates { get; set; }

    public Boulder5(int xOffset, int yOffset, Cavern cavern) 
        : base(cavern)
    {
        BoulderCoordinates = new() { (0, 0), (1, 0), (0, 1), (1, 1) };
        BoulderCoordinates = BoulderCoordinates.Select(b => (b.x + xOffset, b.y + yOffset)).ToHashSet();
    }
}

class Cavern
{
    public int LeftWall = 0;

    public int RightWall = 8;

    public int NumberOfBouldersInPile = 0;

    public HashSet<(int x, int y)> BoulderPile { get; set; } = new();

    public void AddBoulderToPile(Boulder boulder)
    {
        BoulderPile = BoulderPile.Union(boulder.BoulderCoordinates).ToHashSet();
        NumberOfBouldersInPile++;
    }

    public void DrawCavern(Boulder boulder)
    {
        int height = GetTopOfBoulderPile() + 8;

        for (int y = height; y >= 1; y--)
        {
            Console.Write($"{y.ToString().PadLeft(3)}: |");
            for (int x = 1; x <= 7; x++)
            {
                if (BoulderPile.Contains((x, y)))
                {
                    Console.Write("#");
                }
                else if (boulder.BoulderCoordinates.Contains((x, y)))
                {
                    Console.Write("@");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.Write("|");
            Console.WriteLine();
        }
        Console.WriteLine("     |-------|");
        Console.WriteLine();
    }

    public void ReduceBoulderPileSize()
    {
        int column1MaxHeight = BoulderPile.Where(b => b.x == 1).Select(b => b.y).Max();
        int column2MaxHeight = BoulderPile.Where(b => b.x == 1).Select(b => b.y).Max();
        int column3MaxHeight = BoulderPile.Where(b => b.x == 1).Select(b => b.y).Max();
        int column4MaxHeight = BoulderPile.Where(b => b.x == 1).Select(b => b.y).Max();
        int column5MaxHeight = BoulderPile.Where(b => b.x == 1).Select(b => b.y).Max();
        int column6MaxHeight = BoulderPile.Where(b => b.x == 1).Select(b => b.y).Max();
        int column7MaxHeight = BoulderPile.Where(b => b.x == 1).Select(b => b.y).Max();

        int[] maxHeightArray = new[] { column1MaxHeight, column2MaxHeight, column3MaxHeight, column4MaxHeight, column5MaxHeight, column6MaxHeight, column7MaxHeight };

        // Get the minimum max height to make the new bottom
        int minMax = maxHeightArray.Min();

        // Now remove everything below this point since everything below is no longer relevant
        BoulderPile.RemoveWhere(b => b.y > minMax + 100);
    }

    public int GetTopOfBoulderPile()
    {
        if (BoulderPile.Count > 0)
            return BoulderPile.Select(b => b.y).Max();
        return 0;
    }
}

enum Direction
{
    Down,
    Left,
    Right,
}