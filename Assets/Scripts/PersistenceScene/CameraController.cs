using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public CinemachineConfiner2D confiner;
    public CompositeCollider2D cameraBounds;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject newCameraBounds = GameObject.FindWithTag("CameraBounds");
        if (newCameraBounds != null)
        {
            cameraBounds = newCameraBounds.GetComponent<CompositeCollider2D>();
            if (cameraBounds != null && confiner != null)
            {
                confiner.m_BoundingShape2D = cameraBounds;
                confiner.InvalidateCache(); // Reset the cache of confiner
            }
        }
        else
        {
            Debug.LogWarning("No Camera Bound Found");
        }
    }

}
