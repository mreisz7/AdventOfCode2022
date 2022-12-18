using System.Collections;
using System.Text.RegularExpressions;

// Read in the Challenge input

//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
//long yInQuestion = 10; // <-- Test value
//long minXY = 0;        // <-- Test value
//long maxXY = 20;       // <-- Test value

string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");
long yInQuestion = 2_000_000;
long minXY = 0;
long maxXY = 4_000_000;

List<SensorBeaconPair> sensors = new();

foreach (string input in inputData)
{
    SensorBeaconPair sensor = new SensorBeaconPair(input);
    sensors.Add(sensor);
}

Console.WriteLine($"Challenge 1 Answer: {SolveChallenge1(yInQuestion)}");

Console.WriteLine($"Challenge 2 Answer: {SolveChallenge2(minXY, maxXY)}");

long SolveChallenge1(long y)
{
    List<(long xStart, long xEnd)> yRanges = new();
    HashSet<(long x, long y)> yCells = new();
    
    // Get the ranges from the y row
    foreach (SensorBeaconPair sensorPair in sensors)
    {
        (long xStart, long xEnd)? sensorRange = sensorPair.GetCellRangeByY(y);
        if (sensorRange is not null)
        {
            (long xStart, long xEnd) range = ((long xStart, long xEnd))sensorRange;
            for (long x = range.xStart; x <= range.xEnd; x++)
            {
                yCells.Add((x, y));
            }
        }
    }

    // Get the count of beacons on the row in question
    HashSet<(long x, long y)> beaconsOnY = sensors.Where(x => x.Beacon.Y == y).Select(x => (x.Beacon.X, x.Beacon.Y)).ToHashSet();

    return yCells.Count - beaconsOnY.Count;
}

long SolveChallenge2(long minXY, long maxXY)
{
    for (long y = minXY; y <= maxXY; y++)
    {
        for (long x = minXY; x <= maxXY; x++)
        {
            List<SensorBeaconPair> sensorsInRange = sensors.Where(s => s.IsWithinCoverage((x, y))).ToList();
            if (sensorsInRange.Count > 0)
            {
                x = sensorsInRange.Select(s => s.GetCellRangeByY(y)).Select(s => s.Value.xEnd).Max();
            } else
            {
                return x * 4_000_000 + y;
            }

        }
    }

    throw new Exception("Nothing found");
}

partial class SensorBeaconPair
{
    public (long X, long Y) Sensor { get; init; }

    public (long X, long Y) Beacon { get; init; }

    public long ManhattanDistance { get; init; }

    public static long GetManhattanDistance((long X, long Y) sensor, (long X, long Y) beacon)
    {
        long maxX = Math.Max(sensor.X, beacon.X);
        long minX = Math.Min(sensor.X, beacon.X);
        long maxY = Math.Max(sensor.Y, beacon.Y);
        long minY = Math.Min(sensor.Y, beacon.Y);

        return (maxX - minX) + (maxY - minY);
    }

    public bool IsWithinCoverage((long x, long y) coordinate)
    {
        if (GetManhattanDistance(Sensor, (coordinate.x, coordinate.y)) <= ManhattanDistance)
        {
            return true;
        }
        return false;
    }

    public (long xStart, long xEnd)? GetCellRangeByY(long requestedY)
    {
        long yOffset = requestedY - Sensor.Y;

        if (yOffset < -ManhattanDistance || yOffset > ManhattanDistance)
            return null;

        long xStart = Sensor.X - (ManhattanDistance - Math.Abs(yOffset));
        long xEnd = Sensor.X + (ManhattanDistance - Math.Abs(yOffset));
        return (xStart, xEnd);
    }

    public SensorBeaconPair(string sensorData)
    {
        // Example input: Sensor at x=2, y=18: closest beacon is at x=-2, y=15
        Regex sensorInput = MyRegex();
        Match match = sensorInput.Match(sensorData);
        long sensorX = long.Parse(match.Groups["sensorX"].Value);
        long sensorY = long.Parse(match.Groups["sensorY"].Value);
        long beaconX = long.Parse(match.Groups["beaconX"].Value);
        long beaconY = long.Parse(match.Groups["beaconY"].Value);

        Sensor = (sensorX, sensorY);
        Beacon = (beaconX, beaconY);
        ManhattanDistance = GetManhattanDistance(Sensor, Beacon);
    }

    [GeneratedRegex("Sensor at x=(?<sensorX>-?\\d+), y=(?<sensorY>-?\\d+): closest beacon is at x=(?<beaconX>-?\\d+), y=(?<beaconY>-?\\d+)")]
    private static partial Regex MyRegex();
}
