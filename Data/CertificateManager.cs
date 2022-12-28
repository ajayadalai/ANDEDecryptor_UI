using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBDecryptor.Data
{
    public class CertificateManager
    {
        public bool SaveCertificate(string path , string password)
        {
            var repository = new CertificateRepository();
            try
            {
                repository.SaveCertificate(path, password);
            }
            catch(Exception ex)
            {

            }
            return true;
        }

        public Certificate GetCertificate()
        {
            var repository = new CertificateRepository();

            return repository.GetCertificate();
            
        }
    }
}
