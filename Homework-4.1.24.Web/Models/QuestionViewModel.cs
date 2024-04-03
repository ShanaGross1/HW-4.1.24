using Homework_2._1._24.Data;

namespace Homework_4._1._24.Data.Models
{
    public class QuestionViewModel
    {
        public List<Question> Questions { get; set; }
        public Question Question { get; set; }
        public List<Answer> Answers { get; set; }
        public string AskedBy { get; set; }
        public string TagName { get; set; }
    }
}
