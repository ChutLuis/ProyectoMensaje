using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Front.ViewModels
{
    public class LogInViewModel
    {
        [Required(ErrorMessage = "Please enter a UserName.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter a PassWord.")]
        public string PassWord { get; set; }
    }
}
