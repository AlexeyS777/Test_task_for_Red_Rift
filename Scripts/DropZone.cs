using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//----------------------------------------------------------------------------
//
//                          ����, ���������� �� ���� ����.
//                          ��������� ����������� ���� ���� ��������� �����
//
//----------------------------------------------------------------------------

public class DropZone : MonoBehaviour, IDropHandler
{   
    public static event GameManager.NextCard RotateCards;       // �������, ���������� �������� ���� � ����, �������� � GameManager                                                                
    private DragableItem[] cards;                               // ������ ����, ����������� � ���� ���� (���������� ����� �����)
    private Transform hand;                                     // ���������� ��� ��������� ���� (����� �� ���� ��� ���� ����)

    private void Awake()
    {
        DragableItem.DelCrdInDrpZone += DeleteCard;             // ������� ������� ����� �� ������� ���� ��� ���� ����
    }

    private void OnDestroy()
    {
        DragableItem.DelCrdInDrpZone -= DeleteCard;
    }

    private void DeleteCard(int value)                          // ����� �������� ���� �� ������� ���� ����
    {
        cards[value] = null;
    }

    public void Inicializing(int value)                         // ����� ����������� ��������� ���� ����
    {
        cards = new DragableItem[value];
        hand = this.transform.parent.GetChild(0);        
    }

    public void OnDrop(PointerEventData eventData)              // ����� ����������� ����� ������ � ���� ����
    {
        for (int i = 0;i<cards.Length;i++)                      // ���� ��������� ����� � ���� ����
        {
            if (cards[i] == null)                               // ���� ����� ��������, ��������� ��� ������ � ��������� ����
            {
                DragableItem dropCard = eventData.pointerDrag.GetComponent<DragableItem>();
                cards[i] = dropCard;
                                                                // ���� ����� �� ����, �������� ������� �������� ���� � ����
                if (RotateCards != null && dropCard.parentAfterDrag == hand) RotateCards(dropCard.parentIndex); 
                dropCard.parentAfterDrag = this.transform;      // ���������� ����� ��������� ����� (���� ���� ��� ����)           
                dropCard.parentIndex = i;                       // ���������� ����� � ���� ����
                break;
            }
        }
    }    
}
