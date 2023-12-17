using Microsoft.AspNetCore.Mvc;
using Tranning.DataDBContext;
using Tranning.Models;

namespace Tranning.Controllers
{
    public class TrainnertopicController : Controller
    {
        private readonly TranningDBContext _dbContext;
        public TrainnertopicController(TranningDBContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            List<TrainnertopicMapping> model = new List<TrainnertopicMapping>();
            var data = _dbContext.trainner_Topics.Join(_dbContext.Topics,
                tt => tt.topic_id,
                t => t.id,
                (tt, t) => new TrainnertopicMapping
                {
                    trainner_id = tt.trainner_id,
                    topic_id = tt.topic_id,
                    created_at = tt.created_at,
                    updated_at = tt.updated_at,
                    TopicName = t.name,
                    userid = tt.userid
                }).ToList();
            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    var user = _dbContext.Users.Find(item.userid);
                    if (user != null)
                    {
                        model.Add(new TrainnertopicMapping
                        {
                            trainner_id = item.trainner_id,
                            topic_id = item.topic_id,
                            created_at = item.created_at,
                            updated_at = item.updated_at,
                            TopicName = item.TopicName,
                            userid = item.userid,
                            FullName = user.full_name
                        });
                    }
                    else
                    {
                        model.Add(item);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult CreateOrEdit(long id)
        {
            ViewBag.Trainner = _dbContext.Users.Where(x => x.role_id == 1).ToList();
            Trainner_topic model = new Trainner_topic();
            ViewBag.Topic = _dbContext.Topics.ToList();
            var data = _dbContext.trainner_Topics.Find(id);
            if (data != null)
            {
                model = data;
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult CreateOrEdit(Trainner_topic model)
        {
            ViewBag.Trainner = _dbContext.Users.Where(x => x.role_id == 1).ToList();
            ViewBag.Topic = _dbContext.Topics.ToList();
            try
            {
                var ckTrainner = _dbContext.trainner_Topics.Where(x => x.userid == model.userid && x.trainner_id != model.trainner_id).FirstOrDefault();
                if (ckTrainner != null)
                {
                    ViewBag.Error = "Thông tin tài khoản đã tồn tại trong trainner topic";
                    return View(model);
                }
                if (model.trainner_id == 0)
                {
                    model.created_at = DateTime.Now;
                    _dbContext.trainner_Topics.Add(model);
                }
                else
                {
                    var enity = _dbContext.trainner_Topics.Find(model.trainner_id);
                    if (enity != null)
                    {
                        enity.topic_id = model.topic_id;
                        enity.updated_at = DateTime.Now;
                        enity.userid = model.userid;
                    }
                    else
                    {
                        ViewBag.Error = "Cập nhật không thành công";
                        return View(model);
                    }
                }
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewBag.Error = "Có lỗi xảy ra. Vui lòng thử lại";
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Delete(long id)
        {
            try
            {
                var data = _dbContext.trainner_Topics.Find(id);
                if (data != null)
                {
                    // Use Remove method to delete the entity
                    _dbContext.trainner_Topics.Remove(data);
                    _dbContext.SaveChanges();
                    TempData["Deletetrainner"] = true;
                }
                else
                {
                    TempData["Deletetrainner"] = false;
                }
            }
            catch (Exception ex)
            {
                TempData["Deletetrainner"] = false;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}