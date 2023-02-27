using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.Data.Save
{
    [Serializable]
    public class ChoiceSaveData //���������� ��� ������ ������
    {
        [SerializeField] public string Text { get; set; } //��� �� ����� � ���� ����
        [SerializeField] public string NodeID { get; set; } //����� ������ ���� ���������
    }
}
