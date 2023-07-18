using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Media;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E2Grupo1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NuevaUbicacionPage : ContentPage
    {
        Plugin.Media.Abstractions.MediaFile photo = null;
        
        public NuevaUbicacionPage()
        {
            InitializeComponent();
            GetUbicacion();
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

        private async void OnGuardarUbicacionClicked(object sender, EventArgs e)
        {
            await AddLugar();
        }

        private async Task AddLugar()
        {
            try
            {
                var lugar = new
                {
                    descripcion = Descripcion.Text,
                    latitud = Latitud.Text,
                    longitud = Longitud.Text,
                    imagen = Getimage64(),
                };

                string apiUrl = "http://192.168.2.105:80/Lugares/CreateLugar.php";
                using (HttpClient client = new HttpClient())
                {
                    string jsonData = JsonConvert.SerializeObject(lugar);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Lugar creado.");
                        await DisplayAlert("Aviso", "Lugar Ingresado con Éxito", "Continuar");
                        await Navigation.PushAsync(new Views.UbicacionesPage());
                    }
                    else
                    {
                        Console.WriteLine("Error: Lugar no creado.");
                        await DisplayAlert("Aviso", "Error al Intentar Guardar Lugar", "Salir");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding data: {ex.Message}");
            }
        }

        //private async void guardar(object sender, EventArgs e)
        //{
        //    var lugar = new PM2E11248.Models.Lugares
        //    {
        //        Latitud = Latitud.Text,
        //        Longitud = Longitud.Text,
        //        Descripcion = Descripcion.Text,
        //        Foto = GetimageBytes()
        //    };

        //    if (await App.Instancia.AddLugar(lugar) > 0)
        //    {
        //        await DisplayAlert("Aviso", "Lugar Ingresado con Exito", "Continuar");
        //        await Navigation.PushAsync(new Views.PageLugaresGrid());
        //    }
        //    else
        //    {
        //        await DisplayAlert("Aviso", "Error al Intentar Guardar Lugar", "Salir");
        //    }
        //}

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

        private async void GetUbicacion()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var ubicacion = await Geolocation.GetLocationAsync(request);

                if (ubicacion != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Latitud.Text = ubicacion.Latitude.ToString();
                        Longitud.Text = ubicacion.Longitude.ToString();
                    });
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Error", "Servicios de Ubicacion no soportados", "OK");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Error", "Permiso Denegado", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al obtener ubicacion: {ex.Message}", "OK");
            }
        }

    }
}
