using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public static class FileHelper
{
	// Tạo file tạm thời
	public static string CreateTempFile()
	{
		return Path.GetTempFileName();
	}

	// Ghi nội dung của file upload vào file tạm thời
	public static async Task WriteToFileAsync(IFormFile formFile, string filePath)
	{
		using (var stream = new FileStream(filePath, FileMode.Create))
		{
			await formFile.CopyToAsync(stream);
		}
	}

	// Xóa file tạm thời sau khi sử dụng
	public static void DeleteTempFile(string filePath)
	{
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
	}
}
