using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;
    public GameObject buildObjects;
    public GameObject runObjects;
    public GameObject buildWorldMap;
    public GameObject mapInstructions;
    public Text banner;
    public Text message;
    public GameObject mainCamera;
    public GameObject circuitCamera;
    public InputField hackEnabler;
    public Button hack;

    [HideInInspector]
    public string mode;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mode = "Build";
        hackEnabler.onEndEdit.AddListener(delegate
        {
            if (hackEnabler.text.ToLower().Equals("enable"))
            {
                hack.gameObject.SetActive(true);
            }
            else if (hackEnabler.text.ToLower().Equals("disable"))
            {
                hack.gameObject.SetActive(false);
            }
        });
    }

    // starts thread at start
    public void ExecuteCircuit()
    {
        Debug.Log("running circuit");
        // positions thread and starts it
        Thread.instance.gameObject.SetActive(true);
        // resets level
        FindObjectOfType<WorldLevel>().Reset();
        // repositions camera to the start function
        circuitCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.transform.position = new Vector3(100, 100, -10);

        // build to run
        buildObjects.SetActive(false);
        runObjects.SetActive(true);
        mode = "Run";

        AudioManager.instance.PlaySound("Execute");
    }

    // turns off thread
    public void TerminateCircuit(bool levelComplete)
    {
        if (levelComplete)
        {
            Broadcast("Level Complete!", Color.green);
            AudioManager.instance.PlaySound("Complete");
        }
        else
        {
            Broadcast("Level Failed!", Color.red);
            AudioManager.instance.PlaySound("Terminate");
        }

        Debug.Log("terminating circuit");
        // removes thread
        Thread.instance.gameObject.SetActive(false);
        // resets level
        FindObjectOfType<WorldLevel>().Reset();
        // repositions camera to the start function
        circuitCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.transform.position = new Vector3(0, 0, -10);
        // resets all functions
        FunctionManager.instance.ResetAllFunctions();

        // run to build
        buildObjects.SetActive(true);
        runObjects.SetActive(false);
        mode = "Build";
    }

    // broadcast routine
    public void Broadcast(string s, Color c, float duration = 2)
    {
        StartCoroutine(BroadcastRoutine(s, c, duration));
    }

    // presents a message on the banner
    private IEnumerator BroadcastRoutine(string s, Color c, float duration)
    {
        banner.color = c;
        banner.text = s;
        banner.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        banner.transform.parent.gameObject.SetActive(false);
    }

    // presents a message on the bottom text component
    public void DisplayMessage(string s)
    {
        message.text = s;
        message.transform.parent.gameObject.SetActive(true);
    }

    // viewing map
    private void Update()
    {
        if (mode.Equals("Build"))
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                buildWorldMap.SetActive(!buildWorldMap.activeSelf);
                mapInstructions.SetActive(!mapInstructions.activeSelf);
            }
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            hackEnabler.gameObject.SetActive(!hackEnabler.gameObject.activeSelf);
            hackEnabler.Select();
        }
    }
}
