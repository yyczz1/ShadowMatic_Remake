using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public int TargetIndex = 0;
    public float pressTime = 0;
    private bool isPressed = false;
    public bool isSetParent = false;
    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.RotateTarget = InputManager.Instance.RotateTargetList[TargetIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
        {
            pressTime += Time.deltaTime; 
        }

        if (pressTime >= 0.8f)
        {
            ControlBtnLongPress();
        }

        if (Input.GetKeyDown("z"))
        {
            pressTime = 0;
            isPressed = true;
        }
        if (Input.GetKeyUp("z"))
        {
            if (pressTime <= 0.2f && !InputManager.Instance.isRotateBoth)
            {
                ControlBtnSingleClick();
            }
            pressTime = 0;
            isSetParent = false;
            isPressed = false;
            //InputManager.Instance.isRotateBoth = false;
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        pressTime = 0;
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (pressTime <= 0.2f && !InputManager.Instance.isRotateBoth)
        {
            ControlBtnSingleClick();
        }

        pressTime = 0;
        isPressed = false;
        isSetParent = false;
        //InputManager.Instance.isRotateBoth = false ;
    }

    private void ControlBtnSingleClick()
    {
        TargetIndex += 1;
        if (TargetIndex > InputManager.Instance.RotateTargetList.Count - 1)
        {
            TargetIndex = 0;
        }
        InputManager.Instance.RotateTarget = InputManager.Instance.RotateTargetList[TargetIndex];
        //Debug.Log("TargetIndex " + TargetIndex);
    }

    private void ControlBtnLongPress()
    {
        if (!isSetParent) 
        {
            isSetParent = true;
            Vector3 parentPos = new Vector3(0,0,0);
            foreach (var child in InputManager.Instance.RotateTargetList)
            {
                parentPos = parentPos + child.transform.position;
            }
            parentPos /= InputManager.Instance.RotateTargetList.Count;
            InputManager.Instance.TargetParent.position = parentPos;
            InputManager.Instance.isRotateBoth = !InputManager.Instance.isRotateBoth;
            //foreach (var item in InputManager.Instance.RotateTargetList)
            //{
            //    item.SetParent(InputManager.Instance.TargetParent);
            //}
        }
        //Debug.Log("Long Press");
    }
}
