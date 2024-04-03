using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace Homework_2._1._24.Data
{
    public class QuestionRepository
    {
        private readonly string _connectionString;
        public QuestionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Question GetQuestionById(int id)
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            return ctx.Questions.Where(q => q.Id == id).Include(t => t.QuestionsTags).ThenInclude(t => t.Tag).FirstOrDefault();
        }

        public void AddAnswer(Answer answer)
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            ctx.Answers.Add(answer);
            ctx.SaveChanges();
        }

        public List<Question> GetQuestions()
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            return ctx.Questions.Include(t => t.QuestionsTags).ThenInclude(t => t.Tag).ToList();
        }

        public List<Answer> GetAnswersForQuestion(int questionId)
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            return ctx.Answers.Where(a => a.QuestionId == questionId).ToList();
        }

        private int AddTag(string name)
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            var tag = new Tag { Name = name };
            ctx.Tags.Add(tag);
            ctx.SaveChanges();
            return tag.Id;
        }

        public void AddQuestion(Question question, List<string> tags)
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            ctx.Questions.Add(question);
            ctx.SaveChanges();

            foreach (string tag in tags)
            {
                Tag t = ctx.Tags.FirstOrDefault(t => t.Name == tag);

                ctx.QuestionsTags.Add(new QuestionsTags
                {
                    QuestionId = question.Id,
                    TagId = t == null ? AddTag(tag) : t.Id
                });
            }
            ctx.SaveChanges();
        }

        public void AddUser(User user, string password)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            using var ctx = new QuestionsDataContext(_connectionString);
            ctx.Users.Add(user);
            ctx.SaveChanges();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return isValidPassword ? user : null;
        }


        public List<Question> GetQuestionsForTag(string name)
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            return ctx.Questions
                    .Where(c => c.QuestionsTags.Any(t => t.Tag.Name == name))
                    .Include(q => q.QuestionsTags)
                    .ThenInclude(qt => qt.Tag)
                    .ToList();
        }

        public User GetByEmail(string email)
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            return ctx.Users.FirstOrDefault(u => u.Email == email);
        }

        public string GetUserName(int userId)
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            return ctx.Users.Where(u => u.Id == userId).Select(u => u.Name).FirstOrDefault();
        }

        public List<Answer> GetAnswers()
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            return ctx.Answers.ToList();
        }

        public void AddLike(int questionId)
        {
            using var ctx = new QuestionsDataContext(_connectionString);
            ctx.Questions.FirstOrDefault(q => q.Id == questionId).Likes++;
            ctx.SaveChanges();
        }
    }
}
