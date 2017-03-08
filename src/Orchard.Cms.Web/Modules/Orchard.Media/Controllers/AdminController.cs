using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orchard.ContentManagement;
using Orchard.Media.Indexes;
using YesSql.Core.Services;

namespace Orchard.Media.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMediaFileStore _mediaFileStore;

        public AdminController(IMediaFileStore mediaFileStore)
        {
            _mediaFileStore = mediaFileStore;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetFolders(string path)
        {
            var content = (await _mediaFileStore.GetDirectoryContentAsync(path)).Where(x => x.IsDirectory);

            return Json(content.ToArray());
        }

        public async Task<JsonResult> GetMediaItems(string path, [FromServices] YesSql.Core.Services.ISession session)
        {
            var media = await session
                .QueryIndexAsync<MediaPartIndex>(x => x.Folder == path.ToLowerInvariant())
                .OrderBy(x => x.FileName)
                .List();

            return Json(media.ToArray());
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> Upload(
            string folderPath, 
            string contentType, 
            ICollection<IFormFile> files, 
            [FromServices] IMediaService mediaService,
            [FromServices] IAuthorizationService authorizationService,
            [FromServices] IContentManager contentManager)
        {
            if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageOwnMedia))
            {
                return Unauthorized();
            }

            var result = new List<object>();

            // TODO: Validate file extensions

            // Loop through each file in the request
            foreach(var file in files)
            {
                // TODO: support clipboard

                try
                {
                    var mediaFilePath = _mediaFileStore.Combine(folderPath, file.FileName);

                    using (var stream = file.OpenReadStream())
                    {
                        if (!await _mediaFileStore.TrySaveStreamAsync(mediaFilePath, stream))
                        {
                            result.Add(new
                            {
                                name = file.FileName,
                                size = file.Length,
                                error = "Couldn't copy the file in the media store"
                            });
                        }
                    }

                    var media = await mediaService.ImportMediaAsync(mediaFilePath, file.ContentType, contentType);
                    contentManager.Create(media.ContentItem);

                    var mediaFile = await _mediaFileStore.GetFileAsync(mediaFilePath);

                    result.Add(new
                    {
                        name = mediaFile.Name,
                        size = mediaFile.Length,
                        url = mediaFile.AbsolutePath,
                        id = media.ContentItem.ContentItemId,
                        model = new MediaPartIndex
                        {
                            FileName = mediaFile.Name,
                            Folder = mediaFile.Path,
                            Length = mediaFile.Length,
                            MimeType = file.ContentType
                        }
                    });
                }
                catch (Exception ex)
                {
                    result.Add(new
                    {
                        name = file.FileName,
                        size = file.Length,
                        error = ex.Message
                    });
                }
            }

            return Json(new { files = result.ToArray() });
        }
    }
}
