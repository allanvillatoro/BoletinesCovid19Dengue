using App.NETFramework.Core.Application.FilesStorages.Models;
using System.IO;
using System.Threading.Tasks;

namespace App.NETFramework.Core.Application.FilesStorages
{
    public interface IFileStorage
    {
        Task<(string, string)> UploadToBlob(string container, string filename, FileStorageConfiguration fileStorageConfiguration, byte[] imageBuffer = null, Stream stream = null);
        Task<(string, bool)> DeleteFileInBlob(string container, string filename, FileStorageConfiguration fileStorageConfiguration);
    }
}