using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class DocumentController : Controller
    {
        private readonly AppDBContext dbContext;
        public DocumentController()
        {
            dbContext = new AppDBContext();
        }
        // GET: Document/id
        public async Task<ActionResult> Download(int id)
        {
            Document document = dbContext.Documents.FirstOrDefault(row => row.ID == id);
            byte[] fileBytes = System.IO.File.ReadAllBytes(document.FilePath);
            String fileName = String.Format("{0}.{1}", document.FileName, document.FileExtension);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}