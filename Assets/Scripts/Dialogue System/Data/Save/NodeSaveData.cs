using MaryDialogSystem.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.Data.Save
{
    [Serializable] //��������� ������ �� ������ ����
    public class NodeSaveData
    {
        //����� ������ ���������:
        [SerializeField] private string ID { get; set; } //����� ������ ���� ���������
        [SerializeField] private string Name { get; set; } //��� ����
        [SerializeField] private string Text { get; set; } //����� � ����
        [SerializeField] private List<ChoiceSaveData> Choices { get; set; } //������ � ����
        [SerializeField] private DialogueType DialogueType { get; set; } //��� ���� (���������� ���� ��� ������ ������)
        [SerializeField] private Vector2 Position { get; set; } //������� ����
    }

}
