using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using MP3File;
using System.IO;

namespace PlaylistGenerator
{
    public class Playlistwriter_WPF
    {
        /// <summary>
        /// Ermittelt für die Playlist die entsprechenden Songs und schreibt die M3U
        /// </summary>
        /// <param name="music">Alle Songs</param>
        /// <param name="savepath">Speicherpfad für die M3U</param>
        /// <param name="bw">Wenn mit einem Backourndworker gearbeitet werden soll, dann dieser</param>
        /// <param name="BWStartvalue">Start Wert des Backgroundworkers</param>
        /// <param name="ChangeMusicPath">Wenn ein anderer Pfad für den Song in einer Playlist genommen werden soll 
        /// Folgender Aufbau: OldValue|NewValue </param>
        /// <param name="clearPlaylistPath">Soll der Playlistordner gelerrt werden?</param>
        public Playlistwriter_WPF(List<String> music, string savepath, BackgroundWorker bw = null, int BWStartvalue = 0, String ChangeMusicPath = null, Boolean clearPlaylistPath = false, Boolean extractPictures= false, string picPath = "")
        {
            if (BWStartvalue > 80)
            {
                return;
            }

            int reportvalue = 0;
            if (bw != null)
            {
                bw.ReportProgress(BWStartvalue, "Wiedergabelisten werden in den Speicher geladen");
            }

            //Liste für die MP3Files pro liste vorbereiten
            List<List<MP3File.MP3File>> plnames = new();
            plnames.Clear();
            for (int i = 0; i < Playlists.GetPlaylistNames.Count; i++)
            {
                plnames.Add(new List<MP3File.MP3File>());
            }
            int counter = 0;
            //Wenn der Song existiert wird er ausgelesen und geprüft
            foreach (string path in music)
            {
                if (bw != null)
                {
                    reportvalue = BWStartvalue + ((counter * (100 - BWStartvalue - 10)) / music.Count);
                    bw.ReportProgress(reportvalue, path + " wird geprüft");
                    counter++;
                }
                if (File.Exists(path))
                {
                    var file = MP3ReadWrite.ReadMetaData(path);
                    if (file.VerarbeitungsFehler && bw != null)
                    {
                        bw.ReportProgress(reportvalue, "Fehler beim Song:" + path);
                        continue;
                    }
                    //Bilder extraieren
                    try
                    {
                        if (extractPictures)
                        {
                            Directory.CreateDirectory(picPath);
                            var tempfile = TagLib.File.Create(path);
                            var img = tempfile.Tag.Pictures[0].Data.Data;
                            var imgname = tempfile.Tag.Pictures[0].Data.Checksum;
                            File.WriteAllBytes(picPath + imgname + ".png", img);
                        }
                    }
                    catch(Exception ex)
                    {
                        bw.ReportProgress(reportvalue, path + " Exception:"+ex.Message);
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
                if (bw != null)
                {
                    reportvalue = 90;
                    bw.ReportProgress(reportvalue, "Wiedergabelisten fertig ermittelt und werden nun geschrieben.");
                }
                for (int i = 0; i < plnames.Count; i++)
                {
                    if (bw != null)
                    {
                        if (plnames.Count > 1)
                        {
                            reportvalue += ((i * (10)) / plnames.Count);
                        }
                        else
                        {
                            reportvalue = 100;
                        }
                        bw.ReportProgress(reportvalue, "Wiedergabeliste " + Playlists.GetPlaylistNames[i] + " wird geschrieben.");
                    }

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

                        if (!MP3ReadWrite.WritePlaylist(plnames[i], Playlists.GetPlaylistNames[i], savepath, ChangeMusicPath))
                        {
                            //Fehler
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
        }

    }
}
