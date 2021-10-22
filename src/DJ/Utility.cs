using CaseExtensions;

namespace Andtech
{

	class Utility
	{

		public static string Standardize(string x)
		{
			x = x.ToKebabCase();
			x = x.Replace("-", " ");
			return x;
		}
	}
}
