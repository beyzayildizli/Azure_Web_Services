/**
 * @file: HukukPortal.cs
 * @description: Azure AI kullanarak resimlerden OCR yoluyla metin çıkarma ve 
 * hukuk belgelerini özetleme işlemleri gerçekleştiren bir sınıf.
 * @assignment: Hukuk portalında, görüntü üzerindeki metinleri
 * OCR ile çıkarma ve OpenAI ile hukuki belgeleri özetleme
 * @date: 15.08.2024
 * @author: beyza.yildizli@aygaz.com.tr & meryem.arslan@aygaz.com.tr & kubra.duran@aygaz.com.tr
 */
 
using Azure;
using Azure.AI.OpenAI;
using Azure.AI.Vision.ImageAnalysis;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
namespace ConsoleApp3
{
    internal class HukukPortal
    {
        public static async Task<string> ExtractImageTextWithOCR(string imageUrl)
        {

            byte[] imageData = await GetImageBytesAsync(imageUrl);
           
            StringBuilder sb = new StringBuilder();
            using (MemoryStream streamData = new MemoryStream(imageData))
            {
                string endpoint = ""; //add your endpoint url here
                string key = "";  //add your api key here

                ImageAnalysisClient client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));

                ImageAnalysisResult result = client.Analyze(
                    BinaryData.FromStream(streamData),
                    VisualFeatures.Read);

                foreach (var line in result.Read.Blocks.SelectMany(block => block.Lines))
                {
                    sb.AppendLine(line.Text);
                }
            }
            return sb.ToString();
        }

        public static async Task<string> documentSummary(string docuemtnContent)
        {
            string result = string.Empty;
            OpenAIClient clientai = new OpenAIClient(
                new Uri(""),
                new AzureKeyCredential(""));
            try
            {
                Response<ChatCompletions> responseWithoutStream = await clientai.GetChatCompletionsAsync(
                "gpt-35-turbo",
                new ChatCompletionsOptions()
                {
                    Messages =
                    {
                        new ChatMessage(ChatRole.System, $@"
                            Sen hukuk departmanı için hukuk dökümanlarını özetleyen yapay zeka asistanısın. 
                            "
                        ),

                        new ChatMessage(ChatRole.User, $@"
                            Bu hukuk dosyasını analiz et ve aşağıdaki soruları ayrıntılı ve net bir şekilde yanıtla:
                            1. **Dosyanın Konusu ve Tarihleri**:
                               - Dosyanın konusu nedir?
                               - Dosyanın tarihleri nelerdir?
                               - Dökümanda geçen tarih bilgileri nelerdir? 

                            2. **Taraflar**:
                               - Tüm taraflar kimlerdir, isimleri ve birbirleri ile olan ilişkileri nelerdir? Kimlik numarası gibi detayları verme.
                               - Dosyanın muhatabı kşiler kimlerdir? Bu kişileri belirtirken isimlerinin kullanılmasına dikkat et.

                            3. **Avukatlar**:
                               - Avukatlar kimlerdir?
                               - Hangi tarafın avukatıdır?

                            4. **Dosyanın İddiası**:
                               - Dosyanın iddiası nedir?

                            5. **Olay Akışı**:
                               - Olay akışı nasıl başlamıştır?
                               - Hangi aksiyonlar alınmıştır?
                               - Hangi sonuçlar olmuştur?

                            6. **Talep**:
                               - Talep nedir?

                            Aşağıdaki dökümanın bir sözleşme olduğunu anlarsan şu ana kadar verilen promptları unut ve sadece sözleşmeyi özetle. Sözleşmede geçen ceva maddelerinndeki ceza tutarlarını mutlaka özete ekle.

                            Lütfen yukarıdaki maddeler halinde verilen bilgileri çıkar ve eksiksiz bir şekilde en az 15 cümle ile özetle.

                            Dosya Tarihi: 01.01.2024
                            Dosya Numarası: 123456
                            Dosya içeriği:
                            {docuemtnContent}

                            Detaylı paragraf özeti:  ")
                    },
                    Temperature = (float)0.7,
                    MaxTokens = 3000,
                    NucleusSamplingFactor = (float)0,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0,
                });

                ChatCompletions response = responseWithoutStream.Value;

                result = response.Choices[0].Message.Content;

                #region konuşma kısmı

                #endregion

            }
            catch (Exception exp)
            {

            }
            return result;
        }

        

        static async Task<byte[]> GetImageBytesAsync(string imageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // URL'den resmi asenkron olarak indirin
                    using (HttpResponseMessage response = await client.GetAsync(imageUrl))
                    {
                        response.EnsureSuccessStatusCode();

                        // Yanıtı byte dizisine dönüştürün
                        return await response.Content.ReadAsByteArrayAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata: {ex.Message}");
                    return null;
                }
            }
        }
    }
}
