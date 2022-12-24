// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

Dictionary<string, (string input, long? number)> monkeys = new();

foreach (string line in inputData)
{
    string[] input = line.Split(": ");
    if (long.TryParse(input[1], out long number))
    {
        monkeys.Add(input[0], (input[1], number));
    }
    else
    {
        monkeys.Add(input[0], (input[1], null));
    }
}

long FindValue(string input)
{
    if (monkeys.TryGetValue(input, out (string input, long? number) value) && value.number != null)
    {
        return (long)monkeys[input].number;
    }

    long result = long.MinValue;
    string[] inputs = monkeys[input].input.Split(' ');
    string input1 = inputs[0];
    string mathOperator = inputs[1];
    string input2 = inputs[2];

    switch (mathOperator)
    {
        case "+":
            result = FindValue(input1) + FindValue(input2);
            break;
        case "-":
            result = FindValue(input1) - FindValue(input2);
            break;
        case "*":
            result = FindValue(input1) * FindValue(input2);
            break;
        case "/":
            result = FindValue(input1) / FindValue(input2);
            break;
        default:
            throw new NotImplementedException("Operator not supported");
    }

    return result;
}

long rootAnswer = FindValue("root");

Console.WriteLine($"Challenge 1 Answer: {rootAnswer}");