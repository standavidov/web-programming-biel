using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using GT.Models;

namespace GT.Client.Data
{
    public class GrocerySimpleData : IGroceryDataAccess
    {
        private readonly IODataClient _client;

        public GrocerySimpleData(HttpClient client)
        {
            client.BaseAddress = new Uri("https://localhost:55197/odata");
            ODataClientSettings settings = new(client);
            _client = new ODataClient(settings);
        }

        public async Task<Grocery> AddAsync(Grocery item)
        {
            List<ValidationResult> results = new();
            ValidationContext validation = new(item);
            if (Validator.TryValidateObject(item, validation, results))
            {
                return await _client.For<Grocery>().Set(item).InsertEntryAsync();
            }
            else
            {
                throw new ValidationException();
            }
        }

        public async Task<bool> DeleteAsync(Grocery item)
        {
            await _client.For<Grocery>().Key(item.Id).DeleteEntryAsync();
            return true;
        }

        public async Task<IEnumerable<Grocery>> GetAsync(bool showAll)
        {
            var helper = _client.For<Grocery>();
            if (!showAll)
            {
                helper.Filter(grocery => !grocery.Expire);
            }
            else
            {
                helper.OrderBy(grocery => grocery.Name);
            }

            return await helper.FindEntriesAsync();
        }

        public async Task<Grocery> UpdateAsync(Grocery item)
        {
            List<ValidationResult> results = new();
            ValidationContext validation = new(item);
            if (Validator.TryValidateObject(item, validation, results))
            {
                try
                {
                    await _client.For<Grocery>().Key(item.Id).Set(item).UpdateEntryAsync();
                }
                catch (Exception)
                {

                    throw;
                }
                return item;
            }
            else
            {
                throw new ValidationException();
            }
        }
    }
}
