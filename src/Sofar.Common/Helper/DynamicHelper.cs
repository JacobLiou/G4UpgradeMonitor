using System.Text.Json;

namespace Sofar.Common.Helper
{
    /// <summary>
    /// 参数转换辅助
    /// </summary>
    public static class DynamicHelper
    {
        /// <summary>
        /// 动态属性添加
        /// </summary>
        /// <param name="args"></param>
        /// <param name="dynObj"></param>
        public static void AddDynamicProperties(IDictionary<string, object> args, dynamic dynObj)
        {
            var dict = (Dictionary<string, object>)JsonSerializer.Deserialize<Dictionary<string, object>>(dynObj.ToString());
            foreach (var keyValues in dict)
            {
                if (!args.ContainsKey(keyValues.Key))
                    args.Add(keyValues.Key, keyValues.Value.ToString()!);
            }
        }

        public static Dictionary<string, string> GetDynamicProperties(dynamic dynObj)
        {
            Dictionary<string, string> result = new();
            var dict = (Dictionary<string, object>)JsonSerializer.Deserialize<Dictionary<string, object>>(dynObj.ToString());
            foreach (var keyValues in dict)
            {
                result.Add(keyValues.Key, keyValues.Value.ToString()!);
            }

            return result;
        }
    }
}