using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesSample.Data
{
    public class FileRepository : IFileRepository
    {
        private StorageSetting StorageSetting { get; }

        public FileRepository(IOptions<StorageSetting> storageSetting)
        {
            this.StorageSetting = storageSetting.Value;
        }

        public async Task<string> UploadAsync(byte[] binary)
        {
            var account = CloudStorageAccount.Parse(this.StorageSetting.ConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference("images");
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
            await blob.UploadFromByteArrayAsync(binary, 0, binary.Length);
            return blob.Uri.AbsoluteUri;
        }
    }
}
