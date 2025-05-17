
using CardGames.GameType.Scoundral;
using CardGames.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

partial class Program
{
	private const int MF_BYCOMMAND = 0x00000000;
	public const int SC_SIZE = 0xF000;

	[DllImport("user32.dll")]
	public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
	[DllImport("user32.dll")]
	private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
	[DllImport("kernel32.dll", ExactSpelling = true)]
	private static extern IntPtr GetConsoleWindow();

	static void Main(string[] args)
	{

		DisableConsoleWindowResize();

		//TODO: Select Game (Auto pick Scoundral for now)
		//Console.WriteLine("Press ANY key to play Scoundral");
		//Console.ReadKey();

		// Create GameBoard (Use factory pattern to create game board or load saved gameboard)
		IGameBoard gameBoard = new ScoundralGameBoard();
		Thread.Sleep(1000);
		gameBoard.DrawGameRules();
		gameBoard.GameBoardActions.StartGame();
		while (!gameBoard.GameBoardActions.IsGameOver)
		{
			gameBoard.GameBoardActions.BeginningPhase();
			gameBoard.GameBoardActions.MainPhase();
			gameBoard.GameBoardActions.CombatPhase();
			gameBoard.GameBoardActions.EndPhase();
		}
		gameBoard.GameOver();
	}
	private static void DisableConsoleWindowResize()
	{
		IntPtr handle = GetConsoleWindow();
		IntPtr sysMenu = GetSystemMenu(handle, false);

		if (handle != IntPtr.Zero)
		{
			DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
		}
	}


}

