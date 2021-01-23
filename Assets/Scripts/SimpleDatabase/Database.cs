using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace SimpleDatabase
{
    [CreateAssetMenu(fileName = "Database", menuName = "SimpleDatabase/Database", order = -1)]
    public class Database : SerializedScriptableObject
    {
        private static Database instance = null;
        public  static Database Instance
        {
            get
            {
                if (!instance)
                    instance = Resources.Load<Database>("SimpleDatabase/Database");
                return instance;
            }
        }

        [OdinSerialize]
        private Dictionary<string, BaseTable> tables = new Dictionary<string, BaseTable>();

        public Table<T> Select<T>(string tableName) where T : new()
        {
            Debug.AssertFormat(tables.ContainsKey(tableName), "Database::Select - {0} table is not exist", tableName);

            return tables[tableName] as Table<T>;
        }

    #if UNITY_EDITOR
        [Button]
        private void FindAllTables()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(BaseTable)));

            tables = new Dictionary<string, BaseTable>(guids.Length);
            foreach (var guid in guids)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                BaseTable table = UnityEditor.AssetDatabase.LoadAssetAtPath<BaseTable>(assetPath);
                tables.Add(table.name, table);
            }
        }
    #endif
    }
}
