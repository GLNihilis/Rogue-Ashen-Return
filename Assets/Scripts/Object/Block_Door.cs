using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Door : MonoBehaviour
{
    Collider2D col;
    bool callOne;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!callOne &&
                PlayerController.Instance.unlocked_Key_1 &&
                PlayerController.Instance.unlocked_Key_2 &&
                PlayerController.Instance.unlocked_Key_3 &&
                PlayerController.Instance.unlocked_Key_4 &&
                PlayerController.Instance.unlocked_Key_5)
        {
            callOne = true;
            col.isTrigger = true;
        }
    }
}
