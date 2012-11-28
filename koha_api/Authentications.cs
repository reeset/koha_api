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

namespace koha_api
{
    internal class Authentications
    {

        internal bool Authorize(string host, string username, string password, out string return_status, out CookieContainer theJar)
        {
            return_status = "";
            theJar = null;
            CookieContainer cookieJar = new CookieContainer();
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
