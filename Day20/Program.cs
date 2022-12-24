// Read in the Challenge input
using System.Globalization;

//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

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

int answerProblem1 = numberAtZeroPlus1000 + numberAtZeroPlus2000 + numberAtZeroPlus3000;

Console.WriteLine($"Challenge 1 Answer: {answerProblem1}");