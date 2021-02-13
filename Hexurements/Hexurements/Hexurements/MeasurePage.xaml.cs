using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

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

        public override string ToString()
        {
            return timeRecorded + "\t" + (((int)length)/12)+"'"+(length%12)+"\"";
        }
    }
}