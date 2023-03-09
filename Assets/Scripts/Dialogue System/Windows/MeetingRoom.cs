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
        private MyGraphView graphView; //������ �� ��� ����
        private readonly string defaultFileName = "��� ����������� �����"; //����������� ��� ������������ (������� ������ ��� ������)
        private static TextField fileNameTextField; //�������� ���������� ���� (���������, �.�. ��� ������ ����������� �� ����������, ��� �������)
        private Button saveButton; //������ ��� ���������� ���� � �������
        private Button clearButton;
        private Button resetButton;
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
            graphView = new MyGraphView(this); //������ ��������� �������, � ������� ������������ UI ���� ������������
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
            saveButton = ElementUtility.CreateButton("���������", ()=>Save()); //�������������� ������, � �������� ����� (()=>Save()) ������ ������ �� ����� Save, ������� ��������� ������
            clearButton = ElementUtility.CreateButton("��������", ()=>Clear()); //������ ������ ��� ������� ������������, �������� ����� = ������� �� ����� Clear (�.�. ����� ������� �� ������, �� ��������� �� ���� �����)
            resetButton = ElementUtility.CreateButton("�������������", ()=>Reset()); //������ ������ ��� ������������ ������������
            toolbar.Add(fileNameTextField); //�������� � ������� ��������� ����
            toolbar.Add(saveButton); //�������� � ������� ������ ���������� 
            toolbar.Add(clearButton); //�������� � ������� ������ �������� ����������� ������������ 
            toolbar.Add(resetButton); //�������� � ������� ������ ���������� ����������� ������������ �� ��������� ��������
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
        #region Toolbar Actions
        private void Save() //���������� ����������� ������������
        {
            if (string.IsNullOrEmpty(fileNameTextField.value)) //���� ��� �������� ����, ��� ����� ���������, ��:
            {
                EditorUtility.DisplayDialog(
                    "�������� ��� �����",
                    "���������, ��� ��������� ���� ��� ����� �������� ����������",
                    "OK"
                ); //������� ����� �� ������ (DisplayDialog - ����������������� �������)
                return; //������������, �.�. ��� �� ��� ��������� ������ ����
            }

            InputOutputUtilities.Initialize(graphView, fileNameTextField.value); //�������������� ��� ������ ��������� (������ � ������������ (graphView) � �������� ������������ (fileNameTextField.value)
            InputOutputUtilities.Save(); //�������� �� ���������� �� ������� �������
        }
        private void Clear()
        {
            graphView.ClearGraph();
        }
        private void Reset()
        {
            Clear(); //�������� ��
            UpdateFileName(defaultFileName); //������ ��������� ��������
        }

        #endregion
        #region Utility Methods
        public static void UpdateFileName(string newFileName) //����� �������� � ��������, �.�. ������������ ��� �������� ������
        {
            fileNameTextField.value = newFileName; //���������� ���������� ����
        }
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


