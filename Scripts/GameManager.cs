using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-------------------------------------------------------------------------------------
//
//                              Главный класс, запускает игру
//
//                              Создает карты, управляет кнопкой, дает команду на:
//                              - инициализацию карт, дроп зоны
//                              - перемещение карт в руке
//
//                              Добавляет карты в руку и определяет их место
//                              Определяет угол поворота карты в зависимости от места
//
//-------------------------------------------------------------------------------------

public class GameManager : MonoBehaviour
{
    public delegate void NextCard(int index);
    public delegate int EmptySlot(CardController cntrl);

    [SerializeField] private DropZone dropZone;
    [SerializeField] private GameObject card;               // Префаб карты
    [SerializeField] private Transform cardParent;          // Parent карты для сортировки в иерархии, при создании,
    private Vector3 cardPos = new Vector3(0, -3.62f, 0);    // Позиция, выравнивающая карты в руке
    private CardController[] hand;                          // массив карт в руке
    private int cardIndex = 0;                              // Индекс, указывающий место в массиве карт
    private int maxCard;

    //public static int maxCard{get; private set;}
    public static bool gameStarted { get; private set; }     // false -  отключает работу UI


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
        CardDescription.ShuffleNames();                     // Перемешивает имена карт
        gameStarted = false;                                // Отключает кнопку, разрешает создание карт
        maxCard = Random.Range(4, 7);
        hand = new CardController[maxCard];
        dropZone.Inicializing(maxCard);
        NewCard(0);                                         // Команда на создание карт
    }

    public void NewCard(int index)                          // Метод создания новых карт
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
            gameStarted = true;                             // Включает кнопку, если все карты созданы
        }
    }    

    public void ChangeCardParamBtn()                        // Метод изменяющий параметры карты с помощью кнопки
    {        
        if (gameStarted) 
        {
            for(int i = cardIndex; i < maxCard; i++)        // Цикл поиска карты в руке, пропускает пустые места в руке
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

    private void DeleteCard(int index)                      // Метод удаления карты из массива карт для руки
    {
        hand[index] = null;
    }

    private int GetEmptySlotIndex(CardController cntrl)     // Метод поиска пустого места в массиве карт для руки
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

    public static Quaternion GetAngle(int index)            // Метод определения угла поворота карты 
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


    private void RotateCards(int index)                     // Метод дает команду на передвижение карт в руке
    {        
        hand[index] = null;

        for(int i = index; i < hand.Length; i++)            // Определяет пустое место в руке
        {
            if(hand[i] == null)
            {
                for (int j = i + 1; j < hand.Length; j++)   // Ищет карту и передвигает в пустой слот слева
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
