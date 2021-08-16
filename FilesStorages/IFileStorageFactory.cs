namespace App.NETFramework.Core.Application.FilesStorages
{
    public interface IFileStorageFactory
    {
        IFileStorage Create();
    }
}