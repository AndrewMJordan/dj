using CaseExtensions;
using F23.StringSimilarity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
	static List<string> ValidExtensions = new List<string>()
	{
		".mp3",
		".mp4",
		".wav",
		".aiff",
		".flac"
	};

	class FileData
	{
		public string Path { get; set; }
		public double Score { get; set; }
	}

	static void Main(string[] args)
	{
		var root = ".";
		var query = string.Join(" ", args);
		query = query.ToKebabCase();

		Console.WriteLine($"'{query}'");
		var l = new MetricLCS();

		var files = Directory
			.EnumerateFiles(root, "*", SearchOption.AllDirectories)
			.Where(IsMusicFile);
		var datas = files
			.Select(x => new FileData { Path = x, Score = GetScore(x) });

		double GetScore(string path)
		{
			var filename = Path.GetFileNameWithoutExtension(path);
			var distance = l.Distance(query, filename);

			return distance;
		}

		var top = datas.OrderBy(x => x.Score).Take(10);
		var best = top.First();

		foreach (var element in top)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"{element.Path} ({element.Score})");
		}

		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.WriteLine($"{best.Path} ({best.Score})");
	}

	static bool IsMusicFile(string path)
	{
		return ValidExtensions.Contains(Path.GetExtension(path));
	}
}
