using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Intellishoot_PPK_Geotagging_Utility
{
    public partial class Intellishoot_PPK_Geotagging_Utility : Form
    {
        public Intellishoot_PPK_Geotagging_Utility()
        {
            InitializeComponent();
            

            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, label2.Width, label2.Height);

            this.label1.Region = new Region(path);
            this.label2.Region = new Region(path);
        }
        
        private void button3_Click(object sender, EventArgs e)
        {

            bool inList = false;

            foreach (var profile in profiles.Values)
            {

            }

            if (!inList)
            {
                try
                {
                    profileinfo newprofile = new profileinfo();
                    profiles.Add("camera", newprofile);
                    loadprofileList(true, Path.Combine(Environment.CurrentDirectory, "profiles.xml"));
                }
                catch
                {

                }
            }

            string fileExt = Path.GetExtension(LogFilePath.Text);
            List<CameraLog> log = new List<CameraLog>();

            log = PPKList(LogFilePath.Text.ToString());
            tagFromPPK(ImagePath.Text.ToString(), log);
            DialogResult result = DialogResult.OK;
            if (result.Equals(DialogResult.OK))
            {



                    LogFilePath.Visible = true;
                    ImagePath.Visible = true;

                MessageBox.Show("Processing Complete!");
            }
        }
        public void loadprofileList(bool write, string filename)
        {
            bool exists = File.Exists(filename);

            if (write || !exists)
            {
                try
                {
                    XmlTextWriter xmlwriter = new XmlTextWriter(filename, Encoding.ASCII);
                    xmlwriter.Formatting = Formatting.Indented;

                    xmlwriter.WriteStartDocument();

                    xmlwriter.WriteStartElement("Profiles");

                    foreach (string key in profiles.Keys)
                    {
                        try
                        {
                            if (key == "")
                                continue;
                            xmlwriter.WriteStartElement("Profile");
                            xmlwriter.WriteElementString("name", profiles[key].name);
                            xmlwriter.WriteElementString("offset", profiles[key].offset.ToString(new System.Globalization.CultureInfo("en-US")));
                            xmlwriter.WriteEndElement();
                        }
                        catch { }
                    }

                    xmlwriter.WriteEndElement();

                    xmlwriter.WriteEndDocument();
                    xmlwriter.Close();

                }
                catch { }
            }
            else
            {
                try
                {
                    using (XmlTextReader xmlreader = new XmlTextReader(filename))
                    {
                        while (xmlreader.Read())
                        {
                            xmlreader.MoveToElement();
                            try
                            {
                                switch (xmlreader.Name)
                                {
                                    case "Profile":
                                        {
                                            profileinfo profile = new profileinfo();

                                            while (xmlreader.Read())
                                            {
                                                bool dobreak = false;
                                                xmlreader.MoveToElement();
                                                switch (xmlreader.Name)
                                                {
                                                    case "name":
                                                        profile.name = xmlreader.ReadString();
                                                        break;
                                                    case "offset":
                                                        profile.offset = decimal.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "Profile":
                                                        profiles[profile.name] = profile;
                                                        dobreak = true;
                                                        break;
                                                }
                                                if (dobreak)
                                                    break;
                                            }
                                            string temp = xmlreader.ReadString();
                                        }
                                        break;
                                    case "Config":
                                        break;
                                    case "xml":
                                        break;
                                    default:
                                        if (xmlreader.Name == "") // line feeds
                                            break;
                                        //config[xmlreader.Name] = xmlreader.ReadString();
                                        break;
                                }
                            }
                            catch (Exception ee) { Console.WriteLine(ee.Message); } // silent fail on bad entry
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Bad Camera File: " + ex.ToString()); } // bad config file

                // populate list
                foreach (var profile in profiles.Values)
                {
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {


            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.ValidateNames = false;
                openFileDialog1.CheckFileExists = false;
                openFileDialog1.CheckPathExists = true;

                // Always default to Folder Selection.
                openFileDialog1.FileName = "Folder Selection.";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    ImagePath.Text = Path.GetDirectoryName(openFileDialog1.FileName);
                    
                }

                ImagePath.Visible = true;
            }



           /* var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ImagePath.Text = dialog.SelectedPath;
            }
            ImagePath.Visible = true;*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {

                openFileDialog1.Filter = "pos(*.pos);txt files (*.txt)|*.pos;*.txt";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                        LogFilePath.Text = openFileDialog1.FileNames[0].ToString();
                }
                LogFilePath.Visible = true;
            }
        }
        private static string ConvertWhitespacesToSingleSpaces(string value)
        {
            value = Regex.Replace(value, @"\s+", " ");
            return value;
        }
        private static List<CameraLog> PPKList(string FileName)
        {

            List<CameraLog> data = new List<CameraLog>();
            int OrderAdded = 0;

            string[] records = File.ReadAllLines(FileName);
            CameraLog newLog = new CameraLog();


            foreach (string item in records)
            {
                if (item.StartsWith("2"))
                {

                    string line3 = ConvertWhitespacesToSingleSpaces(item);
                    newLog = new CameraLog();
                    string[] gpsLine = line3.Split(new[] { ' ' });

                    string[] splitdate = gpsLine[0].Split('/');
                    string[] splittime = gpsLine[1].Split(':');
                    string[] splittimeMS = splittime[2].Split('.');


                    newLog.tagDatetime = new DateTimeOffset(int.Parse(splitdate[0]), int.Parse(splitdate[1]), int.Parse(splitdate[2]), int.Parse(splittime[0]), int.Parse(splittime[1]), int.Parse(splittimeMS[0]), int.Parse(splittimeMS[1]),TimeSpan.Zero);


                    newLog.Latitude = decimal.Parse(gpsLine[2]);
                    newLog.Longitude = decimal.Parse(gpsLine[3]);
                    newLog.GPSAltitude = double.Parse(gpsLine[4]);
                    //RPY are not used at this time
                    newLog.Yaw = 0;
                    newLog.Pitch = 0;
                    newLog.Roll = 0;
                    newLog.orientationAccuracy = 0;

                    newLog.q = int.Parse(gpsLine[5]);

                    if (newLog.q == 1)
                    {
                        newLog.geotagAccuracy = .05;
                        newLog.enabled = 1;

                    }
                    else if (newLog.q == 2)
                    {
                        newLog.geotagAccuracy = .4;
                        newLog.enabled = 1;

                    }
                    else
                    {
                        newLog.geotagAccuracy = 5;
                        newLog.enabled = 0;

                    }

                    newLog.q = int.Parse(gpsLine[5]);
                    // newLog.q = int.Parse(gpsLine[10]);
                    newLog.LogType = "PPK";
                    newLog.OrderAdded = OrderAdded;

                    data.Add(newLog);

                    OrderAdded = OrderAdded + 1;
                }
                if (item.StartsWith("D"))
                {

                    newLog = new CameraLog();
                    string[] gpsLine = item.Split(new[] { '\t' });


                    newLog.Latitude = decimal.Parse(gpsLine[1]);
                    newLog.Longitude = decimal.Parse(gpsLine[2]);
                    newLog.GPSAltitude = double.Parse(gpsLine[3]);
                    newLog.Yaw = decimal.Parse(gpsLine[4]);
                    newLog.Pitch = decimal.Parse(gpsLine[5]);
                    newLog.Roll = decimal.Parse(gpsLine[6]);
                    newLog.geotagAccuracy = double.Parse(gpsLine[7]);
                    newLog.orientationAccuracy = double.Parse(gpsLine[8]);
                    newLog.enabled = int.Parse(gpsLine[9]);
                    newLog.q = 6;
                    // newLog.q = int.Parse(gpsLine[10]);
                    newLog.LogType = "standard";
                    newLog.OrderAdded = OrderAdded;

                    data.Add(newLog);

                    OrderAdded = OrderAdded + 1;
                }
            }
           
            if (data[0].LogType == "PPK")
            {
                data.Sort((x, y) => DateTimeOffset.Compare(x.tagDatetime, y.tagDatetime));
            }

            return data;



        }

        private static List<Picture> returnPictures(string imageFileName)
        {

            return getPictureDateTime(imageFileName);
        }

        public static double convertToEpoch(DateTime camDateTime)
        {

            double epoch = (camDateTime - new DateTime(1970, 1, 1)).TotalSeconds;



            return epoch;
        }
        private static Regex r = new Regex(":");

        private static double pullDateFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);

                return convertToEpoch(DateTime.Parse(dateTaken));

            }
        }
        private static List<Picture> getPictureDateTime(string filePath)
        {
            int i = 1;
            List<Picture> Pictures = new List<Picture>();
            foreach (string r in GetImages(filePath))
            {
                
                Picture pic = new Picture();
                pic.isused = false;
                pic.estIDX = i;
                pic.ImageName = r.Split('\\').Last();
                if (!pic.ImageName.EndsWith(".tif"))
                {
                    pic.timetaken = pullDateFromImage(r);
                    Pictures.Add(pic);
                    i++;
                }
                else { 
                    if (pic.ImageName.Contains("_1.tif"))
                    {
                        Pictures.Add(pic);
                        i++;
                    }
       
                    }

            }

            return Pictures;
        }

        private static void tagFromPPK(string imageFileName, List<CameraLog> gpsLog)
        {
            int i = 1;
            //get images
            List<Picture> pictureNames = new List<Picture>();

            pictureNames = returnPictures(imageFileName);
            IEnumerator<CameraLog> enum1 = gpsLog.GetEnumerator();
            enum1.MoveNext();
            //put tags with pictures

            //write to pictures
            writeToFilePPK(gpsLog, pictureNames, imageFileName);

            IEnumerable<string> image = GetImages(imageFileName);
            var enum3 = image.GetEnumerator();

        }

        public static IEnumerable<string> GetFiles(string Path)
        {
            //return Directory.GetFiles(Path, "*.JPG,*.tif", SearchOption.AllDirectories)
            //    .AsEnumerable();

            return Directory.EnumerateFiles(Path, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => s.ToLower().EndsWith(".jpg") || s.EndsWith(".tif") || s.ToLower().EndsWith(".tiff")
                || s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".log") || s.ToLower().EndsWith(".tlog") || s.ToLower().EndsWith(".raw"));
        }
        //enumerate through list here
        public static IEnumerable<string> GetImages(string Path)
        {
            //return Directory.GetFiles(Path, "*.JPG,*.tif", SearchOption.AllDirectories)
            //    .AsEnumerable();

            return Directory.EnumerateFiles(Path, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => s.ToLower().EndsWith(".jpg") || s.EndsWith(".tif") || s.ToLower().EndsWith(".tiff")
                || s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".raw"));
        }

        private static void writeToFilePPK(List<CameraLog> finalCoords, List<Picture> pictureName, string ImageFileName)
        {
            IEnumerator<CameraLog> enum1 = finalCoords.GetEnumerator();
            IEnumerator<Picture> enum2 = pictureName.GetEnumerator();
            while ((enum1.MoveNext()) && (enum2.MoveNext()))
            {


                enum1.Current.ImageName = enum2.Current.ImageName;
            }


            StringBuilder sb = new StringBuilder();
            StringBuilder Quality = new StringBuilder();
            double q1 = 0;
            double q2 = 0;
            double q3 = 0;
            double q4 = 0;
            double q5 = 0;
            double q6 = 0;
            foreach (var item in finalCoords)
            {
                if (item.q == 1)
                {
                    q1++;
                }
                if (item.q == 2)
                {
                    q2++;
                }
                if (item.q == 3)
                {
                    q3++;
                }
                if (item.q == 4)
                {
                    q4++;
                }
                if (item.q == 5)
                {
                    q5++;
                }
                if (item.q == 6)
                {
                    q6++;
                }

                item.GPSAltitude = item.GPSAltitude;

                sb.Append(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t", item.ImageName, item.Latitude.ToString(), item.Longitude.ToString(), (item.GPSAltitude).ToString(),(item.Yaw.ToString()), (item.Pitch.ToString()), item.Roll.ToString(),(item.geotagAccuracy.ToString()),(item.orientationAccuracy.ToString()), (item.enabled.ToString())) + Environment.NewLine);
                
         
                Quality.Append(string.Format("{0}\t{1}\t", item.ImageName, item.q.ToString()) + Environment.NewLine);

            }
            Quality.Insert(0, string.Format(Environment.NewLine));
            Quality.Insert(0, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t", "Percentages - FIX", (Math.Round((q1 / (q1 + q2 + q3 + q4 + q5)) * 100, 2)).ToString() + "%", "Float:", (Math.Round((q2 / (q1 + q2 + q3 + q4 + q5)) * 100, 2)).ToString(), "Single: ", (Math.Round((q3 / (q1 + q2 + q3 + q4 + q5)) * 100, 2)).ToString() + "%", "Q4: ", (Math.Round((q4 / (q1 + q2 + q3 + q4 + q5)) * 100, 2)).ToString() + "%", "Q5: ", (Math.Round((q5 / (q1 + q2 + q3 + q4 + q5)) * 100, 2)).ToString() + "%" + Environment.NewLine, 1));
            Quality.Insert(0, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t", "Number of points - FIX: ", q1.ToString(), "Float:", q2.ToString(), "Single: ", q3.ToString(), "Q4: ", q4, "Q5: ", q5.ToString()) + Environment.NewLine, 1);

            //changeimagefilename to imageppicture name
            using (StreamWriter outfile = new StreamWriter(ImageFileName + @"\Geotags.txt"))
            {
                outfile.Write(sb.ToString());
            }
            if (q6 == 0)
            {
                using (StreamWriter outfile = new StreamWriter(ImageFileName + @"\QualityReport.txt"))
                {
                    outfile.Write(Quality.ToString());
                }
            }

        }

        public struct profileinfo
        {
            public string name;
            public decimal offset;
        }

        Dictionary<string, profileinfo> profiles = new Dictionary<string, profileinfo>();

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            loadprofileList(false, Path.Combine(Environment.CurrentDirectory, "profiles.xml"));
        }
    }
}
