using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

// The shape of one signup attempt. The bracketed attributes below are data
// annotations — rules Blazor's EditForm can check automatically, without a
// single handwritten if-statement (see Pages/Registration.razor).
public class RegistrationModel
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;
}
