using System;
using System.Collections.Generic;
using System.Xml;

namespace SubsonicAPI
{
    public class MusicFolder : SubsonicItem
    {
        #region private vars

        private List<MusicFolder> _Folders;
        private List<Song> _Songs;

        #endregion private vars

        #region properties

        public List<MusicFolder> Folders
        {
            get { return _Folders; }
            set { _Folders = value; }
        }

        public List<Song> Songs
        {
            get { return _Songs; }
            set { _Songs = value; }
        }

        #endregion properties

        public MusicFolder()
        {
            _Folders = new List<MusicFolder>();
            _Songs = new List<Song>();              
        }

        public MusicFolder(string theName, string theId)
        {
            _Folders = new List<MusicFolder>();
            _Songs = new List<Song>();

            base.Name = theName;
            base.Id = theId;
        }

        public void AddSong(Song sg)
        {
            if (sg == null) //todo log
            {
                Console.Out.WriteLine("!!!WARN!!!! Unable to load song. not adding to playlist");
                return;
            }
                

            _Songs.Add(sg);
        }

        public void AddFolder(string name, string id)
        {
            MusicFolder newFolder = new MusicFolder(name, id);
            _Folders.Add(newFolder);
        }
        public void AddFolder(MusicFolder f)
        {
            _Folders.Add(f);
        }

        public Song FindSong(string theTitle)
        {
            Song theSong = _Songs.Find(sng => sng.Name == theTitle);

            return theSong;
        }

        public MusicFolder FindFolder(string theFolderName)
        {
            MusicFolder theFolder = _Folders.Find( fldr => fldr.Name == theFolderName );

            return theFolder;
        }

        public override SubsonicItemType ItemType
        {
            get
            {
                return SubsonicItemType.Folder;
            }
        }

        public static MusicFolder FromXml(XmlNode childNode)
        {                                                                                      
            string title = childNode.Attributes["title"].Value;
            string theId = childNode.Attributes["id"].Value;

            return new MusicFolder(title, theId);
        }
    }
}