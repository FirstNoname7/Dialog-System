using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using MaryDialogSystem.Enumerations;
using MaryDialogSystem.Elements;

namespace MaryDialogSystem.Windows
{
    public class MySearchWindow : ScriptableObject, ISearchWindowProvider //отвечает за окошко поиска
    {
        private MyGraphView graphView; //переговорная
        public void Initialize(MyGraphView myGraphView) //инициализирую переговорную, чтобы было где показывать окно поиска
        {
            graphView = myGraphView; //переношу текущую переговорную из переменной myGraphView в глобальную переменную, чтоб ее можно было использовать еще и в методе OnSelectEntry
        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) //создание менюшки для поиска (от интерфейса ISearchWindowProvider)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Создать элемент")), //1 окно поиска (подзаголовок окна поиска)
                new SearchTreeGroupEntry(new GUIContent("Диалоговое окно"), 1), //2 окно поиска (1 означает 1 уровень, т.е. что это является подпапкой основного окна поиска)
                new SearchTreeEntry(new GUIContent("Диалоговое окно"))
                {
                    level = 2,
                    userData = DialogueType.SingleChoice //переопределяю переменную userData на текущий тип диалога
                },
                new SearchTreeEntry(new GUIContent("Кнопки выбора"))
                {
                    level = 2,
                    userData = DialogueType.MultipleChoice //переопределяю переменную userData на текущий тип диалога
                },
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context) //когда выбираю/ нажимаю на любой элемент, который нашла через меню поиска (от интерфейса ISearchWindowProvider)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true); //преобразуем глобальные координаты в локальные с помощью метода GetLocalMousePosition + говорю, что ноду пытаюсь создать из окна поиска
            switch (SearchTreeEntry.userData) //сравниваю переменную userData, в которой указан текущий тип ноды (диалог или кнопки выбора)
            {
                case DialogueType.SingleChoice:
                {
                    SingleChoiceNode singleChoiceNode = (SingleChoiceNode)graphView.CreateNode(DialogueType.SingleChoice, localMousePosition); //создаю ноду, на которую кликнула в контекстном меню
                    graphView.AddElement(singleChoiceNode); //в graphView (переговорную) добавляю ноду, на которую кликнула в контекстном меню
                    return true; //возвращаю true, чтобы нужная нода появлялась только по клику на кнопку, а не сразу как только я наведусь на тип ноды в контекстном меню
                }
                case DialogueType.MultipleChoice:
                {
                    MultipleChoiceNode multipleChoiceNode = (MultipleChoiceNode)graphView.CreateNode(DialogueType.MultipleChoice, localMousePosition); //создаю ноду, на которую кликнула в контекстном меню
                    graphView.AddElement(multipleChoiceNode); //в graphView (переговорную) добавляю ноду, на которую кликнула в контекстном меню

                    return true; //возвращаю true, чтобы нужная нода появлялась только по клику на кнопку, а не сразу как только я наведусь на тип ноды в контекстном меню
                }
                default: //по стандарту
                {
                    return false; //возвращаю false
                }
            }
        }
    }
}

