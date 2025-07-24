namespace AirJourney_Blog.PL.Helper
{
    public class ImageService
    {

        public string UploadFile(IFormFile file, string folderName)
        {


            // 1. Validate the file
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            // 2. Create the target directory if it doesn't exist
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 3. Generate a unique filename
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4. Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // 5. Return the relative URL (for web access)
            return $"/Files/{folderName}/{uniqueFileName}";

        }

        public void DeleteFile(string imageUrl, string folderName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                    throw new Exception("Image URL cannot be empty");

                var fileName = Path.GetFileName(imageUrl); 

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", folderName);
                var filePath = Path.Combine(folderPath, fileName);

                if (!File.Exists(filePath))
                    throw new Exception("File not found");

                File.Delete(filePath);

                Console.WriteLine($"File {fileName} deleted successfully from {folderPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex.Message}");
                throw new Exception("An error occurred while deleting the image", ex);
            }
        }



    }
}
