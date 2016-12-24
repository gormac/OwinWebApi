using System.ComponentModel.DataAnnotations;

namespace Rovale.OwinWebApi.Models
{
    public class SomeObject
    {
        [Required]
        public int Id { get; set; }
        [MaxLength(30)]
        public string SomeText { get; set; }
    }
}
