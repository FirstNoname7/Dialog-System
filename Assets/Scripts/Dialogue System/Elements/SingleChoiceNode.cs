using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaryDialogSystem.Enumerations;
using UnityEditor.Experimental.GraphView;
using MaryDialogSystem.Utilities;
using MaryDialogSystem.Windows;
using MaryDialogSystem.Data.Save;

namespace MaryDialogSystem.Elements
{
    public class SingleChoiceNode : MyNode //���� ��� �������
    {
        public override void Initialize(MyGraphView currentGraphView, Vector2 position)
        {
            base.Initialize(currentGraphView, position);
            dialogueType = DialogueType.SingleChoice; //�������� ��� ������������ ����� � ���������� dialogueType
            ChoiceSaveData choiceData = new ChoiceSaveData()
            {
                Text = "��������� �����"
            }; //����������� ������, ��� ����� � ���� ������� �����
            choices.Add(choiceData); //�������� � ���������� ��� ������ �����
        }
        public override void Draw()
        {
            base.Draw();
            //�������� �������� ���� (output)
            foreach (ChoiceSaveData choice in choices) //��� ������ ������ � ����� ������ ������ ������ ���� �� �����
            {
                Port choicePort = ElementUtility.CreatePort(this, choice.Text); //������ ������ choice, ������ ��� ��������� ����������� ��������� ��� ��� ��� output �������
                choicePort.userData = choice; //���������� ������ ������ ������ � userData
                outputContainer.Add(choicePort);
            }
            RefreshExpandedState(); //��� ����, ����� ����� �� ������ ����, �� ������ ��������� ��� ������� ������� (� ���� ������ ���� ������ ������� choice)

        }
    }
}
