using System;
using System.Drawing;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApiEjercicio
{
    public partial class FormClima : Form
    {

        private const string ApiKey = "aca coloco mi api key";
        private const string ApiUrl = "Aca va la url";
        private readonly HttpClient _httpClient = new HttpClient();

       
        public FormClima()
        {
            InitializeComponent();
            InicializarControles();
        }

        private void InicializarControles()
        {
            this.Text = "Aplicación del Clima";
            this.Size = new Size(400, 350);

            lblError = new Label() { Text = "", AutoSize = true, Location = new Point(10, 10), ForeColor = Color.Red };
            txtBusqueda = new TextBox() { Location = new Point(10, 30), Width = 200 };
            btnBuscar = new Button() { Text = "Buscar Clima", Location = new Point(220, 30) };
            lblCiudad = new Label() { Text = "Ciudad:", AutoSize = true, Location = new Point(10, 70) };
            lblTemperatura = new Label() { Text = "Temperatura:", AutoSize = true, Location = new Point(10, 90) };
            lblDescripcion = new Label() { Text = "Descripción:", AutoSize = true, Location = new Point(10, 110) };
            lblHumedad = new Label() { Text = "Humedad:", AutoSize = true, Location = new Point(10, 130) };
            lblViento = new Label() { Text = "Viento:", AutoSize = true, Location = new Point(10, 150) };
            imgIconoClima = new PictureBox() { SizeMode = PictureBoxSizeMode.Zoom, Size = new Size(100, 100), Location = new Point(250, 70) };

            this.Controls.AddRange(new Control[] { lblError, txtBusqueda, btnBuscar, lblCiudad, lblTemperatura, lblDescripcion, lblHumedad, lblViento, imgIconoClima });

            btnBuscar.Click += btnBuscar_Click;
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            string ciudad = txtBusqueda.Text.Trim();
            if (!string.IsNullOrEmpty(ciudad))
            {
                lblError.Text = "";
                await ObtenerClima(ciudad);
            }
            else
            {
                lblError.Text = "Por favor, ingresa el nombre de una ciudad.";
            }
        }

        private async Task ObtenerClima(string ciudad)
        {
            try
            {
                string url = string.Format(ApiUrl, ciudad, ApiKey);
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResult = await response.Content.ReadAsStringAsync();
                WeatherData climaData = JsonConvert.DeserializeObject<WeatherData>(jsonResult);

                if (climaData != null)
                {
                    lblCiudad.Text = $"Ciudad: {climaData.name}, {climaData.sys?.country}";
                    lblTemperatura.Text = $"Temperatura: {climaData.main?.temp}°C";
                    lblDescripcion.Text = $"Descripción: {climaData.weather?[0]?.description}";
                    lblHumedad.Text = $"Humedad: {climaData.main?.humidity}%";
                    lblViento.Text = $"Viento: {climaData.wind?.speed} m/s";
                    if (climaData.weather?[0]?.icon != null)
                    {
                        await CargarIconoClima(climaData.weather[0].icon);
                    }
                    else
                    {
                        imgIconoClima.Image = null;
                    }
                }
                else
                {
                    lblError.Text = "No se encontraron datos para la ciudad ingresada.";
                    LimpiarDatosClima();
                }
            }
            catch (HttpRequestException ex)
            {
                lblError.Text = $"Error al conectar con la API: {ex.Message}";
                LimpiarDatosClima();
            }
            catch (JsonException ex)
            {
                lblError.Text = $"Error al procesar la respuesta de la API: {ex.Message}";
                LimpiarDatosClima();
            }
            catch (Exception ex)
            {
                lblError.Text = $"Ocurrió un error inesperado: {ex.Message}";
                LimpiarDatosClima();
            }
        }

        private async Task CargarIconoClima(string iconCode)
        {
            try
            {
                string iconUrl = $"https://openweathermap.org/img/wn/{iconCode}@2x.png";
                using (var stream = await _httpClient.GetStreamAsync(iconUrl))
                {
                    imgIconoClima.Image = Image.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
                lblError.Text += $" Error al cargar el icono: {ex.Message}";
                imgIconoClima.Image = null;
            }
        }

        private void LimpiarDatosClima()
        {
            lblCiudad.Text = "Ciudad:";
            lblTemperatura.Text = "Temperatura:";
            lblDescripcion.Text = "Descripción:";
            lblHumedad.Text = "Humedad:";
            lblViento.Text = "Viento:";
            imgIconoClima.Image = null;
        }
    }


    public class WeatherData
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("main")]
        public MainData main { get; set; }

        [JsonProperty("weather")]
        public Weather[] weather { get; set; }

        [JsonProperty("wind")]
        public Wind wind { get; set; }

        [JsonProperty("sys")]
        public Sys sys { get; set; }
    }

    public class MainData
    {
        [JsonProperty("temp")]
        public float temp { get; set; }

        [JsonProperty("humidity")]
        public int humidity { get; set; }
    }

    public class Weather
    {
        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("icon")]
        public string icon { get; set; }
    }

    public class Wind
    {
        [JsonProperty("speed")]
        public float speed { get; set; }
    }

    public class Sys
    {
        [JsonProperty("country")]
        public string country { get; set; }
    }
}