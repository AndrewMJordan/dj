using System;

namespace Andtech.Common.Frecency
{

	public class Frecency
	{
		public readonly double HalfLife;
		public readonly double DecayFactor;
		public readonly double DefaultBaseBonus = 1;

		public Frecency(double halfLife)
		{
			HalfLife = halfLife;
			DecayFactor = Math.Log(2) / HalfLife;
		}

		public DateTime IncreaseScore(DateTime criticalDate)
		{
			var tau = (DateTime.UtcNow - criticalDate.ToUniversalTime()).TotalDays;
			var score = Math.Exp(-DecayFactor * tau);
			Log.WriteLine($"Old score: {score}", ConsoleColor.Blue, Verbosity.silly);
			Log.WriteLine($"Old tau: {tau}", ConsoleColor.Blue, Verbosity.silly);
			var bonus = DefaultBaseBonus;
			Log.WriteLine($"New score: {score + DefaultBaseBonus}", ConsoleColor.Blue, Verbosity.silly);
			var nextTau = Math.Log(score + bonus) / DecayFactor;
			Log.WriteLine($"New tau: {nextTau}", ConsoleColor.Blue, Verbosity.silly);
			return DateTime.UtcNow + TimeSpan.FromDays(nextTau);
		}

		public double Decode(DateTime criticalDate)
		{
			var tau = (DateTime.UtcNow - criticalDate.ToUniversalTime()).TotalDays;
			return Math.Exp(-DecayFactor * tau);
		}
	}
}
