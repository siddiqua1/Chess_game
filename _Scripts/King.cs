using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman {

	private void Start(){
		point = 100;
		hasNotMoved = true;
	}

	public override bool[,] PossibleMove(){
		bool[,] r = new bool[8, 8];

		Chessman c;
		int i, j;

		//Top Side
		i = CurrentX - 1;
		j = CurrentY + 1;
		if (CurrentY != 7) {
			for (int k = 0; k < 3; k++) {
				if (i >= 0 && i < 8) {
					c = BoardManager.Instance.Chessmans [i, j];
					if (c == null)
						r [i, j] = true;
					else if (isWhite != c.isWhite)
						r [i , j] = true;
				}

				i++;
			}
		}

		//Down Side
		i = CurrentX - 1;
		j = CurrentY - 1;
		if (CurrentY != 0) {
			for (int k = 0; k < 3; k++) {
				if (i >= 0 && i < 8) {
					c = BoardManager.Instance.Chessmans [i, j];
					if (c == null)
						r [i, j] = true;
					else if (isWhite != c.isWhite)
						r [i , j] = true;
				}

				i++;
			}
		}

		//Middle Left 
		if (CurrentX != 0){
			c = BoardManager.Instance.Chessmans [CurrentX - 1, CurrentY];
			if (c == null)
				r [CurrentX - 1, CurrentY] = true;
			else if (isWhite != c.isWhite) 
				r [CurrentX - 1, CurrentY] = true;
		}

		//Middle Right 
		if (CurrentX != 7){
			c = BoardManager.Instance.Chessmans [CurrentX + 1, CurrentY];
			if (c == null)
				r [CurrentX + 1, CurrentY] = true;
			else if (isWhite != c.isWhite) 
				r [CurrentX + 1, CurrentY] = true;
		}

		//Castle
		if(hasNotMoved && CurrentX == 4){
			//Right
			if (BoardManager.Instance.Chessmans[7, CurrentY] != null && BoardManager.Instance.Chessmans[7, CurrentY].GetType() == typeof(Rook) && BoardManager.Instance.Chessmans[7, CurrentY].hasNotMoved){
				//check is spaces are open
				if (BoardManager.Instance.Chessmans [5, CurrentY] == null && BoardManager.Instance.Chessmans [6, CurrentY] == null) {
					//Check if squares are in check WHITE
					int[,] T = BoardManager.Instance.blackThreat;
					if (T [4, CurrentY] == 0 && T [5, CurrentY] == 0 && T [6, CurrentY] == 0) {
						r [6, CurrentY] = true;
					}

					//Check if squares are in check BLACK
					T = BoardManager.Instance.whiteThreat;
					if (T [4, CurrentY] == 0 && T [5, CurrentY] == 0 && T [6, CurrentY] == 0) {
						r [6, CurrentY] = true;
					}
				}

			}
			//Left
			if (BoardManager.Instance.Chessmans[0, CurrentY] != null && BoardManager.Instance.Chessmans[0, CurrentY].GetType() == typeof(Rook) && BoardManager.Instance.Chessmans[0, CurrentY].hasNotMoved){
				//check is spaces are open
				if (BoardManager.Instance.Chessmans [2, CurrentY] == null && BoardManager.Instance.Chessmans [3, CurrentY] == null) {
					//Check if squares are in check WHITE
					int[,] T = BoardManager.Instance.blackThreat;
					if (T [4, CurrentY] == 0 && T [3, CurrentY] == 0 && T [2, CurrentY] == 0) {
						r [2, CurrentY] = true;
					}

					//Check if squares are in check BLACK
					T = BoardManager.Instance.whiteThreat;
					if (T [4, CurrentY] == 0 && T [3, CurrentY] == 0 && T [2, CurrentY] == 0) {
						r [2, CurrentY] = true;
					}
				}
			}
		}


		return r;
	}

	public override bool[,] ProtectedPieces ()
	{
		bool[,] r = new bool[8, 8];

		Chessman c;
		int i, j;

		//Top Side
		i = CurrentX - 1;
		j = CurrentY + 1;
		if (CurrentY != 7) {
			for (int k = 0; k < 3; k++) {
				if (i >= 0 && i < 8) {
					c = BoardManager.Instance.Chessmans [i, j];
					if (c == null)
						r [i, j] = false;
					else if (isWhite == c.isWhite)
						r [i , j] = true;
				}

				i++;
			}
		}

		//Down Side
		i = CurrentX - 1;
		j = CurrentY - 1;
		if (CurrentY != 0) {
			for (int k = 0; k < 3; k++) {
				if (i >= 0 && i < 8) {
					c = BoardManager.Instance.Chessmans [i, j];
					if (c == null)
						r [i, j] = false;
					else if (isWhite == c.isWhite)
						r [i , j] = true;
				}

				i++;
			}
		}

		//Middle Left 
		if (CurrentX != 0){
			c = BoardManager.Instance.Chessmans [CurrentX - 1, CurrentY];
			if (c == null)
				r [CurrentX - 1, CurrentY] = false;
			else if (isWhite == c.isWhite) 
				r [CurrentX - 1, CurrentY] = true;
		}

		//Middle Right 
		if (CurrentX != 7){
			c = BoardManager.Instance.Chessmans [CurrentX + 1, CurrentY];
			if (c == null)
				r [CurrentX + 1, CurrentY] = false;
			else if (isWhite == c.isWhite) 
				r [CurrentX + 1, CurrentY] = true;
		}



		return r;
	}
}
