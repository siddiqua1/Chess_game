using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMan {
	
	public float[] myWeights;
	/*
	 * 0 - myThreat
	 * 1 - myProtection
	 * 2 - myMovement
	 * 3 - myPoints
	 * 4 - theirThreat
	 * 5 - theirProtection
	 * 6 - thierMovement
	 * 7 - theirPoints
	 */

	public int n;

	private Dictionary<RobotMan, int[]> winHistory;
	public bool isWhite;
	/*
	 * int[0] = wins
	 * int[1] = loses
	 * int[2] = draws
	 * int[3] = number of games
	 * 
	 */

	private BoardManager b;

	void Start(){
		b = BoardManager.Instance;
	}
		

	public void updateScore(RobotMan opp, int gameState){
		if (gameState == 2) {
			//Game is not actually finished
			return;
		}
		if (winHistory.ContainsKey (opp)) {
			if (gameState == 1) {
				winHistory [opp] [0] += 1;
			} else if (gameState == -1) {
				winHistory [opp] [1] += 1;
			} else {
				winHistory [opp] [2] += 1;
			}
			winHistory [opp] [3] += 1;
		}
		else {
			winHistory.Add (opp, new int[4]);
		}
	}

	public float winsVersusMe(RobotMan opp){
		/*
		 * Provides the win ratio of an opponent against this RobotMan if they have battled 
		 * otherwise -1
		 */
		if (winHistory.ContainsKey (opp) && winHistory[opp][3] != 0) {
			return (winHistory [opp] [0] + 0.0f) / winHistory [opp] [3];
		} 
		else {
			return -1f;
		}
	}

	public float losesVersusMe(RobotMan opp){
		/*
		 * Provides the loss ratio of an opponent against this RobotMan if they have battled
		 * otherwise -1
		 */
		if (winHistory.ContainsKey (opp) && winHistory [opp] [3] != 0) {
			return (winHistory [opp] [1] + 0.0f) / winHistory [opp] [3];
		} else {
			return -1f;
		}
	}

	public float drawsVersusMe(RobotMan opp){
		/*
		 * Provides the draw ratio of an opponent against this RobotMan if they have battled
		 * otherwise -1
		 */
		if (winHistory.ContainsKey (opp) && winHistory [opp] [3] != 0) {
			return (winHistory [opp] [2] + 0.0f) / winHistory [opp] [3];
		} else {
			return -1f;
		}
	}

	public int[] getMyBestMove(){
		int[] move = new int[4]{-1, -1, -1, -1};
		float scoreOfBoard = float.MinValue;
		bool[,] legalMoves;

		if (myWeights == null) {
			return move;
		}

		foreach (Chessman c in b.Chessmans) {
			if (c != null && c.isWhite == isWhite) {
				b.SelectChessman (c.CurrentX, c.CurrentY);
				legalMoves = c.PossibleMove ();
				for (int i = 0; i < 8; i++) {
					for (int j = 0; j < 8; j++) {
						if (legalMoves [i, j]) {
							b.pseudoMove (i, j);
							if (b.isMyKingNotInCheck (isWhite) && b.evaluateBoardState(isWhite, b.Chessmans, myWeights) > scoreOfBoard) {
								scoreOfBoard = b.evaluateBoardState (isWhite, b.Chessmans, myWeights);
								move [0] = c.CurrentX;
								move [1] = c.CurrentY;
								move [2] = i;
								move [3] = j;
							}
							b.UndoMove ();
						}
					}
				}
			}
		}

		return move;




	}
}
