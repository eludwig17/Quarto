using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }
    
    private enum Shape
    {
        Cylinder,
        Prism
    }

    private enum Hollowness
    {
        Hollow,
        Solid
    }

    private enum Height
    {
        Tall,
        Short
    }

    private enum Color
    {
        White,
        Black
    }

    public List<GameObject> unplacedPieces = new List<GameObject>();
    public GameObject[,] boardPieces = new GameObject[4, 4];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameObject[] piecesInScene = GameObject.FindGameObjectsWithTag("GamePiece");
        unplacedPieces.AddRange(piecesInScene);
    }

    public bool TryPlacePiece(GameObject piece, GameObject square)
    {
        if (piece == null || square == null) return false;

        // gets the board position
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

        bool didWin = false;
        
        if (CheckRowForWin(row)) didWin = true;
        if (CheckColumnForWin(col)) didWin = true;
        
        if (row == col && CheckDiagonalForWin(0)) didWin = true;
        if (row + col == 3 && CheckDiagonalForWin(1)) didWin = true;

        if (didWin)
        {
            Debug.Log("QUARTO!");
        }

        return true;
    }

    public bool CheckRowForWin(int row)
    {
        if (row < 0 || row > 3) return false;
        
        return CheckLineForWin(
            boardPieces[row, 0],
            boardPieces[row, 1],
            boardPieces[row, 2],
            boardPieces[row, 3]
        );
    }

    public bool CheckColumnForWin(int column)
    {
        if (column < 0 || column > 3) return false;
        
        return CheckLineForWin(
            boardPieces[0, column],
            boardPieces[1, column],
            boardPieces[2, column],
            boardPieces[3, column]
            );
    }

    public bool CheckDiagonalForWin(int diagonal)
    {
        if (diagonal == 0)
        {
            return CheckLineForWin(
                boardPieces[0, 0],
                boardPieces[1, 1],
                boardPieces[2, 2],
                boardPieces[3, 3]
            );
        }
        else if (diagonal == 1)
        {
            return CheckLineForWin(
                boardPieces[0, 3],
                boardPieces[1, 2],
                boardPieces[2, 1],
                boardPieces[3, 0]
            );
        }

        return false;

    }

    private bool CheckLineForWin(GameObject a, GameObject b, GameObject c, GameObject d)
    {
        if (a == null || b == null || c == null || d == null) return false;
        
        GetFullShapeInfo(a.name, out Height firstHeight, out Hollowness firstHollowness, out Color firstColor, 
            out Shape firstShape);

        bool sameColor = true;
        bool sameHeight = true;
        bool sameShape = true;
        bool sameHollowness = true;
        
        GameObject[] line = {b, c, d};

        foreach (GameObject piece in line)
        {
            GetFullShapeInfo(piece.name, out Height pieceHeight, out Hollowness pieceHollow, out Color pieceColor,
                out Shape pieceShape);
            if (pieceColor != firstColor) sameColor = false;
            if (pieceHeight != firstHeight) sameHeight = false;
            if (pieceShape != firstShape) sameShape = false;
            if (pieceHollow != firstHollowness) sameHollowness = false;
        }
        return sameColor || sameHeight || sameShape || sameHollowness;
    }

    // some code in this function is from ChatGPT, idea from Geist
    string GetShape(string name)
    {

        int start = -1;

        for (int i = name.Length - 1; i >= 0; i--)
        {
            if (char.IsUpper(name[i]))
            {
                start = i;
                break;
            }
        }

        string result = start >= 0 ? name.Substring(start) : string.Empty;

        return result;

    }

    // naming scheme: TallHollowWhiteCylinder
    //                HeightHollownessColorShape

    // Outputs the enum type of each category for the given shape
    void GetFullShapeInfo(string name, out Height height, out Hollowness hollow, out Color material, out Shape shape)
    {
        
        height = name.Contains("Tall") ? Height.Tall : Height.Short;
        hollow = name.Contains("Hollow") ? Hollowness.Hollow : Hollowness.Solid;
        material = name.Contains("White") ? Color.White : Color.Black;
        shape = name.Contains("Cylinder") ? Shape.Cylinder : Shape.Prism;

    }

}