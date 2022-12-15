using System.Text.RegularExpressions;

// Read in the Challenge input

string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
int yInQuestion = 10; // <-- Test value
int minXY = 0;        // <-- Test value
int maxXY = 20;       // <-- Test value

//string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");
//int yInQuestion = 2_000_000;
//int minXY = 0;
//int maxXY = 4000000;

List<SensorBeaconPair> sensors = new();
HashSet<(int X, int Y, char Type)> cells = new();

foreach (string input in inputData)
{
    SensorBeaconPair sensor = new SensorBeaconPair(input);
    sensors.Add(sensor);
    cells.UnionWith(sensor.GetCellsFromY(yInQuestion));
}

Console.WriteLine($"Challenge 1 Answer: {SolveChallenge1(cells, yInQuestion)}");

Console.WriteLine($"Challenge 2 Answer: {SolveChallenge2(minXY, maxXY)}");

int SolveChallenge1(HashSet<(int X, int Y, char Type)> sensorCells, int y)
{
    // Get hashset of SensorBeaconPairs where y matches the provided value
    HashSet<(int X, int Y)> sensorRangeCells = sensorCells.Where(c => c.Y == y).Select(c => (c.X, c.Y)).ToHashSet();

    // Now subtract the ones that are beacons or sensors
    int sensorsAndBeacons = sensorCells.Where(c => (c.Type == 'S' || c.Type == 'B') && c.Y == y).Count();

    return sensorRangeCells.Count - sensorsAndBeacons;
}

int SolveChallenge2(int minXY, int maxXY)
{
    int result = -1;

    for (int x = minXY; x <= maxXY; x++)
    {
        for (int y = minXY; y <= maxXY; y++)
        {
            foreach (SensorBeaconPair sensor in sensors)
            {
                if ()
            }
        }
    }

    return result;
}

partial class SensorBeaconPair
{
    public (int X, int Y) Sensor { get; init; }

    public (int X, int Y) Beacon { get; init; }

    public int ManhattanDistance { get; init; }

    public static int GetManhattanDistance((int X, int Y) sensor, (int X, int Y) beacon)
    {
        int maxX = Math.Max(sensor.X, beacon.X);
        int minX = Math.Min(sensor.X, beacon.X);
        int maxY = Math.Max(sensor.Y, beacon.Y);
        int minY = Math.Min(sensor.Y, beacon.Y);

        return (maxX - minX) + (maxY - minY);
    }

    public HashSet<(int X, int Y, char Type)> GetCellsFromY(int y)
    {
        HashSet<(int X, int Y, char Type)> cellSet = new();

        for (int yOffset = -ManhattanDistance; yOffset <= ManhattanDistance; yOffset++)
        {
            if (Sensor.Y + yOffset == y)
            {
                for (int xOffset = -(ManhattanDistance - Math.Abs(yOffset)); xOffset <= ManhattanDistance - Math.Abs(yOffset); xOffset++)
                {
                    (int X, int Y) cellCoordinate = (Sensor.X + xOffset, Sensor.Y + yOffset);
                    char type = '#';
                    if (cellCoordinate == Sensor)
                        type = 'S';
                    if (cellCoordinate == Beacon)
                        type = 'B';
                    cellSet.Add((cellCoordinate.X, cellCoordinate.Y, type));
                }
            }
        }

        return cellSet;
    } 

    public SensorBeaconPair(string sensorData)
    {
        // Example input: Sensor at x=2, y=18: closest beacon is at x=-2, y=15
        Regex sensorInput = MyRegex();
        Match match = sensorInput.Match(sensorData);
        int sensorX = int.Parse(match.Groups["sensorX"].Value);
        int sensorY = int.Parse(match.Groups["sensorY"].Value);
        int beaconX = int.Parse(match.Groups["beaconX"].Value);
        int beaconY = int.Parse(match.Groups["beaconY"].Value);

        Sensor = (sensorX, sensorY);
        Beacon = (beaconX, beaconY);
        ManhattanDistance = GetManhattanDistance(Sensor, Beacon);
    }

    [GeneratedRegex("Sensor at x=(?<sensorX>-?\\d+), y=(?<sensorY>-?\\d+): closest beacon is at x=(?<beaconX>-?\\d+), y=(?<beaconY>-?\\d+)")]
    private static partial Regex MyRegex();
}
