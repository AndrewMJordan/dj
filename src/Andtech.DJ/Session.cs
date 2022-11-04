using Andtech.DJ.Models;
using Andtech.DJ.Utility;

namespace Andtech.DJ
{
	internal class Session
	{
		public Frecency Frecency { get; set; }
		public string MusicRoot { get; set; }
		public Database Index { get; set; }
		public string IndexPath { get; set; }

		public static Session Instance { get; set; }

		public void CommitIndex()
		{
			Database.Write(IndexPath, Index);
		}
	}
}
