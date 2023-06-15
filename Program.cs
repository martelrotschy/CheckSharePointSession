using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckSharePointSession
{

    class Program
    {
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetCookieEx(string url, string cookieName, IntPtr cookieData, ref int dataSize, int flags, IntPtr reserved);

        static void Main()
        {
            string siteUrl = "https://subdomain-my.sharepoint.com";
            string cookieName = "FedAuth";
            Console.WriteLine("Enter your subdomain (https://[subdomain].sharepoint.com):");
            string subdomain = Console.ReadLine();
            siteUrl = "https://" + subdomain + ".sharepoint.com";
            Console.WriteLine("Checking " + siteUrl);

            bool isSessionActive = CheckAuthenticationSession(siteUrl, cookieName);

            if (isSessionActive)
            {
                Console.WriteLine("Authentication session is active");
            }
            else
            {
                Console.WriteLine("Authentication session is not active");
            }
            Console.ReadLine();
        }

        static bool CheckAuthenticationSession(string siteUrl, string cookieName)
        {
            const int INTERNET_COOKIE_HTTPONLY = 0x00002000;
            const int INTERNET_COOKIE_THIRD_PARTY = 0x00001000;
            int dataSize = 0;

            // Call InternetGetCookieEx with a null pointer to get the required dataSize
            InternetGetCookieEx(siteUrl, cookieName, IntPtr.Zero, ref dataSize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero);

            if (dataSize <= 0)
                return false;

            // Allocate memory for the cookie data
            IntPtr cookieData = Marshal.AllocHGlobal(dataSize);
            try
            {
                // Call InternetGetCookieEx again with the allocated memory to retrieve the cookie data
                if (InternetGetCookieEx(siteUrl, cookieName, cookieData, ref dataSize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
                {
                    string cookieValue = Marshal.PtrToStringAnsi(cookieData);
                    // Check if the cookie value is not empty
                    return !string.IsNullOrEmpty(cookieValue);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(cookieData);
            }

            return false;
        }
    }
}
