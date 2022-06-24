using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using PlaylistGenerator;

namespace PlaylistWPF
{
    /// <summary>
    /// Interaktionslogik für GeneratePlaylists.xaml
    /// </summary>
    public partial class GeneratePlaylists:IDisposable
    {
        private readonly BackgroundWorker initial = new();
        private Boolean readedplaylists;
        bool cancelerror;
        public GeneratePlaylists()
        {
            InitializeComponent();
            initial.DoWork += Initial_DoWork;
            initial.RunWorkerCompleted += Initial_RunWorkerCompleted;
            initial.ProgressChanged += Initial_ProgressChanged;
            initial.WorkerReportsProgress = true;
            initial.RunWorkerAsync();
            prb1.Value = 0;
            lbprb.Content = "Variablen werden ausgelesen.";
        }

        public void Dispose()
        {
            initial?.Dispose();
        }

        private void Initial_DoWork(object sender, DoWorkEventArgs e)
        {
            initial.ReportProgress(0, "Variablen werden ausgelesen");
            bool playlistexist = Playlists.GetPlaylistNames.Count > 0;
            //Alles initialisieren und mit Abfragen bei Fehlern ausführen, wenn kein Autorun gemacht wurde. Autorun wird unten verarbeitet.
            if (Functions.Initialisieren(playlistexist) && !Functions.Autorun)
            {
                if (Playlists.GetPlaylistNames.Count == 0)
                {
                    var resultDialog =
                        MessageBox.Show(
                            "Es wurde keine Wiedergabeliste(n) geladen. Es wird versucht diese aus dem Parameter der Config zu laden.",
                            "Keine Wiedergabeliste geladen.", MessageBoxButton.OKCancel,
                            MessageBoxImage.Information);
                    if (resultDialog == MessageBoxResult.OK)
                    {
                        Playlists.Clear();
                        var res = Playlists.Load(Functions.PlaylistXML);
                        if (res == true)
                        {
                            initial.ReportProgress(0, "Wiedergaben XML wurde geladen.");
                            readedplaylists = true;
                        }
                        else
                        {
                            cancelerror = true;
                            return;
                        }
                    }
                    if (resultDialog == MessageBoxResult.Cancel)
                    {
                        //Abbruch
                        return;
                    }
                }

                initial.ReportProgress(10, Functions.StartDir + " wird nach Musik durchsucht");
                if (Functions.ReadFiles())
                {
                    initial.ReportProgress(20, "Es wurden " + Functions.AllSongs.Count + " Lieder gefunden.");
                   new Playlistwriter_WPF(Functions.AllSongs, Functions.PlaylistSavePath, initial, 20,Functions.ChangeMusicPath,Functions.PlaylistClearFolder, Functions.PictureExtract, Functions.PictureExtractPath);
                }
                else
                {
                    var resultDialog =
                        MessageBox.Show(
                            "Beim auslesen Musikdateien ist ein Fehler aufgetreten. Klicken Sie Ok um das Settingsmenü aufzurufen oder Cancel um die Verarbeitung abzubrechen.",
                            "Fehler beim Initialisieren", MessageBoxButton.OKCancel,
                            MessageBoxImage.Information);
                    if (resultDialog == MessageBoxResult.OK)
                    {
                        Settings se = new()
                        {
                            Owner = Owner
                        };
                        se.Show();
                        //Hide();
                        return;
                    }
                    if (resultDialog == MessageBoxResult.Cancel)
                    {
                        cancelerror = true;
                    }
                }

            }
            else
            {
                //Autorun durchführen.
                if (Functions.Autorun)
                {
                    Playlists.Clear();
                    var res = Playlists.Load(Functions.PlaylistXML);
                    if (res == true)
                    {
                        initial.ReportProgress(0, "Wiedergaben XML wurde geladen.");
                        readedplaylists = true;
                    }
                    else
                    {
                        cancelerror = true;
                        return;
                    }
                    initial.ReportProgress(10, Functions.StartDir + " wird nach Musik durchsucht");
                    if (Functions.ReadFiles())
                    {
                        initial.ReportProgress(20, "Es wurden " + Functions.AllSongs.Count + " Lieder gefunden.");
                        new Playlistwriter_WPF(Functions.AllSongs, Functions.PlaylistSavePath, initial, 20,
                            Functions.ChangeMusicPath, Functions.PlaylistClearFolder, Functions.PictureExtract,Functions.PictureExtractPath);
                    }
                    else
                    {
                        Close();
                        Owner.Close();
                    }
                }
                else
                {
                    var resultDialog =
                        MessageBox.Show(
                            "Beim auslesen der Config ist ein Fehler aufgetreten. Bitte öffnen Sie das Settings Menü um die notwendigen Daten zu setzen..",
                            "Fehler beim Initialisieren", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    if (resultDialog == MessageBoxResult.OK)
                    {
                        cancelerror = true;
                    }
                }
            }
        }

        private void Initial_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prb1.Value = e.ProgressPercentage;
            lbprb.Content = e.UserState;
        }

        private static readonly HttpClient _httpClient = new();
        /// <summary>
        /// Verbindet sich mit den anderen Webs um befehle abzugeben.
        /// </summary>
        /// <param name="nr"></param>
        /// <param name="call"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<String> ConnectToWeb(string call)
        {
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            Uri urlstate = new(call);
            HttpResponseMessage result;
            string returnValue;
            try
            {
                    result = await _httpClient.GetAsync(urlstate);
                    returnValue = await result.Content.ReadAsStringAsync();
                return returnValue;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async void Initial_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
           // MessageBox.Show("done");
            //lbprb.Content = "Fertig";
            prb1.Value = 100;
            if (!string.IsNullOrEmpty(Functions.MusicIndexPath))
            {
                //Sonos UpdateService aufrufen.
                lbprb.Content = "Aufruf Sonos MusikIndexUpdate";
                await ConnectToWeb(Functions.MusicIndexPath);
            }
            if (Functions.Autorun)
            {
                Close();
                Owner.Close();
            }
           
            if (readedplaylists)
            {
                ((MainWindow) Owner).ResetDatabinds();
            }
            if (cancelerror)
            {
                lbprb.Content = "Abbruch wegen Fehler";
            }
            else
            {
                Hide();
            }
        }

    }
}
