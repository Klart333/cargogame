using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : Singleton<TutorialManager>
{
    public const int FirstLevelSceneIndex = 1;

    [SerializeField]
    private GameObject driveInstructions;

    [SerializeField]
    private GameObject lootInstructions;

    private int drive = -1;
    private int loot = -1;
    private LootMenuController lootController;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene fromScene, Scene ToScene)
    {
        if (lootController != null)
        {
            lootController.OnInLoot -= LootController_OnInLoot;
            lootController = null;
        }

        if (ToScene.buildIndex == 0)
        {
            lootController = FindObjectOfType<LootMenuController>();
            if (lootController != null)
            {
                lootController.OnInLoot += LootController_OnInLoot;
            }
        }

        if (ToScene.buildIndex == FirstLevelSceneIndex)
        {
            DisplayDriveInstructions();
        }
    }

    private void LootController_OnInLoot()
    {
        if (loot != -1)
        {
            return;
        }

        loot = PlayerPrefs.GetInt("LootInstructions");
        if (loot == 0)
        {
            PlayerPrefs.SetInt("LootInstructions", 1);
            Instantiate(lootInstructions, FindObjectOfType<Canvas>().transform);
        }
    }

    private void DisplayDriveInstructions()
    {
        if (drive != -1)
        {
            return;
        }

        drive = PlayerPrefs.GetInt("DriveInstructions");
        if (drive == 0)
        {
            PlayerPrefs.SetInt("DriveInstructions", 1);
            Instantiate(driveInstructions, FindObjectOfType<Canvas>().transform);
        }
    }
}
