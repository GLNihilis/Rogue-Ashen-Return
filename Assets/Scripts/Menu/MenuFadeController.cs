using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuFadeController : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Image imageContinueButton;
    [SerializeField] Color activeColor = new Color(0, 0, 0, 0);
    [SerializeField] Color inactiveColor = new Color(1, 1, 1, 0.39f);

    private FadeMenuUI fadeMenuUI;
    [SerializeField] private float fadeTime;

    // Start is called before the first frame update
    void Start()
    {
        fadeMenuUI = GetComponent<FadeMenuUI>();
        fadeMenuUI.FadeOutUI(fadeTime);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateContinueButtonState();
    }

    private void UpdateContinueButtonState()
    {
        string saveFilePath = Application.persistentDataPath + "/save.player.data";
        if (File.Exists(saveFilePath))
        {
            continueButton.interactable = true;
            imageContinueButton.color = activeColor;
        }
        else
        {
            continueButton.interactable = false;
            imageContinueButton.color = inactiveColor;
        }
    }

    IEnumerator FadeAndStartGame(string _sceneToLoad)
    {
        fadeMenuUI.FadeInUI(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(_sceneToLoad);
    }

    public void CallFadeAndStartGame(string _sceneToLoad)
    {
        StartCoroutine(FadeAndStartGame(_sceneToLoad));
    }

    public void StartNewGame(string _sceneToLoad)
    {
        string savePointPath = Application.persistentDataPath + "/save.savepoint.data";
        string playerDataPath = Application.persistentDataPath + "/save.player.data";

        if (File.Exists(savePointPath))
        {
            File.Delete(savePointPath);
            Debug.Log("Deleted #data/save.savepoint.data");
        }

        if (File.Exists(playerDataPath))
        {
            File.Delete(playerDataPath);
            Debug.Log("Deleted #data/save.player.data");
        }

        if (SaveData.Instance.sceneNames != null)
        {
            SaveData.Instance.sceneNames.Clear();
            Debug.Log("Cleared Scene");
        }

        //SaveData.Instance.Initialize();

        CallFadeAndStartGame(_sceneToLoad);
    }

    public IEnumerator LoadPlayerDataAfterSceneLoad()
    {
        yield return new WaitUntil(() => PlayerController.Instance != null);
        SaveData.Instance.Load_PlayerData();
    }

    private IEnumerator FadeAndLoadGame(string _sceneToLoad)
    {
        fadeMenuUI.FadeInUI(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(_sceneToLoad);
        yield return StartCoroutine(LoadPlayerDataAfterSceneLoad());
    }

    public void ContinueGame(string _sceneToLoad)
    {
        if (File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            StartCoroutine(FadeAndLoadGame(_sceneToLoad));
            Debug.Log("Loaded #data/save.player.data");
        }
        else
        {
            Debug.Log("No Load Data Found #data/save.player.data");
        }
    }
}
