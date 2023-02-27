using MaryDialogSystem.Elements;
using MaryDialogSystem.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MaryDialogSystem.Utilities
{
    public static class InputOutputUtilities //���������� � �������� ������������ (���������, �.�. ����� ��� ������������)
    {
        //������ ���������� ����������, ����� ���������� �� ��� ��������� 
        private static MyGraphView myGraphView; //������ �� ��� ������ ������������ 
        private static string graphFileName; //��� ������������
        private static string containerFolderPath;  //���� � ����������� (��� �������� ������� �������� ��� ������������) 
        private static List<MyNode> nodes; //����
        public static void Initialize(MyGraphView graphView, string graphName) //��� �������������� ������� �����
        {
            myGraphView = graphView; //������� ������������
            graphFileName = graphName; //��� ������������
            containerFolderPath = $"Assets/MeetingRoom/Dialogues/{graphFileName}"; //���� � ����� ������� ������������
            nodes = new List<MyNode>(); //�������� ������ ��� ������ � ������
        }
        #region Save Methods
        public static void Save() //����������
        {
            CreateStaticFolders();
            GetElementsFromGraphView();
            //CreateAsset();
        }
        #endregion

        #region Fetch Methods
        private static void GetElementsFromGraphView()
        {
            myGraphView.graphElements.ForEach(graphElement =>
            {
                if(graphElement is MyNode node) //���� � ���� ������������ ��������� ����� ����, ��:
                {
                    nodes.Add(node); //�������� �� � ������ ��� ������������ ���
                    return; //������������
                }
            }); //���������� �� ���� ��������� ������� ������������ ()
        }
        #endregion

        #region Create Methods
        private static void CreateStaticFolders() //��������� ����� = ������, ������� �� �������� � ������������ (�����, ��� �������� ������ ������������)
        {
            CreateFolder("Assets/Editor/MeetingRoom", "Graphs"); //����� ���������, �� ���������� � RunTime (������� ����)
            
            CreateFolder("Assets", "MeetingRoom"); //����� �� ��� ���������, ���������� � RunTime (������� ����), ��� ����� ���� ���������� �������
            CreateFolder("Assets/MeetingRoom", "Dialogues"); //����� ��� ��������� ��������
            CreateFolder("Assets/MeetingRoom/Dialogues", graphFileName); //����� ��� ������� ������������

            CreateFolder(containerFolderPath, "Global"); //����� ����� ��� ������� ������������, ��� ����� �������� ���� � ���� ������������ � ������ ������ ���
            CreateFolder($"{containerFolderPath}/Global", "Dialogues"); //����� ��� �������� ������� ������������
        }
        #endregion

        #region Utility Methods
        private static void CreateFolder(string path, string folderName) //path = ���� � ����������� �����, folderName = ��� ����������� �����
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}")) //���� � ���� Project ���� ����� Graphs �� ���������� ���� (���� $ ����� �������, ����� � ��� ������������ ���������� � �������� ������)
            {
                return; //����� ������ ������������, �� ����� �� �� ��������� ��, ��� ��� ����������
            }
            AssetDatabase.CreateFolder(path, folderName); //���� ����� ���, �� ������ �
        }
        private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject //������� � = � ���� ����� ��������� ����� ��� ������ (���������� �� ����� Type), �� ������ ScriptableObject, ������� ����� ��������� � ���� ������ CreateAsset
        {
            string fullPath = $"{path}/{assetName}.asset"; //������ ����, ��� �������� �����
            T asset = AssetDatabase.LoadAssetAtPath<T>(fullPath); //��������� ��������� ����� �� ���� fullPath � ����������� <T> (������� � ����� ������ CreateAsset ���������)
            if (asset == null) //���� ������ �� ����������, ������ ������� ���:
            {
                asset = ScriptableObject.CreateInstance<T>(); //������ ��������� scriptable object
                AssetDatabase.CreateAsset(asset, fullPath); //������� ����� asset � ���������� ��� � ���� fullPath
            }
            return asset; //���������� ��������� ����� ��� ���������� ���, ������� ��� �����������
        }

        #endregion

    }
}
