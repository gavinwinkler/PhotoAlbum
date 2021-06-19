using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PhotoAlbum.Logic.Interfaces;
using PhotoAlbum.Logic.Models;

namespace PhotoAlbum.Logic.Implementation
{
    /// <inheritdoc />
    /// <summary>
    ///     Class PhotoAlbumClient.
    ///     Implements the <see cref="T:PhotoAlbum.Logic.Interfaces.IPhotoAlbumClient" />
    /// </summary>
    /// <seealso cref="T:PhotoAlbum.Logic.Interfaces.IPhotoAlbumClient" />
    /// <remarks>
    ///     To prevent confusion this is intended to be an injected singleton
    /// </remarks>
    public class PhotoAlbumClient : IPhotoAlbumClient
    {
        private readonly ILogger _logger;
        private readonly IPhotoAlbumRepository _photoAlbumRepository;
        private bool _isDisposing;

        public PhotoAlbumClient(IPhotoAlbumRepository photoAlbumRepository,
                                ILogger logger)
        {
            _photoAlbumRepository = photoAlbumRepository ?? throw new ArgumentNullException(paramName: nameof(photoAlbumRepository));
            _logger = logger ?? throw new ArgumentNullException(paramName: nameof(logger));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if(!_isDisposing)
            {
                _photoAlbumRepository.Dispose();
                GC.SuppressFinalize(obj: this);
                _isDisposing = true;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AlbumModel>> GetAlbums(string albumId = null)
        {
            var loadedData = await _photoAlbumRepository.LoadAlbumData(albumId: albumId);

            return ExtractAlbumsFromDictionary(albumData: loadedData,
                                               albumId: albumId);
        }

        private List<AlbumModel> ExtractAlbumsFromDictionary(Dictionary<string, AlbumModel> albumData,
                                                             string albumId = null)
        {
            var retval = new List<AlbumModel>();

            if(albumData != null)
            {
                if(!string.IsNullOrWhiteSpace(value: albumId))
                {
                    if(albumData.ContainsKey(key: albumId)
                       && albumData[key: albumId] != null)
                    {
                        retval.Add(item: albumData[key: albumId]);
                    }
                }
                else
                {
                    foreach(var key in albumData.Keys)
                    {
                        if(albumData[key: key] != null)
                        {
                            retval.Add(item: albumData[key: key]);
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning(message: "The album data is null");
            }

            return retval;
        }
    }
}