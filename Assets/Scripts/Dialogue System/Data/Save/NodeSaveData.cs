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
        public string ID { get; set; } //����� ������ ���� ���������
        public string Name { get; set; } //��� ����
        public string Text { get; set; } //����� � ����
        public List<ChoiceSaveData> Choices { get; set; } //������ � ����
        public DialogueType DialogueType { get; set; } //��� ���� (���������� ���� ��� ������ ������)
        public Vector2 Position { get; set; } //������� ����
    }

}
