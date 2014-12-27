namespace SjisUnzip
{
	/// <summary>
	/// Bootstrapper to get out of the static context.
	/// </summary>
	static class AppRunner
	{
		static void Main(string[] args)
		{
			var mainApp = new SjisUnzipApp();
			mainApp.Main(args);
		} 
	}
}