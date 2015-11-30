using System;
using System.Xml;

namespace SubsonicAPI
{
    public class Song : SubsonicItem
    {
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }

        public Song()
        {   
        }

        public Song(string theTitle, string theId, string artistName, string albumName)
        {
            ArtistName = artistName;
            AlbumName = albumName;
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
            return String.Format("#{0} : {1} - ({2}) - {3}", Id, ArtistName, AlbumName, Name);
        }

        public static Song FromXml(XmlNode xmlNode)
        {
            if (xmlNode == null) return null;
            if (xmlNode.Attributes == null) return null;
            string title = xmlNode.Attributes["title"].Value;
            string id = xmlNode.Attributes["id"].Value;
            string artist = xmlNode.Attributes["artist"].Value;
            string album = xmlNode.Attributes["album"].Value;
            

            return new Song(title, id, artist, album);
        }
    }
}