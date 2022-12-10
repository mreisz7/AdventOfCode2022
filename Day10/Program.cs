// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

List<int> SignalStrengthList = new();

int cycleNumber = 0;
int registerValue = 1;

Dictionary<int, int> registerValuesByCycle = new();

foreach (string line in inputData)
{
    // Start the cycle
    cycleNumber++;
    // Get the value during the cycle
    registerValuesByCycle.Add(cycleNumber, registerValue);
    if (line.Trim() == "noop")
    {
        // End the cycle
        continue;
    }
    else if (line.StartsWith("addx"))
    {
        cycleNumber++;
        // Get the value during the first cycle of the addx 2 part cycle
        registerValuesByCycle.Add(cycleNumber, registerValue);

        // Increment the registerValue at the end of the second addx 2 part cycle
        int registerValueToAdd = int.Parse(line.Split(' ')[1]);
        registerValue += registerValueToAdd;
    }
}

int totalSignalStrength = registerValuesByCycle.Where(x => ((x.Key + 20) % 40 == 0)).Sum(x => x.Key * x.Value);

Console.WriteLine($"Challenge 1 Answer: {totalSignalStrength}");

Console.WriteLine("Challenge 2 Answer: ");

foreach (int cycleNum in registerValuesByCycle.Keys.Order())
{
    int spritePosition = registerValuesByCycle[cycleNum] + 1;
    int cycleNumByRow = (cycleNum % 40);
    if (Math.Abs(spritePosition - cycleNumByRow) <= 1)
    {
        Console.Write("#");
    }
    else
    {
        Console.Write(" ");
    }

    // Add line breaks
    if (cycleNum % 40 == 0)
        Console.WriteLine();
}