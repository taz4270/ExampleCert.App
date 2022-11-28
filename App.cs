using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExampleCert
{
    class App : Application
    {
        public App()
        {
            MainPage = new ContentPage
            {
                Content = new Button
                {
                    Text = "Request",
                    TextTransform = TextTransform.Uppercase,
                    HeightRequest = 60,
                    WidthRequest = 120,
                    Command = new Command(OnClick)
                }
            };
        }

        const string Url = "URL";

        const string Password = "PASSWORD";

        async void OnClick()
        {
            try
            {
                Stream stream = await FileSystem.OpenAppPackageFileAsync("certificate.cer");

                using MemoryStream s = new();
                stream.CopyTo(s);
                stream.Dispose();

                X509Certificate2 cert = new(s.ToArray(), Password);

                SocketsHttpHandler shh = new()
                {
                    UseCookies = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    SslOptions =
                {
                    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                    LocalCertificateSelectionCallback = (a,b,c,d,e) => c[0],
                    RemoteCertificateValidationCallback = (a,b,c,d) => true,

                }
                };

                shh.SslOptions.ClientCertificates.Add(cert);

                HttpClient c = new(shh);

                var x = await c.GetAsync(new Uri(Url));
                _ = 1;
            }
            catch (Exception e)
            {
                _ = e;
            }
        }
    }
}
