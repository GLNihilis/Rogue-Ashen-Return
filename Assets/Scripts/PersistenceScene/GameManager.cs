using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    AudioSource audioSource;
    public string transitionedFromScene; // Store the previous scene
    [SerializeField] private int targetFPS = 60;

    [SerializeField] SavePoint savePoint;
    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;

    [SerializeField] private FadeMenuUI victoryScreen;
    [SerializeField] private GameObject victory;
    [SerializeField] private FadeMenuUI pauseMenu;
    [SerializeField] private GameObject pause;
    [SerializeField] private float fadeTime;
    public bool gameIsPaused = false;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        SaveData.Instance.Initialize();

        audioSource = GetComponent<AudioSource>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        SaveScene();
        DontDestroyOnLoad(gameObject);
        savePoint = FindObjectOfType<SavePoint>();
    }

    private void Start()
    {
        StartCoroutine(PlayAudio());

        victory.SetActive(true);
        pause.SetActive(true);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            SaveData.Instance.Save_PlayerData();
            Debug.Log("File Saved #data/save.savepoint.data");
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            StartCoroutine(ActiveVictoryScreen());
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            StartCoroutine(CanvasController.Instance.ActiveDeathScreen());
        }

        //if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        //{
        //    pauseMenu.FadeInUI(fadeTime);
        //    Time.timeScale = 0;
        //    gameIsPaused = true;
        //}
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }

    public void SaveGame()
    {
        SaveData.Instance.Save_PlayerData();
    }

    public void RespawnPlayer()
    {
        SaveData.Instance.Load_SavePoint();

        if (SaveData.Instance.savePointSceneNames != null) // Load the save point scene name if it exist
        {
            SceneManager.LoadScene(SaveData.Instance.savePointSceneNames);
            Debug.Log("Save Point Scene Names Data Found // #data/save.savepoint.data");
        }
        else
        {
            Debug.Log("Save Point Scene Names Data Not Found");
        }

        if (SaveData.Instance.savePointPosition != null) // Load the save point position if it exist
        {
            respawnPoint = SaveData.Instance.savePointPosition;
            Debug.Log("Save Point Position Data Found // #data/save.savepoint.data");
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
            Debug.Log("Save Point Position Data Not Found");
        }

        PlayerController.Instance.transform.position = respawnPoint;
        StartCoroutine(CanvasController.Instance.DeactiveDeathScreen());
        PlayerController.Instance.Respawned();
    }

    public IEnumerator PlayAudio()
    {
        yield return new WaitForSeconds(10f);
        audioSource.Play();
    }

    public void OnPauseMenu(InputAction.CallbackContext context)
    {
        if (context.started && !gameIsPaused)
        {
            pauseMenu.FadeInUI(fadeTime);
            Time.timeScale = 0;
            GameManager.Instance.gameIsPaused = true;
        }
    }

    public IEnumerator ActiveVictoryScreen()
    {
        yield return new WaitForSeconds(0.8f);
        victoryScreen.FadeInUI(fadeTime);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        GameManager.Instance.gameIsPaused = false;
    }

    public void OnQuit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
        #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
        #endif

        #if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
        #elif (UNITY_STANDALONE)
            Application.Quit();
        #elif (UNITY_WEBGL)
            SceneManager.LoadScene("QuitScene")
        #endif
        }
    }
}
