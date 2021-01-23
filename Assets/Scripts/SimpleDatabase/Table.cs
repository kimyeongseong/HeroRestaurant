using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace SimpleDatabase
{
    public class Table<T> : BaseTable where T : new()
    {
    #if UNITY_EDITOR
        [ShowInInspector, TitleGroup("IO Utility")]
        private TextAsset xmlAsset = null;
        [ShowInInspector, TitleGroup("IO Utility")]
        private string    primarySign = string.Empty;

        private string primaryKey = string.Empty;

        [TitleGroup("IO Utility"), Button("Load From Xml")]
        private void OnLoadFromXml()
        {
            if (xmlAsset != null)
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xmlAsset.text);

                XmlNode root = document.LastChild;
                XmlNodeList nodeList = root.ChildNodes;

                table = new Dictionary<string, T>(nodeList.Count);

                primaryKey = nodeList[0].
                             ChildNodes[0].
                             Cast<XmlNode>().
                             Where(x => x.Name.StartsWith(primarySign)).
                             Select(x => x.Name.Remove(0)).
                             FirstOrDefault();

                primaryKey = string.IsNullOrEmpty(primaryKey) ?
                             nodeList[0].ChildNodes[0].Name : primaryKey;

                var fields = typeof(T).GetFields();
                foreach (XmlNode nodes in nodeList)
                {
                    object row = new T();

                    int i = 0;
                    foreach (XmlNode node in nodes)
                    {
                        fields[i].SetValue(row, CastTo(node.InnerText, fields[i].FieldType));
                        i += 1;
                    }

                    table.Add(nodes[primaryKey].InnerText, (T)row);
                }
            }
            else
                Debug.LogWarning("Xml Template(XmlAsset) Not Exist");
        }

        private object CastTo(string value, System.Type type)
        {
            if (type.IsEnum)
                return System.Enum.Parse(type, value);
            else if (type.IsArray)
                return value.Split(',').Select(item => System.Convert.ChangeType(item, type)).ToArray();
            else
                return System.Convert.ChangeType(value, type);
        }
    #endif

        [Title("Data Table")]
        [OdinSerialize, DictionaryDrawerSettings(KeyLabel = "Primary Keys", ValueLabel = "Rows")]
        private Dictionary<string, T> table = null;

        public T[] Rows { get { return table.Values.ToArray(); } }

        public T Select(string primaryKey)
        {
            Debug.AssertFormat(table.ContainsKey(primaryKey), "Table::Select - {0} Row is Not Exist", primaryKey);

            T row = table[primaryKey];
            return row;
        }
    }
}