using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//----------------------------------------------------------------------
//
//                  ����� ���������� �� ��������� ������
//                  ������ ��������� ��������, ��������� ��������
//
//----------------------------------------------------------------------

public class CounterText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;   // ����� ��������
    [SerializeField] private Animator anim;                 // ��������� ���������

    public void SetText(int value, string param)            // �������� �������� � ��������� ���������
    {
        string sign = "";
        if (value > 0) sign = "+";                          // ���������� + ����� ������������� ���������
        counterText.text = sign + value;
        anim.Play(param);                                   // ��������� �������� �������� ��� ����������� ���������
    }

    public void Destroy()                                   // ���������� ������� � ����� �������� (����������� � ��������)
    {
        Destroy(this.transform.parent.gameObject);
    }

}
