using System;
using System.Collections.Generic;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PM2E2Grupo1.Views
{
    public partial class UbicacionMapa : ContentPage
    {

        private double latitud;
        private double longitud;

        public UbicacionMapa(double latitud, double longitud)
        {
            InitializeComponent();
            this.latitud = latitud;
            this.longitud = longitud;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var ubicacion = await Geolocation.GetLocationAsync();
            if (ubicacion == null)
            {
                return;
            }
            var pin = new Pin()
            {
                Position = new Xamarin.Forms.Maps.Position(latitud, longitud),
                Label = "Ubicacion de Fotografia"
            };

            mapa.Pins.Add(pin);
            mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(latitud, longitud), Distance.FromMeters(100)));
        }

    }
}
