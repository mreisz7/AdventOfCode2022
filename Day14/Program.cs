// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

// Set the Console Width now
//Console.BufferWidth = 600;

Dictionary<(int x, int y), string> caveMap = new();

foreach (string line in inputData)
{
    (int x, int y)[] wallInstructions = line.Split(" -> ").Select(x => (int.Parse(x.Split(',')[0]), int.Parse(x.Split(',')[1]))).ToArray();
    for (int i = 1; i < wallInstructions.Length; i++)
    {
        int x1 = wallInstructions[i - 1].x;
        int x2 = wallInstructions[i].x;
        int y1 = wallInstructions[i - 1].y;
        int y2 = wallInstructions[i].y;

        int startX = (x1 > x2) ? x2 : x1;
        int endX =   (x1 > x2) ? x1 : x2;
        int startY = (y1 > y2) ? y2 : y1;
        int endY =   (y1 > y2) ? y1 : y2;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if (!caveMap.ContainsKey((x, y)))
                {
                    caveMap.Add((x, y), "#");
                }
            }
        }
    }
}

int _Bottom = caveMap.Keys.Select(x => x.y).Max();

Console.WriteLine($"Challenge 1 Answer: {StartSandDrop(caveMap)}");
DrawCaveWalls(caveMap);

// Challenge 2
Dictionary<(int x, int y), string> newCaveMap = new(caveMap);

for (int x = 500 - (_Bottom + 5); x <= 500 + (_Bottom + 5); x++)
{
    newCaveMap.Add((x, _Bottom + 2), "#");
}

Console.WriteLine($"Challenge 2 Answer: {StartSandDrop(newCaveMap)}");
DrawCaveWalls(newCaveMap);

void DrawCaveWalls(Dictionary<(int x, int y), string> caveMap)
{
    int top    = 0;
    int bottom = caveMap.Keys.Select(x => x.y).Max();
    int minX   = caveMap.Keys.Select(x => x.x).Min();
    int maxX   = caveMap.Keys.Select(x => x.x).Max();

    for (int y = top; y <= bottom; y++)
    {
        string yLeftPad = y.ToString().PadLeft(3);
        Console.Write($"{yLeftPad}  || ");
        for (int x = minX; x <= maxX; x++)
        {
            if (y == 0 && x == 499)
            {
                Console.Write("\\");
                continue;
            }
            if (y == 0 && x == 501)
            {
                Console.Write("/");
                continue;
            }
            if (caveMap.ContainsKey((x, y)))
            {
                Console.Write(caveMap[(x, y)]);
            }
            else
            {
                Console.Write(" ");
            }
        }
        Console.Write(" ||");
        Console.WriteLine();
    }
}

int StartSandDrop(Dictionary<(int x, int y), string> caveMap)
{
    int amountOfSandDropped = 0;

    // Clear sand from caveMap
    foreach (var item in caveMap.Where(x => x.Value == "o"))
    {
        caveMap.Remove(item.Key);
    }

    while (DroppingSandFallsIntoTheAbyss(caveMap))
    {
        amountOfSandDropped++;
    }

    //Console.SetCursorPosition(0, _Bottom + 2);
    
    return amountOfSandDropped;
}

bool DroppingSandFallsIntoTheAbyss(Dictionary<(int x, int y), string> caveMap)
{
    int bottom = caveMap.Keys.Select(x => x.y).Max();
    int minX = caveMap.Keys.Select(x => x.x).Min();
    int maxX = caveMap.Keys.Select(x => x.x).Max();

    bool sandAtRest = false;
    (int x, int y) sandDropPoint = (500, 0);
    int sandX = sandDropPoint.x;
    int consoleOffset = maxX - (maxX - minX);
    for (int y = sandDropPoint.y; y < bottom; y++)
    {
        // Draw 
        //Console.SetCursorPosition(sandX + 8 - consoleOffset, y);
        //Console.Write("o");

        Thread.Sleep(0);
        if (caveMap.ContainsKey((sandX, y)))
        {
            sandAtRest = false;
            break;
        }
        if (caveMap.ContainsKey((sandX, y + 1)) && !caveMap.ContainsKey((sandX - 1, y + 1)))
        {
            //Console.SetCursorPosition(sandX + 8 - consoleOffset, y);
            //Console.Write(" ");
            sandX -= 1;
            if (sandX < minX)
            {
                sandAtRest = false;
                break;
            }
        }
        else if (caveMap.ContainsKey((sandX, y + 1)) && !caveMap.ContainsKey((sandX + 1, y + 1)))
        {
            //Console.SetCursorPosition(sandX + 8 - consoleOffset, y);
            //Console.Write(" ");
            sandX += 1;
            if (sandX > maxX)
            {
                sandAtRest = false;
                break;
            }
        }
        else if (caveMap.ContainsKey((sandX, y + 1)) && caveMap.ContainsKey((sandX - 1, y + 1)) && caveMap.ContainsKey((sandX + 1, y + 1)))
        {
            caveMap.Add((sandX, y), "o");
            sandAtRest = true;
            break;
        }
        //Console.SetCursorPosition(sandX + 8 - consoleOffset, y);
        //Console.Write(" ");
    }

    return sandAtRest;
}

