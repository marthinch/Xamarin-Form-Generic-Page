using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GenericPage.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page3 : ContentPage
    {
        private static DataTemplate dataTemplate;
        private static List<object> listItems;
        private static int index, item;
        private static bool lastItem;


        private ObservableCollection<object> observableItems;
        private CancellationTokenSource cancellationTokenSearching;

        public Page3()
        {
            InitializeComponent();

            Title = "Generic page by instance";

            ListViewItems.ItemTemplate = dataTemplate;
        }

        public static Page3 Instance<T>() where T : class, new()
        {
            index = 0;
            item = 15;

            dataTemplate = Application.Current.Resources?["ObjectTemplate"] as DataTemplate;

            // Generate data
            listItems = new List<object>();
            for (int i = 0; i <= 30; i++)
            {
                T item = new T();
                item.GetType().GetProperty("Id").SetValue(item, i);
                item.GetType().GetProperty("Name").SetValue(item, "Override value " + i);
                listItems.Add(item);
            }

            return new Page3();
        }

        private ObservableCollection<dynamic> GetData()
        {
            var data = listItems;

            if (!string.IsNullOrEmpty(EntrySearch.Text))
            {
                data = data.Where(a =>
                {
                    var objectValue = a.GetType().GetProperty("Name").GetValue(a);

                    var itemValue = (string)objectValue;
                    return itemValue.ToLower().Contains(EntrySearch.Text.ToLower());
                }).ToList();
                if (data == null || data.Count == 0)
                {
                    lastItem = true;
                    return null;
                }
            }

            var items = data.Skip(index * item).Take(item).ToList();
            if (items != null && items.Count > 0)
            {
                if (observableItems == null || observableItems.Count == 0)
                {
                    observableItems = new ObservableCollection<object>(items);
                }
                else
                {
                    foreach (var item in items)
                    {
                        observableItems.Add(item);
                    }
                }

                index++;
            }
            else
            {
                index = 0;
                lastItem = true;
            }

            return observableItems;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ListViewItems.ItemsSource = GetData();
        }

        private void EntrySearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Interlocked.Exchange(ref this.cancellationTokenSearching, new CancellationTokenSource()).Cancel();
                Task.Delay(TimeSpan.FromMilliseconds(500), this.cancellationTokenSearching.Token).ContinueWith(task =>
                {
                    index = 0;
                    observableItems?.Clear();

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        EntrySearch.Text = e.NewTextValue;
                        ListViewItems.ItemsSource = GetData();
                    });

                }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch
            {
                //Ignore any Threading errors
            }
        }

        private void ListViewItems_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private void ListViewItems_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (lastItem)
                return;

            if (observableItems.Last().Equals(e.Item))
            {
                ListViewItems.ItemsSource = GetData();
            }
        }
    }
}