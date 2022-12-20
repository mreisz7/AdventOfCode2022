// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

HashSet<(int x, int y, int z)> droplets = new();

foreach (string cube in inputData)
{
    string[] coordinates = cube.Split(',');
    int x = int.Parse(coordinates[0]);
    int y = int.Parse(coordinates[1]);
    int z = int.Parse(coordinates[2]);

    droplets.Add((x, y, z));
}

Console.WriteLine($"Challenge 1 Answer: {CalculateSurfaceArea(droplets)}");


// Challenge 2:

HashSet<(int x, int y, int z)> filledShell = GetOuterShellOfDroplet(droplets);

int challenge2Answer = CalculateSurfaceArea(filledShell);

Console.WriteLine($"Challenge 2 Answer: {challenge2Answer}");

int CalculateSurfaceArea(HashSet<(int x, int y, int z)> shape)
{
    Dictionary<(int x, int y, int z), int> shapeDict = new();

    foreach ((int x, int y, int z) coord in shape)
    {
        shapeDict.Add(coord, 6);
    }

    foreach ((int x, int y, int z) coord in shapeDict.Keys)
    {
        // X + 1
        if (shapeDict.ContainsKey((coord.x + 1, coord.y, coord.z)))
        {
            shapeDict[coord]--;
        }

        // X - 1
        if (shapeDict.ContainsKey((coord.x - 1, coord.y, coord.z)))
        {
            shapeDict[coord]--;
        }

        // Y + 1
        if (shapeDict.ContainsKey((coord.x, coord.y + 1, coord.z)))
        {
            shapeDict[coord]--;
        }

        // Y - 1
        if (shapeDict.ContainsKey((coord.x, coord.y - 1, coord.z)))
        {
            shapeDict[coord]--;
        }

        // Z + 1
        if (shapeDict.ContainsKey((coord.x, coord.y, coord.z + 1)))
        {
            shapeDict[coord]--;
        }

        // Z - 1
        if (shapeDict.ContainsKey((coord.x, coord.y, coord.z - 1)))
        {
            shapeDict[coord]--;
        }
    }

    int result = shapeDict.Select(d => d.Value).Sum();

    return result;
}

HashSet<(int x, int y, int z)> GetOuterShellOfDroplet(HashSet<(int x, int y, int z)> shape)
{
    HashSet<(int x, int y, int z)> shell = new();

    // Define the outer bounds around the droplet
    int xMin = shape.Select(d => d.x).Min() - 1;
    int xMax = shape.Select(d => d.x).Max() + 1;
    int yMin = shape.Select(d => d.y).Min() - 1;
    int yMax = shape.Select(d => d.y).Max() + 1;
    int zMin = shape.Select(d => d.z).Min() - 1;
    int zMax = shape.Select(d => d.z).Max() + 1;

    // Start in one corner and start navigating outwards from there
    List<(int x, int y, int z)> visitedCubes = new();
    
    Stack<(int x, int y, int z)> cubeNavigators = new();
    cubeNavigators.Push((xMin, yMin, zMin));

    while (cubeNavigators.Count > 0)
    {
        (int x, int y, int z) currentCube = cubeNavigators.Pop();
        visitedCubes.Add(currentCube);

        if (shape.Contains(currentCube))
        {
            shell.Add(currentCube);
        }
        else
        {
            // Up
            if (currentCube.y + 1 <= yMax && !visitedCubes.Contains((currentCube.x, currentCube.y + 1, currentCube.z)))
            {
                cubeNavigators.Push((currentCube.x, currentCube.y + 1, currentCube.z));
            }

            // Down
            if (currentCube.y - 1 >= yMin && !visitedCubes.Contains((currentCube.x, currentCube.y - 1, currentCube.z)))
            {
                cubeNavigators.Push((currentCube.x, currentCube.y - 1, currentCube.z));
            }

            // Left
            if (currentCube.x - 1 >= xMin && !visitedCubes.Contains((currentCube.x - 1, currentCube.y, currentCube.z)))
            {
                cubeNavigators.Push((currentCube.x - 1, currentCube.y, currentCube.z));
            }

            // Right
            if (currentCube.x + 1 <= xMax && !visitedCubes.Contains((currentCube.x + 1, currentCube.y, currentCube.z)))
            {
                cubeNavigators.Push((currentCube.x + 1, currentCube.y, currentCube.z));
            }

            // Front
            if (currentCube.z - 1 >= zMin && !visitedCubes.Contains((currentCube.x, currentCube.y, currentCube.z - 1)))
            {
                cubeNavigators.Push((currentCube.x, currentCube.y, currentCube.z - 1));
            }

            // Back
            if (currentCube.z + 1 <= zMax && !visitedCubes.Contains((currentCube.x, currentCube.y, currentCube.z + 1)))
            {
                cubeNavigators.Push((currentCube.x, currentCube.y, currentCube.z + 1));
            }
        }
    }

    HashSet<(int x, int y, int z)> filledShape = new();

    // Now check for voids with blocks on each of the 6 sides
    for (int z = zMin; z <= zMax; z++)
    {
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                fullCube.Add((x, y, z));

                if (!visitedCubes.Contains((x, y, z)) || shell.Contains((x, y, z)))
                {
                    filledShape.Add((x, y, z));
                }
            }
        }
    }

    return filledShape;
}

enum Direction
{
    Up,
    Down,
    Left,
    Right,
    Front,
    Back
}