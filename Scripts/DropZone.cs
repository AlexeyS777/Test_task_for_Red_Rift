using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//----------------------------------------------------------------------------
//
//                          Клас, отвечающий за дроп зону.
//                          Описывает способность дроп зоны принимать карты
//
//----------------------------------------------------------------------------

public class DropZone : MonoBehaviour, IDropHandler
{   
    public static event GameManager.NextCard RotateCards;       // Событие, вызывающее движение карт в руке, подписан в GameManager                                                                
    private DragableItem[] cards;                               // Массив карт, находящихся в дроп зоне (определяет место карты)
    private Transform hand;                                     // Переменная для сравнения карт (взята из руки или дроп зоны)

    private void Awake()
    {
        DragableItem.DelCrdInDrpZone += DeleteCard;             // Событие удаляет карту из массива карт для дроп зоны
    }

    private void OnDestroy()
    {
        DragableItem.DelCrdInDrpZone -= DeleteCard;
    }

    private void DeleteCard(int value)                          // Метод удаления карт из массива дроп зоны
    {
        cards[value] = null;
    }

    public void Inicializing(int value)                         // Метод отпределяет параметры дроп зоны
    {
        cards = new DragableItem[value];
        hand = this.transform.parent.GetChild(0);        
    }

    public void OnDrop(PointerEventData eventData)              // Метод позволяющей карте упасть в дроп зону
    {
        for (int i = 0;i<cards.Length;i++)                      // Ищет свободное место в дроп зоне
        {
            if (cards[i] == null)                               // Если место свободно, заполняет его картой и прерывает цикл
            {
                DragableItem dropCard = eventData.pointerDrag.GetComponent<DragableItem>();
                cards[i] = dropCard;
                                                                // Если карта из руки, вызывает событие движения карт в руке
                if (RotateCards != null && dropCard.parentAfterDrag == hand) RotateCards(dropCard.parentIndex); 
                dropCard.parentAfterDrag = this.transform;      // Назначение места положения карты (дроп зона или рука)           
                dropCard.parentIndex = i;                       // Назначение места в дроп зоне
                break;
            }
        }
    }    
}
