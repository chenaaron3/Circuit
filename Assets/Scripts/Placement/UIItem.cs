using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public string symbol;
    public GameObject prefab;

    private void Start()
    {
        if (prefab != null)
        {
            GetComponent<Image>().color = prefab.GetComponent<SpriteRenderer>().color;
        }
        if (symbol.Equals("ViewFunction"))
        {
            GetComponent<Button>().onClick.AddListener(delegate
            {
                FunctionManager.instance.ViewFunction(prefab.transform.parent.parent.gameObject);
            });
        }
        else if (symbol.Equals("Delete"))
        {
            GetComponent<Button>().onClick.AddListener(delegate
            {
                FunctionManager.instance.RemoveFunction(transform.parent.gameObject);
            });
        }
        else
        {
            GetComponent<Button>().onClick.AddListener(delegate
            {
                if (PlacementManager.instance.holdingItem == null)
                {
                    if (prefab != null)
                    {
                        PlacementManager.instance.SetHoldingItem(Instantiate(prefab));
                    }
                }
            });
        }
    }

    private void OnEnable()
    {
        // updates parent panel
        GetComponentInParent<AdaptablePanel>().UpdatePanel();
    }

    private void OnDisable()
    {
        AdaptablePanel ap = GetComponentInParent<AdaptablePanel>();
        if (ap != null)
        {
            // updates parent panel
            ap.UpdatePanel();
        }
    }

    private void Update()
    {
        // displays delete option
        if (symbol.Equals("ViewFunction"))
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (transform.parent.Find("Delete") != null)
                {
                    transform.parent.Find("Delete").gameObject.SetActive(true);
                }
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                if (transform.parent.Find("Delete") != null)
                {
                    transform.parent.Find("Delete").gameObject.SetActive(false);
                }
            }
        }
    }
}
