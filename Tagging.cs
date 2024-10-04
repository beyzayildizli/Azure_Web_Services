/**
 * @file: Tagging.cs
 * @description: URL'sini aldığı görüntüyü kenarlarında bulunan
 * beyaz boşlukları kırptıktan sonra, Azure.AI.Vision servislerinden
 * yararlanarak etiketler ve bu etiketleri döndürür.
 * @assignment: Image Tagging With Azure AI Service
 * @date: 24.07.2024
 * @author: beyza.yildizli@aygaz.com.tr & meryem.arslan@aygaz.com.tr & kubra.duran@aygaz.com.tr
 */

using System;
using Azure;
using Azure.AI.Vision.ImageAnalysis; //dotnet add package Azure.AI.Vision.ImageAnalysis --prerelease  
using SixLabors.ImageSharp; //dotnet add package SixLabors.ImageSharp
using SixLabors.ImageSharp.PixelFormats;

class Tagging
{
    /* Bu fonksiyon bir görüntüyü URL'den indirir, beyaz satır ve sütunları kaldırır,
    analiz yaparak etiketler oluşturur, ve bu etiketleri bir string olarak döndürür. */
//     public static async Task<string> PictureTaggingAsync(string imageUrl)
//     {
//         try
//         {
//            /* Görüntüyü işledikten sonra kaydedilecek dosyanın yolunu belirtir */
//         string outputImagePath = "copyImage.jpg";

//         /* DownloadImageFromUrl fonksiyonunu kullanarak verilen URL'den görüntüyü indirir
//         ve Image<Rgba32> türünde bir nesne olarak inputImage değişkenine atar.
//         await anahtar kelimesi, bu işlemin tamamlanmasını bekler. */
//         Image<Rgba32> inputImage = await DownloadImageFromUrl(imageUrl);

//         /* RemoveWhiteRowsAndColumns fonksiyonunu kullanarak inputImage görüntüsünden
//         beyaz satır ve sütunları kaldırır. İşlenmiş görüntüyü outputImage değişkenine atar. */
//         Image<Rgba32> outputImage = RemoveWhiteRowsAndColumns(inputImage);

//         /* outputImage'i belirtilen outputImagePath yolunda kaydeder. Bu adımda görüntü diske yazılır. */
//         outputImage.Save(outputImagePath);

//         /* TagListFromPath fonksiyonunu kullanarak outputImagePath'deki görüntüden etiketler alır.
//         string.Join(", ", tags) ifadesi, etiketleri bir araya getirir ve virgüllerle ayırarak bir string oluşturur. */
//         string[] tags = await TagListFromPath(outputImagePath);
//         string tagsString = string.Join(", ", tags);

//         /* Eğer outputImagePath altında bir dosya varsa, bu dosyayı siler.
//         Bu, geçici olarak kaydedilen görüntü dosyasının temizlenmesini sağlar. */
//         if (File.Exists(outputImagePath))
//         {
//             File.Delete(outputImagePath); // outputu sil
//         }

//         return tagsString;
//         }
//         catch (Exception ex)
//         {
//     throw;
//             Console.WriteLine($"An error occurred: {ex.Message}");
//             return $"Hata : {ex.Message} {ex.StackTrace}";
//         }
//     }

    public static async Task<List<string>> PictureTaggingAsync(string imageUrl)
    {
        try
        {
           
        /* Görüntüyü işledikten sonra kaydedilecek dosyanın yolunu belirtir */
        string outputImagePath = "copyImage.jpg";

        /* DownloadImageFromUrl fonksiyonunu kullanarak verilen URL'den görüntüyü indirir
        ve Image<Rgba32> türünde bir nesne olarak inputImage değişkenine atar.
        await anahtar kelimesi, bu işlemin tamamlanmasını bekler. */
        Image<Rgba32> inputImage = await DownloadImageFromUrl(imageUrl);

        /* RemoveWhiteRowsAndColumns fonksiyonunu kullanarak inputImage görüntüsünden
        beyaz satır ve sütunları kaldırır. İşlenmiş görüntüyü outputImage değişkenine atar. */
        Image<Rgba32> outputImage = RemoveWhiteRowsAndColumns(inputImage);

        /* outputImage'i belirtilen outputImagePath yolunda kaydeder. Bu adımda görüntü diske yazılır. */
        outputImage.Save(outputImagePath);

        /* TagListFromPath fonksiyonunu kullanarak outputImagePath'deki görüntüden etiketler alır.
        string.Join(", ", tags) ifadesi, etiketleri bir araya getirir ve virgüllerle ayırarak bir string oluşturur. */
        string[] tags = await TagListFromPath(outputImagePath);
        // string tagsString = string.Join(", ", tags);

        /* Eğer outputImagePath altında bir dosya varsa, bu dosyayı siler.
        Bu, geçici olarak kaydedilen görüntü dosyasının temizlenmesini sağlar. */
        if (File.Exists(outputImagePath))
        {
            File.Delete(outputImagePath); // outputu sil
        }

        return tags.ToList();
        }
        catch (Exception ex)
        {
            throw; 
        }
    }

/* Bu fonksiyon, bir dosya yolundan görüntüyü alır, Azure Image Analysis API kullanarak
    görüntüyü analiz eder ve görüntüye ilişkin etiketleri bir dizi olarak döndürür.
    Asenkron yapısı sayesinde ağ isteği yapılırken uygulamanın diğer işlemlerine devam edebilir. */

    static async Task<string[]> TagListFromPath(string path)
    {
        string endpoint = ""; //add your endpoint url here
        string key = "";  //add your api key here

        /* ImageAnalysisClient, Azure Image Analysis API'si ile iletişim kurmak için kullanılır.
        endpoint ve key ile birlikte AzureKeyCredential kullanılarak kimlik doğrulama sağlanır. */
        ImageAnalysisClient client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));

        /* FileStream, belirtilen path'ten bir dosya açar. FileMode.Open parametresi
        dosyanın mevcut olduğundan emin olur ve yalnızca okuma modunda açar. using bloğu,
        dosya işlemi bitince FileStream'in otomatik olarak kapanmasını sağlar. */
        using FileStream stream = new FileStream(path, FileMode.Open);

        /* AnalyzeAsync yöntemi, FileStream'den BinaryData oluşturur ve bu veriyi Azure Image
        Analysis API'sine gönderir. VisualFeatures.Tags parametresi, API'den etiketlerin alınmasını
        belirtir. await anahtar kelimesi, bu işlemin tamamlanmasını bekler. */
        ImageAnalysisResult result = await client.AnalyzeAsync(
            BinaryData.FromStream(stream),
            VisualFeatures.Tags);

        /* ImageAnalysisResult nesnesinin Tags özelliği, API'den dönen etiketleri içerir.
        Her bir DetectedTag nesnesinin Name özelliği tags listesine eklenir. */
        List<string> tags = new List<string>();
        foreach (DetectedTag tag in result.Tags.Values)
        {
            tags.Add(tag.Name);
        }
        
        return tags.ToArray();
    }



    /* Bu fonksiyon bir URL'den görüntü indirir ve bu verileri bir akışa dönüştürdükten
    sonra Image<Rgba32> türünde bir görüntü nesnesine dönüştürür. Asenkron yapısı sayesinde,
    ağ isteği yapılırken uygulamanın diğer işlemlerine devam edebilir. */
    static async Task<Image<Rgba32>> DownloadImageFromUrl(string url)
    {
        /* HttpClient sınıfı, HTTP istekleri yapmak için kullanılır.
        using bloğu, HttpClient nesnesinin işimiz bitince doğru bir
        şekilde serbest bırakılmasını sağlar. */
        using (HttpClient client = new HttpClient())
        {
            /* GetByteArrayAsync yöntemi, belirtilen URL'den görüntü
            verilerini bir byte dizisi (byte[]) olarak asenkron bir
            şekilde alır. await anahtar kelimesi, bu işlemin
            tamamlanmasını bekler. */
            byte[] imageBytes = await client.GetByteArrayAsync(url);

            /* MemoryStream, byte dizisini bir akış (stream) olarak işlemek için
            kullanılır. Bu, byte dizisinin bir akış nesnesine dönüştürülmesini sağlar. */
            using (var ms = new MemoryStream(imageBytes))
            {
                /* Image.Load<Rgba32>(ms) ifadesi, ImageSharp kütüphanesinin bir
                parçasıdır ve MemoryStream'den bir Image<Rgba32> nesnesi oluşturur.
                Bu, byte dizisini bir görüntü nesnesine dönüştürür. */
                return Image.Load<Rgba32>(ms); 
            }
        }
    }

/* İşlenecek görüntüyü alır ve görüntüden beyaz satır ve
    sütunları kaldırarak daha küçük bir görüntü oluşturur. */
    static Image<Rgba32> RemoveWhiteRowsAndColumns(Image<Rgba32> image)
    {
        /* Görüntünün genişlik ve yüksekliği alınır. */
        int width = image.Width;
        int height = image.Height;

        /* Görüntüdeki her satır ve sütunun beyaz olup olmadığını
        saklamak için iki boolean dizi tanımlanır. */
        bool[] whiteRows = new bool[height];
        bool[] whiteColumns = new bool[width];

        /* Her satırın beyaz olup olmadığını kontrol eder. Eğer bir satırda
        herhangi bir piksel beyaz değilse, bu satırın beyaz olmadığını işaretler. */
        for (int y = 0; y < height; y++)
        {
            bool isWhiteRow = true;
            for (int x = 0; x < width; x++)
            {
                Rgba32 pixelColor = image[x, y];
                if (pixelColor.R != 255 || pixelColor.G != 255 || pixelColor.B != 255)
                {
                    isWhiteRow = false;
                    break;
                }
            }
            whiteRows[y] = isWhiteRow;
        }

        /* Her sütunun beyaz olup olmadığını kontrol eder. Eğer bir sütunda
        herhangi bir piksel beyaz değilse, bu sütunun beyaz olmadığını işaretler. */
        for (int x = 0; x < width; x++)
        {
            bool isWhiteColumn = true;
            for (int y = 0; y < height; y++)
            {
                Rgba32 pixelColor = image[x, y];
                if (pixelColor.R != 255 || pixelColor.G != 255 || pixelColor.B != 255)
                {
                    isWhiteColumn = false;
                    break;
                }
            }
            whiteColumns[x] = isWhiteColumn;
        }

        /* Beyaz olmayan sütun ve satır sayısını sayarak
        yeni görüntünün boyutlarını hesaplar. */
        int newWidth = 0;
        int newHeight = 0;

        for (int x = 0; x < width; x++)
        {
            if (!whiteColumns[x])
            {
                newWidth++;
            }
        }



        for (int y = 0; y < height; y++)
        {
            if (!whiteRows[y])
            {
                newHeight++;
            }
        }



        /* Yeni bir görüntü oluşturur. */
        var newImage = new Image<Rgba32>(newWidth, newHeight);

        /* Beyaz satır ve sütunları içermeyen yeni görüntüyü doldurur. */
        int newX = 0;
        for (int x = 0; x < width; x++)
        {
            if (!whiteColumns[x])
            {
                int newY = 0;
                for (int y = 0; y < height; y++)
                {
                    if (!whiteRows[y])
                    {
                        newImage[newX, newY] = image[x, y];
                        newY++;
                    }
                }
                newX++;
            }
        }

        return newImage;
    }
}