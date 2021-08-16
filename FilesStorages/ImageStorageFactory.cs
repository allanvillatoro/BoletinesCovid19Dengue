namespace App.NETFramework.Core.Application.FilesStorages
{
    public class ImageStorageFactory : IFileStorageFactory
    {
        public IFileStorage Create()
        {
            return new ImageStorage();
        }
    }
}