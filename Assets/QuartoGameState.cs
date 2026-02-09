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

        print(CheckRowForWin(row));

        return true;
    }

    public bool CheckRowForWin(int row)
    {
        bool same_color = true;
        bool same_height = true;
        bool same_shape = true;
        bool same_hollowness = true;


        // Material first_material = null;
        // float first_height = 0.0f;
        // string first_shape = "";
        // string first_hollow = "";

        Height first_height;
        Hollowness first_hollowness;
        Color first_color;
        Shape first_shape;


        // naming scheme: TallHollowWhiteCylinder
        //                HeightHollownessColorShape

        if (boardPieces[row, 0] == null) return false;

        GetFullShapeInfo(boardPieces[row, 0].name, out first_height, out first_hollowness, out first_color, out first_shape);

        for (int i = 0; i < 4; i++)
        {

            if (boardPieces[row, i] == null) return false;

            string piece_name = boardPieces[row, i].name;

            if (piece_name != null)
            {
                if (i > 0)
                {

                    // Gets all the relevant info of a piece
                    GetFullShapeInfo(piece_name, out Height piece_height, out Hollowness piece_hollowness, out Color piece_color, out Shape piece_shape);

                    if (piece_color != first_color)
                    {
                        same_color = false;
                    }

                    if (piece_height != first_height)
                    {
                        same_height = false;
                    }

                    if (piece_shape != first_shape)
                    {
                        same_shape = false;
                    }

                    if (piece_hollowness != first_hollowness)
                    {
                        same_hollowness = false;
                    }
                }

            }
            else // row is not full
            {
                return false;
            }

        }


        return same_color | same_height | same_hollowness | same_shape;
    }

    public bool CheckColumnForWin(int column)
    {
        return false;

    }

    public bool CheckDiagonalForWin(int diagonal)
    {
        return false;

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