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
using Android.App;
using System.Threading;

namespace Hexurements
{
    public partial class MeasurePage : ContentPage
    {
        ObservableCollection<Measurement> measurements = new ObservableCollection<Measurement>();
        private Location location1;
        private Location location2;
        private double distance;
        private int clickNum = 0;
        private string mode = "points";
        private System.Timers.Timer t;
        private int updateTime = 500;

        public MeasurePage()
        {
            //create switch to be in constant measure mode
            //when in constant measure mode, do while with a setInterval to update display until back, stop, or reset is clicked
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
                    if (clickNum == 0)
                    {
                        swcMode.IsEnabled = false;
                        location1 = location;
                        lblConstantDist.Text = "";
                        clickNum++;
                        lblText.Text = "Location 1 Recorded";
                        lblDebug.Text = location1.Latitude + " " + location1.Longitude;
                        btnMeasure.Text = "Stop Measurement";
                        btnClear.IsVisible = true;
                        btnMeasure.Text = "End Measurement";
                        if (swcMode.IsToggled) startConstantMeasure();
                    }
                    else
                    {
                        if (swcMode.IsToggled)
                        {
                            t.Stop();
                            t.Dispose();
                        }
                        swcMode.IsEnabled = true;
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
                        lblDebug.Text += "\n" + location2.Latitude + " " + location2.Longitude;
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

        private void startConstantMeasure()
        {
            t = new System.Timers.Timer(updateTime);
            t.Elapsed += T_Elapsed;
            t.AutoReset = true;
            t.Enabled = true;
        }

        private async void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                CancellationTokenSource cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                location2 = location;
                double flatDistFeet = Location.CalculateDistance(location1, location2, DistanceUnits.Miles) * 5280;
                if (location1.Altitude.HasValue && location2.Altitude.HasValue && (location1.Altitude != 0 || location2.Altitude != 0))
                {
                    double heightFeet = (location2.Altitude.Value - location1.Altitude.Value) * 3.2808;
                    distance = Math.Sqrt(Math.Pow(flatDistFeet, 2) + Math.Pow(heightFeet, 2));
                }
                else
                {
                    distance = flatDistFeet;
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    lblConstantDist.Text = ((int)distance) + "'  " + Math.Round((distance - ((int)distance)) * 12, 2) + "\"";
                });
            } 
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnClear_Clicked(object sender, EventArgs e)
        {
            if (swcMode.IsToggled)
            {
                t.Stop();
                t.Dispose();
            }

            swcMode.IsEnabled = true;
            btnMeasure.Text = "Start Measurement";
            lblConstantDist.Text = "";
            lblDebug.Text = "";
            lblText.Text = "";
            btnClear.IsVisible = false;
            clickNum = 0;
            location1 = null;
            btnMeasure.Text = "Start Measurement";
        }

        private void swcMode_Toggled(object sender, ToggledEventArgs e)
        {
            mode = swcMode.IsToggled ? "constant" : "points";
            lblMode.Text = swcMode.IsToggled ? "Mode (Current: Constant): " : "Mode (Current: Points): ";
            //Console.WriteLine(mode);
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