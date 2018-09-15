using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALMRestWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            ALMRestApi api = new ALMRestApi();
            string authcookie = api.Login("https://<hostname>/qcbin", "qa", "qa");
            bool isAutheticated = api.IsAuthenticated("https://<hostname>/qcbin", authcookie);
            string domainlist = api.GetDomains("https://<hostname>/qcbin", authcookie);
            string projectlist = api.GetProjects("https://<hostname>/qcbin", authcookie, "D1");
            string defect = api.GetEntity("https://<hostname>/qcbin", authcookie, "D1", "P1", "defects", "1");
            api.Logout("https://<hostname>/qcbin", authcookie);
            Console.ReadLine();
        }
    }
}
