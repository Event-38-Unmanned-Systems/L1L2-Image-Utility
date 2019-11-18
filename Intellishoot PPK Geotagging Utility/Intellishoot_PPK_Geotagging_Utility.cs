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
            comboBox1.SelectedIndex = 0;

            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, label2.Width, label2.Height);

            this.label1.Region = new Region(path);
            this.label2.Region = new Region(path);
            this.label3.Region = new Region(path);
            this.label4.Region = new Region(path);
        }
        
        private void button3_Click(object sender, EventArgs e)
        {

            bool inList = false;

            foreach (var profile in profiles.Values)
            {
                if (profile.name == comboBox1.Text || comboBox1.Text == "Select/Add an Antenna")
                {
                    inList = true;
                }
            }

            if (!inList)
            {
                try
                {
                    profileinfo newprofile = new profileinfo();
                    newprofile.offset = decimal.Parse(camera_height_offset.Text);
                    newprofile.name = comboBox1.Text;
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
            tagFromPPK(ImagePath.Text.ToString(), log, camera_height_offset.Text);
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
                    if (!comboBox1.Items.Contains(profile.name))
                        comboBox1.Items.Add(profile.name);
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ImagePath.Text = dialog.SelectedPath;
            }
            ImagePath.Visible = true;
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



                    newLog.Latitude = decimal.Parse(gpsLine[2]);
                    newLog.Longitude = decimal.Parse(gpsLine[3]);
                    newLog.GPSAltitude = double.Parse(gpsLine[4]);
                    newLog.q = int.Parse(gpsLine[5]);
                    // newLog.q = int.Parse(gpsLine[10]);
                    newLog.LogType = "CAM";
                    newLog.OrderAdded = OrderAdded;

                    data.Add(newLog);

                    OrderAdded = OrderAdded + 1;
                }
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
                pic.timetaken = pullDateFromImage(r);
                pic.ImageName = r.Split('\\').Last();
                Pictures.Add(pic);
                i++;
            }

            return Pictures;
        }
        private static void tagFromPPK(string imageFileName, List<CameraLog> gpsLog, string height_offset)
        {
            int i = 1;
            //get images
            List<Picture> pictureNames = new List<Picture>();

            pictureNames = returnPictures(imageFileName);
            IEnumerator<CameraLog> enum1 = gpsLog.GetEnumerator();
            enum1.MoveNext();
            //put tags with pictures

            //write to pictures
            writeToFilePPK(gpsLog, pictureNames, imageFileName, height_offset);

            IEnumerable<string> image = GetImages(imageFileName);
            var enum3 = image.GetEnumerator();

            //foreach (CameraLog item in gpsLog)
            //{
            //    enum3.MoveNext();
            //    if (enum3.Current != null)
            //    {
            //        if (image.Count() >= i && item.q == 1)
            //        {
            //            i++;
            //            Helpers.ImageUtilityHelper.WriteCoordinatesToImage(enum3.Current,
            //                        double.Parse(item.Latitude.ToString()), double.Parse(item.Longitude.ToString()),
            //                        double.Parse(item.GPSAltitude.ToString()));
            //        }
            //        else
            //        {
            //            bool exists = System.IO.Directory.Exists(Path.GetDirectoryName(enum3.Current) + "\\processed\\");

            //            if (!exists)
            //                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(enum3.Current) + "\\processed\\");
            //            i++;
            //            System.IO.File.Copy(enum3.Current, Path.GetDirectoryName(enum3.Current) + "\\processed\\" + Path.GetFileName(enum3.Current), true);
            //        }

            //    }
            //}

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

        private static void writeToFilePPK(List<CameraLog> finalCoords, List<Picture> pictureName, string ImageFileName, string height_offset)
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

                item.GPSAltitude = item.GPSAltitude;
                if (item.q == 1)
                {
                    sb.Append(string.Format("{0}\t{1}\t{2}\t{3}\t", item.ImageName, item.Latitude.ToString(), item.Longitude.ToString(), (item.GPSAltitude - double.Parse(height_offset)).ToString()) + Environment.NewLine);
                }
                Quality.Append(string.Format("{0}\t{1}\t", item.ImageName, item.q.ToString()) + Environment.NewLine);


            }
            Quality.Insert(0, string.Format(Environment.NewLine));
            Quality.Insert(0, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t", "Percentages - FIX", (Math.Round((q1 / (q1 + q2 + q3 + q4 + q5)) * 100, 2)).ToString() + "%", "Float:", (Math.Round((q2 / (q1 + q2 + q3 + q4 + q5)) * 100, 2)).ToString(), "Single: ", (Math.Round((q3 / (q1 + q2 + q3 + q4 + q5)) * 100, 2)).ToString() + "%", "Q4: ", (Math.Round((q4 / (q1 + q2 + q3 + q4 + q5)) * 100, 2)).ToString() + "%", "Q5: ", (Math.Round((q5 / (q1 + q2 + q3 + q4 + q5)) * 100, 2)).ToString() + "%" + Environment.NewLine, 1));
            Quality.Insert(0, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t", "Number of points - FIX: ", q1.ToString(), "Float:", q2.ToString(), "Single: ", q3.ToString(), "Q4: ", q4, "Q5: ", q5.ToString()) + Environment.NewLine, 1);

            //changeimagefilename to imageppicture name
            using (StreamWriter outfile = new StreamWriter(ImageFileName + @"\ImageLog.txt"))
            {
                outfile.Write(sb.ToString());
            }
            using (StreamWriter outfile = new StreamWriter(ImageFileName + @"\QualityReport.txt"))
            {
                outfile.Write(Quality.ToString());
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
           if (comboBox1.Text == "Select/Add an Antenna")
            {
                camera_height_offset.Visible = true;
            }
            else
            {
                camera_height_offset.Visible = true;

                foreach (var profile in profiles)
                {
                    if (profile.Value.name.ToString() == comboBox1.Text.ToString())
                    {
                        camera_height_offset.Text = profile.Value.offset.ToString();
                    }
                }
            }
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            loadprofileList(false, Path.Combine(Environment.CurrentDirectory, "profiles.xml"));
        }
    }
}
