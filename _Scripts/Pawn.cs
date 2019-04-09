﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chessman {

	private void Start(){
		point = 1;
	}

	public override bool[,] PossibleMove(){
		bool[,] r = new bool[8, 8];
		Chessman c, c2;

		//White Pawn Move
		if(isWhite){
			//Diagonal Left
			if(CurrentX != 0 && CurrentY != 7){
				int[] e = BoardManager.Instance.EnPassantMove;
				if (e [0] == CurrentX - 1 && e [1] == CurrentY + 1)
					r [CurrentX - 1, CurrentY + 1] = true;

				c = BoardManager.Instance.Chessmans [CurrentX - 1, CurrentY + 1];
				if (c != null && !c.isWhite)
					r [CurrentX - 1, CurrentY + 1] = true;
			}

			//Diagonal Right
			if(CurrentX != 7 && CurrentY != 7){
				int[] e = BoardManager.Instance.EnPassantMove;
				if (e [0] == CurrentX + 1 && e [1] == CurrentY + 1)
					r [CurrentX + 1, CurrentY + 1] = true;

				c = BoardManager.Instance.Chessmans [CurrentX + 1, CurrentY + 1];
				if (c != null && !c.isWhite)
					r [CurrentX + 1, CurrentY + 1] = true;
			}

			//Middle
			if (CurrentY != 7){
				c = BoardManager.Instance.Chessmans [CurrentX, CurrentY + 1];
				if (c == null)
					r [CurrentX, CurrentY + 1] = true;
				
			}

			//Middle on first move
			if (CurrentY == 1){
				c = BoardManager.Instance.Chessmans [CurrentX, CurrentY + 1];
				c2 = BoardManager.Instance.Chessmans [CurrentX, CurrentY + 2];
				if (c == null && c2 == null) 
					r [CurrentX, CurrentY + 2] = true;
				
			}

		}
		else{
			//Diagonal Left
			if(CurrentX != 0 && CurrentY != 0){
				int[] e = BoardManager.Instance.EnPassantMove;
				if (e [0] == CurrentX - 1 && e [1] == CurrentY - 1)
					r [CurrentX - 1, CurrentY - 1] = true;

				c = BoardManager.Instance.Chessmans [CurrentX - 1, CurrentY - 1];
				if (c != null && c.isWhite)
					r [CurrentX - 1, CurrentY - 1] = true;
			}

			//Diagonal Right
			if(CurrentX != 7 && CurrentY != 0){
				int[] e = BoardManager.Instance.EnPassantMove;
				if (e [0] == CurrentX + 1 && e [1] == CurrentY - 1)
					r [CurrentX + 1, CurrentY - 1] = true;

				c = BoardManager.Instance.Chessmans [CurrentX + 1, CurrentY - 1];
				if (c != null && c.isWhite)
					r [CurrentX + 1, CurrentY - 1] = true;
			}

			//Middle
			if (CurrentY != 0){
				c = BoardManager.Instance.Chessmans [CurrentX, CurrentY - 1];
				if (c == null)
					r [CurrentX, CurrentY - 1] = true;

			}

			//Middle on first move
			if (CurrentY == 6){
				c = BoardManager.Instance.Chessmans [CurrentX, CurrentY - 1];
				c2 = BoardManager.Instance.Chessmans [CurrentX, CurrentY - 2];
				if (c == null && c2 == null) 
					r [CurrentX, CurrentY - 2] = true;

			}
		}

		return r;
	}

	public override bool[,] ProtectedPieces(){
		bool[,] r = new bool[8, 8];
		Chessman c;

		//White Pawn
		if (isWhite){
			//Diagonal Left
			if(CurrentX != 0 && CurrentY != 7){
				c = BoardManager.Instance.Chessmans [CurrentX - 1, CurrentY + 1];
				if (c != null && c.isWhite)
					r [CurrentX - 1, CurrentY + 1] = true;
			}

			//Diagonal Right
			if(CurrentX != 7 && CurrentY != 7){
				c = BoardManager.Instance.Chessmans [CurrentX + 1, CurrentY + 1];
				if (c != null && c.isWhite)
					r [CurrentX + 1, CurrentY + 1] = true;
			}
		}

		//Black Pawn
		else{
			//Diagonal Left
			if(CurrentX != 0 && CurrentY != 7){
				c = BoardManager.Instance.Chessmans [CurrentX - 1, CurrentY + 1];
				if (c != null && !c.isWhite)
					r [CurrentX - 1, CurrentY + 1] = true;
			}

			//Diagonal Right
			if(CurrentX != 7 && CurrentY != 7){
				c = BoardManager.Instance.Chessmans [CurrentX + 1, CurrentY + 1];
				if (c != null && !c.isWhite)
					r [CurrentX + 1, CurrentY + 1] = true;
			}
		}
		return r;
	}

}
