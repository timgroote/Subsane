using System;
using System.Xml;

namespace SubsonicAPI
{
    public class LoginResult
    {
        public LoginResult(bool success, string mesg)
        {
            Success = success;
            Message = mesg;
        }

        public string Message { get; private set; }

        public Boolean Success { get; private set; }

        public static LoginResult FromXml(XmlNode node)
        {
            bool success;
            string mesg;

            if (node.Attributes["status"].Value == "ok")
            {
                success = true;
                mesg = "ok";
            }
            else
            {
                success = false;
                mesg = String.Format("{0} - {1} ({2}) {3}", node.Attributes["status"].Value,
                    node.ChildNodes[0].Name,
                    node.ChildNodes[0].Attributes["code"],
                    node.ChildNodes[0].Attributes["message"]);
            }
            

            return new LoginResult(success, mesg);
        }

    }
}