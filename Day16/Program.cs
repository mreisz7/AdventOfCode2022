using System.Text.RegularExpressions;

// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Dictionary<string, Valve> valves = new();

foreach (string input in inputData.Order())
{
    _ = new Valve(input, valves);
}

foreach (string valve in valves.Keys)
{
    valves[valve].MapShortestPathToEveryValve();
}

Console.WriteLine($"Challenge 1 Answer: {SolveChallenge1()}");

Console.WriteLine($"Challenge 2 Answer: {SolveChallenge2()}");

int SolveChallenge1()
{
    string startingPoint = "AA";
    int maxTotalFlow = -1;

    Queue<(int totalFlowRate, int timeRemaining, string currentValve, List<string> closedValves)> queue = new();
    queue.Enqueue((0, 30, startingPoint, new()));

    while (queue.Count > 0)
    {
        //if (queue.Count % 100 == 0)
        //{
        //    Console.WriteLine(queue.Count);
        //}

        var potentialPath = queue.Dequeue();


        if (potentialPath.timeRemaining == 0)
        {
            if (potentialPath.totalFlowRate > maxTotalFlow)
            {
                Console.WriteLine(string.Join(", ", potentialPath.closedValves) + $" - ({potentialPath.totalFlowRate})");
                maxTotalFlow = potentialPath.totalFlowRate;
                continue;
            }
        }

        if (potentialPath.currentValve != "XXX")
        {
            Valve currentValve = valves[potentialPath.currentValve];

            Dictionary<string, int> potentialFlowRates = new();

            foreach (string valve in currentValve.Valves.Keys)
            {
                potentialFlowRates.Add(valve, currentValve.GetPotentialFlowValue(valve, potentialPath.timeRemaining, potentialPath.closedValves));
            }

            List<(int flowRate, string nextStop)> stops = potentialFlowRates.Where(v => v.Value > 0)
                    .OrderByDescending(v => v.Value)
                    .Select(v => (v.Value, v.Key)).ToList();

            if (stops.Count == 0)
            {
                queue.Enqueue((
                    potentialPath.totalFlowRate,
                    0,
                    "XXX",
                    potentialPath.closedValves));
            }
            else
            {
                foreach ((int flowRate, string nextStop) stop in stops)
                {
                    int flowRate = stop.flowRate;
                    string nextStop = stop.nextStop;

                    List<string> closedValves = new(potentialPath.closedValves);

                    if (!closedValves.Contains(nextStop) && flowRate > 0)
                        closedValves.Add(nextStop);

                    int newTimeRemaining = potentialPath.timeRemaining - currentValve.ShortestPaths[nextStop].Count;

                    if (newTimeRemaining < 0)
                        break;

                    (int totalFlowRate, int timeRemaining, string currentValve, List<string> closedValves) newPotentialPath =
                        (potentialPath.totalFlowRate + flowRate,
                        potentialPath.timeRemaining - currentValve.ShortestPaths[nextStop].Count,
                        nextStop,
                        closedValves);

                    queue.Enqueue(newPotentialPath);
                }
            }
        }
    }

    return maxTotalFlow;
}


int SolveChallenge2()
{
    string startingPoint = "AA";
    int maxTotalFlow = -1;

    Queue<(int totalFlowRate, (int timeRemaining, string currentValve) human, (int timeRemaining, string currentValve) elephant, List<string> closedValves)> queue = new();
    queue.Enqueue((0, (26, startingPoint), (26, startingPoint), new()));

    while (queue.Count > 0)
    {
        var potentialPath = queue.Dequeue();

        if (potentialPath.human.timeRemaining == 0 && potentialPath.elephant.timeRemaining == 0)
        {
            if (potentialPath.totalFlowRate > maxTotalFlow)
            {
                Console.WriteLine(string.Join(", ", potentialPath.closedValves) + $" - ({potentialPath.totalFlowRate})");
                maxTotalFlow = potentialPath.totalFlowRate;
                continue;
            }
        }

        if (potentialPath.human.currentValve != "XXX" && potentialPath.elephant.currentValve != "XXX")
        {
            Valve? currentValveHuman = potentialPath.human.currentValve != "XXX" ? valves[potentialPath.human.currentValve] : null;
            Valve? currentValveElephant = potentialPath.elephant.currentValve != "XXX" ? valves[potentialPath.elephant.currentValve] : null;

            Dictionary<string, int> potentialFlowRatesHuman = new(), potentialFlowRatesElephant = new();
            List<(int flowRate, string nextStop)> stopsHuman = new(), stopsElephant = new();

            if (currentValveHuman is not null && potentialPath.human.timeRemaining >= potentialPath.elephant.timeRemaining)
            {
                foreach (string valve in currentValveHuman.Valves.Keys)
                {
                    potentialFlowRatesHuman.Add(valve, currentValveHuman.GetPotentialFlowValue(valve, potentialPath.human.timeRemaining, potentialPath.closedValves));
                }
                stopsHuman = potentialFlowRatesHuman.Where(v => v.Value > 0)
                        .OrderByDescending(v => v.Value)
                        .Select(v => (v.Value, v.Key)).ToList();
            }

            if (currentValveElephant is not null && potentialPath.elephant.timeRemaining >= potentialPath.human.timeRemaining)
            {
                foreach (string valve in currentValveElephant.Valves.Keys)
                {
                    potentialFlowRatesElephant.Add(valve, currentValveElephant.GetPotentialFlowValue(valve, potentialPath.elephant.timeRemaining, potentialPath.closedValves));
                }
                stopsElephant = potentialFlowRatesElephant.Where(v => v.Value > 0)
                        .OrderByDescending(v => v.Value)
                        .Select(v => (v.Value, v.Key)).ToList();
            }

            if (stopsHuman.Count == 0 && stopsElephant.Count == 0)
            {
                queue.Enqueue((
                    potentialPath.totalFlowRate,
                    (0, "XXX"),
                    (0, "XXX"),
                    potentialPath.closedValves));
            }
            else
            {
                // human
                foreach ((int flowRate, string nextStop) stop in stopsHuman)
                {
                    int flowRate = stop.flowRate;
                    string nextStop = stop.nextStop;

                    List<string> closedValves = new(potentialPath.closedValves);

                    if (!closedValves.Contains(nextStop) && flowRate > 0)
                        closedValves.Add(nextStop);

                    int newTimeRemaining = potentialPath.human.timeRemaining - currentValveHuman.ShortestPaths[nextStop].Count;

                    if (newTimeRemaining < 0)
                        break;

                    (int totalFlowRate, (int timeRemaining, string currentValve) human, (int timeRemaining, string currentValve) elephant, List<string> closedValves) newPotentialPath =
                        (potentialPath.totalFlowRate + flowRate,
                        (potentialPath.human.timeRemaining - currentValveHuman.ShortestPaths[nextStop].Count, nextStop),
                        (potentialPath.elephant.timeRemaining, potentialPath.elephant.currentValve),
                        closedValves);

                    queue.Enqueue(newPotentialPath);
                }

                // elephant
                foreach ((int flowRate, string nextStop) stop in stopsElephant)
                {
                    int flowRate = stop.flowRate;
                    string nextStop = stop.nextStop;

                    List<string> closedValves = new(potentialPath.closedValves);

                    if (!closedValves.Contains(nextStop) && flowRate > 0)
                        closedValves.Add(nextStop);

                    int newTimeRemaining = potentialPath.human.timeRemaining - currentValveElephant.ShortestPaths[nextStop].Count;

                    if (newTimeRemaining < 0)
                        break;

                    (int totalFlowRate, (int timeRemaining, string currentValve) human, (int timeRemaining, string currentValve) elephant, List<string> closedValves) newPotentialPath =
                        (potentialPath.totalFlowRate + flowRate,
                        (potentialPath.human.timeRemaining, potentialPath.human.currentValve),
                        (potentialPath.elephant.timeRemaining - currentValveElephant.ShortestPaths[nextStop].Count, nextStop),
                        closedValves);

                    queue.Enqueue(newPotentialPath);
                }
            }
        }
    }

    return maxTotalFlow;
}


partial class Valve
{
    public string Name { get; init; }

    public int FlowRate { get; init; }

    public List<string> AdjacentValves { get; init; }

    public Dictionary<string, Valve> Valves { get; init; }

    public Dictionary<string, List<string>> ShortestPaths { get; set; } = new();

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

    public int GetPotentialFlowValue(string valve, int timeRemaining, List<string> closedValves)
    {
        Valve targetValve = Valves[valve];

        if (closedValves.Contains(valve))
        {
            return 0;
        }

        if (targetValve == this)
        {
            return 0;
        }

        if (targetValve.FlowRate == 0)
        {
            return 0;
        }

        int distanceToValve = ShortestPaths[valve].Count;

        if (timeRemaining < distanceToValve)
        {
            return int.MinValue;
        }

        int potentialFlowRate = (timeRemaining - distanceToValve) * targetValve.FlowRate;

        return potentialFlowRate;
    }

    public void MapShortestPathToEveryValve()
    {
        foreach (string valve in Valves.Keys)
        {
            ShortestPaths.Add(valve, GetShortestPathToValve(valve, new List<string>()));
            //Console.Write($"Shortest path {Name} to {valve}: ");
            //Console.Write(string.Join(" -> ", ShortestPaths[valve]));
            //Console.WriteLine();
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
