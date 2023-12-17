using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tranning.DataDBContext
{
    [Table("trainner_topic")]
    public class Trainner_topic
    {
        [Key]
        public long trainner_id { get; set; }
        public long topic_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public long userid { get; set; }
    }
}