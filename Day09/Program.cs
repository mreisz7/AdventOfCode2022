// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Rope rope = new();

foreach (string line in inputData)
{
    (char direction, int steps) instruction = (line[0], int.Parse(line.Split(" ")[1]));
    rope.MoveHead(instruction.direction, instruction.steps);
}

HashSet<string> distinctTailSteps = new(rope.TailSteps);

Console.WriteLine($"Challenge 1 Answer: {distinctTailSteps.Count}");

class Rope
{
    public Coordinates Head { get; set; } = new Coordinates(0, 0);
    public Coordinates Tail { get; set; } = new Coordinates(0, 0);

    public List<string> HeadSteps { get; set; } = new();
    public List<string> TailSteps { get; set; } = new();

    public void MoveHead(char direction, int steps)
    {
        Coordinates tempHead = new();
        for (int i = 0; i < steps; i++)
        {
            tempHead.SetCoordinates(Head);
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
            HeadSteps.Add(Head.Coords);
            if (!TailWithinPlankOfHead)
            {
                Tail.SetCoordinates(tempHead);
                TailSteps.Add(Tail.Coords);
            }
        }
    }

    public bool TailWithinPlankOfHead => (Math.Abs(Tail.X - Head.X) <= 1 && Math.Abs(Tail.Y - Head.Y) <= 1);

    public Rope()
    {
        HeadSteps.Add(Head.Coords);
        TailSteps.Add(Tail.Coords);
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
