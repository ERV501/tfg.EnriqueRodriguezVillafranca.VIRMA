using UnityEngine;
using System.Net.Http;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class DataBaseController : MonoBehaviour
{
    // --  REST Api Urls -- \\
    private const string URL_Images = "http://192.168.1.135:3000/images";
    private const string URL_IDs = "http://192.168.1.135:3000/images/IDs";

    // --  Paths -- \\
    private const string PATH_Images = "DB/ImageItems/";

    // --  Globales -- \\
    private static string received_IDs = "", current_IDs = "", received_ImageFiles = "";

    // --  Controllers -- \\
    [SerializeField]
    ImageItemInstantiator imageItemInstantiator;


    // --  Start is called before the first frame update -- \\
    void Start()
    {
        //Actualización inicial DB
        GetAllItems();

        //Comprobar si es necesario actualizar la DB cada 10 min
        InvokeRepeating("CheckIDs", 600, 600); // Metodo a invocar, tiempo para invocar, periodo
    }

    // -- Funcionalidad DB controller -- \\
    private void GetAllItems()
    {
        using(var client = new HttpClient()){
            HttpResponseMessage images = client.GetAsync(URL_Images).Result;
            images.EnsureSuccessStatusCode(); //comprobar que se ha realizado correctamente
            string json = images.Content.ReadAsStringAsync().Result; //leer mensaje como string

            // Deserializamos el json array que recibimos
            ImageItem[] imageItemArray = DeserializeJsonArray<ImageItem>("{\"Items\":" + json.ToString() + "}"); //es necesario que todo el json se reciba como un solo objeto

            string imagePath = Path.Combine(Application.persistentDataPath, PATH_Images); //Formatear path correctamente de acuerdo al OS actual
            string uploadPath = Path.Combine(imagePath, "uploads"); //Formatear path correctamente de acuerdo al OS actual

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string filePath, jsonItemData, formattedImageFile;
            string[] currentIdList = new string[imageItemArray.Length];
            string[] receivedImageFilesList = new string[imageItemArray.Length];

            for (int i = 0; i < imageItemArray.Length; i++)
            {
                //store JSON
                filePath = Path.Combine(imagePath, imageItemArray[i]._id);

                jsonItemData = JsonUtility.ToJson(imageItemArray[i], true); //formatear como JSON
                File.WriteAllText(filePath + ".json", jsonItemData); //Crear y escribir JSON, si ya existe, se sobreescribe (para evitar incoherencias con la DB en caso de Updates, se permite siempre)

                currentIdList[i] = imageItemArray[i]._id;

                //store image uploads
                StartCoroutine(GetTextureFile(imagePath, imageItemArray[i].imageFile));

                formattedImageFile = imageItemArray[i].imageFile.Replace(':', '-'); //Reemplazar caracteres no permitidos por windows
                formattedImageFile = formattedImageFile.Replace('/', '\\'); //Reemplazar barra del path

                receivedImageFilesList[i] = Path.Combine(imagePath, formattedImageFile); //Path de imagenes que hemos recibido
            }

            current_IDs = string.Join(",", currentIdList);
            received_ImageFiles = string.Join(",", receivedImageFilesList);

            imageItemInstantiator.ReadJSONdata(); //Instanciar items

        }
    }

    private void CheckIDs()
    {
        GetAllIDs();

        if (!received_IDs.Equals(current_IDs))
        {
            GetAllItems();
        }
    }

    private void GetAllIDs()
    {
        using (var client = new HttpClient()){
            var ids = client.GetAsync(URL_IDs).Result;
            string json = ids.Content.ReadAsStringAsync().Result;

            // Deserializamos el json array que recibimos
            ImageItem[] imageItemArray = DeserializeJsonArray<ImageItem>("{\"Items\":" + json.ToString() + "}"); //es necesario que todo el json se reciba como un solo objeto
            
            string[] receivedIdList = new string[imageItemArray.Length];

            for (int i = 0; i < imageItemArray.Length; i++)
            {
                receivedIdList[i] = imageItemArray[i]._id;
            }

            received_IDs = string.Join(",", receivedIdList);
        }
    }


    IEnumerator GetTextureFile(string imagePath, string fileName)
    {
        UnityWebRequest url = UnityWebRequestTexture.GetTexture(Path.Combine(URL_Images,fileName));
        yield return url.SendWebRequest();

        if (url.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(url.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)url.downloadHandler).texture;
            byte[] bytes = myTexture.EncodeToPNG();

            string path = Path.Combine(imagePath,fileName.Replace(':', '-')); //Path y nombre del archivo imagen, sustituir ':' pues no está permitido en nombres de archivo de Windows
            File.WriteAllBytes(path, bytes);
        }
    }

    // -- JSON objects methods -- \\

    public static ImageItem[] DeserializeJsonArray<ImageItem>(string json)
    {
        ImageRootItem<ImageItem> rootItem = JsonUtility.FromJson<ImageRootItem<ImageItem>>(json);
        return rootItem.Items;
    }

}
