using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using MaryDialogSystem.Enumerations;
using MaryDialogSystem.Elements;

namespace MaryDialogSystem.Windows
{
    public class MySearchWindow : ScriptableObject, ISearchWindowProvider //�������� �� ������ ������
    {
        private MyGraphView graphView; //������������
        public void Initialize(MyGraphView myGraphView) //������������� ������������, ����� ���� ��� ���������� ���� ������
        {
            graphView = myGraphView; //�������� ������� ������������ �� ���������� myGraphView � ���������� ����������, ���� �� ����� ���� ������������ ��� � � ������ OnSelectEntry
        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) //�������� ������� ��� ������ (�� ���������� ISearchWindowProvider)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("������� �������")), //1 ���� ������ (������������ ���� ������)
                new SearchTreeGroupEntry(new GUIContent("���������� ����"), 1), //2 ���� ������ (1 �������� 1 �������, �.�. ��� ��� �������� ��������� ��������� ���� ������)
                new SearchTreeEntry(new GUIContent("���������� ����"))
                {
                    level = 2,
                    userData = DialogueType.SingleChoice //������������� ���������� userData �� ������� ��� �������
                },
                new SearchTreeEntry(new GUIContent("������ ������"))
                {
                    level = 2,
                    userData = DialogueType.MultipleChoice //������������� ���������� userData �� ������� ��� �������
                },
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context) //����� �������/ ������� �� ����� �������, ������� ����� ����� ���� ������ (�� ���������� ISearchWindowProvider)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true); //����������� ���������� ���������� � ��������� � ������� ������ GetLocalMousePosition + ������, ��� ���� ������� ������� �� ���� ������
            switch (SearchTreeEntry.userData) //��������� ���������� userData, � ������� ������ ������� ��� ���� (������ ��� ������ ������)
            {
                case DialogueType.SingleChoice:
                {
                    SingleChoiceNode singleChoiceNode = (SingleChoiceNode)graphView.CreateNode(DialogueType.SingleChoice, localMousePosition); //������ ����, �� ������� �������� � ����������� ����
                    graphView.AddElement(singleChoiceNode); //� graphView (������������) �������� ����, �� ������� �������� � ����������� ����
                    return true; //��������� true, ����� ������ ���� ���������� ������ �� ����� �� ������, � �� ����� ��� ������ � �������� �� ��� ���� � ����������� ����
                }
                case DialogueType.MultipleChoice:
                {
                    MultipleChoiceNode multipleChoiceNode = (MultipleChoiceNode)graphView.CreateNode(DialogueType.MultipleChoice, localMousePosition); //������ ����, �� ������� �������� � ����������� ����
                    graphView.AddElement(multipleChoiceNode); //� graphView (������������) �������� ����, �� ������� �������� � ����������� ����

                    return true; //��������� true, ����� ������ ���� ���������� ������ �� ����� �� ������, � �� ����� ��� ������ � �������� �� ��� ���� � ����������� ����
                }
                default: //�� ���������
                {
                    return false; //��������� false
                }
            }
        }
    }
}

