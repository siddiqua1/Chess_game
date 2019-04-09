using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Chessman {

	private void Start(){
		point = 3;
	}

	public override bool[,] PossibleMove(){
		bool[,] r = new bool[8, 8];

		Chessman c;
		int i, j;

		//Diagonal Up Left
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i--;
			j++;
			if (i < 0 || j >= 8)
				break;
			c = BoardManager.Instance.Chessmans [i, j];
			if (c == null)
				r [i, j] = true;
			else {
				if (c.isWhite != isWhite)
					r [i, j] = true;
				break;
			}
		}

		//Diagonal Up Right
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i++;
			j++;
			if (i >= 8 || j >= 8)
				break;
			c = BoardManager.Instance.Chessmans [i, j];
			if (c == null)
				r [i, j] = true;
			else {
				if (c.isWhite != isWhite)
					r [i, j] = true;
				break;
			}
		}

		//Diagonal Down Left
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i--;
			j--;
			if (i < 0 || j < 0)
				break;
			c = BoardManager.Instance.Chessmans [i, j];
			if (c == null)
				r [i, j] = true;
			else {
				if (c.isWhite != isWhite)
					r [i, j] = true;
				break;
			}
		}

		//Diagonal Down Right
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i++;
			j--;
			if (i >= 8 || j < 0)
				break;
			c = BoardManager.Instance.Chessmans [i, j];
			if (c == null)
				r [i, j] = true;
			else {
				if (c.isWhite != isWhite)
					r [i, j] = true;
				break;
			}
		}

		return r;
	}

	public override bool[,] ProtectedPieces ()
	{
		bool[,] r = new bool[8, 8];

		Chessman c;
		int i, j;

		//Diagonal Up Left
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i--;
			j++;
			if (i < 0 || j >= 8)
				break;
			c = BoardManager.Instance.Chessmans [i, j];
			if (c == null)
				r [i, j] = false;
			else {
				if (c.isWhite == isWhite)
					r [i, j] = true;
				break;
			}
		}

		//Diagonal Up Right
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i++;
			j++;
			if (i >= 8 || j >= 8)
				break;
			c = BoardManager.Instance.Chessmans [i, j];
			if (c == null)
				r [i, j] = false;
			else {
				if (c.isWhite == isWhite)
					r [i, j] = true;
				break;
			}
		}

		//Diagonal Down Left
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i--;
			j--;
			if (i < 0 || j < 0)
				break;
			c = BoardManager.Instance.Chessmans [i, j];
			if (c == null)
				r [i, j] = false;
			else {
				if (c.isWhite == isWhite)
					r [i, j] = true;
				break;
			}
		}

		//Diagonal Down Right
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i++;
			j--;
			if (i >= 8 || j < 0)
				break;
			c = BoardManager.Instance.Chessmans [i, j];
			if (c == null)
				r [i, j] = false;
			else {
				if (c.isWhite == isWhite)
					r [i, j] = true;
				break;
			}
		}

		return r;
	}
}
