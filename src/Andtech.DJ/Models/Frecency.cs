using System;

namespace Andtech.DJ.Utility
{

	internal class Frecency
	{
		public readonly double HalfLife;
		public readonly double DecayFactor;

		public Frecency(double halfLife)
		{
			HalfLife = halfLife;
			DecayFactor = Math.Log(2) / HalfLife;
		}

		public DateTime IncreaseScore(DateTime criticalDate)
		{
			var tau = Math.Abs((DateTime.UtcNow - criticalDate).TotalDays);
			var score = Math.Exp(DecayFactor * tau);
			var nextScore = score + 1.0 * Math.Exp(-DecayFactor * tau);
			var nextTau = Math.Log(nextScore) / DecayFactor;
			return DateTime.UtcNow + TimeSpan.FromDays(nextTau);
		}

		public double Decode(DateTime criticalDate)
		{
			var tau = (DateTime.UtcNow - criticalDate).TotalDays;
			return Math.Exp(-DecayFactor * tau);
		}
	}
}
