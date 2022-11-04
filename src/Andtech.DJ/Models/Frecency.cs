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

		public DateTime IncreaseScore(DateTime freqDate)
		{
			var tau = Math.Abs((DateTime.UtcNow - freqDate).TotalDays);
			var score = Math.Exp(DecayFactor * tau);
			var nextScore = score + 1.0 * Math.Exp(-DecayFactor * tau);
			var nextTau = Math.Log(nextScore) / DecayFactor;
			return DateTime.UtcNow + TimeSpan.FromDays(nextTau);
		}

		public double Decode(DateTime freqDate)
		{
			var tau = (DateTime.UtcNow - freqDate).TotalDays;
			return Math.Exp(-DecayFactor * tau);
		}
	}
}
