using Andtech.Common.Frecency;

namespace Andtech.DJ
{
	internal class Session
	{
		public Frecency Frecency { get; set; }
		public string MusicRoot { get; set; }
		public Cache Index { get; set; }
		public string IndexPath { get; set; }

		public static Session Instance { get; set; }

		public void CommitIndex()
		{
			Cache.Write(IndexPath, Index);
		}
	}
}
