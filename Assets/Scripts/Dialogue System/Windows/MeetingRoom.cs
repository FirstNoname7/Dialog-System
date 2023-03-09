using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using MaryDialogSystem.Utilities;

namespace MaryDialogSystem.Windows
{
    public class MeetingRoom : EditorWindow //отвечает за визуальную сторону редактора (визуалка моей переговорной)
    {
        private MyGraphView graphView; //ссылка на мой граф
        private readonly string defaultFileName = "Имя диалогового файла"; //стандартное имя переговорной (открыто только для чтения)
        private static TextField fileNameTextField; //название текстового поля (статичное, т.к. эти данные загружаются из сохраняшек, так удобнее)
        private Button saveButton; //кнопка для сохранения инфы в тулбаре
        private Button clearButton;
        private Button resetButton;
        [MenuItem("Dialogue System/Meeting Room")] //чтоб создать менюшку свою в верхней части юнити
        public static void Open() //тут действия при открытии менюшки
        {
            GetWindow<MeetingRoom>("Meeting Room"); //создаётся окошко с именем Metting Room
        }

        private void OnEnable() //срабатывает перед тем, как ты тыкнула на кнопку Meeting Room
        {
            AddGraphView();
            AddToolbar();
            AddStyles();
        }


        #region Add
        private void AddGraphView()
        {
            graphView = new MyGraphView(this); //создаю экземпляр скрипта, в котором показывается UI моей переговорной
            graphView.StretchToParentSize(); //растягиваю UI на размер всей переговорной (когда меняешь её размер, будет меняться размер UI)
            rootVisualElement.Add(graphView); //добавляю UI в переговорную (rootVisualElement - эт дефолт от скрипта EditorWindow)
        }

        private void AddToolbar() //добавить верхнюю панель переговорной
        {
            Toolbar toolbar = new Toolbar(); //класс, отвечающий за тулбар
            fileNameTextField = ElementUtility.CreateTextField(defaultFileName, "Имя файла:", callback => 
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters(); //старое значение текстового поля заменяется на новое и в новом удаляются все пробелы и спец. символы
            }); //в тулбаре текстовое поле с стандартным названием (создание происходит в моем методе CreateTextField)
            saveButton = ElementUtility.CreateButton("Сохранить", ()=>Save()); //инициализируем кнопку, в обратный вызов (()=>Save()) ставим ссылку на метод Save, который сохраняет данные
            clearButton = ElementUtility.CreateButton("Очистить", ()=>Clear()); //создаю кнопку для очистки переговорной, обратный вызов = переход на метод Clear (т.е. когда кликаем на кнопку, то переходим на этот метод)
            resetButton = ElementUtility.CreateButton("Перезагрузить", ()=>Reset()); //создаю кнопку для перезагрузки переговорной
            toolbar.Add(fileNameTextField); //добавляю к тулбару текстовое поле
            toolbar.Add(saveButton); //добавляю к тулбару кнопку сохранения 
            toolbar.Add(clearButton); //добавляю к тулбару кнопку удаления содержимого переговорной 
            toolbar.Add(resetButton); //добавляю к тулбару кнопку перезаписи содержимого переговорной на дефолтные значения
            toolbar.AddStyleSheets("MyToolbarStyles.uss"); //устанавливаю стиль для тулбара
            rootVisualElement.Add(toolbar); //добавляю в переговорную тулбар 
        }


        private void AddStyles() //добавляю свой Style Sheet для фона окошка
        {
            rootVisualElement.AddStyleSheets("MyStyleSheet.uss");
            //"MyStyleSheet.uss" - загружаю свой стиль для окошка с помощью EditorGUIUtility.Load, в скобках пишу название моего стиля (находится в папке Editor Default Resources,
            //папка называется именно так, потому что это дефолтное имя для ресурсов, которые создаешь для какого-нить своего редактора)
        }
        #endregion
        #region Toolbar Actions
        private void Save() //сохраняшка содержимого переговорной
        {
            if (string.IsNullOrEmpty(fileNameTextField.value)) //если нет названия того, что нужно сохранить, то:
            {
                EditorUtility.DisplayDialog(
                    "Неверное имя файла",
                    "Убедитесь, что введенное вами имя файла является допустимым",
                    "OK"
                ); //выводим текст об ошибке (DisplayDialog - зарезервированная функция)
                return; //возвращаемся, т.к. нам не над сохранять пустой файл
            }

            InputOutputUtilities.Initialize(graphView, fileNameTextField.value); //инициализируем что именно сохранять (данные в переговорной (graphView) и название переговорной (fileNameTextField.value)
            InputOutputUtilities.Save(); //ссылаюсь на сохраняшку из другого скрипта
        }
        private void Clear()
        {
            graphView.ClearGraph();
        }
        private void Reset()
        {
            Clear(); //ремуваем всё
            UpdateFileName(defaultFileName); //ставим дефолтные названия
        }

        #endregion
        #region Utility Methods
        public static void UpdateFileName(string newFileName) //метод публичен и статичен, т.к. используется при загрузке данных
        {
            fileNameTextField.value = newFileName; //обновление текстового поля
        }
        public void EnableSaving()
        {
            saveButton.SetEnabled(true); //ну это как saveButton.SetActive(true), только вкл. объект в переговорной, а не в инспекторе
        }
        public void DisableSaving() 
        {
            saveButton.SetEnabled(false); //ну это как saveButton.SetActive(false), только выкл. объект в переговорной, а не в инспекторе
        }

        #endregion
    }
}


