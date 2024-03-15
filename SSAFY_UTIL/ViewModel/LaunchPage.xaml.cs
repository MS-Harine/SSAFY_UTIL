using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using SSAFY_UTIL.Service.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.StartScreen;
using Location = SSAFY_UTIL.Model.Location;

namespace SSAFY_UTIL.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchPage : Page
    {
        private Firebase db = Firebase.GetInstance();

        private ObservableCollection<DateForVis> Dates = new();
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
                Dates.Add(new DateForVis(date));
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
            if (SelectedLocation == null || (DateForVis)(sender as ListView).SelectedItem == null)
                return;

            var SelectedItem = (DateForVis)(sender as ListView).SelectedItem;
            TypeListEmptyBorder.Visibility = Visibility.Collapsed;
            TypeListHolidayBorder.Visibility = Visibility.Collapsed;
            TypeListLoadingIndicator.Visibility = Visibility.Collapsed;
            TypeList.Visibility = Visibility.Collapsed;

            if (SelectedItem.isDisable)
            {
                TypeListHolidayBorder.Visibility = Visibility.Visible;
            }
            else
            {
                SelectedDate = SelectedItem.Date;
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
                    MenuTypes.Clear();
                    foreach (string type in result.Select(v => (string)v))
                    {
                        MenuTypes.Add(type);
                    }
                    TypeList.Visibility = Visibility.Visible;
                }
                TypeListLoadingIndicator.Visibility = Visibility.Collapsed;
            }
        }
    }

    class DateForVis
    {
        public DateOnly Date;
        public string Prefix;
        public string DayWeek;
        public string Postfix;
        public string Color;
        public bool isDisable;

        public DateForVis(DateOnly date)
        {
            Date = date;

            if (date == DateOnly.FromDateTime(DateTime.Now))
            {
                Prefix = "¿À´Ã";
                DayWeek = "";
                Postfix = "";
            }
            else
            {
                Prefix = date.ToString("M¿ù ddÀÏ (");
                DayWeek = date.ToString("ddd");
                Postfix = ")";
            }

            Color = "black";
            isDisable = false;
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                Color = "blue";
                isDisable = true;
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                Color = "red";
                isDisable = true;
            }
        }
    }
}
