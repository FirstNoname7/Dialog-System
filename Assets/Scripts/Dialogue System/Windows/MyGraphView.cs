using MaryDialogSystem.Data.Error;
using MaryDialogSystem.Data.Save;
using MaryDialogSystem.Elements;
using MaryDialogSystem.Enumerations;
using MaryDialogSystem.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MaryDialogSystem.Windows
{
    public class MyGraphView : GraphView //������� ������ �� ���� ������ �����
    {
        private MeetingRoom editorWindow; //�������� �� ������������
        private MySearchWindow searchWindow;
        private SerializableDictionary<string, MyNodeErrorData> serializableDictionary; //��� ������ ������ � ����� � ���� ������� (���� = ��� ����, �������� = MyNodeErrorData, ������� ��������� ���� � ����)

        private int repeatedNamesAmount; //������� ������������� ���
        public int RepeatedNamesAmount 
        {
            get => repeatedNamesAmount; //��������� ������ �������� ������ ��� ����������, �� �� ������ �
            set
            {
                repeatedNamesAmount = value; //��������� ����������� ��������
                if (repeatedNamesAmount == 0)
                {
                    editorWindow.EnableSaving(); //�������� ������ "���������" (����� � ������������ ��� ���������� ���)
                }
                if (repeatedNamesAmount == 1)
                {
                    editorWindow.DisableSaving(); //��������� ������ "���������" (����������, ����� � ������������ ��������� 2 ���������� ����)
                }

            }
        }

        public MyGraphView(MeetingRoom meetingRoom) //�����������, � ���� � �������� ��������� ������� ������������, ����� ������������������� ��
        {
            editorWindow = meetingRoom; //������������� ������������
            serializableDictionary = new SerializableDictionary<string, MyNodeErrorData>(); //������������� �������, ����� � ���� �������� ��� ����, ������� ������
            AddManipulators();
            AddSearchWindow(); //�������� ������ ������
            AddGridBackground(); //��������� ��� ��� ������
            OnElementsDeleted(); //���� � ���-�� ������, ���� ����
            OnGraphViewChanged(); //��� ��������� ������ (���������� ��� �������� ����-����) ����� ������ ������
            AddStyles(); //�������� ���� Style Sheet ��� ���� ������
        }

        #region Overrided Methods
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) //�������������� �����, ������� �������� �� ����� (����� � ������ ����)
        {
            List<Port> compatiblePorts = new List<Port>(); //�������� �� ����������� ����� (��, ������� ����� �����������)
            ports.ForEach(port =>
            {
                if (startPort == port) //���� ��������� ���������� ������� ���� � ����� ����, ��� ���������, �������:
                {
                    return; //������������, ����� ��������� ��������� �����
                }
                if (startPort.node == port.node) //���� ��������� ���������� ���� � ������� ����, ��:
                {
                    return; //������������, ����� ��������� ��������� �����
                }
                if (startPort.direction == port.direction) //���� ��������� ���������� ������� ���� � �������� ��� �������� � ���������, ��:
                {
                    return; //������������, ����� ��������� ��������� �����
                }
                compatiblePorts.Add(port); //� ������ ������� ��������� ���� � ����������. ��� ��������, ��� ����� ���� ����� ���������� � �����, ���� ����������

            }); //ports - ��������� ����������, ������� ��������� � �������� �����, �.�. � ���� ������ � ������������
            //ForEach ����� ����� ���, ��� �������� ������ ���� �� ����� ����� ������
            return compatiblePorts; //��������� ����, ������� ����� ���������� � ������ ����
        }
        #endregion
        #region Create
        public MyNode CreateNode(DialogueType dialogueType, Vector2 position) //�������� ���� ������������� ���� (dialogueType) �� ������������ ������� (�������� position)
        {
            Type nodeType = Type.GetType($"MaryDialogSystem.Elements.{dialogueType}Node"); //�������� ������� ��� ���� (��� ������� ��� ��� ������ ������)
            MyNode node = (MyNode)Activator.CreateInstance(nodeType); //������ � UI ��������� ��� ����
            node.Initialize(this, position); //�������������� ����, ���� �� ���������� ������� ����, ��� ������ ���� � ������������
            node.Draw(); //��������� ���������� � ���� (���� ��� ����� ����� �������, ����� � ���� � �.�.)
            AddNode(node); 
            AddElement(node); //��������� ���� � ������������
            return node; //���������� ����
        }

        #endregion
        #region Manipulators
        private void AddManipulators() //�������� � ������������
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale); //���� ����� ���� �����������/��������� ����� (��� �������) (������ ��������� ����� ��� ��������� ������� � ��������� (���� � ��� �����))
            this.AddManipulator(new ContentDragger()); //���� ����� ���� ������ �� �����, ������ �� �������� ����
            //this.AddManipulator(new RectangleSelector()); //����� ����� ���� �������� ��������� ���, ������� ��
            this.AddManipulator(new SelectionDragger()); //����� ����� ���� ������� ����
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DialogueType.SingleChoice)); //������ ����������� ����, ���� ��� ����� �� ��� ����� ���� ������� ��� ���� "������"
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DialogueType.MultipleChoice)); //������ ����������� ����, ���� ��� ����� �� ��� ����� ���� ������� ��� ���� "������ ������"

        }
        private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType dialogueType) //�������� ������������ ���� ��� ������ ���� ���� (���� ��� ������ ��� ��� ������ ������)
                                                                                                     //���������: actionTitle ��� ��������� ������, dialogueType ��� �������� ���� ���� (������ ��� ������ ������)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))); //� ����������� ���� ����� ������� ����� ����. ������� ������� ������ �� ����, ��� ��������� ������ + ������������ � ��������� ����������
            return contextualMenuManipulator; //���������� ����� ������ � ����������� ����
        }

        #endregion
        #region Add
        private void AddSearchWindow()
        {
            if (searchWindow == null) //���� ������ ��� ������ ���, ��:
            {
                searchWindow = ScriptableObject.CreateInstance<MySearchWindow>(); //������� ���
                searchWindow.Initialize(this); //� ��������������
            }
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow); //������ �� �������� ������������ ����
            //����� ���������� ��� ����� �� ������, ����� �������� ������������
        }
        private void AddGridBackground() //��������� ��� ��� ������
        {
            GridBackground gridBackground = new GridBackground(); //������� ��� ��� �����
            gridBackground.StretchToParentSize(); //���������� UI �� ������ ���� ������������ (����� ������� � ������, ����� �������� ������ UI)
            Insert(0, gridBackground); //��, ��� ������� ��� ��� ������
        }
        private void AddStyles() //�������� ���� Style Sheet
        {
            this.AddStyleSheets("MyStyleSheet.uss", "NodeStyles.uss"); 
            //"MyStyleSheet.uss" - �������� ���� ����� ��� ������ � ������� EditorGUIUtility.Load, � ������� ���� �������� ����� ����� (��������� � ����� Editor Default Resources,
            //����� ���������� ������ ���, ������ ��� ��� ��������� ��� ��� ��������, ������� �������� ��� ������-���� ������ ���������)
            //"NodeStyles.uss" - ����� ��� ���
        }
        #endregion
        #region Utilities
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isMySearchWindow = false) //����� ���� ���������� ���, ��� �� ������� ������, ���� ���� �� ������������� �� ������������
                                                                                                   //(���� ����� �� �������, ����� ������������ ��������� ������ ������������ � ���� �� ������� ���� �� ��������� ����� �������, �� ������ ������ �� ������� ����)
                                                                                                   ////�� ���� ��� ����������� ��������� ������� � ��������� �����������, �� ����������
        ///������ �������� isMySearchWindow, ����� ���������, ���� ��������� �� ������������ ����, ������� � �������, ��� �� ����, ������� ���������� ��� ���
        {
            Vector2 worldMousePosition = mousePosition; //������� ���������� ����������
            if (isMySearchWindow)
            {
                worldMousePosition -= editorWindow.position.position; //������� �� ��������� ���� ������� ���� ������, ����� ���� ��������� ����� � �����, � �� ��� ���
            }
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition); //����������� ���������� ���������� � ��������� (WorldToLocal) � ������ ������ ������������ (contentViewContainer)
            return localMousePosition; //��������� ��������� �������
        }
        #endregion
        #region Repeated Elements
        public void AddNode(MyNode node) //�������� ����, ����� � ������������ ��� �� ����� ����. ���� ���� ����, �� �������� �� � ������� ����� ������
        {

            string nodeName = node.dialogueName.ToLower(); //� ������� � �������� ����� ������ ��� ����, ��� ������� + ����� ��� ��� � ������� �����, ���� �������� ����� � ������ ���� (������: ��� Unity ����������� � ���������� ����������, ���������� � ���������� �����) (������ ��� ��������� ��������� �� ����� �����)
            if (!serializableDictionary.ContainsKey(nodeName)) //���� � ������� ��� ����� � ������� ������, ��:
            {
                MyNodeErrorData myNodeErrorData = new MyNodeErrorData(); //������������� ������ ������
                myNodeErrorData.Nodes.Add(node); //����������� ������� ����
                serializableDictionary.Add(nodeName, myNodeErrorData); //�������� ���� ������� �� ��������� "������", ������� ����� �������� � ������, ���� ���� 2 ���������� ����
                return; //���� ������ �� ���� �� ������� �������
            }

            List<MyNode> nodesList = serializableDictionary[nodeName].Nodes; //������ ������� ���

            nodesList.Add(node); //���� ������� ���� �� �����������, ������ ����� ��� ���� ��� ����, ������ ��������� ��� � �������
            Color errorColor = serializableDictionary[nodeName].ErrorData.color; //������ ���� ���� � �������
            node.SetErrorStyle(errorColor); //������������ ���� ���� � �������
            if (nodesList.Count == 2) //���� � ������������ ������ ����� ����� ����, ��:
            {
                ++RepeatedNamesAmount; //��������� ��� ������ ����
                nodesList[0].SetErrorStyle(errorColor); //��������� ���� ������ ���� (��� ����� ����� ����� ��������������� ������ �����, ������� ��������� ����� ������ ����)
            }
        }

        public void RemoveNode(MyNode node) //������� �� ������ ����, � ������� �� �������� ��������
        {
            string nodeName = node.dialogueName.ToLower(); //��������� �� ������� �������� � ���� + ����� ��� ��� � ������� �����, ���� �������� ����� � ������ ���� (������: ��� Unity ����������� � ���������� ����������, ���������� � ���������� �����) (������ ��� ��������� ��������� �� ����� �����)
            List<MyNode> currentNodes = serializableDictionary[nodeName].Nodes;
            currentNodes.Remove(node); //������� ������� ���� �� �������
            node.ResetStyle(); //������ ����������� ����� �����
            if(currentNodes.Count == 1) //���� � ������������ 1 ����, ��:
            {
                --RepeatedNamesAmount; //������� ��� ���� ����
                currentNodes[0].ResetStyle(); //������ ����������� ����� ��� ���� ���� �� �������
                return;
            }
            if(currentNodes.Count == 0) //���� � ������������ ��� ���, ��:
            {
                serializableDictionary.Remove(nodeName); //������� ��� ���� �� �������
            }
        }
        #endregion
        #region Callbacks
        private void OnElementsDeleted() //����������, ��� ����� ����������� ��� �������� �������� �� ������������
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type edgeType = typeof(Edge); //�������� �� ��������, ��������� �� ������� ����

                List<Edge> edgesToDelete = new List<Edge>(); //������ ���������, ��������� �� ������� ����
                List<MyNode> nodesToDelete = new List<MyNode>(); //������������� ����, ������� ����� �������
                foreach(GraphElement element in selection)
                {
                    if(element is MyNode node) //���������, �������� �� ������� GraphElement ����� (MyNode) ��� ����������� �� ������� GraphElement �� ������ MyNode
                                          //(� ���� ������ ����� ������ ��������, �������� �� GraphElement ������� �����)
                    {
                        nodesToDelete.Add(node); //�������� � ������ ��� �������� �� ����, ������� ������� �������
                        continue; //��������� ������ ������
                    }

                    if (element.GetType() == edgeType) //���� ������� ������� = ��������, ��������� �� ����, ��:
                    {
                        Edge edge = (Edge)element; //�������� �������� ������� ���������, ������� �� ������
                        edgesToDelete.Add(edge); //��������� � ������ ��� �������� ��������
                        continue; //��������� ������ ������
                    }
                }

                DeleteElements(edgesToDelete); //������� �������� ����� ����������������� ����� DeleteElements

                foreach (MyNode node in nodesToDelete) //��������� �� ���� �����, ������� ���� �������
                {
                    RemoveNode(node); //������ ����, ������� ���� �������
                    RemoveElement(node); //����� ������ �������� (�� ����������������� ����� �� GraphView)
                    node.DisconnectAllPorts(); //������� �������/�������� �������� (����� ��� �������� ���� ��� �� ���������� � ������������)
                }

            };
        }
        private void OnGraphViewChanged() //����������������� �����, �����������, ����� �������� ������ � graph view (������������) (���� ��� ���������� ������ � �������)
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null) //���� �������� ������� �����-���� �������� � graph view (������������), ��:
                {
                    foreach (Edge edge in changes.edgesToCreate) //���������� ������ �� ������� �������� � ������������
                    {
                        //������ ��������� ���� � �����/������ ��� ��
                        MyNode nextNode = (MyNode)edge.input.node; //������ ���� � � ������� � ������� ����������� ���������� � ������ �����
                        ChoiceSaveData choiceData = (ChoiceSaveData)edge.output.userData;  //������ �������� ����� � ������ ������
                        choiceData.NodeID = nextNode.ID; //������ ������������� ��������� ����
                    }
                }
                if (changes.elementsToRemove != null) //���� �������� ������� �����-���� �������� �� graph view (������������), ��:
                {
                    Type edgeType = typeof(Type); //����������-��������� � ��� ������ Type
                    foreach (GraphElement element in changes.elementsToRemove) //���������� ������ �� ������� ��������, ������� �������� ������� � ������������
                    {
                        if (element.GetType() != edgeType) //���� ������� �� � ����� Type, ��:
                        {
                            continue; //����������, �.�. ��� ����� �������
                        }
                        Edge edge = (Edge)element; //������� �������
                        ChoiceSaveData choiceData = (ChoiceSaveData)edge.output.userData; //������� ������ ������ ������
                        choiceData.NodeID = ""; //�������� �������������
                    }
                }
                return changes;
            };
        }
        #endregion
    }

}
