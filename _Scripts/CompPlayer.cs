using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CompPlayer : MonoBehaviour {

	private static BoardManager b;
	private static int maxDepth = 4;
	private int[] nextMoveLocation = new int[4];

	public CompPlayer(BoardManager board){
		b = board;
	}

	//{{currentx, currenty, destinationx, destinationy}, {...}}

	public int[] getLegalMoves(int player){
		
		/*foreach (Chessman c in b.Chessmans) {
			int x = c.CurrentX;
			int y = c.CurrentY;
			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					
				}
			}

		}*/

		int[] move = new int[4]{ -1, -1, -1, -1 };
		float scoreOfMove = float.MinValue + 1;
		float tempPoint = float.MinValue;
		bool[,] moves;

		for (int a = 0; a < 8; a++) {
			for (int c = 0; c < 8; c++) {
				if (b.Chessmans [a, c] != null && !b.Chessmans [a, c].isWhite) {
					b.SelectChessman (a, c);
					moves = b.Chessmans [a, c].PossibleMove ();

					for (int i = 0; i < 8; i++) {
						for (int j = 0; j < 8; j++) {

							if (moves [i, j]) {
								b.pseudoMove (i, j);

								if (b.isMyKingNotInCheck (false)) {
									tempPoint = evaluationFunction (b);
									if (scoreOfMove < tempPoint) {
										move [0] = a;
										move [1] = c;
										move [2] = i;
										move [3] = j;
										scoreOfMove = tempPoint;
										tempPoint = float.MinValue;
										Debug.Log (move[0] + " "  + move[1] + " "  + move[2] + " "  + move[3] + " "  + " Score:" + scoreOfMove);
									}
								}

								b.UndoMove ();
								b.pseudoActiveChessman = b.ClearList (b.pseudoActiveChessman);
							}
						}
					}

					b.selectedChessman = null;


				}	
			}
		}


		return nextMoveLocation;
	}


	public int evaluationFunction(BoardManager board){
		/* f(p) = 9(Q-Q')
       + 5(R-R')
       + 3(B-B' + N-N')
       + 1(P-P')
       - 0.5(D-D' + S-S' + I-I')
       + 0.1(M-M') + ...
 
		KQRBNP = number of kings, queens, rooks, bishops, knights and pawns
		D,S,I = doubled, blocked and isolated pawns
		M = Mobility (the number of legal moves)
		*/

		int diffQ = 0;
		int diffR = 0;
		int diffB = 0;
		int diffN = 0;
		int diffP = 0;

		foreach(Chessman c in board.Chessmans){
			if (c.GetType () == typeof(Queen) && ((c.isWhite == true && board.whiteIsPlayer == true) || (c.isWhite == false && board.blackIsPlayer == true))) {
				diffQ -= 1;
			}
			else if (c.GetType () == typeof(Queen) && ((c.isWhite == false && board.whiteIsPlayer == true) || (c.isWhite == true && board.blackIsPlayer == false))) {
				diffQ += 1;
			}

			if (c.GetType () == typeof(Rook) && ((c.isWhite == true && board.whiteIsPlayer == true) || (c.isWhite == false && board.blackIsPlayer == true))) {
				diffR -= 1;
			}
			else if (c.GetType () == typeof(Rook) && ((c.isWhite == false && board.whiteIsPlayer == true) || (c.isWhite == true && board.blackIsPlayer == false))) {
				diffR += 1;
			}

			if (c.GetType () == typeof(Bishop) && ((c.isWhite == true && board.whiteIsPlayer == true) || (c.isWhite == false && board.blackIsPlayer == true))) {
				diffB -= 1;
			}
			else if (c.GetType () == typeof(Bishop) && ((c.isWhite == false && board.whiteIsPlayer == true) || (c.isWhite == true && board.blackIsPlayer == false))) {
				diffB += 1;
			}

			if (c.GetType () == typeof(Knight) && ((c.isWhite == true && board.whiteIsPlayer == true) || (c.isWhite == false && board.blackIsPlayer == true))) {
				diffN -= 1;
			}
			else if (c.GetType () == typeof(Knight) && ((c.isWhite == false && board.whiteIsPlayer == true) || (c.isWhite == true && board.blackIsPlayer == false))) {
				diffN += 1;
			}

			if (c.GetType () == typeof(Pawn) && ((c.isWhite == true && board.whiteIsPlayer == true) || (c.isWhite == false && board.blackIsPlayer == true))) {
				diffP -= 1;
			}
			else if (c.GetType () == typeof(Pawn) && ((c.isWhite == false && board.whiteIsPlayer == true) || (c.isWhite == true && board.blackIsPlayer == false))) {
				diffP += 1;
			}
		}

		int score = 9 * diffQ + 5 * diffR + 3 * (diffB + diffN) + diffP;

		return score;

	}

	public int minimax(int player, int depth, int alpha, int beta){

		/* This is the minimax function which should give the best move for the computer by changing a field called nextMoveLocation
		 * Parameters: 
		 *     player: int
		 *         A 1 would correspond to the computer player and 2 is the player
		 *     depth: int
		 *         This is how far deep down the tree the minimax is at right now. Checks with respect to maxDepth.
		 *     alpha and beta: int
		 *         The alpha and beta to cut down on the time
		 * 
		 * Returns:
		 *     score: int
		 *         This is the best score for the player (minimized if 2, maximized if 1)
		 * 
		 */

		if (beta <= alpha) {
			if (player == 1)
				return int.MaxValue;
			return int.MinValue;
		}

		int win = b.winState ();
		if ((win == 1 && b.whiteIsPlayer == false) || (win == -1 && b.whiteIsPlayer == true))
			return int.MaxValue / 2;
		else if ((win == 1 && b.whiteIsPlayer == true) || (win == -1 && b.whiteIsPlayer == false))
			return int.MinValue / 2;
		else if (win == 0)
			return 0;

		if (depth == maxDepth)
			return evaluationFunction (b);

		int min = int.MaxValue;
		int max = int.MinValue;

		//for all possible moves for the player

		for(int i = 0; i<3; i++){
			int currentScore = 0;
			//If there exists a legal move, continue
			int[] currentMove = new int[4];
			if(player == 1){
				//Do the current legal move
				currentScore = minimax(2, depth+1, alpha, beta);
				if(depth == 0){
					if(currentScore > max) nextMoveLocation = currentMove; //Should be current move
					if(currentScore == int.MaxValue/2){
						//Undo move
						b.UndoMove();
						break;
					}
				}

				max = Math.Max(currentScore, max);
				alpha = Math.Max(currentScore, alpha);

			}

			else if(player == 2){
				//do move
				
				currentScore = minimax(1, depth+1, alpha, beta);
				min = Math.Min(currentScore, min);
				beta = Math.Min(currentScore, beta);
			}
			b.UndoMove ();
			if(currentScore == int.MaxValue || currentScore == int.MinValue) break;
		}
		if(player == 1) return max;
		else return min;

	}
}

