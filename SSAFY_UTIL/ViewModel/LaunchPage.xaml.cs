using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using SSAFY_UTIL.Service.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Location = SSAFY_UTIL.Model.Location;

namespace SSAFY_UTIL.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchPage : Page
    {
        private Firebase db = Firebase.GetInstance();

        private ObservableCollection<Tuple<string, DateOnly>> Dates = new();
        private ObservableCollection<string> Locations = new();
        private ObservableCollection<string> MenuTypes = new();

        private string SelectedLocation;
        private DateOnly SelectedDate;

        public LaunchPage()
        {
            this.InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            foreach (string key in SSAFY_UTIL.Model.Location.LocationInfo.Keys)
            {
                Locations.Add(key);
            }
            UpdateDateList();
        }

        private void UpdateDateList()
        {
            Dates.Clear();
            for (int i = -7; i <= 7; i++)
            {
                DateOnly date = DateOnly.FromDateTime(DateTime.Now.AddDays(i));
                if (i == 0)
                {
                    Dates.Add(new Tuple<string, DateOnly>(date.ToString("¿À´Ã"), date));
                }
                else
                {
                    Dates.Add(new Tuple<string, DateOnly>(date.ToString("MM¿ù ddÀÏ (ddd)"), date));
                }
            }
        }

        private void LocationListSelected(object sender, SelectionChangedEventArgs _)
        {
            SelectedLocation = (string)(sender as ListView).SelectedItem;
            DateList.Visibility = Visibility.Visible;

            UpdateDateList();
            if (DateList.SelectedItem == null)
            {
                DateList.UpdateLayout();
                DateList.ScrollIntoView(DateList.Items[5]);
                DateList.ScrollIntoView(DateList.Items[9]);
            }
        }

        private async void DateListSelected(object sender, SelectionChangedEventArgs _)
        {
            if (SelectedLocation == null || (Tuple<string, DateOnly>)(sender as ListView).SelectedItem == null)
                return;

            SelectedDate = ((Tuple<string, DateOnly>)(sender as ListView).SelectedItem).Item2;
            TypeList.Visibility = Visibility.Visible;

            TypeListLoadingIndicator.Visibility = Visibility.Visible;
            JArray result = await db.Read(
                Location.LocationInfo[SelectedLocation].ToString() + '/' +
                SelectedDate.ToString() +
                "/types.json");
            
            if (result == null)
            {
                TypeListEmptyBorder.Visibility = Visibility.Visible;
            }
            else
            {
                TypeListEmptyBorder.Visibility = Visibility.Collapsed;
                MenuTypes.Clear();
                foreach (string type in result.Select(v => (string)v))
                {
                    MenuTypes.Add(type);
                }
            }
            
            TypeListLoadingIndicator.Visibility = Visibility.Collapsed;
        }


    }
}
