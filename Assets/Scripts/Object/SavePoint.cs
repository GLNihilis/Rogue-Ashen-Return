using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    public bool inRange;
    public bool interacted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            interacted = true;

            if (interacted)
            {
                SaveData.Instance.savePointSceneNames = SceneManager.GetActiveScene().name;
                SaveData.Instance.savePointPosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
                SaveData.Instance.Save_SavePoint();
                SaveData.Instance.Save_PlayerData();
            }
            else
            {
                Debug.Log("Save Point Fail !");
            }
            
            Debug.Log("Save Point Checked ! #data/save.savepoint.data");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
            interacted = false;
        }
    }
}
