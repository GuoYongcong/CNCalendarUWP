using System;

namespace CNCalendarUWP
{
    public class Schedule 
    {
        public DateTime Date { get; set; }//日期
        public string Title { get; set; }//标题
        public string Body { get; set; }//内容
        public Schedule()
        {
            //Date = new DateTime();
        }
        public Schedule(DateTime date, string title, string body)
        {
            //Date = new DateTime();
            Date = date;
            Title = title;
            Body = body;
        }

        public int Compare(Schedule x, Schedule y)
        {
            if ((x.Date.Year == y.Date.Year)
                && (x.Date.Month == y.Date.Month)
                && (x.Date.Day == y.Date.Day))
            {
                return 0;
            }
            else
            {
                if (x.Date > y.Date)
                { return 1; }
                return -1;
            }
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string title = "";
            foreach (char ch in Title)
            {
                title += ch;
            }
            string body = "";
            foreach (char c in Body)
            {
                body += c;
            }

            return (Date.ToString() + title + body);
            throw new NotImplementedException();
        }
    }
}
