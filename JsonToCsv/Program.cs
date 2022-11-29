using System;
using System.IO;
using System.Linq;
using System.Text.Json;

var inFileName = args[0];
var outFileName = args[1];

Console.WriteLine($"Reading from {inFileName} to {outFileName}...");

using var fileIn = File.OpenRead(inFileName);
using var doc = JsonDocument.Parse(fileIn);

var csv = Convert(doc.RootElement, "");
File.WriteAllText(outFileName, csv);

string Convert(JsonElement elem, string name)
{
    switch (elem.ValueKind)
    {
        case JsonValueKind.String:
        case JsonValueKind.Number:
        case JsonValueKind.True:
        case JsonValueKind.False:
        case JsonValueKind.Null:
            return $"{name},{elem.GetRawText()}";
        case JsonValueKind.Array:
            return string.Join("\n", elem.EnumerateArray().Select((e, i) => Convert(e, $"{name}[{i}]")));
        case JsonValueKind.Object:
            return string.Join("\n", elem.EnumerateObject().Select(p => Convert(p.Value, name + "." + p.Name)));
        case JsonValueKind.Undefined:
            return "";
        default:
            throw new Exception($"Could not parse element: {elem.ToString()}");
    }
}
