using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PM2E2Grupo1.Models;
using Xamarin.Forms;

namespace PM2E2Grupo1.Views
{
    public partial class UbicacionesPage : ContentPage
    {
        public ObservableCollection<Location> ubicaciones { get; set; }
        private Location selectedLocation;

        public UbicacionesPage()
        {
            InitializeComponent();
            ubicaciones = new ObservableCollection<Location>();
            PopulateList();
        }

        private async Task PopulateList()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "http://192.168.2.105:80/Lugares/Readlugar.php";
                    string jsonResult = await client.GetStringAsync(apiUrl);
                    LugarResponse response = JsonConvert.DeserializeObject<LugarResponse>(jsonResult);
                    List<Lugar> lugares = response.datos;
                    ubicaciones = new ObservableCollection<Location>(ConvertToLocationList(lugares));
                    ubicacionesListView.ItemsSource = ubicaciones;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
        }

        private List<Location> ConvertToLocationList(List<Lugar> lugares)
        {
            return lugares.Select(lugar => new Location
            {
                Id = lugar.Id,
                titulo = lugar.Descripcion,
                latitud = lugar.Latitud,
                longitud = lugar.Longitud
            }).ToList();
        }

        private void OnUbicacionSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            selectedLocation = (Location)e.SelectedItem;
        }

        public async void OnMapaClicked(object sender, EventArgs e)
        {
            if (selectedLocation == null)
            {
                Console.WriteLine("Error: No se ha seleccionado una ubicación.");
                return;
            }

            if (!string.IsNullOrEmpty(selectedLocation.latitud) && !string.IsNullOrEmpty(selectedLocation.longitud))
            {
                double latitud = double.Parse(selectedLocation.latitud);
                double longitud = double.Parse(selectedLocation.longitud);

                await Navigation.PushAsync(new UbicacionMapa(latitud, longitud));
            }
            else
            {
                Console.WriteLine("Error: Latitud and longitud are empty.");
            }
        }

        public async void OnActualizarUbicacionClicked(object sender, EventArgs e)
        {
            if (selectedLocation == null)
            {
                Console.WriteLine("Error: No se ha seleccionado una ubicación válida para actualizar.");
                return;
            }

            // Pass the selected location's ID to the ActualizarUbicacionPage
            int lugarId = selectedLocation.Id;
            await Navigation.PushAsync(new ActualizarUbicacionPage(lugarId));
        }


        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            if (selectedLocation == null)
            {
                Console.WriteLine("Error: No se ha seleccionado una ubicación válida para eliminar.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = $"http://192.168.2.105:80/Lugares/DeleteLugar.php?id={selectedLocation.Id}";

                    HttpResponseMessage response = await client.DeleteAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Lugar eliminado.");

                        // Remove the selected location from the ubicaciones list
                        ubicaciones.Remove(selectedLocation);

                        // Reset the selectedLocation to null
                        selectedLocation = null;
                    }
                    else
                    {
                        Console.WriteLine("Error: Lugar no eliminado.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting data: {ex.Message}");
            }

        }


        public class Location
        {
            public int Id { get; set; }
            public string titulo { get; set; }
            public string latitud { get; set; }
            public string longitud { get; set; }

            public string display => $"Lat: {latitud}, Lon: {longitud}";
        }
    }
}
