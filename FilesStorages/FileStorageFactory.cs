namespace App.NETFramework.Core.Application.FilesStorages
{
    public static class FileStorageFactory
    {
        private static IFileStorageFactory _currentIFileStorageFactory;

        public static void SetCurrent(IFileStorageFactory fileStorageFactory)
        {
            _currentIFileStorageFactory = fileStorageFactory;
        }

        public static IFileStorage Create()
        {
            if (_currentIFileStorageFactory == null)
            {
                _currentIFileStorageFactory = new ImageStorageFactory();
            }
            return _currentIFileStorageFactory?.Create();
        }
    }
}