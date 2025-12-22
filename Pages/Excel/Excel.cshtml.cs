using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Excel
{
    [Authorize(Roles = "Moderator")]
    public class ExcelModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ExcelModel(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [TempData]
        public string? Message { get; set; }

        public IActionResult OnPostExport()
        {
            using var workbook = new XLWorkbook();

            // --- Authors sheet ---
            var authors = _db.Authors.ToList();
            var wsAuthors = workbook.Worksheets.Add("Authors");
            wsAuthors.Cell(1, 1).Value = "Id";
            wsAuthors.Cell(1, 2).Value = "Name";
            wsAuthors.Cell(1, 3).Value = "Bio";

            int row = 2;
            foreach (var a in authors)
            {
                wsAuthors.Cell(row, 1).Value = a.Id;
                wsAuthors.Cell(row, 2).Value = a.Name;
                wsAuthors.Cell(row, 3).Value = a.Bio;
                row++;
            }

            // --- Collections sheet ---
            var collections = _db.Collections.ToList();
            var wsCollections = workbook.Worksheets.Add("Collections");
            wsCollections.Cell(1, 1).Value = "Id";
            wsCollections.Cell(1, 2).Value = "Title";
            wsCollections.Cell(1, 3).Value = "Description";
            row = 2;
            foreach (var c in collections)
            {
                wsCollections.Cell(row, 1).Value = c.Id;
                wsCollections.Cell(row, 2).Value = c.Title;
                wsCollections.Cell(row, 3).Value = c.Description;
                row++;
            }

            // --- Images sheet ---
            var images = _db.Images.ToList();
            var wsImages = workbook.Worksheets.Add("Images");
            wsImages.Cell(1, 1).Value = "Id";
            wsImages.Cell(1, 2).Value = "Title";
            wsImages.Cell(1, 3).Value = "FileName";
            wsImages.Cell(1, 4).Value = "PaintingDate";
            wsImages.Cell(1, 5).Value = "Description";
            wsImages.Cell(1, 6).Value = "AuthorId";
            wsImages.Cell(1, 7).Value = "CollectionId";
            wsImages.Cell(1, 8).Value = "CreatedAt";
            row = 2;
            foreach (var i in images)
            {
                wsImages.Cell(row, 1).Value = i.Id;
                wsImages.Cell(row, 2).Value = i.Title;
                wsImages.Cell(row, 3).Value = i.FileName;
                wsImages.Cell(row, 4).Value = i.PaintingDate?.ToString("yyyy-MM-dd");
                wsImages.Cell(row, 5).Value = i.Description;
                wsImages.Cell(row, 6).Value = i.AuthorId;
                wsImages.Cell(row, 7).Value = i.CollectionId;
                wsImages.Cell(row, 8).Value = i.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            // --- Tags sheet ---
            var tags = _db.Tags.ToList();
            var wsTags = workbook.Worksheets.Add("Tags");
            wsTags.Cell(1, 1).Value = "Id";
            wsTags.Cell(1, 2).Value = "Name";
            wsTags.Cell(1, 3).Value = "ImageId";
            row = 2;
            foreach (var t in tags)
            {
                wsTags.Cell(row, 1).Value = t.Id;
                wsTags.Cell(row, 2).Value = t.Name;
                wsTags.Cell(row, 3).Value = t.ImageId;
                row++;
            }

            // --- Likes sheet ---
            var likes = _db.Likes.ToList();
            var wsLikes = workbook.Worksheets.Add("Likes");
            wsLikes.Cell(1, 1).Value = "Id";
            wsLikes.Cell(1, 2).Value = "ImageId";
            wsLikes.Cell(1, 3).Value = "UserId";
            wsLikes.Cell(1, 4).Value = "CreatedAt";
            row = 2;
            foreach (var l in likes)
            {
                wsLikes.Cell(row, 1).Value = l.Id;
                wsLikes.Cell(row, 2).Value = l.ImageId;
                wsLikes.Cell(row, 3).Value = l.UserId;
                wsLikes.Cell(row, 4).Value = l.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "OnlineGallery.xlsx");
        }

        public async Task<IActionResult> OnPostImportAsync(IFormFile ExcelFile)
        {
            if (ExcelFile == null || ExcelFile.Length == 0)
            {
                Message = "Файл не выбран";
                return Page();
            }

            using var stream = ExcelFile.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            // --- Authors ---
            var wsAuthors = workbook.Worksheet("Authors");
            var authors = new List<Author>();
            foreach (var row in wsAuthors.RowsUsed().Skip(1))
            {
                authors.Add(new Author
                {
                    Id = row.Cell(1).GetValue<int>(),
                    Name = row.Cell(2).GetValue<string>(),
                    Bio = row.Cell(3).GetValue<string?>()
                });
            }
            _db.Authors.RemoveRange(_db.Authors);
            await _db.Authors.AddRangeAsync(authors);

            // --- Collections ---
            var wsCollections = workbook.Worksheet("Collections");
            var collections = new List<Collection>();
            foreach (var row in wsCollections.RowsUsed().Skip(1))
            {
                collections.Add(new Collection
                {
                    Id = row.Cell(1).GetValue<int>(),
                    Title = row.Cell(2).GetValue<string>(),
                    Description = row.Cell(3).GetValue<string?>()
                });
            }
            _db.Collections.RemoveRange(_db.Collections);
            await _db.Collections.AddRangeAsync(collections);

            // --- Images ---
            var wsImages = workbook.Worksheet("Images");
            var images = new List<ImageItem>();
            foreach (var row in wsImages.RowsUsed().Skip(1))
            {
                images.Add(new ImageItem
                {
                    Id = row.Cell(1).GetValue<int>(),
                    Title = row.Cell(2).GetValue<string>(),
                    FileName = row.Cell(3).GetValue<string>(),
                    PaintingDate = row.Cell(4).GetDateTime(),
                    Description = row.Cell(5).GetValue<string?>(),
                    AuthorId = row.Cell(6).GetValue<int?>(),
                    CollectionId = row.Cell(7).GetValue<int?>(),
                    CreatedAt = row.Cell(8).GetDateTime()
                });
            }
            _db.Images.RemoveRange(_db.Images);
            await _db.Images.AddRangeAsync(images);

            // --- Tags ---
            var wsTags = workbook.Worksheet("Tags");
            var tags = new List<ImageTag>();
            foreach (var row in wsTags.RowsUsed().Skip(1))
            {
                tags.Add(new ImageTag
                {
                    Id = row.Cell(1).GetValue<int>(),
                    Name = row.Cell(2).GetValue<string>(),
                    ImageId = row.Cell(3).GetValue<int>()
                });
            }
            _db.Tags.RemoveRange(_db.Tags);
            await _db.Tags.AddRangeAsync(tags);

            // --- Likes ---
            var wsLikes = workbook.Worksheet("Likes");
            var likes = new List<Like>();
            foreach (var row in wsLikes.RowsUsed().Skip(1))
            {
                likes.Add(new Like
                {
                    Id = row.Cell(1).GetValue<int>(),
                    ImageId = row.Cell(2).GetValue<int>(),
                    UserId = row.Cell(3).GetValue<string>(),
                    CreatedAt = row.Cell(4).GetDateTime()
                });
            }
            _db.Likes.RemoveRange(_db.Likes);
            await _db.Likes.AddRangeAsync(likes);

            await _db.SaveChangesAsync();
            Message = "Импорт успешно выполнен!";
            return Page();
        }
    }
}
