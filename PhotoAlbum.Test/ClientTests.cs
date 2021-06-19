using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PhotoAlbum.Logic.Implementation;
using PhotoAlbum.Logic.Interfaces;
using PhotoAlbum.Logic.Models;

namespace PhotoAlbum.Test
{
    [TestFixture]
    public class ClientTests
    {
        private Dictionary<string, AlbumModel> _cachedResults;

        [TestCase]
        public async Task When_AlbumId_Is_1_Expect_Not_Null()
        {
            var repoMock = GetRepoMock();
            var loggerMock = GetLoggerMock();

            var client = new PhotoAlbumClient(photoAlbumRepository: repoMock.Object,
                                              logger: loggerMock.Object);

            var result = await client.GetAlbums(albumId: "1");

            Assert.IsNotNull(anObject: result);
        }

        [TestCase]
        public async Task When_AlbumId_Is_1_Expect_1_Album()
        {
            var repoMock = GetRepoMock();
            var loggerMock = GetLoggerMock();

            var client = new PhotoAlbumClient(photoAlbumRepository: repoMock.Object,
                                              logger: loggerMock.Object);

            var result = await client.GetAlbums(albumId: "1");

            var asList = result.ToList();

            Assert.AreEqual(expected: 1,
                            actual: asList.Count);
        }

        [TestCase]
        public async Task When_AlbumId_Is_1_Expect_50_Photos()
        {
            var repoMock = GetRepoMock();
            var loggerMock = GetLoggerMock();

            var client = new PhotoAlbumClient(photoAlbumRepository: repoMock.Object,
                                              logger: loggerMock.Object);

            var result = await client.GetAlbums(albumId: "1");

            var asList = result.ToList();

            Assert.AreEqual(expected: 50,
                            actual: asList[index: 0]
                                    .Photos.Count);
        }

        [TestCase]
        public async Task When_AlbumId_Is_1_Expect_First_Photo_Title_Testing()
        {
            var repoMock = GetRepoMock();
            var loggerMock = GetLoggerMock();

            var client = new PhotoAlbumClient(photoAlbumRepository: repoMock.Object,
                                              logger: loggerMock.Object);

            var result = await client.GetAlbums(albumId: "1");

            var asList = result.ToList();

            Assert.AreEqual(expected: "Testing",
                            actual: asList[index: 0]
                                    .Photos[index: 0]
                                    .Title);
        }

        [TestCase]
        public async Task When_AlbumId_Is_1_Expect_All_Photos_To_Be_Album_1()
        {
            var repoMock = GetRepoMock();
            var loggerMock = GetLoggerMock();

            var client = new PhotoAlbumClient(photoAlbumRepository: repoMock.Object,
                                              logger: loggerMock.Object);

            var result = await client.GetAlbums(albumId: "1");

            var asList = result.ToList();

            var allAlbum1 = true;

            foreach(var item in asList[index: 0]
                .Photos)
            {
                if(!item.AlbumId.Equals(value: "1",
                                        comparisonType: StringComparison.InvariantCultureIgnoreCase))
                {
                    allAlbum1 = false;
                    break;
                }
            }

            Assert.IsTrue(condition: allAlbum1);
        }

        [TestCase]
        public async Task When_AlbumId_Is_Null_Expect_Not_Null()
        {
            var repoMock = GetRepoMock();
            var loggerMock = GetLoggerMock();

            var client = new PhotoAlbumClient(photoAlbumRepository: repoMock.Object,
                                              logger: loggerMock.Object);

            var result = await client.GetAlbums();

            Assert.IsNotNull(anObject: result);
        }

        [TestCase]
        public async Task When_AlbumId_Is_Null_Expect_100_Albums()
        {
            var repoMock = GetRepoMock();
            var loggerMock = GetLoggerMock();

            var client = new PhotoAlbumClient(photoAlbumRepository: repoMock.Object,
                                              logger: loggerMock.Object);

            var result = await client.GetAlbums();

            var asList = result.ToList();

            Assert.AreEqual(expected: 100,
                            actual: asList.Count);
        }

        [TestCase]
        public async Task When_AlbumId_Is_Null_Expect_First_Album_Photo_Title_Testing()
        {
            var repoMock = GetRepoMock();
            var loggerMock = GetLoggerMock();

            var client = new PhotoAlbumClient(photoAlbumRepository: repoMock.Object,
                                              logger: loggerMock.Object);

            var result = await client.GetAlbums();

            var asList = result.ToList();

            Assert.AreEqual(expected: "Testing",
                            actual: asList[index: 0]
                                    .Photos[index: 0]
                                    .Title);
        }

        [TestCase]
        public async Task When_AlbumId_Is_Gibberish_Expect_Not_Null()
        {
            var repoMock = GetRepoMock();
            var loggerMock = GetLoggerMock();

            var client = new PhotoAlbumClient(photoAlbumRepository: repoMock.Object,
                                              logger: loggerMock.Object);

            var result = await client.GetAlbums(albumId: "rage monkey");

            Assert.IsNotNull(anObject: result);
        }

        [TestCase]
        public async Task When_AlbumId_Is_Gibberish_Expect_0_Photos()
        {
            var repoMock = GetRepoMock();
            var loggerMock = GetLoggerMock();

            var client = new PhotoAlbumClient(photoAlbumRepository: repoMock.Object,
                                              logger: loggerMock.Object);

            var result = await client.GetAlbums(albumId: "rage monkey");

            var asList = result.ToList();

            Assert.AreEqual(expected: 0,
                            actual: asList.Count);
        }

        private Mock<IPhotoAlbumRepository> GetRepoMock()
        {
            var retval = new Mock<IPhotoAlbumRepository>();

            retval.Setup(expression: x => x.LoadAlbumData(It.IsAny<string>()))
                  .ReturnsAsync(value: GetDataFromFile());

            return retval;
        }

        private Mock<ILogger> GetLoggerMock()
        {
            return new Mock<ILogger>();
        }

        private Dictionary<string, AlbumModel> GetDataFromFile()
        {
            var retval = new Dictionary<string, AlbumModel>();

            if(_cachedResults == null)
            {
                string jsonPhotoData = null;

                using(var fs = new FileStream(path: $"{Environment.CurrentDirectory}\\TestingData.json",
                                              mode: FileMode.Open))
                {
                    using(var reader = new StreamReader(stream: fs))
                    {
                        jsonPhotoData = reader.ReadToEnd();
                    }
                }

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
                }

                _cachedResults = retval;
            }
            else
            {
                retval = _cachedResults;
            }

            return retval;
        }
    }
}