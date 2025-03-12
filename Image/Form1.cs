using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq; 
using System.IO; 

namespace Image
{
    public partial class Form1 : Form
    {
        private string _apiKey = "QTc5CKxwobr8-IBqYZTP6ItE-ID_8QxyI5_eBiSgQa0";
        private string _apiUrl = "https://api.unsplash.com/photos/random";
        private readonly HttpClient _httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string query = textBox1.Text;
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Пожалуйста, введите запрос.");
                return;
            }

            try
            {
                string imageUrl = await GetRandomImageUrl(query);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    await LoadAndDisplayImage(imageUrl);
                }
                else
                {
                    MessageBox.Show("Не удалось найти изображение.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task<string> GetRandomImageUrl(string query)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {_apiKey}");

                string url = $"{_apiUrl}?query={Uri.EscapeDataString(query)}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JObject result = JObject.Parse(json);
                    return result["urls"]["regular"].ToString();
                }
                else
                {
                    MessageBox.Show($"Ошибка API: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запросе к API: {ex.Message}");
                return null;
            }
        }

        private async Task LoadAndDisplayImage(string imageUrl)
        {
            if (!IsValidUrl(imageUrl))
            {
                MessageBox.Show("Неверный URL изображения.");
                return;
            }

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(imageUrl);
                if (response.IsSuccessStatusCode)
                {
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        try
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                            picImage.Image = image;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при создании изображения: {ex.Message}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Ошибка HTTP: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
            }
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Введите запрос ._.");
        }

      

    }
}
