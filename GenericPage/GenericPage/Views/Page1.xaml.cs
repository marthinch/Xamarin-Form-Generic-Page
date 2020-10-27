using GenericPage.Models;
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
    public partial class Page1 : ContentPage
    {
        private ObservableCollection<Sample> observableItems;
        private int index, item;
        private bool lastItem;


        private CancellationTokenSource cancellationTokenSearching;


        public Page1()
        {
            InitializeComponent();

            Title = "Default page";

            index = 0;
            item = 15;

            DataTemplate dataTemplate = Application.Current.Resources?["ObjectTemplate"] as DataTemplate;
            ListViewItems.ItemTemplate = dataTemplate;

            cancellationTokenSearching = new CancellationTokenSource();

            ListViewItems.ItemsSource = GetData();
        }

        private List<Sample> CustomData()
        {
            List<Sample> samples = new List<Sample>();

            for (int i = 0; i <= 30; i++)
            {
                Sample item = new Sample();
                item.Id = i;
                item.Name = "Override value " + i;
                samples.Add(item);
            }

            return samples;
        }

        private ObservableCollection<Sample> GetData()
        {
            var data = CustomData();

            if (!string.IsNullOrEmpty(EntrySearch.Text))
            {
                data = data.Where(a => a.Name.ToLower().Contains(EntrySearch.Text.ToLower())).ToList();
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
                    observableItems = new ObservableCollection<Sample>(items);
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

            var item = e.Item as Sample;
            if (item == null)
                return;

            if (observableItems.Last().Equals(item))
            {
                ListViewItems.ItemsSource = GetData();
            }
        }
    }
}