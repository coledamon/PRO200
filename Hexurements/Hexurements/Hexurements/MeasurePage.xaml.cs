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

namespace Hexurements
{
    public partial class MeasurePage : ContentPage
    {
        ObservableCollection<Measurement> measurements = new ObservableCollection<Measurement>();
        public MeasurePage()
        {
            InitializeComponent();
            Measurement m = new Measurement();
            m.Length = 28.9;
            m.TimeRecorded = DateTime.Now;
            measurements.Add(m);
            measurements.Add(m);
            measurements.Add(m);
            measurements.Add(m);
            lstMeasurements.ItemsSource = measurements;
        }

        private void btnMeasure_Clicked(object sender, EventArgs e)
        {
            Measurement m = new Measurement();
            m.Length = 28.9;
            m.TimeRecorded = DateTime.Now;
            measurements.Add(m);

            try
            {
                //var location = await Geolocation.GetLastKnownLocationAsync();

                //if (location != null)
                //{
                //    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                //}
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
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
            return timeRecorded + "\t\t\t     " + (((int)length)/12)+"'  "+(length%12)+"\"";
        }
    }
}