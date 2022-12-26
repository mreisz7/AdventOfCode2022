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

long rootAnswer = FindValue("root");

//Console.WriteLine($"Challenge 1 Answer: {rootAnswer}");

Console.WriteLine($"Challenge 2 Answer: {FindRootEquality("root")}");

long FindValue(string input, long? humanInput = null)
{
    if (humanInput != null && input == "humn")
    {
        return (long)humanInput;
    }

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
            result = FindValue(input1, humanInput) + FindValue(input2, humanInput);
            break;
        case "-":
            result = FindValue(input1, humanInput) - FindValue(input2, humanInput);
            break;
        case "*":
            result = FindValue(input1, humanInput) * FindValue(input2, humanInput);
            break;
        case "/":
            result = FindValue(input1, humanInput) / FindValue(input2, humanInput);
            break;
        default:
            throw new NotImplementedException("Operator not supported");
    }

    return result;
}

long FindRootEquality(string monkey)
{
    string[] inputs = monkeys[monkey].input.Split(' ');
    string input1 = inputs[0];
    string mathOperator = inputs[1];
    string input2 = inputs[2];

    // Find which side has the human input
    long leftSideValue = FindValue(input1);
    long rightSideValue = FindValue(input2);

    long leftSideAltered = FindValue(input1, 42);
    long rightSideAltered = FindValue(input2, 42);

    long mustEqual, humanValue;

    if (leftSideValue != leftSideAltered) 
    {
        mustEqual = rightSideValue;
        humanValue = FindHumanValue(input1, mustEqual);
    }
    else
    {
        mustEqual = leftSideValue;
        humanValue = FindHumanValue(input2, mustEqual);
    }

    return humanValue;
}

long FindHumanValue(string monkey, long valueMustEqual)
{
    if (monkey == "humn")
    {
        return valueMustEqual;
    }

    string[] inputs = monkeys[monkey].input.Split(' ');
    string input1 = inputs[0];
    string mathOperator = inputs[1];
    string input2 = inputs[2];

    // Find which side has the human input
    long leftSideValue = FindValue(input1);
    long rightSideValue = FindValue(input2);

    long leftSideAltered = FindValue(input1, 42);
    long rightSideAltered = FindValue(input2, 42);

    long mustEqual = 0;
    long humanValue;

    if (leftSideValue != leftSideAltered)
    {
        switch (mathOperator)
        {
            case "*":
                mustEqual = valueMustEqual / rightSideValue;
                break;
            case "/":
                mustEqual = rightSideValue * valueMustEqual;
                break;
            case "+":
                mustEqual = valueMustEqual - rightSideValue;
                break;
            case "-":
                mustEqual = rightSideValue + valueMustEqual;
                break;
            default:
                throw new NotImplementedException("Operator not supported");
        }
        humanValue = FindHumanValue(input1, mustEqual);
    }
    else
    {
        switch (mathOperator)
        {
            case "*":
                mustEqual = valueMustEqual / leftSideValue;
                break;
            case "/":
                mustEqual = leftSideValue / valueMustEqual;
                break;
            case "+":
                mustEqual = valueMustEqual - leftSideValue;
                break;
            case "-":
                mustEqual = leftSideValue - valueMustEqual;
                break;
            default:
                throw new NotImplementedException("Operator not supported");
        }
        humanValue = FindHumanValue(input2, mustEqual);
    }

    return humanValue;
}