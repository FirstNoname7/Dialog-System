using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.Data.Save
{
    public class GraphSaveDataSO: ScriptableObject //сохраняшка данных в виде Scriptable Object (SO)
    {
        [field: SerializeField] public string FileName { get; set; } //перед модификатором доступа нужно поставить именно [field: SerializeField], чтоб все переменные, которые здесь есть, выводились в инспекторе в SO
        [field: SerializeField] public List<NodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Nodes = new List<NodeSaveData>();
        }
    }

}
