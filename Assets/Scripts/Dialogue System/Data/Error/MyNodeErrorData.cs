using MaryDialogSystem.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.Data.Error
{
    public class MyNodeErrorData //для вывода данных об ошибках в нодах
    {
        public MyErrorData ErrorData { get; set; } //данные об ошибках (в виде цвета нод)
        public List<MyNode> Nodes { get; set; } //текущие ноды
        public MyNodeErrorData() //в конструкторе инициализируем переменные
        {
            ErrorData = new MyErrorData();
            Nodes = new List<MyNode>();
        }
    }
}

