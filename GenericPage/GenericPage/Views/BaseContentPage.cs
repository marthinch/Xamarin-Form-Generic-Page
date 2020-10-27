using Xamarin.Forms;

namespace GenericPage.Views
{
    public class BaseContentPage<T> : ContentPage where T : class, new()
    {
        public T CurrentModel { get; set; }
    }
}
