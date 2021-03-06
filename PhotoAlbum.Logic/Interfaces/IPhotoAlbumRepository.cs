using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoAlbum.Logic.Models;

namespace PhotoAlbum.Logic.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    ///     Interface IPhotoAlbumRepository
    ///     Implements the <see cref="T:System.IDisposable" />
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    /// <autogeneratedoc />
    public interface IPhotoAlbumRepository : IDisposable
    {
        /// <summary>
        ///     Loads the album data.
        /// </summary>
        /// <param name="albumId">The album identifier.</param>
        /// <returns>Task&lt;Dictionary&lt;System.String, AlbumModel&gt;&gt;.</returns>
        /// <remarks>
        ///     This does not return null
        /// </remarks>
        Task<Dictionary<string, AlbumModel>> LoadAlbumData(string albumId = null);
    }
}