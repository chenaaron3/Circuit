using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionManager : MonoBehaviour
{
    public static FunctionManager instance;

    // management
    public GameObject currentFunction;
    List<GameObject> uiUFunctionList;
    List<GameObject> circuitUFunctionList;
    List<GameObject> uiDFunctionList;
    List<GameObject> circuitDFunctionList;
    Queue<Texture2D> functionSymbols;

    // creation
    public GameObject functionSetUI;
    public GameObject functionDefinitionTemplate;
    public GameObject functionCallTemplate;
    public GameObject functionScope;
    public Texture2D[] uFunctionSymbolTextures;
    public Texture2D[] dFunctionSymbolTextures;

    // constants
    public RectTransform initialUIUFunction;
    public Transform initialCircuitUFunction;
    public RectTransform initialUIDFunction;
    public Transform initialCircuitDFunction;
    float uiMargin = -100;
    float circuitMargin = -10;

    // definition descriptions
    public string[] definitionDescriptions;

    public Button writeButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // undefined
        uiUFunctionList = new List<GameObject>();
        uiUFunctionList.Add(initialUIUFunction.gameObject);
        circuitUFunctionList = new List<GameObject>();
        circuitUFunctionList.Add(initialCircuitUFunction.gameObject);

        // defined
        uiDFunctionList = new List<GameObject>();
        uiDFunctionList.Add(initialUIDFunction.gameObject);
        circuitDFunctionList = new List<GameObject>();
        circuitDFunctionList.Add(initialCircuitDFunction.gameObject);

        // create a queue with randomized textures
        functionSymbols = new Queue<Texture2D>();
        List<Texture2D> textureList = new List<Texture2D>();
        for (int j = 0; j < uFunctionSymbolTextures.Length; j++)
        {
            textureList.Add(uFunctionSymbolTextures[j]);
        }
        while (textureList.Count > 0)
        {
            Texture2D t = textureList[(int)(Random.value * textureList.Count)];
            functionSymbols.Enqueue(t);
            textureList.Remove(t);
        }

        writeButton.onClick.AddListener(delegate { StateManager.instance.DisplayMessage(FindObjectOfType<WorldLevel>().message); });

        StartCoroutine(Delay());
    }

    // create the defined functions after a delay
    IEnumerator Delay()
    {
        yield return null;
        AddDefinedFunction();
        for (int j = 0; j < uiDFunctionList.Count; j++)
        {
            GameObject uiDFunction = uiDFunctionList[j];
            if (j > 0)
            {
                Destroy(uiDFunction.transform.Find("Delete").gameObject);
            }
            uiDFunction.transform.Find("Definition").GetComponent<Button>().onClick.AddListener(delegate
            {
                StateManager.instance.DisplayMessage("This function should " + definitionDescriptions[uiDFunctionList.IndexOf(uiDFunction)]);
            });
        }
        initialUIDFunction.parent.gameObject.SetActive(false);
    }

    // create defined function panel
    public void AddDefinedFunction()
    {
        foreach (Texture2D texture in dFunctionSymbolTextures)
        {
            AddFunction(texture, initialUIDFunction, initialCircuitDFunction, uiDFunctionList, circuitDFunctionList);
        }
        UpdateFunctionPositions(uiDFunctionList, circuitDFunctionList, initialUIDFunction, initialCircuitDFunction);
    }

    // add 1 undefined function
    public void AddUndefinedFunction()
    {
        if (functionSymbols.Count > 0)
        {
            AddFunction(functionSymbols.Dequeue(), initialUIUFunction, initialCircuitUFunction, uiUFunctionList, circuitUFunctionList);
        }
        else
        {
            StateManager.instance.DisplayMessage("Cannot define any more functions!!");
        }
    }

    // generalized function for defined/undefined functions
    public void AddFunction(Texture2D targetTexture, RectTransform initUIFunc, Transform initCircuitFunc, List<GameObject> uiFuncList, List<GameObject> circuitFuncList)
    {
        // creates the ui function
        GameObject uiFunction = Instantiate(functionSetUI, initUIFunc.parent);
        Sprite targetSprite = Sprite.Create(targetTexture, new Rect(0.0f, 0.0f, targetTexture.width, targetTexture.height), new Vector2(0.5f, 0.5f), 224.0f);
        targetSprite.name = targetTexture.name;
        uiFunction.transform.Find("Call").Find("RawImage").GetComponent<RawImage>().texture = targetTexture;
        uiFunction.name = targetTexture.name + uiFunction.name;
        uiFuncList.Add(uiFunction);

        // creates the circuit function
        GameObject newFunctionScope = Instantiate(functionScope, initCircuitFunc.parent);
        newFunctionScope.name = targetTexture.name + newFunctionScope.name;
        GameObject newFunctionDefinition = Instantiate(functionDefinitionTemplate, newFunctionScope.transform.Find("Operators"));
        newFunctionDefinition.transform.Find("Graphics").GetComponent<SpriteRenderer>().sprite = targetSprite;
        uiFunction.transform.Find("Definition").GetComponent<UIItem>().prefab = newFunctionDefinition;
        newFunctionDefinition.name = targetTexture.name + newFunctionDefinition.name;
        GameObject newFunctionCall = Instantiate(functionCallTemplate, new Vector2(1000, 1000), Quaternion.identity);
        newFunctionCall.transform.Find("Graphics").GetComponent<SpriteRenderer>().sprite = targetSprite;
        uiFunction.transform.Find("Call").GetComponent<UIItem>().prefab = newFunctionCall;
        newFunctionCall.name = targetTexture.name + newFunctionCall.name;
        circuitFuncList.Add(newFunctionScope);

        UpdateFunctionPositions(uiUFunctionList, circuitUFunctionList, initialUIUFunction, initialCircuitUFunction);
    }

    // deleting an undefined function
    public void RemoveFunction(GameObject uiFunction)
    {
        // removes function
        int currentIndex = circuitUFunctionList.IndexOf(currentFunction);
        int index = uiUFunctionList.IndexOf(uiFunction);
        GameObject ui = uiUFunctionList[index];
        GameObject circuit = circuitUFunctionList[index];
        uiUFunctionList.RemoveAt(index);
        circuitUFunctionList.RemoveAt(index);
        // destroys function
        Texture2D shape = (Texture2D)ui.transform.Find("Call").Find("RawImage").GetComponent<RawImage>().texture;
        functionSymbols.Enqueue(shape);
        foreach (Function f in FindObjectsOfType<Function>())
        {
            if (f.functionName.Equals(shape.name))
            {
                Destroy(f.gameObject);
            }
        }
        DestroyImmediate(ui);
        Destroy(circuit);
        // repositions
        UpdateFunctionPositions(uiUFunctionList, circuitUFunctionList, initialUIUFunction, initialCircuitUFunction);
        // reposition camera if deleting previous function
        if (index <= currentIndex)
        {
            StateManager.instance.mainCamera.transform.position -= new Vector3(0, circuitMargin, 0);
        }
    }

    // repositions functions based on the creation order
    void UpdateFunctionPositions(List<GameObject> uifl, List<GameObject> cfl, RectTransform iuif, Transform icf)
    {
        for (int j = 0; j < uifl.Count; j++)
        {
            uifl[j].GetComponent<RectTransform>().position = new Vector2(iuif.position.x, iuif.position.y + uiMargin * j);
            cfl[j].transform.position = new Vector2(icf.position.x, icf.position.y + circuitMargin * j);
        }
        iuif.GetComponentInParent<AdaptablePanel>().UpdatePanel();
    }

    // repositions camera to the function and sets currentFunction
    public void ViewFunction(GameObject function)
    {
        StateManager.instance.mainCamera.transform.position = function.transform.position + new Vector3(0, 0, -10);
        currentFunction = function;
    }

    // resets a given function
    public void ResetFunction(GameObject function)
    {
        foreach (Transform o in function.transform.Find("Operators"))
        {
            o.gameObject.SetActive(true);
        }
    }

    // resets all functions
    public void ResetAllFunctions()
    {
        foreach (GameObject function in circuitUFunctionList)
        {
            ResetFunction(function);
        }
        foreach (GameObject function in circuitDFunctionList)
        {
            ResetFunction(function);
        }
    }

    // resets current Function
    public void ClearCurrentFunction()
    {
        foreach (Transform o in currentFunction.transform.Find("Operators"))
        {
            Function f = o.GetComponent<Function>();
            if (f != null && f.symbol.Equals("Definition"))
            {
                o.transform.localPosition = Vector3.zero;
            }
            else
            {
                Destroy(o.gameObject);
            }
        }
    }
}
