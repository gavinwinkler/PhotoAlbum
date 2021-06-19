﻿using System;
using System.Collections.Generic;

namespace PhotoAlbum.Logic.Models
{
    /// <summary>
    ///     Class AlbumDataCacheModel.
    /// </summary>
    /// <autogeneratedoc />
    public class AlbumDataCacheModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AlbumDataCacheModel" /> class.
        /// </summary>
        /// <autogeneratedoc />
        public AlbumDataCacheModel()
        {
            AlbumData = new Dictionary<string, AlbumModel>();
        }

        /// <summary>
        ///     Gets or sets the album data.
        /// </summary>
        /// <value>The album data.</value>
        /// <autogeneratedoc />
        public Dictionary<string, AlbumModel> AlbumData { get; set; }

        /// <summary>
        ///     Gets or sets the expiration date.
        /// </summary>
        /// <value>The expiration date.</value>
        /// <autogeneratedoc />
        public DateTimeOffset ExpirationDate { get; set; }
    }
}