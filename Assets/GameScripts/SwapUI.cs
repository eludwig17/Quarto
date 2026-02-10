using UnityEngine;
using TMPro;

public class SwapUI : MonoBehaviour
{

    // public TextMeshPro

    public GameObject MainMenu;
    public GameObject HUD;
    public GameObject BoardController;
    private BoardControllerScript boardControllerScript;
    public TMP_InputField P1_Name_Input;
    public TMP_InputField P2_Name_Input;
    public TextMeshProUGUI P1_Name_Display;
    public TextMeshProUGUI P2_Name_Display;
    public TextMeshProUGUI StatusText;
   // public string PlayerTurn;

    void OnEnable()
    {
        // Menu enabling/disabling
        MainMenu.SetActive(true);
        HUD.SetActive(false);

        // Gives the UIManager access to gamestate info
        boardControllerScript = BoardController.GetComponent<BoardControllerScript>();
        Debug.Log(boardControllerScript.gameObject.name);
        
        //Debug.Log(boardControllerScript == null); // this outputs false; why is the event not working?
        
        boardControllerScript.OnTurnPlayed.AddListener(UpdateTurnText);
    }


    void Update()
    {

    }

    public void StartGame()
    {
        MainMenu.SetActive(false);
        HUD.SetActive(true);
        P1_Name_Display.text = $"Player 1: {P1_Name_Input.text}";
        P2_Name_Display.text = $"Player 2: {P2_Name_Input.text}";

    }

    void UpdateTurnText(string text)
    {
        StatusText.text = $"Game Status: {text}'s Turn";
        Debug.Log("Lol!");
    }

}
