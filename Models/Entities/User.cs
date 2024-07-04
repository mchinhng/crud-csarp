using System.ComponentModel.DataAnnotations;

namespace UserInfo.Web.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; }

        public string Gender { get; set; }

        [Required(ErrorMessage = "DoB is invalid")]
        [DataType(DataType.DateTime)]
        public DateTime DoB { get; set; }

        public bool DeleletedFlg {get; set;}
    }
}