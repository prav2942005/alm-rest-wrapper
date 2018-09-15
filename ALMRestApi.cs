using System;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Xml;

namespace ALMRestWrapper
{
    public class ALMRestApi
    {
        const string ALM_IS_AUTHENTICATED = "{0}/rest/is-authenticated";
        const string ALM_AUTHENTICATE = "{0}/api/authentication/sign-in";
        const string ALM_SESSION = "/rest/site-session";
        const string ALM_ENTITY = "{0}/rest/domains/{1}/projects/{2}/{3}/{4}";
        const string ALM_LOGOUT = "{0}/authentication-point/logout";
        const string ALM_DOMAIN = "{0}/rest/domains";
        const string ALM_PROJECT = "{0}/rest/domains/{1}/projects";

        CookieContainer ccontainer = new CookieContainer();

        public string Login(string url, string username, string pwd)
        {
            string returnValue = "";
            using (HttpWebResponse response = PerformRequest(string.Format(ALM_AUTHENTICATE, url), "GET", username + ":" + pwd))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                    returnValue = response.Headers["set-cookie"].ToString();
            }
            return returnValue;
        }

        public bool IsAuthenticated(string url, string authcookie)
        {
            bool returnValue = false;
            using (HttpWebResponse response = PerformRequest(string.Format(ALM_IS_AUTHENTICATED, url), "GET", "", "", authcookie))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                    returnValue = true;
            }
            return returnValue;
        }

        public void Logout(string url, string authcookie)
        {
            using (HttpWebResponse response = PerformRequest(string.Format(ALM_LOGOUT, url), "GET", "", "", authcookie))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                { }
            }
        }


        public string GetDomains(string url, string authcookie)
        {
            string returnValue = "";

            using (HttpWebResponse response = PerformRequest(string.Format(ALM_DOMAIN, url), "GET", "", "", authcookie))
            {
                Stream streamresponse = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(streamresponse))
                {
                    string tempstream = sr.ReadToEnd();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(tempstream);
                    XmlNode xnode = doc.SelectSingleNode("//Domains");
                    returnValue = JsonConvert.SerializeXmlNode(xnode).Replace("@Name", "Name");
                    Console.WriteLine(returnValue);
                }
            }

            return returnValue;
        }


        public string GetProjects(string url, string authcookie, string domain)
        {
            string returnValue = "";

            using (HttpWebResponse response = PerformRequest(string.Format(ALM_PROJECT, url, domain), "GET", "", "", authcookie))
            {
                Stream streamresponse = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(streamresponse))
                {
                    string tempstream = sr.ReadToEnd();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(tempstream);
                    XmlNode xnode = doc.SelectSingleNode("//Projects");
                    returnValue = JsonConvert.SerializeXmlNode(xnode).Replace("@Name", "Name");
                    Console.WriteLine(returnValue);
                }
            }

            return returnValue;
        }


        public string GetEntity(string url, string authcookie, string domain, string project, string entitytype, string entityid)
        {
            string returnValue = "";

            using (HttpWebResponse response = PerformRequest(string.Format(ALM_ENTITY, url, domain, project, entitytype, entityid), "GET", "", "", authcookie))
            {
                Stream streamresponse = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(streamresponse))
                {
                    string tempstream = sr.ReadToEnd();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(tempstream);
                    XmlNode xnode = doc.SelectSingleNode("//Entity/Fields");
                    returnValue = JsonConvert.SerializeXmlNode(xnode).Replace("@Name", "Name");
                    Console.WriteLine(returnValue);
                }
            }

            return returnValue;
        }

        private HttpWebResponse PerformRequest(string requrl, string method, string loginvalues = "", string data = "", string authcookie = "", string reqtype = "application/xml")
        {
            HttpWebResponse res = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requrl);
            req.Method = method;
            req.ContentType = reqtype;
            req.CookieContainer = ccontainer;
            req.AllowAutoRedirect = true;
            if (authcookie.Length == 0)
                req.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(loginvalues));
            else
                req.Headers[HttpRequestHeader.Cookie] = authcookie;

            try
            {
                res = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException wex)
            {
                Console.WriteLine(((HttpWebResponse)wex.Response).StatusCode);
            }

            return res;
        }
    }
}
