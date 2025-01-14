using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;

    // Scene Stuff
    public HashSet<string> sceneNames;

    // Save Point Stuff
    public string savePointSceneNames;
    public Vector2 savePointPosition;

    // Player Stuff
    public float playerHealth;
    public float playerMana;
    public float playerStamina;
    public Vector2 playerPosition;
    public string lastScene;

    public bool playerUnlockedDoubleJump;
    public bool playerUnlockedHealing;
    public bool playerUnlockedRolling;

    public bool playerUnlockedFireBall;
    public bool playerUnlockedWindSlash;
    public bool playerUnlockedEarthBump;
    public bool playerUnlockedWaterTornado;

    public bool Key_1;
    public bool Key_2;
    public bool Key_3;
    public bool Key_4;
    public bool Key_5;

    public void Initialize()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.savepoint.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create((Application.persistentDataPath + "/save.savepoint.data")));
        }

        if (!File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create((Application.persistentDataPath + "/save.player.data")));
        }

        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }

    public void Save_SavePoint()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.savepoint.data")))
        {
            writer.Write(savePointSceneNames);

            writer.Write(savePointPosition.x);
            writer.Write(savePointPosition.y);
        }
    }

    public void Load_SavePoint()
    {
        if (File.Exists(Application.persistentDataPath + "/save.savepoint.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.savepoint.data")))
            {
                savePointSceneNames = reader.ReadString();

                savePointPosition.x = reader.ReadSingle();
                savePointPosition.y = reader.ReadSingle();
            }
        }
        else
        {
            Debug.Log("File doesn't exist // #data/save.savepoint.data");
        }
    }

    public void Save_PlayerData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
        {
            playerHealth = PlayerController.Instance.damageable.Health;
            writer.Write(playerHealth);
            playerMana = PlayerController.Instance.spellable.Mana;
            writer.Write(playerMana);
            playerStamina = PlayerController.Instance.spellable.Stamina;
            writer.Write(playerStamina);

            playerUnlockedDoubleJump = PlayerController.Instance.unlocked_DoubleJump;
            writer.Write(playerUnlockedDoubleJump);
            playerUnlockedHealing = PlayerController.Instance.unlocked_Healing;
            writer.Write(playerUnlockedHealing);
            playerUnlockedRolling = PlayerController.Instance.unlocked_Rolling;
            writer.Write(playerUnlockedRolling);

            playerUnlockedFireBall = PlayerController.Instance.unlocked_FireBall;
            writer.Write(playerUnlockedFireBall);
            playerUnlockedWindSlash = PlayerController.Instance.unlocked_WindSlash;
            writer.Write(playerUnlockedWindSlash);
            playerUnlockedEarthBump = PlayerController.Instance.unlocked_EarthBump;
            writer.Write(playerUnlockedEarthBump);
            playerUnlockedWaterTornado = PlayerController.Instance.unlocked_WaterTornado;
            writer.Write(playerUnlockedWaterTornado);

            Key_1 = PlayerController.Instance.unlocked_Key_1;
            writer.Write(Key_1);
            Key_2 = PlayerController.Instance.unlocked_Key_2;
            writer.Write(Key_2);
            Key_3 = PlayerController.Instance.unlocked_Key_3;
            writer.Write(Key_3);
            Key_4 = PlayerController.Instance.unlocked_Key_4;
            writer.Write(Key_4);
            Key_5 = PlayerController.Instance.unlocked_Key_5;
            writer.Write(Key_5);


            playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
    }

    public void Load_PlayerData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {
                playerHealth = reader.ReadSingle();
                playerMana = reader.ReadSingle();
                playerStamina = reader.ReadSingle();

                playerUnlockedDoubleJump = reader.ReadBoolean();
                playerUnlockedHealing = reader.ReadBoolean();
                playerUnlockedRolling = reader.ReadBoolean();

                playerUnlockedFireBall = reader.ReadBoolean();
                playerUnlockedWindSlash = reader.ReadBoolean();
                playerUnlockedEarthBump = reader.ReadBoolean();
                playerUnlockedWaterTornado = reader.ReadBoolean();

                Key_1 = reader.ReadBoolean();
                Key_2 = reader.ReadBoolean();
                Key_3 = reader.ReadBoolean();
                Key_4 = reader.ReadBoolean();
                Key_5 = reader.ReadBoolean();

                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();

                lastScene = reader.ReadString();

                if (PlayerController.Instance != null)
                {
                    SceneManager.LoadScene(lastScene);

                    PlayerController.Instance.transform.position = playerPosition;

                    PlayerController.Instance.unlocked_Key_1 = Key_1;
                    PlayerController.Instance.unlocked_Key_2 = Key_2;
                    PlayerController.Instance.unlocked_Key_3 = Key_3;
                    PlayerController.Instance.unlocked_Key_4 = Key_4;
                    PlayerController.Instance.unlocked_Key_5 = Key_5;

                    PlayerController.Instance.unlocked_FireBall = playerUnlockedFireBall;
                    PlayerController.Instance.unlocked_WindSlash = playerUnlockedWindSlash;
                    PlayerController.Instance.unlocked_EarthBump = playerUnlockedEarthBump;
                    PlayerController.Instance.unlocked_WaterTornado = playerUnlockedWaterTornado;

                    PlayerController.Instance.unlocked_DoubleJump = playerUnlockedDoubleJump;
                    PlayerController.Instance.unlocked_Healing = playerUnlockedHealing;
                    PlayerController.Instance.unlocked_Rolling = playerUnlockedRolling;

                    PlayerController.Instance.spellable.Stamina = playerStamina;
                    PlayerController.Instance.spellable.Mana = playerMana;
                    PlayerController.Instance.damageable.Health = playerHealth;
                }
                else
                {
                    Debug.LogError("Player instance is missing!");
                }
            }
        }
        else
        {
            Debug.Log("File doesn't exist // #data/save.player.data");

            PlayerController.Instance.unlocked_Key_1 = false;
            PlayerController.Instance.unlocked_Key_2 = false;
            PlayerController.Instance.unlocked_Key_3 = false;
            PlayerController.Instance.unlocked_Key_4 = false;
            PlayerController.Instance.unlocked_Key_5 = false;


            PlayerController.Instance.unlocked_FireBall = false;
            PlayerController.Instance.unlocked_WindSlash = false;
            PlayerController.Instance.unlocked_EarthBump = false;
            PlayerController.Instance.unlocked_WaterTornado = false;

            PlayerController.Instance.unlocked_DoubleJump = false;
            PlayerController.Instance.unlocked_Healing = false;
            PlayerController.Instance.unlocked_Rolling = false;

            PlayerController.Instance.spellable.Stamina = PlayerController.Instance.spellable.MaxStamina;
            PlayerController.Instance.spellable.Mana = PlayerController.Instance.spellable.MaxMana;
            PlayerController.Instance.damageable.Health = PlayerController.Instance.damageable.MaxHealth;
        }
    }
}
