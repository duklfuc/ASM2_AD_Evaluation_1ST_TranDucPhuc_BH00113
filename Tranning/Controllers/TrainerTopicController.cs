using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tranning.DataDBContext;
using Tranning.Models;

namespace Tranning.Controllers
{
    public class TrainerTopicController : Controller
    {
        private readonly TranningDBContext _dbContext;
        public TrainerTopicController(TranningDBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index(string SearchString)
        {
            TrainerTopicModel trainertopicModel = new TrainerTopicModel();
            trainertopicModel.TrainerTopicDetailLists = new List<TrainerTopicDetail>();

            var data = _dbContext.TrainerTopics
                .Where(m => m.deleted_at == null)
                .Join(
                    _dbContext.Users,
                    trainerTopic => trainerTopic.trainer_id,
                    trainer => trainer.id,
                    (trainerTopic, trainer) => new
                    {
                        TrainerTopic = trainerTopic,
                        TrainerName = trainer.full_name
                    })
                .Join(
                    _dbContext.Topics,
                    result => result.TrainerTopic.topic_id,
                    topic => topic.id,
                    (result, topic) => new
                    {
                        result.TrainerTopic,
                        result.TrainerName,
                        TopicName = topic.name
                    })
                .ToList();

            foreach (var item in data)
            {
                trainertopicModel.TrainerTopicDetailLists.Add(new TrainerTopicDetail
                {
                    topic_id = item.TrainerTopic.topic_id,
                    trainer_id = item.TrainerTopic.trainer_id,
                    trainerName = item.TrainerName,
                    topicName = item.TopicName,
                    created_at = item.TrainerTopic.created_at,
                    updated_at = item.TrainerTopic.updated_at
                });
            }
            ViewData["CurrentFilter"] = SearchString ?? "";

            return View(trainertopicModel);
        }


        [HttpGet]
        public IActionResult Add()
        {
            TrainerTopicDetail trainertopic = new TrainerTopicDetail();
            var topicList = _dbContext.Topics
              .Where(m => m.deleted_at == null)
              .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name }).ToList();
            ViewBag.Stores = topicList;

            var trainerList = _dbContext.Users
              .Where(m => m.deleted_at == null && m.role_id == 3)
              .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.full_name }).ToList();
            ViewBag.Stores1 = trainerList;

            return View(trainertopic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TrainerTopicDetail trainertopic)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var trainertopicData = new TrainerTopic()
                    {
                        topic_id = trainertopic.topic_id,
                        trainer_id = trainertopic.trainer_id,
                        created_at = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    };

                    _dbContext.TrainerTopics.Add(trainertopicData);
                    _dbContext.SaveChanges(true);
                    TempData["saveStatus"] = true;
                }

                catch (Exception ex)
                {

                    TempData["saveStatus"] = false;
                }
                return RedirectToAction(nameof(TrainerTopicController.Index), "TrainerTopic");
            }


            var courseList = _dbContext.Courses
              .Where(m => m.deleted_at == null)
              .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name }).ToList();
            ViewBag.Stores = courseList;

            var traineeList = _dbContext.Users
              .Where(m => m.deleted_at == null && m.role_id == 3)
              .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.full_name }).ToList();
            ViewBag.Stores1 = traineeList;


            Console.WriteLine(ModelState.IsValid);
            foreach (var key in ModelState.Keys)
            {
                var error = ModelState[key].Errors.FirstOrDefault();
                if (error != null)
                {
                    Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
                }
            }
            return View(trainertopic);
        }

        [HttpGet]
        public IActionResult Delete(int trainerId)
        {
            var trainerTopic = _dbContext.TrainerTopics
                .Where(tt => tt.trainer_id == trainerId && tt.deleted_at == null)
                .FirstOrDefault();

            if (trainerTopic == null)
            {
                // Handle the case where the record is not found
                return NotFound();
            }

            // Soft delete by setting the deleted_at field
            trainerTopic.deleted_at = DateTime.Now;

            _dbContext.SaveChanges();

            // Redirect to the Index action to refresh the list
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, TrainerTopicDetail trainertopic)
        {
            if (id != trainertopic.topic_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTrainerTopic = _dbContext.TrainerTopics
                        .FirstOrDefault(tt => tt.topic_id == id && tt.deleted_at == null);

                    if (existingTrainerTopic == null)
                    {
                        return NotFound();
                    }

                    existingTrainerTopic.trainer_id = trainertopic.trainer_id;
                    existingTrainerTopic.updated_at = DateTime.Now;

                    _dbContext.Update(existingTrainerTopic);
                    await _dbContext.SaveChangesAsync();

                    TempData["updateStatus"] = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in Update action: {ex}");
                    TempData["updateStatus"] = false;
                }
                return RedirectToAction("Index", "Home"); // Change to a known working action and controller
            }

            // If ModelState is not valid, re-populate dropdown lists and return to the view
            var topicList = _dbContext.Topics
                .Where(m => m.deleted_at == null)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name }).ToList();
            ViewBag.Stores = topicList;

            var trainerList = _dbContext.Users
                .Where(m => m.deleted_at == null && m.role_id == 3)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.full_name }).ToList();
            ViewBag.Stores1 = trainerList;

            Console.WriteLine("Model state is not valid. Validation errors:");
            foreach (var key in ModelState.Keys)
            {
                var error = ModelState[key].Errors.FirstOrDefault();
                if (error != null)
                {
                    Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
                }
            }

            return View(trainertopic);
        }

    }
}