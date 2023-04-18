using AutoMapper;
using Domain.Entities;
using Newtonsoft.Json;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HttpHelper
{
    public interface IHttpClientHelper
    {
        Task<List<Location>> LocationDetails(float longtitude, float latitude);
        Task<Predictions> LocationPredictions(string parameters);
        Task<LocationDto> Location(string parameters);
        Task<LocationDto> LongitudeAndLatitlude(string parameters);
    }

    public class HttpClientHelper : IHttpClientHelper
    {
       
        public async Task<List<Location>> LocationDetails(float longtitude, float latitude)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(string.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius=100000&type=bar&key=AIzaSyCF14T7yj0sisgCqX0qFCsSHSwiwJ7RWoY", latitude,longtitude ));
                var result = JsonConvert.DeserializeObject<PlacesApiQueryResponse>(response);
                var geometry = result.results.Select(x => x.geometry).ToList();
                var locations = geometry.Select(x => x.location).ToList();
                //var latitudes = locations.Select(x => x.lat);
                //var longitudes = locations.Select(x => x.lng);
                //var locationDetails = _mapper.Map<List<LocationDetailsDto>>(locations);

                return locations;
            }


        }

        public async Task<Predictions> LocationPredictions(string parameters)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&types=geocode&key=AIzaSyCF14T7yj0sisgCqX0qFCsSHSwiwJ7RWoY", parameters));
                var result = JsonConvert.DeserializeObject<Predictions>(response);

                return result;
            }


        }
        public async Task<LocationDto> Location(string parameters)
        {
            using (var client = new HttpClient())
            {
                if (parameters != null)
                {

                    var response = await client.GetStringAsync(string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&types=geocode&key=AIzaSyCF14T7yj0sisgCqX0qFCsSHSwiwJ7RWoY", parameters));
                    var result = JsonConvert.DeserializeObject<Predictions>(response);
                    var placeid = result.predictions.Select(x => x.place_id).FirstOrDefault();
                    var newresp = await client.GetStringAsync(string.Format("https://maps.googleapis.com/maps/api/place/details/json?place_id={0}&key=AIzaSyCF14T7yj0sisgCqX0qFCsSHSwiwJ7RWoY", placeid));
                    var reet = newresp.Trim();
                    var res = JsonConvert.DeserializeObject<Rootobject>(reet);
                    var Latitude = res.result.geometry.location.lat;
                    var longitude = res.result.geometry.location.lng;
                    return new LocationDto
                    {
                        latitude = Latitude,
                        longitude = longitude
                    };
                }

                return new LocationDto
                {
                    latitude = 0,
                    longitude = 0
                };
            }


        }

        public async Task<LocationDto> LongitudeAndLatitlude(string parameters)
        {
            using (var client = new HttpClient())
            {
                if (parameters != null)
                {
                    var newresp = await client.GetStringAsync(string.Format("https://maps.googleapis.com/maps/api/place/details/json?place_id={0}&key=AIzaSyCF14T7yj0sisgCqX0qFCsSHSwiwJ7RWoY", parameters));
                    var reet = newresp.Trim();
                    var res = JsonConvert.DeserializeObject<Rootobject>(reet);
                    var Latitude = res.result.geometry.location.lat;
                    var longitude = res.result.geometry.location.lng;
                    return new LocationDto
                    {
                        latitude = Latitude,
                        longitude = longitude
                    };
                }
                return new LocationDto
                {
                    latitude = 0,
                    longitude = 0
                };
            }


        }

        private async Task<Predictions> PrivateLocationPredictions(string parameters)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&types=geocode&key=AIzaSyCF14T7yj0sisgCqX0qFCsSHSwiwJ7RWoY", parameters));
                var result = JsonConvert.DeserializeObject<Predictions>(response);

                return result;
            }


        }

        private async Task<LocationDto> PrivteLocation(string parameters)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&types=geocode&key=AIzaSyCF14T7yj0sisgCqX0qFCsSHSwiwJ7RWoY", parameters));
                var result = JsonConvert.DeserializeObject<Predictions>(response);
                var placeid = result.predictions.Select(x => x.place_id).FirstOrDefault();
                var newresp = await client.GetStringAsync(string.Format("https://maps.googleapis.com/maps/api/place/details/json?place_id={0}&key=AIzaSyCF14T7yj0sisgCqX0qFCsSHSwiwJ7RWoY", placeid));
                var reet = newresp.Trim();
                var res = JsonConvert.DeserializeObject<Rootobject>(reet);
                var Latitude = res.result.geometry.location.lat;
                var longitude = res.result.geometry.location.lng;
                return new LocationDto
                {
                    latitude = Latitude,
                    longitude = longitude
                };
            }


        }
    }
}
