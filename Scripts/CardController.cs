using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

//-----------------------------------------------------------------------------------------------
//
//                                          Класс отвечающий за параметры карты, ее движение
//                                          Создает текстовый объект - счетчик
//
//-----------------------------------------------------------------------------------------------


public class CardController : MonoBehaviour
{
    public static event GameManager.NextCard nextCard;                      // Создает новую карту, подписан в GameManager
    public static event GameManager.NextCard rotateCard;                    // Передвигает карту в руке, подписан в GameManager
    public static event GameManager.NextCard setRaycast;                    // Отключает raycast арта у карты, подписан здесь


    [SerializeField] private Image image;                                   // Загружается из интернета
    [SerializeField] private Transform _cardRotation;                       // Поворачивает карту
    [SerializeField] private TextMeshProUGUI mana_txt;
    [SerializeField] private TextMeshProUGUI attack_txt;
    [SerializeField] private TextMeshProUGUI health_txt;
    [SerializeField] private TextMeshProUGUI cardName_txt;
    [SerializeField] private TextMeshProUGUI description_txt;
    [SerializeField] private GameObject counter_txt;                        // Префаб счетчика
    [SerializeField] private Image _fireOutline;                            // Подсветка карты
    [SerializeField] private Image _backGround;                             // Арт карты
    [SerializeField] private float rotSpeed;                                // Скорость поворота карты

    private Animator anim;
    private DragableItem thisDragItem;
    private int mana;
    private int attack;
    private int health;

    public int cardIndex { get { return thisDragItem.parentIndex; } set { thisDragItem.parentIndex = value; } }
    public Transform cardRotation{ get { return _cardRotation; } }
    public Image fireOutline { get {return _fireOutline; } }
    public Image backGround { get {return _backGround; } }

    private void Awake()
    {
        CardController.setRaycast += SetRaycast;
    }

    private void OnDestroy()
    {
        CardController.setRaycast -= SetRaycast;
    }
    public void CardInicializing(int index)         // Определяет параметры карты
    {
        anim = GetComponent<Animator>();
        thisDragItem = GetComponent<DragableItem>();
        //thisDragItem.cardCntrl = this;
        thisDragItem.parentIndex = index;
        StartCoroutine(LoadImage(index));
        mana = Random.Range(1, 10);
        attack = Random.Range(1, 10);
        health = Random.Range(1, 10);

        
        cardName_txt.text = CardDescription.GetName(index);
        description_txt.text = CardDescription.GetDescription(cardName_txt.text);
        mana_txt.text = "" + mana;
        attack_txt.text = "" + attack;
        health_txt.text = "" + health;
    }    

    private void SetRaycast(int value)             // Включает и отключает raycast арта карты
    {
        this._backGround.raycastTarget = (value != 0) ? true : false;
    }

    public IEnumerator CardRotation(int index)     // Передвигает карту в руке
    {
        Quaternion angle = GameManager.GetAngle(index);
        if (setRaycast != null) setRaycast(0);

        while (_cardRotation.rotation.z != angle.z)
        {
            _cardRotation.rotation = Quaternion.RotateTowards(_cardRotation.rotation, angle, rotSpeed * Time.deltaTime);
            yield return null;
        }

        if (setRaycast != null) setRaycast(1);

        if (!GameManager.gameStarted && nextCard != null)
        {
            index++;
            nextCard(index);
        } 
    }
    
    private IEnumerator LoadImage(int index)      // Назначает изображение из интернета
    {
        _cardRotation.gameObject.SetActive(false);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://picsum.photos/200/300");
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            image.sprite = sprite;
        }

        _cardRotation.gameObject.SetActive(true);
        StartCoroutine(CardRotation(index));
    }

    public void ChangeParam(int value, int param)   // Изменяет параметры карты                            
    {
        GameObject g;

        switch (param)                              // Выбирает параметр карты
        {
            case 0:
                mana += value;
                if (mana < 0) mana = 0;
                mana_txt.text = "" + mana;                
                g = Instantiate(counter_txt, mana_txt.transform.position, Quaternion.Euler(0, 0, 0), transform.parent.transform.parent);
                g.GetComponentInChildren<CounterText>().SetText(value,"ManaAnim");
                break;
            case 1:
                attack += value;
                if (attack < 0) attack = 0;
                attack_txt.text = "" + attack;
                g = Instantiate(counter_txt, attack_txt.transform.position, Quaternion.Euler(0, 0, 0), transform.parent.transform.parent);
                g.GetComponentInChildren<CounterText>().SetText(value, "AttackAnim");
                break;
            case 2:
                health += value;
                if (health < 1)       // Уничтожает карту, если у нее закончилось здоровье
                {
                    health = 0;
                    if (rotateCard != null) rotateCard(thisDragItem.parentIndex);
                    anim.enabled = true;
                    anim.Play("Death");
                }
                health_txt.text = "" + health;
                g = Instantiate(counter_txt, health_txt.transform.position, Quaternion.Euler(0, 0, 0), transform.parent.transform.parent);
                g.GetComponentInChildren<CounterText>().SetText(value, "HealthAnim");
                break;
        }
    }

    public void DestroyCard()         // Метод уничтожения карты
    {
        Destroy(this.gameObject);
    }
}
