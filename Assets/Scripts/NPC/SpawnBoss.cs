using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public static SpawnBoss Instance;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject boss;
    CompositeCollider2D col;
    public GameObject bossUI;
    bool callOne;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<CompositeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!callOne)
            {
                StartCoroutine(WalkIntoBossRoom());
                callOne = true;
            }
        }
    }

    public IEnumerator WalkIntoBossRoom()
    {
        yield return new WaitForSeconds(2f);
        col.isTrigger = false;
        GameObject bossInstance = Instantiate(boss, spawnPoint.position, Quaternion.identity);
    }

    public void IsNotTriggered()
    {
        col.isTrigger = true;
    }
}
