using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ExternalDevices
{
    /// <summary>
    /// Klasse,die den Marantz AVR in ein Objekt hält
    /// </summary>
    public static class Marantz
    {
        #region InternalVariables
        //private static String mUrl;
        private static Uri mUrl;
        private static readonly String mXMLPath = "/goform/formMainZone_MainZoneXml.xml";
        private static readonly String mInputPath = "MainZone/index.put.asp";
        private static MarantzInputs _selectedMarantzInputs;
        private static Boolean _PowerOn;
        private static String _volume;
        private static HttpClient _httpClient;
        #endregion InternalVariables
        /// <summary>
        /// Initialisieren des Marantz mit IP
        /// </summary>
        /// <param name="IP">Ip des Marantz</param>
        /// <returns></returns>
        public static async Task<bool> Initialisieren(IPAddress IP)
        {
            try
            {
                if (String.IsNullOrEmpty(IP.ToString())) return false;
                mUrl = new Uri("http://"+ IP.ToString());
                return await Init();
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Initialisieren des Marantz mit URL
        /// </summary>
        /// <param name="URL">URL mit oder ohne HTTP</param>
        /// <returns></returns>
        public static async Task<bool> Initialisieren(string URL)
        {
            try
            {
                if (String.IsNullOrEmpty(URL)) return false;
                if (!URL.ToUpper().StartsWith("HTTP"))
                    URL = "http://" + URL;
                mUrl = new Uri(URL);
                return await Init();
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Initialisieren des Marantz mit URI
        /// </summary>
        /// <param name="url">URI</param>
        /// <returns></returns>
        public static async Task<bool> Initialisieren(Uri url)
        {
            try
            {
                if (String.IsNullOrEmpty(url.AbsoluteUri)) return false;
                mUrl = url;
                return await Init();
            }
            catch
            {
                return false;
            }
        }
        private static async Task<Boolean> Init()
        {
            try
            {
                if(_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }
                XmlDocument myXmlDocument = new();
                //Load Async
                HttpResponseMessage getResponse = await _httpClient.GetAsync(mUrl + mXMLPath);
                var response = getResponse.Content.ReadAsStringAsync();
                //end Async
                if(response == null || string.IsNullOrEmpty(response.Result)) return false;

                myXmlDocument.LoadXml(response.Result);
                //myXmlDocument.Load(mUrl + mXMLPath); //Load NOT LoadXml
                XmlNode powerstateNode = myXmlDocument.SelectSingleNode("descendant::Power");
                XmlNode inputFuncSelectNode = myXmlDocument.SelectSingleNode("descendant::InputFuncSelect");
                XmlNode volumeStateNode = myXmlDocument.SelectSingleNode("descendant::MasterVolume");
                if (powerstateNode == null || inputFuncSelectNode == null || volumeStateNode == null) return false;
                _PowerOn = powerstateNode.InnerText == "ON";
                _volume = volumeStateNode.InnerText;
                try
                {
                    _selectedMarantzInputs = (MarantzInputs)Enum.Parse(typeof(MarantzInputs), inputFuncSelectNode.InnerText);
                }
                catch
                {
                    if (inputFuncSelectNode.InnerText.ToLower().Contains("tv"))
                    {
                        _selectedMarantzInputs = MarantzInputs.TV;
                    }
                    else
                    {
                        _selectedMarantzInputs = MarantzInputs.Unknowing;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Change the Input
        /// </summary>
        /// <param name="inp"></param>
        private async static void MarantzInput(string inp)
        {
            if(_httpClient == null)
            {
                _httpClient = new HttpClient();
            }
            byte[] byteArray = Encoding.UTF8.GetBytes(inp);
            HttpContent httpContent = new ByteArrayContent(byteArray);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            httpContent.Headers.ContentLength = byteArray.Length;
            HttpResponseMessage getResponse = await _httpClient.PostAsync(mUrl + "/" + mInputPath, httpContent);
        }
        /// <summary>
        /// Lautstärke Setzen (Format "-30.0")
        /// </summary>
        public static string Volume
        {
            get { return _volume; }
            set
            {
                if (value == _volume) return;
                if (!_volume.StartsWith("-") || (!_volume.EndsWith(".0") && !_volume.EndsWith(".5"))) return;
                _volume = value;
                MarantzInput("cmd0=PutMasterVolumeSet%2F" + _volume);
            }
        }
        /// <summary>
        /// Marantz Ein und Ausschalten
        /// </summary>
        public static Boolean PowerOn
        {
            get { return _PowerOn; }
            set
            {
                if (value == _PowerOn) return;
                _PowerOn = value;
                if (_PowerOn)
                {
                    MarantzInput("cmd0=PutZone_OnOff%2FON&cmd1=aspMainZone_WebUpdateStatus%2F");
                }
                else
                {
                    MarantzInput("cmd0=PutZone_OnOff%2FOFF&cmd1=aspMainZone_WebUpdateStatus%2F");
                }
            }

        }
        /// <summary>
        /// Ausgewählte Quelle Setzen
        /// </summary>
        public static MarantzInputs SelectedInput
        {
            get
            {
                return _selectedMarantzInputs;
            }
            set
            {
                if (value == _selectedMarantzInputs) return;
                switch (value)
                {
                    case MarantzInputs.Sonos:
                        MarantzInput("cmd0=PutZone_InputFunction%2FCD&cmd1=aspMainZone_WebUpdateStatus%2F");
                        break;
                    case MarantzInputs.Film:
                        MarantzInput("cmd0=PutZone_InputFunction%2FBD&cmd1=aspMainZone_WebUpdateStatus%2F");
                        break;
                    case MarantzInputs.Wii:
                        MarantzInput("cmd0=PutZone_InputFunction%2FDVD&cmd1=aspMainZone_WebUpdateStatus%2F");
                        break;
                    case MarantzInputs.TV:
                    case MarantzInputs.PS4:
                        MarantzInput("cmd0=PutZone_InputFunction%2FTV&cmd1=aspMainZone_WebUpdateStatus%2F");
                        break;
                    case MarantzInputs.TUNER:
                        MarantzInput("cmd0=PutZone_InputFunction%2FTUNER&cmd1=aspMainZone_WebUpdateStatus%2F");
                        break;
                }
                _selectedMarantzInputs = value;

            }
        }
    }
    /// <summary>
    /// Enum mit den möglichen Selektierbaren Quellen
    /// </summary>
    public enum MarantzInputs
    {
        Sonos,
        Film,
        Wii,
        PS4,
        TV,
        TUNER,
        Unknowing
    }
}
