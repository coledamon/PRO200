using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading;
using Android.App;

namespace Hexurements
{
    public partial class MeasurePage : ContentPage
    {
        ObservableCollection<Measurement> measurements = new ObservableCollection<Measurement>();
        private Location location1;
        private Location location2;
        private double distance;
        private int clickNum = 0;

        public MeasurePage()
        {
            InitializeComponent();
            lstMeasurements.ItemsSource = measurements;
        }

        private void btnMeasure_Clicked(object sender, EventArgs e)
        {
            getLocation();   
        }

        private async void getLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                CancellationTokenSource cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);                

                if (location != null)
                {
                    if(clickNum == 0)
                    {
                        location1 = location;
                        clickNum++;
                        lblText.Text = "Location 1 Recorded";
                        btnClear.IsVisible = true;
                        btnMeasure.Text = "End Measurement";
                    }
                    else
                    {
                        location2 = location;
                        clickNum--;
                        double flatDistFeet = Location.CalculateDistance(location1, location2, DistanceUnits.Miles)*5280;
                        if (location1.Altitude.HasValue && location2.Altitude.HasValue && (location1.Altitude != 0 || location2.Altitude != 0))
                        {
                            double heightFeet = (location2.Altitude.Value - location1.Altitude.Value) * 3.2808;
                            distance = Math.Sqrt(Math.Pow(flatDistFeet, 2) + Math.Pow(heightFeet, 2));
                        }
                        else
                        {
                            distance = flatDistFeet;
                        }
                        Measurement m = new Measurement() { Length = distance, TimeRecorded = DateTime.Now };
                        distance = 0;
                        measurements.Add(m);
                        lblText.Text = "Distance Recorded Below";
                        btnMeasure.Text = "Start Measurement";
                        btnClear.IsVisible = false;
                        location1 = null;
                        location2 = null;
                    }
                    //btnMeasure.Text = $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}";
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await DisplayAlert("Error", "This feature is not supported on your device.", "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await DisplayAlert("Error", "This feature is not enabled on your device.", "OK");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                await DisplayAlert("Error", "Location Permissions are currently not allowed for this app. This must be changed in settings if you would like to use this feature.", "OK");
            }
            catch (Exception ex)
            {
                // Unable to get location
                await DisplayAlert("Error", "The location could not be retrieved, please try again.", "OK");
            }
        }

        private void btnClear_Clicked(object sender, EventArgs e)
        {
            btnMeasure.Text = "Start Measurement";
            lblText.Text = "";
            btnClear.IsVisible = false;
            clickNum = 0;
            location1 = null;
        }
    }

    public class Measurement : INotifyPropertyChanged
    {
        private double length;
        private DateTime timeRecorded;

        public double Length
        {
            get { return length; }
            set
            {
                if(value != length)
                {
                    length = value;
                    onPropertyChanged("Length");
                }
            }
        }

        public DateTime TimeRecorded
        {
            get { return timeRecorded; }
            set
            {
                timeRecorded = value;
                onPropertyChanged("TimeRecorded");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void onPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Data
        {
            get
            {
                return ToString();
            }
        }

        public override string ToString()
        {
            return timeRecorded + "\t\t\t     " + ((int)length) +"'  "+Math.Round((length-((int)length))*12, 2)+"\"";
        }
    }
}