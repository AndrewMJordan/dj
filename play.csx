#!/usr/bin/env dotnet-script

#r "nuget: CaseExtensions, 1.1.0"
#r "nuget: F23.StringSimilarity, 4.1.0"

using CaseExtensions;
using System.IO;
using System.Linq;
using F23.StringSimilarity;

var query = string.Join(" ", Args);
query = query.ToKebabCase();

Console.WriteLine($"'{query}'");
var l = new MetricLCS();

var bestScore = 10000000.0;
var best = string.Empty;
var files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories);
foreach (var file in files)
{
	var filename = Path.GetFileName(file);
	var distance = l.Distance(query, filename);
	Console.WriteLine($"{filename} {distance}");

	if (distance < bestScore)
	{
		bestScore = distance;
		best = file;
	}
}

Console.WriteLine("Hello world!");
Console.WriteLine($"{best} ({bestScore})");
