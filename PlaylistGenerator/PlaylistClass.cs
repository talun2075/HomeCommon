using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;


namespace PlaylistGenerator
{
    /// <summary>
    /// Enthält eine Playlist
    /// </summary>
    public class PlaylistClass
    {
        public List<String> conditionGroupsNames = new();
        /// <summary>
        /// Erzeugt eine Playlist
        /// </summary>
        public PlaylistClass()
        {
            PlaylistConditionGroups = new List<ConditionObjectGroup>();
        }
        /// <summary>
        /// Erzeugt eine Playlist
        /// </summary>
        /// <param name="_name">Name der Playlist</param>
        public PlaylistClass(String _name)
        {
            Playlist = _name;
            PlaylistConditionGroups = new List<ConditionObjectGroup>();
        }
        /// <summary>
        /// Name der Playlist
        /// </summary>
        public String Playlist { get; set; }
        /// <summary>
        /// Sortierreihenfolge der Playlist, wenn sortiert werden soll.
        /// Erlaubt: Titel, Artist, Random
        /// </summary>
        public PlaylistSortOrder Sort { get; set; }

        /// <summary>
        /// Alle Bedingungsgruppen
        /// </summary>
        public List<ConditionObjectGroup> PlaylistConditionGroups { get; set; }
        /// <summary>
        /// Alle Namen der Bedingungsgruppen
        /// </summary>
        public List<String> GetConditionGroupNames
        {
            get
            {
                conditionGroupsNames.Clear();
                if (PlaylistConditionGroups != null && PlaylistConditionGroups.Count > 0)
                {

                    foreach (ConditionObjectGroup cop in PlaylistConditionGroups)
                    {
                        conditionGroupsNames.Add(cop.Name);
                    }
                }
                return conditionGroupsNames;

            }
        }
    }
    /// <summary>
    /// Playlist Sort Order Enums
    /// </summary>
    public enum PlaylistSortOrder
    {
        Title,
        Artist,
        Random,
        Rating,
        RatingMine,
        NotSet
    }

    /// <summary>
    /// enthält alle Playlisten
    /// </summary>
    static public class Playlists
    {
        static private List<PlaylistClass> pll = new();
        static private readonly List<String> playlistNames = new();
        private static readonly XmlSerializer xmls = new(typeof(List<PlaylistClass>));
        /// <summary>
        /// Gibt eine Liste mit allen Playlists
        /// </summary>
        static public List<PlaylistClass> GetPlaylists => pll;

        /// <summary>
        /// Liefert eine Liste mit allen Playlistnamen
        /// </summary>
        static public List<String> GetPlaylistNames
        {
            get
            {
                playlistNames.Clear();
                foreach (PlaylistClass pc in pll)
                {
                    playlistNames.Add(pc.Playlist);
                }

                return playlistNames;
            }
        }
        /// <summary>
        /// Hinzufügen einer Playlist
        /// </summary>
        /// <param name="p"></param>
        static public void Add(PlaylistClass p)
        {
            pll.Add(p);
        }
        /// <summary>
        /// Entfernen einer Playlist
        /// </summary>
        /// <param name="p"></param>
        static public void Remove(PlaylistClass p)
        {
            pll.Remove(p);
        }
        /// <summary>
        /// Alles löschen
        /// </summary>
        static public void Clear()
        {
            pll.Clear();
        }
        /// <summary>
        /// Speichern in den Pfad (inkl. Datei)
        /// </summary>
        /// <param name="path">Pfad wird ohne überprüfung versucht zu speichern.</param>
        static public void Save(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return;
            }
            StreamWriter textWriter = new(path);
            xmls.Serialize(textWriter, GetPlaylists);
            textWriter.Close();
        }
        static public bool? Load(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return false;
                }
                StreamReader reader = new(path);
                List<PlaylistClass> playlistlist = (List<PlaylistClass>)xmls.Deserialize(reader);
                Clear();
                pll = playlistlist.OrderBy(x => x.Playlist).ToList();
                foreach (PlaylistClass pl in pll)
                {
                    playlistNames.Add(pl.Playlist);
                }
            }
            catch { return false; }
            return true;
        }
    }

    
}


