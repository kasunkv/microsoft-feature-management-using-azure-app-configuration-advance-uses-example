using Microsoft.FeatureManagement;
using MusicStore.Data;
using MusicStore.Shared.FeatureManagement;
using MusicStore.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicStore.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IFeatureManager _featureManager;
        private readonly double _discount = 0.25;

        public AlbumService(IAlbumRepository albumRepository, IFeatureManagerSnapshot featureManager)
        {
            _albumRepository = albumRepository;
            _featureManager = featureManager;
        }

        public List<Album> AllAlbums()
        {
            var albums = _albumRepository.GetAll();
            return albums;
        }

        public async Task<List<Album>> PromotionalAlbumsAsync(int albumCount = 6)
        {
            var promotionalAlbums = GetRandomAlbums(albumCount);

            if (await _featureManager.IsEnabledAsync(Features.PromotionDiscounts))
            {
                foreach (var album in promotionalAlbums)
                {
                    album.PromoPrice = Math.Round((album.Price - (album.Price * _discount)), 2);
                }
            }

            return promotionalAlbums;
        }

        public List<Album> UserPreferenceAlbums(int albumCount = 4)
        {
            var userAlbums = GetRandomAlbums(albumCount);
            return userAlbums;
        }

        private List<Album> GetRandomAlbums(int albumCount)
        {
            List<Album> result = new List<Album>();
            var albums = _albumRepository.GetAll();
            var random = new Random();

            for (int i = 0; i < albumCount; i++)
            {
                result.Add(albums[random.Next(0, albums.Count)]);
            }

            return result;
        }
    }
}
