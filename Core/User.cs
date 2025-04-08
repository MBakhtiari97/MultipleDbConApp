using System.ComponentModel.DataAnnotations;

namespace Core;

class User
{
    [Key]
    [Required]
    public int UserId { get; set; }
    [Required]
    [StringLength(250, ErrorMessage = "{0} is too long")]
    public string Username { get; set; } = string.Empty;
    [Required]
    [StringLength(250, ErrorMessage = "{0} is too long")]
    [EmailAddress(ErrorMessage = "{0} is not valid")]
    public string EmailAddress { get; set; } = string.Empty;
    [Required]
    [StringLength(20, ErrorMessage = "{0} is too long")]
    public string Password { get; set; } = string.Empty;
    [Required]
    public DateTime RegisterDate { get; set; } = DateTime.Now;

    #region Navigation Properties
    public ICollection<Log> Logs { get; set; } = null!;
    #endregion
}