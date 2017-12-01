namespace NhInterMySQL.Model
{
    public class Task
    {
        public virtual int task_id { get; set; }
        public virtual string title { get; set; }
        public virtual string content { get; set; }
        public virtual string reward { get; set; }
        public virtual int target { get; set; }
        public virtual int exp { get; set; }
    }
}
