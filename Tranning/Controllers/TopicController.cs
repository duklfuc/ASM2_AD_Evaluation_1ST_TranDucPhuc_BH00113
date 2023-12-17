using Microsoft.AspNetCore.Mvc;
using Tranning.DataDBContext;
using Tranning.Models;

namespace Tranning.Controllers
{
    public class TopicController : Controller
    {
        private readonly TranningDBContext _dbContext;
        public TopicController(TranningDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<TopicMapping> model = new List<TopicMapping>();
            var data = _dbContext.Topics.Join(_dbContext.Courses,
                t => t.course_id,
                c => c.id,
                (t, c) => new TopicMapping
                {
                    course_id = t.course_id,
                    id = t.id,
                    name = t.name,
                    description = t.description,
                    videos = t.videos,
                    status = t.status,
                    attach_file = t.attach_file,
                    documents = t.documents,
                    created_at = t.created_at,
                    updated_at = t.updated_at,
                    CourseName = c.name
                }).ToList();
            model = data;
            return View(model);
        }
        [HttpGet]
        public IActionResult Add()
        {
            TopicDetail topic = new TopicDetail();
            ViewBag.Course = _dbContext.Courses.ToList();
            return View(topic);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)]
        public IActionResult Add(TopicDetail topic)
        {
            ViewBag.Course = _dbContext.Courses.ToList();
            try
            {
                string imagePhoto = UploadFile(topic.fileImage);
                string fileVideos = UploadVideos(topic.fileVideos);
                string fileDocument = string.Empty;
                if (topic.fileDocument != null)
                {
                    fileDocument = UploadFile(topic.fileDocument);
                }
                var topicData = new Topic()
                {
                    course_id = topic.course_id,
                    name = topic.name,
                    description = topic.description,
                    videos = fileVideos,
                    status = topic.status,
                    documents = fileDocument,
                    attach_file = imagePhoto,
                    created_at = DateTime.Now
                };
                _dbContext.Topics.Add(topicData);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra.Vui lòng thử lại";
                return View(topic);
            }
        }
        private string UploadFile(IFormFile file)
        {
            string filePath = string.Empty;
            try
            {
                if (file != null)
                {
                    string pathUploadImage = "wwwroot\\uploads\\images";
                    string fileName = file.FileName;
                    fileName = Path.GetFileName(fileName);
                    string uniqueStr = Guid.NewGuid().ToString();
                    fileName = uniqueStr + "-" + fileName;
                    if (!Directory.Exists(pathUploadImage))
                    {
                        Directory.CreateDirectory(pathUploadImage);
                    }
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), pathUploadImage, fileName);
                    var stream = new FileStream(uploadPath, FileMode.Create);
                    file.CopyToAsync(stream);
                    filePath = fileName;
                }
            }
            catch (Exception ex)
            {

            }
            return filePath;
        }
        //[RequestSizeLimit(104857600)]
        private string UploadVideos(IFormFile file)
        {
            string filePath = string.Empty;
            try
            {
                if (file != null)
                {
                    string pathUploadVideos = "wwwroot\\uploads\\videos";
                    string fileName = file.FileName;
                    fileName = Path.GetFileName(fileName);
                    string uniqueStr = Guid.NewGuid().ToString();
                    fileName = uniqueStr + "-" + fileName;
                    if (!Directory.Exists(pathUploadVideos))
                    {
                        Directory.CreateDirectory(pathUploadVideos);
                    }
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), pathUploadVideos, fileName);
                    var stream = new FileStream(uploadPath, FileMode.Create);
                    file.CopyToAsync(stream);
                    filePath = fileName;
                }
            }
            catch (Exception ex)
            {

            }
            return filePath;
        }
        [HttpGet]
        public IActionResult Update(long id)
        {
            ViewBag.Course = _dbContext.Courses.ToList();
            TopicDetail topic = new TopicDetail();
            var data = _dbContext.Topics.Find(id);
            if (data != null)
            {
                topic.id = data.id;
                topic.course_id = data.course_id;
                topic.name = data.name;
                topic.description = data.description;
                topic.videos = data.videos;
                topic.status = data.status;
                topic.documents = data.documents;
                topic.attach_file = data.attach_file;
            }
            return View(topic);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)]
        public IActionResult Update(TopicDetail topic)
        {
            ViewBag.Course = _dbContext.Courses.ToList();
            try
            {
                var data = _dbContext.Topics.Find(topic.id);
                if (data != null)
                {
                    string imagePhoto = UploadFile(topic.fileImage);
                    string fileVideos = UploadVideos(topic.fileVideos);
                    string fileDocument = string.Empty;
                    if (topic.fileDocument != null)
                    {
                        fileDocument = UploadFile(topic.fileDocument);
                    }
                    data.course_id = topic.course_id;
                    data.name = topic.name;
                    data.updated_at = DateTime.Now;
                    data.description = topic.description;
                    data.videos = (!string.IsNullOrEmpty(fileVideos) ? fileVideos : data.videos);
                    data.status = topic.status;
                    data.documents = (!string.IsNullOrEmpty(fileDocument) ? fileDocument : data.documents);
                    data.attach_file = (!string.IsNullOrEmpty(imagePhoto) ? imagePhoto : data.attach_file);
                    data.created_at = DateTime.Now;
                    _dbContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Cập nhật thông tin không thành công";
                    return View(topic);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra.Vui lòng thử lại";
                return View(topic);
            }
        }
        [HttpGet]
        public IActionResult Delete(long id)
        {
            try
            {
                var data = _dbContext.Topics.Find(id);
                if (data != null)
                {
                    // Use Remove method to delete the entity
                    _dbContext.Topics.Remove(data);
                    _dbContext.SaveChanges();
                    TempData["DeleteTopic"] = true;
                }
                else
                {
                    TempData["DeleteTopic"] = false;
                }
            }
            catch (Exception ex)
            {
                TempData["DeleteTopic"] = false;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}