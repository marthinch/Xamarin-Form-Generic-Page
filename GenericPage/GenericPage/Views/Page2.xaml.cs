﻿using GenericPage.Models;
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
    public partial class Page2 : BaseContentPage<Sample>
    {
        private ObservableCollection<object> observableItems;
        private int index, item;
        private bool lastItem;


        private CancellationTokenSource cancellationTokenSearching;


        public Page2()
        {
            InitializeComponent();

            Title = "Generic page by base page";

            index = 0;
            item = 15;

            DataTemplate dataTemplate = Application.Current.Resources?["ObjectTemplate"] as DataTemplate;
            ListViewItems.ItemTemplate = dataTemplate;

            cancellationTokenSearching = new CancellationTokenSource();

            ListViewItems.ItemsSource = GetData();
        }

        private List<object> CustomData()
        {
            List<object> list = new List<object>();

            for (int i = 0; i <= 30; i++)
            {
                CurrentModel = new Sample();
                CurrentModel.Id = i;
                CurrentModel.Name = "Override value " + i;

                list.Add(CurrentModel);
            }

            return list;
        }

        private ObservableCollection<object> GetData()
        {
            var data = CustomData();

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


        private void EntrySearch_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
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

        private void ListViewItems_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {

        }

        private void ListViewItems_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
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