using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chessman: MonoBehaviour {

	public int CurrentX { set; get; }
	public int CurrentY { set; get; }
	public int point = 0;
	public bool isWhite;
	public bool hasNotMoved = false;
	public bool initiallySet = false;

	public virtual void SetPosition(int x, int y, bool pseudo){
		CurrentX = x;
		CurrentY = y;
		if (!initiallySet) {
			initiallySet = true;
		}
		if (initiallySet && !pseudo) {
			hasNotMoved = false;
		}
	}

	public virtual bool[,] PossibleMove(){
		/*
		 *
		 *Give a boolean board of possible moves and INCLUDES moves that are illegal due to check (use update for cehcks to fix)
		 *
		 */
		return new bool[8,8];
	}

	public virtual bool[,] ProtectedPieces(){
		/*
		 *
		 *Give a boolean board of places that are protected (by an ally piece) due to there ability to attack if if that piece were to be eaten
		 *
		 */
		return new bool[8,8];
	}
}
