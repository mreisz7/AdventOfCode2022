using System.Text.RegularExpressions;

// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

// Store all of the blueprints in a dictionary
Dictionary<int, Blueprint> blueprints = new();
for (int i = 0; i < inputData.Length; i++)
{
    blueprints.Add(i + 1, new Blueprint(inputData[i]));
}

Dictionary<int, int> maximumGeodesCollected = new();
foreach (Blueprint blueprint in blueprints.Values)
{
    Console.WriteLine($"Starting calculation for blueprint #{blueprint.BlueprintNumber}");
    maximumGeodesCollected.Add(blueprint.BlueprintNumber, MaximumNumberOfGeodesCollected(blueprint, 24));
}

int Challenge1Answer = maximumGeodesCollected.Sum(x => x.Key * x.Value);

Console.WriteLine($"Challenge 1 Answer: {Challenge1Answer}");

List<Blueprint> firstThreeBlueprints = blueprints.Where(x => x.Key <= 3).Select(x => x.Value).ToList();

Dictionary<int, int> maximumGeodesCollectedPart2 = new();
foreach (Blueprint blueprint in firstThreeBlueprints)
{
    Console.WriteLine($"Starting calculation for blueprint #{blueprint.BlueprintNumber}");
    maximumGeodesCollectedPart2.Add(blueprint.BlueprintNumber, MaximumNumberOfGeodesCollected(blueprint, 32));
}

int Challenge2Answer = maximumGeodesCollectedPart2[1] * maximumGeodesCollectedPart2[2] * maximumGeodesCollectedPart2[3];

Console.WriteLine($"Challenge 2 Answer: {Challenge2Answer}");

int MaximumNumberOfGeodesCollected(Blueprint blueprint, int maxTurns)
{
    Stack<((int ore, int clay, int obsidian, int geode) robots, (int ore, int clay, int obsidian, int geode) resources, int turn)> queue = new();
    queue.Push(((1, 0, 0, 0), (0, 0, 0, 0), 0));
    HashSet<((int ore, int clay, int obsidian, int geode) robots, (int ore, int clay, int obsidian, int geode) resources, int turn)> tried = new();

    int maximumNumberOfGeodesCollected = 0;

    // Find max resources needed for any single robot
    int maxOre = Max(blueprint.OreRobotOreCost, blueprint.ClayRobotOreCost, blueprint.ObsidianRobotOreCost, blueprint.GeodeRobotOreCost);
    int maxClay = blueprint.ObsidianRobotClayCost;
    int maxObsidian = blueprint.GeodeRobotObsidianCost;

    while (queue.Count > 0)
    {
        ((int ore, int clay, int obsidian, int geode) robots, (int ore, int clay, int obsidian, int geode) resources, int turn) current = queue.Pop();

        if (tried.Contains(current))
            continue;

        tried.Add(current);

        // Decide which scenarios to proceed with
        List<int> scenariosToPursue = new();

        // Build an ore robot
        if (current.robots.ore < maxOre && 
            current.resources.ore >= blueprint.OreRobotOreCost)
            scenariosToPursue.Add(1);

        // Build a clay robot
        if (current.robots.clay < maxClay && 
            current.resources.ore >= blueprint.ClayRobotOreCost)
            scenariosToPursue.Add(2);

        // Build an obsidian robot
        if (current.robots.obsidian < maxObsidian && 
            current.resources.ore >= blueprint.ObsidianRobotOreCost && 
            current.resources.clay >= blueprint.ObsidianRobotClayCost)
            scenariosToPursue.Add(3);

        // Build a geode robot
        if (current.resources.ore >= blueprint.GeodeRobotOreCost && 
            current.resources.obsidian >= blueprint.GeodeRobotObsidianCost)
            scenariosToPursue.Add(4);

        // Increase the turn
        current.turn++;

        // Collect resources
        current.resources.ore += current.robots.ore;
        current.resources.clay += current.robots.clay;
        current.resources.obsidian += current.robots.obsidian;
        current.resources.geode += current.robots.geode;

        // Prune if you can't make enough geode robots in the time remaining to improve on the current score
        int timeRemaining = maxTurns - current.turn;
        if (current.resources.geode + (current.robots.geode * timeRemaining) + timeRemaining + 25 < maximumNumberOfGeodesCollected)
            continue;

        // If more Geodes have been collected than previously recorded then increase the count
        if (current.turn == maxTurns)
        {
            if (current.resources.geode > maximumNumberOfGeodesCollected)
                maximumNumberOfGeodesCollected = current.resources.geode;
            continue;
        }

        // Now enqueue the build scenarios
        if (scenariosToPursue.Contains(4))
        {
            var buildGeodeRobotScenario = current;
            buildGeodeRobotScenario.resources.ore -= blueprint.GeodeRobotOreCost;
            buildGeodeRobotScenario.resources.obsidian -= blueprint.GeodeRobotObsidianCost;
            buildGeodeRobotScenario.robots.geode++;
            queue.Push(buildGeodeRobotScenario);
            continue;
        }

        // Enqueue the "Do Nothing" scenario
        var doNothingScenario = current;
        queue.Push(doNothingScenario);

        if (scenariosToPursue.Contains(1))
        {
            var buildOreRobotScenario = current;
            buildOreRobotScenario.resources.ore -= blueprint.OreRobotOreCost;
            buildOreRobotScenario.robots.ore++;
            queue.Push(buildOreRobotScenario);
        }

        if (scenariosToPursue.Contains(2))
        {
            var buildClayRobotScenario = current;
            buildClayRobotScenario.resources.ore -= blueprint.ClayRobotOreCost;
            buildClayRobotScenario.robots.clay++;
            queue.Push(buildClayRobotScenario);
        }

        if (scenariosToPursue.Contains(3))
        {
            var buildObsidianRobotScenario = current;
            buildObsidianRobotScenario.resources.ore -= blueprint.ObsidianRobotOreCost;
            buildObsidianRobotScenario.resources.clay -= blueprint.ObsidianRobotClayCost;
            buildObsidianRobotScenario.robots.obsidian++;
            queue.Push(buildObsidianRobotScenario);
        }
    }

    return maximumNumberOfGeodesCollected;
}

static int Max(params int[] numberItems)
{
    return numberItems.Max();
}

partial class Blueprint
{
    public int BlueprintNumber { get; init; }

    public int OreRobotOreCost { get; init; }

    public int ClayRobotOreCost { get; init; }

    public int ObsidianRobotOreCost { get; init; }

    public int ObsidianRobotClayCost { get; init; }

    public int GeodeRobotOreCost { get; init; }

    public int GeodeRobotObsidianCost { get; init; }

    public Blueprint(string blueprint)
    {
        Regex blueprintInput = MyRegex();
        Match match = blueprintInput.Match(blueprint);
        BlueprintNumber = int.Parse(match.Groups["blueprintNumber"].Value);
        OreRobotOreCost = int.Parse(match.Groups["oreRobotOreCost"].Value);
        ClayRobotOreCost = int.Parse(match.Groups["clayRobotOreCost"].Value);
        ObsidianRobotOreCost = int.Parse(match.Groups["obsidianRobotOreCost"].Value);
        ObsidianRobotClayCost = int.Parse(match.Groups["obsidianRobotClayCost"].Value);
        GeodeRobotOreCost = int.Parse(match.Groups["geodeRobotOreCost"].Value);
        GeodeRobotObsidianCost = int.Parse(match.Groups["geodeRobotObsidianCost"].Value);
    }

    // Example: Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
    [GeneratedRegex("Blueprint (?<blueprintNumber>\\d+): Each ore robot costs (?<oreRobotOreCost>\\d+) ore. Each clay robot costs (?<clayRobotOreCost>\\d+) ore. Each obsidian robot costs (?<obsidianRobotOreCost>\\d+) ore and (?<obsidianRobotClayCost>\\d+) clay. Each geode robot costs (?<geodeRobotOreCost>\\d+) ore and (?<geodeRobotObsidianCost>\\d+) obsidian.")]
    private static partial Regex MyRegex();
}
