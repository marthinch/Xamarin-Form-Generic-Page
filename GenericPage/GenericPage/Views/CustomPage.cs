using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GenericPage.Views
{
    public class CustomPage<T> : ContentPage where T : class, new()
    {
        private ObservableCollection<T> observableItems;
        private int index, item;
        private bool lastItem;


        private Entry entry;
        private ListView listView;
        private DataTemplate dataTemplate;
        private CancellationTokenSource cancellationTokenSearching;


        public CustomPage()
        {
            Title = "Generic page by C#";

            index = 0;
            item = 15;

            dataTemplate = Application.Current.Resources?["ObjectTemplate"] as DataTemplate;

            cancellationTokenSearching = new CancellationTokenSource();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CustomView();
        }

        private List<T> CustomData()
        {
            List<T> list = new List<T>();

            for (int i = 0; i <= 30; i++)
            {
                T item = new T();
                item.GetType().GetProperty("Id").SetValue(item, i);
                item.GetType().GetProperty("Name").SetValue(item, "Override value " + i);
                list.Add(item);
            }

            return list;
        }

        private void CustomView()
        {
            entry = new Entry();
            entry.Placeholder = "Search";
            entry.TextChanged += Entry_TextChanged;

            listView = new ListView();
            listView.ItemsSource = GetData();
            listView.ItemAppearing += ListView_ItemAppearing;
            listView.ItemTapped += ListView_ItemTapped;

            // List item template
            listView.ItemTemplate = dataTemplate;

            StackLayout stackLayout = new StackLayout();
            stackLayout.Children.Add(entry);
            stackLayout.Children.Add(listView);
            stackLayout.Margin = new Thickness(10, 0);

            Content = stackLayout;
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
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
                        entry.Text = e.NewTextValue;
                        listView.ItemsSource = GetData();
                    });

                }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch
            {
                //Ignore any Threading errors
            }
        }

        private ObservableCollection<T> GetData()
        {
            var data = CustomData();

            if (!string.IsNullOrEmpty(entry.Text))
            {
                data = data.Where(a =>
                {
                    var selectedObject = a as T;
                    var objectValue = selectedObject.GetType().GetProperty("Name").GetValue(a);

                    var itemValue = (string)objectValue;
                    return itemValue.ToLower().Contains(entry.Text.ToLower());
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
                lastItem = false;

                if (observableItems == null || observableItems.Count == 0)
                {
                    observableItems = new ObservableCollection<T>(items);
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

        private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (lastItem)
                return;

            var item = e.Item as T;
            if (item == null)
                return;

            if (observableItems.Last().Equals(item))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    listView.ItemsSource = GetData();
                });
            }
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

    }
}
