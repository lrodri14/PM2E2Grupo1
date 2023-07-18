using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM2E2Grupo1.Views;
using Xamarin.Forms;

namespace PM2E2Grupo1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public async void OnNuevaUbicacionClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.NuevaUbicacionPage());
        }

        public async void OnUbicacionesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.UbicacionesPage());
        }
    }
}
