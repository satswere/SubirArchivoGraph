using System.Globalization;

namespace SubirArchivoGraph.Models
{
    public class AuthenticationConfigModel
    {
            public string Instance { get; set; } = "https://login.microsoftonline.com/{0}";
            public string ApiUrl { get; set; } = "https://graph.microsoft.com/";
            public string ClientId { get; set; }
            public string Tenant { get; set; }
            public string DriveId { get; set; }
            public string SiteId { get; set; } 
            public string ItemID { get; set; }
            public string ListId { get; set; }
            public string Authority
            {
                get
                {
                    return String.Format(CultureInfo.InvariantCulture, Instance, Tenant);
                }
            }
            public string ClientSecret { get; set; }

            public static AuthenticationConfigModel ReadFromJsonFile(string path)
            {
                IConfigurationRoot Configuration;

                var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);

                Configuration = builder.Build();
                return Configuration.Get<AuthenticationConfigModel>();
            }
        }
    }
