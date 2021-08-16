using App.NETFramework.Core.Application.FilesStorages.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace App.NETFramework.Core.Application.FilesStorages
{
    public class ImageStorage : IFileStorage
    {
        public async Task<(string, bool)> DeleteFileInBlob(string container, string filename, FileStorageConfiguration fileStorageConfiguration)
        {
            string storageConnectionString = fileStorageConfiguration.StorageConnectionString;
            string AZURE_STORAGE_CDN = fileStorageConfiguration.AzureStorage_Cdn;
             CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'uploadblob' and append a GUID value to it to make the name unique.
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(container);
                    if (!await cloudBlobContainer.ExistsAsync())
                        return ("El Blob Container NO existe", false);

                    // Set the permissions so the blobs are public.
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);
                    await cloudBlockBlob.DeleteAsync();

                    return (string.Empty, true);
                }
                catch (StorageException ex)
                {
                    return (ex.Message, false);
                }
            }
            else
            {
                return (string.Empty, false);
            }
        }

        public async Task<(string, string)> UploadToBlob(string container, string filename, FileStorageConfiguration fileStorageConfiguration, byte[] imageBuffer = null, Stream stream = null)
        {
            string storageConnectionString = fileStorageConfiguration.StorageConnectionString;
            string AZURE_STORAGE_CDN = fileStorageConfiguration.AzureStorage_Cdn;

            CloudStorageAccount storageAccount;
            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'uploadblob' and append a GUID value to it to make the name unique.
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(container);
                    if (!await cloudBlobContainer.ExistsAsync())
                        await cloudBlobContainer.CreateAsync();

                    // Set the permissions so the blobs are public.
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);

                    if (imageBuffer != null)
                    {
                        // OPTION A: use imageBuffer (converted from memory stream)
                        await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);
                    }
                    else if (stream != null)
                    {
                        // OPTION B: pass in memory stream directly
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return ("Problemas con el archivo al momento de subir la imagen", null);
                    }

                    return (string.Empty, $"{AZURE_STORAGE_CDN}/{container}/{filename}");
                }
                catch (StorageException ex)
                {
                    return (ex.Message, null);
                }
            }
            else
            {
                return (string.Empty, null);
            }
        }
    }
}