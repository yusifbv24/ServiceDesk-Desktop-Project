namespace ServiceDesk.Class
{
    public class TableDependencies
    {
        public class TicketTable
        {
            public string code { get; set; } = null;
            public string dep_name { get; set; } = null;
            public string worker { get; set; } = null;
            public string device { get; set; } = null;
            public string task { get; set; } = null;
            public string solution { get; set; } = null;
            public string fullname { get; set; }
            public string finished_time { get; set; } = null;
        }
        public class UserTable
        {
            public string session { get; set; } = null;
            public string ip_address { get; set; } = null;
            public float csat { get; set; } = default;
        }
        public class UserSessionTable
        {
            public float isActive {  get; set; } 
        }
        public class RatingTable
        {
            public float rating { set; get; } = default;
            public string message { set; get; } = null;
        }
        public class StatusTable
        {
            public string status { set; get; }
        }
        public class TaskTable
        {
            public string task { get; set; } = null;
        }
    }
}
