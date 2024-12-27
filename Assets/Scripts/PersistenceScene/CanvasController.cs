using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public static CanvasController Instance;
    public SceneFader sceneFader;

    [SerializeField] GameObject deathScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        sceneFader = GetComponentInChildren<SceneFader>();
    }

    public void Update()
    {

    }

    public IEnumerator ActiveDeathScreen()
    {
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));

        yield return new WaitForSeconds(0.8f);
        deathScreen.SetActive(true);
        Time.timeScale = 0;
        GameManager.Instance.gameIsPaused = true;
    }

    public IEnumerator DeactiveDeathScreen()
    {
        GameManager.Instance.UnPauseGame();
        yield return new WaitForSeconds(0.5f);
        deathScreen.SetActive(false);

        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
    }
}
