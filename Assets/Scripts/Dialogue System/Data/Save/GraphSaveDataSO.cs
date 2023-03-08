using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.Data.Save
{
    public class GraphSaveDataSO: ScriptableObject //���������� ������ � ���� Scriptable Object (SO)
    {
        [field: SerializeField] public string FileName { get; set; } //����� ������������� ������� ����� ��������� ������ [field: SerializeField], ���� ��� ����������, ������� ����� ����, ���������� � ���������� � SO
        [field: SerializeField] public List<NodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Nodes = new List<NodeSaveData>();
        }
    }

}
