using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SwapUI : MonoBehaviour
{

    // public TextMeshPro
    [Header("Main Menu")]
    public GameObject MainMenu;
    [Header("HUD")]
    public GameObject HUD;
    public TextMeshProUGUI StatusText;
    
    [Header("Pause Menu")]
    public GameObject pauseMenu; 
    
    [Header("Game Over")]
    public GameObject gameOverPanel;
    
    [Header("Board Controller")]
    public GameObject BoardController;
    private BoardControllerScript boardControllerScript;
    
    [Header("Player name Inputs")]
    public TMP_InputField P1_Name_Input;
    public TMP_InputField P2_Name_Input;
    public TextMeshProUGUI P1_Name_Display;
    public TextMeshProUGUI P2_Name_Display;
    
    private bool isSubscribed = false;
    

   // public string PlayerTurn;

   void Awake()
   {
       SetUIState_MainMenu();

       if (BoardController == null)
       {
           boardControllerScript = BoardController.GetComponent<BoardControllerScript>();
           if (boardControllerScript == null)
           {
               Debug.LogError($"Board Controller not found on {BoardController.name}");
           }
       }
   }
   
    void OnEnable()
    {
        if (BoardController == null)
        {
            Debug.Log("SwapUI: Board Controller is not assigned yet");
            return;
        }

        boardControllerScript = BoardController.GetComponent<BoardControllerScript>();
        if (boardControllerScript == null)
        {
            Debug.LogError("SwapUI: BoardControllerScript missing on BoardController.");
            return;
        }
        
        boardControllerScript.OnTurnPlayed.AddListener(UpdateTurnText);
        
        SubscribeToBoardEvents();
    }

    void OnDisable()
    {
        if (boardControllerScript != null)
        {
            boardControllerScript.OnTurnPlayed.RemoveListener(UpdateTurnText);
        }
        UnsubscribeFromBoardEvents();
    }

    private void SubscribeToBoardEvents()
    {
        if (isSubscribed) return;
        if (boardControllerScript == null) return;
        
        boardControllerScript.OnTurnPlayed.AddListener(UpdateTurnText);
        isSubscribed = true;
    }

    private void UnsubscribeFromBoardEvents()
    {
        if (!isSubscribed) return;
        if (boardControllerScript == null) return;
        
        boardControllerScript.OnTurnPlayed.RemoveListener(UpdateTurnText);
        isSubscribed = false;
    }

    public void StartGame()
    {
       Time.timeScale = 1f;

       SetUIState_HUD();
       
       string p1Name = (P1_Name_Input != null && !string.IsNullOrWhiteSpace(P1_Name_Input.text))
           ? P1_Name_Input.text
           : "Player 1";

       string p2Name = (P2_Name_Input != null && !string.IsNullOrWhiteSpace(P2_Name_Input.text))
           ? P2_Name_Input.text
           : "Player 2";

       if (P1_Name_Display != null) P1_Name_Display.text = $"Player 1: {p1Name}";
       if (P2_Name_Display != null) P2_Name_Display.text = $"Player 2: {p2Name}";
       
       if (StatusText != null) StatusText.text = "Game Status: Starting...";
    }

    void UpdateTurnText(string currentPlayerName)
    {
        if (StatusText == null) return;
        
        StatusText.text = $"Game Status: {currentPlayerName}'s Turn";
        //Debug.Log($"Game Status: {currentPlayerName}'s Turn");
    }
    
    public void ShowMainMenu(){
        Time.timeScale = 1f;
        SetUIState_MainMenu();
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;
        
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    private void SetUIState_MainMenu()
    {
        if (MainMenu != null) MainMenu.SetActive(true);
        if (HUD != null) HUD.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void SetUIState_HUD()
    {
        if (HUD != null) HUD.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (MainMenu != null) MainMenu.SetActive(false);
    }

    private void SetUIState_Pause()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);
        if (HUD  != null) HUD.SetActive(true);
        if (MainMenu != null) MainMenu.SetActive(false);
    }
    
    public void PauseGame(){
        Time.timeScale = 0f;
        SetUIState_Pause();
    }
    
    public void ResumeGame(){
        Time.timeScale = 1f;
        SetUIState_HUD();
    }
    
    public void RestartGame(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SetUIState_HUD();
        if (StatusText != null) StatusText.text = "Game Status: Restart...";
    }

    public void QuitGame(){
        Application.Quit();
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}
