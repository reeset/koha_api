/*******************************************************
 *  Author: Terry Reese
 *  Project: koha_api
 *  License: http://creativecommons.org/publicdomain/zero/1.0
 *  To the extent possible under law, Terry Reese has waived 
 *  all copyright and related or neighboring rights to koha_api. 
 *  This work is published from:  United States. 
 ********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace koha_api
{
    internal class Authentications
    {

        private bool pssl = false;

        internal bool Ignore_SSL_Certificates
        {
            set { pssl = value; }
            get { return pssl; }
        }
        /// <summary>
        /// Together with the AcceptAllCertifications method right
        /// below this causes to bypass errors caused by SLL-Errors.
        /// </summary>
        public static void IgnoreBadCertificates()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

        /// <summary>
        /// In Short: the Method solves the Problem of broken Certificates.
        /// Sometime when requesting Data and the sending Webserverconnection
        /// is based on a SSL Connection, an Error is caused by Servers whoes
        /// Certificate(s) have Errors. Like when the Cert is out of date
        /// and much more... So at this point when calling the method,
        /// this behaviour is prevented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certification"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns>true</returns>
        private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        } 

        internal bool Authorize(string host, string username, string password, out string return_status, out CookieContainer theJar)
        {
            return_status = "";
            theJar = null;
            CookieContainer cookieJar = new CookieContainer();
            if (Ignore_SSL_Certificates == true)
            {
                IgnoreBadCertificates();
            }
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(host + "/cgi-bin/koha/svc/authentication?userid=" + username + "&password=" + password);
            

            request.CookieContainer = cookieJar;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252));
            string tmp = reader.ReadToEnd();

            //Add cookies to CookieJar (Cookie Container)
            foreach (Cookie cookie in response.Cookies)
            {
                cookieJar.Add(new Cookie(cookie.Name.Trim(), cookie.Value.Trim(), cookie.Path, cookie.Domain));
            }

            reader.Close();
            response.Close();

            if (tmp.IndexOf("<status>ok</status>") > -1)
            {
                return_status = "ok";
                theJar = cookieJar;
                return true;
            }
            else
            {
                return_status = tmp;
                theJar = cookieJar;
                return false;
            }

        }
    }
}
