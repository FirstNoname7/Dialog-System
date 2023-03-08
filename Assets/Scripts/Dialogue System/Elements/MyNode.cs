using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using MaryDialogSystem.Enumerations;
using UnityEngine.UIElements;
using MaryDialogSystem.Utilities;
using MaryDialogSystem.Windows;
using System;
using MaryDialogSystem.Data.Save;
using System.Linq;

namespace MaryDialogSystem.Elements
{
    public class MyNode : Node //�������� �� �������� ��� � ������
    {
        //{ get; set; } � ���������� ��� ����, ����� ����� ���� ����������, ��� ��������� �� ��� ���������� (���� �������, �� ��� ����������� �������� ������� "������:���-��")
        public string ID { get; set; } //������������� ���� (����� ��� ����������)
        public string dialogueName { get; set; } //��� ����, ��� �������
        public List<ChoiceSaveData> choices { get; set; } //������ � ���������� ��� ������
        public string text { get; set; } //����� � ���������� ����
        public DialogueType dialogueType { get; set; } //��� ������� (������ ����� (SingleChoice) ��� ������ ������ (MultipleChoice))

        protected MyGraphView myGraphView; //��� �������� �������� ������ � ���� ���������� ����� ����������� protected
        private Color defaultBackgroundColor; //����������� ���� ����
        public virtual void Initialize(MyGraphView currentGraphView, Vector2 position) //������ ��������� �������� ����������
        {
            ID = Guid.NewGuid().ToString(); //�������������� ������� ������������� ���� (������� Guid.NewGuid() ������� ID ������������� �� ���) � ����������� ��� � �������, ���� ����� ������������ (ToString())
            myGraphView = currentGraphView; //�������������� ���� ������������
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f); //������ ��������� ���� ����
            dialogueName = "�";
            choices = new List<ChoiceSaveData>();
            text = "���� ����� ����";
            SetPosition(new Rect(position,Vector2.zero)); //����������� ������� ����, ������� ������, ���� ��� �� ����������� �� ����� � ��� �� �����
            extensionContainer.AddToClassList("mary-node__extension-container"); //�������� ����� mary-node__extension-container �� Style Sheet
            mainContainer.AddToClassList("mary-node__main-container");
        }

        public virtual void Draw() //������ UI ��� ����������, ������� ���� � ���� �������
        {
            //��������� ����
            TextField dialogueNameTextField = ElementUtility.CreateTextField(dialogueName, null, callback => 
            {
                TextField target = (TextField)callback.target; //���������� ��������� ����, ����� � �� ����� ���� ��������� �������
                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters(); //������� ������� � ����. ������� � ������� ������� �� ������� TextUtility

                if (string.IsNullOrEmpty(target.value)) //���� ������ ��������� ����, ��:
                {
                    if (!string.IsNullOrEmpty(dialogueName)) //���� ���� ���-�� � dialogueName (������ ��� ����), ��:
                    {
                        ++myGraphView.RepeatedNamesAmount; //��������� �� RepeatedNamesAmount, ��� ��������� ������. ������ ���������, �.�. dialogueName = ������ ��� ����, � ��� ���� ������� �� ������ � ������� ����, ������� � ������ ��������� �����
                    }
                }
                else //� �������� ������ (���� ��������� ���� �� �����)
                {
                    --myGraphView.RepeatedNamesAmount; //��� � ������ ����
                }

                myGraphView.RemoveNode(this); //������ ������ ���� (�� ������ ���������)
                dialogueName = target.value; //������������ ����� �������� �������� ���� (� ��������� ������ ��������)
                myGraphView.AddNode(this); //�������� ����  � ����� ���������
            
            }); //������� ���������� dialogueName � UI. callback ������� ����� ������� ���� � �� �������� � �������, ���������� ��� ����� �� ������ ��� ����

            dialogueNameTextField.AddClasses("mary-node__textfield", "mary-node__textfield", "mary-node__textfield__hidden"); //�������� ����� ���������� ����

            titleContainer.Insert(0, dialogueNameTextField); //������ ���������� dialogueName ��� ��������� ����

            //������� �������� ���� (input)
            Port inputPort = ElementUtility.CreatePort(this, "��������� ���� ���������� �����", Orientation.Vertical, Direction.Input, Port.Capacity.Multi);
            //���� ������������ ��� ��������� (������), �������� � ����:  Orientation.Horizontal - ������� ����� ���� �������������, Direction.Input - ������ ����� ��������� �������� ��� ������� ��������,
            //Port.Capacity.Multi - �� ����� ���� ����� ������������ ��������� ���������, typeof(bool) - ��� ����
            inputContainer.Add(inputPort); //��������� �� ���� ���� ���������, ������� ������� � ���������� inputPort

            //���������� ���� (������ ��������� ������)
            Foldout textFlodout = ElementUtility.CreateFoldout("�����"); //��������� ��� ����������� ���� (�� ����� �� ������� ��������� �����)

            TextField textTextField = ElementUtility.CreateTextArea(text, null, callback=> 
            {
                text = callback.newValue;
            }); //������, ��� ����� ������ ����� ������� + �������� �����, ���� ��������� ���������� �����, ���� �� ���������
            textTextField.AddClasses("mary-node__textfield", "mary-node__quote-textfield"); //�������� ����� ���������� ����, ��� ���� ����� ������

            textFlodout.Add(textTextField); //� ��������� ��������� ���� ��� ����� ���� ������� 

            VisualElement customDataContainer = new VisualElement(); //������� ���������� ������� � UI ������������ (���������� ��� ����)
            customDataContainer.AddToClassList("mary-node__custom-data-container"); //����� ��� ���� ����� � ������
            customDataContainer.Add(textFlodout); //��������� ���� ����������
            extensionContainer.Add(customDataContainer); //��������� ����������� ��������� (��������� ���� "�����")

        }

        #region Utility Methods
        public void DisconnectAllPorts()
        {
            DisconnectPorts(inputContainer); //inputContainer - ����������� ���������, ���������� �� ������� �������� � ���� � ������ �����
            DisconnectPorts(outputContainer); //inputContainer - ����������� ���������, ���������� �� �������� �������� � ���� � ������ �����
        }
        private void DisconnectPorts(VisualElement container) //��� �������� �������/�������� ���������
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected) //���� ����� (��������) ���������� (connected - ��� ����������� ���������� �� ������ �����), ��
                {
                    continue; //���� ������
                }
                myGraphView.DeleteElements(port.connections); //������� ������������ ����� (��������) (connections - ��� ����������� ���������� �� ������ �����)
            }
        }

        public bool IsStartingNode() //���������, ������� ���� ������ ��� ���
        {
            Port inputPort = (Port)inputContainer.Children().First(); //������� ���� = �� ���������� ���� ������ �������� ������ (� ������������ � Port, �.�. �� ��������� ����� ��������� ���������� IEnumerable)
            return !inputPort.connected; //���������� true - ���� ���� ������ ������� ��������, false - ���� ������� �������� �� ����. ������ ��� ���������, �.�. ���� ��� �������� (connected), �� ���� ����
        }
        public void SetErrorStyle(Color color) //��������� ���� ��� � �������
        {
            mainContainer.style.backgroundColor = color; //����� ���� ������� � ��������, ����� �� � ����� � ����
        }

        public void ResetStyle() //����������� ����� ���
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        #endregion
    }
}
