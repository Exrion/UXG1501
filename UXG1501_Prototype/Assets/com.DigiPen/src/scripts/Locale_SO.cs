using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptables/Locale SO")]
public class Locale_SO : ScriptableObject
{
    public UDictionary<string, string> locale_en = new UDictionary<string, string>();
    public UDictionary<string, string> locale_jp = new UDictionary<string, string>();

    public static string GetValue(UDictionary<string, string> dict, string key) => dict.FirstOrDefault(x => x.Key == key).Value;
}
