// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Dictionary<int, Monkey> monkeys = new();

for (int i = 0; i < (inputData.Length + 1) / 7; i++)
{
    int startingIndex = i * 7;
    int endingIndex = startingIndex + 6;

    _ = new Monkey(inputData[startingIndex..endingIndex], monkeys);
}

for (int i = 0; i < 20; i++)
{
    foreach (int monkeyNumber in monkeys.Keys.Order())
    {
        monkeys[monkeyNumber].TakeTurn();
    }
}

long[] MostActiveMonkey = monkeys.Values.OrderByDescending(x => x.ItemsInspected).Take(2).Select(x => x.ItemsInspected).ToArray();
long MonkeyBusinessScore = MostActiveMonkey[0] * MostActiveMonkey[1];

Console.WriteLine($"Challenge 1 Answer: {MonkeyBusinessScore}");

// Create a new Dictionary of Monkeys to complete part 2
Dictionary<int, Monkey> monkeysPart2 = new();

for (int i = 0; i < (inputData.Length + 1) / 7; i++)
{
    int startingIndex = i * 7;
    int endingIndex = startingIndex + 6;

    _ = new Monkey(inputData[startingIndex..endingIndex], monkeysPart2);
}

HashSet<int> distinctDivisors = monkeysPart2.Values.Select(x => x.DivisibleByTest).ToHashSet<int>();
long leastCommonDenominator = distinctDivisors.Aggregate(1, (a, b) => a * b);

// Now do 10,000 rounds of the part 2 inspection
for (int i = 0; i < 10_000; i++)
{
    foreach (int monkeyNumber in monkeysPart2.Keys.Order())
    {
        monkeysPart2[monkeyNumber].TakeTurnPart2(leastCommonDenominator);
    }
}

long[] MostActiveMonkeyPart2 = monkeysPart2.Values.OrderByDescending(x => x.ItemsInspected).Take(2).Select(x => x.ItemsInspected).ToArray();
long MonkeyBusinessScorePart2 = MostActiveMonkeyPart2[0] * MostActiveMonkeyPart2[1];

Console.WriteLine($"Challenge 2 Answer: {MonkeyBusinessScorePart2}");

class Monkey
{
    public int Id { get; set; }

    public Dictionary<int, Monkey> MonkeyGroup { get; init; }

    public Queue<long> Items { get; set; } = new();

    public long ItemsInspected = 0;

    public InspectionOperator InspectionOperator { get; set; }

    public int? InspectionModifier { get; set; }

    public int DivisibleByTest { get; set; }

    public int MonkeyToThrowToOnTrue { get; set; }
    
    public int MonkeyToThrowToOnFalse { get; set; }

    public int NumberOfInspections = 0;

    private static InspectionOperator GetInspectionOperator(char inspectionOperatorCharacter) => inspectionOperatorCharacter switch
    {
        '*' => InspectionOperator.Multiply,
        '/' => InspectionOperator.Divide,
        '+' => InspectionOperator.Add,
        '-' => InspectionOperator.Subtract,
        _ => throw new ArgumentOutOfRangeException($"Unexpected Operator value: {inspectionOperatorCharacter}"),
    };

    public void TakeTurn()
    {
        while (Items.Count > 0)
        {
            long item = Items.Dequeue();
            ItemsInspected++;
            long worryLevel = CalculateWorryLevel(item);
            worryLevel = (long)Math.Floor((float)worryLevel / 3);
            bool isDivisible = worryLevel % DivisibleByTest == 0;
            int monkeyToThrowTo;
            if (isDivisible)
            {
                monkeyToThrowTo = MonkeyToThrowToOnTrue;
            }
            else
            {
                monkeyToThrowTo = MonkeyToThrowToOnFalse;
            }
            MonkeyGroup[monkeyToThrowTo].Items.Enqueue(worryLevel);
        }
    }

    public void TakeTurnPart2(long divisor)
    {
        while (Items.Count > 0)
        {
            long item = Items.Dequeue();
            ItemsInspected++;
            long worryLevel = CalculateWorryLevel(item);
            worryLevel %= divisor;
            bool isDivisible = worryLevel % DivisibleByTest == 0;
            int monkeyToThrowTo;
            if (isDivisible)
            {
                monkeyToThrowTo = MonkeyToThrowToOnTrue;
            }
            else
            {
                monkeyToThrowTo = MonkeyToThrowToOnFalse;
            }
            MonkeyGroup[monkeyToThrowTo].Items.Enqueue(worryLevel);
        }
    }

    public long CalculateWorryLevel(long item)
    {
        long worryLevel = 0;
        long modifier = (InspectionModifier is not null) ? (long)InspectionModifier : item;
        switch (InspectionOperator)
        {
            case InspectionOperator.Multiply:
                worryLevel = item * modifier;
                break;
            case InspectionOperator.Divide:
                worryLevel = item / modifier;
                break;
            case InspectionOperator.Add:
                worryLevel = item + modifier;
                break;
            case InspectionOperator.Subtract:
                worryLevel = item - modifier;
                break;
        }
        return worryLevel;
    }

    public Monkey(string[] initializerStrings, Dictionary<int, Monkey> monkeys)
    {
        if (!initializerStrings[0].StartsWith("Monkey") || initializerStrings.Length != 6)
        {
            throw new Exception("Initialization failed due to invalid input formatting");
        }

        // Save a reference to the dictionary that contains this monkey
        MonkeyGroup = monkeys;

        // Get the monkey Id from line 1
        // Example: "Monkey 0:" -> Id = 0
        Id = int.Parse(initializerStrings[0].Split(' ')[1].Replace(":", string.Empty));

        // Get the list of items (ints) that the monkey is starting with
        // Example: "  Starting items: 79, 98" -> Items = [79, 98]
        string[] ItemsStringArray = initializerStrings[1]["  Starting items: ".Length..].Split(", ");
        Items = new Queue<long>(Array.ConvertAll<string, long>(ItemsStringArray, long.Parse));

        // Get the InspectionOperator and the InspectionModifier
        // Example: "  Operation: new = old * 19" -> InspectionOperator = InspectionOperator.Multiply; InspectionModifier = 19;
        char OperatorCharacter = initializerStrings[2]["  Operation: new = old ".Length..][0];
        InspectionOperator = GetInspectionOperator(OperatorCharacter);
        string inspectionModifier = initializerStrings[2]["  Operation: new = old * ".Length..];
        if (inspectionModifier.Equals("old"))
        {
            InspectionModifier = null;
        }
        else
        {
            InspectionModifier = int.Parse(inspectionModifier);
        }

        // Get the DivisibleByTest value
        // Example: "  Test: divisible by 23" -> DivisibleByTest = 23
        DivisibleByTest = int.Parse(initializerStrings[3]["  Test: divisible by ".Length..]);

        // Find the Id of the Monkey to throw to if the test resolves to true
        // Examples: "    If true: throw to monkey 2" -> MonkeyToThrowToOnTrue = 2
        MonkeyToThrowToOnTrue = int.Parse(initializerStrings[4]["    If true: throw to monkey ".Length..]);

        // Find the Id of the Monkey to throw to if the test resolves to false
        // Examples: "    If false: throw to monkey 3" -> MonkeyToThrowToOnFalse = 3
        MonkeyToThrowToOnFalse = int.Parse(initializerStrings[5]["    If false: throw to monkey ".Length..]);

        // Finally, add this to the Group;
        MonkeyGroup.Add(Id, this);
    }
}

enum InspectionOperator
{
    Multiply,
    Divide,
    Add,
    Subtract,
}
