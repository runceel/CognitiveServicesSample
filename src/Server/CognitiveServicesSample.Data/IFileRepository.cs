using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesSample.Data
{
    public interface IFileRepository
    {
        Task<string> UploadAsync(byte[] binary);
    }
}
