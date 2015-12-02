using System;
using System.Collections;
using System.Xml;

namespace SubsonicAPI
{
    public class Song : SubsonicItem
    {
        private static string[] PlayableTypes = new[] {"mp3", "mp2", "mp1", "ogg", "wav", "aiff"};

        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string FileType { get; set; }

        public Boolean IsPlayable
        {
            get
            {
                return ((IList) PlayableTypes).Contains(FileType.ToLowerInvariant());
            }
        }

        public Song()
        {   
        }

        public Song(string theTitle, string theId, string artistName, string albumName, string fileType)
        {
            ArtistName = artistName;
            AlbumName = albumName;
            FileType = fileType;
            Name = theTitle;
            Id = theId;
        }

        public override SubsonicItemType ItemType
        {
            get
            {
                return SubsonicItemType.Song;
            }
        }

        public override string ToString()
        {
            if (!IsPlayable)
            {
                return String.Format("#{0} : {1} - ({2}) - {3} ({4}) not playable", Id, ArtistName, AlbumName, Name, FileType);
            }
            return String.Format("#{0} : {1} - ({2}) - {3} {4}", Id, ArtistName, AlbumName, Name, FileType);
        }

        public static Song FromXml(XmlNode xmlNode)
        {
            if (xmlNode == null) return null;
            if (xmlNode.Attributes == null) return null;
            
            string title = xmlNode.Attributes["title"].Value;
            string id = xmlNode.Attributes["id"].Value;
            string artist = xmlNode.Attributes["artist"].Value;
            string album = xmlNode.Attributes["album"].Value;
            string fileType = xmlNode.Attributes["suffix"].Value;
            

            return new Song(title, id, artist, album, fileType);
        }
    }
}