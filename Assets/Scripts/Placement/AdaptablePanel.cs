using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaptablePanel : MonoBehaviour
{
    RectTransform rt;

    private void Awake()
    {
        if (name.Equals("OperatorPanel"))
        {
            foreach (RectTransform duo in rt)
            {
                if (!duo.name.Equals("Title"))
                {
                    duo.gameObject.SetActive(false);
                    foreach (RectTransform o in duo)
                    {
                        if (!o.name.Equals("LockFrame"))
                        {
                            o.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        UpdatePanel();
    }

    // panel changes size based on the height of its active children
    public void UpdatePanel()
    {
        if (rt == null)
        {
            rt = GetComponent<RectTransform>();
        }

        float sumOfHeights = 0;
        foreach (RectTransform child in rt)
        {
            if (child.gameObject.activeSelf)
            {
                sumOfHeights += child.GetComponent<RectTransform>().rect.height;
            }
        }
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, sumOfHeights);
    }
}
