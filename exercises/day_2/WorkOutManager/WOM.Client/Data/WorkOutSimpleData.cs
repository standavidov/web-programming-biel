﻿using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using WOM.Models;

namespace WOM.Client.Data
{
    public class WorkOutSimpleData : IWorkOutDataAccess
    {
        private readonly IODataClient _client;

        public WorkOutSimpleData(HttpClient client)
        {
            client.BaseAddress = new Uri("https://localhost:55197/odata");
            ODataClientSettings settings = new(client);
            _client = new ODataClient(settings);
        }

        public async Task<WorkOut> AddAsync(WorkOut item)
        {
            List<ValidationResult> results = new();
            ValidationContext validation = new(item);
            if (Validator.TryValidateObject(item, validation, results))
            {
                return await _client.For<WorkOut>().Set(item).InsertEntryAsync();
            }
            else
            {
                throw new ValidationException();
            }
        }

        public async Task<bool> DeleteAsync(WorkOut item)
        {
            await _client.For<WorkOut>().Key(item.Id).DeleteEntryAsync();
            return true;
        }

        public async Task<IEnumerable<WorkOut>> GetAsync(bool showAll)
        {
            var helper = _client.For<WorkOut>();
            if (!showAll)
            {
                helper.Filter(workOut => !workOut.Complete);
            }
            else
            {
                helper.OrderBy(workOut => workOut.Description);
            }

            return await helper.FindEntriesAsync();
        }

        public async Task<WorkOut> UpdateAsync(WorkOut item)
        {
            List<ValidationResult> results = new();
            ValidationContext validation = new(item);
            if (Validator.TryValidateObject(item, validation, results))
            {
                try
                {
                    await _client.For<WorkOut>().Key(item.Id).Set(item).UpdateEntryAsync();
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
