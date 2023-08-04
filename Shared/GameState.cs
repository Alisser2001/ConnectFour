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
		(int, int, string) consecutiveHorizontalPieces;
		//(int, string) consecutiveDiagonalPieces;
		//Arreglar error: EN algun punto cuando pongo dos fichas juntas en las dos ultimas casillas, el juego se tilda
		int column;
		Random random = new();
		for (int col = 0; col < 7; col++)
		{
			consecutiveHorizontalPieces = CountConsecutiveHorizontalPieces(col, 1);
			if (consecutiveHorizontalPieces.Item1 >= 2 && consecutiveHorizontalPieces.Item3 == "left" && consecutiveHorizontalPieces.Item2 > 0)
			{
				return consecutiveHorizontalPieces.Item2 - 1;
			}
			if (consecutiveHorizontalPieces.Item1 >= 2 && consecutiveHorizontalPieces.Item3 == "right" && col < 6 && (consecutiveHorizontalPieces.Item2 + consecutiveHorizontalPieces.Item1) <= 6)
			{
				return consecutiveHorizontalPieces.Item2 + consecutiveHorizontalPieces.Item1;
			}
		}
		for (int col = 0; col < 7; col++)
		{
			consecutiveHorizontalPieces = CountConsecutiveHorizontalPieces(col, 2);
			if (consecutiveHorizontalPieces.Item1 >= 2 && consecutiveHorizontalPieces.Item3 == "left" && consecutiveHorizontalPieces.Item2 > 0)
			{
				return consecutiveHorizontalPieces.Item2 - 1;
			}
			if (consecutiveHorizontalPieces.Item1 >= 2 && consecutiveHorizontalPieces.Item3 == "right" && col < 6 && (consecutiveHorizontalPieces.Item2 + consecutiveHorizontalPieces.Item1) <= 6)
			{
				return consecutiveHorizontalPieces.Item2 + consecutiveHorizontalPieces.Item1;
			}
		}
		/*for (int col = 0; col < 7; col++)
		{
			consecutiveDiagonalPieces = CountConsecutiveDiagonalPieces(col, 1);
			if (consecutiveDiagonalPieces.Item1 >= 2 && consecutiveDiagonalPieces.Item2 == "left-up" && col > 0)
			{
				if (col - consecutiveDiagonalPieces.Item1 < 0)
				{
					return col - consecutiveDiagonalPieces.Item1;
				}
			}
			if (consecutiveDiagonalPieces.Item1 >= 2 && consecutiveDiagonalPieces.Item2 == "right-up" && col < 6)
			{
				if (col + consecutiveDiagonalPieces.Item1 > 6)
				{
					return col + consecutiveDiagonalPieces.Item1;
				}
			}
			if (consecutiveDiagonalPieces.Item1 >= 2 && consecutiveDiagonalPieces.Item2 == "left-down" && col > 0)
			{
				if (col - consecutiveDiagonalPieces.Item1 < 0)
				{
					return col - consecutiveDiagonalPieces.Item1;
				}
			}
			if (consecutiveDiagonalPieces.Item1 >= 2 && consecutiveDiagonalPieces.Item2 == "right-down" && col < 6)
			{
				if (col + consecutiveDiagonalPieces.Item1 > 6)
				{
					return col + consecutiveDiagonalPieces.Item1;
				}
			}
		}
		for (int col = 0; col < 7; col++)
		{
			consecutiveDiagonalPieces = CountConsecutiveDiagonalPieces(col, 2);
			if (consecutiveDiagonalPieces.Item1 >= 2 && consecutiveDiagonalPieces.Item2 == "left-up" && col > 0)
			{
				if (col - consecutiveDiagonalPieces.Item1 < 0)
				{
					return col - consecutiveDiagonalPieces.Item1;
				}
			}
			if (consecutiveDiagonalPieces.Item1 >= 2 && consecutiveDiagonalPieces.Item2 == "right-up" && col < 6)
			{
				if (col + consecutiveDiagonalPieces.Item1 > 6)
				{
					return col + consecutiveDiagonalPieces.Item1;
				}
			}
			if (consecutiveDiagonalPieces.Item1 >= 2 && consecutiveDiagonalPieces.Item2 == "left-down" && col > 0)
			{
				if (col - consecutiveDiagonalPieces.Item1 < 0)
				{
					return col - consecutiveDiagonalPieces.Item1;
				}
			}
			if (consecutiveDiagonalPieces.Item1 >= 2 && consecutiveDiagonalPieces.Item2 == "right-down" && col < 6)
			{
				if (col + consecutiveDiagonalPieces.Item1 > 6)
				{
					return col + consecutiveDiagonalPieces.Item1;
				}
			}
		}*/
		for (int col = 0; col < 7; col++)
		{
			consecutiveVerticalPieces = CountConsecutiveVerticalPieces(col, 1);
			if (consecutiveVerticalPieces >= 3 && TheBoard[col] == 0)
			{
				return col;
			}
		}
		for (int col = 0; col < 7; col++)
		{
			consecutiveVerticalPieces = CountConsecutiveVerticalPieces(col, 2);
			if (consecutiveVerticalPieces >= 2 && TheBoard[col] == 0)
			{
				return col;
			}
		}
		do
		{
			column = random.Next(0, 7);
		} while (TheBoard[column] != 0);
		return column;
	}

	private int CountConsecutiveVerticalPieces(int column, int playerNumber)
	{
		int consecutiveCount = 0;
		for (int row = 5; row >= 0; row--)
		{
			int index = row * 7 + column;
			if (TheBoard[index] == playerNumber)
			{
				consecutiveCount++;
				if (consecutiveCount >= 2 && index - 7 >= 0 && TheBoard[index - 7] == 0)
				{
					return consecutiveCount;
				}
			}
			else
			{
				consecutiveCount = 0;
			}
		}
		return 0;
	}

	private (int, int, string) CountConsecutiveHorizontalPieces(int column, int playerNumber)
	{
		int consecutiveCount = 0;
		for (int row = 5; row >= 0; row--)
		{	
			int index = row * 7;
			for (int col = column; col < 7; col++)
			{
				//Con este logro recorrer todas las posiciones
				/*if(TheBoard[col + index] == 0){
					return (consecutiveCount, "none");
				}*/
				//Hallar la forma de guardar y mandar el valor de la columna desde donde empieza a contar
				if (TheBoard[col + index] == playerNumber)
				{
					consecutiveCount++;
					if (consecutiveCount >= 2 && TheBoard[col + index + 1] == 0 && (col + index + 8 > 41 || TheBoard[col + index + 8] != 0))
					{
						return (consecutiveCount, (col-consecutiveCount)+1, "right");
					}
					if (consecutiveCount >= 2 && TheBoard[col + index - consecutiveCount] == 0 && (col + index + 6 > 41 || TheBoard[col + index + 6] != 0))
					{
						return (consecutiveCount, (col-consecutiveCount)+1, "left");
					}
				}
				else
				{
					consecutiveCount = 0;
				}
			}
		}
		return (consecutiveCount, 0, "left");
	}

	/*private (int, string) CountConsecutiveDiagonalPieces(int column, int playerNumber)
	{
		int consecutiveCount = 0;
		for (int row = 5; row >= 0; row--)
		{
			int index = row * 7;
			int flag = 0;
			//Esquina superior izquierda
			for (int col = 6; col >= column; col--)
			{
				if (col != 0 && TheBoard[col] == 0 && TheBoard[col + index - flag] == playerNumber)
				{
					consecutiveCount++;
					if (consecutiveCount >= 2 && col != 0 && TheBoard[col + index - 8] == 0 && TheBoard[col + index - 1] != 0)
					{
						return (consecutiveCount, "left-up");
					}
				}
				else
				{
					consecutiveCount = 0;
				}
				flag += 7;
			}
			flag = 0;
			//Esquina superior derecha
			for (int col = column; col < 7; col++)
			{
				if (col != 6 && TheBoard[col] == 0 && TheBoard[col + index - flag] == playerNumber)
				{
					consecutiveCount++;
					if (consecutiveCount >= 2 && col != 6 && TheBoard[col + index - 6] == 0 && TheBoard[col + index + 1] != 0)
					{
						return (consecutiveCount, "right-up");
					}
				}
				else
				{
					consecutiveCount = 0;
				}
				flag += 7;
			}
			flag = 0;
			//Esquina inferior izquierda
			for (int col = 6; col >= column; col--)
			{
				if (col != 0 && TheBoard[col + index + flag] < 42 && TheBoard[col + index + flag] == playerNumber)
				{
					consecutiveCount++;
					if (consecutiveCount >= 2 && col != 0 && TheBoard[col + index + 7] < 42 && TheBoard[col + index + 6] == 0)
					{
						return (consecutiveCount, "left-dowm");
					}
				}
				else
				{
					consecutiveCount = 0;
				}
				flag += 7;
			}
			flag = 0;
			//Esquina inferior derecha
			for (int col = column; col < 7; col++)
			{
				if (col != 6 && TheBoard[col + index + flag] < 42 && TheBoard[col + index + flag] == playerNumber)
				{
					consecutiveCount++;
					if (consecutiveCount >= 2 && col != 6 && TheBoard[col + index + 7] < 42 && TheBoard[col + index + 8] == 0)
					{
						return (consecutiveCount, "right-down");
					}
				}
				else
				{
					consecutiveCount = 0;
				}
				flag += 7;
			}
		}
		return (consecutiveCount, "none");
	}*/

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