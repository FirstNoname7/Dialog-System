using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.Data.Save
{
    public class GraphSaveDataSO: ScriptableObject //сохран€шка данных в виде Scriptable Object (SO)
    {
        [SerializeField] public string FileName { get; set; }
        [SerializeField] public List<NodeSaveData> Nodes { get; set; }
        [SerializeField] public List<string> OldNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Nodes = new List<NodeSaveData>();
        }
    }

}
