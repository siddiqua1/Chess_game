using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chessman {

	private void Start(){
		point = 3;
	}

	public override bool[,] PossibleMove(){
		bool[,] r = new bool[8, 8];

		//Up 2 Left 1
		KnightMove(CurrentX - 1, CurrentY + 2, ref r);
		//Up 2 Right 1
		KnightMove(CurrentX + 1, CurrentY + 2, ref r);

		//Right 2 Up 1
		KnightMove(CurrentX + 2, CurrentY + 1, ref r);
		//Right 2 Down 1
		KnightMove(CurrentX + 2, CurrentY - 1, ref r);

		//Left 2 Up 1
		KnightMove(CurrentX - 2, CurrentY + 1, ref r);
		//Left 2 Down 1
		KnightMove(CurrentX - 2, CurrentY - 1, ref r);

		//Down 2 Left 1
		KnightMove(CurrentX - 1, CurrentY - 2, ref r);
		//Down 2 Right 1
		KnightMove(CurrentX + 1, CurrentY - 2, ref r);

		return r;
	}

	public void KnightMove(int x, int y, ref bool[,] r){

		Chessman c;
		if (x >= 0 && x < 8 && y >= 0 && y < 8) {
			c = BoardManager.Instance.Chessmans [x, y];
			if (c == null)
				r [x, y] = true;
			else if (isWhite != c.isWhite) {
				r [x, y] = true;
			}
		}
	}

	public override bool[,] ProtectedPieces ()
	{
		bool[,] r = new bool[8, 8];

		//Up 2 Left 1
		KnightProtect(CurrentX - 1, CurrentY + 2, ref r);
		//Up 2 Right 1
		KnightProtect(CurrentX + 1, CurrentY + 2, ref r);

		//Right 2 Up 1
		KnightProtect(CurrentX + 2, CurrentY + 1, ref r);
		//Right 2 Down 1
		KnightProtect(CurrentX + 2, CurrentY - 1, ref r);

		//Left 2 Up 1
		KnightProtect(CurrentX - 2, CurrentY + 1, ref r);
		//Left 2 Down 1
		KnightProtect(CurrentX - 2, CurrentY - 1, ref r);

		//Down 2 Left 1
		KnightProtect(CurrentX - 1, CurrentY - 2, ref r);
		//Down 2 Right 1
		KnightProtect(CurrentX + 1, CurrentY - 2, ref r);

		return r;
	}

	public void KnightProtect(int x, int y, ref bool[,] r){

		Chessman c;
		if (x >= 0 && x < 8 && y >= 0 && y < 8) {
			c = BoardManager.Instance.Chessmans [x, y];
			if (c == null)
				r [x, y] = false;
			else if (isWhite == c.isWhite) {
				r [x, y] = true;
			}
		}
	}
}
