﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhotoAlbum.Logic.Interfaces;
using PhotoAlbum.Logic.Models;

namespace PhotoAlbum.Logic.Implementation
{
    /// <inheritdoc />
    /// <summary>
    ///     Class PhotoAlbumRepository.
    ///     Implements the <see cref="T:PhotoAlbum.Logic.Interfaces.IPhotoAlbumRepository" />
    /// </summary>
    /// <seealso cref="T:PhotoAlbum.Logic.Interfaces.IPhotoAlbumRepository" />
    /// <remarks>
    ///     This is not a good candidate for unit testing because all it does is make a call to the third party url
    ///     Integration testing is appropriate but will not be included in the unit test suite so as not to slow the build down
    /// </remarks>
    public class PhotoAlbumRepository : IPhotoAlbumRepository
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private bool _isDisposing;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PhotoAlbumRepository" /> class.
        /// </summary>
        /// <param name="urlToLoadDataFrom">The URL to load data from.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        /// <autogeneratedoc />
        public PhotoAlbumRepository(string urlToLoadDataFrom,
                                    ILogger logger)
        {
            if(string.IsNullOrWhiteSpace(value: urlToLoadDataFrom))
            {
                throw new ArgumentNullException(paramName: nameof(urlToLoadDataFrom));
            }

            _logger = logger ?? throw new ArgumentNullException(paramName: nameof(logger));
            _httpClient = new HttpClient();
            _baseUrl = urlToLoadDataFrom;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if(!_isDisposing)
            {
                _httpClient.Dispose();
                GC.SuppressFinalize(obj: this);
                _isDisposing = true;
            }
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, AlbumModel>> LoadAlbumData(string albumId = null)
        {
            var retval = new Dictionary<string, AlbumModel>();

            if(!string.IsNullOrWhiteSpace(value: _baseUrl))
            {
                var builder = new StringBuilder();

                builder.Append(value: _baseUrl);

                if(!string.IsNullOrWhiteSpace(value: albumId))
                {
                    builder.Append(value: $"?albumId={albumId}");
                }

                try
                {
                    //load the data from the url
                    var response = await _httpClient.GetAsync(requestUri: builder.ToString());

                    if(response.IsSuccessStatusCode)
                    {
                        var jsonPhotoData = await response.Content.ReadAsStringAsync();

                        if(!string.IsNullOrWhiteSpace(value: jsonPhotoData))
                        {
                            var photos = JsonConvert.DeserializeObject<List<PhotoModel>>(value: jsonPhotoData);

                            if(photos?.Any() == true)
                            {
                                //load the data photo data into albums
                                foreach(var item in photos)
                                {
                                    if(item != null)
                                    {
                                        if(string.IsNullOrWhiteSpace(value: item.AlbumId))
                                        {
                                            item.AlbumId = "No Album";
                                        }

                                        if(retval.ContainsKey(key: item.AlbumId))
                                        {
                                            retval[key: item.AlbumId]
                                                .Photos.Add(item: item);
                                        }
                                        else
                                        {
                                            retval.Add(key: item.AlbumId,
                                                       value: new AlbumModel());

                                            retval[key: item.AlbumId]
                                                .Photos.Add(item: item);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _logger.LogWarning(message: $"The call to endpoint: {builder} returned no photos");
                            }
                        }
                        else
                        {
                            _logger.LogWarning(message: $"The call to endpoint: {builder} returned no data");
                        }
                    }
                    else
                    {
                        _logger.LogError(message: $"The call to endpoint: {builder} failed with status {response.StatusCode}");
                    }
                }
                catch(Exception ex)
                {
                    retval = new Dictionary<string, AlbumModel>();

                    _logger.LogError(exception: ex,
                                     message: "Failed to load the photo data due to an exception");
                }
            }
            else
            {
                _logger.LogError(message: "Failed to load the photo as somehow the base url is not set");
            }

            return retval;
        }
    }
}