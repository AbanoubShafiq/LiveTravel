using AirJourney_Blog.PL.Helper;
using AirJourney_Blog.Service.Services.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirJourney_Blog.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IImageService imgSer;

        //private readonly ImageService imgSer;

        public FileController(IImageService imgSer,
            ILogger<FileController> logger)
        {
            this.imgSer = imgSer;
            //imgSer = _imgSer;
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var (fileId, url) = await imgSer.UploadImageAsync(file);
            return Ok(new { ImageUrl = url, FileId = fileId });
        }

        //[HttpDelete]
        //public IActionResult DeleteImage(string folderName, string fileName)
        //{
        //    try
        //    {
        //        // Input validation
        //        if (string.IsNullOrWhiteSpace(fileName))
        //            return BadRequest("File name cannot be empty");

        //        if (string.IsNullOrWhiteSpace(folderName))
        //            return BadRequest("Folder name cannot be empty");

        //        imgSer.DeleteFile(fileName, folderName);
        //        return Ok(new { Message = $"File {fileName} deleted successfully" });


        //    }
        //    catch
        //    {
        //        return StatusCode(500, new { Error = "An error occurred while deleting the file" });

        //    }
        //}


    }
}
