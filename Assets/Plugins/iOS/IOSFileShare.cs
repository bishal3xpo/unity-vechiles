using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

public class IOSShareFile : MonoBehaviour
{
    // Import the native iOS sharing function
    [DllImport("__Internal")]
    private static extern void _NativeShare(string filePath);

    // Function to share a file
    public static void ShareFile(string filePath)
    {
        #if UNITY_IOS && !UNITY_EDITOR
        if (File.Exists(filePath))
        {
            _NativeShare(filePath);
        }
        else
        {
            Debug.LogError("File does not exist at path: " + filePath);
        }
        #else
        Debug.Log("iOS sharing is only available on iOS devices. File path: " + filePath);
        #endif
    }
}