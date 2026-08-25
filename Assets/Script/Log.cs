using UnityEngine;

public class Log : MonoBehaviour
{
    /// <summary>
    /// string 대신 nameof() 쓸것 
    /// </summary>
    /// <param name="className"></param>
    /// <param name="methodName"></param>
    /// <param name="targetName"></param>
    public static void LogNull(string className, string methodName, string targetName = null)
    {
        if(targetName == null)
        {
            Debug.LogError($"[{className}] / [{methodName}] 인스펙터 null입니다.");
        }
        else
        {
            Debug.LogError($"[{className}] / [{methodName}]  {targetName}이 null입니다.");
        }

    }


}