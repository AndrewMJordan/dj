using System;

namespace Andtech.DJ.Utility
{

	internal class Frecency
	{
		public readonly double HalfLife;
		public readonly double DecayFactor;
		public readonly double DefaultBaseBonus = 1.0;

		public Frecency(double halfLife)
		{
			HalfLife = halfLife;
			DecayFactor = Math.Log(2) / HalfLife;
		}

		public DateTime IncreaseScore(DateTime criticalDate)
		{
			var tau = Math.Abs((DateTime.UtcNow - criticalDate).TotalDays);
			var score = Math.Exp(-DecayFactor * tau);
			var bonus = DefaultBaseBonus;
			var nextTau = Math.Log(score + bonus) / DecayFactor;
			return DateTime.UtcNow + TimeSpan.FromDays(nextTau);
		}

		public double Decode(DateTime criticalDate)
		{
			var tau = (DateTime.UtcNow - criticalDate).TotalDays;
			return Math.Exp(-DecayFactor * tau);
		}
	}
}
