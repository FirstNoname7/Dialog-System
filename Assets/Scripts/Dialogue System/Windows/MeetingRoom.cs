using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using MaryDialogSystem.Utilities;

namespace MaryDialogSystem.Windows
{
    public class MeetingRoom : EditorWindow //�������� �� ���������� ������� ��������� (�������� ���� ������������)
    {
        private readonly string defaultFileName = "��� ����������� �����"; //����������� ��� ������������ (������� ������ ��� ������)
        private TextField fileNameTextField; //�������� ���������� ����
        private Button saveButton; //������ ��� ���������� ���� � �������
        [MenuItem("Dialogue System/Meeting Room")] //���� ������� ������� ���� � ������� ����� �����
        public static void Open() //��� �������� ��� �������� �������
        {
            GetWindow<MeetingRoom>("Meeting Room"); //�������� ������ � ������ Metting Room
        }

        private void OnEnable() //����������� ����� ���, ��� �� ������� �� ������ Meeting Room
        {
            AddGraphView();
            AddToolbar();
            AddStyles();
        }


        #region Add
        private void AddGraphView()
        {
            MyGraphView graphView = new MyGraphView(this); //������ ��������� �������, � ������� ������������ UI ���� ������������
            graphView.StretchToParentSize(); //���������� UI �� ������ ���� ������������ (����� ������� � ������, ����� �������� ������ UI)
            rootVisualElement.Add(graphView); //�������� UI � ������������ (rootVisualElement - �� ������ �� ������� EditorWindow)
        }

        private void AddToolbar() //�������� ������� ������ ������������
        {
            Toolbar toolbar = new Toolbar(); //�����, ���������� �� ������
            fileNameTextField = ElementUtility.CreateTextField(defaultFileName, "��� �����:", callback => 
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters(); //������ �������� ���������� ���� ���������� �� ����� � � ����� ��������� ��� ������� � ����. �������
            }); //� ������� ��������� ���� � ����������� ��������� (�������� ���������� � ���� ������ CreateTextField)
            saveButton = ElementUtility.CreateButton("���������"); //�������������� ������
            toolbar.Add(fileNameTextField); //�������� � ������� ��������� ����
            toolbar.Add(saveButton); //�������� � ������� ������ ���������� 
            toolbar.AddStyleSheets("MyToolbarStyles.uss"); //������������ ����� ��� �������
            rootVisualElement.Add(toolbar); //�������� � ������������ ������ 
        }

        private void AddStyles() //�������� ���� Style Sheet ��� ���� ������
        {
            rootVisualElement.AddStyleSheets("MyStyleSheet.uss");
            //"MyStyleSheet.uss" - �������� ���� ����� ��� ������ � ������� EditorGUIUtility.Load, � ������� ���� �������� ����� ����� (��������� � ����� Editor Default Resources,
            //����� ���������� ������ ���, ������ ��� ��� ��������� ��� ��� ��������, ������� �������� ��� ������-���� ������ ���������)
        }
        #endregion

        #region Utility Methods
        public void EnableSaving()
        {
            saveButton.SetEnabled(true); //�� ��� ��� saveButton.SetActive(true), ������ ���. ������ � ������������, � �� � ����������
        }
        public void DisableSaving() 
        {
            saveButton.SetEnabled(false); //�� ��� ��� saveButton.SetActive(false), ������ ����. ������ � ������������, � �� � ����������
        }

        #endregion
    }
}


