// Read in the Challenge input
//string[] inputData = System.IO.File.ReadAllLines(@".\Challenge1Input_Test.txt");
string[] inputData = System.IO.File.ReadAllLines(@".\Challenge1Input.txt");

// This will store the calorie totals for each elf
List<int> calorieTotals = new();

int runningTotal = 0;
foreach (string line in inputData)
{
    if (line == string.Empty)
    {
        calorieTotals.Add(runningTotal);
        runningTotal = 0;
        continue;
    }

    bool parseSucceeded = int.TryParse(line, out int calorieValue);

    if (parseSucceeded)
    {
        runningTotal += calorieValue;
    }
}

Console.WriteLine("Challenge 1 Answer: " + calorieTotals.Max());

// Challenge 2
List<int> Top3ElfsByTotalCalories = calorieTotals.OrderByDescending(x => x).Take(3).ToList();

Console.WriteLine("Challenge 2 Answer: " + Top3ElfsByTotalCalories.Sum());
