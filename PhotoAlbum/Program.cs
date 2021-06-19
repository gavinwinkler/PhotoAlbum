using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotoAlbum.Logic.Implementation;
using PhotoAlbum.Logic.Models;

namespace PhotoAlbum
{
    internal class Program
    {
        private static bool DisplayPhotoData(IEnumerable<AlbumModel> albums)
        {
            if(albums != null)
            {
                var albumsList = albums.ToList();

                var multipleAlbums = albumsList.Count > 1;

                foreach(var album in albumsList)
                {
                    if(album != null)
                    {
                        foreach(var photo in album.Photos)
                        {
                            if(photo != null)
                            {
                                Console.WriteLine(value: multipleAlbums
                                                             ? $"Album: {photo.AlbumId} Id: {photo.Id} Title: {photo.Title}"
                                                             : $"Id: {photo.Id} Title: {photo.Title}");
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static void Main(string[] args)
        {
            Console.WriteLine(value: "Welcome to the Photo Album Integration tests!");

            Console.WriteLine(value: "You can type help for assistance with the supported commands.");

            MainAsync()
                .Wait();

            Console.WriteLine(value: "And now we are done. Bye Bye");
        }

        private static async Task MainAsync()
        {
            //basic setup you can ignore
            var shouldLoop = true;

            var serviceProvider = new ServiceCollection()
                                  .AddLogging()
                                  .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                                        .CreateLogger(categoryName: "Console");

            var photoAlbumRepository = new PhotoAlbumRepository(urlToLoadDataFrom: "https://jsonplaceholder.typicode.com/photos",
                                                                logger: logger);

            var photoAlbumClient = new PhotoAlbumClient(photoAlbumRepository: photoAlbumRepository,
                                                        logger: logger);

            //start of console control loop
            while(shouldLoop)
            {
                var command = Console.ReadLine();

                switch(command.ToUpperInvariant())
                {
                    case "EXIT":
                        shouldLoop = false;
                        break;
                    case "HELP":
                        Console.WriteLine(value: "*****************");
                        Console.WriteLine(value: "Supported Commands:");
                        Console.WriteLine(value: "help (displays help)");
                        Console.WriteLine(value: "exit (exits the program)");
                        Console.WriteLine(value: "album all (displays all the photos for all the albums)");
                        Console.WriteLine(value: "album <album id here> (displays all the photos for all the specified album)");
                        Console.WriteLine(value: "*****************");
                        break;
                    default:
                        if(!string.IsNullOrWhiteSpace(value: command))
                        {
                            var split = command.Split(separator: " ");

                            if(split.Length == 2)
                            {
                                if(split[0]
                                    .Equals(value: "album",
                                            comparisonType: StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if(split[1]
                                        .Equals(value: "all",
                                                comparisonType: StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        var allPhotos = await photoAlbumClient.GetAlbums();

                                        DisplayPhotoData(albums: allPhotos);
                                    }
                                    else
                                    {
                                        var albumPhotos = await photoAlbumClient.GetAlbums(albumId: split[1]);

                                        DisplayPhotoData(albums: albumPhotos);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine(value: "Sorry I don't understand, you can type help for assistance with the supported commands.");
                            }
                        }
                        else
                        {
                            Console.WriteLine(value: "You must enter a command, you can type help for assistance with the supported commands.");
                        }

                        break;
                }
            }
            //end of console control loop
        }
    }
}