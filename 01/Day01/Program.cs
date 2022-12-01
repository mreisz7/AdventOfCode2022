// Read in the Challenge 1 input
//string[] inputData = System.IO.File.ReadAllLines(@"..\..\..\Challenge1Input_Test.txt");
string[] inputData = System.IO.File.ReadAllLines(@"..\..\..\Challenge1Input.txt");

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

Console.WriteLine(calorieTotals.Max());