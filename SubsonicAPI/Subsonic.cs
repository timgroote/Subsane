using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace SubsonicAPI
{
    #region Classes

    #endregion Classes

    /// <summary>
    /// C# Implementation of the Subsonic API
    /// http://www.subsonic.org/pages/api.jsp
    /// </summary>
    public static class Subsonic
    {
        // Should be set from application layer when the application is loaded
        public static string appName;

        // Version of the REST API implemented
        private static string apiVersion = "1.3.0";

        // Set with the login method
        static string server;
        static string authHeader;

        /// <summary>
        /// Takes parameters for server, username and password to generate an auth header
        /// and Pings the server
        /// </summary>
        /// <param name="theServer"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns>Resulting XML (Future boolean)</returns>
        public static string LogIn(string theServer, string user, string password)
        {
            string result = "Nothing Happened";

            server = theServer;
            authHeader = user + ":" + password;
            authHeader = Convert.ToBase64String(Encoding.Default.GetBytes(authHeader));

            Stream theStream = MakeGenericRequest("ping", null);

            StreamReader sr = new StreamReader(theStream);

            result = sr.ReadToEnd();

            /// TODO: Parse the result and determine if logged in or not

            return result;
        }

        /// <summary>
        /// Uses the Auth Header for logged in user to make an HTTP request to the server 
        /// with the given Subsonic API method and parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>Datastream of the server response</returns>
        public static Stream MakeGenericRequest(string method, Dictionary<string, string> parameters)
        {
            // Check to see if Logged In yet
            if (string.IsNullOrEmpty(authHeader))
            {
                // Throw a Not Logged In exception
                Exception e = new Exception("No Authorization header.  Must Log In first");
                return null;
            }
            else
            {
                if (!method.EndsWith(".view"))
                    method += ".view";

                string requestURL = BuildRequestURL(method, parameters);

                WebRequest theRequest = WebRequest.Create(requestURL);
                theRequest.Method = "GET";

                theRequest.Headers["Authorization"] = "Basic " + authHeader;

                WebResponse response = theRequest.GetResponse();

                Stream dataStream = response.GetResponseStream();

                return dataStream;
            }
        }

        /// <summary>
        /// Creates a URL for a request but does not make the actual request using set login credentials an dmethod and parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>Proper Subsonic API URL for a request</returns>
        public static string BuildRequestURL(string method, Dictionary<string, string> parameters)
        {
            string requestURL = "http://" + server + "/rest/" + method + "?v=" + apiVersion + "&c=" + appName;
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> parameter in parameters)
                {
                    requestURL += "&" + parameter.Key + "=" + parameter.Value;
                }
            }
            return requestURL;
        }

        public static Song GetSong(string id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("id", id);
            // Make the request
            Stream theStream = MakeGenericRequest("getSong", parameters);

            Song s = null;

            using (StreamReader sr = new StreamReader(theStream))
            {
                string result = sr.ReadToEnd();

                // Parse the resulting XML string into an XmlDocument
                XmlDocument myXML = new XmlDocument();
                myXML.LoadXml(result);
                if (myXML.ChildNodes[1].Name == "subsonic-response")
                {
                    if (myXML.ChildNodes[1].FirstChild.Name == "song")
                    {
                        s = new Song(
                            myXML.ChildNodes[1].FirstChild.Attributes["title"].Value,
                            myXML.ChildNodes[1].FirstChild.Attributes["id"].Value,
                            myXML.ChildNodes[1].FirstChild.Attributes["artist"].Value,
                            myXML.ChildNodes[1].FirstChild.Attributes["album"].Value
                            );
                    }
                }
            }

            return s;
        }

        /// <summary>
        /// Returns an indexed structure of all artists.
        /// </summary>
        /// <param name="musicFolderId">Required: No; If specified, only return artists in the music folder with the given ID.</param>
        /// <param name="ifModifiedSince">Required: No; If specified, only return a result if the artist collection has changed since the given time.</param>
        /// <returns>Dictionary, Key = Artist and Value = id</returns>
        public static Dictionary<string, string> GetIndexes(string musicFolderId = "", string ifModifiedSince = "")
        {
            // Load the parameters if provided
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(musicFolderId))
                parameters.Add("musicFolderId", musicFolderId);

            if (!string.IsNullOrEmpty(ifModifiedSince))
                parameters.Add("ifModifiedSince", ifModifiedSince);

            // Make the request
            Stream theStream = MakeGenericRequest("getIndexes", parameters);
            // Read the response as a string
            using (StreamReader sr = new StreamReader(theStream))
            {
                string result = sr.ReadToEnd();

                // Parse the resulting XML string into an XmlDocument
                XmlDocument myXML = new XmlDocument();
                myXML.LoadXml(result);

                // Parse the XML document into a Dictionary
                Dictionary<string, string> artists = new Dictionary<string, string>();
                if (myXML.ChildNodes[1].Name == "subsonic-response")
                {
                    if (myXML.ChildNodes[1].FirstChild.Name == "indexes")
                    {
                        int i = 0;
                        for (i = 0; i < myXML.ChildNodes[1].FirstChild.ChildNodes.Count; i++)
                        {
                            int j = 0;
                            for (j = 0; j < myXML.ChildNodes[1].FirstChild.ChildNodes[i].ChildNodes.Count; j++)
                            {
                                string artist = myXML.ChildNodes[1].FirstChild.ChildNodes[i].ChildNodes[j].Attributes["name"].Value;
                                string id = myXML.ChildNodes[1].FirstChild.ChildNodes[i].ChildNodes[j].Attributes["id"].Value;

                                artists.Add(artist, id);
                            }
                        }
                    }
                }
                return artists;
            }
        }

        /// <summary>
        /// Streams a given music file. (Renamed from request name "stream")
        /// </summary>
        /// <param name="id">Required: Yes; A string which uniquely identifies the file to stream. 
        /// Obtained by calls to getMusicDirectory.</param>
        /// <param name="maxBitRate">Required: No; If specified, the server will attempt to 
        /// limit the bitrate to this value, in kilobits per second. If set to zero, no limit 
        /// is imposed. Legal values are: 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256 and 320. </param>
        /// <returns></returns>
        public static Stream StreamSong(string id, int? maxBitRate = null)
        {
            // Reades the id of the song and sets it as a parameter
            Dictionary<string, string> theParameters = new Dictionary<string,string>();
            theParameters.Add("id", id);
            if (maxBitRate.HasValue)
                theParameters.Add("maxBitRate", maxBitRate.ToString());

            // Makes the request
            Stream theStream = MakeGenericRequest("stream", theParameters);

            return theStream;
        }


        /// <summary>
        /// Streams a given music file to a byte array, then returns that
        /// </summary>
        /// <param name="id">Required: Yes; A string which uniquely identifies the file to stream. 
        /// Obtained by calls to getMusicDirectory.</param>
        /// <param name="maxBitRate">Required: No; If specified, the server will attempt to 
        /// limit the bitrate to this value, in kilobits per second. If set to zero, no limit 
        /// is imposed. Legal values are: 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256 and 320. </param>
        /// <returns></returns>
        public static byte[] PreloadSong(string id, int? maxBitRate = null)
        {
            byte[] outBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Stream memStream = StreamSong(id, maxBitRate))
                {
                    CopyStream(memStream,ms);
                }
                outBytes = ms.ToArray();
            }           
            return outBytes;
        }

        //keep away from .net 's default way of handling this since we want to keep compatibility with mono!!!
        private static void CopyStream(Stream input, Stream output)
        {
            byte[] b = new byte[32768];
            int r;
            while ((r = input.Read(b, 0, b.Length)) > 0)
                output.Write(b, 0, r);
        }


        /// <summary>
        /// Returns a listing of all files in a music directory. Typically used to get list of albums for an artist, or list of songs for an album.
        /// </summary>
        /// <param name="id">A string which uniquely identifies the music folder. Obtained by calls to getIndexes or getMusicDirectory.</param>
        /// <returns>MusicFolder object containing info for the specified directory</returns>
        public static MusicFolder GetMusicDirectory(string id)
        {
            Dictionary<string, string> theParameters = new Dictionary<string, string>
            {
                {"id", id}
            };

            Stream theStream = MakeGenericRequest("getMusicDirectory", theParameters);

            StreamReader sr = new StreamReader(theStream);

            string result = sr.ReadToEnd();

            XmlDocument myXML = new XmlDocument();
            myXML.LoadXml(result);

            MusicFolder theFolder = new MusicFolder("ArtistFolder", id);

            if (myXML.ChildNodes[1].Name == "subsonic-response")
            {
                if (myXML.ChildNodes[1].FirstChild.Name == "directory")
                {
                    theFolder.Name = myXML.ChildNodes[1].FirstChild.Attributes["name"].Value;
                    theFolder.Id = myXML.ChildNodes[1].FirstChild.Attributes["id"].Value;

                    int i = 0;
                    for (i = 0; i < myXML.ChildNodes[1].FirstChild.ChildNodes.Count; i++)
                    {
                        bool isDir = bool.Parse(myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["isDir"].Value);
                        
                        if (isDir)
                            theFolder.AddFolder(MusicFolder.FromXml(myXML.ChildNodes[1].FirstChild.ChildNodes[i]));
                        else
                            theFolder.AddSong(Song.FromXml(myXML.ChildNodes[1].FirstChild.ChildNodes[1]));
                    }
                }
            }

            return theFolder;
        }
		
		/// <summary>
		/// Returns what is currently being played by all users. Takes no extra parameters. 
		/// </summary>
		public static Dictionary<String, Song> GetNowPlaying()
		{
            #region example data
            //EXAMPLE DATA
            //    <? xml version = "1.0" encoding = "UTF-8" ?>\n < subsonic - response xmlns = "http://subsonic.org/restapi" status = "ok" version = "1.12.0" >
            //   < nowPlaying >
            //   < entry id = "58160"
            //   parent = "57870"
            //   isDir = "false"
            //   title = "As Far As I Remember"
            //   album = "The Connection"
            //   artist = "Papa Roach"
            //   track = "13"
            //   year = "2012"
            //   genre = "Rock"
            //   coverArt = "57870"
            //   size = "8910048"
            //   contentType = "audio/mpeg" suffix = "mp3"
            //   duration = "222"
            //   bitRate = "320"
            //   path = "Papa Roach/Papa Roach - The Connection/13. As Far As I Remember.mp3"
            //   isVideo = "false"
            //   created = "2015-06-11T12:13:24.000Z"
            //   albumId = "4023"
            //   artistId = "622"
            //   type = "music"
            //   username = "dave"
            //   minutesAgo = "2"
            //   playerId = "7" />
            //   </ nowPlaying >
            //</ subsonic - response >\n"
            #endregion

            Dictionary <String, Song> nowPlaying = new Dictionary<string, Song>();
			
			Dictionary<string, string> theParameters = new Dictionary<string, string>();
			Stream theStream = MakeGenericRequest("getNowPlaying", theParameters);
			StreamReader sr = new StreamReader(theStream);
			string result = sr.ReadToEnd();
            XmlDocument nowPlayingXML = new XmlDocument();
            nowPlayingXML.LoadXml(result);
		    if (nowPlayingXML.ChildNodes[1].Name == "subsonic-response")
		    {
		        if (nowPlayingXML.ChildNodes[1].FirstChild.Name == "nowPlaying")
		        {   
		            foreach (XmlNode childnode in nowPlayingXML.ChildNodes[1].FirstChild)
		            {
		                string user = childnode.Attributes["username"].Value;

                        Song sg = new Song(
                            childnode.Attributes["artist"].Value, 
                            childnode.Attributes["album"].Value, 
                            childnode.Attributes["title"].Value,
                            childnode.Attributes["id"].Value
                        );
                        
		                nowPlaying.Add(user + childnode.Attributes["playerId"].Value, sg);
                    }
		        }
            }
		    return nowPlaying;
		}
    }

}