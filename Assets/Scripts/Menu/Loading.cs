using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene()
    {
        StartCoroutine(WaitLoading());
        SceneManager.LoadScene("Dungeon_L0");
    }

    public IEnumerator WaitLoading()
    {
        yield return new WaitForSeconds(3f);
    }
}
