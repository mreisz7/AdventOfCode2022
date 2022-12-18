using System.Text.RegularExpressions;

// Read in the Challenge input
string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
//string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Dictionary<string, Valve> valves = new();

foreach (string input in inputData)
{
    _ = new Valve(input, valves);
}

foreach (string valve in valves.Keys)
{
    valves[valve].MapShortestPathToEveryValve();
}

int SolveChallenge1()
{
    string startingPoint = "AA";
    int totalFlowRate = 0;
    int timeRemaining = 30;
    Valve currentValve = valves[startingPoint];

    while (timeRemaining >= 0)
    {


    }


    return totalFlowRate;
}

Console.WriteLine($"Challenge 1 Answer: {SolveChallenge1()}");

partial class Valve
{
    public string Name { get; init; }

    public int FlowRate { get; init; }

    public List<string> AdjacentValves { get; init; }

    Dictionary<string, Valve> Valves { get; init; }

    Dictionary<string, List<string>> ShortestPaths { get; set; } = new();

    public List<string>? GetShortestPathToValve(string valve, List<string> path)
    {
        List<string> newPath = new(path);
        newPath.Add(Name);

        if (valve == Name)
        {
            return newPath;
        }

        if (AdjacentValves.Contains(valve))
        {
            return Valves[valve].GetShortestPathToValve(valve, newPath);
        }

        int shortest = int.MaxValue;
        List<string> shortestPath = new();
        foreach (string possibleValve in AdjacentValves.Except(newPath))
        {
            List<string>? temp = Valves[possibleValve].GetShortestPathToValve(valve, newPath);
            if (temp is not null && temp?.Count < shortest && temp.Last() == valve)
            {
                shortest = temp.Count;
                shortestPath = temp;
            }
        }

        if (shortestPath.Count != 0)
            return shortestPath;

        return null;

        throw new Exception("Couldn't find a path");
    }

    public void MapShortestPathToEveryValve()
    {
        foreach (string valve in Valves.Keys)
        {
            ShortestPaths.Add(valve, GetShortestPathToValve(valve, new List<string>()));
            Console.Write($"Shortest path {Name} to {valve}: ");
            Console.Write(string.Join(" -> ", ShortestPaths[valve]));
            Console.WriteLine();
        }
    }

    public Valve(string valveInputData, Dictionary<string, Valve> dictionary)
    {
        Regex valveInput = MyRegex();
        Match match = valveInput.Match(valveInputData);
        Name = match.Groups["valveName"].Value;
        FlowRate = int.Parse(match.Groups["flowRate"].Value);
        AdjacentValves = match.Groups["additionalValves"].Value.Split(", ").ToList();

        Valves = dictionary;
        Valves.Add(Name, this);
    }

    // Example: Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
    [GeneratedRegex("Valve (?<valveName>[A-Za-z]{2}) has flow rate=(?<flowRate>\\d+); tunnels? leads? to valves? (?<additionalValves>(.*$))")]
    private static partial Regex MyRegex();
}
