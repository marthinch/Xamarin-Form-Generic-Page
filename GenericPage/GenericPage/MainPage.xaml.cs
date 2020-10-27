using GenericPage.Models;
using GenericPage.Views;
using System;
using Xamarin.Forms;

namespace GenericPage
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button1_Clicked(object sender, EventArgs e)
        {
            Page1 page1 = new Page1();
            await Navigation.PushAsync(page1);
        }

        private async void Button2_Clicked(object sender, EventArgs e)
        {
            Page2<NewSample> page2 = new Page2<NewSample>();
            await Navigation.PushAsync(page2);
        }

        private async void Button3_Clicked(object sender, EventArgs e)
        {
            var customPage = new CustomPage<NewSample>();
            await Navigation.PushAsync(customPage);
        }
    }
}
