using MP3File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistGenerator
{
    /// <summary>
    /// Diese Klasse soll die Playlisten definitionen auslesen und schreiben.
    /// </summary>
    public class Playlistwriter
    {
        private static string startDir = String.Empty;
        private static string savepath = String.Empty;
        private static readonly List<String> allSongs = new();
        private static string changeMusicPath = String.Empty;
        private static Boolean clearPlaylistPath = false;
        /// <summary>
        /// Ermittelt für die Playlist die entsprechenden Songs und schreibt die M3U
        /// </summary>
        /// <param name="SavepathPlaylists">Speicherpfad für die M3U</param>
        /// <param name="Startpath">Lesepfad für die Musik</param>
        /// <param name="LoadPathPlaylistDefinition">Lesepfad für die Playlistdefinition (XML)</param>
        /// <param name="ChangeMusicPath">Wenn ein anderer Pfad für den Song in einer Playlist genommen werden soll 
        /// Folgender Aufbau: OldValue|NewValue </param>
        /// <param name="clearPlaylistPath">Soll der Playlistordner gelerrt werden?</param>
        public Playlistwriter(string SavepathPlaylists,string Startpath,string LoadPathPlaylistDefinition, String ChangeMusicPath = null, Boolean ClearPlaylistPath = false)
        {
            savepath = SavepathPlaylists;
            startDir = Startpath;
            changeMusicPath = ChangeMusicPath;
            clearPlaylistPath = ClearPlaylistPath;
            Playlists.Load(LoadPathPlaylistDefinition);
        }
        public static async Task<Boolean> StartCreation()
        {
            var retvalcpl = false;
            var retval = await ReadFiles();
            if (retval)
                retvalcpl = await CreatePlaylist();
            return retvalcpl;
        }
        internal static async Task<Boolean> CreatePlaylist()
        {
            return await Task.Run(() =>
            {
                if (!allSongs.Any()) return false;
                //Liste für die MP3Files pro liste vorbereiten
                List<List<MP3File.MP3File>> plnames = new();
                for (int i = 0; i < Playlists.GetPlaylistNames.Count; i++)
                {
                    plnames.Add(new List<MP3File.MP3File>());
                }
                //Wenn der Song existiert wird er ausgelesen und geprüft
                foreach (string path in allSongs)
                {
                    if (File.Exists(path))
                    {
                        var file = MP3ReadWrite.ReadMetaData(path);
                        if (file.VerarbeitungsFehler)
                        {
                            continue;
                        }
                        int playlistcounter = 0;
                        foreach (PlaylistClass playlist in Playlists.GetPlaylists)
                        {
                            bool resval = true;
                            bool[] groupconditioner = new bool[playlist.PlaylistConditionGroups.Count];
                            int gcc = 0;
                            foreach (ConditionObjectGroup playlistConditionGroup in playlist.PlaylistConditionGroups)
                            {
                                //Wenn resval einmal auf false ist, dann wurde bei einer Gruppe False gesetzt und dann wird weiter gemacht
                                //Gruppen sind mit AND verbunden.
                                if (resval == false)
                                {
                                    continue;
                                }
                                resval = ConditionCecker.Ceck(playlistConditionGroup, file);
                                groupconditioner[gcc] = resval;
                                gcc++;
                            }
                            bool resvalgc = true;
                            foreach (var b in groupconditioner)
                            {
                                if (b == false)
                                {
                                    resvalgc = false;
                                    break;
                                }
                            }
                            //song der Liste zufügen, weil er passt.
                            if (resvalgc)
                            {
                                plnames[playlistcounter].Add(file);
                            }
                            playlistcounter++;
                        }//Ende der GetPlaylists schleife
                    }
                }
                if (clearPlaylistPath)
                {
                    var files = Directory.GetFiles(savepath);
                    foreach (var pltoKill in files)
                    {
                        try
                        {
                            if (File.Exists(pltoKill))
                            {
                                File.Delete(pltoKill);
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                //hier die Playlist schreiben lassen.
                if (plnames.Count > 0)
                {
                    for (int i = 0; i < plnames.Count; i++)
                    {
                        if (plnames[i].Count > 0)
                        {
                            PlaylistSortOrder sortorder = Playlists.GetPlaylists[i].Sort;
                            if (sortorder != PlaylistSortOrder.NotSet)
                            {
                                if (sortorder == PlaylistSortOrder.Title)
                                {
                                    plnames[i] = plnames[i].OrderBy(x => x.Titel).ToList();
                                }
                                if (sortorder == PlaylistSortOrder.Artist)
                                {
                                    plnames[i] = plnames[i].OrderBy(x => x.Artist).ToList();
                                }
                                if (sortorder == PlaylistSortOrder.Random)
                                {
                                    Random rng = new();
                                    List<MP3File.MP3File> list = plnames[i];
                                    int n = list.Count;
                                    while (n > 1)
                                    {
                                        n--;
                                        int k = rng.Next(n + 1);
                                        MP3File.MP3File value = list[k];
                                        list[k] = list[n];
                                        list[n] = value;
                                    }

                                }
                                if (sortorder == PlaylistSortOrder.Rating)
                                {
                                    plnames[i] = plnames[i].OrderBy(x => x.Bewertung).ToList();
                                }
                                if (sortorder == PlaylistSortOrder.RatingMine)
                                {
                                    plnames[i] = plnames[i].OrderBy(x => x.BewertungMine).ToList();
                                }
                            }
                            if (!MP3ReadWrite.WritePlaylist(plnames[i], Playlists.GetPlaylistNames[i], savepath, changeMusicPath))
                            {
                                //Fehler //todo: logging
                            }
                        }
                        else
                        {
                            //Prüfen, ob Datei schon vorhanden obwohl null, dann löschen
                            var dir = Directory.CreateDirectory(savepath);
                            string m3uname = Playlists.GetPlaylistNames[i] + ".m3u";
                            string file = dir.FullName + "\\" + m3uname;
                            if (File.Exists(file))
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }
                return true;
            });
        }
        internal static async Task<Boolean> ReadFiles()
        {
            await Task.Run(() =>
            {
                
            });

            try
            {
                //Startverzeichnis durchsuchen nach MP3
                var tempallesongs = Directory.GetFiles(startDir, "*.*", SearchOption.AllDirectories).ToList();
                allSongs.Clear();
                for (int i = 0; i < tempallesongs.Count; i++)
                {
                    if (tempallesongs[i].ToLower().EndsWith(".flac") || tempallesongs[i].ToLower().EndsWith(".mp3") || tempallesongs[i].ToLower().EndsWith(".m4a"))
                    {
                        allSongs.Add(tempallesongs[i]);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                var k = ex.Message;
                return false;
            }
        }
    }
}
