using UnityEngine;
using TMPro;

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
        if (_gameStarted){
            UpdateHUD();
            UpdateGameOverScreen();
            PauseInput();
        }
    }

    public void StartGame()
    {
        _gameStarted = true;

        MainMenu.SetActive(false);
        HUD.SetActive(true);
        P1_Name_Display.text = $"Player 1: {P1_Name_Input.text}";
        P2_Name_Display.text = $"Player 2: {P2_Name_Input.text}";
        _gameManager.InitGame();

    }

    void UpdateTurnText(string text)
    {
        StatusText.text = $"Game Status: {text}'s Turn";
        Debug.Log("Lol!");
    }
    
    void ShowMainMenu(){
        _gameStarted = false;
        _isPaused = false;
        Time.timeScale = 1f;
        
        mainMenuPanel.SetActive(true);
        hudPage.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverPanel.SetActive(false);
        quitPage.SetActive(false);

        //Update this line with board controller script more than likely
        //DodgeWaveGameManager.IsGameOver = true;
    }
    
    public void PlayGame(){
        
    }
    
    public void MainMenu(){
        ShowMainMenu();
    }
    
    public void PauseGame(){
        _isPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }
    
    public void ResumeGame(){
        _isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }
    
    void PauseInput(){
        // update this line for our projects game manager like in showmainmenu function
        //if (DodgeWaveGameManager.IsGameOver)
            return;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)){
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
        
    } 
    
    public void RestartGame(){
        Time.timeScale = 1f;
        _isPaused = false;
        _gameStarted = true;
        gameOverPanel.SetActive(false);
        pauseMenu.SetActive(false);
        
        _gameManager.InitGame();
    }
    
    public void ShowQuitPage(){
        quitPage.SetActive(true);
    }
    
    public void HideQuitPage(){
        quitPage.SetActive(false);
    }

    public void QuitGame(){
        Application.Quit();
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}
