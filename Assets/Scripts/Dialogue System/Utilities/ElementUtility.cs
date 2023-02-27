using MaryDialogSystem.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MaryDialogSystem.Utilities
{
    public static class ElementUtility
    {
        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null) //value = ��� ����� ���������� ��������� ����,
                                                                                                                               //onValueChanged = �������� ��� ��������� �������
                                                                                                                               //��� ������� ���� ������ ������
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            }; //������ ����������-��������, � ��������� ���� ������� ������ �������� �� ���������� value � label
            if (onValueChanged != null) //���� �������� ���-�� �������� � ��������� ����, ��:
            {
                textField.RegisterValueChangedCallback(onValueChanged); //������������ ��� ��������� 
            }
            return textField; //���������� ��������� ����
        }

        public static TextField CreateTextArea(string value = null, string label = null,  EventCallback<ChangeEvent<string>> onValueChanged = null) //��� ������� ������������ ��� ������ ������
                                                                                                                              //(������ ���, ���� ����� ���� ��������� ������ ������ �������)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);
            textArea.multiline = true; //����� ����� ���� �������� ��������� ����� (��� ���������� ������ ������)
            return textArea;
        }

        public static Foldout CreateFoldout(string title, bool collapsed = false) //��� �������� ���������� ������� (� ��� ����� �������� ��� �������� � ����) 
                                                                          //title = ���������, collapse = ��������, �������� �� ������ ��� ��������
        {
            Foldout foldOut = new Foldout()
            {
                text = title,
                value = !collapsed
            }; //���������� � ��������-����������� �������� ��������� � ���������, ������� �� ��� ��������.
               //���� ������� - ����� ���������, ���� ������� - ����� ���������
            return foldOut;
        }

        public static Button CreateButton(string name, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = name
            };
            return button;
        }

        public static Port CreatePort(this MyNode node, string portName = "", Orientation orientation = Orientation.Vertical, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
            //������� ��� input, ��� � output �����, �.�. � ��������� ����� ����� �������� ��������. this � ������ ��������� ��������, ��� ���� ����� ������� ����
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool)); //������� ����
            port.portName = portName; //���� ��� �����
            return port; //���������� ��������� ����
        }
    }
}

