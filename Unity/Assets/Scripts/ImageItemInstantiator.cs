using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class ImageItemInstantiator : MonoBehaviour
{
    // Paths
    private const string PATH_Images = "DB/ImageItems/";

    // Parameters
    public Camera mainCamera;
    public GPStoUTM utmCalculatorScript;
    public GameObject imageItemPrefab;
    public GameObject imageItemUI;

    [HideInInspector]
    public List<ImageItem> imageItemList = new List<ImageItem>();

    public void ReadJSONdata()
    {
        ImageItem imageItem;

        string db_path = Path.Combine(Application.persistentDataPath, PATH_Images);

        foreach (string filePath in Directory.GetFiles(db_path))
        {
            StreamReader reader = new StreamReader(filePath);
            imageItem = JsonUtility.FromJson<ImageItem>(reader.ReadToEnd());

            //imageItemList.Add(imageItem);

            //Instantiate items
            InstantiateImageItem(imageItem, db_path);

            reader.Close();
        }
    }
    
    public void InstantiateImageItem(ImageItem imageItem, string db_path)
    {
        float[] position = utmCalculatorScript.calculateUnityPosition(imageItem.latitude,imageItem.longitude);

        Vector3 positionVector = new Vector3(position[0], 0, position[1]);
        Quaternion azimuthVector = Quaternion.Euler(new Vector3(0, imageItem.azimuth, 0));

        GameObject imageItemInstance = Instantiate(imageItemPrefab, positionVector, azimuthVector);
        imageItemInstance.transform.parent = gameObject.transform;

        //Instantiate images as textures
        Texture2D texture2d = new Texture2D(899, 1599); //899x1599 image
        string texture_path = Path.Combine(db_path, imageItem.imageFile.Replace(':', '-'));

        byte[] image_bytes = File.ReadAllBytes(texture_path);
        texture2d.LoadImage(image_bytes);

        RawImage rawImage = imageItemInstance.transform.Find("ImageFile/RawImage").GetComponent<RawImage>() as RawImage; //Search for RawImage component
        rawImage.texture = texture2d;

        //Set camera usde for events
        Canvas canvas = imageItemInstance.transform.Find("ImageFile").GetComponent<Canvas>() as Canvas; //Search for Canvas component
        canvas.worldCamera = mainCamera;
    }
}