using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GT.Client.Data;
using GT.Models;
using Microsoft.AspNetCore.Http;

namespace GT.Client.ViewModels
{
    public class GroceryViewModel : INotifyPropertyChanged
    {
        private readonly IGroceryDataAccess _dataAccess;
        private readonly Grocery _newGrocery = new();

        public GroceryViewModel(IGroceryDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public List<string> Errors { get; } = new();
        public string newName = null;
        private int asyncCount = 0;
        public string NewName
        {
            get => newName;
            set
            {
                newName = value;
                _newGrocery.Name = value;
                Errors.Clear();
                RaisePropChange(nameof(NewName));
            }
        }
        public bool ValidationErrors
        {
            get
            {
                return Errors.Any();
            }
        }

        public async Task AddNewAsync()
        {
       
            List<ValidationResult> results = new();
            ValidationContext validation = new(_newGrocery);
            Errors.Clear();
            if (!string.IsNullOrWhiteSpace(NewName) && !ValidationErrors)
            {
                Grocery newItem = new() { Name = NewName };
               
                await _dataAccess.AddAsync(newItem);

                NewName = string.Empty;
                RaisePropChange(nameof(GroceryAsync));
            }
            else if (!Validator.TryValidateObject(_newGrocery, validation, results))
            {
                foreach (var result in results)
                {
                    Errors.Add(result.ErrorMessage);
                }
                RaisePropChange(nameof(Errors));
            }
        }

        public async Task<IEnumerable<Grocery>> GroceryAsync()
        {
            var result = await _dataAccess.GetAsync(showAll);
            return result;
        }

        public async Task RemoveAsync(Grocery grocery)
        {
           
            await _dataAccess.DeleteAsync(grocery);
           
            RaisePropChange(nameof(GroceryAsync));
        }

        public async Task MarkGroceryAsExpireAsync(Grocery grocery)
        {
            grocery.MarkExpire();
           
            await _dataAccess.UpdateAsync(grocery);
           
            RaisePropChange(nameof(GroceryAsync));
        }

 
        private bool showAll;
        public bool ShowAll
        {
            get => showAll;
            set
            {
                if (value != showAll)
                {
                    showAll = value;
                    RaisePropChange(nameof(ShowAll), true);
                }
            }
        }


        private void RaisePropChange(string property, bool includeGroceries = false)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            if (includeGroceries)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroceryAsync)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
  
    }
}
