namespace ConnectFour.Shared;

public class GameState
{

	static GameState()
	{
		CalculateWinningPlaces();
	}
	public enum WinState
	{
		No_Winner = 0,
		Player1_Wins = 1,
		Player2_Wins = 2,
		Tie = 3
	}

	public int PlayerTurn => TheBoard.Count(x => x != 0) % 2 + 1;
	public int CurrentTurn { get { return TheBoard.Count(x => x != 0); } }
	public static readonly List<int[]> WinningPlaces = new();
	public bool TwoPlayers = false;
	public void Two_Player()
	{
		TwoPlayers = true;
	}
	public void One_Player()
	{
		TwoPlayers = false;
	}

	//Cada que cambia el estado del juego, recalcula por columnas, filas y diagonales 
	public static void CalculateWinningPlaces()
	{
		for (byte row = 0; row < 6; row++)
		{
			byte rowCol1 = (byte)(row * 7);
			byte rowColEnd = (byte)((row + 1) * 7 - 1);
			byte checkCol = rowCol1;
			while (checkCol <= rowColEnd - 3)
			{
				WinningPlaces.Add(new int[] {
					checkCol,
					(byte)(checkCol + 1),
					(byte)(checkCol + 2),
					(byte)(checkCol + 3)
					});
				checkCol++;
			}
		}
		for (byte col = 0; col < 7; col++)
		{
			byte colRow1 = col;
			byte colRowEnd = (byte)(35 + col);
			byte checkRow = colRow1;
			while (checkRow <= 14 + col)
			{
				WinningPlaces.Add(new int[] {
					checkRow,
					(byte)(checkRow + 7),
					(byte)(checkRow + 14),
					(byte)(checkRow + 21)
					});
				checkRow += 7;
			}
		}
		for (byte col = 0; col < 4; col++)
		{
			byte colRow1 = (byte)(21 + col);
			byte colRowEnd = (byte)(35 + col);
			byte checkPos = colRow1;
			while (checkPos <= colRowEnd)
			{
				WinningPlaces.Add(new int[] {
					checkPos,
					(byte)(checkPos - 6),
					(byte)(checkPos - 12),
					(byte)(checkPos - 18)
					});
				checkPos += 7;
			}
		}
		for (byte col = 0; col < 4; col++)
		{
			byte colRow1 = (byte)(0 + col);
			byte colRowEnd = (byte)(14 + col);
			byte checkPos = colRow1;
			while (checkPos <= colRowEnd)
			{
				WinningPlaces.Add(new int[] {
					checkPos,
					(byte)(checkPos + 8),
					(byte)(checkPos + 16),
					(byte)(checkPos + 24)
					});
				checkPos += 7;
			}
		}
	}


	public WinState CheckForWin()
	{
		if (TheBoard.Count(x => x != 0) < 7) return WinState.No_Winner;
		foreach (var scenario in WinningPlaces)
		{
			if (TheBoard[scenario[0]] == 0) continue;
			if (TheBoard[scenario[0]] ==
				TheBoard[scenario[1]] &&
				TheBoard[scenario[1]] ==
				TheBoard[scenario[2]] &&
				TheBoard[scenario[2]] ==
				TheBoard[scenario[3]]) return (WinState)TheBoard[scenario[0]];
		}
		if (TheBoard.Count(x => x != 0) == 42) return WinState.Tie;
		return WinState.No_Winner;
	}


	public byte PlayPiece(int column)
	{
		if (CheckForWin() != 0) throw new ArgumentException("Game is over");
		if (TheBoard[column] != 0) throw new ArgumentException("Column is full");
		var landingSpot = column;
		for (var i = column; i < 42; i += 7)
		{
			if (TheBoard[landingSpot + 7] != 0) break;
			landingSpot = i;
		}
		TheBoard[landingSpot] = PlayerTurn;
		return ConvertLandingSpotToRow(landingSpot);
	}

	public byte PlayAgainstMachine(int column)
	{
		if (CheckForWin() != 0) throw new ArgumentException("Game is over");
		var landingSpot = column;
		for (var i = column; i < 42; i += 7)
		{
			if (TheBoard[landingSpot + 7] != 0) break;
			landingSpot = i;
		}
		TheBoard[landingSpot] = PlayerTurn;
		return ConvertLandingSpotToRow(landingSpot);
	}

	public int RandomColumn()
	{
		int consecutiveVerticalPieces;
		Random random = new Random();
		for (int col = 0; col < 7; col++)
		{
			consecutiveVerticalPieces = CountConsecutiveVerticalPieces(col, 2);
			if (consecutiveVerticalPieces >= 2 && TheBoard[col] == 0)
			{
				return col;
			}
		}
		for (int col = 0; col < 7; col++)
		{
			consecutiveVerticalPieces = CountConsecutiveVerticalPieces(col, 1);
			if (consecutiveVerticalPieces >= 2 && TheBoard[col] == 0)
			{
				return col;
			}
		}
		int column;
		do
		{
			column = random.Next(0, 7);
		} while (TheBoard[column] != 0);
		return column;
	}

	private int CountConsecutiveVerticalPieces(int column, int playerNumber)
	{
		int consecutiveCount = 0;
		for (int row = 0; row < 6; row++)
		{
			int index = row * 7 + column;
			if (TheBoard[index] == playerNumber)
			{
				consecutiveCount++;
			}
			else
			{
				consecutiveCount = 0;
			}
		}
		Console.WriteLine("Col: " + column + " - Count: " + consecutiveCount);
		if (consecutiveCount >= 2)
		{
			return consecutiveCount;
		} 
		else
		{
			return 0;
		}
	}


	public List<int> TheBoard { get; private set; } = new List<int>(new int[42]);


	public void ResetBoard()
	{
		TheBoard = new List<int>(new int[42]);
	}


	private byte ConvertLandingSpotToRow(int landingSpot)
	{
		return (byte)(Math.Floor(landingSpot / (decimal)7) + 1);
	}

}