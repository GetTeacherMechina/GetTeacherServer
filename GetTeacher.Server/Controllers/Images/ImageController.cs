using Microsoft.AspNetCore.Mvc;

public class ImagesController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public ImagesController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [Route("/api/v1/images")]
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");
        var imageName = string.Concat(Path.GetExtension(file.FileName).Where(char.IsLetterOrDigit));
        var uniqueFileName = $"{Guid.NewGuid()}{imageName}.png";
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = $"/images/{uniqueFileName}";

        return Ok(new { Url = imageUrl });
    }
    [Route("/api/v1/images/{uid}")]
    [HttpGet]
    public IActionResult GetImage(string uid)
    {
        if (string.IsNullOrWhiteSpace(uid))
            return BadRequest("Invalid UID.");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        var filePath = Path.Combine(uploadsFolder, uid);

        if (!System.IO.File.Exists(filePath))
            return NotFound("Image not found.");

        var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
        var contentType = fileExtension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(fileStream, contentType);
    }
}
