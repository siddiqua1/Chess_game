using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public int gameState;

	public bool isComputingMove = false;

	public bool whiteIsPlayer = true;
	public bool blackIsPlayer = true;

	public RobotMan robot1;
	public RobotMan robot2;

	public bool trainIsWhite = true;
	public bool isTraining = false;

	public float[] weights;

	public static BoardManager Instance { set; get; }
	private bool[,] allowedMoves { set; get; }

	public Chessman[,] Chessmans { set; get; }
	public Chessman selectedChessman;

	private const float TILE_SIZE = 1.0f;
	private const float TILE_OFFSET = 0.5f;

	private int selectionX = -1;
	private int selectionY = -1;

	public List<GameObject> chessmanPrefabs;
	private List<GameObject> activeChessman ;

	private Quaternion orientation = Quaternion.Euler(0, 90, 0);

	public bool isWhiteTurn = true;
	public int turnCount;

	public int[] EnPassantMove { set; get;}

	//on white threat, if piece is black it is threat, if white it protects, if null movement
	public int[,] whiteThreat { set; get;}
	public int[,] blackThreat { set; get;}


	public Chessman[] undoPieces;
	public int[] undoLocations;

	public List<GameObject> pseudoActiveChessman;

	//Keep track of the kings to easily see if in check
	private GameObject whiteKing;
	private GameObject blackKing;


	public List<GameObject> ClearList(List<GameObject> list){
		/*
		*
		*Outputs a cleared list of game objects
		*Used for the pseudo move, where a promotion destroys a pawn and replaces with queen
		*This function clears the temporary creation of queens
		*
		*/
		foreach(GameObject go in list)
			Destroy(go);
		list = new List<GameObject> ();
		return list;
	}

	public void UndoMove(Chessman[] undoP = null, int[] undoL = null){
		/*
		 * 
		 *Undoes the previous move where the previous gameobjects and their locations are stored in UndoPieces and undoLocations
		 *This function is used to minimizes he time and memory space needed to create a seperate pseudoboard in order to check for checks
		 * 
		 */

		if (undoP == null && undoL == null) {
			undoP = undoPieces;
			undoL = undoLocations;
		}

		Chessmans[undoL[0], undoL[1]] = undoP[0];
		selectedChessman = undoP [0];
		if (undoP[0] != null)
			undoP [0].SetPosition (undoL [0], undoL [1], true);


		Chessmans[undoL[2], undoL[3]] = undoP[1];
		if (undoP[1] != null)
			undoP [1].SetPosition (undoL [2], undoL [3], true);

		//Castleing Undo
		if (!(undoL [4] == -1 || undoL [5] == -1)) {
			
			Chessmans [undoL [4], undoL [5]] = undoP [2];
			if (undoP [2] != null)
				undoP [2].SetPosition (undoL [4], undoL [5], true);
		}

		if (!(undoL [6] == -1 || undoL [7] == -1)) {
			Chessmans [undoL [6], undoL [7]] = undoP [3];
			if (undoP [3] != null)
				undoP [3].SetPosition (undoL [6], undoL [7], true);
		}

		whiteThreat = threatBoard (true, Chessmans);
		blackThreat = threatBoard (false, Chessmans);
	}

	public void pseudoMove(int x, int y, bool? isWhite = null){
		/*
		 * 
		 * Does a pseudomove to a paticular location and then updates the undo pieces and locations as well as the threat boards so that the 'pseudo' board can be evaluated
		 * Does NOT change turns or move the actual pieces 
		 * as such if you do a pseudomove and want to use that move, undo the move and use the actual move function
		 * 
		 */
		if (isWhite == null)
			isWhite = isWhiteTurn;

		if (selectedChessman != null) {
			Chessman c = Chessmans [x, y];
			undoPieces [0] = selectedChessman;
			undoLocations [0] = selectedChessman.CurrentX;
			undoLocations [1] = selectedChessman.CurrentY;



			if (c != null) {
				undoPieces [1] = c;
			} else {
				undoPieces [1] = null;
			}
			undoLocations [2] = x;
			undoLocations [3] = y;



			if (c != null && c.isWhite != isWhite) {
				//Capture
				if (c.GetType() == typeof(King)){
					return;
				}



			}

			undoPieces [2] = null;
			undoLocations [4] = -1;
			undoLocations [5] = -1;
			undoPieces [3] = null;
			undoLocations [6] = -1;
			undoLocations [7] = -1;
			Chessman c1 = null;

			if (selectedChessman.GetType () == typeof(King)) {
				
				if (x == 6 && Chessmans [7, selectedChessman.CurrentY] != null) {
					c = Chessmans [7, selectedChessman.CurrentY];

					Chessmans [5, selectedChessman.CurrentY] = c;
					Chessmans [7, selectedChessman.CurrentY] = c1;


					c.SetPosition (5, selectedChessman.CurrentY, true);





					undoPieces [2] = Chessmans [5, selectedChessman.CurrentY];
					undoLocations [4] = 7;
					undoLocations [5] = selectedChessman.CurrentY;

					undoPieces [3] = Chessmans [7, selectedChessman.CurrentY];
					undoLocations [6] = 5;
					undoLocations [7] = selectedChessman.CurrentY;
				}
				else if (x == 2 && Chessmans [0, selectedChessman.CurrentY] != null) {
					c = Chessmans [0, selectedChessman.CurrentY];
					Chessmans [0, selectedChessman.CurrentY] = null;
					Chessmans [3, selectedChessman.CurrentY] = c;
					c.SetPosition (3, selectedChessman.CurrentY, true);

					//c.transform.position = GetTileCenter (3, selectedChessman.CurrentY);



					undoPieces [2] = Chessmans [3, selectedChessman.CurrentY];
					undoLocations [4] = 0;
					undoLocations [5] = selectedChessman.CurrentY;

					undoPieces [3] = Chessmans [0, selectedChessman.CurrentY];
					undoLocations [6] = 3;
					undoLocations [7] = selectedChessman.CurrentY;
				}
			}

			if (selectedChessman.GetType () == typeof(Pawn) && x == EnPassantMove [0] && y == EnPassantMove [1]) {
				if (isWhiteTurn)
					c = Chessmans [x, y - 1];
				else 
					c = Chessmans [x, y + 1];
				
			}

			if (selectedChessman.GetType () == typeof(Pawn)) {
				if (y == 7) {
					SpawnPseudoChessman (1, x, y);
					selectedChessman = Chessmans [x, y];
				}
				else if (y == 0) {
					SpawnPseudoChessman (7, x, y);
					selectedChessman = Chessmans [x, y];
				}

			}


			Chessmans [selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
			Chessmans [x, y] = selectedChessman;
			Chessmans[x, y].SetPosition(x, y, true);

			whiteThreat = threatBoard (true, Chessmans);
			blackThreat = threatBoard (false, Chessmans);
		}
	}

	public bool isMyKingNotInCheck(bool? isWhite = null){
		/*
		 * 
		 * returns true if the inputted team's king is not in check
		 * 
		 */
		if (isWhite == null)
			isWhite = isWhiteTurn;

		if ((bool)isWhite) {
			return blackThreat [whiteKing.GetComponent<Chessman> ().CurrentX, whiteKing.GetComponent<Chessman> ().CurrentY] == 0;
		}
		else {
			return whiteThreat [blackKing.GetComponent<Chessman> ().CurrentX, blackKing.GetComponent<Chessman> ().CurrentY] == 0;
		}
	}

	public bool[,] updateForChecks(bool[,] r, bool? isWhite = null){
		/*
		 * 
		 * Takes a board of boolean which tells where you can and can't go, then iterates over to see if any of those moves would  leave you in check, then consquently removes them
		 * 
		 */
		if (isWhite == null)
			isWhite = isWhiteTurn;

		bool[,] correctedMoves = new bool[8,8];

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				
				if (r [i, j]) {
					pseudoMove (i, j, isWhite);


					correctedMoves [i, j] = isMyKingNotInCheck (isWhite);

					UndoMove ();
					pseudoActiveChessman = ClearList (pseudoActiveChessman);
				}
			}
		}

		return correctedMoves;
	}

	private void Start(){
		Instance = this;
		pseudoActiveChessman = new List<GameObject> ();
		undoPieces = new Chessman[4];
		undoLocations = new int[8];
		SpawnAllChessmans ();



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
		robot2 = new RobotMan ();
		robot2.isWhite = false;
		robot2.myWeights = new float[8];
		robot2.myWeights [0] = 1f;
		robot2.myWeights [1] = .9f;
		robot2.myWeights [2] = .2f;
		robot2.myWeights [3] = .3f;
		robot2.myWeights [4] = -1f;
		robot2.myWeights [5] = -.9f;
		robot2.myWeights [6] = -.2f;
		robot2.myWeights [7] = -.3f;
		robot2.n = 4;

	}

	private void Update(){
		/*
		 * 
		 * Allows user to click board wow
		 * 
		 */
		UpdateSelection ();
		DrawChessBoard ();

		if (isComputingMove)
			return;

		if (gameState != 2) {
			
			return;
		}

		if (Input.GetMouseButtonDown (0) && ((isWhiteTurn && whiteIsPlayer) || (!isWhiteTurn && blackIsPlayer))) {
			if (selectionX >= 0 && selectionY >= 0) {
				if (selectedChessman == null) {
					//Select a piece
					SelectChessman (selectionX, selectionY);
				} else {
					//Move a piece
					MoveChessman (selectionX, selectionY);
				}
			} 
		}

		if (!isWhiteTurn && !blackIsPlayer) {
			float[] move = GetComputerMove (robot2, 1, isWhiteTurn);
			if (move [0] == -1) {
				Debug.Log ("Game is finished" + gameState);
				pseudoActiveChessman = new List<GameObject> ();
				undoPieces = new Chessman[4];
				undoLocations = new int[8];
				SpawnAllChessmans ();
			}
			SelectChessman ((int)move [0],(int) move [1]);
			MoveChessman ((int)move [2], (int)move [3]);

		}
	}

	public void SelectChessman(int x, int y, bool? isWhite = null){
		/*
		 * 
		 * Takes a 'clicked' tile and selects the piece on the tile if avaliable
		 * Finds all the possible moves for the piece and pushes it the avaliable moves
		 * hihglight avaliable moves on the board
		 * Use this for AI probably to not mess up everything else
		 * 
		 */
		if (isWhite == null)
			isWhite = isWhiteTurn;

		if (Chessmans [x, y] == null)
			return;
		if (Chessmans [x, y].isWhite != isWhite)
			return;

		bool hasAtleastOneMove = false;
		allowedMoves = Chessmans [x, y].PossibleMove ();
		selectedChessman = Chessmans [x, y];
		allowedMoves = updateForChecks (allowedMoves, isWhite);

		for (int i = 0; i < 8; i++)
			for (int j = 0; j < 8; j++)
				if (allowedMoves [i, j])
					hasAtleastOneMove = true;

		if (!hasAtleastOneMove) {
			selectedChessman = null;
			return;
		}

		BoardHighlights.Instance.HighlightAllowedMoves (allowedMoves);
	}

	private void MoveChessman(int x, int y){
		/*
		 * 
		 * Move the selected chessman to a location, updates undo, updates board threats, and DESTROYS caputed pieces, only do if committed to a move cause no undo for you
		 * Also chnages to opponents turn and hide highlights on the board
		 * 
		 */
		if (allowedMoves[x, y]) {

			Chessman c = Chessmans [x, y];


			undoPieces [0] = selectedChessman;
			undoLocations [0] = selectedChessman.CurrentX;
			undoLocations [1] = selectedChessman.CurrentY;

			if (c != null) {
				undoPieces [1] = c;
			}
			undoLocations [2] = x;
			undoLocations [3] = y;

			if (c != null && c.isWhite != isWhiteTurn) {
				//Capture
				if (c.GetType() == typeof(King)){
					return;
				}

				activeChessman.Remove(c.gameObject);
				Destroy (c.gameObject);

			}

			undoPieces [2] = null;
			undoLocations [4] = -1;
			undoLocations [5] = -1;
			undoPieces [3] = null;
			undoLocations [6] = -1;
			undoLocations [7] = -1;
			Chessman c1 = null;

			if (selectedChessman.GetType () == typeof(King)) {
				if (x == 6 && Chessmans [7, selectedChessman.CurrentY] != null) {
					c = Chessmans [7, selectedChessman.CurrentY];
					Chessmans [5, selectedChessman.CurrentY] = c;
					Chessmans [7, selectedChessman.CurrentY] = c1;

					c.SetPosition (5, selectedChessman.CurrentY, false);

					c.transform.position = GetTileCenter (5, selectedChessman.CurrentY);



					undoPieces [2] = Chessmans [5, selectedChessman.CurrentY];
					undoLocations [4] = 7;
					undoLocations [5] = selectedChessman.CurrentY;

					undoPieces [3] = Chessmans [7, selectedChessman.CurrentY];
					undoLocations [6] = 5;
					undoLocations [7] = selectedChessman.CurrentY;
				}
				else if (x == 2 && Chessmans [0, selectedChessman.CurrentY] != null) {
					c = Chessmans [0, selectedChessman.CurrentY];
					Chessmans [0, selectedChessman.CurrentY] = null;
					Chessmans [3, selectedChessman.CurrentY] = c;
					c.SetPosition (3, selectedChessman.CurrentY, false);

					c.transform.position = GetTileCenter (3, selectedChessman.CurrentY);



					undoPieces [2] = Chessmans [3, selectedChessman.CurrentY];
					undoLocations [4] = 0;
					undoLocations [5] = selectedChessman.CurrentY;

					undoPieces [3] = Chessmans [0, selectedChessman.CurrentY];
					undoLocations [6] = 3;
					undoLocations [7] = selectedChessman.CurrentY;
				}
			}

			if (selectedChessman.GetType () == typeof(Pawn) && x == EnPassantMove [0] && y == EnPassantMove [1]) {
				if (isWhiteTurn)
					c = Chessmans [x, y - 1];
				else 
					c = Chessmans [x, y + 1];
				activeChessman.Remove(c.gameObject);
				Destroy (c.gameObject);
			}

			EnPassantMove [0] = -1;
			EnPassantMove [1] = -1;
			if (selectedChessman.GetType () == typeof(Pawn)) {
				if (y == 7) {
					activeChessman.Remove(selectedChessman.gameObject);
					Destroy (selectedChessman.gameObject);
					SpawnChessman (1, x, y);
					selectedChessman = Chessmans [x, y];
				}
				else if (y == 0) {
					activeChessman.Remove(selectedChessman.gameObject);
					Destroy (selectedChessman.gameObject);
					SpawnChessman (7, x, y);
					selectedChessman = Chessmans [x, y];
				}

				if (selectedChessman.CurrentY == 1 && y == 3) {
					EnPassantMove [0] = x;
					EnPassantMove [1] = 2;
				}
				else if (selectedChessman.CurrentY == 6 && y == 4) {
					EnPassantMove [0] = x;
					EnPassantMove [1] = 5;
				}
			}


			Chessmans [selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
			selectedChessman.transform.position = GetTileCenter (x, y);
			selectedChessman.SetPosition (x, y, false);
			Chessmans [x, y] = selectedChessman;

			isWhiteTurn = !isWhiteTurn;
			whiteThreat = threatBoard (true, Chessmans);
			blackThreat = threatBoard (false, Chessmans);
			if (!isWhiteTurn) {
				turnCount++;
			}
		}

		gameState = winState ();
		BoardHighlights.Instance.HideHighlights ();
		selectedChessman = null;
		Debug.Log(printBoard(Chessmans));
	}

	private void SpawnChessman(int index, int x, int y){
		/*
		 * 
		 * Creates a chess man at (x,y) based on the predetermenied prefabs list, make sure order is correct
		 * 
		 */
		Quaternion thisOrientation = orientation;
		if (index > 6) {
			thisOrientation = Quaternion.Euler(0, -90, 0);
		}
		GameObject go = Instantiate (chessmanPrefabs [index], GetTileCenter(x, y), thisOrientation) as GameObject;
		go.transform.SetParent (transform);

		if (index == 0) {
			whiteKing = go;
		} else if (index == 6) {
			blackKing = go;
		}

		Chessmans [x, y] = go.GetComponent<Chessman> ();
		Chessmans [x, y].SetPosition (x, y, false);
		activeChessman.Add (go);
	}

	private void SpawnPseudoChessman(int index, int x, int y){
		/*
		 * 
		 * Creates a pseudo chessman specifically for the queen promotion becasue Chessman is a monobehaviour script and must be attached to a gameobject
		 * 
		 */
		Quaternion thisOrientation = orientation;
		if (index > 6) {
			thisOrientation = Quaternion.Euler(0, -90, 0);
		}
		GameObject go = Instantiate (chessmanPrefabs [index], GetTileCenter(x, y), thisOrientation) as GameObject;
		go.transform.SetParent (transform);
		go.transform.position = new Vector3 (100, 100, 100);

		if (index == 0) {
			whiteKing = go;
		} else if (index == 6) {
			blackKing = go;
		}

		Chessmans [x, y] = go.GetComponent<Chessman> ();
		Chessmans [x, y].SetPosition (x, y, false);
		pseudoActiveChessman.Add (go);
	}
		
	private void SpawnAllChessmans(){
		/*
		 * 
		 * Creates the board and can be used to restart board
		 * 
		 */
		if (activeChessman != null) {
			activeChessman = ClearList (activeChessman);
		}
		activeChessman = new List<GameObject> ();
		Chessmans = new Chessman[8, 8];
		EnPassantMove = new int[2]{-1, -1};
		whiteThreat = new int[8, 8];
		blackThreat = new int[8, 8];
		turnCount = 1;
		gameState = 2;


		//White King
		SpawnChessman (0, 4, 0);

		//White Queen
		SpawnChessman (1, 3, 0);

		//White Rooks
		SpawnChessman (2, 0, 0);
		SpawnChessman (2, 7, 0);

		//White Bishops
		SpawnChessman (3, 2, 0);
		SpawnChessman (3, 5, 0);

		//White Knights
		SpawnChessman (4, 1, 0);
		SpawnChessman (4, 6, 0);

		//White Pawns
		for (int i = 0; i < 8; i++) {
			SpawnChessman (5, i, 1);
		}

		//Black King
		SpawnChessman (6, 4, 7);

		//Black Queen
		SpawnChessman (7, 3, 7);

		//Black Rooks
		SpawnChessman (8, 0, 7);
		SpawnChessman (8, 7, 7);

		//Black Bishops
		SpawnChessman (9, 2, 7);
		SpawnChessman (9, 5, 7);

		//Black Knights
		SpawnChessman (10, 1, 7);
		SpawnChessman (10, 6, 7);

		//Black Pawns
		for (int i = 0; i < 8; i++) {
			SpawnChessman (11, i, 6);
		}

		whiteThreat = threatBoard (true, Chessmans);
		blackThreat = threatBoard (false, Chessmans);

	}
  
	public int getPoints(bool isWhite, Chessman[,] Board){
		/*
		 * 
		 * Get the total points on the board for the inputted team
		 * 
		 */
		int p = 0;
		foreach (Chessman c in Board) {
			if (c != null && c.isWhite == isWhite)
				p += c.point;
		}
		return p;
	}

	public int[,] threatBoard(bool isWhite, Chessman[,] Board){
		/*
		 * 
		 * Gives a int[,] of the threats made the inputted team
		 * based on how many pieces have access to a tile from the team
		 * 
		 */

		int[,] threats = new int[8, 8];
		bool[,] r;
		bool[,] protects;



		foreach (Chessman c in Board) {
			if (c != null && isWhite == c.isWhite) {
				allowedMoves = c.PossibleMove ();
				r = allowedMoves;
				protects = c.ProtectedPieces ();
				for (int i = 0; i < 8; i++) {
					for (int j = 0; j < 8; j++) {
						if (r [i, j] || protects[i,j]) {
							//or is okay since a protected pieces will never have be a threat
							threats [i, j]++; 
						}
					}
				}
			}
		}
			

		return threats;
	}

	private void UpdateSelection(){
		if (!Camera.main)
			return;
		RaycastHit hit;
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 25.0f, LayerMask.GetMask ("ChessPlane"))) {
			selectionX = (int)hit.point.x;
			selectionY = (int)hit.point.z;
		} else {
			selectionX = -1;
			selectionY = -1;
		}
	}

	private Vector3 GetTileCenter (int x, int y){
		Vector3 origin = Vector3.zero;
		origin.x += (TILE_SIZE * x) + TILE_OFFSET;
		origin.z += (TILE_SIZE * y) + TILE_OFFSET;
		return origin;
	}

	private void DrawChessBoard(){
		Vector3 widthLine = Vector3.right * 8;
		Vector3 heightLine = Vector3.forward * 8;

		for (int i = 0; i <= 8; i++){
			Vector3 start = Vector3.forward * i;
			Debug.DrawLine (start, start + widthLine);
			for (int j = 0; j <= 8; j++) {
				start = Vector3.right * j;
				Debug.DrawLine (start, start + heightLine);
			}
		}

		//draw selection
		if (selectionX >= 0 && selectionY >= 0){
			Debug.DrawLine (Vector3.forward * selectionY + Vector3.right * selectionX, Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
			Debug.DrawLine (Vector3.forward * (selectionY + 1) + Vector3.right * selectionX, Vector3.forward * (selectionY) + Vector3.right * (selectionX + 1));
		}
	}



	//____________________________________________________________________________________________________________________________________________________________
	/*
	 * Me Messing with my own AI 
	 */

	public float evaluateBoardState(bool isWhite, Chessman[,] Board, float[] w){
		/*
		 * 
		 * Evaluate the board with weights
		 * Evaluates the black threat, black protection, black movement, black points, white threat, white protection, white movement, white points
		 * 
		 */
		int[,] myT = threatBoard (isWhite, Board);
		int[,] theirT = threatBoard (!isWhite, Board);

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
		/*
		Debug.Log (w[0] * myThreat(isWhite, myT) + " " + w[1] * myProtection(isWhite, myT) + " " + w[2] * myMovement(isWhite, myT) + " " + w[3] * getPoints(isWhite, Board) + " " +
			w[4] * myThreat(!isWhite, theirT) + " " + w[5] * myProtection(!isWhite, theirT) + " " + w[6] * myMovement(!isWhite, theirT) + " " + w[7] * getPoints(!isWhite, Board) + "\n");
		*/
		return 	w[0] * myThreat(isWhite, myT) + w[1] * myProtection(isWhite, myT) + w[2] * myMovement(isWhite, myT) + w[3] * getPoints(isWhite, Board) +
				w[4] * myThreat(!isWhite, theirT) + w[5] * myProtection(!isWhite, theirT) + w[6] * myMovement(!isWhite, theirT) + w[7] * getPoints(!isWhite, Board);
	}

	public float[] GetComputerMove(RobotMan r, int depth, bool whiteTurn){
		isComputingMove = true;
		float[] move = new float[5]{ -1, -1, -1, -1, float.MinValue };
		float tempPoint = float.MinValue;
		bool[,] moves;
		Chessman[] localUndoPieces = new Chessman[4];
		int[] localUndoLocations = new int[8];

		for (int a = 0; a < 8; a++) {
			for (int b = 0; b < 8; b++) {
				if (Chessmans [a, b] != null && Chessmans [a, b].isWhite == whiteTurn) {
					SelectChessman (a, b, whiteTurn);
					//print ("Selected Piece: " + a + " " + b);
					if (selectedChessman == null){
						Debug.Log ("Error at: " + a + " " + b + "\n" + printBoard(Chessmans));
						break;
					}
					moves = Chessmans [a, b].PossibleMove ();
					moves = updateForChecks (moves, whiteTurn);

					for (int i = 0; i < 8; i++) {
						for (int j = 0; j < 8; j++) {
							tempPoint = float.MinValue;
							if (moves [i, j]) {
								pseudoMove (i, j, whiteTurn);

								for (int x = 0; x < 4; x++) {
									localUndoPieces [x] = undoPieces [x];
									localUndoLocations [2 * x] = undoLocations [2 * x];
									localUndoLocations [2 * x + 1] = undoLocations [2 * x + 1];
								}
								if (isMyKingNotInCheck(whiteTurn)){
									tempPoint = evaluateBoardState (r.isWhite, Chessmans, r.myWeights);
									if (r.n > depth && move[4] != float.MinValue && tempPoint > move[4] * .9) {
										move = GetComputerMove (r, depth + 1, !whiteTurn);
										move [0] = a;
										move [1] = b;
										move [2] = i;
										move [3] = j;
									} else {
										if (move [4] < tempPoint) {
											move [0] = a;
											move [1] = b;
											move [2] = i;
											move [3] = j;
											move [4] = tempPoint;
										}
									}
								}

								Debug.Log("Move: " + a + " " + b + " " + i + " " + j + "\nDEPTH: " + depth + "\n" + printBoard(Chessmans));

								UndoMove(localUndoPieces, localUndoLocations);
								pseudoActiveChessman = ClearList (pseudoActiveChessman);

								for (int x = 0; x < 4; x++) {
									localUndoPieces [x] = null;
									localUndoLocations [2 * x] = -1;
									localUndoLocations [2 * x + 1] = -1;
								}

							}
						}
					}
				}
				selectedChessman = null;
			}
		}

		/*
		for (int a = 0; a < 8; a++) {
			for (int b = 0; b < 8; b++) {
				if (Chessmans [a, b] != null && Chessmans [a, b].isWhite == r.isWhite) {
					SelectChessman (a, b);
					moves = Chessmans [a, b].PossibleMove ();
					moves = updateForChecks (moves, r.isWhite);

					for (int i = 0; i < 8; i++) {
						for (int j = 0; j < 8; j++) {
							tempPoint = float.MinValue;
							if (moves [i, j]) {
								pseudoMove (i, j);
								if (isMyKingNotInCheck (r.isWhite)) {
									tempPoint = evaluateBoardState (r.isWhite, Chessmans, r.myWeights);
									if (move [4] < tempPoint) {
										move [0] = a;
										move [1] = b;
										move [2] = i;
										move [3] = j;
										move [4] = tempPoint;
									}
								}

								Debug.Log ("Move: " + a + " " + b + " " + i + " " + j + "\nScore: " + tempPoint);

								UndoMove ();
								pseudoActiveChessman = ClearList (pseudoActiveChessman);

							}
						}

					}



					selectedChessman = null;


				}	
			}
		}
		*/

		isComputingMove = false;
		//Debug.Log ("Finished computation: " + move [0] + " " + move [1] + " " + move [2] + " " + move [3]);
		return move;
	}

	public bool hasMoves(bool isWhite){
		bool[,] legalMoves;

		foreach (Chessman c in Chessmans) {
			if (c != null && c.isWhite == isWhite) {
				SelectChessman (c.CurrentX, c.CurrentY);
				legalMoves = updateForChecks (c.PossibleMove (), c.isWhite);
				for (int i = 0; i < 8; i++) {
					for (int j = 0; j < 8; j++) {
						if (legalMoves [i, j]) {
							
							return true;
						}
					}
				}
			}
		}
		selectedChessman = null;

		return false;
	}

	public int winState(){
		/*
		 * Returns 2 when the game is not over
		 */

		bool whiteHasOneMove = hasMoves(true);
		bool blackHasOneMove = hasMoves(false);


		//Black Win 
		if (!whiteHasOneMove && !isMyKingNotInCheck(true))
			return -1;


		//White Win
		if (!blackHasOneMove && !isMyKingNotInCheck(false))
			return 1;
			


		//Draw
		if ( !(whiteHasOneMove && blackHasOneMove) || turnCount > 8000){
			
			return 0;
		}

		return 2;
	}

	public int myMovement(bool isWhite, int[,] board){
		int count = 0;
		int[,] b = board;

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (b [i, j] != 0 && Chessmans [i, j] == null)
					count++;
			}
		}

		return count;
	}

	public int myProtection(bool isWhite, int[,] board){
		int count = 0;
		int[,] b = board;

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (b [i, j] != 0 && Chessmans [i, j] != null && Chessmans [i, j].isWhite == isWhite)
					count += b [i, j] * Chessmans [i, j].point;
			}
		}

		return count;
	}

	public int myThreat(bool isWhite, int[,] board){
		int count = 0;
		int[,] b = board;

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (b [i, j] != 0 && Chessmans [i, j] != null && Chessmans[i, j].isWhite != isWhite)
					count += b [i, j] * Chessmans [i, j].point;
			}
		}

		return count;
	}

	public string printBoard(Chessman[,] board){
		string r = "";
		for (int j = 7; j >= 0; j--) {
			for (int i = 7; i >= 0; i--) {
				if (board [i, j] == null) {
					r += "  ";
				} else {
					if (board [i, j].isWhite) {
						r += "W";
					} else {
						r += "B";
					}

					if (board [i, j].GetType () == typeof(Pawn))
						r += "P";
					if (board [i, j].GetType () == typeof(King))
						r += "K";
					if (board [i, j].GetType () == typeof(Knight))
						r += "N";
					if (board [i, j].GetType () == typeof(Queen))
						r += "Q";
					if (board [i, j].GetType () == typeof(Rook))
						r += "R";
					if (board [i, j].GetType () == typeof(Bishop))
						r += "B";
				}
				r+= "\t";
			}
			r += "\n\n";
		}
		return r;
	}
}



