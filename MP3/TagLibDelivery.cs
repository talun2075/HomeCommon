using MP3.DTO;

namespace MP3
{
    /// <summary>
    /// Liefert informationen direkt aus der TagLib ohne zum mp3 zu werden. 
    /// </summary>
    public static class TagLibDelivery
    {
        private static TagLib.File CreateFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
               return TagLib.File.Create(path);
            }
            return null;
        }
        /// <summary>
        /// Liefert den Haswert für das Bild vom Pfad
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPictureHash(string path)
        {
            var h = CreateFile(path);
            if (h.Tag.Pictures.Length == 0) return "no-cover";
            return h.Tag.Pictures[0].Data.Checksum.ToString();
        }
        /// <summary>
        /// Liefert den Haswert für das Bild vom Pfad
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static MP3ImageData GetPictureHashAndType(string path)
        {
            var retval = new MP3ImageData();
            var h = CreateFile(path);

            if (h.Tag.Pictures.Length == 0) return retval;
            retval.Hash = h.Tag.Pictures[0].Data.Checksum.ToString();
            if (h.Tag.Pictures[0].MimeType.Contains("jpeg") || h.Tag.Pictures[0].MimeType.Contains("jpg"))
            {
              retval.Extension = ".jpg";
            }
            return retval;
        }
    }
}
