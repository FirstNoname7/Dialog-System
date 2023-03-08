using MaryDialogSystem.Data.Save;
using MaryDialogSystem.DataForSave;
using MaryDialogSystem.Elements;
using MaryDialogSystem.ScriptableObjects;
using MaryDialogSystem.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private static Dictionary<string, DialogueSO> createdDialogues; //�������, ��� ���� = �������� ������� ���������� ����, �������� = SO ��� ���������� ��������
        public static void Initialize(MyGraphView graphView, string graphName) //��� �������������� ������� �����
        {
            myGraphView = graphView; //������� ������������
            graphFileName = graphName; //��� ������������
            containerFolderPath = $"Assets/MeetingRoom/Dialogues/{graphFileName}"; //���� � ����� ������� ������������
            nodes = new List<MyNode>(); //�������� ������ ��� ������ � ������
            createdDialogues = new Dictionary<string, DialogueSO>(); //�������������� ����������
        }
        #region Save Methods
        public static void Save() //����������
        {
            CreateStaticFolders();
            GetElementsFromGraphView();
            GraphSaveDataSO graphSaveDataSO = CreateAsset<GraphSaveDataSO>("Assets/Editor/MeetingRoom/Graphs", $"{graphFileName}Graph"); //������� ����� ��� ���������� ScriptableObjects 
            graphSaveDataSO.Initialize(graphFileName); //�������������� ��� ����� ��� ���������� � ScriptableObject

            DialogueContainerSO dialogueContainer = CreateAsset<DialogueContainerSO>(containerFolderPath, graphFileName); //������ ����� ���������� ��� �������� (���� � �������� ����� ���������� ����������� containerFolderPath � graphFileName)
            dialogueContainer.Initialize(graphFileName); //�������������� ��� ����� ��� ���������� �������� � ScriptableObject

            SaveNodes(graphSaveDataSO, dialogueContainer);

            SaveAsset(graphSaveDataSO); //��������� ������ �� ������ ��� SO (Scriptable Object)
            SaveAsset(dialogueContainer); //��������� ��������� ��� ��������
        }

        #endregion

        #region Groups
        #region Nodes
        private static void SaveNodes(GraphSaveDataSO graphSaveDataSO, DialogueContainerSO dialogueContainer)
        {
            List<string> nodeNames = new List<string>(); //����������, �������� ����� ���
            foreach (MyNode node in nodes) //��� ������ ����:
            {
                SaveNodeToGraph(node, graphSaveDataSO); //��������� ���� � ������������
                SaveNodeToScriptableObject(node, dialogueContainer); //��������� ���� � ������������ ScriptableObject
                nodeNames.Add(node.dialogueName); //� ������ ��� ��� ��� ����� ��� ������� ����
            }
            UpdateDialogueChoicesConnections();
            UpdateOldNodes(nodeNames, graphSaveDataSO); //��������� ������ ���� (������� ��������������, � ������������ ��������� � graphSaveDataSO)
        }


        private static void SaveNodeToGraph(MyNode node, GraphSaveDataSO graphSaveDataSO)
        {
            List<ChoiceSaveData> choices = new List<ChoiceSaveData>(); //������ � �������� �������� ������

            foreach (ChoiceSaveData choice in choices) //��� ������ ������ ������:
            {
                ChoiceSaveData choiceData = new ChoiceSaveData() //� ������������ �������������� ���������� �� ������� ChoiceSaveData
                {
                    Text = choice.Text, //����� � ���������� ��� ������ ��, ��� �������� � ������� ������ ������
                    NodeID = choice.NodeID //������ � ���������� ��� �������������� �������� ������� ����
                };
                choices.Add(choiceData); //� ������� ������ ��������� ��, ������� ������ ��� ������������������� ����� ����������� 
            }

            NodeSaveData nodeData = new NodeSaveData() //���� ������ �� ������� NodeSaveData � �������������� �� �� ������ ������� ����, ������� ������ � ���� �����
            {
                ID = node.ID, //�������������
                Name = node.dialogueName, //��� ����
                Choices = choices, //������ ���� (���� ��� ���� ������ ������). ��� ��� ���� ���� �� ������� ���������� � ������������������� � ��� ����, ���� ������ � ������ ������ ����������� ������ ����� �� ����� �� ������ Save, � �� ������ ����
                Text = node.text, //����� � ����
                DialogueType = node.dialogueType, //��� ���� (���������� ���� ��� ������ ������)
                Position = node.GetPosition().position //������� ����
            };

            graphSaveDataSO.Nodes.Add(nodeData); //� ������ ������ ��� ��������� ������� ����
        }

        private static void SaveNodeToScriptableObject(MyNode node, DialogueContainerSO dialogueContainer)
        {
            DialogueSO dialogue;
            dialogue = CreateAsset<DialogueSO>($"{containerFolderPath}/Global/Dialogues", node.dialogueName); //������ ����� ��� �������� � �����
            dialogueContainer.Dialogues.Add(dialogue); //��������� ������ �������� � ����� � ��������� ��� ��������
            dialogue.Initialize(
                node.dialogueName,
                node.text,
                ConvertNodeChoicesToDialogueChoices(node.choices), //������ ��� ������ ������
                node.dialogueType,
                node.IsStartingNode() //���������, �� �� ������ ���� ��� ��� (���� �� ������� ����� ������ ���, ������ ��� ������ ����)
            ); //�������������� �� � ����������� ������� (���� ������ �� ������� ����)

            createdDialogues.Add(node.ID, dialogue); //� ������� ��������� ����� ���� (��������) � �������� (SO ��� ���������� ��������)

            SaveAsset(dialogue); //��������� ������ ������� �� ���
        }

        private static List<DialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<ChoiceSaveData> nodeChoices)
        {
            //nodeChoices = ������ ������ � �����, dialogueChoices = ������ ������ � ��������
            List<DialogueChoiceData> dialogueChoices = new List<DialogueChoiceData>(); //������ ��� ������ ������ � �������
            foreach (ChoiceSaveData nodeChoice in nodeChoices) //����������� ������ ������ ������
            {
                DialogueChoiceData choiceData = new DialogueChoiceData()
                {
                    Text = nodeChoice.Text,
                }; //����� ����������� ����������� ����� ��� ������ ������ ������ (������ �� ���� � ������ ���� �����)

                dialogueChoices.Add(choiceData); //��������� ������� ������ � ������ � �������� ������ � �������
            }
            return dialogueChoices;
        }

        private static void UpdateDialogueChoicesConnections()
        {
            foreach (MyNode node in nodes) //��� ������ ����:
            {
                DialogueSO dialogue = createdDialogues[node.ID]; //��������� �� �������� ����� �� ������� createdDialogues
                for (int choiceIndex = 0; choiceIndex < node.choices.Count; choiceIndex++) //���������� �� ���� �������
                {
                    ChoiceSaveData nodeChoice = node.choices[choiceIndex]; //������ ������ ������ � ����������� ���������� � nodeChoice
                    if (string.IsNullOrEmpty(nodeChoice.NodeID)) //���������, ���� �� �������� � ������ ������, ���� ����, ��:
                    {
                        continue; //���������� ������ ����
                    }

                    dialogue.Choices[choiceIndex].NextDialogue = createdDialogues[nodeChoice.NodeID]; //������ ��������� ���� � ����������
                    SaveAsset(dialogue); //��������� ����� ����
                }
            }
        }

        private static void UpdateOldNodes(List<string> currentNodeNames, GraphSaveDataSO graphSaveDataSO)
        {
            if (graphSaveDataSO.OldNodeNames != null && graphSaveDataSO.OldNodeNames.Count != 0) //���� ������ �� ������� ������ ���������� (�� ���� � �� ����� ����), ��:
            {
                List<string> nodesToRemove = graphSaveDataSO.OldNodeNames.Except(currentNodeNames).ToList(); //������ ���� ��� ��������:���, ����� (Except) ������� ���� (������� ������� ��������)
                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove); //������� ����� �� ���� $"{containerFolderPath}/Global/Dialogues" � ������ nodesToRemove
                }
            }

            graphSaveDataSO.OldNodeNames = new List<string>(currentNodeNames); //� ������, ������� ���� ���������, ��������� ������ ������� ���� (�� � �����)
        }


        #endregion

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

        private static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset"); //������� ����� �� ���� path � ������ assetName
        }


        private static void SaveAsset(UnityEngine.Object asset) //��������� ����� ������ ��� ��������� ������ �� ����� ����������� � ���� project � ����������� ����
        {
            EditorUtility.SetDirty(asset); //�������, ��� ����� ���������
            AssetDatabase.SaveAssets(); //��������� ���������� ����� (����� ��� "�������", ������������� ������ � ��������� ��)
            AssetDatabase.Refresh(); //��������� ����, ���� ���� ���������� ������ (� ����������� �������)
        }
        #endregion

    }
}
