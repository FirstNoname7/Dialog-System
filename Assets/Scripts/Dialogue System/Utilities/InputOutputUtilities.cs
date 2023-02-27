using MaryDialogSystem.Elements;
using MaryDialogSystem.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public static void Initialize(MyGraphView graphView, string graphName) //тут иницилаизируем базовые штуки
        {
            myGraphView = graphView; //текущая переговорная
            graphFileName = graphName; //имя переговорной
            containerFolderPath = $"Assets/MeetingRoom/Dialogues/{graphFileName}"; //путь к файлу текущей переговорной
            nodes = new List<MyNode>(); //выделяем память под список с нодами
        }
        #region Save Methods
        public static void Save() //сохраняшка
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

        #endregion

    }
}
