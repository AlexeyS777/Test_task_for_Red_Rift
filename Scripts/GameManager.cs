using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-------------------------------------------------------------------------------------
//
//                              ������� �����, ��������� ����
//
//                              ������� �����, ��������� �������, ���� ������� ��:
//                              - ������������� ����, ���� ����
//                              - ����������� ���� � ����
//
//                              ��������� ����� � ���� � ���������� �� �����
//                              ���������� ���� �������� ����� � ����������� �� �����
//
//-------------------------------------------------------------------------------------

public class GameManager : MonoBehaviour
{
    public delegate void NextCard(int index);
    public delegate int EmptySlot(CardController cntrl);

    [SerializeField] private DropZone dropZone;
    [SerializeField] private GameObject card;               // ������ �����
    [SerializeField] private Transform cardParent;          // Parent ����� ��� ���������� � ��������, ��� ��������,
    private Vector3 cardPos = new Vector3(0, -3.62f, 0);    // �������, ������������� ����� � ����
    private CardController[] hand;                          // ������ ���� � ����
    private int cardIndex = 0;                              // ������, ����������� ����� � ������� ����
    private int maxCard;

    //public static int maxCard{get; private set;}
    public static bool gameStarted { get; private set; }     // false -  ��������� ������ UI


    private void Awake()
    {
        CardController.nextCard += NewCard;
        CardController.rotateCard += RotateCards;
        DragableItem.FindSlotInHand += GetEmptySlotIndex;
        DragableItem.DelCrdInHnd += DeleteCard;
        DropZone.RotateCards += RotateCards;
    }

    private void OnDestroy()
    {
        CardController.nextCard -= NewCard;
        CardController.rotateCard -= RotateCards;
        DragableItem.FindSlotInHand -= GetEmptySlotIndex;
        DragableItem.DelCrdInHnd -= DeleteCard;
        DropZone.RotateCards -= RotateCards;
    }

    private void Start()
    {
        CardDescription.ShuffleNames();                     // ������������ ����� ����
        gameStarted = false;                                // ��������� ������, ��������� �������� ����
        maxCard = Random.Range(4, 7);
        hand = new CardController[maxCard];
        dropZone.Inicializing(maxCard);
        NewCard(0);                                         // ������� �� �������� ����
    }

    public void NewCard(int index)                          // ����� �������� ����� ����
    {
        cardIndex = index;

        if (cardIndex < maxCard)
        {            
            GameObject crd = Instantiate(card, cardPos, new Quaternion(0, 0, 0, 0), cardParent);
            crd.transform.localScale = Vector3.one;
            CardController crdContrl = crd.GetComponent<CardController>();
            crdContrl.CardInicializing(cardIndex);
            hand[cardIndex] = crdContrl;
        }
        else
        {
            cardIndex = 0;
            gameStarted = true;                             // �������� ������, ���� ��� ����� �������
        }
    }    

    public void ChangeCardParamBtn()                        // ����� ���������� ��������� ����� � ������� ������
    {        
        if (gameStarted) 
        {
            for(int i = cardIndex; i < maxCard; i++)        // ���� ������ ����� � ����, ���������� ������ ����� � ����
            {
                if (hand[i] != null)
                {
                    int param = Random.Range(0, 3);
                    int paramValue = Random.Range(-2, 10);

                    hand[i].ChangeParam(paramValue, param);
                    cardIndex = i + 1;
                    if (cardIndex >= maxCard) cardIndex = 0;
                    break;
                }
                else if (i == maxCard - 1) 
                { 
                    cardIndex = 0;
                    i = -1;
                }
            }            
        }        
    }

    private void DeleteCard(int index)                      // ����� �������� ����� �� ������� ���� ��� ����
    {
        hand[index] = null;
    }

    private int GetEmptySlotIndex(CardController cntrl)     // ����� ������ ������� ����� � ������� ���� ��� ����
    {
        int index = 0; 
        for (int i = 0;i < hand.Length;i++) 
        {
            if (hand[i] == null)
            {
                index = i;
                break;
            }
        }

        hand[index] = cntrl;
        return index;
    }

    public static Quaternion GetAngle(int index)            // ����� ����������� ���� �������� ����� 
    {
        Vector3 ang = Vector3.zero;

        switch (index)
        {
            case 0:
                ang.z = 18f;
                break;
            case 1:
                ang.z = 11f;
                break;
            case 2:
                ang.z = 4f;
                break;
            case 3:
                ang.z = -3f;
                break;
            case 4:
                ang.z = -10f;
                break;
            case 5:
                ang.z = -17f;
                break;
        }

        Quaternion angle = Quaternion.Euler(ang);
        return angle;
    }


    private void RotateCards(int index)                     // ����� ���� ������� �� ������������ ���� � ����
    {        
        hand[index] = null;

        for(int i = index; i < hand.Length; i++)            // ���������� ������ ����� � ����
        {
            if(hand[i] == null)
            {
                for (int j = i + 1; j < hand.Length; j++)   // ���� ����� � ����������� � ������ ���� �����
                {
                    if(hand[j] != null)
                    {                        
                        hand[i] = hand[j];
                        hand[i].cardIndex = i;
                        hand[i].StartCoroutine(hand[i].CardRotation(i));
                        hand[j] = null;
                        break;
                    }
                }
            }
        }        
    }

}
