using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Xml;

namespace E38_Delay_Utility
{
    public partial class Delay : Form
    {
        public Delay()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;

            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, label1.Width, label1.Height);

            
            this.label2.Region = new Region(path);
            this.label3.Region = new Region(path);
            this.label4.Region = new Region(path);

        }
        Dictionary<string, camerainfo> cameras = new Dictionary<string, camerainfo>();
        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {

                openFileDialog1.Filter = "Obs, SBP (*.obs,*.SBP)|*.obs;*.sbp;";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;

                try
                {
                    openFileDialog1.InitialDirectory = "C://";
                }
                catch { } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    string fileExt = Path.GetExtension(openFileDialog1.ToString());
                    if (fileExt == ".sbp")
                    {
                        Pos_FilePath.Text = openFileDialog1.FileName;
                        Process process = Process.Start(Path.Combine(Environment.CurrentDirectory, "sbp2rinex.exe"), ("rinex2sbp " + @"""" + openFileDialog1.FileName.ToString() + @""""));
                        int id = process.Id;
                        Process tempProc = Process.GetProcessById(id);
                        this.Visible = false;
                        tempProc.WaitForExit();
                        this.Visible = true;
                        Pos_FilePath.Text = (Path.GetDirectoryName(openFileDialog1.FileName.ToString()) + Path.DirectorySeparatorChar + (Path.GetFileNameWithoutExtension(openFileDialog1.FileName.ToString()) + ".obs"));
                    }
                    else
                    {
                        Pos_FilePath.Text = openFileDialog1.FileName;
                    }
                    Pos_FilePath.Visible = true;
                }
            }


        }
        private void Delay_Click_Click(object sender, System.EventArgs e)
        {
           bool inList = false;

            foreach (var camera in cameras.Values)
            {
                if (camera.name == comboBox1.Text || comboBox1.Text == "R10C" || comboBox1.Text == "RX1RII" || comboBox1.Text == "Phantom 4k" || comboBox1.Text == "Select Camera") 
                {
                    inList = true;
                }
            }
         
            if (!inList)
            {
                try
                {
                    camerainfo newcamera = new camerainfo();
                    newcamera.delay = decimal.Parse(textBox1.Text);
                    newcamera.name = comboBox1.Text;
                    cameras.Add("camera", newcamera);
                    loadCameraList(true, Path.Combine(Environment.CurrentDirectory, "cameras.xml"));
                }
                catch
                {

                }
            }

            System.IO.StreamReader file = new System.IO.StreamReader(Pos_FilePath.Text);
            string rnxver = file.ReadLine();


            if (rnxver.Contains("2.11"))
            {
                rinex211();
            }
            else if (rnxver.Contains("3.0"))
            {
                rinex3();
            }




        }

        public static double ConvertSecondsToMilliseconds(double seconds)
        {
            return TimeSpan.FromSeconds(seconds).TotalMilliseconds;
        }

        public static double ConvertMinutesToMilliseconds(double minutes)
        {
            return TimeSpan.FromMinutes(minutes).TotalMilliseconds;
        }

        public static double ConvertHoursToMilliseconds(double hours)
        {
            return TimeSpan.FromHours(hours).TotalMilliseconds;
        }

        public struct camerainfo
        {
            public string name;
            public decimal delay;
        }

        public void loadCameraList(bool write, string filename)
        {
            bool exists = File.Exists(filename);

            if (write || !exists)
            {
                try
                {
                    XmlTextWriter xmlwriter = new XmlTextWriter(filename, Encoding.ASCII);
                    xmlwriter.Formatting = Formatting.Indented;

                    xmlwriter.WriteStartDocument();

                    xmlwriter.WriteStartElement("Cameras");

                    foreach (string key in cameras.Keys)
                    {
                        try
                        {
                            if (key == "")
                                continue;
                            xmlwriter.WriteStartElement("Camera");
                            xmlwriter.WriteElementString("name", cameras[key].name);
                            xmlwriter.WriteElementString("delay", cameras[key].delay.ToString(new System.Globalization.CultureInfo("en-US")));
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
                                    case "Camera":
                                        {
                                            camerainfo camera = new camerainfo();

                                            while (xmlreader.Read())
                                            {
                                                bool dobreak = false;
                                                xmlreader.MoveToElement();
                                                switch (xmlreader.Name)
                                                {
                                                    case "name":
                                                        camera.name = xmlreader.ReadString();
                                                        break;
                                                    case "delay":
                                                        camera.delay = decimal.Parse(xmlreader.ReadString(), new System.Globalization.CultureInfo("en-US"));
                                                        break;
                                                    case "Camera":
                                                        cameras[camera.name] = camera;
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
                foreach (var camera in cameras.Values)
                {
                    if (!comboBox1.Items.Contains(camera.name))
                        comboBox1.Items.Add(camera.name);
                }
            }
        }

        private void rinex211()
        {
            decimal msdelay = 0m;
            if (comboBox1.Text == "R10C")
            {       //shutter happens -.235ms before picture is taken. 205 is from the camera 30 from tiny circuit
                msdelay = -0.235m;
                textBox1.Visible = false;

            }
            else if (comboBox1.Text == "RX1RII")
            {
                msdelay = 0m;
                textBox1.Visible = false;
            }
            else if (comboBox1.Text == "Phantom 4k")
            {
                msdelay = -0.055m;
                textBox1.Visible = false;
            }
            else if (comboBox1.Text == "Select Camera")
            {
                textBox1.Visible = true;
            }
            else
            {
                msdelay = decimal.Parse(textBox1.Text.ToString());
                textBox1.Visible = true;
            }


            string[] records = File.ReadAllLines(Pos_FilePath.Text);
            bool shiftStarted = false;

            List<PulseLine> pulseLine = new List<PulseLine>();

            List<string> newFile = new List<string>();
            int i = 0;
            int r = 0;
            int z = 0;

            foreach (string line1 in records)
            {

                if (shiftStarted == false)
                {
                    if (line1.ToString().Length > 48 && line1.ToString().Length < 80)
                    {
                        z++;
                    }
                }

                i++;
                if (line1.Contains(" 5  0") && line1.Length < 40)
                {
                    string line3 = ConvertWhitespacesToSingleSpaces(line1);
                    bool first = true;
                    shiftStarted = true;
                    PulseLine recordPulse = new PulseLine();
                    string[] line2 = line3.Split(new[] { ' ' });
                    //reset r
                    r = 0;
                    int shift = z;
                    //start and reset enum
                    var readLine1 = records.GetEnumerator();

                    readLine1.Reset();
                    readLine1.MoveNext();


                    //add millis delay
                    line2[6] = (decimal.Parse(line2[6]) + msdelay).ToString();

                    //iterate through list untill space is found
                    while (readLine1.Current != null)
                    {   //check to appropriate format for line in list
                        if (!first)
                        {
                            readLine1.MoveNext();
                        }
                        first = false;

                        if ((readLine1.Current.ToString()).Length > 48 && (readLine1.Current.ToString()).Length < 80)
                        {
                            if (shift == 0)
                            {
                                r++;
                            }
                            if (shift > 0)
                            {
                                shift--;
                            }


                            //split and parse
                            string line4 = ConvertWhitespacesToSingleSpaces(readLine1.Current.ToString());
                            string[] split = (line4.Split(new[] { ' ' }));
                            if (split.Length > 8)
                            {
                                if (shift == 0)
                                {
                                    if (int.Parse(split[4]) > int.Parse(line2[4]))
                                    {
                                        recordPulse.shiftcounter = r + z;

                                        break;
                                    }
                                    if (int.Parse(split[4]) >= int.Parse(line2[4]) && int.Parse(split[5]) > int.Parse(line2[5]))
                                    {
                                        recordPulse.shiftcounter = r + z;

                                        break;
                                    }
                                    if (int.Parse(split[4]) >= int.Parse(line2[4]) && int.Parse(split[5]) >= int.Parse(line2[5]) && float.Parse(split[6]) > float.Parse(line2[6]))
                                    {
                                        recordPulse.shiftcounter = r + z;

                                        break;
                                    }
                                }

                            }

                        }







                    }
                    recordPulse.pulseLine = String.Join(" ", line2);

                    pulseLine.Add(recordPulse);
                    //delay X times then write
                }
            }
            var enum1 = pulseLine.GetEnumerator();

            foreach (string line1 in records)
            {

                if (line1.Contains(" 5  0") && line1.Length < 40)
                {
                    enum1.MoveNext();

                    int m = 0;
                    string[] line2 = line1.Split(new[] { ' ' });
                    foreach (string space in line2)
                    {
                        Regex.Replace(space, @"\s+", "");
                        line2[m] = space;
                        m++;

                    }
                    line2 = line2.Where(x => !string.IsNullOrEmpty(x)).ToArray();


                    line2[5] = (decimal.Parse(line2[5]) + msdelay).ToString();
                    if (decimal.Parse(line2[5]) < 0)
                    {
                        //if under 0 shift time from minutes column
                        line2[5] = (decimal.Parse(line2[5]) + 60m).ToString();
                        line2[4] = (decimal.Parse(line2[4]) - 1m).ToString();
                        //if minute shifts grab from minutes column
                        if (decimal.Parse(line2[4]) < 0)
                        {
                            line2[4] = (decimal.Parse(line2[4]) + 60m).ToString();
                            line2[3] = (decimal.Parse(line2[3]) - 1m).ToString();
                        }
                    }
                    m = 0;
                    foreach (string item in line2)
                    {
                        if (m == 0)
                        {
                            line2[m] = (" " + item);
                        }
                        try
                        {
                            if (decimal.Parse(item) < 10m)
                            {
                                line2[m] = (" " + item);
                            }
                        }
                        catch
                        { }
                        m++;
                    }
                    enum1.Current.pulseLine = String.Join(" ", line2);
                }
            }



            i = 0;
            int b = 0;
            shiftStarted = false;
            var enumerator = pulseLine.GetEnumerator();
            enumerator.MoveNext();

            foreach (string line in records)
            {
                if ((line.Length) > 48 && (line.Length < 80))
                {
                    b++;
                }
                bool skip = false;
                if (line.Contains(" 5  0") && line.Length < 40)
                {
                    if (enumerator.Current != null)
                    {
                        if (enumerator.Current.shiftcounter == 0)
                        {

                            newFile.Add(enumerator.Current.pulseLine);
                            enumerator.MoveNext();
                        }
                        shiftStarted = true;
                        skip = true;
                    }
                    skip = true;
                }

                if (enumerator.Current != null)
                {
                    if ((enumerator.Current.shiftcounter) == (b))
                    {
                        newFile.Add(enumerator.Current.pulseLine);
                        enumerator.MoveNext();
                    }



                }
                if (!skip)
                {
                    newFile.Add(line);
                }

            }

            using (System.IO.StreamWriter file =
          new System.IO.StreamWriter(Pos_FilePath.Text + "_" + "formatted.obs"))
            {
                foreach (string line in newFile)
                {
                    file.WriteLine(line);

                }
            }
        }

        private void rinex3()
        {
            decimal msdelay = 0m;
            if (comboBox1.Text == "R10C")
            {       //shutter happens -.235ms before picture is taken. 205 is from the camera 30 from tiny circuit
                msdelay = -0.235m;
                textBox1.Visible = false;
            }
            else if (comboBox1.Text == "RX1RII")
            {
                textBox1.Visible = false;
            }
            else if (comboBox1.Text == "Phantom 4k")
            {
                textBox1.Visible = false;
            }
            else if (comboBox1.Text == "Select Camera")
            {
                textBox1.Visible = true;
            }
            else
            {
                msdelay = decimal.Parse(textBox1.Text.ToString());
                textBox1.Visible = true;
            }

            string[] records = File.ReadAllLines(Pos_FilePath.Text);
            List<string> records2 = new List<string>();
            List<EventLine> satLine = new List<EventLine>();
            List<EventLine> eventline = new List<EventLine>();

            foreach (string line in records)
            {
                EventLine curEvent = new EventLine();

                if (line.Contains(">"))
                {



                    if (line.Contains(" 5  0") && line.Length < 40)
                    {

                        string whitespace = ConvertWhitespacesToSingleSpaces(line);
                        string[] whitespace2;
                        whitespace2 = whitespace.Split(new[] { ' ' });
                        whitespace2[6] = (decimal.Parse(whitespace2[6]) + msdelay).ToString();
                        if (decimal.Parse(whitespace2[6]) < 0)
                        {
                            //if under 0 shift time from minutes column
                            whitespace2[6] = (decimal.Parse(whitespace2[6]) + 60m).ToString();
                            whitespace2[5] = (decimal.Parse(whitespace2[5]) - 1m).ToString();
                            if (decimal.Parse(whitespace2[5]) < 0)
                            {
                                whitespace2[5] = (decimal.Parse(whitespace2[5]) + 60m).ToString();
                                whitespace2[4] = (decimal.Parse(whitespace2[4]) - 1m).ToString();

                                if (decimal.Parse(whitespace2[4]) < 0)
                                {
                                    whitespace2[4] = (decimal.Parse(whitespace2[4]) + 24m).ToString();
                                    whitespace2[3] = (decimal.Parse(whitespace2[3]) - 1m).ToString();

                                }

                            }

                        }
                        curEvent.year = double.Parse(whitespace2[1]);
                        curEvent.month = double.Parse(whitespace2[2]);
                        curEvent.day = double.Parse(whitespace2[3]);
                        curEvent.hour = double.Parse(whitespace2[4]);
                        curEvent.min = double.Parse(whitespace2[5]);
                        curEvent.sec = double.Parse(whitespace2[6]);

                        eventline.Add(curEvent);
                    }
                    else
                    {

                        string whitespace = ConvertWhitespacesToSingleSpaces(line);
                        string[] whitespace2;
                        whitespace2 = whitespace.Split(new[] { ' ' });
                        curEvent.year = double.Parse(whitespace2[1]);
                        curEvent.month = double.Parse(whitespace2[2]);
                        curEvent.day = double.Parse(whitespace2[3]);
                        curEvent.hour = double.Parse(whitespace2[4]);
                        curEvent.min = double.Parse(whitespace2[5]);
                        curEvent.sec = double.Parse(whitespace2[6]);
                        curEvent.otherDat = double.Parse(whitespace2[7]);
                        satLine.Add(curEvent);
                        records2.Add(line);
                    };



                }

                else { records2.Add(line); }

            }
            var eventEnum = eventline.GetEnumerator();
            var satEnum = satLine.GetEnumerator();

            List<string> records3 = new List<string>();
            double prevsat = 71;
            eventEnum.MoveNext();
            bool skipthis = false;
            foreach (string line in records2)
            {
                EventLine curEvent = new EventLine();

                if (line.Contains(">"))
                {

                    satEnum.MoveNext();
                    if (satEnum.Current != null)
                    {
                        if (satEnum.Current.sec == prevsat)
                        {
                            skipthis = true;
                        }
                        else skipthis = false;
                    }
                    if (eventEnum.Current != null && satEnum.Current != null)
                    {

                        double eventTime = ConvertSecondsToMilliseconds(eventEnum.Current.sec) + ConvertMinutesToMilliseconds(eventEnum.Current.min) + ConvertHoursToMilliseconds(eventEnum.Current.hour);
                        double satTime = ConvertSecondsToMilliseconds(satEnum.Current.sec) + ConvertMinutesToMilliseconds(satEnum.Current.min) + ConvertHoursToMilliseconds(satEnum.Current.hour);

                        if (eventTime <= satTime)
                        {

                            records3.Add(">" + addwhiteSpaces(eventEnum.Current.year) + addwhiteSpaces(eventEnum.Current.month) + addwhiteSpaces(eventEnum.Current.day) + addwhiteSpaces(eventEnum.Current.hour) + addwhiteSpaces(eventEnum.Current.min) + (addwhiteSpacesseconds(eventEnum.Current.sec)).PadLeft(1, '0') + "  5  0");
                            eventEnum.MoveNext();
                        }



                    }
                    if (satEnum.Current != null)
                    {
                        prevsat = satEnum.Current.sec;
                    }
                    else skipthis = false;

                    if (!skipthis)
                    {
                        records3.Add(line);
                    }
                }

                else if (!skipthis) { records3.Add(line); }

            }


            using (System.IO.StreamWriter file =
new System.IO.StreamWriter(Pos_FilePath.Text + "_" + "formatted.obs"))
            {
                foreach (string line in records3)
                {
                    file.WriteLine(line);

                }
            }


        }

        public static string addwhiteSpaces(double value)
        {

            if (value < 10)
            {
                return ("  " + value.ToString());
            }

            else return (" " + value.ToString());

        }
        public static string addwhiteSpacesseconds(double value)
        {

            if (value < 1)
            {
                return ("  " + value.ToString("#0.0000000"));
            }
            else if (value < 10)
            {
                return ("  " + value.ToString("##.0000000"));
            }

            else return (" " + value.ToString("##.0000000"));

        }
        public static string ConvertWhitespacesToSingleSpaces(string value)
        {
            value = Regex.Replace(value, @"\s+", " ");
            return value;
        }
        public static Boolean isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z\s.,]*$");
            return rg.IsMatch(strToCheck);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadCameraList(false, Path.Combine(Environment.CurrentDirectory, "cameras.xml"));
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox1.Text == "R10C")
            {       //shutter happens -.235ms before picture is taken. 205 is from the camera 30 from tiny circuit
                textBox1.Visible = false;

            }
            else if (comboBox1.Text == "RX1RII")
            {
                textBox1.Visible = false;
            }
            else if (comboBox1.Text == "Phantom 4k")
            {
                textBox1.Visible = false;
            }
            else if (comboBox1.Text == "Select/Add Camera")
            {
                textBox1.Visible = true;
            }
            else
            {
                textBox1.Visible = true;
                
                foreach (var camera in cameras)
                {
                    if (camera.Value.name.ToString() == comboBox1.Text.ToString())
                    {
                        textBox1.Text = camera.Value.delay.ToString();
                    }
                }
            }
        }
    }
}
