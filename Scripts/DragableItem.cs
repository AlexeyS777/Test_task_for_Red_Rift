using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//----------------------------------------------------------------------------------------------
//
//                                  �����, ���������� �� �������� drag and drop ����
//                                  �������� � ��������� �������� �����
//
//----------------------------------------------------------------------------------------------

public class DragableItem : MonoBehaviour, IDragHandler,IBeginDragHandler, IEndDragHandler
{    
    public static event GameManager.NextCard DelCrdInDrpZone;           // ������� ����� �� ������� ���� ���� ����, �������� � DropZone
    public static event GameManager.NextCard DelCrdInHnd;               // ������� ����� �� ������� ���� ����, �������� � GameManager
    public static event GameManager.EmptySlot FindSlotInHand;           // ���� ������ ���� � ������� ���� ����, �������� � GameManager

    [HideInInspector] 
    public Transform parentAfterDrag;                                   // ��������� ��������� ����� (� ���� ��� � ���� ����)
    public CardController cardCntrl;
    private Vector3 handPosition;                                       // ������� ����� � ����
    private Transform handTransform;                                    // ��������� ���� � �������� (��� ���� ���� ��� ����)
    

    public int parentIndex { get; set; }                                // ������� ����� � �������� ���� ���� � ���� ����

    private void Start()
    {
        cardCntrl = GetComponent<CardController>();
        handTransform = transform.parent;
        handPosition = transform.position;
        parentAfterDrag = handTransform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {        
        this.StartCoroutine(Outline(1));                                        // �������� ��������� �����

        if (this.transform.parent.tag == "DropZone" && DelCrdInDrpZone != null) // ������� ����� �� ������� ���� ���� 
        {
            DelCrdInDrpZone(parentIndex);
        }
        else if (DelCrdInHnd != null)
        {
            DelCrdInHnd(parentIndex);                                           // ������� ����� �� ������� ���� 
        }

        if(parentAfterDrag != handTransform) parentAfterDrag = null;            // ��������� ��� � ���������� �����
        cardCntrl.cardRotation.rotation = Quaternion.Euler(Vector3.zero);
        transform.SetParent(transform.parent.transform.parent);
        transform.SetAsLastSibling();
        cardCntrl.backGround.raycastTarget = false;                             // ���������  raycast ��� ���� �����
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        this.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.StopAllCoroutines();                                               // ��������� ��� � ��������� �����
        this.StartCoroutine(Outline(0));

        
        if (parentAfterDrag == null || parentAfterDrag == handTransform)        // ������ ����� � ����
        {
            parentAfterDrag = handTransform;
            transform.SetParent(handTransform);            
            transform.position = handPosition;
            if (FindSlotInHand != null) parentIndex = FindSlotInHand(cardCntrl);
        }
        else                                                                    // ������ ����� � ���� ����
        {
            transform.SetParent(parentAfterDrag);
            transform.position = parentAfterDrag.position;
        }

        
        transform.SetSiblingIndex(parentIndex);
        cardCntrl.cardRotation.rotation = GameManager.GetAngle(parentIndex);
        cardCntrl.backGround.raycastTarget = true;
    }    

    private IEnumerator Outline(float value)                                    // ��������� � ���������� ��������� �����
    {
        if (cardCntrl.fireOutline.color.a == 0) cardCntrl.fireOutline.enabled = true;

        Color alpha = cardCntrl.fireOutline.color;
        alpha.a = value;

        while (cardCntrl.fireOutline.color.a != value)                         
        {
            cardCntrl.fireOutline.color = Vector4.MoveTowards(cardCntrl.fireOutline.color, alpha, 3 * Time.deltaTime);
            yield return null;
        }

        if (cardCntrl.fireOutline.color.a == 0) cardCntrl.fireOutline.enabled = false;
    }
}
