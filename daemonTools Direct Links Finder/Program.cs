using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace daemonTools_Direct_Links_Finder
{
    public class Program
    {
        public static string getParams(int idProduct)
        {
            switch (idProduct)
            {
                case 1:
                    return "dtLiteTrial";
                case 2:
                    return "dtLitePaid";
                case 3:
                    return "dtProPaid";
                case 4:
                    return "dtUltraPaid";
                default:
                    return new Random().Next(1, 3)==1 ? "dtProTrial" : "dtUltraTrial";
            }
        }

        static void Main(string[] args)
        {
            string menuProducts = '\n' +
                "+-----+--------------------------------------------+\n" +
                "| ID  |  DaemonTools Products Direct Links Finder  |\n" +
                "+-----+--------------------------------------------+\n" +
                "|  1  | DaemonTools Lite Free                      |\n" +
                "|  2  | DaemonTools Lite Personal                  |\n" +
                "|  3  | DaemonTools Pro                            |\n" +
                "|  4  | DaemonTools Ultra                          |\n" +
                "+-----+--------------------------------------------+\n\n" +
                "Enter the ID of the product to get its direct link: ";
            Console.Write(menuProducts);
            if (!int.TryParse(Console.ReadLine(), out int idProduct) || idProduct <= 0 || idProduct > 4)
            {
                Console.WriteLine("Bad boy, enter a valid ID number from the list! :(");
            }
            else
            {
                string text = getParams(idProduct);
                string[] arrServers = new string[]{
                    "http://downloader1.disk-tools.com/downloader",
                    "http://downloader2.disk-tools.com/downloader"};
                foreach (string str in arrServers)
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(str + "?p=" + Uri.EscapeDataString(AESCrypter.EncryptString(text)));
                    httpWebRequest.Proxy = new WebProxy();
                    httpWebRequest.UserAgent = "DtOfflineInstaller 1.4.24";
                    using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                    {
                        using (Stream responseStream = httpWebResponse.GetResponseStream())
                        {
                            using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                            {
                                Console.WriteLine(AESCrypter.DecryptString(streamReader.ReadToEnd()));
                            }
                        }
                    }

                }
            }
            Console.ReadKey();
        }
    }
}
