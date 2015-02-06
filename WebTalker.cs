using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace BenebotV3
{
    class WebTalker
    {
        public static string HttpGet(string uri)
        {
            try
            {
                // Create a request for the URL.
                WebRequest request = WebRequest.Create(uri);
                // If required by the server, set the credentials.
                request.Credentials = CredentialCache.DefaultCredentials;
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                //Console.WriteLine(((HttpWebResponse) response).StatusDescription);
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                Debug.Assert(dataStream != null, "dataStream != null");
                var reader = new StreamReader(dataStream);
                // Read the content.
                var responseFromServer = reader.ReadToEnd();
                // Display the content.
                // Clean up the streams and the response.
                reader.Close();
                response.Close();
                return responseFromServer;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
