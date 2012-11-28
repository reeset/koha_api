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
    public class Bib_Actions
    {
        private CookieContainer cookieJar = null;
        private string perror_message = "";
        private string pusername = "";
        private string ppassword = "";
        private string p_host = "";
        
        public string Error_Message
        {
            set { perror_message = value; }
            get { return perror_message; }
        }

        public string Debug_Info {
            get
            {
                if (cookieJar == null)
                {
                    return "Authorized: No" + "\n" +
                           "Last Error: " + perror_message;
                }
                else
                {
                    string tmp = "Authorized: Yes" + "\n";
                    foreach (System.Net.Cookie c in cookieJar.GetCookies(new Uri(Host)))
                    {
                        tmp += c.Name + ":" + c.Value + "\n";
                    }
                    return tmp;
                }
            }
        }

        public string Host
        {
            set { p_host = value; }
            get { return p_host; }
        }

        public bool Authorize(string username, string password)
        {
            Authentications objA = new Authentications();
            pusername = username;
            ppassword = password;
            bool is_authorized = false;

            try
            {
                is_authorized = objA.Authorize(Host, username, password, out perror_message, out cookieJar);
                
            }
            catch
            {
                Error_Message = "Undefined authorization error";
                return false;
            }

            return is_authorized;
        }
        //data is retrieved in MARCXML
        public string GetRecord(string id)
        {
            string uri = Host + "/cgi-bin/koha/svc/bib/" + id + "?userid=" + pusername + "&password=" + ppassword;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.CookieContainer = cookieJar;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string tmp = reader.ReadToEnd();

                reader.Close();
                response.Close();
                return tmp;
            }
            catch (WebException e) 
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Error_Message = e.Message;
                    return e.Message;
                }
                else
                {
                    Error_Message = e.Message;
                    return e.Message;
                }
            }
            
        }

        public bool CreateRecord(string rec)
        {
            return UpdateRecord(rec, "");
        }


        //Data must be passed in MARCXML
        public bool UpdateRecord(string rec, string id)
        {
            string uri = "";

            if (id == "")
            {
                //this is for new records
                uri = Host + "/cgi-bin/koha/svc/new_bib";
            }
            else
            {
                //this is for updated records
                uri = Host + "/cgi-bin/koha/svc/bib/" + id;
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.CookieContainer = cookieJar;
            request.Method = "POST";
            request.ContentType = @"text/xml";
            System.IO.StreamWriter writer = new System.IO.StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
            writer.Write(rec);
            writer.Flush();
            writer.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252));
                string tmp = reader.ReadToEnd();

                reader.Close();
                response.Close();

                return true;
            } catch (WebException e ){
                Error_Message = e.Message;
                return false;
            }
        }

        //pass in a marcxml record and return 
        //the record id if present
        public string GetRecordId(string xml, string field, string subfield)
        {

            string xpath = "";
            try
            {

                if (xml.IndexOf("marc:collection") > -1)
                {
                    xpath = "/marc:collection/marc:record/marc:datafield[@tag='" + field + "']";
                }
                else
                {
                    xpath = "/marc:record/marc:datafield[@tag='" + field + "']";
                }

                System.Xml.XmlDocument objXML = new System.Xml.XmlDocument();
                System.Xml.XmlNamespaceManager ns = new System.Xml.XmlNamespaceManager(objXML.NameTable);
                ns.AddNamespace("marc", "http://www.loc.gov/MARC21/slim");

                System.Xml.XmlNode objNode;
                System.Xml.XmlNode objSubfield;
                objXML.LoadXml(xml);
                objNode = objXML.SelectSingleNode(xpath, ns);
                objNode = objXML.SelectSingleNode(xpath, ns);
                objSubfield = objNode.SelectSingleNode("marc:subfield[@code='" + subfield + "']", ns);
                return objSubfield.InnerText;

            }
            catch (System.Exception xe)
            {
                perror_message += xe.ToString();
                return "";
            }

        }
    }
}
