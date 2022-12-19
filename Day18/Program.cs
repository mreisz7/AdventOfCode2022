// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Dictionary<(int x, int y, int z), int> droplets = new();

foreach (string cube in inputData)
{
    string[] coordinates = cube.Split(',');
    int x = int.Parse(coordinates[0]);
    int y = int.Parse(coordinates[1]);
    int z = int.Parse(coordinates[2]);

    droplets.Add((x, y, z), 6);
}

foreach ((int x, int y, int z) coord in droplets.Keys)
{
    // X + 1
    if (droplets.ContainsKey((coord.x + 1, coord.y, coord.z)))
    {
        droplets[coord]--;
    }

    // X - 1
    if (droplets.ContainsKey((coord.x - 1, coord.y, coord.z)))
    {
        droplets[coord]--;
    }

    // Y + 1
    if (droplets.ContainsKey((coord.x, coord.y + 1, coord.z)))
    {
        droplets[coord]--;
    }

    // Y - 1
    if (droplets.ContainsKey((coord.x, coord.y - 1, coord.z)))
    {
        droplets[coord]--;
    }

    // Z + 1
    if (droplets.ContainsKey((coord.x, coord.y, coord.z + 1)))
    {
        droplets[coord]--;
    }

    // Z - 1
    if (droplets.ContainsKey((coord.x, coord.y, coord.z - 1)))
    {
        droplets[coord]--;
    }
}

int part1Total = droplets.Select(d => d.Value).Sum();

Console.WriteLine($"Challenge 1 Answer: {part1Total}");
