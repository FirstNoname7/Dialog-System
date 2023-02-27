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
    public class MyGraphView : GraphView //создает окошко по типу шейдер графа
    {
        private MeetingRoom editorWindow; //ссылаюсь на переговорную
        private MySearchWindow searchWindow;
        private SerializableDictionary<string, MyNodeErrorData> serializableDictionary; //для вывода ошибок в нодах в виде словаря (ключ = имя ноды, значение = MyNodeErrorData, который соединяет цвет и ноды)

        private int repeatedNamesAmount; //счетчик повторяющихся имён
        public int RepeatedNamesAmount 
        {
            get => repeatedNamesAmount; //позволяем другим скриптам читать эту переменную, но не менять её
            set
            {
                repeatedNamesAmount = value; //позволяем настраивать значения
                if (repeatedNamesAmount == 0)
                {
                    editorWindow.EnableSaving(); //Включить кнопку "Сохранить" (когда в переговорной нет одинаковых нод)
                }
                if (repeatedNamesAmount == 1)
                {
                    editorWindow.DisableSaving(); //Выключить кнопку "Сохранить" (выключится, когда в переговорной находятся 2 одинаковые ноды)
                }

            }
        }

        public MyGraphView(MeetingRoom meetingRoom) //конструктор, у него в качестве параметра передаю переговорную, чтобы проинициализировать ее
        {
            editorWindow = meetingRoom; //инициализирую переговорную
            serializableDictionary = new SerializableDictionary<string, MyNodeErrorData>(); //инициализирую словарь, чтобы в него помещать все ноды, которые создаю
            AddManipulators();
            AddSearchWindow(); //добавляю окошко поиска
            AddGridBackground(); //добавляем фон для окошка
            OnElementsDeleted(); //если я что-то удаляю, идем сюда
            OnGraphViewChanged(); //при изменении данных (добавлении или удалении чего-либл) будет логика отсюда
            AddStyles(); //добавляю свой Style Sheet для фона окошка
        }

        #region Overrided Methods
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) //переопределяем метод, который отвечает за порты (входы и выходы ноды)
        {
            List<Port> compatiblePorts = new List<Port>(); //отвечает за совместимые порты (те, которые могут скрепляться)
            ports.ForEach(port =>
            {
                if (startPort == port) //если пытаешься подключить текущую ноду к самой себе, это нелогично, поэтому:
                {
                    return; //возвращаемся, чтобы проверить остальные порты
                }
                if (startPort.node == port.node) //если пытаешься подключить порт к текущей ноде, то:
                {
                    return; //возвращаемся, чтобы проверить остальные порты
                }
                if (startPort.direction == port.direction) //если пытаешься подключить входной порт к входному или выходной к выходному, то:
                {
                    return; //возвращаемся, чтобы проверить остальные порты
                }
                compatiblePorts.Add(port); //в других случаях добавляем порт к переменной. Это означает, что такой порт можно прикрепить к месту, куда наводишься

            }); //ports - дефолтная переменная, которая относится к текущему графу, т.е. в моем случае к переговорной
            //ForEach здесь хорош тем, что выделяет каждый порт из общей массы портов
            return compatiblePorts; //возвращаю порт, который можно прикрепить к другой ноде
        }
        #endregion
        #region Create
        public MyNode CreateNode(DialogueType dialogueType, Vector2 position) //создание ноды определенного типа (dialogueType) на определенной позиции (параметр position)
        {
            Type nodeType = Type.GetType($"MaryDialogSystem.Elements.{dialogueType}Node"); //указываю текущий тип ноды (для диалога или для кнопок выбора)
            MyNode node = (MyNode)Activator.CreateInstance(nodeType); //создаю в UI выбранный тип ноды
            node.Initialize(this, position); //инициализируем ноду, чтоб не показывать пустоту тому, кто увидит ноду в переговорной
            node.Draw(); //добавляем наполнение в ноду (поле для ввода фразы диалога, входы в ноду и т.д.)
            AddNode(node); 
            AddElement(node); //добавляем ноду в переговорную
            return node; //возвращаем ноду
        }

        #endregion
        #region Manipulators
        private void AddManipulators() //действия с переговорной
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale); //чтоб можно было увеличивать/уменьшать экран (для скролла) (можешь поставить точки для остановки скролла в параметры (макс и мин точки))
            this.AddManipulator(new ContentDragger()); //чтоб можно было тянуть за экран, кликая на колесико мыши
            //this.AddManipulator(new RectangleSelector()); //чтобы можно было выделить несколько нод, выделяя их
            this.AddManipulator(new SelectionDragger()); //чтобы можно было двигать ноды
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DialogueType.SingleChoice)); //создаю контекстное меню, чтоб при клике на ПКМ можно было выбрать тип ноды "диалог"
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DialogueType.MultipleChoice)); //создаю контекстное меню, чтоб при клике на ПКМ можно было выбрать тип ноды "кнопки выбора"

        }
        private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType dialogueType) //создание контекстного меню для выбора типа ноды (нода для текста или для кнопок выбора)
                                                                                                     //параметры: actionTitle для заголовка кнопки, dialogueType для указания типа ноды (диалог или кнопки выбора)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))); //в контекстном меню можно создать новую ноду. Позиция берется исходя из того, где находится курсор + превращается в локальные координаты
            return contextualMenuManipulator; //возвращаем новые пункты в контекстное меню
        }

        #endregion
        #region Add
        private void AddSearchWindow()
        {
            if (searchWindow == null) //если окошка для поиска нет, то:
            {
                searchWindow = ScriptableObject.CreateInstance<MySearchWindow>(); //создаем его
                searchWindow.Initialize(this); //и инициализируем
            }
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow); //запрос на создание контекстного меню
            //будет вызываться при клике на пробел, когда выделена переговорная
        }
        private void AddGridBackground() //добавляем фон для окошка
        {
            GridBackground gridBackground = new GridBackground(); //создает фон для сетки
            gridBackground.StretchToParentSize(); //растягиваю UI на размер всей переговорной (когда меняешь её размер, будет меняться размер UI)
            Insert(0, gridBackground); //то, что выводит фон для окошка
        }
        private void AddStyles() //добавляю свои Style Sheet
        {
            this.AddStyleSheets("MyStyleSheet.uss", "NodeStyles.uss"); 
            //"MyStyleSheet.uss" - загружаю свой стиль для окошка с помощью EditorGUIUtility.Load, в скобках пишу название моего стиля (находится в папке Editor Default Resources,
            //папка называется именно так, потому что это дефолтное имя для ресурсов, которые создаешь для какого-нить своего редактора)
            //"NodeStyles.uss" - стиль для нод
        }
        #endregion
        #region Utilities
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isMySearchWindow = false) //чтобы нода появлялась там, где мы кликаем мышкой, даже если мы переместились по переговорной
                                                                                                   //(если этого не сделать, будет зафиксирован начальный размер переговорной и если ты создашь ноду за пределами этого размера, то можешь вообще не увидеть ноду)
                                                                                                   ////то есть тут привязываем положение курсора к локальным координатам, не глобальным
        ///создаю параметр isMySearchWindow, чтобы проверить, нода создается из контекстного меню, которое я создала, или из окна, которое появляется при ПКМ
        {
            Vector2 worldMousePosition = mousePosition; //достаем глобальные координаты
            if (isMySearchWindow)
            {
                worldMousePosition -= editorWindow.position.position; //отнимаю от координат мыши позицию окна поиска, чтобы нода появилась рядом с окном, а не под ним
            }
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition); //преобразуем глобальные координаты в локальные (WorldToLocal) в рамках данной переговорной (contentViewContainer)
            return localMousePosition; //возвращаю локальную позицию
        }
        #endregion
        #region Repeated Elements
        public void AddNode(MyNode node) //добавляю ноды, когда в переговорной нет ни одной ноды. Если ноды есть, то обновляю их с помощью этого метода
        {

            string nodeName = node.dialogueName.ToLower(); //в словарь в качестве ключа ставлю имя того, кто говорит + делаю это имя с большой буквы, чтоб привести слова к одному виду (пример: как Unity преобразует в инспекторе переменные, написанные в верблюжьем стиле) (только это изменение визуально не будет видно)
            if (!serializableDictionary.ContainsKey(nodeName)) //если в словаре нет ключа с текущим именем, то:
            {
                MyNodeErrorData myNodeErrorData = new MyNodeErrorData(); //инициализирую скрипт ошибки
                myNodeErrorData.Nodes.Add(node); //добавляется текущая нода
                serializableDictionary.Add(nodeName, myNodeErrorData); //добавляю ключ словаря со значением "ошибка", которое будет показано в случае, если есть 2 одинаковые ноды
                return; //чтоб логика не ушла за пределы условия
            }

            List<MyNode> nodesList = serializableDictionary[nodeName].Nodes; //список текущих нод

            nodesList.Add(node); //если условие выше не выполняется, значит такое имя ноды уже есть, значит добавляем его в словарь
            Color errorColor = serializableDictionary[nodeName].ErrorData.color; //нахожу цвет ноды с ошибкой
            node.SetErrorStyle(errorColor); //устанавливаю цвет ноде с ошибкой
            if (nodesList.Count == 2) //если в переговорной сейчас более одной ноды, то:
            {
                ++RepeatedNamesAmount; //добавляем имя первой ноды
                nodesList[0].SetErrorStyle(errorColor); //обновляем цвет первой ноды (без этого цвета будут устанавливаться только нодам, которые добавлены после первой ноды)
            }
        }

        public void RemoveNode(MyNode node) //удаляем из ошибки ноды, у которых мы поменяли название
        {
            string nodeName = node.dialogueName.ToLower(); //ссылаемся на текущее название в ноде + делаю это имя с большой буквы, чтоб привести слова к одному виду (пример: как Unity преобразует в инспекторе переменные, написанные в верблюжьем стиле) (только это изменение визуально не будет видно)
            List<MyNode> currentNodes = serializableDictionary[nodeName].Nodes;
            currentNodes.Remove(node); //удоляем текущие ноды из словаря
            node.ResetStyle(); //ставим стандартный стиль нодам
            if(currentNodes.Count == 1) //если в переговорной 1 нода, то:
            {
                --RepeatedNamesAmount; //убираем имя этой ноды
                currentNodes[0].ResetStyle(); //ставим стандартный стиль для этой ноды из словаря
                return;
            }
            if(currentNodes.Count == 0) //если в переговорной нет нод, то:
            {
                serializableDictionary.Remove(nodeName); //удаляем имя ноды из словаря
            }
        }
        #endregion
        #region Callbacks
        private void OnElementsDeleted() //показывает, что будет происходить при удалении элемента из переговорной
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type edgeType = typeof(Edge); //отвечает за проводок, отходящий от текущей ноды

                List<Edge> edgesToDelete = new List<Edge>(); //список проводков, отходящих от текущей ноды
                List<MyNode> nodesToDelete = new List<MyNode>(); //инициализирую ноды, которые нужно удалить
                foreach(GraphElement element in selection)
                {
                    if(element is MyNode node) //проверяет, является ли текущий GraphElement нодой (MyNode) или наследуется ли текущий GraphElement от класса MyNode
                                          //(в моем случае важна первая проверка, является ли GraphElement текущей нодой)
                    {
                        nodesToDelete.Add(node); //добавляю в список для удаления те ноды, которые пытаюсь удалить
                        continue; //продолжаю логику дальше
                    }

                    if (element.GetType() == edgeType) //если текущий элемент = проводок, отходящий от ноды, то:
                    {
                        Edge edge = (Edge)element; //проводок является текущим элементом, который мы удолим
                        edgesToDelete.Add(edge); //добавляем в список для удаления проводок
                        continue; //продолжаю логику дальше
                    }
                }

                DeleteElements(edgesToDelete); //удаляем проводки через зарезервированный метод DeleteElements

                foreach (MyNode node in nodesToDelete) //прохожусь по всем нодам, которые надо удалить
                {
                    RemoveNode(node); //удаляю ноды, которые надо удалить
                    RemoveElement(node); //также удаляю элементы (эт зарезервированный метод от GraphView)
                    node.DisconnectAllPorts(); //удоляем входные/выходные проводки (чтобы при удалении ноды они не оставались в переговорной)
                }

            };
        }
        private void OnGraphViewChanged() //зарезервированный метод, срабатывает, когда меняются данные в graph view (переговорной) (надо для сохранения данных в будущем)
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null) //если пытаемся создать какие-либо элементы в graph view (переговорной), то:
                {
                    foreach (Edge edge in changes.edgesToCreate) //проходимся циклом по каждому элементу в переговорной
                    {
                        //создаём следующую ноду и входы/выходы для неё
                        MyNode nextNode = (MyNode)edge.input.node; //создаём ноду и её входной с помощью стандартных переменных в шейдер графе
                        ChoiceSaveData choiceData = (ChoiceSaveData)edge.output.userData;  //создаём выходные порты у кнопок выбора
                        choiceData.NodeID = nextNode.ID; //создаём идентификатор следующей ноды
                    }
                }
                if (changes.elementsToRemove != null) //если пытаемся удалить какие-либо элементы из graph view (переговорной), то:
                {
                    Type edgeType = typeof(Type); //переменная-конвертер в тип данных Type
                    foreach (GraphElement element in changes.elementsToRemove) //проходимся циклом по каждому элементу, которые пытаемся удалить в переговорной
                    {
                        if (element.GetType() != edgeType) //если элемент не с типом Type, то:
                        {
                            continue; //продолжаем, т.к. его нужно удалить
                        }
                        Edge edge = (Edge)element; //удоляем элемент
                        ChoiceSaveData choiceData = (ChoiceSaveData)edge.output.userData; //удоляем данных кнопок выбора
                        choiceData.NodeID = ""; //зануляем идентификатор
                    }
                }
                return changes;
            };
        }
        #endregion
    }

}
