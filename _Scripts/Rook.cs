using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Chessman {


	private void Start(){
		point = 5;
		hasNotMoved = true;
	}
		


	public override bool[,] PossibleMove(){
		bool[,] r = new bool[8, 8];

		Chessman c;
		int i;

		//Right
		i = CurrentX;
		while (true) {
			i++;
			if (i >= 8)
				break;
			c = BoardManager.Instance.Chessmans [i, CurrentY];
			if (c == null)
				r [i, CurrentY] = true;
			else {
				if (c.isWhite != isWhite)
					r [i, CurrentY] = true;
				break;
			}
		}

		//Left
		i = CurrentX;
		while (true) {
			i--;
			if (i < 0)
				break;
			c = BoardManager.Instance.Chessmans [i, CurrentY];
			if (c == null)
				r [i, CurrentY] = true;
			else {
				if (c.isWhite != isWhite)
					r [i, CurrentY] = true;
				break;
			}
		}

		//Up
		i = CurrentY;
		while (true) {
			i++;
			if (i >= 8)
				break;
			c = BoardManager.Instance.Chessmans [CurrentX, i];
			if (c == null)
				r [CurrentX, i] = true;
			else {
				if (c.isWhite != isWhite)
					r [CurrentX, i] = true;
				break;
			}
		}

		//Down
		i = CurrentY;
		while (true) {
			i--;
			if (i < 0)
				break;
			c = BoardManager.Instance.Chessmans [CurrentX, i];
			if (c == null)
				r [CurrentX, i] = true;
			else {
				if (c.isWhite != isWhite)
					r [CurrentX, i] = true;
				break;
			}
		}

		return r;
	}

	public override bool[,] ProtectedPieces ()
	{
		bool[,] r = new bool[8, 8];

		Chessman c;
		int i;

		//Right
		i = CurrentX;
		while (true) {
			i++;
			if (i >= 8)
				break;
			c = BoardManager.Instance.Chessmans [i, CurrentY];
			if (c == null)
				r [i, CurrentY] = false;
			else {
				if (c.isWhite == isWhite)
					r [i, CurrentY] = true;
				break;
			}
		}

		//Left
		i = CurrentX;
		while (true) {
			i--;
			if (i < 0)
				break;
			c = BoardManager.Instance.Chessmans [i, CurrentY];
			if (c == null)
				r [i, CurrentY] = false;
			else {
				if (c.isWhite == isWhite)
					r [i, CurrentY] = true;
				break;
			}
		}

		//Up
		i = CurrentY;
		while (true) {
			i++;
			if (i >= 8)
				break;
			c = BoardManager.Instance.Chessmans [CurrentX, i];
			if (c == null)
				r [CurrentX, i] = false;
			else {
				if (c.isWhite == isWhite)
					r [CurrentX, i] = true;
				break;
			}
		}

		//Down
		i = CurrentY;
		while (true) {
			i--;
			if (i < 0)
				break;
			c = BoardManager.Instance.Chessmans [CurrentX, i];
			if (c == null)
				r [CurrentX, i] = false;
			else {
				if (c.isWhite == isWhite)
					r [CurrentX, i] = true;
				break;
			}
		}

		return r;
	}
}
