using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;

public class PDFDownloader : MonoBehaviour
{
    public void InvokeDownloadPDF()
    {
        Debug.Log("PDF: Invoked InvokeDownloadPDF");
        StartCoroutine(DownloadPDF("https://ms-3xpo-wp.s3.ap-southeast-1.amazonaws.com/wp-content/uploads/sites/6/2022/11/03203235/dummy.pdf", "downloadedFromAWS.pdf"));
    }

    public void InvokeDownloadPDFFirebase()
    {
        Debug.Log("PDF: Invoked InvokeDownloadPDFFirebase");
        StartCoroutine(DownloadPDF("https://firebasestorage.googleapis.com/v0/b/com-threexpo-3xpoverse.appspot.com/o/test-files%2Fdummy1.pdf?alt=media&token=8d9cb6cd-113a-4b50-9c7f-08a6ff58fdef", "downloadedFromFirebase.pdf"));
    }

    IEnumerator DownloadPDF(string pdfUrl, string fileName)
    {
        Debug.Log("PDF: invoked DownloadPDF");

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            yield return new WaitForSeconds(0.5f);
        }
#endif

        using (UnityWebRequest www = UnityWebRequest.Get(pdfUrl))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                string localFilePath = GetDownloadPath(fileName);

                try
                {
                    File.WriteAllBytes(localFilePath, www.downloadHandler.data);
                    Debug.Log("PDF file downloaded and saved locally at: " + localFilePath);
#if UNITY_IOS
                    if (File.Exists(localFilePath))
                    {
                        IOSShareSheet.ShowShareSheet(localFilePath); // Call method to show ShareSheet
#endif
                    }
                    else
                    {
                        Debug.Log("FileDoes not exist");
                    }

                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error saving PDF: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("Error downloading PDF: " + www.error);
            }
        }
    }



    private string GetDownloadPath(string fileName)
    {
        string downloadPath = "";

#if UNITY_ANDROID
        AndroidJavaClass androidEnvironment = new AndroidJavaClass("android.os.Environment");
        AndroidJavaObject downloadsDirectory = androidEnvironment.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", androidEnvironment.GetStatic<string>("DIRECTORY_DOWNLOADS"));
        downloadPath = downloadsDirectory.Call<string>("getAbsolutePath");
#elif UNITY_IOS
        downloadPath = Path.Combine(Application.persistentDataPath, fileName);
        if (!Directory.Exists(downloadPath))
        {
            Directory.CreateDirectory(downloadPath);
        }
#else
        downloadPath = Path.Combine(Application.persistentDataPath, "Downloads");
        if (!Directory.Exists(downloadPath))
        {
            Directory.CreateDirectory(downloadPath);
        }
#endif

        return downloadPath;
    }
}
