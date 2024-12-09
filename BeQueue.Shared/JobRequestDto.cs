using System.ComponentModel.DataAnnotations;

namespace BeQueue.Shared;

public class JobRequestDto
{
  [Required]
  [MinLength(3)]
  public required string JobName { get; set; }
}