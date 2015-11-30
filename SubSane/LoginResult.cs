namespace SubSane
{
    public class LoginResult
    {
        public bool Success { get; private set; }

        //todo : ugly ugly ugly.
        private string expectedResult = @"subsonic-response xmlns=""http://subsonic.org/restapi"" status=""ok""";

        public LoginResult (string subsonicReply)
        {
            if (subsonicReply.Contains(expectedResult))
            {
                Success = true;
            }
            else
            {
                Success = false;
            }
        }
    }
}