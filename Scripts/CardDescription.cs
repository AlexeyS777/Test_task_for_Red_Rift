using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-------------------------------------------------------------------------------
//                    
//          Класс отвечающий за определение имен и описаний карт,
//          используется при определении карты в CardController
//
//-------------------------------------------------------------------------------

public static class CardDescription
{
    private static Dictionary<string, string> cardDescription = new Dictionary<string, string>() 
    {
        { "Командир орков",     "Яростный рев - снижает атаку противника на 2."},
        { "Лесная фея",         "Тернии - опутывает врага колючими терниями, наносит 3 ед. урона."},
        { "Каменный голем",     "Величие гор - повышает здоровье союзников на 2."},
        { "Кровавый клык",      "Потрошение - наносит противнику кровавые раны когтями и 4 ед. урона." },
        { "Огненный троль",     "Огненная хватка - увеличивает атаку на 3 ед. в течении 2 ходов." },
        { "Асгарот",            "Метеоритный дождь - все противники получают 2 ед. урона." }
    };

    private static string[] cardName = new string[]
    {
        "Командир орков",
        "Лесная фея",
        "Каменный голем",
        "Кровавый клык",
        "Огненный троль",
        "Асгарот"
    };

    public static void ShuffleNames()                       // Метод перемешивает имена
    {                                                       // 
        for (int i = 0; i < cardName.Length; i++)           // Используется для назначения 
        {                                                   // случайных имен картам
            int r = Random.Range(i,cardName.Length);
            string n = cardName[r];

            cardName[r] = cardName[i];
            cardName[i] = n;
        }
    }

    public static string GetName(int index)   // - - - - - - Метод возвращающий имя карты
    {
        return cardName[index];
    }

    public static string GetDescription(string name) // - -  Метод возвращает описание карты 
    {
        return cardDescription.ContainsKey(name) ? cardDescription[name] : null;
    }
}
