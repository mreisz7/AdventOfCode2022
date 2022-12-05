using System.Text.RegularExpressions;

// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Dictionary<int, CrateStack> crateStacks = new();
List<(int numberOfCrates, int fromStack, int toStack)> stackingInstructions = new();

// Example string: move 2 from 2 to 5
Regex movePattern = new Regex(@"move (?<numberOfCrates>\d+) from (?<fromStack>\d+) to (?<toStack>\d+)");

// Read the lines backwards to build the stacks in the correct order
for (int i = inputData.Length - 1; i >= 0; i--)
{
    if (inputData[i].TrimStart().StartsWith("["))
    {
        int numberOfStacks = (inputData[i].Length + 1) / 4;
        for (int j = 0; j < numberOfStacks; j++)
        {
            if (inputData[i].ElementAt(j * 4) == '[')
            {
                if (!crateStacks.ContainsKey((j + 1)))
                    crateStacks[(j + 1)] = new CrateStack();
                crateStacks[(j + 1)].PlaceOne(inputData[i].ElementAt((j * 4) + 1));
            }
        }
    }
}

// Now get the instruction sets
foreach (string line in inputData)
{
    if (line.StartsWith("move"))
    {
        Match match = movePattern.Match(line);
        int numberOfCrates = int.Parse(match.Groups["numberOfCrates"].Value);
        int fromStack = int.Parse(match.Groups["fromStack"].Value);
        int toStack = int.Parse(match.Groups["toStack"].Value);
        stackingInstructions.Add((numberOfCrates, fromStack, toStack));
    }
}

foreach ((int numberOfCrates, int fromStack, int toStack) instructionSet in stackingInstructions)
{
    for (int i = 0; i < instructionSet.numberOfCrates; i++)
    {
        // Take the crates one at a time from the "fromStack"
        char crateToMove = crateStacks[instructionSet.fromStack].TakeOne();
        // Place them in the "toStack"
        crateStacks[instructionSet.toStack].PlaceOne(crateToMove);
    }
}

Console.Write("Challenge 1 Answer: ");
for (int i = 0; i < crateStacks.Count; i++)
{
    Console.Write(crateStacks[(i + 1)].SeeTopCrate());
}
Console.WriteLine();

class CrateStack
{
    public Stack<char> crates = new();

    public char TakeOne()
    {
        return crates.Pop();
    }

    public void PlaceOne(char crate)
    {
        crates.Push(crate);
    }

    public char SeeTopCrate()
    {
        return crates.Peek();
    }
}
