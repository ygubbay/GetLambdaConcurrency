// See https://aka.ms/new-console-template for more information
using Amazon.Lambda;
using Amazon.Lambda.Model;

Console.WriteLine("List of lambda concurrency in Account");


var client = new AmazonLambdaClient();

var functionNames = new List<string>();
var response = await client.ListFunctionsAsync();
foreach (var f in response.Functions)
{
    functionNames.Add(f.FunctionName);
}

while (!string.IsNullOrEmpty(response.NextMarker))
{
    
    response = await client.ListFunctionsAsync(new ListFunctionsRequest { Marker = response.NextMarker });
    foreach (var f in response.Functions)
    {
        functionNames.Add(f.FunctionName);
    }
}


var dicFunctions = new Dictionary<string, GetFunctionConcurrencyResponse>();

int i = 0;
foreach (string functionName in functionNames)
{
    var request = new GetFunctionConcurrencyRequest { FunctionName = functionName  };
    var concurr = await client.GetFunctionConcurrencyAsync(request);

    if (concurr.ReservedConcurrentExecutions > 0)
    {
        dicFunctions.Add(functionName, concurr);
    }
    Console.Write($"Function {++i}: {functionName}");
}

var lines = new List<string>();
foreach (var f in dicFunctions)
{
    lines.Add($"{f.Key}\t {f.Value.ReservedConcurrentExecutions}");
}

await File.WriteAllLinesAsync("lambda_concurrency.txt", lines);