using MaryDialogSystem.Data.Save;
using MaryDialogSystem.Enumerations;
using MaryDialogSystem.Utilities;
using MaryDialogSystem.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MaryDialogSystem.Elements
{
    public class MultipleChoiceNode : MyNode //���� ��� ������ ������
    {
        ChoiceSaveData choiceData = new ChoiceSaveData()
        {
            Text = "����� ������"
        }; //����������� ������, ��� ����� � ���� ������� �����

        public override void Initialize(MyGraphView currentGraphView, Vector2 position)
        {
            base.Initialize(currentGraphView, position);
            dialogueType = DialogueType.MultipleChoice; //�������� ��� ������������ ����� � ���������� dialogueType

            choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            Button addChoiceButton = ElementUtility.CreateButton("�������� �����", ()=>
            {
                Port choicePort = CreateChoicePort(choiceData); //��������� ���� � ����� ������
                choices.Add(choiceData); //��������� ����� ������

                outputContainer.Add(choicePort);

            }); //������ ������ ��� ���������� ������ ������ + �������� foreach, ���� ����� ���� ����� ����� �������� ������ �������
                //(�� � �������� ������ ��������� ������ ����� outputContainer.Add(choicePort))

            addChoiceButton.AddToClassList("mary-node__button"); //�������� ����� �������
            mainContainer.Insert(1, addChoiceButton); //�������� � ���� ������ ��� ���������� ������ ������

            //�������� �������� ���� (output) � ������ "�������"
            foreach (ChoiceSaveData choice in choices) //��� ������ ������ � ����� ������ ������ ������ ���� �� �����
            {
                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Add(choicePort);
            }
            RefreshExpandedState(); //��� ����, ����� ����� �� ������ ����, �� ������ ��������� ��� ������� ������� (� ���� ������ ���� ������ ������� choice)

        }
        #region Elements Creation
        private Port CreateChoicePort(object userData) //userData = ����������������� ����������, ������� ����� ������� ����� ������
        {
            Port choicePort = ElementUtility.CreatePort(this); //�������� ������ �� ���������� ����, �.�. ��������� ����������� ��������� ������������� ����, ��� ������ ���� �����
            choicePort.userData = userData; //������ � ����� ������ ������ ���� �� ���������
            ChoiceSaveData choiceData = (ChoiceSaveData) userData; //������������ ������ �� ��������� userData � ������ ��� ������� ������
            Button deleteChoiceButton = ElementUtility.CreateButton("�������", ()=> 
            {
                if (choices.Count == 1) //���� ������ ������� 1 ������ ������ � ���� ��� ������ ������, ��:
                {
                    return; //�� ���� ������ �� ������ ����, �.�. �� �� ����� ��� ������ ������ ������� ��� ����
                }
                if (choicePort.connected) //���� � ����� ���� ����������� ��������, ��:
                {
                    myGraphView.DeleteElements(choicePort.connections); //������� ��
                }
                choices.Remove(choiceData); //������� ������� ������ � ������ � ���
                myGraphView.RemoveElement(choicePort); //������� ����
            }); //������ ��� �������� ������ ������
            deleteChoiceButton.AddToClassList("mary-node__button"); //�������� ����� ������ "�������"

            TextField choiceTextField = ElementUtility.CreateTextField(choiceData.Text, null, callback=> 
            {
                choiceData.Text = callback.newValue;
            }); //��� ������ ������ �� ����� � ���������� ������� (choiceData.Text = callback.newValue - ����� ����� ���������� ������ ���, ����� �� ������ ��� � ������ ������)
            choiceTextField.AddClasses("mary-node__textfield", "mary-node__choice-textfield", "mary-node__textfield__hidden"); //�������� ����� ���������� ���� ������


            choicePort.Add(deleteChoiceButton); //������ ������ �������� ������ �� �����
            choicePort.Add(choiceTextField); //������ ���� ��� �������� ������ ������
            return choicePort;
        }
        #endregion
    }
}
