using UnityEngine;
using System.Runtime.InteropServices;

public class IOSShareSheet : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _ShowShareSheet(string filePath);
#endif

    public static void ShowShareSheet(string filePath)
    {
#if UNITY_IOS && !UNITY_EDITOR
        _ShowShareSheet(filePath);
#else
        Debug.Log("ShareSheet is only available on iOS devices.");
#endif
    }
}