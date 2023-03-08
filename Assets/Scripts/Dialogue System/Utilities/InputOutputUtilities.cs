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
    public static class InputOutputUtilities //сохранение и загрузка переговорной (статичный, т.к. много где используется)
    {
        //ставлю переменные статичными, чтобы определить их как константы 
        private static MyGraphView myGraphView; //ссылка на мое окошко переговорной 
        private static string graphFileName; //имя переговорной
        private static string containerFolderPath;  //путь к контейнерам (где хранятся базовые элементы для переговорной) 
        private static List<MyNode> nodes; //ноды
        private static Dictionary<string, DialogueSO> createdDialogues; //словарь, где ключ = айдишник текущей диалоговой ноды, значение = SO для сохраняшки диалогов
        public static void Initialize(MyGraphView graphView, string graphName) //тут иницилаизируем базовые штуки
        {
            myGraphView = graphView; //текущая переговорная
            graphFileName = graphName; //имя переговорной
            containerFolderPath = $"Assets/MeetingRoom/Dialogues/{graphFileName}"; //путь к файлу текущей переговорной
            nodes = new List<MyNode>(); //выделяем память под список с нодами
            createdDialogues = new Dictionary<string, DialogueSO>(); //инициализируем переменную
        }
        #region Save Methods
        public static void Save() //сохраняшка
        {
            CreateStaticFolders();
            GetElementsFromGraphView();
            GraphSaveDataSO graphSaveDataSO = CreateAsset<GraphSaveDataSO>("Assets/Editor/MeetingRoom/Graphs", $"{graphFileName}Graph"); //создаем ассет для сохраняшки ScriptableObjects 
            graphSaveDataSO.Initialize(graphFileName); //инициализируем имя файла для сохраняшки в ScriptableObject

            DialogueContainerSO dialogueContainer = CreateAsset<DialogueContainerSO>(containerFolderPath, graphFileName); //создаю ассет контейнера для диалогов (путь и название файла определены переменными containerFolderPath и graphFileName)
            dialogueContainer.Initialize(graphFileName); //инициализируем имя файла для контейнера диалогов в ScriptableObject

            SaveNodes(graphSaveDataSO, dialogueContainer);

            SaveAsset(graphSaveDataSO); //сохраняем данные из ассета для SO (Scriptable Object)
            SaveAsset(dialogueContainer); //сохраняем контейнер для диалогов
        }

        #endregion

        #region Groups
        #region Nodes
        private static void SaveNodes(GraphSaveDataSO graphSaveDataSO, DialogueContainerSO dialogueContainer)
        {
            List<string> nodeNames = new List<string>(); //переменная, хранящая имена нод
            foreach (MyNode node in nodes) //для каждой ноды:
            {
                SaveNodeToGraph(node, graphSaveDataSO); //сохраняем ноду в переговорной
                SaveNodeToScriptableObject(node, dialogueContainer); //сохраняем ноду в определенный ScriptableObject
                nodeNames.Add(node.dialogueName); //в список для имён нод кладём имя текущей ноды
            }
            UpdateDialogueChoicesConnections();
            UpdateOldNodes(nodeNames, graphSaveDataSO); //обновляем старые ноды (удаляем неиспользуемые, а используемые сохраняем в graphSaveDataSO)
        }


        private static void SaveNodeToGraph(MyNode node, GraphSaveDataSO graphSaveDataSO)
        {
            List<ChoiceSaveData> choices = new List<ChoiceSaveData>(); //список с текущими кнопками выбора

            foreach (ChoiceSaveData choice in choices) //для каждой кнопки выбора:
            {
                ChoiceSaveData choiceData = new ChoiceSaveData() //в конструкторе переопределяем переменные из скрипта ChoiceSaveData
                {
                    Text = choice.Text, //кладём в переменную для текста то, что написали в текущей кнопке выбора
                    NodeID = choice.NodeID //кладем в переменную для идентификатора айдишник текущей ноды
                };
                choices.Add(choiceData); //к кнопкам выбора добавляем ту, которую только что проинициализировали через конструктор 
            }

            NodeSaveData nodeData = new NodeSaveData() //берём данные из скрипта NodeSaveData и переопределяем их на данные текущей ноды, которая попала в этот метод
            {
                ID = node.ID, //идентификатор
                Name = node.dialogueName, //имя ноды
                Choices = choices, //кнопки ноды (если это нода кнопок выбора). Для них чуть выше мы сделали переменную и проинициализировали её для того, чтоб данные в кнопке выбора сохранялись только когда мы нажмём на кнопку Save, а не каждый кадр
                Text = node.text, //текст в ноде
                DialogueType = node.dialogueType, //тип ноды (диалоговое окно или кнопки выбора)
                Position = node.GetPosition().position //позиция ноды
            };

            graphSaveDataSO.Nodes.Add(nodeData); //к общему списку нод добавляем текущую ноду
        }

        private static void SaveNodeToScriptableObject(MyNode node, DialogueContainerSO dialogueContainer)
        {
            DialogueSO dialogue;
            dialogue = CreateAsset<DialogueSO>($"{containerFolderPath}/Global/Dialogues", node.dialogueName); //создаю ассет для диалогов в нодах
            dialogueContainer.Dialogues.Add(dialogue); //добавляем ассеты диалогов в нодах в контейнер для диалогов
            dialogue.Initialize(
                node.dialogueName,
                node.text,
                ConvertNodeChoicesToDialogueChoices(node.choices), //данные для кнопок выбора
                node.dialogueType,
                node.IsStartingNode() //проверяем, мы на первой ноде или нет (если во входном порте ничего нет, значит это первая нода)
            ); //инициализируем всё с обновлёнными данными (берём данные от текущей ноды)

            createdDialogues.Add(node.ID, dialogue); //в словарь добавляем новый ключ (айдишник) и значение (SO для сохранения диалогов)

            SaveAsset(dialogue); //сохраняем данные диалога из нод
        }

        private static List<DialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<ChoiceSaveData> nodeChoices)
        {
            //nodeChoices = кнопки выбора в нодах, dialogueChoices = кнопки выбора в диалогах
            List<DialogueChoiceData> dialogueChoices = new List<DialogueChoiceData>(); //список для кнопок выбора в диалоге
            foreach (ChoiceSaveData nodeChoice in nodeChoices) //настраиваем каждую кнопку выбора
            {
                DialogueChoiceData choiceData = new DialogueChoiceData()
                {
                    Text = nodeChoice.Text,
                }; //через конструктор настраиваем текст для каждой кнопки выбора (достаём из ноды в диалог этот текст)

                dialogueChoices.Add(choiceData); //добавляем текущую кнопку к списку с кнопками выбора в диалоге
            }
            return dialogueChoices;
        }

        private static void UpdateDialogueChoicesConnections()
        {
            foreach (MyNode node in nodes) //для каждой ноды:
            {
                DialogueSO dialogue = createdDialogues[node.ID]; //ссылаемся на значение ключа из словаря createdDialogues
                for (int choiceIndex = 0; choiceIndex < node.choices.Count; choiceIndex++) //проходимся по всем кнопкам
                {
                    ChoiceSaveData nodeChoice = node.choices[choiceIndex]; //каждую кнопку выбора в отдельности записываем в nodeChoice
                    if (string.IsNullOrEmpty(nodeChoice.NodeID)) //проверяем, пуст ли айдишник у кнопки выбора, если пуст, то:
                    {
                        continue; //продолжаем логику ниже
                    }

                    dialogue.Choices[choiceIndex].NextDialogue = createdDialogues[nodeChoice.NodeID]; //создаём следующую ноду с айдишником
                    SaveAsset(dialogue); //сохраняем новые ноды
                }
            }
        }

        private static void UpdateOldNodes(List<string> currentNodeNames, GraphSaveDataSO graphSaveDataSO)
        {
            if (graphSaveDataSO.OldNodeNames != null && graphSaveDataSO.OldNodeNames.Count != 0) //если список со старыми нодами существует (не пуст и не равен нулю), то:
            {
                List<string> nodesToRemove = graphSaveDataSO.OldNodeNames.Except(currentNodeNames).ToList(); //старые ноды для удаления:все, кроме (Except) текущей ноды (которую недавно добавили)
                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove); //удаляем ассет по пути $"{containerFolderPath}/Global/Dialogues" с именем nodesToRemove
                }
            }

            graphSaveDataSO.OldNodeNames = new List<string>(currentNodeNames); //в данные, которые надо сохранить, добавляем только текущую ноду (по её имени)
        }


        #endregion

        #endregion

        #region Fetch Methods
        private static void GetElementsFromGraphView()
        {
            myGraphView.graphElements.ForEach(graphElement =>
            {
                if(graphElement is MyNode node) //если в окно переговорной поступают новые ноды, то:
                {
                    nodes.Add(node); //добавить их к списку уже существующих нод
                    return; //возвращаемся
                }
            }); //проходимся по всем элементам текущей переговорной ()
        }
        #endregion

        #region Create Methods
        private static void CreateStaticFolders() //статичные папки = данные, которые не меняются в переговорной (папка, где хранятся данные переговорной)
        {
            CreateFolder("Assets/Editor/MeetingRoom", "Graphs"); //папка редактора, не включается в RunTime (процесс игры)
            
            CreateFolder("Assets", "MeetingRoom"); //папка не для редактора, включается в RunTime (процесс игры), там будет сама диалоговая система
            CreateFolder("Assets/MeetingRoom", "Dialogues"); //папка для элементов диалогов
            CreateFolder("Assets/MeetingRoom/Dialogues", graphFileName); //папка для текущей переговорной

            CreateFolder(containerFolderPath, "Global"); //общая папка для текущей переговорной, где будут хранится ноды в этой переговорной и данные внутри них
            CreateFolder($"{containerFolderPath}/Global", "Dialogues"); //папка для диалогов текущей переговорной
        }
        #endregion

        #region Utility Methods
        private static void CreateFolder(string path, string folderName) //path = путь к создаваемой папке, folderName = имя создаваемой папки
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}")) //если в окне Project есть папка Graphs по указанному пути (знак $ перед строкой, чтобы в ней использовать переменные в качестве текста)
            {
                return; //тогда просто возвращаемся, не будем же мы добавлять то, что уже существует
            }
            AssetDatabase.CreateFolder(path, folderName); //если папки нет, то создаём её
        }
        private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject //делегат Т = в него можно запихнуть любой тип данных (сокращение от слова Type), мы пихаем ScriptableObject, который будем создавать в этом методе CreateAsset
        {
            string fullPath = $"{path}/{assetName}.asset"; //полный путь, где создадим ассет
            T asset = AssetDatabase.LoadAssetAtPath<T>(fullPath); //загружаем созданный ассет по пути fullPath с параметрами <T> (которые в нашем методе CreateAsset находятся)
            if (asset == null) //если ассета не существует, значит добавим его:
            {
                asset = ScriptableObject.CreateInstance<T>(); //создаём экземпляр scriptable object
                AssetDatabase.CreateAsset(asset, fullPath); //создаем ассет asset и запихиваем его в путь fullPath
            }
            return asset; //возвращаем созданный ассет или подгружаем тот, который уже существовал
        }

        private static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset"); //удаляем ассет по пути path с именем assetName
        }


        private static void SaveAsset(UnityEngine.Object asset) //благодаря этому методу при изменении ассета он будет сохраняться в окне project в обновленном виде
        {
            EditorUtility.SetDirty(asset); //говорим, что ассет поменялся
            AssetDatabase.SaveAssets(); //сохраняем измененный ассет (видит все "грязные", несохраненные ассеты и сохраняет их)
            AssetDatabase.Refresh(); //обновляем инфу, чтоб были актуальные данные (с обновленным ассетом)
        }
        #endregion

    }
}
