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
            Page2 page2 = new Page2();
            await Navigation.PushAsync(page2);
        }

        private async void Button3_Clicked(object sender, EventArgs e)
        {
            Page3 page3 = Page3.Instance<Sample>();
            await Navigation.PushAsync(page3);
        }

        private async void Button4_Clicked(object sender, EventArgs e)
        {
            CustomPage<Sample> customPage = new CustomPage<Sample>();
            await Navigation.PushAsync(customPage);
        }
    }
}
