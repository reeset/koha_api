Implementation of the Koha API in C#.  

Authentication Example:

string username = "[your username]";
string password = "[your password]";
objb = new koha_api.Bib_Actions();
objb.Host = "[your host]"; //i.e.: http://www.kohacatalog.com

if (objb.Authorize(username, password) == true)
  {
     System.Windows.Forms.MessageBox.Show(objb.Debug_Info + "\n" +
                                          "Authorized");
  }
  else
  {
     System.Windows.Forms.MessageBox.Show(objb.Error_Message);
  }
}