using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using PM2E2Grupo1.Models;
using Xamarin.Forms;

namespace PM2E2Grupo1.Views
{
    public partial class ActualizarUbicacionPage : ContentPage
    {
        private int lugarId;
        private byte[] audioData;
        private byte[] imageData;
        Plugin.Media.Abstractions.MediaFile photo = null;

        public ActualizarUbicacionPage(int id)
        {
            InitializeComponent();
            lugarId = id;
            FetchData();
        }

        public string Getimage64()
        {
            if (photo != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = photo.GetStream();
                    stream.CopyTo(memory);
                    byte[] fotobyte = memory.ToArray();
                    string base64String = Convert.ToBase64String(fotobyte);
                    return base64String;
                }
            }
            return null;
        }


        public byte[] GetimageBytes()
        {
            if (photo != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = photo.GetStream();
                    stream.CopyTo(memory);
                    byte[] fotobyte = memory.ToArray();
                    return fotobyte;
                }
            }
            return null;
        }

        private async void tomarFoto(object sender, EventArgs e)
        {
            photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "MYAPP",
                Name = "Foto.jpg",
                SaveToAlbum = true
            });

            if (photo != null)
            {
                Foto.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
            }
        }

        public async Task UpdateLugar()
        {
            try
            {
                var lugar = new
                {
                    id = lugarId,
                    descripcion = Descripcion.Text,
                    latitud = Latitud.Text,
                    longitud = Longitud.Text,
                    imagen = GetimageBytes()
                    
                };

                string apiUrl = "http://192.168.2.105:80/Lugares/UpdateLugar.php";
                using (HttpClient client = new HttpClient())
                {
                    string jsonData = JsonConvert.SerializeObject(lugar);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Lugar actualizado.");
                        await DisplayAlert("Aviso", "Lugar Actualizado con Éxito", "Continuar");
                        await Navigation.PushAsync(new Views.UbicacionesPage());
                    }
                    else
                    {
                        Console.WriteLine("Error: Lugar no actualizado.");
                        await DisplayAlert("Aviso", "Error al Intentar Actualizar Lugar", "Salir");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating data: {ex.Message}");
            }
        }

        private async void OnGuardarUbicacionClicked(object sender, EventArgs e)
        {
            await UpdateLugar();
        }


        private async Task FetchData()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = $"http://192.168.2.105:80/Lugares/ReadOneLugar.php?id={lugarId}";

                    // Make the request to the server
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        var lugarResponse = JsonConvert.DeserializeObject<LugarResponse>(jsonResult);

                        // Assuming the response contains a list of 'datos' objects
                        if (lugarResponse != null && lugarResponse.datos.Count > 0)
                        {
                            var lugar = lugarResponse.datos[0];

                            // Store the image data directly in the imageData variable
                            imageData = lugar.Imagen;
                            Console.WriteLine($"Image data length: {imageData.Length}");

                            // Create an ImageSource from the byte array and set it as the Image component's Source
                            try
                            {
                                // Create an ImageSource from the byte array and set it as the Image component's Source
                                Foto.Source = ImageSource.FromStream(() => new MemoryStream(imageData));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error loading image: {ex.Message}");
                            }

                            // Populate the Descripcion, Latitud, and Longitud with the fetched data
                            Descripcion.Text = lugar.Descripcion;
                            Latitud.Text = lugar.Latitud;
                            Longitud.Text = lugar.Longitud;

                            // Store the audio data in the audioData variable
                            //audioData = lugar.Audio;
                        }
                        else
                        {
                            Console.WriteLine("Error: Empty response or invalid data.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Failed to fetch data.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
        }
    }
}
