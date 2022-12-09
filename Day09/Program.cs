// Read in the Challenge input

//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test1.txt");
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test2.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Rope rope = new();

foreach (string line in inputData)
{
    (char direction, int steps) instruction = (line[0], int.Parse(line.Split(" ")[1]));
    rope.MoveHead(instruction.direction, instruction.steps);
}

HashSet<string> distinctTailSteps = new(rope.TailSteps?[1]);

Console.WriteLine($"Challenge 1 Answer: {distinctTailSteps.Count}");

HashSet<string> distinctKnot9Steps = new(rope.TailSteps?[9]);

Console.WriteLine($"Challenge 2 Answer: {distinctKnot9Steps.Count}");

class Rope
{
    public Coordinates Head { get; set; } = new Coordinates(0, 0);
    public Coordinates Tail { get; set; } = new Coordinates(0, 0);

    public List<string>? HeadSteps { get; set; } = null;
    public Dictionary<int, List<string>>? TailSteps { get; set; } = null;

    public int Depth { get; init; }
    public Rope RootKnot { get; init; }
    public Rope? ParentKnot { get; init; }
    public Rope? ChildKnot { get; init; } = null;

    public void MoveHead(char direction, int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            switch (direction)
            {
                case 'U':
                    Head.Y += 1;
                    break;
                case 'D':
                    Head.Y -= 1;
                    break;
                case 'R':
                    Head.X += 1;
                    break;
                case 'L':
                    Head.X -= 1;
                    break;
                default:
                    throw new Exception($"Provided direction is not valid ({direction})");
            }
            if (Depth == 1)
            {
                //Console.WriteLine($"Head moved {direction} 1 step");
                RootKnot.HeadSteps?.Add(Head.Coords);
            }
            MoveTail();
        }
    }

    public void MoveTail()
    {
        if (!TailWithinPlankOfHead)
        {
            if (Head.X > Tail.X)
                Tail.X += 1;
            if (Head.X < Tail.X)
                Tail.X -= 1;
            if (Head.Y > Tail.Y)
                Tail.Y += 1;
            if (Head.Y < Tail.Y)
                Tail.Y -= 1;

            // Do a sanity check to make sure that this has put things right before moving on
            if (!TailWithinPlankOfHead) 
                throw new Exception("Something went wrong!");

            RootKnot.TailSteps?[Depth].Add(Tail.Coords);
            ChildKnot?.Head.SetCoordinates(Tail);
            ChildKnot?.MoveTail();
        }
    }

    public bool TailWithinPlankOfHead => (Math.Abs(Tail.X - Head.X) <= 1 && Math.Abs(Tail.Y - Head.Y) <= 1);

    public Rope()
    {
        HeadSteps = new();
        TailSteps = new();

        HeadSteps.Add(Head.Coords);
        for (int i = 1; i <= 9; i++)
        {
            TailSteps.Add(i, new());
        }
        TailSteps[1].Add(Tail.Coords);

        RootKnot = this;
        ParentKnot = null;
        Depth = 1;
        ChildKnot = new(this);
    }

    public Rope(Rope parent)
    {
        Head = new(parent.Tail);
        RootKnot = parent.RootKnot;
        ParentKnot = parent;
        Depth = parent.Depth + 1;

        if (Depth < 9)
        {
            ChildKnot = new(this);
        }
        RootKnot.TailSteps?[Depth].Add(Tail.Coords);
    }
}

record Coordinates
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;

    public void SetCoordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetCoordinates(Coordinates coordinates)
    {
        X = coordinates.X;
        Y = coordinates.Y;
    }

    public string Coords => $"{X}, {Y}";

    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Coordinates(Coordinates coordinates)
    {
        X = coordinates.X;
        Y = coordinates.Y;
    }

    public Coordinates()
    {
        X = 0;
        Y = 0;
    }
}
