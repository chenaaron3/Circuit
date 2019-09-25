using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Dropdown levelSelect;

    List<WorldLevel> levels;
    HashSet<int> discoveredLevels;
    int currentLevel = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        levelSelect.ClearOptions();
        levels = new List<WorldLevel>();
        discoveredLevels = new HashSet<int>();
        foreach (Transform child in transform)
        {
            levels.Add(child.GetComponent<WorldLevel>());
        }
        LoadLevel(0);
    }

    // go to next level
    public void NextLevel()
    {
        if (currentLevel == levels.Count - 1)
        {
            StateManager.instance.TerminateCircuit(true);
            StateManager.instance.Broadcast("Game Completed! Thanks for playing!", Color.cyan, 10);
        }
        else
        {
            UnloadLevel(currentLevel);
            currentLevel++;
            LoadLevel(currentLevel);
            StateManager.instance.TerminateCircuit(true);
        }
    }

    // unloads a level
    void UnloadLevel(int i)
    {
        levels[i].gameObject.SetActive(false);
    }

    // loads a level
    void LoadLevel(int i)
    {
        if (!discoveredLevels.Contains(i))
        {
            discoveredLevels.Add(i);
            levelSelect.AddOptions(new List<string> { "Lvl. " + i + ": " + levels[i].name });
        }
        levels[i].gameObject.SetActive(true);
        levels[i].UnlockUI();
        levels[i].Reset();
        levelSelect.value = i;
    }

    // random access level
    public void SelectLevel(int i)
    {
        UnloadLevel(currentLevel);
        currentLevel = i;
        LoadLevel(currentLevel);
    }
}
