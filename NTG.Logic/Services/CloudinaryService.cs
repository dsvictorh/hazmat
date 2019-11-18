using System.Linq;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NTG.Logic.Services
{
    public class ImageInfo
    {
        public Uri Image { get; internal set; }
        public string PublicId { get; internal set; }
        public string Created { get; internal set; }
    }

    public static class CloudinaryService
    {
        private const string _CLOUD_NAME = "CLOUDINARY_CLOUD_NAME";
        private const string _API_KEY = "CLOUDINARY_API_KEY";
        private const string _API_SECRET = "CLOUDINARY_API_SECRET";

        private const string _DEV_CLOUD_NAME = "DevCloudNameHere";
        private const string _DEV_API_KEY = "DevAPIKeyHere";
        private const string _DEV_API_SECRET = "DevAPISecretHere";

        private static Account GetAccount() {
            var cloudName = Environment.GetEnvironmentVariable(_CLOUD_NAME) ?? _DEV_CLOUD_NAME;
            var apiKey = Environment.GetEnvironmentVariable(_API_KEY) ?? _DEV_API_KEY;
            var apiSecret = Environment.GetEnvironmentVariable(_API_SECRET) ?? _DEV_API_SECRET;

            return new Account(
              cloudName,
              apiKey,
              apiSecret);
        }

        public static IEnumerable<ImageInfo> GetImages(string folderStructure, string[] tags = null, int maxResults = 500)
        {
            var resources = new Cloudinary(GetAccount()).ListResources(new ListResourcesByPrefixParams { Prefix = folderStructure, Tags = tags != null, MaxResults = maxResults, Type = "upload", Direction = "desc"  }).Resources;
            if (resources != null)
            {
                return resources.Where(r => tags == null || (r.Tags != null && tags.Intersect(r.Tags).Any())).Select(r => new ImageInfo { Image = r.SecureUri, PublicId = r.PublicId, Created = r.Created });
            }

            return Enumerable.Empty<ImageInfo>();
        }

        public static CloudinaryResult UploadImage(string folderStructure, string fileName, string[] tags, string path)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(path),
                Folder = folderStructure,
                Tags = tags != null ? string.Join(",", tags) : string.Empty,
            };

            var result = new Cloudinary(GetAccount()).Upload(uploadParams);
            return new CloudinaryResult {
                uri = result.SecureUri,
                publicId = result.PublicId,
                error = result.Error != null ? result.Error.Message : string.Empty
            };
        }

        public static CloudinaryResult UploadImage(string folderStructure, string fileName, Stream stream)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, stream),
                Folder = folderStructure,
            };

            var result = new Cloudinary(GetAccount()).Upload(uploadParams);
            return new CloudinaryResult
            {
                uri = result.SecureUri,
                publicId = result.PublicId,
                error = result.Error != null ? result.Error.Message : string.Empty
            };
        }

        public static CloudinaryResult DeleteImage(string publicId)
        {
            
            var cloudinary = new Cloudinary(GetAccount());
            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image,
                Invalidate = true,
            };

            var result = new Cloudinary(GetAccount()).Destroy(deleteParams);

            return new CloudinaryResult
            {
                error = result.Error != null ? 
                                result.Error.Message : 
                                string.Empty
            };
        }

        public static CloudinaryResult DeleteFolder(string prefix)
        {
            var deleteParams = new DelResParams()
            {
                ResourceType = ResourceType.Image,
                Prefix = prefix,
            };

            var result = new Cloudinary(GetAccount()).DeleteResources(deleteParams);
            return new CloudinaryResult
            {
                error = result.Error != null ? result.Error.Message : string.Empty
            };
        }
    }

    public struct CloudinaryResult
    {
        public Uri uri;
        public string publicId;
        public string error;
    }
}
