using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public String Getimage64()
        {
            if (photo != null)
            {
                using (MemoryStream memory = new MemoryStream())

                {
                    Stream stream = photo.GetStream();
                    stream.CopyTo(memory);
                    byte[] fotobyte = memory.ToArray();
                    String Base64 = Convert.ToBase64String(fotobyte);
                    return Base64;
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
