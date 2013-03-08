using plUploadHandler.Properties;
using System;
using System.IO;

namespace Westwind.plUpload
{

    /// <summary>
    /// Upload handler that uploads files into a specific folder.
    /// Depending on the unique_names flag on the plUpload component,
    /// the filename will either be unique or the original filename.
    /// </summary>
    public class plUploadFileHandler : plUploadBaseHandlerAsync
    {
        /// <summary>
        /// Physical folder location where the file will be uploaded.
        /// 
        /// Note that you can assign an IIS virtual path (~/path)
        /// to this property, which automatically translates to a 
        /// physical path.
        /// </summary>
        public string FileUploadPhysicalPath
        {
            get
            {
                if (_FileUploadPhysicalPath.StartsWith("~"))
                    _FileUploadPhysicalPath = Context.Server.MapPath(_FileUploadPhysicalPath);
                return _FileUploadPhysicalPath;
            }
            set
            {
                _FileUploadPhysicalPath = value;
            }
        }
        private string _FileUploadPhysicalPath;


        public plUploadFileHandler()
        {
            FileUploadPhysicalPath = "~/temp/";
        }

        /// <summary>
        /// Stream each chunk to a file and effectively append it. 
        /// </summary>
        /// <param name="chunkStream"></param>
        /// <param name="chunk"></param>
        /// <param name="chunks"></param>
        /// <param name="uploadedFilename"></param>
        /// <returns></returns>
        protected override bool OnUploadChunk(Stream chunkStream, int chunk, int chunks, string uploadedFilename)
        {
            var path = FileUploadPhysicalPath;

            // try to create the path
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException(Resources.UploadDirectoryDoesnTExistAndCouldnTCreate);                    
                }                
            }

            using (var stream = new FileStream(Path.Combine(path, uploadedFilename), (chunk == 0) ? FileMode.CreateNew : FileMode.Append))
            {
                chunkStream.CopyTo(stream, 16384);
            }

            return true;
        }
    }
}
