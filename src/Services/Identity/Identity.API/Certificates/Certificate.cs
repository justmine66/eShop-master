using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

namespace Identity.API.Certificates
{
    /// <summary>
    /// 证书
    /// </summary>
    static class Certificate
    {
        /// <summary>
        /// 获取X509证书.第二版
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 Get()
        {
            var assembly = typeof(Certificate).GetTypeInfo().Assembly;
            var names = assembly.GetManifestResourceNames();
            var certNamespace = names.FirstOrDefault(name => name.Contains("idsrv3test.pfx"));
            /***********************************************************************************************
             *  Please note that here we are using a local certificate only for testing purposes. In a 
             *  real environment the certificate should be created and stored in a secure way, which is out
             *  of the scope of this project.
             **********************************************************************************************/
            using (var stream = assembly.GetManifestResourceStream(certNamespace))
            {
                return new X509Certificate2(ReadStream(stream), "idsrv3test");
            }
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
