// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Console.WriteLine($"Challenge 1 Answer: {SolvePart1()}");

Console.WriteLine($"Challenge 1 Answer: {SolvePart2()}");

int SolvePart1()
{
    List<(int number, int originalIndex)> values = new();

    for (int i = 0; i < inputData.Length; i++)
    {
        int value = int.Parse(inputData[i]);
        values.Add((value, i));
    }

    for (int i = 0; i < values.Count; i++)
    {
        var targetValue = values.Where(x => x.originalIndex == i).First();
        var targetIndex = values.IndexOf(targetValue);
        var newTargetIndex = (targetIndex + targetValue.number) % (values.Count - 1);

        if (newTargetIndex <= 0 && targetIndex + targetValue.number != 0)
        {
            newTargetIndex = values.Count - 1 + newTargetIndex;
        }

        if (targetValue.number != 0)
        {
            values.RemoveAt(targetIndex);
            values.Insert(newTargetIndex, targetValue);
        }
    }

    var zeroValue = values.Where(x => x.number == 0).First();
    int indexOfZero = values.IndexOf(zeroValue);

    int numberAtZeroPlus1000 = values[(indexOfZero + 1000) % values.Count].number;
    int numberAtZeroPlus2000 = values[(indexOfZero + 2000) % values.Count].number;
    int numberAtZeroPlus3000 = values[(indexOfZero + 3000) % values.Count].number;

    return numberAtZeroPlus1000 + numberAtZeroPlus2000 + numberAtZeroPlus3000;
}

long SolvePart2()
{
    List<(long number, int originalIndex)> valuesPart2 = new();

    long multiplier = 811589153;

    for (int i = 0; i < inputData.Length; i++)
    {
        int value = int.Parse(inputData[i]);
        valuesPart2.Add(((long)value * multiplier, i));
    }

    for (int n = 0; n < 10; n++)
    {
        for (int i = 0; i < valuesPart2.Count; i++)
        {
            var targetValue = valuesPart2.Where(x => x.originalIndex == i).First();
            var targetIndex = valuesPart2.IndexOf(targetValue);
            var newTargetIndex = (int)((targetIndex + targetValue.number) % (valuesPart2.Count - 1));

            if (newTargetIndex <= 0 && targetIndex + targetValue.number != 0)
            {
                newTargetIndex = valuesPart2.Count - 1 + newTargetIndex;
            }

            if (targetValue.number != 0)
            {
                valuesPart2.RemoveAt(targetIndex);
                valuesPart2.Insert(newTargetIndex, targetValue);
            }
        }
    }

    var zeroValue = valuesPart2.Where(x => x.number == 0).First();
    int indexOfZero = valuesPart2.IndexOf(zeroValue);

    long numberAtZeroPlus1000 = valuesPart2[(indexOfZero + 1000) % valuesPart2.Count].number;
    long numberAtZeroPlus2000 = valuesPart2[(indexOfZero + 2000) % valuesPart2.Count].number;
    long numberAtZeroPlus3000 = valuesPart2[(indexOfZero + 3000) % valuesPart2.Count].number;

    return numberAtZeroPlus1000 + numberAtZeroPlus2000 + numberAtZeroPlus3000;
}
