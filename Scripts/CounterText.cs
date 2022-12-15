using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//----------------------------------------------------------------------
//
//                  Класс отвечающий за текстовый сетчик
//                  Меняет параметры счетчика, запускает анимацию
//
//----------------------------------------------------------------------

public class CounterText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;   // Текст счетчика
    [SerializeField] private Animator anim;                 // Компонент аниматора

    public void SetText(int value, string param)            // Выбирает анимацию и назначает параметры
    {
        string sign = "";
        if (value > 0) sign = "+";                          // Показывает + перед положительным значением
        counterText.text = sign + value;
        anim.Play(param);                                   // Запускает анимацию счетчика для конкретного параметра
    }

    public void Destroy()                                   // Уничтожает счетчик в конце анимации (запускается в анимации)
    {
        Destroy(this.transform.parent.gameObject);
    }

}
