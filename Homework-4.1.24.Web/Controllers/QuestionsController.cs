using Homework_2._1._24.Data;
using Homework_4._1._24.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;

namespace Homework_4._1._24.Data.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly string _connectionString;

        public QuestionsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }

        public IActionResult Index()
        {
            QuestionRepository repo = new(_connectionString);

            return View(new QuestionViewModel()
            {
                Questions = repo.GetQuestions(),
                Answers = repo.GetAnswers()
            });
        }

        public IActionResult ViewByTag(string name)
        {
            QuestionRepository repo = new(_connectionString);

            return View(new QuestionViewModel()
            {
                Questions = repo.GetQuestionsForTag(name),
                TagName = name
            });
        }

        public IActionResult DidLikeQuestion(int id)
        {
            var questionsLiked = HttpContext.Session.Get<List<int>>("likedQuestionIds");
            return Json(new
            {
                notPreviouslyLiked = questionsLiked == null || !questionsLiked.Contains(id)
            });
        }

        [HttpPost]
        public void LikeQuestion(int id)
        {
            QuestionRepository repo = new(_connectionString);
            repo.AddLike(id);

            List<int> likedQuestionIds = HttpContext.Session.Get<List<int>>("likedQuestionIds") ?? new();
            likedQuestionIds.Add(id);

            HttpContext.Session.Set("likedQuestionIds", likedQuestionIds);
        }

        [Authorize]
        public IActionResult AskAQuestion()
        {
            return View();
        }

        public IActionResult ViewQuestion(int questionId)
        {
            QuestionRepository repo = new(_connectionString);
            var question = repo.GetQuestionById(questionId);

            return View(new QuestionViewModel()
            {
                Question = question,
                AskedBy = repo.GetUserName(question.UserId),
                Answers = repo.GetAnswersForQuestion(question.Id)
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddAnswer(int questionId, string text)
        {
            QuestionRepository repo = new(_connectionString);

            repo.AddAnswer(new Answer()
            {
                PostedDate = DateTime.Now,
                QuestionId = questionId,
                Text = text,
                UserId = repo.GetByEmail(User.Identity.Name).Id
            });

            return Redirect("/");
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(Question question, List<string> tags)
        {
            QuestionRepository repo = new(_connectionString);
            question.UserId = repo.GetByEmail(User.Identity.Name).Id;

            repo.AddQuestion(question, tags);
            return Redirect("/");
        }

    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}