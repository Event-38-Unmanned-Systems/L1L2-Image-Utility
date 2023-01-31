using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace Position_Adverage
{
    public partial class Adverage : Form
    {

         string filepath;

        public Adverage()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {

                openFileDialog1.Filter = "pos(*.pos)|*.pos";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filepath = openFileDialog1.FileNames[0].ToString();
                    FilePath.Text = Path.GetFileName(filepath);
                    FilePath.Visible = true;
                    Process.Visible = true;
                }
                else {
                    //tell error 
                };

            }
        }

        private static string ConvertWhitespacesToSingleSpaces(string value)
        {
            value = Regex.Replace(value, @"\s+", " ");
            return value;
        }

        private static List<posline> PPKList(string FileName)
        {

            List<posline> data = new List<posline>();
            int OrderAdded = 0;

            string[] records = File.ReadAllLines(FileName);
            posline newLog = new posline();


            foreach (string item in records)
            {
                if (item.StartsWith("2"))
                {

                    string line3 = ConvertWhitespacesToSingleSpaces(item);
                    newLog = new posline();
                    string[] gpsLine = line3.Split(new[] { ' ' });



                    newLog.latitude = double.Parse(gpsLine[2]);
                    newLog.longitude = double.Parse(gpsLine[3]);
                    newLog.height = double.Parse(gpsLine[4]);
                    newLog.Q = int.Parse(gpsLine[5]);
                    newLog.ns = int.Parse(gpsLine[6]);
                    newLog.sdn = double.Parse(gpsLine[7]);
                    newLog.sde = double.Parse(gpsLine[8]);
                    newLog.sdu = double.Parse(gpsLine[9]);
                    newLog.sdne = double.Parse(gpsLine[10]);
                    newLog.sdun = double.Parse(gpsLine[11]);
                    newLog.age = double.Parse(gpsLine[12]);
                    newLog.ratio = double.Parse(gpsLine[13]);
                    newLog.sampleNumber = OrderAdded;

                    data.Add(newLog);
    
                    OrderAdded = OrderAdded + 1;
                }
            }

            return data;



        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public double GetDistance(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        private void Process_Click(object sender, EventArgs e)
        {
            int pointsWithinDist = 0;
            int pointsCollected = 0;
            double adverageLat = 0;
            double adverageLon = 0;
            double adverageHeight = 0;
            List<posline> list = new List<posline>();
            List<double> adveragedLonList = new List<double>();
            List<double> adveragedLatList = new List<double>();
            List<double> adveragedHeightList = new List<double>();

            list = PPKList(filepath);

            for (int i=1; i < list.Count; i++)
            {
                double dist;
                dist = GetDistance(list[i-1].longitude, list[i-1].latitude, list[i].longitude, list[i].latitude);
                

                if (dist > .03 && pointsWithinDist > 25 )
                {
                    adveragedLonList.Add(adverageLon/pointsWithinDist);
                    adveragedLatList.Add(adverageLat /pointsWithinDist);
                    adveragedHeightList.Add(adverageHeight / pointsWithinDist);
                    pointsWithinDist = 0;
                    adverageLat = 0;
                    adverageLon = 0;
                    adverageHeight = 0;
                    pointsCollected++;
                    
                }
                else
                {
                    adverageLat = adverageLat + list[i].latitude;
                    adverageLon = adverageLon + list[i].longitude;
                    adverageHeight = adverageHeight + list[i].height;
                    pointsWithinDist++;
                }
            }

            if (pointsWithinDist > 25 )
                {
                adveragedLonList.Add(adverageLon / pointsWithinDist);
                adveragedLatList.Add(adverageLat / pointsWithinDist);
                adveragedHeightList.Add(adverageHeight / pointsWithinDist);
                pointsWithinDist = 0;
                adverageLat = 0;
                adverageLon = 0;
                adverageHeight = 0;
                pointsCollected++;

            }


            writeToFile(adveragedLatList, adveragedLonList, adveragedHeightList, "0");
        }

        private void writeToFile(List<double> latitude, List<double> longitude, List<double> Altitude, string height_offset)
        {
           


            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < latitude.Count; i++)
            {
                sb.Append(string.Format("{0}\t{1}\t{2}\t", latitude[i], longitude[i], (Altitude[i] - double.Parse(height_offset)).ToString()) + Environment.NewLine);
            }
            //changeimagefilename to imageppicture name
            using (StreamWriter outfile = new StreamWriter(Path.GetDirectoryName(filepath) + @"\AdveragedPoints.txt"))
            {
                outfile.Write(sb.ToString());
            }

        }
    }
}
