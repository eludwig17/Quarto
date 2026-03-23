using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

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
        bool same_material = true;
        bool same_height = true;
        bool same_shape = true;
        bool same_emptiness = false;


        Material first_material = null;
        float first_height = 0.0f;
        string first_shape = "";
        string first_hollow = "";

        for (int i = 0; i < 4; i++)
        {
            GameObject piece = boardPieces[row, i];

            if (boardPieces[row, i] != null)
            {
            MeshRenderer mesh_renderer = piece.GetComponent<MeshRenderer>();
                if (i > 0)
                {
                    if (mesh_renderer.material != first_material)
                    {
                        same_material = false;
                    }

                    if (mesh_renderer.bounds.size.y >= 2 * first_height || mesh_renderer.bounds.size.y <= first_height / 2) // give it some buffer room
                    {
                        same_height = false;
                    }

                    if (GetShapeType(piece.name) != first_shape)
                    {
                        same_shape = false;
                    }
                }
                else
                {
                    first_material = mesh_renderer.material;
                    first_height = mesh_renderer.bounds.size.y;
                    first_shape = GetShapeType(piece.name);
                }
            } else // row is not full
            {
                return false; 
            }

        }


        return same_material | same_height | same_emptiness | same_shape;
    }

    public bool CheckColumnForWin(int column)
    {
        return false;

    }

    public bool CheckDiagonalForWin(int diagonal)
    {
        return false;

    }

    // most of code in this function is from ChatGPT, idea from Geist
    string GetShapeType(string name)
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
}