// -- JSON objects definition -- \\

[System.Serializable]

public class ImageItem
{
    public string _id;
    public string imageFile;
    public float azimuth;
    public double latitude;
    public double longitude;
    public string timestamp;
}

public class ImageRootItem<ImageItem> //wrapper para poder recibirlo como array
{
    public ImageItem[] Items;
}