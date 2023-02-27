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
        private readonly string defaultFileName = "»м€ диалогового файла"; //стандартное им€ переговорной (открыто только дл€ чтени€)
        private TextField fileNameTextField; //название текстового пол€
        private Button saveButton; //кнопка дл€ сохранени€ инфы в тулбаре
        [MenuItem("Dialogue System/Meeting Room")] //чтоб создать менюшку свою в верхней части юнити
        public static void Open() //тут действи€ при открытии менюшки
        {
            GetWindow<MeetingRoom>("Meeting Room"); //создаЄтс€ окошко с именем Metting Room
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
            MyGraphView graphView = new MyGraphView(this); //создаю экземпл€р скрипта, в котором показываетс€ UI моей переговорной
            graphView.StretchToParentSize(); //раст€гиваю UI на размер всей переговорной (когда мен€ешь еЄ размер, будет мен€тьс€ размер UI)
            rootVisualElement.Add(graphView); //добавл€ю UI в переговорную (rootVisualElement - эт дефолт от скрипта EditorWindow)
        }

        private void AddToolbar() //добавить верхнюю панель переговорной
        {
            Toolbar toolbar = new Toolbar(); //класс, отвечающий за тулбар
            fileNameTextField = ElementUtility.CreateTextField(defaultFileName, "»м€ файла:", callback => 
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters(); //старое значение текстового пол€ замен€етс€ на новое и в новом удал€ютс€ все пробелы и спец. символы
            }); //в тулбаре текстовое поле с стандартным названием (создание происходит в моем методе CreateTextField)
            saveButton = ElementUtility.CreateButton("—охранить"); //инициализируем кнопку
            toolbar.Add(fileNameTextField); //добавл€ю к тулбару текстовое поле
            toolbar.Add(saveButton); //добавл€ю к тулбару кнопку сохранени€ 
            toolbar.AddStyleSheets("MyToolbarStyles.uss"); //устанавливаю стиль дл€ тулбара
            rootVisualElement.Add(toolbar); //добавл€ю в переговорную тулбар 
        }

        private void AddStyles() //добавл€ю свой Style Sheet дл€ фона окошка
        {
            rootVisualElement.AddStyleSheets("MyStyleSheet.uss");
            //"MyStyleSheet.uss" - загружаю свой стиль дл€ окошка с помощью EditorGUIUtility.Load, в скобках пишу название моего стил€ (находитс€ в папке Editor Default Resources,
            //папка называетс€ именно так, потому что это дефолтное им€ дл€ ресурсов, которые создаешь дл€ какого-нить своего редактора)
        }
        #endregion

        #region Utility Methods
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


