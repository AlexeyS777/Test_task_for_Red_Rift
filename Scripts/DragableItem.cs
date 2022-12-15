using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//----------------------------------------------------------------------------------------------
//
//                                  Класс, отвечающий за механику drag and drop карт
//                                  Включает и выключает свечение карты
//
//----------------------------------------------------------------------------------------------

public class DragableItem : MonoBehaviour, IDragHandler,IBeginDragHandler, IEndDragHandler
{    
    public static event GameManager.NextCard DelCrdInDrpZone;           // Удаляет карту из массива карт дроп зоны, подписан в DropZone
    public static event GameManager.NextCard DelCrdInHnd;               // Удаляет карту из массива карт руки, подписан в GameManager
    public static event GameManager.EmptySlot FindSlotInHand;           // Ищет пустой слов в массиве карт руки, подписан в GameManager

    [HideInInspector] 
    public Transform parentAfterDrag;                                   // Индикатор положения карты (в руке или в дроп зоне)
    public CardController cardCntrl;
    private Vector3 handPosition;                                       // Позиция карты в руке
    private Transform handTransform;                                    // Положение руки в иерархии (нет дроп зоны для руки)
    

    public int parentIndex { get; set; }                                // Позиция карты в массивах карт руки и дроп зоны

    private void Start()
    {
        cardCntrl = GetComponent<CardController>();
        handTransform = transform.parent;
        handPosition = transform.position;
        parentAfterDrag = handTransform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {        
        this.StartCoroutine(Outline(1));                                        // Включает подсветку карты

        if (this.transform.parent.tag == "DropZone" && DelCrdInDrpZone != null) // Удаляет карту из массива дроп зоны 
        {
            DelCrdInDrpZone(parentIndex);
        }
        else if (DelCrdInHnd != null)
        {
            DelCrdInHnd(parentIndex);                                           // Удаляет карту из массива руки 
        }

        if(parentAfterDrag != handTransform) parentAfterDrag = null;            // Устраняет баг с положением карты
        cardCntrl.cardRotation.rotation = Quaternion.Euler(Vector3.zero);
        transform.SetParent(transform.parent.transform.parent);
        transform.SetAsLastSibling();
        cardCntrl.backGround.raycastTarget = false;                             // Отключает  raycast для арта карты
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        this.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.StopAllCoroutines();                                               // Устраняет баг с движением карты
        this.StartCoroutine(Outline(0));

        
        if (parentAfterDrag == null || parentAfterDrag == handTransform)        // Кладет карту в руку
        {
            parentAfterDrag = handTransform;
            transform.SetParent(handTransform);            
            transform.position = handPosition;
            if (FindSlotInHand != null) parentIndex = FindSlotInHand(cardCntrl);
        }
        else                                                                    // Кладет карту в дроп зону
        {
            transform.SetParent(parentAfterDrag);
            transform.position = parentAfterDrag.position;
        }

        
        transform.SetSiblingIndex(parentIndex);
        cardCntrl.cardRotation.rotation = GameManager.GetAngle(parentIndex);
        cardCntrl.backGround.raycastTarget = true;
    }    

    private IEnumerator Outline(float value)                                    // Включение и отключение подсветки карты
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
