using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GPSMapFile
{
    public partial class Form1 : Form
    {

        #region GlobalVariebles
        private List<GGA> dataGGA = new List<GGA>();
        private List<GSA> dataGSA = new List<GSA>();
        private List<RMC> dataRMC = new List<RMC>();

        private const int godIndexShift = 138;

        #endregion


        public Form1()
        {

            InitializeComponent();

        }

        #region Eventsna kliknuti

        private void btnReadGPS_Click(object sender, EventArgs e)
        {
            string seznamCtenychSouboru = ListFileTerminalsOpen();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = seznamCtenychSouboru;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DivideDataByType(ReadDataFromFile(openFileDialog1.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Pojebalo se to nekde pri nacitani Puvodni error: " + ex.Message);
                }
            }

        }


        private void ExportToArray_Click(object sender, EventArgs e)
        {
            double[] speedData = TakeGodSpeed(TakeSpeedFromRMCVector(),godIndexShift);
            double[,] xyData = TakeGodXYRMC(TakeXYRMC(), godIndexShift);
            DisplayDataSpeed(speedData);

            string[] lines = MakeStrinForJavaScriptArray(xyData,speedData);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string seznamCtenychSouboru = ListFileTerminalsSave();
            saveFileDialog.Filter = seznamCtenychSouboru;
            saveFileDialog.RestoreDirectory = true;
            

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1:
                            File.WriteAllLines(saveFileDialog.FileName + ".js", lines);
                            break;
                        default:
                            File.WriteAllLines(saveFileDialog.FileName + ".txt", lines);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Pojebalo se to nekde pri ukladani Puvodni error: " + ex.Message);
                }
            }
        }

        #endregion

        #region Splitovani a parsovani dat

        private void DivideDataByType(string[] inputDataFromFile)
        {
            for (int i = 0; i < inputDataFromFile.Length; i++)
            {
                string checkSum = GetChecksum(inputDataFromFile[i]);
                string[] transferLevel = inputDataFromFile[i].Split(',');

                if (transferLevel[0] == "$GPGGA")
                {
                    GGA ggaItem = GetGGAData(transferLevel);
                    if (ggaItem.Checksum.Substring(1, 2) == checkSum)
                    {
                        dataGGA.Add(ggaItem);
                    }
                    else
                    {
                        break;
                    }
                }
                if (transferLevel[0] == "$GPGSA")
                {
                    GSA gsaItem = GetGSAData(transferLevel);
                    if (gsaItem.Checksum == checkSum)
                    {
                        dataGSA.Add(gsaItem);
                    }
                    else
                    {
                        break;
                    }
                }

                if (transferLevel[0] == "$GPRMC")
                {
                    RMC rmcItem = GetRMCData(transferLevel);
                    if (rmcItem.Checksum == checkSum)
                    {
                        dataRMC.Add(rmcItem);
                    }
                    else
                    {
                        break;
                    }
                }

            }

        }

        private GGA GetGGAData(string[] splitedRow)
        {
            ////        $GPGGA 0,123519 1,4807.038 2,N 3,01131.000 4,E 5,1 6,08 7,0.9 8,545.4 9,M 10,46.9 11,M 12, 13,*47 14
            GGA rData = new GGA();
            splitedRow = ReplaceSeparator(splitedRow, ".", ",");
            splitedRow = ReplaceNull(splitedRow);
            rData.Name = splitedRow[0];
            rData.Time = splitedRow[1];
            rData.CharLatitude = splitedRow[3];
            rData.CharLongitude = splitedRow[5];
            rData.Latitude = ConvetToDEgLat(splitedRow[2], rData.CharLatitude);
            rData.Longitude = ConvetToDEgLong(splitedRow[4], rData.CharLongitude);
            rData.FixQuality = Convert.ToInt32(splitedRow[6]);
            rData.NuberSat = Convert.ToInt32(splitedRow[7]);
            rData.HorizontalDilution = Convert.ToDouble(splitedRow[8]);
            rData.Altitude = Convert.ToDouble(splitedRow[9]);
            rData.HeightOfGeoid = Convert.ToDouble(splitedRow[11]);
            rData.Checksum = splitedRow[14];

            return rData;
        }

        private GSA GetGSAData(string[] splitedRow)
        {

            //        $GPGSA 0,A 1,3 2,04 3,05 4, 5,09 6,12 7, 8, 9,24 10, 11, 12, 13, 14,2.5 15,1.3 16,2.1*39 17
            GSA rData = new GSA();
            splitedRow = ReplaceSeparator(splitedRow, ".", ",");
            splitedRow = ReplaceNull(splitedRow);
            string[] pomoc;
            if (String.IsNullOrWhiteSpace(splitedRow[17]))
            {
                pomoc = new string[2] { "0", "0" };
            }
            else
            {
                pomoc = splitedRow[17].Split('*');
            }

            rData.Name = splitedRow[0];
            rData.SelectionMode = splitedRow[1];
            rData.ThreeDFix = Convert.ToInt32(splitedRow[2]);
            for (int i = 0; i < 12; i++)
            {
                rData.PRNsOfSatellites[i] = Convert.ToDouble(splitedRow[i + 3]);
            }
            rData.PDOP = Convert.ToDouble(splitedRow[15]);
            rData.HDOP = Convert.ToDouble(splitedRow[16]);
            rData.VDOP = Convert.ToDouble(String.IsNullOrWhiteSpace(pomoc[0]) ? "0" : pomoc[0]);
            rData.Checksum = pomoc[1];

            return rData;
        }

        private RMC GetRMCData(string[] splitedRow)
        {

            //        $GPRMC 0,123519 1,A 2,4807.038 3,N 4,01131.000 5,E 6,022.4 7,084.4 8,230394 9,003.1 10,W*6A 11

            RMC rData = new RMC();
            splitedRow = ReplaceSeparator(splitedRow, ".", ",");
            splitedRow = ReplaceNull(splitedRow);
            string[] pomoc;
            if (String.IsNullOrWhiteSpace(splitedRow[12]))
            {
                pomoc = new string[2] { "0", "0" };
            }
            else
            {
                pomoc = splitedRow[12].Split('*');
            }
            rData.Name = splitedRow[0];
            rData.Time = splitedRow[1];
            rData.Status = Convert.ToChar(splitedRow[2]);
            rData.CharLongitude = (splitedRow[6]);
            rData.CharLatitude = (splitedRow[4]);
            rData.Longitude = ConvetToDEgLong(splitedRow[5], rData.CharLongitude);
            rData.Latitude = ConvetToDEgLat(splitedRow[3], rData.CharLatitude);
            rData.SpeedOverGround = Convert.ToDouble(splitedRow[7]);
            rData.TrackAngleDegree = Convert.ToDouble(splitedRow[8]);
            rData.Date = splitedRow[9];
            rData.MagneticVariation = Convert.ToDouble(splitedRow[10]);
            rData.Checksum = pomoc[1];

            return rData;
        }

        #endregion

        #region Zakladni vztahovani dat z naparsovaneho souboru

        public double[] TakeHeightFromVectorGGA()
        {
            double[] heiArr = new double[dataGGA.Count()];
            int iter = 0;
            foreach (GGA alti in dataGGA)
            {
                heiArr[iter] = alti.Altitude;
                iter++;
            }
            return heiArr;

        }

        public double[] TakeNumberSatFromGSAVector()
        {
            double[] satArr = new double[dataGSA.Count()];
            int iter = 0;
            foreach (GSA nSat in dataGSA)
            {
                double sumaSat = 0;
                for (int i = 0; i < 12; i++)
                {
                    sumaSat += nSat.PRNsOfSatellites[i];
                }
                satArr[iter] = sumaSat;
                iter++;
            }
            return satArr;

        }

        public double[] TakeSpeedFromRMCVector()
        {
            double[] speedArr = new double[dataRMC.Count()];
            int iter = 0;
            foreach (RMC speed in dataRMC)
            {
                speedArr[iter] = speed.SpeedOverGround;
                iter++;
            }
            return speedArr;

        }

        public double[] TakeDiferenceBetweenSentence()
        {
            int delkosVecsa = dataRMC.Count() - 1;
            double[] senArr = new double[delkosVecsa];
            List<DateTime> casoveJednot = new List<DateTime>();
            foreach (RMC item in dataRMC)
            {

                casoveJednot.Add(new DateTime(
                    Convert.ToInt32("20" + item.Date.Substring(4, 2))/*y*/,
                    Convert.ToInt32(item.Date.Substring(2, 2))/*mon*/,
                    Convert.ToInt32(item.Date.Substring(0, 2))/*d*/,
                    Convert.ToInt32(item.Time.Substring(0, 2))/*h*/,
                    Convert.ToInt32(item.Time.Substring(2, 2))/*min*/,
                    Convert.ToInt32(item.Time.Substring(4, 2))/*m*/
                    ));

            }

            for (int i = 0; i < delkosVecsa; i++)
            {
                senArr[i] = (casoveJednot[i + 1] - casoveJednot[i]).Seconds;
            }


            return senArr;
        }

        public double[,] TakeXYRMC()
        {
            double[,] naseSourad = new double[2, dataRMC.Count()];
            int ite = 0;
            foreach (RMC sourad in dataRMC)
            {
                naseSourad[0, ite] = sourad.Longitude;
                naseSourad[1, ite] = sourad.Latitude;
                ite++;
            }

            return naseSourad;
        }

        public double[,] TakeXYGGA()
        {
            double[,] naseSourad = new double[2, dataGGA.Count()];
            int ite = 0;
            foreach (GGA sourad in dataGGA)
            {
                naseSourad[0, ite] = sourad.Longitude;
                naseSourad[1, ite] = sourad.Latitude;
                ite++;
            }

            return naseSourad;
        }

        public double[,] TakeGodXYRMC(double[,] inputArr, int shift)
        {
            double[,] naseSourad = new double[2, inputArr.GetLength(1) - shift];

            for (int i = 0; i < inputArr.GetLength(1) - shift; i++)
            {

                naseSourad[0, i] = inputArr[0, i + shift];
                naseSourad[1, i] = inputArr[1, i + shift];

            }

            return naseSourad;
        }

        public double[] TakeGodSpeed(double[] inputArr, int shift)
        {
            double[] naseSourad = new double[inputArr.Length - shift];

            for (int i = 0; i < inputArr.Length - shift; i++)
            {
                naseSourad[i] = (inputArr[i + shift]) * 1.852;
            }

            return naseSourad;
        }

        public string[] MakeStrinForHTML(double[,] inputArrXY)
        {
            string[] htmlFile = new string[inputArrXY.GetLength(1) + 2];//inputArrXY.GetLength(1)+2];
            htmlFile[0] = "var flightPlanCoordinates = [";
            for (int i = 0; i < inputArrXY.GetLength(1); i++)
            {
                htmlFile[i + 1] = "new google.maps.LatLng(" + Convert.ToString(inputArrXY[1, i]).Replace(",", ".") + "," + Convert.ToString(inputArrXY[0, i]).Replace(",", ".") + "),";
            }
            htmlFile[inputArrXY.GetLength(1) + 1] = "];";//inputArrXY.GetLength(1)+1]; =
            return htmlFile;
        }

        public string[] MakeStrinForJavaScriptArray(double[,] inputArrXY)
        {
            string[] htmlFile = new string[inputArrXY.GetLength(1) + 2];//inputArrXY.GetLength(1)+2];
            htmlFile[0] = "var markr = [";
            for (int i = 0; i < inputArrXY.GetLength(1); i++)
            {
                htmlFile[i + 1] = "["
                    + Convert.ToString(inputArrXY[1, i]).Replace(",", ".")
                    + ","
                    + Convert.ToString(inputArrXY[0, i]).Replace(",", ".")
                    + "],";
            }
            htmlFile[inputArrXY.GetLength(1) + 1] = "];";//inputArrXY.GetLength(1)+1]; =
            return htmlFile;
        }

        public string[] MakeStrinForJavaScriptArray(double[,] inputArrXY, double[] inputSpeedArray)
        {
            string[] htmlFile = new string[inputArrXY.GetLength(1) + 2];//inputArrXY.GetLength(1)+2];
            htmlFile[0] = "var policko = [";
            for (int i = 0; i < inputArrXY.GetLength(1); i++)
            {
                htmlFile[i + 1] = "["
                    + Convert.ToString(inputArrXY[1, i]).Replace(",", ".")
                    + ","
                    + Convert.ToString(inputArrXY[0, i]).Replace(",", ".")
                    + ","
                    + Convert.ToString(inputSpeedArray[i]).Replace(",", ".")
                    + "],";
            }
            htmlFile[inputArrXY.GetLength(1) + 1] = "];";//inputArrXY.GetLength(1)+1]; =
            return htmlFile;
        }

        #endregion

        #region Bezne rutiny

        /// <summary>
        /// Funkce pro nacitani dat ze souboru
        /// </summary>
        /// <param name="route">Cesta k souboru</param>
        private string[] ReadDataFromFile(string route)
        {
            return File.ReadAllLines(route);
        }

        private string ListFileTerminalsOpen()
        {
            List<string> cteneTypySouboru = new List<string>();
            cteneTypySouboru.Add("NMEA files (*.nmea)|*.nmea");
            cteneTypySouboru.Add("Javascr files (*.js*)|*.js*");
            cteneTypySouboru.Add("NMEA file (*.NMEA)|*.NMEA");
            cteneTypySouboru.Add("Textak file (*.txt)|*.txt");
            cteneTypySouboru.Add("Textak file (*.TXT)|*.TXT");
            cteneTypySouboru.Add("Html files (*.html*)|*.html*");
            cteneTypySouboru.Add("All files (*.*)|*.*");

            string seznamCtenychSouboru = "";

            for (int i = 0; i < cteneTypySouboru.Count(); i++)
            {
                if (i < cteneTypySouboru.Count() - 1)
                { seznamCtenychSouboru += cteneTypySouboru[i] + "|"; }
                else
                { seznamCtenychSouboru += cteneTypySouboru[i]; }
            }

            return seznamCtenychSouboru;

        }
        private string ListFileTerminalsSave()
        {
            List<string> cteneTypySouboru = new List<string>();
            cteneTypySouboru.Add("Javascr files (*.js*)|*.js*");
            cteneTypySouboru.Add("Textak file (*.txt)|*.txt");

            string seznamCtenychSouboru = "";

            for (int i = 0; i < cteneTypySouboru.Count(); i++)
            {
                if (i < cteneTypySouboru.Count() - 1)
                { seznamCtenychSouboru += cteneTypySouboru[i] + "|"; }
                else
                { seznamCtenychSouboru += cteneTypySouboru[i]; }
            }

            return seznamCtenychSouboru;

        }
        // kod prevzat z vyhledavani googlem na stack overFlow, nedela problem mi reverse engeneering a pripadne vylepseni
        //https://www.google.cz/search?q=houw++enumarate+check+sum&ie=utf-8&oe=utf-8&aq=t&rls=org.mozilla:en-US:unofficial&client=firefox-aurora&channel=sb&gfe_rd=cr&ei=0zYvU5fEMqeK8Qegg4HoAw#channel=sb&q=how+enumerate+checksum+nmea+stack+overflow&rls=org.mozilla:en-US:unofficial
        //http://stackoverflow.com/questions/5089791/nmea-checksum-in-c-sharp-net-cf

        private static string GetChecksum(string sentence)
        {
            //Start with first Item
            int checksum = Convert.ToByte(sentence[sentence.IndexOf('$') + 1]);
            // Loop through all chars to get a checksum
            for (int i = sentence.IndexOf('$') + 2; i < sentence.IndexOf('*'); i++)
            {
                // No. XOR the checksum with this character's value
                checksum ^= Convert.ToByte(sentence[i]);
            }
            // Return the checksum formatted as a two-character hexadecimal
            return checksum.ToString("X2");
        }

        private double ConvetToDEgLong(string nonDegValue, string logn)
        {
            if (logn == "E" || logn == "e")
            {
                return Convert.ToDouble(nonDegValue.Substring(0, 3)) + (Convert.ToDouble(nonDegValue.Substring(3, 5)) / (60));
            }
            if (logn == "W" || logn == "w")
            {
                return -(Convert.ToDouble(nonDegValue.Substring(0, 3)) + (Convert.ToDouble(nonDegValue.Substring(3, 5)) / (60)));
            }
            else
            {
                return 0;
            }
        }

        private double ConvetToDEgLat(string nonDegValue, string lat)
        {
            if (lat == "N" || lat == "n")
            {
                return Convert.ToDouble(nonDegValue.Substring(0, 2)) + (Convert.ToDouble(nonDegValue.Substring(2, 6)) / (60));
            }
            if (lat == "S" || lat == "s")
            {
                return -(Convert.ToDouble(nonDegValue.Substring(0, 2)) + (Convert.ToDouble(nonDegValue.Substring(2, 6)) / (60)));
            }
            else
            {
                return 0;
            }

        }

        #region rReplace metody

        private string[] ReplaceSeparator(string[] inputStringArray, string oldSeparator, string newSeparator)
        {
            string[] newArrayWithNewSeparator = new string[inputStringArray.Length];
            for (int i = 0; i < inputStringArray.Length; i++)
            {
                newArrayWithNewSeparator[i] = inputStringArray[i].Replace(oldSeparator, newSeparator);
            }
            return newArrayWithNewSeparator;
        }

        /// <summary>
        /// Funkce pro vykresleni dat
        /// </summary>
        /// <param name="inputDataArray">Nacita vstupni double pole</param>
        /// <param name="nameLine">Nazev rady</param>
        public string[] ReplaceNull(string[] inputStringArray)
        {

            for (int i = 0; i < inputStringArray.Length; i++)
            {
                if (String.IsNullOrWhiteSpace(inputStringArray[i]))
                {
                    inputStringArray[i] = "0";
                }
            }
            return inputStringArray;
        }

        #endregion

        private void DisplayDataSpeed(double[] speed)
        {
            //30+(6%(speed[i]))*36
            tBCol.Text = null;
            for (int i = 0; i < speed.Length; i+=20)
            {

                tBCol.Text += speed[i].ToString() + ", " + ((speed[i])%6).ToString() + Environment.NewLine;
                
            }
        }

        #endregion
    }
}
