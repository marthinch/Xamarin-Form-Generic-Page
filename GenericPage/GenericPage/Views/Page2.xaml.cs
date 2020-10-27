using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GenericPage.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page2<T> : ContentPage where T : class
    {
        public Page2()
        {
            InitializeComponent();
        }
    }
}