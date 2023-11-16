using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace ExternalDevices
{
    /// <summary>
    /// Klasse,die den Denon AVR in ein Objekt hält
    /// </summary>
    public static class Denon
    {
        #region InternalVariables
        //private static String mUrl;
        private static Uri mUrl;
        private static readonly String mXMLPath = "goform/formMainZone_MainZoneXmlStatusLite.xml";
        private static readonly String mInputPath = "/goform/formiPhoneAppDirect.xml?";
        private static DenonInputs _selectedDenonInputs;
        private static Boolean _PowerOn;
        private static String _volume;
        private static Boolean _muteOn;
        private static HttpClient _httpClient;

        private static readonly Dictionary<DenonInputs, string> DenonInputsToStringLookup = new Dictionary<DenonInputs, string>()
        {
            { DenonInputs.Phono         , "PHONO"       },
            { DenonInputs.Cd            , "CD"          },
            { DenonInputs.Tuner         , "TUNER"       },
            { DenonInputs.Dvd           , "DVD"         },
            { DenonInputs.Filme       , "BD"          },
            { DenonInputs.Tv            , "TV"          },
            { DenonInputs.SatCbl        , "SAT/CBL"     },
            { DenonInputs.Sonos   , "MPLAY"       },
            { DenonInputs.PS4          , "GAME"        },
            { DenonInputs.HdRadio       , "HDRADIO"     },
            { DenonInputs.Net           , "NET"         },
            { DenonInputs.Pandora       , "PANDORA"     },
            { DenonInputs.SiriusXm      , "SIRIUSXM"    },
            { DenonInputs.Spotify       , "SPOTIFY"     },
            { DenonInputs.LastFm        , "LASTFM"      },
            { DenonInputs.Flickr        , "FLICKR"      },
            { DenonInputs.IRadio        , "IRADIO"      },
            { DenonInputs.Server        , "SERVER"      },
            { DenonInputs.Favorites     , "FAVORITES"   },
            { DenonInputs.Aux1          , "AUX1"        },
            { DenonInputs.Aux2          , "AUX2"        },
            { DenonInputs.Aux3          , "AUX3"        },
            { DenonInputs.Aux4          , "AUX4"        },
            { DenonInputs.Aux5          , "AUX5"        },
            { DenonInputs.Aux6          , "AUX6"        },
            { DenonInputs.Aux7          , "AUX7"        },
            { DenonInputs.BlueTooth     , "BT"          },
            { DenonInputs.UsbIpod       , "USB/IPOD"    },
            { DenonInputs.Usb           , "USB"         },
            { DenonInputs.Ipd           , "IPD"         },
            { DenonInputs.Irp           , "IRP"         },
            { DenonInputs.Fvp           , "FVP"         },
        };
        private static readonly Dictionary<string, DenonInputs> StringToDenonInputsLookup = new Dictionary<String, DenonInputs>(){
            { "PHONO",DenonInputs.Phono},
            { "CD",DenonInputs.Cd},
            { "TUNER",DenonInputs.Tuner},
            {"BD", DenonInputs.Filme},
            {"TV", DenonInputs.Tv},
            { "SAT/CBL",DenonInputs.SatCbl},
            { "MPLAY",DenonInputs.Sonos},
            { "GAME",DenonInputs.PS4},
            { "NET",DenonInputs.Net}
        };

        #endregion InternalVariables
        /// <summary>
        /// Initialisieren des Denon mit IP
        /// </summary>
        /// <param name="IP">Ip des Denon</param>
        /// <returns></returns>
        public static async Task<bool> Initialisieren(IPAddress IP, int port = 8080)
        {
            try
            {
                if (String.IsNullOrEmpty(IP.ToString())) return false;
                mUrl = new Uri("http://" + IP.ToString() + ":" + port);
                return await Init();
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Initialisieren des Denon mit URL
        /// </summary>
        /// <param name="URL">URL mit oder ohne HTTP</param>
        /// <returns></returns>
        public static async Task<bool> Initialisieren(string URL, int port = 8080)
        {
            try
            {
                if (String.IsNullOrEmpty(URL)) return false;
                if (!URL.ToUpper().StartsWith("HTTP"))
                    URL = "http://" + URL + ":" + port;
                mUrl = new Uri(URL + ":" + port);
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
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }
                XmlDocument myXmlDocument = new();
                //Load Async
                HttpResponseMessage getResponse = await _httpClient.GetAsync(mUrl + mXMLPath);
                var response = getResponse.Content.ReadAsStringAsync();
                //end Async
                if (response == null || string.IsNullOrEmpty(response.Result)) return false;

                myXmlDocument.LoadXml(response.Result);
                //myXmlDocument.Load(mUrl + mXMLPath); //Load NOT LoadXml
                XmlNode powerstateNode = myXmlDocument.SelectSingleNode("descendant::Power");
                XmlNode inputFuncSelectNode = myXmlDocument.SelectSingleNode("descendant::InputFuncSelect");
                XmlNode volumeStateNode = myXmlDocument.SelectSingleNode("descendant::MasterVolume");
                XmlNode muteStateNode = myXmlDocument.SelectSingleNode("descendant::Mute");
                if (powerstateNode == null || inputFuncSelectNode == null || volumeStateNode == null) return false;
                _PowerOn = powerstateNode.InnerText == "ON";
                _volume = volumeStateNode.InnerText;
                _muteOn = muteStateNode.InnerText == "ON";
                try
                {
                    _selectedDenonInputs = StringToDenonInputsLookup[inputFuncSelectNode.InnerText];
                }
                catch
                {
                    if (inputFuncSelectNode.InnerText.ToLower().Contains("tv"))
                    {
                        _selectedDenonInputs = DenonInputs.Tv;
                    }
                    else
                    {
                        _selectedDenonInputs = DenonInputs.Unknown;
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
        private async static void DenonInput(string inp)
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
            }
            _= await _httpClient.GetAsync(mUrl + "/" + mInputPath+inp);
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
                DenonInput("MV" + _volume);
            }
        }
        /// <summary>
        /// Denon Ein und Ausschalten
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
                    DenonInput("ZMON");
                }
                else
                {
                    DenonInput("ZMOFF");
                }
            }

        }
        /// <summary>
        /// Denon Mute State
        /// </summary>
        public static Boolean MuteOn
        {
            get { return _muteOn; }
            set
            {
                if (value == _muteOn) return;
                _muteOn = value;
                if (_muteOn)
                {
                    DenonInput("MUON");
                }
                else
                {
                    DenonInput("MUOFF");
                }
            }

        }
        /// <summary>
        /// Ausgewählte Quelle Setzen
        /// </summary>
        public static DenonInputs SelectedInput
        {
            get
            {
                return _selectedDenonInputs;
            }
            set
            {
                if (value == _selectedDenonInputs) return;
                DenonInput("SI" + DenonInputsToStringLookup[value]);
                _selectedDenonInputs = value;

            }
        }
    }
    /// <summary>
    /// Enum mit den möglichen Selektierbaren Quellen
    /// </summary>
    public enum DenonInputs
    {
        Phono,
        Cd,
        Tuner,
        Dvd,
        BlueRay,
        Tv,
        SatCbl,
        MediaPlayer,
        Game,
        HdRadio,
        Net,
        Pandora,
        SiriusXm,
        Spotify,
        LastFm,
        Flickr,
        IRadio,
        Server,
        Favorites,
        Aux1,
        Aux2,
        Aux3,
        Aux4,
        Aux5,
        Aux6,
        Aux7,
        BlueTooth,
        UsbIpod,
        Usb,
        Ipd,
        Irp,
        Fvp,
        //Own Renamed
        Sonos,
        Filme,
        PS4,
        Unknown
    }

}
