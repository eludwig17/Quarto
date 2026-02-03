using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {
    public static GameState Instance { get; private set; }
    
    public List<GameObject> unplacedPieces = new List<GameObject>();
    public GameObject[,] boardPieces = new GameObject[4, 4];

    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start(){
        GameObject[] piecesInScene = GameObject.FindGameObjectsWithTag("GamePiece");
        unplacedPieces.AddRange(piecesInScene);
    }

    public bool TryPlacePiece(GameObject piece, GameObject square){
        if (piece == null || square == null) return false;

        // gets the board pos
        BoardSquare squareComponent = square.GetComponent<BoardSquare>();
        if (squareComponent == null) return false;

        int row = squareComponent.row;
        int col = squareComponent.col;

        if (row < 0 || row > 3 || col < 0 || col > 3) return false; // here checks if board positon/valid exists for both row/col
        if (boardPieces[row, col] != null) return false; // checks board spots
        if (!unplacedPieces.Contains(piece)) return false; // checks if available to place

        // places piece & updates state
        boardPieces[row, col] = piece;
        unplacedPieces.Remove(piece);
        return true;
    }
 
    public bool IsWinningRow(int rowIndex){
        if (rowIndex < 0 || rowIndex > 3) return false;

        GameObject a = boardPieces[rowIndex, 0];
        GameObject b = boardPieces[rowIndex, 1];
        GameObject c = boardPieces[rowIndex, 2];
        GameObject d = boardPieces[rowIndex, 3];

        bool isWin = IsWinningLine(a, b, c, d);
        if (isWin){
            Debug.Log($"GOT A WINNING SEQUENCE ON ROW {rowIndex}");
        }
        return isWin;
    }

    private static bool IsWinningLine(GameObject a, GameObject b, GameObject c, GameObject d){
        if (a == null || b == null || c == null || d == null) return false;

        GamePieceTraits ta = a.GetComponent<GamePieceTraits>();
        GamePieceTraits tb = b.GetComponent<GamePieceTraits>();
        GamePieceTraits tc = c.GetComponent<GamePieceTraits>();
        GamePieceTraits td = d.GetComponent<GamePieceTraits>();

        if (ta == null || tb == null || tc == null || td == null) return false;

        // currently only checked for win on all the cylinders in a row
        return !ta.IsPrism && !tb.IsPrism && !tc.IsPrism && !td.IsPrism;
    }
}