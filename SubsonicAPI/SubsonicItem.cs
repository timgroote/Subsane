using System;

namespace SubsonicAPI
{
    public enum SubsonicItemType
    {
        Folder, Song
    }

    public abstract class SubsonicItem
    {
        public string Name;
        public string Id;
        
        public abstract SubsonicItemType ItemType { get; }
                                                       
        public override string ToString()
        {
            return String.Format ("{0}-{1}",Id, Name);
        }
    }
}