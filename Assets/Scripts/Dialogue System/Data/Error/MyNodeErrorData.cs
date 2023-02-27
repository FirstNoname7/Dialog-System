using MaryDialogSystem.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.Data.Error
{
    public class MyNodeErrorData //��� ������ ������ �� ������� � �����
    {
        public MyErrorData ErrorData { get; set; } //������ �� ������� (� ���� ����� ���)
        public List<MyNode> Nodes { get; set; } //������� ����
        public MyNodeErrorData() //� ������������ �������������� ����������
        {
            ErrorData = new MyErrorData();
            Nodes = new List<MyNode>();
        }
    }
}

