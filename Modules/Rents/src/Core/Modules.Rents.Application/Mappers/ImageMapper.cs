using Common.Application;
using Common.Application.Buckets;
using Microsoft.AspNetCore.Http;

namespace Modules.Rents.Application.Mappers
{
    public class ImageMapper(IFileService fileService) : IMapper<ICollection<IFormFile>, Task<ICollection<string>>>
    {
        public async Task<ICollection<string>> Map(ICollection<IFormFile> source)
        {
            ICollection<string> paths = [];
            foreach (var img in source)
            {
                paths.Add(await fileService.UploadFileAsync(img));
            }
            return paths;
        }
    }
}
