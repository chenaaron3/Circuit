using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager instance;

    public GameObject holdingItem;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (StateManager.instance.mode.Equals("Run"))
        {
            return;
        }

        // placing an item
        if (holdingItem != null)
        {
            Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // if click
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RaycastHit2D circuitFloorHit = Physics2D.Raycast(mouseWorldPoint, Vector2.zero, .1f, 1 << 11);
                // if selecting a valid circuit block
                if (circuitFloorHit)
                {
                    // if circuit is editable
                    if (!circuitFloorHit.collider.transform.parent.parent.CompareTag("ReadOnly"))
                    {
                        holdingItem.SetActive(false);
                        RaycastHit2D otherOperatorHit = Physics2D.Raycast(mouseWorldPoint, Vector2.zero, .1f, 1 << 8);
                        // if trying to replace operator
                        if (otherOperatorHit)
                        {
                            Function f = otherOperatorHit.collider.GetComponent<Function>();
                            // if trying to replace a function definition
                            if (f != null && f.symbol == "Definition")
                            {
                                f.lastPosition = f.transform.position;
                            }
                            // swap holding and placed item
                            PlaceHoldingItem(circuitFloorHit.collider.gameObject);
                            SetHoldingItem(otherOperatorHit.collider.gameObject);
                        }
                        // if placing over empty block
                        else
                        {
                            // place the holding item
                            PlaceHoldingItem(circuitFloorHit.collider.gameObject);
                        }
                    }
                }
                // if trying to place on non-circuit block
                else
                {
                    Function f = holdingItem.GetComponent<Function>();
                    // if function definition, put it back on its last placed position
                    if (f != null && f.symbol == "Definition")
                    {
                        RaycastHit2D otherOperatorHit = Physics2D.Raycast(f.lastPosition, Vector2.zero, .1f, 1 << 8);
                        // if last position is not already occupied by another operator
                        if (!otherOperatorHit)
                        {
                            f.transform.position = f.lastPosition;
                            holdingItem = null;
                        }
                    }
                    // else delete
                    else
                    {
                        Destroy(holdingItem);
                    }
                }
            }
            // have holding item follow the mouse
            else
            {
                holdingItem.transform.position = mouseWorldPoint;
            }
        }
        // not holding anything, relocating
        else
        {
            // if click
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D otherOperatorHit = Physics2D.Raycast(mouseWorldPoint, Vector2.zero, .1f, 1 << 8);
                // if clicking on operator
                if (otherOperatorHit)
                {
                    // if circuit is editable
                    if (!otherOperatorHit.collider.transform.parent.parent.CompareTag("ReadOnly"))
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            otherOperatorHit.collider.GetComponent<Operator>().Turn();
                        }
                        else if (Input.GetKey(KeyCode.LeftControl))
                        {
                            otherOperatorHit.collider.GetComponent<Operator>().Delete();
                        }
                        else
                        {
                            // replace holding item
                            SetHoldingItem(otherOperatorHit.collider.gameObject);
                            Function f = holdingItem.GetComponent<Function>();
                            // if function definition, record position
                            if (f != null && f.symbol == "Definition")
                            {
                                f.lastPosition = f.transform.position;
                            }
                        }
                    }
                }
            }
        }
    }

    private void PlaceHoldingItem(GameObject circuitFloor)
    {
        // assigns parent
        holdingItem.transform.position = circuitFloor.transform.position;
        holdingItem.transform.parent = circuitFloor.transform.parent.parent.Find("Operators");
        Action holdingAction = holdingItem.GetComponent<Action>();
        if (holdingAction != null)
        {
            foreach (Action a in holdingItem.transform.parent.GetComponentsInChildren<Action>())
            {
                if (a != holdingAction && a.symbol.Equals(holdingAction.symbol))
                {
                    Destroy(holdingItem);
                    holdingItem = null;
                    return;
                }
            }
        }
        holdingItem.SetActive(true);
        holdingItem.GetComponent<SpriteRenderer>().sortingLayerName = "Operator";
        foreach (SpriteRenderer childsr in holdingItem.GetComponentsInChildren<SpriteRenderer>())
        {
            childsr.sortingLayerName = "Operator";
        }
        holdingItem = null;
    }

    public void SetHoldingItem(GameObject item)
    {
        holdingItem = item;
        item.GetComponent<SpriteRenderer>().sortingLayerName = "Holding";
        foreach (SpriteRenderer childsr in item.GetComponentsInChildren<SpriteRenderer>())
        {
            childsr.sortingLayerName = "Holding";
        }
    }
}
