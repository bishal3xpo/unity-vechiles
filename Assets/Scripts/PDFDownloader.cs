using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PDFtoImage;
using SkiaSharp;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

#region SKBitmap converter
public static class SkiaSharpExtensions
{
    public static Texture2D ToTexture2D(this SKBitmap skBitmap)
    {
        Texture2D texture = new Texture2D(skBitmap.Width, skBitmap.Height, TextureFormat.RGBA32, false);
        byte[] pixels = skBitmap.Bytes;
        texture.LoadRawTextureData(pixels);
        texture.Apply();
        return texture;
    }
}
#endregion

public class PDFDownloader : MonoBehaviour
{
    [SerializeField] private string pdfFileName;
    [SerializeField] private GameObject rawImagePrefab; // Prefab with RawImage component
    [SerializeField] private RectTransform contentTransform; // Content transform of the Scroll View
    [SerializeField] private string imagesSaveDirectory = "SavedImages"; // Directory to save images
    [SerializeField] GameObject ScrollView;

    public IEnumerator RenderImages(IEnumerable<SKBitmap> images)
    {
        float totalHeight = 0f;
        List<float> heights = new List<float>();

        int imageIndex = 0;
        foreach (var image in images)
        {
            Texture2D texture = image.ToTexture2D();

            GameObject rawImageObject = Instantiate(rawImagePrefab, contentTransform);
            RawImage rawImage = rawImageObject.GetComponent<RawImage>();
            rawImage.texture = texture;

            float contentWidth = contentTransform.rect.width;
            float aspectRatio = (float)texture.height / texture.width;
            float adjustedHeight = contentWidth * aspectRatio;

            RectTransform rawImageRect = rawImageObject.GetComponent<RectTransform>();
            rawImageRect.sizeDelta = new Vector2(contentWidth, adjustedHeight);

            heights.Add(adjustedHeight);
            totalHeight += adjustedHeight * 2f;

            // Save the image to a file
            SaveTextureAsPNG(texture, imageIndex);
            imageIndex++;

        }

        contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, totalHeight);

        float currentY = 0f;
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            RectTransform childRect = contentTransform.GetChild(i).GetComponent<RectTransform>();
            childRect.anchoredPosition = new Vector2(0, -currentY);
            currentY += heights[i];
        }

        ScrollView.SetActive(true);
        yield return null;
    }

    private void SaveTextureAsPNG(Texture2D texture, int index)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, imagesSaveDirectory);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, $"image_{index}.png");
        byte[] imageData = texture.EncodeToPNG();

        File.WriteAllBytes(filePath, imageData);
        Debug.Log($"Image saved at: {filePath}");
    }

    public void InvokeDownloadPDF()
    {
        Debug.Log("PDF: Invoked InvokeDownloadPDF");
        StartCoroutine(DownloadPDF("https://ms-3xpo-wp.s3.ap-southeast-1.amazonaws.com/wp-content/uploads/sites/6/2022/11/03203235/dummy.pdf", "downloadedFromAWS.pdf"));
    }

    public void InvokeDownloadPDFFirebase()
    {
        Debug.Log("PDF: Invoked InvokeDownloadPDFFirebase");
        StartCoroutine(DownloadPDF("https://firebasestorage.googleapis.com/v0/b/com-threexpo-3xpoverse.appspot.com/o/test-files%2FAMSBB-2022-23.pdf?alt=media&token=69c56f39-b05a-4377-966a-54713e0a7b7b", "downloadedFromFirebase.pdf"));
    }

    private IEnumerator DownloadPDF(string pdfUrl, string fileName)
    {
        Debug.Log("PDF: Invoked DownloadPDF");

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
                    IOSShareFile.ShareFile(localFilePath);
#endif

                    // Load and render PDF as images
                    TryLoadPDFToImage(localFilePath);
                }
                catch (Exception e)
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

    public void TryLoadPDFToImage(string pdfFilePath)
    {
        if (File.Exists(pdfFilePath))
        {
            byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
            string base64String = Convert.ToBase64String(pdfBytes);

            Debug.Log("Base64 String: " + base64String);

            RenderOptions options = new();

            var images = Conversion.ToImages(base64String, options: new(Dpi: 300, Rotation: PdfRotation.Rotate0, WithAspectRatio:true));
            StartCoroutine(RenderImages(images));
        }
        else
        {
            Debug.LogError("PDF file not found at path: " + pdfFilePath);
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
        downloadPath = Path.Combine(Application.persistentDataPath, "Downloads");
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

        return Path.Combine(downloadPath, fileName);
    }
}
