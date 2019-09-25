using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLevel : MonoBehaviour
{
    public string message;
    public GameObject[] uiToUnlock;
    public GameObject[] testCases;
    int currentTestCase;

    // unlocks tools
    public void UnlockUI()
    {
        foreach (GameObject g in uiToUnlock)
        {
            g.transform.parent.gameObject.SetActive(true);
            g.SetActive(true);
        }
    }

    // revives all enemies and resets the actor's position
    public void Reset()
    {
        // resets all enemies in all test cases
        foreach (GameObject testCase in testCases)
        {
            testCase.SetActive(false);
            foreach (Transform child in testCase.transform)
            {
                child.gameObject.SetActive(true);
                if (child.CompareTag("Enemy"))
                {
                    child.GetComponent<Enemy>().Reset();
                }
            }
        }
        // goes to first test case
        currentTestCase = 0;
        ResetTestCase(currentTestCase);
        // displays message
        if (!message.Equals(""))
        {
            StateManager.instance.DisplayMessage(message);
        }
    }

    void ResetTestCase(int testCaseIndex)
    {
        // repositions actor
        testCases[testCaseIndex].SetActive(true);
        GameObject start = testCases[testCaseIndex].transform.Find("Start").gameObject;
        Actor.instance.transform.position = start.transform.position;
        Actor.instance.SetDirection(Vector2Int.RoundToInt(start.transform.right));
    }

    // checks if all the enemies are killed, called when actor meets donut
    public IEnumerator CheckTestCase(GameObject donut)
    {
        // checking if passed this test case
        foreach (Transform child in testCases[currentTestCase].transform)
        {
            if (child.CompareTag("Enemy"))
            {
                if (child.gameObject.activeSelf)
                {
                    StateManager.instance.TerminateCircuit(false);
                    yield return null;
                }
            }
        }

        AudioManager.instance.PlaySound("Complete");
        yield return Actor.instance.Celebrate(donut);

        // if on last test case, go to next level
        if (currentTestCase == testCases.Length - 1)
        {
            LevelManager.instance.NextLevel();
        }
        // else go to next test case
        else
        {
            testCases[currentTestCase].SetActive(false);
            currentTestCase++;
            ResetTestCase(currentTestCase);
        }
    }
}