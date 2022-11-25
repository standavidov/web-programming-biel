using System;
using System.ComponentModel.DataAnnotations;

namespace GT.Models
{
    public class Grocery
    {
        public Grocery()
        {
            Added = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public bool Expire { get; set; }
        public DateTime Added { get; set; }
        public DateTime? MarkedExpire { get; set; }

        [Required(ErrorMessage = "Grocery name is required.")]
        public string Name { get; set; }

        public void MarkExpire()
        {
            if (!Expire)
            {
                Expire = true;
                MarkedExpire = DateTime.UtcNow;
            }
        }
    }
}
