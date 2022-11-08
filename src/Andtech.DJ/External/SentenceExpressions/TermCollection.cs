using System.Collections;
using System.Collections.Generic;

namespace Andtech.Common.Text.SentenceExpressions
{

	public class TermCollection : IEnumerable<Term>
	{
		public bool Success { get; internal set; }
		public Term[] Terms { get; internal set; }

		public IEnumerator<Term> GetEnumerator()
		{
			return ((IEnumerable<Term>)Terms).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Terms.GetEnumerator();
		}
	}
}
