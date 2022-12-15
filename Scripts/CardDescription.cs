using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-------------------------------------------------------------------------------
//                    
//          ����� ���������� �� ����������� ���� � �������� ����,
//          ������������ ��� ����������� ����� � CardController
//
//-------------------------------------------------------------------------------

public static class CardDescription
{
    private static Dictionary<string, string> cardDescription = new Dictionary<string, string>() 
    {
        { "�������� �����",     "�������� ��� - ������� ����� ���������� �� 2."},
        { "������ ���",         "������ - ��������� ����� �������� ��������, ������� 3 ��. �����."},
        { "�������� �����",     "������� ��� - �������� �������� ��������� �� 2."},
        { "�������� ����",      "���������� - ������� ���������� �������� ���� ������� � 4 ��. �����." },
        { "�������� �����",     "�������� ������ - ����������� ����� �� 3 ��. � ������� 2 �����." },
        { "�������",            "����������� ����� - ��� ���������� �������� 2 ��. �����." }
    };

    private static string[] cardName = new string[]
    {
        "�������� �����",
        "������ ���",
        "�������� �����",
        "�������� ����",
        "�������� �����",
        "�������"
    };

    public static void ShuffleNames()                       // ����� ������������ �����
    {                                                       // 
        for (int i = 0; i < cardName.Length; i++)           // ������������ ��� ���������� 
        {                                                   // ��������� ���� ������
            int r = Random.Range(i,cardName.Length);
            string n = cardName[r];

            cardName[r] = cardName[i];
            cardName[i] = n;
        }
    }

    public static string GetName(int index)   // - - - - - - ����� ������������ ��� �����
    {
        return cardName[index];
    }

    public static string GetDescription(string name) // - -  ����� ���������� �������� ����� 
    {
        return cardDescription.ContainsKey(name) ? cardDescription[name] : null;
    }
}
